using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002DA RID: 730
public sealed class EnemyHitEffectsBasic : MonoBehaviour, IHitEffectReciever, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	// Token: 0x14000049 RID: 73
	// (add) Token: 0x060019DB RID: 6619 RVA: 0x00076E24 File Offset: 0x00075024
	// (remove) Token: 0x060019DC RID: 6620 RVA: 0x00076E5C File Offset: 0x0007505C
	public event EnemyHitEffectsBasic.ReceivedHitEffectDelegate ReceivedHitEffect;

	// Token: 0x170002A9 RID: 681
	// (get) Token: 0x060019DD RID: 6621 RVA: 0x00076E91 File Offset: 0x00075091
	// (set) Token: 0x060019DE RID: 6622 RVA: 0x00076E99 File Offset: 0x00075099
	public bool OverrideBloodColor
	{
		get
		{
			return this.overrideBloodColor;
		}
		set
		{
			this.overrideBloodColor = value;
		}
	}

	// Token: 0x170002AA RID: 682
	// (get) Token: 0x060019DF RID: 6623 RVA: 0x00076EA2 File Offset: 0x000750A2
	// (set) Token: 0x060019E0 RID: 6624 RVA: 0x00076EAA File Offset: 0x000750AA
	public Color BloodColorOverride
	{
		get
		{
			return this.bloodColorOverride;
		}
		set
		{
			this.bloodColorOverride = value;
		}
	}

	// Token: 0x060019E1 RID: 6625 RVA: 0x00076EB3 File Offset: 0x000750B3
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x060019E2 RID: 6626 RVA: 0x00076EBC File Offset: 0x000750BC
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		if (!this.poolCreated && this.profile != null)
		{
			this.EnsurePersonalPool(this.profile);
		}
		return true;
	}

	// Token: 0x060019E3 RID: 6627 RVA: 0x00076EF8 File Offset: 0x000750F8
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x060019E4 RID: 6628 RVA: 0x00076F14 File Offset: 0x00075114
	public void ReceiveHitEffect(float attackDirection)
	{
		this.ReceiveHitEffect(new HitInstance
		{
			Direction = attackDirection,
			AttackType = AttackTypes.Generic
		});
	}

	// Token: 0x060019E5 RID: 6629 RVA: 0x00076F40 File Offset: 0x00075140
	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		this.ReceiveHitEffect(hitInstance, this.effectOrigin);
	}

	// Token: 0x060019E6 RID: 6630 RVA: 0x00076F54 File Offset: 0x00075154
	public void ReceiveHitEffect(HitInstance hitInstance, Vector2 effectOrigin2D)
	{
		if (this.didFireThisFrame)
		{
			return;
		}
		this.didFireThisFrame = true;
		Vector3 vector = effectOrigin2D.ToVector3(this.effectOrigin.z);
		if (this.sendDamageFlashEvent)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "DAMAGE FLASH", true);
		}
		bool flag = false;
		if (hitInstance.AttackType == AttackTypes.Coal)
		{
			flag = true;
			Effects.EnemyCoalHurtSound.SpawnAndPlayOneShot(base.transform.TransformPoint(vector), null);
		}
		if (!hitInstance.ForceNotWeakHit && hitInstance.DamageDealt <= 0 && hitInstance.HitEffectsType != EnemyHitEffectsProfile.EffectsTypes.LagHit)
		{
			Effects.WeakHitEffectShake.DoShake(this, true);
			GameObject weakHitEffectPrefab = Effects.WeakHitEffectPrefab;
			if (weakHitEffectPrefab)
			{
				float hitDirectionAsAngle = hitInstance.GetHitDirectionAsAngle(HitInstance.TargetType.Regular);
				weakHitEffectPrefab.Spawn(base.transform.TransformPoint(vector), Quaternion.Euler(0f, 0f, hitDirectionAsAngle));
			}
			return;
		}
		if (!flag && this.ReceivedHitEffect != null)
		{
			this.ReceivedHitEffect(hitInstance, effectOrigin2D);
		}
		if (hitInstance.AttackType != AttackTypes.Lightning)
		{
			Color color = this.hitFlashColor;
		}
		else
		{
			Color color2 = Effects.LightningHitEffects.HitFlashColor;
		}
		if (flag)
		{
			if (this.spriteFlash)
			{
				if (hitInstance.RageHit)
				{
					this.spriteFlash.FlashEnemyHitRage();
					return;
				}
				this.spriteFlash.FlashEnemyHit(hitInstance);
			}
			return;
		}
		if (this.doHitFlash && this.spriteFlash)
		{
			if (hitInstance.RageHit)
			{
				this.spriteFlash.FlashEnemyHitRage();
			}
			else
			{
				this.spriteFlash.FlashEnemyHit(hitInstance);
			}
		}
		if (this.forceMinimalEffects && hitInstance.HitEffectsType == EnemyHitEffectsProfile.EffectsTypes.Full)
		{
			hitInstance.HitEffectsType = EnemyHitEffectsProfile.EffectsTypes.Minimal;
		}
		if (this.overrideBloodColor)
		{
			this.SpawnEffects(base.transform, vector, hitInstance, new Color?(this.bloodColorOverride));
			return;
		}
		this.SpawnEffects(base.transform, vector, hitInstance, null);
	}

	// Token: 0x060019E7 RID: 6631 RVA: 0x0007711A File Offset: 0x0007531A
	public void SetEffectOrigin(Vector3 newEffectOrigin)
	{
		this.effectOrigin = newEffectOrigin;
	}

	// Token: 0x060019E8 RID: 6632 RVA: 0x00077123 File Offset: 0x00075323
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x060019E9 RID: 6633 RVA: 0x0007712C File Offset: 0x0007532C
	public void SpawnEffects(Transform spawnPoint, Vector3 offset, HitInstance damageInstance, Color? bloodColorOverride = null)
	{
		float blackThreadAmount = (float)(this.isBlackThreaded ? 1 : 0);
		this.profile.SpawnEffects(spawnPoint, offset, damageInstance, bloodColorOverride, blackThreadAmount);
	}

	// Token: 0x060019EA RID: 6634 RVA: 0x00077158 File Offset: 0x00075358
	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			this.isBlackThreaded = true;
			return;
		}
		this.isBlackThreaded = false;
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x0007716C File Offset: 0x0007536C
	private void EnsurePersonalPool(EnemyHitEffectsProfile.ProfileSection profileSection)
	{
		foreach (EnemyHitEffectsProfile.HitFlingConfig hitFlingConfig in profileSection.SpawnFlings)
		{
			if (!(hitFlingConfig.Prefab == null))
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, hitFlingConfig.Prefab, 3, false, false, false);
			}
		}
		foreach (GameObject gameObject in profileSection.SpawnEffectPrefabs)
		{
			if (!(gameObject == null))
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, gameObject, 3, false, false, false);
			}
		}
	}

	// Token: 0x060019EC RID: 6636 RVA: 0x000771F3 File Offset: 0x000753F3
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.effectOrigin, 0.25f);
	}

	// Token: 0x060019EE RID: 6638 RVA: 0x0007722F File Offset: 0x0007542F
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x040018D1 RID: 6353
	[SerializeField]
	private Vector3 effectOrigin;

	// Token: 0x040018D2 RID: 6354
	[Tooltip("Disable if there are no listeners for this event, to save the expensive recursive send event.")]
	public bool sendDamageFlashEvent = true;

	// Token: 0x040018D3 RID: 6355
	[Space]
	[SerializeField]
	private EnemyHitEffectsProfile.ProfileSection profile;

	// Token: 0x040018D4 RID: 6356
	[SerializeField]
	private bool doHitFlash;

	// Token: 0x040018D5 RID: 6357
	[SerializeField]
	private Color hitFlashColor = Color.white;

	// Token: 0x040018D6 RID: 6358
	[SerializeField]
	private bool overrideBloodColor;

	// Token: 0x040018D7 RID: 6359
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideBloodColor", true, false, true)]
	private Color bloodColorOverride;

	// Token: 0x040018D8 RID: 6360
	[SerializeField]
	private bool forceMinimalEffects;

	// Token: 0x040018D9 RID: 6361
	private SpriteFlash spriteFlash;

	// Token: 0x040018DA RID: 6362
	private bool didFireThisFrame;

	// Token: 0x040018DB RID: 6363
	private bool poolCreated;

	// Token: 0x040018DC RID: 6364
	private bool hasAwaken;

	// Token: 0x040018DD RID: 6365
	private bool hasStarted;

	// Token: 0x040018DE RID: 6366
	private bool isBlackThreaded;

	// Token: 0x020015B9 RID: 5561
	// (Invoke) Token: 0x060087DD RID: 34781
	public delegate void ReceivedHitEffectDelegate(HitInstance hitInstance, Vector2 effectOrigin2D);
}
