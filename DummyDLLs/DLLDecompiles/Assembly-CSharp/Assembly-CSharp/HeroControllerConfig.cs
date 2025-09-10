using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003B4 RID: 948
[CreateAssetMenu(menuName = "Hornet/HeroController Config")]
public class HeroControllerConfig : ScriptableObject, IIncludeVariableExtensions
{
	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001FC2 RID: 8130 RVA: 0x00091239 File Offset: 0x0008F439
	public bool CanPlayNeedolin
	{
		get
		{
			return this.canPlayNeedolin;
		}
	}

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x06001FC3 RID: 8131 RVA: 0x00091241 File Offset: 0x0008F441
	public bool CanBrolly
	{
		get
		{
			return this.canBrolly;
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06001FC4 RID: 8132 RVA: 0x00091249 File Offset: 0x0008F449
	public bool CanDoubleJump
	{
		get
		{
			return this.canDoubleJump;
		}
	}

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001FC5 RID: 8133 RVA: 0x00091251 File Offset: 0x0008F451
	public bool CanNailCharge
	{
		get
		{
			return this.canNailCharge;
		}
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06001FC6 RID: 8134 RVA: 0x00091259 File Offset: 0x0008F459
	public bool CanBind
	{
		get
		{
			return this.canBind;
		}
	}

	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001FC7 RID: 8135 RVA: 0x00091261 File Offset: 0x0008F461
	public bool CanHarpoonDash
	{
		get
		{
			return this.canHarpoonDash;
		}
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001FC8 RID: 8136 RVA: 0x00091269 File Offset: 0x0008F469
	public bool ForceBareInventory
	{
		get
		{
			return this.forceBareInventory;
		}
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001FC9 RID: 8137 RVA: 0x00091271 File Offset: 0x0008F471
	public HeroControllerConfig.DownSlashTypes DownSlashType
	{
		get
		{
			return this.downSlashType;
		}
	}

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001FCA RID: 8138 RVA: 0x00091279 File Offset: 0x0008F479
	public string DownSlashEvent
	{
		get
		{
			return this.downSlashEvent;
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001FCB RID: 8139 RVA: 0x00091281 File Offset: 0x0008F481
	public float DownSpikeAnticTime
	{
		get
		{
			return this.downspikeAnticTime;
		}
	}

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001FCC RID: 8140 RVA: 0x00091289 File Offset: 0x0008F489
	public float DownSpikeTime
	{
		get
		{
			return this.downspikeTime;
		}
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001FCD RID: 8141 RVA: 0x00091291 File Offset: 0x0008F491
	public float DownspikeSpeed
	{
		get
		{
			return this.downspikeSpeed;
		}
	}

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001FCE RID: 8142 RVA: 0x00091299 File Offset: 0x0008F499
	public float DownspikeRecoveryTime
	{
		get
		{
			return this.downspikeRecoveryTime;
		}
	}

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001FCF RID: 8143 RVA: 0x000912A1 File Offset: 0x0008F4A1
	public bool DownspikeBurstEffect
	{
		get
		{
			return this.downspikeBurstEffect;
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001FD0 RID: 8144 RVA: 0x000912A9 File Offset: 0x0008F4A9
	public bool DownspikeThrusts
	{
		get
		{
			return this.downspikeThrusts;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x000912B1 File Offset: 0x0008F4B1
	public float DashStabSpeed
	{
		get
		{
			return this.dashStabSpeed;
		}
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001FD2 RID: 8146 RVA: 0x000912B9 File Offset: 0x0008F4B9
	public float DashStabTime
	{
		get
		{
			return this.dashStabTime;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001FD3 RID: 8147 RVA: 0x000912C1 File Offset: 0x0008F4C1
	public bool ForceShortDashStabBounce
	{
		get
		{
			return this.forceShortDashStabBounce;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001FD4 RID: 8148 RVA: 0x000912C9 File Offset: 0x0008F4C9
	public float DashStabBounceJumpSpeed
	{
		get
		{
			return this.dashStabBounceJumpSpeed;
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001FD5 RID: 8149 RVA: 0x000912D1 File Offset: 0x0008F4D1
	public int DashStabSteps
	{
		get
		{
			return this.dashStabSteps;
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001FD6 RID: 8150 RVA: 0x000912D9 File Offset: 0x0008F4D9
	public virtual float AttackDuration
	{
		get
		{
			return this.attackDuration;
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001FD7 RID: 8151 RVA: 0x000912E1 File Offset: 0x0008F4E1
	public virtual float QuickAttackSpeedMult
	{
		get
		{
			return this.quickAttackSpeedMult;
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06001FD8 RID: 8152 RVA: 0x000912E9 File Offset: 0x0008F4E9
	public virtual float AttackRecoveryTime
	{
		get
		{
			return this.attackRecoveryTime;
		}
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06001FD9 RID: 8153 RVA: 0x000912F1 File Offset: 0x0008F4F1
	public virtual float AttackCooldownTime
	{
		get
		{
			return this.attackCooldownTime;
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06001FDA RID: 8154 RVA: 0x000912F9 File Offset: 0x0008F4F9
	public virtual float QuickAttackCooldownTime
	{
		get
		{
			return this.quickAttackCooldownTime;
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06001FDB RID: 8155 RVA: 0x00091301 File Offset: 0x0008F501
	public bool CanTurnWhileSlashing
	{
		get
		{
			return this.canTurnWhileSlashing;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06001FDC RID: 8156 RVA: 0x00091309 File Offset: 0x0008F509
	public bool ChargeSlashRecoils
	{
		get
		{
			return this.chargeSlashRecoils;
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06001FDD RID: 8157 RVA: 0x00091311 File Offset: 0x0008F511
	public float ChargeSlashLungeSpeed
	{
		get
		{
			return this.chargeSlashLungeSpeed;
		}
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06001FDE RID: 8158 RVA: 0x00091319 File Offset: 0x0008F519
	public float ChargeSlashLungeDeceleration
	{
		get
		{
			return this.chargeSlashLungeDeceleration;
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06001FDF RID: 8159 RVA: 0x00091321 File Offset: 0x0008F521
	public int ChargeSlashChain
	{
		get
		{
			return this.chargeSlashChain;
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06001FE0 RID: 8160 RVA: 0x00091329 File Offset: 0x0008F529
	public bool WallSlashSlowdown
	{
		get
		{
			return this.wallSlashSlowdown;
		}
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x00091331 File Offset: 0x0008F531
	private bool IsDownSlashTypeDownSpike()
	{
		return this.downSlashType == HeroControllerConfig.DownSlashTypes.DownSpike;
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x0009133C File Offset: 0x0008F53C
	private bool ShowDownSlashSpeed()
	{
		return this.downspikeThrusts && this.IsDownSlashTypeDownSpike();
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x0009134E File Offset: 0x0008F54E
	private bool IsDownSlashTypeSlash()
	{
		return this.downSlashType == HeroControllerConfig.DownSlashTypes.Slash;
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x00091359 File Offset: 0x0008F559
	private bool IsDownSlashTypeCustom()
	{
		return this.downSlashType == HeroControllerConfig.DownSlashTypes.Custom;
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x00091364 File Offset: 0x0008F564
	private void OnValidate()
	{
		if (this.dashStabSteps < 1)
		{
			this.dashStabSteps = 1;
		}
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x00091376 File Offset: 0x0008F576
	public tk2dSpriteAnimationClip GetAnimationClip(string clipName)
	{
		if (!this.heroAnimOverrideLib)
		{
			return null;
		}
		return this.heroAnimOverrideLib.GetClipByName(clipName);
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x00091393 File Offset: 0x0008F593
	public void OnUpdatedVariable(string variableName)
	{
	}

	// Token: 0x04001ED0 RID: 7888
	[Header("Animation")]
	[SerializeField]
	private tk2dSpriteAnimation heroAnimOverrideLib;

	// Token: 0x04001ED1 RID: 7889
	[Header("Abilities")]
	[SerializeField]
	private bool canPlayNeedolin;

	// Token: 0x04001ED2 RID: 7890
	[SerializeField]
	private bool canBrolly;

	// Token: 0x04001ED3 RID: 7891
	[SerializeField]
	private bool canDoubleJump;

	// Token: 0x04001ED4 RID: 7892
	[SerializeField]
	private bool canNailCharge;

	// Token: 0x04001ED5 RID: 7893
	[SerializeField]
	private bool canBind;

	// Token: 0x04001ED6 RID: 7894
	[SerializeField]
	private bool canHarpoonDash;

	// Token: 0x04001ED7 RID: 7895
	[Header("UI")]
	[SerializeField]
	private bool forceBareInventory;

	// Token: 0x04001ED8 RID: 7896
	[Header("\"Constants\"")]
	[SerializeField]
	private HeroControllerConfig.DownSlashTypes downSlashType;

	// Token: 0x04001ED9 RID: 7897
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDownSlashTypeCustom", true, true, true)]
	[InspectorValidation]
	private string downSlashEvent;

	// Token: 0x04001EDA RID: 7898
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDownSlashTypeDownSpike", true, true, true)]
	private float downspikeAnticTime;

	// Token: 0x04001EDB RID: 7899
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDownSlashTypeDownSpike", true, true, true)]
	private float downspikeTime;

	// Token: 0x04001EDC RID: 7900
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDownSlashSpeed", true, true, true)]
	private float downspikeSpeed;

	// Token: 0x04001EDD RID: 7901
	[SerializeField]
	private float downspikeRecoveryTime;

	// Token: 0x04001EDE RID: 7902
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDownSlashTypeDownSpike", true, true, true)]
	private bool downspikeBurstEffect = true;

	// Token: 0x04001EDF RID: 7903
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDownSlashTypeDownSpike", true, true, true)]
	private bool downspikeThrusts = true;

	// Token: 0x04001EE0 RID: 7904
	[Space]
	[SerializeField]
	private float dashStabSpeed;

	// Token: 0x04001EE1 RID: 7905
	[SerializeField]
	private float dashStabTime;

	// Token: 0x04001EE2 RID: 7906
	[SerializeField]
	private bool forceShortDashStabBounce;

	// Token: 0x04001EE3 RID: 7907
	[SerializeField]
	private float dashStabBounceJumpSpeed;

	// Token: 0x04001EE4 RID: 7908
	[SerializeField]
	private int dashStabSteps = 1;

	// Token: 0x04001EE5 RID: 7909
	[Space]
	[SerializeField]
	private float attackDuration;

	// Token: 0x04001EE6 RID: 7910
	[SerializeField]
	private float quickAttackSpeedMult;

	// Token: 0x04001EE7 RID: 7911
	[SerializeField]
	private float attackRecoveryTime;

	// Token: 0x04001EE8 RID: 7912
	[SerializeField]
	private float attackCooldownTime;

	// Token: 0x04001EE9 RID: 7913
	[SerializeField]
	private float quickAttackCooldownTime;

	// Token: 0x04001EEA RID: 7914
	[SerializeField]
	private bool canTurnWhileSlashing;

	// Token: 0x04001EEB RID: 7915
	[Space]
	[SerializeField]
	private bool chargeSlashRecoils;

	// Token: 0x04001EEC RID: 7916
	[SerializeField]
	private float chargeSlashLungeSpeed;

	// Token: 0x04001EED RID: 7917
	[SerializeField]
	private float chargeSlashLungeDeceleration = 1f;

	// Token: 0x04001EEE RID: 7918
	[SerializeField]
	private int chargeSlashChain;

	// Token: 0x04001EEF RID: 7919
	[SerializeField]
	private bool wallSlashSlowdown;

	// Token: 0x02001669 RID: 5737
	public enum DownSlashTypes
	{
		// Token: 0x04008AB3 RID: 35507
		DownSpike,
		// Token: 0x04008AB4 RID: 35508
		Slash,
		// Token: 0x04008AB5 RID: 35509
		Custom
	}
}
