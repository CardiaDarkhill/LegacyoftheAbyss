using System;
using UnityEngine;

// Token: 0x020003B5 RID: 949
public class HeroControllerConfigWarrior : HeroControllerConfig
{
	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06001FE9 RID: 8169 RVA: 0x000913BD File Offset: 0x0008F5BD
	public override float AttackDuration
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.AttackDuration;
			}
			return this.rageAttackDuration;
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06001FEA RID: 8170 RVA: 0x000913DD File Offset: 0x0008F5DD
	public override float AttackRecoveryTime
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.AttackRecoveryTime;
			}
			return this.rageAttackRecoveryTime;
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06001FEB RID: 8171 RVA: 0x000913FD File Offset: 0x0008F5FD
	public override float QuickAttackCooldownTime
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.QuickAttackCooldownTime;
			}
			return this.rageQuickAttackCooldownTime;
		}
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x06001FEC RID: 8172 RVA: 0x0009141D File Offset: 0x0008F61D
	public override float AttackCooldownTime
	{
		get
		{
			if (!HeroController.instance.WarriorState.IsInRageMode)
			{
				return base.AttackCooldownTime;
			}
			return this.rageAttackCooldownTime;
		}
	}

	// Token: 0x04001EF0 RID: 7920
	[Header("Warrior")]
	[SerializeField]
	private float rageAttackDuration;

	// Token: 0x04001EF1 RID: 7921
	[SerializeField]
	private float rageAttackRecoveryTime;

	// Token: 0x04001EF2 RID: 7922
	[SerializeField]
	private float rageAttackCooldownTime;

	// Token: 0x04001EF3 RID: 7923
	[SerializeField]
	private float rageQuickAttackCooldownTime;
}
