using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002D7 RID: 727
[RequireComponent(typeof(EnemyHitEffectsRegular))]
public class EnemyDeathEffectsRegular : EnemyDeathEffects
{
	// Token: 0x170002A7 RID: 679
	// (get) Token: 0x060019CA RID: 6602 RVA: 0x00076790 File Offset: 0x00074990
	protected override Color? OverrideBloodColor
	{
		get
		{
			if (!this.hitEffects.OverrideBloodColor)
			{
				return null;
			}
			return new Color?(this.hitEffects.BloodColorOverride);
		}
	}

	// Token: 0x170002A8 RID: 680
	// (get) Token: 0x060019CB RID: 6603 RVA: 0x000767C4 File Offset: 0x000749C4
	// (set) Token: 0x060019CC RID: 6604 RVA: 0x000767CC File Offset: 0x000749CC
	public EnemyDeathEffectsProfile Profile
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

	// Token: 0x060019CD RID: 6605 RVA: 0x00076808 File Offset: 0x00074A08
	public override bool OnAwake()
	{
		if (base.OnAwake())
		{
			this.hitEffects = base.GetComponent<EnemyHitEffectsRegular>();
			if (!this.poolCreated && this.profile)
			{
				this.profile.EnsurePersonalPool(base.gameObject);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060019CE RID: 6606 RVA: 0x00076848 File Offset: 0x00074A48
	protected override void EmitEffects(GameObject corpseObj)
	{
		bool overrideBloodColor = this.hitEffects.OverrideBloodColor;
		Color bloodColorOverride = this.hitEffects.BloodColorOverride;
		base.EmitSound();
		base.ShakeCameraIfVisible();
		Transform transform = null;
		if (corpseObj != null)
		{
			transform = corpseObj.transform;
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.FlashEnemyHit();
			}
		}
		if (!this.profile)
		{
			return;
		}
		float blackThreadAmount = base.GetBlackThreadAmount();
		if (overrideBloodColor)
		{
			this.profile.SpawnEffects(base.transform, this.effectOrigin, transform, new Color?(bloodColorOverride), blackThreadAmount);
			return;
		}
		EnemyDeathEffectsProfile enemyDeathEffectsProfile = this.profile;
		Transform transform2 = base.transform;
		Vector3 effectOrigin = this.effectOrigin;
		Transform corpse = transform;
		float blackThreadAmount2 = blackThreadAmount;
		enemyDeathEffectsProfile.SpawnEffects(transform2, effectOrigin, corpse, null, blackThreadAmount2);
	}

	// Token: 0x040018BC RID: 6332
	[Space]
	[SerializeField]
	private EnemyDeathEffectsProfile profile;

	// Token: 0x040018BD RID: 6333
	private EnemyHitEffectsRegular hitEffects;

	// Token: 0x040018BE RID: 6334
	private bool poolCreated;

	// Token: 0x020015B6 RID: 5558
	[ActionCategory("Hollow Knight")]
	public class SimulateDeath : FsmStateAction
	{
		// Token: 0x060087D3 RID: 34771 RVA: 0x00278041 File Offset: 0x00276241
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x060087D4 RID: 34772 RVA: 0x00278050 File Offset: 0x00276250
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				HealthManager component2 = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					float? attackDirection2;
					if (component2)
					{
						int attackDirection = component2.GetAttackDirection();
						attackDirection2 = new float?(DirectionUtils.GetAngle(attackDirection));
					}
					else
					{
						attackDirection2 = null;
					}
					GameObject corpseObj;
					if (component.IsCorpseRecyclable)
					{
						bool flag;
						corpseObj = component.EmitCorpse(attackDirection2, 1f, AttackTypes.Generic, NailElements.None, null, null, out flag);
					}
					else
					{
						corpseObj = null;
					}
					component.EmitEffects(corpseObj);
					component.RecordKillForJournal();
				}
			}
			base.Finish();
		}

		// Token: 0x0400884B RID: 34891
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}

	// Token: 0x020015B7 RID: 5559
	[ActionCategory("Hollow Knight")]
	public class SimulateDeathV2 : FsmStateAction
	{
		// Token: 0x060087D6 RID: 34774 RVA: 0x002780EF File Offset: 0x002762EF
		public override void Reset()
		{
			this.Target = null;
			this.ForceEmitIfCorpseNotRecyclable = null;
		}

		// Token: 0x060087D7 RID: 34775 RVA: 0x00278100 File Offset: 0x00276300
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				HealthManager component2 = safe.GetComponent<HealthManager>();
				if (component != null)
				{
					float? attackDirection2;
					if (component2)
					{
						int attackDirection = component2.GetAttackDirection();
						attackDirection2 = new float?(DirectionUtils.GetAngle(attackDirection));
					}
					else
					{
						attackDirection2 = null;
					}
					GameObject corpseObj;
					if (component.IsCorpseRecyclable || this.ForceEmitIfCorpseNotRecyclable.Value)
					{
						bool flag;
						corpseObj = component.EmitCorpse(attackDirection2, 1f, AttackTypes.Generic, NailElements.None, null, null, out flag);
					}
					else
					{
						corpseObj = null;
					}
					component.EmitEffects(corpseObj);
					component.RecordKillForJournal();
				}
			}
			base.Finish();
		}

		// Token: 0x0400884C RID: 34892
		public FsmOwnerDefault Target;

		// Token: 0x0400884D RID: 34893
		public FsmBool ForceEmitIfCorpseNotRecyclable;
	}

	// Token: 0x020015B8 RID: 5560
	[ActionCategory("Hollow Knight")]
	public class SimulateDeathNoCorpse : FsmStateAction
	{
		// Token: 0x060087D9 RID: 34777 RVA: 0x002781AC File Offset: 0x002763AC
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x060087DA RID: 34778 RVA: 0x002781B8 File Offset: 0x002763B8
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				if (component != null)
				{
					component.EmitEffects(null);
					component.RecordKillForJournal();
				}
			}
			base.Finish();
		}

		// Token: 0x0400884E RID: 34894
		public FsmOwnerDefault Target;
	}
}
