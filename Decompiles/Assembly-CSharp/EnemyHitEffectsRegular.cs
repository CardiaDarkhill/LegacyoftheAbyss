using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class EnemyHitEffectsRegular : MonoBehaviour, IHitEffectReciever, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	// Token: 0x1400004A RID: 74
	// (add) Token: 0x060019FF RID: 6655 RVA: 0x00077A94 File Offset: 0x00075C94
	// (remove) Token: 0x06001A00 RID: 6656 RVA: 0x00077ACC File Offset: 0x00075CCC
	public event EnemyHitEffectsRegular.ReceivedHitEffectDelegate ReceivedHitEffect;

	// Token: 0x170002AE RID: 686
	// (get) Token: 0x06001A01 RID: 6657 RVA: 0x00077B01 File Offset: 0x00075D01
	// (set) Token: 0x06001A02 RID: 6658 RVA: 0x00077B09 File Offset: 0x00075D09
	public EnemyHitEffectsProfile Profile
	{
		get
		{
			return this.profile;
		}
		set
		{
			if (this.profile != value)
			{
				this.profile = value;
				if (this.profile)
				{
					this.profile.EnsurePersonalPool(base.gameObject);
					this.poolCreated = true;
				}
			}
		}
	}

	// Token: 0x170002AF RID: 687
	// (get) Token: 0x06001A03 RID: 6659 RVA: 0x00077B45 File Offset: 0x00075D45
	public Vector3 EffectOrigin
	{
		get
		{
			return this.effectOrigin;
		}
	}

	// Token: 0x170002B0 RID: 688
	// (get) Token: 0x06001A04 RID: 6660 RVA: 0x00077B4D File Offset: 0x00075D4D
	// (set) Token: 0x06001A05 RID: 6661 RVA: 0x00077B55 File Offset: 0x00075D55
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

	// Token: 0x170002B1 RID: 689
	// (get) Token: 0x06001A06 RID: 6662 RVA: 0x00077B5E File Offset: 0x00075D5E
	// (set) Token: 0x06001A07 RID: 6663 RVA: 0x00077B66 File Offset: 0x00075D66
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

	// Token: 0x06001A08 RID: 6664 RVA: 0x00077B6F File Offset: 0x00075D6F
	protected void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06001A09 RID: 6665 RVA: 0x00077B78 File Offset: 0x00075D78
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		if (!this.poolCreated && this.profile)
		{
			this.profile.EnsurePersonalPool(base.gameObject);
		}
		return true;
	}

	// Token: 0x06001A0A RID: 6666 RVA: 0x00077BC9 File Offset: 0x00075DC9
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

	// Token: 0x06001A0B RID: 6667 RVA: 0x00077BE4 File Offset: 0x00075DE4
	public void ReceiveHitEffect(float attackDirection)
	{
		this.ReceiveHitEffect(new HitInstance
		{
			Direction = attackDirection,
			AttackType = AttackTypes.Generic
		});
	}

	// Token: 0x06001A0C RID: 6668 RVA: 0x00077C10 File Offset: 0x00075E10
	public void ReceiveHitEffect(HitInstance hitInstance)
	{
		this.ReceiveHitEffect(hitInstance, this.effectOrigin);
	}

	// Token: 0x06001A0D RID: 6669 RVA: 0x00077C24 File Offset: 0x00075E24
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
		EnemyHitEffectsProfile enemyHitEffectsProfile = (hitInstance.AttackType == AttackTypes.Lightning) ? Effects.LightningHitEffects : this.profile;
		if (!enemyHitEffectsProfile || flag)
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
		if (enemyHitEffectsProfile.DoHitFlash && this.spriteFlash)
		{
			if (hitInstance.RageHit)
			{
				this.spriteFlash.FlashEnemyHitRage();
			}
			else if (enemyHitEffectsProfile.OverrideHitFlashColor)
			{
				this.spriteFlash.FlashEnemyHit(enemyHitEffectsProfile.HitFlashColor);
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
		float num = this.isBlackThreaded ? 1f : 0f;
		if (this.overrideBloodColor)
		{
			enemyHitEffectsProfile.SpawnEffects(base.transform, vector, hitInstance, new Color?(this.bloodColorOverride), num);
			return;
		}
		EnemyHitEffectsProfile enemyHitEffectsProfile2 = enemyHitEffectsProfile;
		Transform transform = base.transform;
		Vector3 offset = vector;
		HitInstance damageInstance = hitInstance;
		float blackThreadAmount = num;
		enemyHitEffectsProfile2.SpawnEffects(transform, offset, damageInstance, null, blackThreadAmount);
	}

	// Token: 0x06001A0E RID: 6670 RVA: 0x00077E2B File Offset: 0x0007602B
	public void SetEffectOrigin(Vector3 newEffectOrigin)
	{
		this.effectOrigin = newEffectOrigin;
	}

	// Token: 0x06001A0F RID: 6671 RVA: 0x00077E34 File Offset: 0x00076034
	protected void Update()
	{
		this.didFireThisFrame = false;
	}

	// Token: 0x06001A10 RID: 6672 RVA: 0x00077E3D File Offset: 0x0007603D
	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			this.isBlackThreaded = true;
			return;
		}
		this.isBlackThreaded = false;
	}

	// Token: 0x06001A11 RID: 6673 RVA: 0x00077E51 File Offset: 0x00076051
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.effectOrigin, 0.25f);
	}

	// Token: 0x06001A12 RID: 6674 RVA: 0x00077E73 File Offset: 0x00076073
	public float GetEffectOriginX()
	{
		return this.effectOrigin.x;
	}

	// Token: 0x06001A13 RID: 6675 RVA: 0x00077E80 File Offset: 0x00076080
	public float GetEffectOriginY()
	{
		return this.effectOrigin.y;
	}

	// Token: 0x06001A15 RID: 6677 RVA: 0x00077E9C File Offset: 0x0007609C
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x040018F6 RID: 6390
	[SerializeField]
	private Vector3 effectOrigin;

	// Token: 0x040018F7 RID: 6391
	[Tooltip("Disable if there are no listeners for this event, to save the expensive recursive send event.")]
	public bool sendDamageFlashEvent = true;

	// Token: 0x040018F8 RID: 6392
	[Space]
	[SerializeField]
	private EnemyHitEffectsProfile profile;

	// Token: 0x040018F9 RID: 6393
	[SerializeField]
	private bool overrideBloodColor;

	// Token: 0x040018FA RID: 6394
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideBloodColor", true, false, true)]
	private Color bloodColorOverride;

	// Token: 0x040018FB RID: 6395
	[SerializeField]
	private bool forceMinimalEffects;

	// Token: 0x040018FC RID: 6396
	private SpriteFlash spriteFlash;

	// Token: 0x040018FD RID: 6397
	private bool didFireThisFrame;

	// Token: 0x040018FE RID: 6398
	private bool poolCreated;

	// Token: 0x040018FF RID: 6399
	private bool hasAwaken;

	// Token: 0x04001900 RID: 6400
	private bool hasStarted;

	// Token: 0x04001901 RID: 6401
	private bool isBlackThreaded;

	// Token: 0x020015BF RID: 5567
	// (Invoke) Token: 0x060087E5 RID: 34789
	public delegate void ReceivedHitEffectDelegate(HitInstance hitInstance, Vector2 effectOrigin2D);
}
