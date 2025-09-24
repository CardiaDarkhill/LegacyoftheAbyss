using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000543 RID: 1347
public class RosaryCache : MonoBehaviour, IHitResponder
{
	// Token: 0x1700054D RID: 1357
	// (get) Token: 0x06003037 RID: 12343 RVA: 0x000D4D16 File Offset: 0x000D2F16
	// (set) Token: 0x06003038 RID: 12344 RVA: 0x000D4D1E File Offset: 0x000D2F1E
	private protected int State { protected get; private set; }

	// Token: 0x1700054E RID: 1358
	// (get) Token: 0x06003039 RID: 12345 RVA: 0x000D4D27 File Offset: 0x000D2F27
	protected int StateCount
	{
		get
		{
			if (this.hitStates == null)
			{
				return 0;
			}
			return this.hitStates.Count;
		}
	}

	// Token: 0x1700054F RID: 1359
	// (get) Token: 0x0600303A RID: 12346 RVA: 0x000D4D3E File Offset: 0x000D2F3E
	protected virtual int HitCount
	{
		get
		{
			return this.StateCount;
		}
	}

	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x0600303B RID: 12347 RVA: 0x000D4D46 File Offset: 0x000D2F46
	// (set) Token: 0x0600303C RID: 12348 RVA: 0x000D4D4E File Offset: 0x000D2F4E
	private protected bool IsReactivating { protected get; private set; }

	// Token: 0x0600303D RID: 12349 RVA: 0x000D4D58 File Offset: 0x000D2F58
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.flingSpawnPos, 0.1f);
		if (!this.clampHitPos)
		{
			return;
		}
		Bounds bounds = new Bounds
		{
			min = this.clampHitPosMin,
			max = this.clampHitPosMax
		};
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x000D4DD4 File Offset: 0x000D2FD4
	protected virtual void Awake()
	{
		this.State = this.startState;
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out int value)
			{
				value = this.State;
			};
			this.persistent.OnSetSaveState += delegate(int value)
			{
				this.State = value;
				this.IsReactivating = true;
				for (int j = 0; j <= Mathf.Min(this.State, this.hitStates.Count - 1); j++)
				{
					this.hitStates[j].Activate();
				}
				this.IsReactivating = false;
				if (this.State >= this.hitStates.Count - 1)
				{
					this.SetCompletedReturning();
					if (this.breaker)
					{
						this.breaker.SetAlreadyBroken();
					}
				}
			};
		}
		GameObject gameObject = base.gameObject;
		bool flag = false;
		if (this.hitEffectPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(gameObject, this.hitEffectPrefab, 1, false, false, false);
			flag = true;
		}
		if (this.flingOnStateChange.Length != 0)
		{
			for (int i = 0; i < this.flingOnStateChange.Length; i++)
			{
				FlingUtils.EnsurePersonalPool(this.flingOnStateChange[i], gameObject);
			}
			flag = true;
		}
		ChainPushReaction component = base.GetComponent<ChainPushReaction>();
		if (component != null)
		{
			component.OnTouched += this.Touched;
		}
		if (flag)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(gameObject);
		}
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x000D4EAC File Offset: 0x000D30AC
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		bool flag = true;
		AudioEventRandom audioEventRandom = this.hitSound;
		int state = this.State;
		if (this.hitStates.Count > 0)
		{
			int state2 = this.State;
			this.State = state2 + 1;
			if (this.State >= this.hitStates.Count)
			{
				this.State = this.hitStates.Count - 1;
				if (!this.canHitAfterLastState)
				{
					return IHitResponder.Response.None;
				}
			}
			if (this.State >= this.HitCount)
			{
				flag = false;
				audioEventRandom = this.emptyHitSound;
			}
		}
		else
		{
			flag = false;
			audioEventRandom = this.emptyHitSound;
		}
		if (this.State != state)
		{
			this.<Hit>g__DoFling|35_0();
			AttackTypes attackType = damageInstance.AttackType;
			if (attackType != AttackTypes.Explosion && attackType != AttackTypes.Heavy)
			{
				if (attackType != AttackTypes.Lightning)
				{
					goto IL_DF;
				}
			}
			while (this.State < this.hitStates.Count - 1)
			{
				int state2 = this.State;
				this.State = state2 + 1;
				this.<Hit>g__DoFling|35_0();
			}
		}
		IL_DF:
		Vector3 position = base.transform.position;
		float? y = new float?(this.GetHitSourceY(damageInstance.Source.transform.position.y));
		Vector3 vector = position.Where(null, y, null);
		if (this.clampHitPos)
		{
			Vector3 vector2 = base.transform.TransformPoint(this.clampHitPosMin);
			Vector3 vector3 = base.transform.TransformPoint(this.clampHitPosMax);
			if (vector.x < vector2.x)
			{
				vector.x = vector2.x;
			}
			else if (vector.x > vector3.x)
			{
				vector.x = vector3.x;
			}
			if (vector.y < vector2.y)
			{
				vector.y = vector2.y;
			}
			else if (vector.y > vector3.y)
			{
				vector.y = vector3.y;
			}
		}
		if (this.hitEffectPrefab)
		{
			this.hitEffectPrefab.Spawn(vector);
		}
		if (flag)
		{
			this.hitCameraShake.DoShake(this, true);
		}
		audioEventRandom.SpawnAndPlayOneShot(vector, null);
		this.RespondToHit(damageInstance, vector);
		this.OnHit.Invoke();
		float num;
		switch (damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			num = -1f;
			break;
		case HitInstance.HitDirection.Right:
			num = 1f;
			break;
		case HitInstance.HitDirection.Up:
		case HitInstance.HitDirection.Down:
			if (damageInstance.Source.transform.position.x > base.transform.position.x)
			{
				num = -1f;
			}
			else
			{
				num = 1f;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (num > 0f)
		{
			this.OnHitRight.Invoke();
		}
		else
		{
			this.OnHitLeft.Invoke();
		}
		if (this.breaker && this.State == this.hitStates.Count - 1)
		{
			this.breaker.BreakSelf();
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x000D51AE File Offset: 0x000D33AE
	public void Touched()
	{
		this.Touched(base.transform.position);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x000D51C1 File Offset: 0x000D33C1
	public void Touched(Vector3 touchPosition)
	{
		if (this.State >= this.HitCount)
		{
			this.emptyTouchSound.SpawnAndPlayOneShot(touchPosition, null);
			return;
		}
		this.touchSound.SpawnAndPlayOneShot(touchPosition, null);
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x000D51EE File Offset: 0x000D33EE
	protected virtual float GetHitSourceY(float sourceHeight)
	{
		return sourceHeight;
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x000D51F1 File Offset: 0x000D33F1
	protected virtual void RespondToHit(HitInstance damageInstance, Vector2 hitPos)
	{
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x000D51F3 File Offset: 0x000D33F3
	protected virtual void SetCompletedReturning()
	{
		if (this.deactivateCompleted)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x000D52BC File Offset: 0x000D34BC
	[CompilerGenerated]
	private void <Hit>g__DoFling|35_0()
	{
		RosaryCache.HitState hitState = (this.hitStates.Count > 0) ? this.hitStates[this.State] : null;
		if (hitState != null)
		{
			hitState.Activate();
		}
		FlingUtils.Config[] array = this.flingOnStateChange;
		for (int i = 0; i < array.Length; i++)
		{
			FlingUtils.SpawnAndFling(array[i], base.transform, this.flingSpawnPos, null, -1f);
		}
	}

	// Token: 0x04003315 RID: 13077
	[SerializeField]
	private CameraShakeTarget hitCameraShake;

	// Token: 0x04003316 RID: 13078
	[SerializeField]
	private GameObject hitEffectPrefab;

	// Token: 0x04003317 RID: 13079
	[SerializeField]
	private bool clampHitPos;

	// Token: 0x04003318 RID: 13080
	[SerializeField]
	private Vector2 clampHitPosMin;

	// Token: 0x04003319 RID: 13081
	[SerializeField]
	private Vector2 clampHitPosMax;

	// Token: 0x0400331A RID: 13082
	[SerializeField]
	private AudioEventRandom hitSound;

	// Token: 0x0400331B RID: 13083
	[SerializeField]
	private AudioEventRandom emptyHitSound;

	// Token: 0x0400331C RID: 13084
	[Space]
	[SerializeField]
	private AudioEventRandom touchSound;

	// Token: 0x0400331D RID: 13085
	[SerializeField]
	private AudioEventRandom emptyTouchSound;

	// Token: 0x0400331E RID: 13086
	[Space]
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x0400331F RID: 13087
	[SerializeField]
	private bool deactivateCompleted;

	// Token: 0x04003320 RID: 13088
	[SerializeField]
	private int startState = -1;

	// Token: 0x04003321 RID: 13089
	[SerializeField]
	protected List<RosaryCache.HitState> hitStates = new List<RosaryCache.HitState>();

	// Token: 0x04003322 RID: 13090
	[FormerlySerializedAs("canHitLastState")]
	[SerializeField]
	private bool canHitAfterLastState;

	// Token: 0x04003323 RID: 13091
	[SerializeField]
	private FlingUtils.Config[] flingOnStateChange;

	// Token: 0x04003324 RID: 13092
	[SerializeField]
	private Vector2 flingSpawnPos;

	// Token: 0x04003325 RID: 13093
	[SerializeField]
	private Breakable breaker;

	// Token: 0x04003326 RID: 13094
	[Space]
	public UnityEvent OnHit;

	// Token: 0x04003327 RID: 13095
	public UnityEvent OnHitLeft;

	// Token: 0x04003328 RID: 13096
	public UnityEvent OnHitRight;

	// Token: 0x0200184B RID: 6219
	[Serializable]
	protected class HitState
	{
		// Token: 0x060090A6 RID: 37030 RVA: 0x00295DD0 File Offset: 0x00293FD0
		public void Activate()
		{
			if (this.isActivated)
			{
				return;
			}
			this.isActivated = true;
			UnityEvent onActivated = this.OnActivated;
			if (onActivated == null)
			{
				return;
			}
			onActivated.Invoke();
		}

		// Token: 0x060090A7 RID: 37031 RVA: 0x00295DF2 File Offset: 0x00293FF2
		public RosaryCache.HitState Duplicate()
		{
			return new RosaryCache.HitState
			{
				OnActivated = new UnityEvent()
			};
		}

		// Token: 0x0400917E RID: 37246
		[FormerlySerializedAs("OnHit")]
		public UnityEvent OnActivated;

		// Token: 0x0400917F RID: 37247
		private bool isActivated;
	}
}
