using System;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004A3 RID: 1187
public class BreakableHolder : DebugDrawColliderRuntimeAdder, IHitResponder, IBreakerBreakable
{
	// Token: 0x06002B15 RID: 11029 RVA: 0x000BC3E0 File Offset: 0x000BA5E0
	private void OnEnable()
	{
		this.ResetHits();
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x000BC3E8 File Offset: 0x000BA5E8
	private void Start()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out int value)
			{
				value = this.hitsLeft;
				if (this.forwardToBreakable)
				{
					this.forwardToBreakable.SetHitsToBreak(this.hitsLeft);
				}
			};
			this.persistent.OnSetSaveState += delegate(int value)
			{
				this.hitsLeft = value;
				if (this.hitsLeft <= 0)
				{
					this.SetBroken();
					if (this.forwardToBreakable)
					{
						this.forwardToBreakable.SetAlreadyBroken();
					}
				}
			};
		}
		if (this.forwardToBreakable)
		{
			this.forwardToBreakable.SetHitsToBreak(this.hitsLeft);
			float num = this.forwardToBreakable.GetHitCoolDown();
			if (num > 0f)
			{
				this.hitCooldown = ((this.hitCooldown > 0f) ? Mathf.Min(this.hitCooldown, num) : num);
				num = Mathf.Min(this.hitCooldown, num);
			}
			this.forwardToBreakable.SetHitCoolDownDuration(num);
		}
		if (this.breakPrefab != null)
		{
			Transform transform = base.transform;
			this.breakEffects = Object.Instantiate<GameObject>(this.breakPrefab, transform.position, transform.rotation);
			this.breakEffects.SetActive(false);
		}
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x000BC4DB File Offset: 0x000BA6DB
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		return this.DoHit(damageInstance.AttackType, damageInstance.Direction, damageInstance.Source) ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x000BC500 File Offset: 0x000BA700
	private bool DoHit(AttackTypes attackType, float direction, GameObject source)
	{
		if (this.hitsLeft <= 0)
		{
			return false;
		}
		bool flag;
		if (attackType == AttackTypes.Heavy)
		{
			this.hitsLeft = 0;
			flag = true;
		}
		else
		{
			if (this.lastHitTime > Time.timeAsDouble)
			{
				return false;
			}
			this.lastHitTime = Time.timeAsDouble + (double)this.hitCooldown;
			this.hitsLeft--;
			flag = (this.hitsLeft <= 0);
		}
		this.DoHitWithPayout(flag, direction, source.transform.position.x > base.transform.position.x);
		if (!flag)
		{
			return true;
		}
		this.SetBroken();
		if (this.forwardToBreakable)
		{
			this.forwardToBreakable.BreakSelf();
		}
		return true;
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x000BC5B3 File Offset: 0x000BA7B3
	private void DoHitWithPayout(bool doBreak, float direction, bool isFromRight)
	{
		if (this.strikePrefab)
		{
			this.strikePrefab.Spawn(base.transform.position);
		}
		this.FlingHolding(this.payoutPerHit, isFromRight);
		this.DoHit(doBreak, direction, isFromRight);
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x000BC5F0 File Offset: 0x000BA7F0
	private void DoHit(bool doBreak, float direction, bool isFromRight)
	{
		if (doBreak)
		{
			this.breakCameraShake.DoShake(this, true);
			this.breakSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			this.FlingHolding(this.finalPayout, isFromRight);
			if (this.breakEffects)
			{
				this.breakEffects.transform.position = base.transform.position;
				this.breakEffects.SetActive(true);
			}
			this.Break.Invoke();
			foreach (BreakableHolder.ObjectFling objectFling in this.debrisParts)
			{
				if (objectFling.Object)
				{
					objectFling.Object.SetActive(true);
					MinMaxFloat minMaxFloat = isFromRight ? objectFling.RightAngleRange : objectFling.LeftAngleRange;
					FlingUtils.FlingObject(new FlingUtils.SelfConfig
					{
						Object = objectFling.Object,
						SpeedMin = objectFling.FlingSpeedRange.Start,
						SpeedMax = objectFling.FlingSpeedRange.End,
						AngleMin = minMaxFloat.Start,
						AngleMax = minMaxFloat.End
					}, base.transform, Vector3.zero);
				}
			}
			return;
		}
		this.hitCameraShake.DoShake(this, true);
		if (this.hitSoundTable)
		{
			this.hitSoundTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		else
		{
			this.hitSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		}
		BreakableHolder.HitDirection hitDirection = BreakableHolder.HitDirection.Down;
		if (direction < 45f)
		{
			hitDirection = BreakableHolder.HitDirection.Right;
		}
		else if (direction < 135f)
		{
			hitDirection = BreakableHolder.HitDirection.Down;
		}
		else if (direction < 225f)
		{
			hitDirection = BreakableHolder.HitDirection.Left;
		}
		switch (hitDirection)
		{
		case BreakableHolder.HitDirection.Left:
			if (!base.transform.eulerAngles.x.IsWithinTolerance(10f, 90f))
			{
				this.DoHitEffects(100f, 140f, new Vector3(180f, 90f, 270f));
			}
			break;
		case BreakableHolder.HitDirection.Right:
			if (!base.transform.eulerAngles.z.IsWithinTolerance(10f, 270f))
			{
				this.DoHitEffects(20f, 40f, new Vector3(0f, 90f, 270f));
			}
			break;
		case BreakableHolder.HitDirection.Down:
			if (!base.transform.eulerAngles.z.IsWithinTolerance(10f, 180f))
			{
				this.DoHitEffects(70f, 110f, new Vector3(-90f, -180f, -180f));
			}
			break;
		}
		this.HitStarted.Invoke();
		Vector3 initialPosition = base.transform.position;
		if (!this.noHitShake)
		{
			this.StartTimerRoutine(0f, 0.2f, delegate(float time)
			{
				Vector3 b = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
				this.transform.position = Vector3.Lerp(initialPosition + b, initialPosition, time);
			}, null, delegate
			{
				this.HitEnded.Invoke();
			}, false);
		}
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x000BC8FC File Offset: 0x000BAAFC
	private void DoHitEffects(float pAngleMin, float pAngleMax, Vector3 dustRotation)
	{
		if (this.hitFlingPrefab)
		{
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.hitFlingPrefab,
				AmountMin = 3,
				AmountMax = 5,
				SpeedMin = 15f,
				SpeedMax = 20f,
				AngleMin = pAngleMin,
				AngleMax = pAngleMax,
				OriginVariationX = 0.25f,
				OriginVariationY = 0.25f
			}, base.transform, Vector3.zero, null, -1f);
		}
		if (this.hitDustPrefab)
		{
			this.hitDustPrefab.Spawn(base.transform.position + new Vector3(0f, 0f, 0.1f), Quaternion.Euler(dustRotation));
		}
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x000BC9D8 File Offset: 0x000BABD8
	private void SetBroken()
	{
		if (this.isBroken)
		{
			return;
		}
		this.isBroken = true;
		this.Broken.Invoke();
		NoiseMaker.CreateNoise(base.transform.position, this.noiseRadius, NoiseMaker.Intensities.Normal, false);
		Collider2D component = base.GetComponent<Collider2D>();
		if (component)
		{
			component.enabled = false;
		}
		if (this.resetHitsOnBreak)
		{
			this.ResetHits();
		}
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x000BCA41 File Offset: 0x000BAC41
	private void ResetHits()
	{
		this.isBroken = false;
		this.hitsLeft = this.totalHits;
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x000BCA58 File Offset: 0x000BAC58
	private void FlingHolding(int amount, bool isDirectionRight)
	{
		GameObject randomGameObjectByProbability = Probability.GetRandomGameObjectByProbability(this.holdingGameObjects);
		if (randomGameObjectByProbability)
		{
			Vector3 lossyScale = base.transform.lossyScale;
			if (lossyScale.x < 0f)
			{
				isDirectionRight = !isDirectionRight;
			}
			if (lossyScale.y < 0f)
			{
				isDirectionRight = !isDirectionRight;
			}
			MinMaxFloat relativeAngleRange = this.GetRelativeAngleRange(isDirectionRight ? this.rightAngleRange : this.leftAngleRange);
			if (Gameplay.IsShellShardPrefab(randomGameObjectByProbability))
			{
				FlingUtils.SpawnAndFlingShellShards(new FlingUtils.Config
				{
					Prefab = randomGameObjectByProbability,
					AmountMin = amount,
					AmountMax = amount,
					SpeedMin = this.flingSpeedRange.Start,
					SpeedMax = this.flingSpeedRange.End,
					AngleMin = relativeAngleRange.Start,
					AngleMax = relativeAngleRange.End,
					OriginVariationX = 0.25f,
					OriginVariationY = 0.25f
				}, base.transform, this.originOffset, null);
				return;
			}
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = randomGameObjectByProbability,
				AmountMin = amount,
				AmountMax = amount,
				SpeedMin = this.flingSpeedRange.Start,
				SpeedMax = this.flingSpeedRange.End,
				AngleMin = relativeAngleRange.Start,
				AngleMax = relativeAngleRange.End,
				OriginVariationX = 0.25f,
				OriginVariationY = 0.25f
			}, base.transform, this.originOffset, null, -1f);
		}
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x000BCBE8 File Offset: 0x000BADE8
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.TransformPoint(this.originOffset);
		HandleHelper.Draw2DAngle(position, this.GetRelativeAngleRange(this.leftAngleRange).Start, this.GetRelativeAngleRange(this.leftAngleRange).End, new float?(1f));
		HandleHelper.Draw2DAngle(position, this.GetRelativeAngleRange(this.rightAngleRange).Start, this.GetRelativeAngleRange(this.rightAngleRange).End, new float?(1f));
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x000BCC6C File Offset: 0x000BAE6C
	private MinMaxFloat GetRelativeAngleRange(MinMaxFloat angleRange)
	{
		float num = this.angleOffset * Mathf.Sign(base.transform.localScale.x);
		float num2 = base.transform.eulerAngles.z + num;
		return new MinMaxFloat(angleRange.Start + num2, angleRange.End + num2);
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x06002B21 RID: 11041 RVA: 0x000BCCBE File Offset: 0x000BAEBE
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			return BreakableBreaker.BreakableTypes.Basic;
		}
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x000BCCC4 File Offset: 0x000BAEC4
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		if (!this.canBreakFromBreaker)
		{
			return;
		}
		if (this.isBroken)
		{
			return;
		}
		bool flag = breaker.transform.position.x > base.transform.position.x;
		float direction = (float)(flag ? -1 : 1);
		if (this.strikePrefab)
		{
			this.strikePrefab.Spawn(base.transform.position);
		}
		while (this.hitsLeft > 0)
		{
			this.hitsLeft--;
			this.FlingHolding(this.payoutPerHit, flag);
		}
		this.DoHit(true, direction, flag);
		this.SetBroken();
		if (this.forwardToBreakable)
		{
			this.forwardToBreakable.BreakSelf();
		}
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x000BCD80 File Offset: 0x000BAF80
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		if (!this.canBreakFromBreaker)
		{
			return;
		}
		float direction = (float)((breaker.transform.position.x > base.transform.position.x) ? 180 : 0);
		this.DoHit(AttackTypes.Generic, direction, breaker.gameObject);
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000BCDD3 File Offset: 0x000BAFD3
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.None, false);
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x000BCE07 File Offset: 0x000BB007
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04002C32 RID: 11314
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x04002C33 RID: 11315
	[Space]
	[SerializeField]
	private int finalPayout;

	// Token: 0x04002C34 RID: 11316
	[SerializeField]
	private int payoutPerHit;

	// Token: 0x04002C35 RID: 11317
	[SerializeField]
	private int totalHits;

	// Token: 0x04002C36 RID: 11318
	private int hitsLeft;

	// Token: 0x04002C37 RID: 11319
	[SerializeField]
	private float hitCooldown = 0.15f;

	// Token: 0x04002C38 RID: 11320
	private double lastHitTime;

	// Token: 0x04002C39 RID: 11321
	private bool isBroken;

	// Token: 0x04002C3A RID: 11322
	[SerializeField]
	private bool resetHitsOnBreak;

	// Token: 0x04002C3B RID: 11323
	[SerializeField]
	private float noiseRadius = 3f;

	// Token: 0x04002C3C RID: 11324
	[SerializeField]
	private Probability.ProbabilityGameObject[] holdingGameObjects;

	// Token: 0x04002C3D RID: 11325
	[SerializeField]
	private BreakableHolder.ObjectFling[] debrisParts;

	// Token: 0x04002C3E RID: 11326
	[SerializeField]
	private Vector3 originOffset;

	// Token: 0x04002C3F RID: 11327
	[SerializeField]
	private MinMaxFloat rightAngleRange;

	// Token: 0x04002C40 RID: 11328
	[SerializeField]
	private MinMaxFloat leftAngleRange;

	// Token: 0x04002C41 RID: 11329
	[SerializeField]
	private float angleOffset;

	// Token: 0x04002C42 RID: 11330
	[SerializeField]
	private MinMaxFloat flingSpeedRange;

	// Token: 0x04002C43 RID: 11331
	[Space]
	[SerializeField]
	private bool canBreakFromBreaker = true;

	// Token: 0x04002C44 RID: 11332
	[SerializeField]
	private Breakable forwardToBreakable;

	// Token: 0x04002C45 RID: 11333
	[Space]
	[SerializeField]
	private GameObject strikePrefab;

	// Token: 0x04002C46 RID: 11334
	[SerializeField]
	private GameObject breakPrefab;

	// Token: 0x04002C47 RID: 11335
	[SerializeField]
	private GameObject hitFlingPrefab;

	// Token: 0x04002C48 RID: 11336
	[SerializeField]
	private GameObject hitDustPrefab;

	// Token: 0x04002C49 RID: 11337
	[SerializeField]
	private CameraShakeTarget hitCameraShake;

	// Token: 0x04002C4A RID: 11338
	[SerializeField]
	private CameraShakeTarget breakCameraShake;

	// Token: 0x04002C4B RID: 11339
	[SerializeField]
	public bool noHitShake;

	// Token: 0x04002C4C RID: 11340
	[SerializeField]
	private AudioSource audioPlayerPrefab;

	// Token: 0x04002C4D RID: 11341
	[SerializeField]
	private AudioEventRandom breakSound;

	// Token: 0x04002C4E RID: 11342
	[SerializeField]
	private AudioEventRandom hitSound;

	// Token: 0x04002C4F RID: 11343
	[SerializeField]
	private RandomAudioClipTable hitSoundTable;

	// Token: 0x04002C50 RID: 11344
	[Space]
	public UnityEvent Break;

	// Token: 0x04002C51 RID: 11345
	public UnityEvent Broken;

	// Token: 0x04002C52 RID: 11346
	public UnityEvent HitStarted;

	// Token: 0x04002C53 RID: 11347
	public UnityEvent HitEnded;

	// Token: 0x04002C54 RID: 11348
	private GameObject breakEffects;

	// Token: 0x020017BB RID: 6075
	private enum HitDirection
	{
		// Token: 0x04008F31 RID: 36657
		Left,
		// Token: 0x04008F32 RID: 36658
		Right,
		// Token: 0x04008F33 RID: 36659
		Down
	}

	// Token: 0x020017BC RID: 6076
	[Serializable]
	private struct ObjectFling
	{
		// Token: 0x04008F34 RID: 36660
		public GameObject Object;

		// Token: 0x04008F35 RID: 36661
		public MinMaxFloat LeftAngleRange;

		// Token: 0x04008F36 RID: 36662
		public MinMaxFloat RightAngleRange;

		// Token: 0x04008F37 RID: 36663
		public MinMaxFloat FlingSpeedRange;
	}
}
