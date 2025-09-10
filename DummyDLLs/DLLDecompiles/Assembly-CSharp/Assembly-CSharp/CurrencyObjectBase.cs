using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public abstract class CurrencyObjectBase : MonoBehaviour, ICurrencyObject, IBurnable, IHitResponder, AntRegion.INotify, IBreakerBreakable
{
	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06002192 RID: 8594
	protected abstract CurrencyType? CurrencyType { get; }

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06002193 RID: 8595 RVA: 0x0009AFC0 File Offset: 0x000991C0
	private static bool IsHeroDead
	{
		get
		{
			int frameCount = Time.frameCount;
			if (CurrencyObjectBase._lastUpdate != frameCount)
			{
				CurrencyObjectBase._lastUpdate = frameCount;
				HeroController instance = HeroController.instance;
				CurrencyObjectBase._isHeroDead = (!instance || instance.cState.dead);
			}
			return CurrencyObjectBase._isHeroDead;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06002194 RID: 8596 RVA: 0x0009B007 File Offset: 0x00099207
	private static bool CanPickupExist
	{
		get
		{
			if (CurrencyObjectBase.lastPickupUpdate != Time.frameCount)
			{
				CurrencyObjectBase.lastPickupUpdate = Time.frameCount;
				CurrencyObjectBase.canPickupExist = GameManager.instance.CanPickupsExist();
			}
			return CurrencyObjectBase.canPickupExist;
		}
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x0009B034 File Offset: 0x00099234
	public static void ProcessHeroDeath()
	{
		CurrencyObjectBase._currencyObjects.ReserveListUsage();
		foreach (CurrencyObjectBase currencyObjectBase in CurrencyObjectBase._currencyObjects.List)
		{
			currencyObjectBase.OnHeroDeath();
		}
		CurrencyObjectBase._currencyObjects.ReleaseListUsage();
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x0009B09C File Offset: 0x0009929C
	protected virtual void Awake()
	{
		if (this.pickupRange)
		{
			this.pickupRange.OnTriggerEntered += this.HandleHeroEnter;
		}
		this.audioSource = base.GetComponent<AudioSource>();
		this.rend = base.GetComponent<Renderer>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.collider = base.GetComponent<Collider2D>();
		this.hasRenderer = this.rend;
		this.hasAcidEffect = this.acidEffect;
		this.hasMagnetEffect = this.magnetEffect;
		this.hasMagnetTool = this.magnetTool;
		this.hasMagnetBuff = this.magnetBuffTool;
		if (this.hasMagnetTool)
		{
			this.magnetToolStatus = this.magnetTool.Status;
		}
		if (this.hasMagnetBuff)
		{
			this.magnetBuffStatus = this.magnetBuffTool.Status;
		}
		this.defaultGravityScale = this.body.gravityScale;
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.activated = value;
				if (this.activated)
				{
					base.gameObject.SetActive(false);
				}
			};
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += this.OnToolEquipsUpdated;
		if (this.pickupVibrationAsset)
		{
			this.pickupVibration = this.pickupVibrationAsset.VibrationData;
		}
		this.visibilityGroup = base.gameObject.AddComponent<VisibilityGroup>();
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x0009B221 File Offset: 0x00099421
	protected virtual void OnDestroy()
	{
		if (this.pickupRange)
		{
			this.pickupRange.OnTriggerEntered -= this.HandleHeroEnter;
		}
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x0009B248 File Offset: 0x00099448
	protected virtual void OnEnable()
	{
		this.activated = false;
		this.body.bodyType = RigidbodyType2D.Dynamic;
		this.body.gravityScale = this.defaultGravityScale;
		this.collider.enabled = true;
		this.collider.isTrigger = false;
		this.isAttracted = false;
		this.attractVelocity = Vector2.zero;
		this.isMoving = false;
		if (this.isBroken)
		{
			if (this.breakEffect)
			{
				this.breakEffect.SetActive(false);
			}
			this.isBroken = false;
		}
		this.SetVisible(true);
		if (this.hasAcidEffect)
		{
			this.acidEffect.gameObject.SetActive(false);
		}
		if (this.hasMagnetEffect)
		{
			this.magnetEffect.SetActive(false);
		}
		this.pickupStartTime = Time.timeAsDouble + 0.3499999940395355;
		this.OnToolEquipsUpdated();
		CurrencyObjectBase._currencyObjects.Add(this);
		if (this.hasStarted)
		{
			this.OnStartOrEnable();
			ComponentSingleton<CurrencyObjectBaseCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
		}
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x0009B351 File Offset: 0x00099551
	protected virtual void Start()
	{
		this.OnStartOrEnable();
		this.hasStarted = true;
		ComponentSingleton<CurrencyObjectBaseCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x0009B376 File Offset: 0x00099576
	protected virtual void OnStartOrEnable()
	{
		if (!CurrencyObjectBase.CanPickupExist)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x0009B38C File Offset: 0x0009958C
	protected virtual void OnDisable()
	{
		ComponentSingleton<CurrencyObjectBaseCallbackHooks>.Instance.OnFixedUpdate -= this.OnFixedUpdate;
		if (this.isBroken)
		{
			if (this.breakEffect)
			{
				this.breakEffect.SetActive(false);
			}
			this.isBroken = false;
		}
		if (this.getterRoutine != null)
		{
			base.StopCoroutine(this.getterRoutine);
			this.getterRoutine = null;
		}
		this.body.bodyType = RigidbodyType2D.Dynamic;
		CurrencyObjectBase._currencyObjects.Remove(this);
		this.isDisabling = false;
		this.landedColliders.Clear();
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x0009B41C File Offset: 0x0009961C
	private void OnHeroDeath()
	{
		this.activated = true;
		this.collider.enabled = true;
		this.collider.isTrigger = false;
		this.isAttracted = false;
		this.isMoving = false;
		this.body.bodyType = RigidbodyType2D.Dynamic;
		this.body.gravityScale = this.defaultGravityScale;
		Vector2 linearVelocity = this.body.linearVelocity;
		linearVelocity.x = 0f;
		if (linearVelocity.y > 0f)
		{
			linearVelocity.y = 0f;
		}
		this.body.linearVelocity = linearVelocity;
		if (this.hasMagnetEffect)
		{
			this.magnetEffect.SetActive(false);
		}
		if (this.getterRoutine != null)
		{
			base.StopCoroutine(this.getterRoutine);
			this.getterRoutine = null;
		}
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x0009B4E0 File Offset: 0x000996E0
	private void OnToolEquipsUpdated()
	{
		if (!base.isActiveAndEnabled || this.activated || this.isDisabling)
		{
			return;
		}
		if (this.MagnetToolIsEquipped())
		{
			if (this.getterRoutine == null)
			{
				this.getterRoutine = base.StartCoroutine(this.Getter());
				return;
			}
		}
		else if (this.getterRoutine != null)
		{
			base.StopCoroutine(this.getterRoutine);
			this.getterRoutine = null;
		}
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x0009B544 File Offset: 0x00099744
	private bool MagnetToolIsEquipped()
	{
		return this.hasMagnetTool && this.magnetToolStatus.IsEquipped;
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x0009B55B File Offset: 0x0009975B
	private bool MagnetBuffEquipped()
	{
		return this.hasMagnetBuff && this.magnetBuffStatus.IsEquipped;
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x0009B574 File Offset: 0x00099774
	private void OnFixedUpdate()
	{
		if (this.activated)
		{
			return;
		}
		if (this.isAttracted)
		{
			if (!this.hero)
			{
				this.hero = HeroController.instance.transform;
			}
			Vector3 position = this.hero.transform.position;
			Vector3 position2 = base.transform.position;
			Vector2 vector = new Vector2(position.x - position2.x, position.y - 0.5f - position2.y);
			vector = Vector2.ClampMagnitude(vector, 1f);
			vector *= 200f;
			this.attractVelocity += vector * Time.fixedDeltaTime;
			this.attractVelocity = Vector2.ClampMagnitude(this.attractVelocity, 20f);
		}
		else if (!this.isMoving)
		{
			return;
		}
		Vector2 b = this.attractVelocity * Time.fixedDeltaTime;
		this.body.linearVelocity = Vector2.zero;
		this.body.position += this.offsetPos + b;
		this.offsetPos = Vector2.zero;
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x0009B69C File Offset: 0x0009989C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.pickupRange)
		{
			return;
		}
		if (collision.CompareTag("Acid"))
		{
			float num = 0f;
			if (this.hasAcidEffect)
			{
				this.acidEffect.gameObject.SetActive(true);
				ParticleSystem.MainModule main = this.acidEffect.main;
				num = Mathf.Max(num, main.duration + main.startLifetime.constant);
			}
			this.Disable(num);
			return;
		}
		this.HandleHeroEnter(collision, base.gameObject);
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x0009B722 File Offset: 0x00099922
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (this.landedColliders.Add(other.collider))
		{
			this.Land();
		}
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x0009B740 File Offset: 0x00099940
	protected virtual void Land()
	{
		int frameCount = Time.frameCount;
		if (CurrencyObjectBase.lastLandFrame != frameCount)
		{
			CurrencyObjectBase.lastLandFrame = frameCount;
			CurrencyObjectBase.playedCount = 0;
		}
		if (CurrencyObjectBase.playedCount < 5)
		{
			CurrencyObjectBase.playedCount++;
			this.PlaySound(this.landSounds);
		}
		this.isOnGround = true;
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x0009B78F File Offset: 0x0009998F
	private void OnCollisionExit2D(Collision2D other)
	{
		if (this.isOnGround && this.landedColliders.Remove(other.collider) && this.landedColliders.Count == 0)
		{
			this.LeftGround();
		}
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x0009B7BF File Offset: 0x000999BF
	protected virtual void LeftGround()
	{
		this.isOnGround = false;
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x0009B7C8 File Offset: 0x000999C8
	protected virtual void HandleHeroEnter(Collider2D collision, GameObject sender)
	{
		if (!collision.CompareTag("HeroBox"))
		{
			return;
		}
		this.DoCollect();
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x0009B7E0 File Offset: 0x000999E0
	public void DoCollect()
	{
		if (this.activated || Time.timeAsDouble < this.pickupStartTime)
		{
			return;
		}
		if (CurrencyObjectBase.IsHeroDead)
		{
			return;
		}
		if (this.pickupEffect)
		{
			Vector3 position = base.transform.position;
			position.z = 0.001f;
			this.pickupEffect.Spawn(position);
		}
		VibrationManager.PlayVibrationClipOneShot(this.pickupVibration, null, false, "", false);
		float recycleTime = this.PlaySound(this.pickupSounds);
		if (!this.Collect())
		{
			return;
		}
		this.Disable(recycleTime);
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x0009B878 File Offset: 0x00099A78
	private float PlaySound(IReadOnlyList<AudioClip> sounds)
	{
		if (!this.audioSource || sounds.Count <= 0)
		{
			return 0f;
		}
		AudioClip audioClip = sounds[Random.Range(0, sounds.Count)];
		if (audioClip)
		{
			this.audioSource.PlayOneShot(audioClip);
			return audioClip.length;
		}
		return 0f;
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x0009B8D4 File Offset: 0x00099AD4
	private void SetVisible(bool visible)
	{
		if (this.hasRenderer)
		{
			this.rend.enabled = visible;
		}
		this.SetRendererActive(visible);
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x0009B8F1 File Offset: 0x00099AF1
	protected virtual void SetRendererActive(bool active)
	{
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x0009B8F4 File Offset: 0x00099AF4
	public void Disable(float recycleTime)
	{
		this.activated = true;
		this.SetVisible(false);
		if (this.hasMagnetEffect)
		{
			this.magnetEffect.SetActive(false);
		}
		if (this.persistent)
		{
			this.persistent.SaveState();
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (this.isDisabling)
		{
			this.waitTime = recycleTime;
			return;
		}
		this.collider.enabled = false;
		this.body.bodyType = RigidbodyType2D.Kinematic;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		this.isDisabling = true;
		if (this.getterRoutine != null)
		{
			base.StopCoroutine(this.getterRoutine);
			this.getterRoutine = null;
		}
		base.StartCoroutine(this.DisableAfterTime(recycleTime));
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x0009B9C1 File Offset: 0x00099BC1
	private IEnumerator DisableAfterTime(float recycleTime)
	{
		this.waitTime = 0f;
		while (recycleTime > 0f)
		{
			yield return null;
			if (this.waitTime > recycleTime)
			{
				recycleTime = this.waitTime;
				this.waitTime = 0f;
			}
			recycleTime -= Time.deltaTime;
		}
		if (this.breakEffect)
		{
			this.breakEffect.SetActive(false);
			this.isBroken = false;
		}
		this.isDisabling = false;
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x0009B9D7 File Offset: 0x00099BD7
	private IEnumerator Getter()
	{
		if (this.magnetToolRange)
		{
			while (!this.magnetToolRange.IsHeroInRange())
			{
				yield return null;
			}
		}
		float num = this.magnetStartDelay.GetRandomValue();
		if (this.MagnetBuffEquipped())
		{
			num *= this.magnetBuffDelayMultiplier;
		}
		yield return new WaitForSeconds(num);
		if (this.hasMagnetEffect)
		{
			this.magnetEffect.SetActive(true);
		}
		this.collider.isTrigger = true;
		this.body.gravityScale = 0f;
		this.body.linearVelocity = Vector2.zero;
		float magnetAttractDelayLeft = this.magnetAttractDelay.GetRandomValue();
		if (this.magnetStartMoveDuration > 0f)
		{
			this.isMoving = true;
			Vector2 startPos = this.body.position;
			Vector2 targetPos = startPos + new Vector2(0f, this.magnetStartHeight.GetRandomValue());
			Vector2 previousPos = startPos;
			float elapsed = 0f;
			while (elapsed <= this.magnetStartMoveDuration)
			{
				float num2 = elapsed / this.magnetStartMoveDuration;
				num2 = this.magnetStartMoveCurve.Evaluate(num2);
				Vector2 vector = Vector2.LerpUnclamped(startPos, targetPos, num2);
				this.offsetPos += vector - previousPos;
				previousPos = vector;
				yield return null;
				elapsed += Time.deltaTime;
				if (!this.isAttracted)
				{
					magnetAttractDelayLeft -= Time.deltaTime;
					if (magnetAttractDelayLeft <= 0f)
					{
						this.isAttracted = true;
					}
				}
			}
			startPos = default(Vector2);
			targetPos = default(Vector2);
			previousPos = default(Vector2);
		}
		if (!this.isAttracted && magnetAttractDelayLeft > 0f)
		{
			yield return new WaitForSeconds(magnetAttractDelayLeft);
			this.isAttracted = true;
		}
		yield break;
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x0009B9E6 File Offset: 0x00099BE6
	private bool Collect()
	{
		bool flag = this.Collected();
		if (flag && !this.hasCheckedPopup)
		{
			this.hasCheckedPopup = true;
			this.CollectPopup();
		}
		return flag;
	}

	// Token: 0x060021AF RID: 8623 RVA: 0x0009BA08 File Offset: 0x00099C08
	public virtual void CollectPopup()
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(this.firstGetPDBool) && !instance.GetVariable(this.firstGetPDBool))
		{
			instance.SetVariable(this.firstGetPDBool, true);
		}
		if (string.IsNullOrEmpty(this.popupPDBool) || instance.GetVariable(this.popupPDBool))
		{
			return;
		}
		instance.SetVariable(this.popupPDBool, true);
		if (!this.popupName.IsEmpty)
		{
			CollectableUIMsg.Spawn(new UIMsgDisplay
			{
				Name = this.popupName,
				Icon = this.popupSprite,
				IconScale = 1f
			}, null, false);
		}
	}

	// Token: 0x060021B0 RID: 8624
	protected abstract bool Collected();

	// Token: 0x060021B1 RID: 8625 RVA: 0x0009BABA File Offset: 0x00099CBA
	public virtual void BurnUp()
	{
		this.Disable(3f);
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x0009BAC8 File Offset: 0x00099CC8
	public void Break()
	{
		this.body.bodyType = RigidbodyType2D.Kinematic;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		if (this.breakEffect)
		{
			this.isBroken = true;
			this.breakEffect.SetActive(true);
		}
		this.Disable(1f);
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x0009BB2C File Offset: 0x00099D2C
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.activated || Time.timeAsDouble < this.pickupStartTime)
		{
			return IHitResponder.Response.None;
		}
		AttackTypes attackType = damageInstance.AttackType;
		if (attackType == AttackTypes.Acid || attackType == AttackTypes.Splatter || attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Coal || attackType == AttackTypes.Fire || attackType == AttackTypes.Spikes)
		{
			return IHitResponder.Response.None;
		}
		if (BreakOnHazard.IsCogDamager(damageInstance.Source))
		{
			return IHitResponder.Response.None;
		}
		float magnitudeMultForType = damageInstance.GetMagnitudeMultForType(HitInstance.TargetType.Currency);
		Vector2 vector;
		if (damageInstance.CircleDirection)
		{
			vector = damageInstance.GetActualDirection(base.transform, HitInstance.TargetType.Currency).AngleToDirection();
			vector.x *= 5f;
			if (this.isOnGround)
			{
				vector.y = Random.Range(5f, 15f);
			}
			else
			{
				vector.y *= (float)((vector.y > 0f) ? 10 : 3);
			}
		}
		else
		{
			HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
			Vector2 vector2 = new Vector2(Random.Range(5f, 1.5f), Random.Range(8f, 2f));
			if (Random.Range(0f, 1f) <= 0.25f)
			{
				HitInstance.HitDirection hitDirection2;
				if (hitDirection != HitInstance.HitDirection.Left)
				{
					if (hitDirection != HitInstance.HitDirection.Right)
					{
						hitDirection2 = hitDirection;
					}
					else
					{
						hitDirection2 = HitInstance.HitDirection.Left;
					}
				}
				else
				{
					hitDirection2 = HitInstance.HitDirection.Right;
				}
				hitDirection = hitDirection2;
			}
			if (hitDirection != HitInstance.HitDirection.Left)
			{
				if (hitDirection != HitInstance.HitDirection.Right)
				{
					vector = new Vector2(Random.Range(-3f, 3f), Random.Range(5f, 10f));
				}
				else
				{
					vector = vector2;
				}
			}
			else
			{
				vector = new Vector2(-vector2.x, vector2.y);
			}
		}
		if (this.insideAntRegion)
		{
			this.insideAntRegion.ResetTracker(base.gameObject);
		}
		this.body.AddForce(vector * magnitudeMultForType, ForceMode2D.Impulse);
		if ((this.body.constraints & RigidbodyConstraints2D.FreezeRotation) == RigidbodyConstraints2D.None)
		{
			float num = 3f;
			float torque = Random.Range(-num, num) * magnitudeMultForType;
			this.body.AddTorque(torque, ForceMode2D.Impulse);
		}
		return new IHitResponder.HitResponse(IHitResponder.Response.GenericHit, false);
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x0009BD20 File Offset: 0x00099F20
	public static void SendOnCameraShakedWorldForce(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		foreach (CurrencyObjectBase currencyObjectBase in CurrencyObjectBase._currencyObjects.List)
		{
			currencyObjectBase.OnCameraShakedWorldForce(intensity);
		}
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x0009BD78 File Offset: 0x00099F78
	protected virtual bool CanReactCameraShake()
	{
		return this.isOnGround && this.body.linearVelocity.y <= 0f;
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x0009BD9E File Offset: 0x00099F9E
	private void OnCameraShakedWorldForce(CameraShakeWorldForceIntensities intensity)
	{
		if (intensity < CameraShakeWorldForceIntensities.Intense)
		{
			return;
		}
		if (!this.CanReactCameraShake())
		{
			return;
		}
		if (!this.visibilityGroup.IsVisible)
		{
			return;
		}
		this.RandomJostle();
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x0009BDC4 File Offset: 0x00099FC4
	private void RandomJostle()
	{
		Vector2 force = new Vector2(Random.Range(-3f, 3f), Random.Range(10f, 20f));
		this.body.AddForce(force, ForceMode2D.Impulse);
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x0009BE03 File Offset: 0x0009A003
	public void EnteredAntRegion(AntRegion antRegion)
	{
		this.insideAntRegion = antRegion;
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x0009BE0C File Offset: 0x0009A00C
	public void ExitedAntRegion(AntRegion antRegion)
	{
		if (this.insideAntRegion == antRegion)
		{
			this.insideAntRegion = null;
		}
	}

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x060021BA RID: 8634 RVA: 0x0009BE23 File Offset: 0x0009A023
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			return BreakableBreaker.BreakableTypes.Basic;
		}
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x0009BE26 File Offset: 0x0009A026
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		this.RandomJostle();
	}

	// Token: 0x060021BC RID: 8636 RVA: 0x0009BE2E File Offset: 0x0009A02E
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		this.RandomJostle();
	}

	// Token: 0x060021BF RID: 8639 RVA: 0x0009BE8B File Offset: 0x0009A08B
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0400204F RID: 8271
	[SerializeField]
	private AudioClip[] landSounds;

	// Token: 0x04002050 RID: 8272
	[SerializeField]
	private AudioClip[] pickupSounds;

	// Token: 0x04002051 RID: 8273
	[SerializeField]
	private VibrationData pickupVibration;

	// Token: 0x04002052 RID: 8274
	[SerializeField]
	private VibrationDataAsset pickupVibrationAsset;

	// Token: 0x04002053 RID: 8275
	[SerializeField]
	private ParticleSystem acidEffect;

	// Token: 0x04002054 RID: 8276
	[SerializeField]
	private TriggerEnterEvent pickupRange;

	// Token: 0x04002055 RID: 8277
	[SerializeField]
	private ToolItem magnetTool;

	// Token: 0x04002056 RID: 8278
	[SerializeField]
	private ToolItem magnetBuffTool;

	// Token: 0x04002057 RID: 8279
	[SerializeField]
	private float magnetBuffDelayMultiplier;

	// Token: 0x04002058 RID: 8280
	[SerializeField]
	private AlertRange magnetToolRange;

	// Token: 0x04002059 RID: 8281
	[SerializeField]
	private GameObject magnetEffect;

	// Token: 0x0400205A RID: 8282
	[SerializeField]
	private GameObject breakEffect;

	// Token: 0x0400205B RID: 8283
	[SerializeField]
	private GameObject pickupEffect;

	// Token: 0x0400205C RID: 8284
	[SerializeField]
	private MinMaxFloat magnetStartDelay = new MinMaxFloat(1f, 1.7f);

	// Token: 0x0400205D RID: 8285
	[SerializeField]
	private MinMaxFloat magnetStartHeight;

	// Token: 0x0400205E RID: 8286
	[SerializeField]
	private float magnetStartMoveDuration;

	// Token: 0x0400205F RID: 8287
	[SerializeField]
	private AnimationCurve magnetStartMoveCurve;

	// Token: 0x04002060 RID: 8288
	[SerializeField]
	private MinMaxFloat magnetAttractDelay = new MinMaxFloat(0.3f, 0.5f);

	// Token: 0x04002061 RID: 8289
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002062 RID: 8290
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	protected string firstGetPDBool;

	// Token: 0x04002063 RID: 8291
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	protected string popupPDBool;

	// Token: 0x04002064 RID: 8292
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	protected LocalisedString popupName;

	// Token: 0x04002065 RID: 8293
	[SerializeField]
	protected Sprite popupSprite;

	// Token: 0x04002066 RID: 8294
	private Coroutine getterRoutine;

	// Token: 0x04002067 RID: 8295
	private bool isAttracted;

	// Token: 0x04002068 RID: 8296
	private bool isMoving;

	// Token: 0x04002069 RID: 8297
	private bool activated;

	// Token: 0x0400206A RID: 8298
	private float defaultGravityScale;

	// Token: 0x0400206B RID: 8299
	private Vector2 offsetPos;

	// Token: 0x0400206C RID: 8300
	private Vector2 attractVelocity;

	// Token: 0x0400206D RID: 8301
	private AntRegion insideAntRegion;

	// Token: 0x0400206E RID: 8302
	private const float PICKUP_START_DELAY = 0.35f;

	// Token: 0x0400206F RID: 8303
	private double pickupStartTime;

	// Token: 0x04002070 RID: 8304
	private Transform hero;

	// Token: 0x04002071 RID: 8305
	private AudioSource audioSource;

	// Token: 0x04002072 RID: 8306
	private Renderer rend;

	// Token: 0x04002073 RID: 8307
	private Rigidbody2D body;

	// Token: 0x04002074 RID: 8308
	private Collider2D collider;

	// Token: 0x04002075 RID: 8309
	private readonly HashSet<Collider2D> landedColliders = new HashSet<Collider2D>();

	// Token: 0x04002076 RID: 8310
	private bool hasRenderer;

	// Token: 0x04002077 RID: 8311
	private bool hasAcidEffect;

	// Token: 0x04002078 RID: 8312
	private bool hasMagnetEffect;

	// Token: 0x04002079 RID: 8313
	private bool hasMagnetTool;

	// Token: 0x0400207A RID: 8314
	private bool hasMagnetBuff;

	// Token: 0x0400207B RID: 8315
	private bool isDisabling;

	// Token: 0x0400207C RID: 8316
	private float waitTime;

	// Token: 0x0400207D RID: 8317
	private VisibilityGroup visibilityGroup;

	// Token: 0x0400207E RID: 8318
	private bool isOnGround;

	// Token: 0x0400207F RID: 8319
	private bool hasStarted;

	// Token: 0x04002080 RID: 8320
	private static readonly UniqueList<CurrencyObjectBase> _currencyObjects = new UniqueList<CurrencyObjectBase>();

	// Token: 0x04002081 RID: 8321
	private static int _lastUpdate = -1;

	// Token: 0x04002082 RID: 8322
	private static bool _isHeroDead;

	// Token: 0x04002083 RID: 8323
	private bool isBroken;

	// Token: 0x04002084 RID: 8324
	private static int lastPickupUpdate = -1;

	// Token: 0x04002085 RID: 8325
	private static bool canPickupExist;

	// Token: 0x04002086 RID: 8326
	private ToolItemManager.ToolStatus magnetToolStatus;

	// Token: 0x04002087 RID: 8327
	private ToolItemManager.ToolStatus magnetBuffStatus;

	// Token: 0x04002088 RID: 8328
	private static int lastLandFrame;

	// Token: 0x04002089 RID: 8329
	private static int playedCount;

	// Token: 0x0400208A RID: 8330
	private const int LAND_SOUND_LIMIT = 5;

	// Token: 0x0400208B RID: 8331
	protected bool hasCheckedPopup;
}
