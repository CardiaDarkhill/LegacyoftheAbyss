using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Token: 0x0200018D RID: 397
[Serializable]
public class HeroControllerStates
{
	// Token: 0x17000180 RID: 384
	// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0004A822 File Offset: 0x00048A22
	public bool Invulnerable
	{
		get
		{
			return this.invulnerable || this.invulnerableCount > 0;
		}
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0004A837 File Offset: 0x00048A37
	public HeroControllerStates()
	{
		this.facingRight = false;
		if (HeroControllerStates.boolFieldAccessOptimizer == null)
		{
			HeroControllerStates.boolFieldAccessOptimizer = new BoolFieldAccessOptimizer<HeroControllerStates>();
		}
		this.Reset();
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x0004A868 File Offset: 0x00048A68
	public bool GetState(string stateName)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return HeroControllerStates.boolFieldAccessOptimizer.GetField(this, stateName);
		}
		FieldInfo field = base.GetType().GetField(stateName);
		if (field != null)
		{
			return (bool)field.GetValue(HeroController.instance.cState);
		}
		Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
		return false;
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x0004A8CC File Offset: 0x00048ACC
	public void SetState(string stateName, bool value)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			HeroControllerStates.boolFieldAccessOptimizer.SetField(this, stateName, value);
			return;
		}
		FieldInfo field = base.GetType().GetField(stateName);
		if (field != null)
		{
			try
			{
				field.SetValue(HeroController.instance.cState, value);
				return;
			}
			catch (Exception ex)
			{
				string str = "Failed to set cState: ";
				Exception ex2 = ex;
				Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
				return;
			}
		}
		Debug.LogError("HeroControllerStates: Could not find bool named" + stateName + "in cState");
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x0004A960 File Offset: 0x00048B60
	public static bool CStateExists(string stateName)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return HeroControllerStates.boolFieldAccessOptimizer.FieldExists(typeof(HeroControllerStates), stateName);
		}
		return typeof(HeroControllerStates).GetField(stateName) != null;
	}

	// Token: 0x06000F75 RID: 3957 RVA: 0x0004A998 File Offset: 0x00048B98
	public void Reset()
	{
		this.onGround = false;
		this.jumping = false;
		this.falling = false;
		this.dashing = false;
		this.isSprinting = false;
		this.isBackSprinting = false;
		this.backDashing = false;
		this.touchingWall = false;
		this.wallSliding = false;
		this.wallClinging = false;
		this.transitioning = false;
		this.attacking = false;
		this.lookingUp = false;
		this.lookingDown = false;
		this.altAttack = false;
		this.upAttacking = false;
		this.downAttacking = false;
		this.downTravelling = false;
		this.bouncing = false;
		this.dead = false;
		this.isFrostDeath = false;
		this.hazardDeath = false;
		this.willHardLand = false;
		this.recoiling = false;
		this.recoilFrozen = false;
		this.invulnerable = false;
		this.casting = false;
		this.castRecoiling = false;
		this.preventDash = false;
		this.preventBackDash = false;
		this.dashCooldown = false;
		this.backDashCooldown = false;
		this.attackCount = 0;
		this.downspikeInvulnerabilitySteps = 0;
		this.isToolThrowing = false;
		this.toolThrowCount = 0;
		this.throwingToolVertical = 0;
		this.isInCancelableFSMMove = false;
		this.mantling = false;
		this.isBinding = false;
		this.needolinPlayingMemory = false;
		this.isScrewDownAttacking = false;
		this.evading = false;
		this.fakeHurt = false;
		this.invulnerabilitySources.Clear();
		this.invulnerableCount = 0;
		this.isTriggerEventsPaused = false;
		this.isInCutsceneMovement = false;
	}

	// Token: 0x06000F76 RID: 3958 RVA: 0x0004AAF9 File Offset: 0x00048CF9
	public void ClearInvulnerabilitySources()
	{
		this.invulnerabilitySources.Clear();
		this.invulnerableCount = 0;
	}

	// Token: 0x06000F77 RID: 3959 RVA: 0x0004AB0D File Offset: 0x00048D0D
	public void AddInvulnerabilitySource(object source)
	{
		if (source == null)
		{
			return;
		}
		this.invulnerabilitySources.Add(source);
		this.invulnerableCount = this.invulnerabilitySources.Count;
	}

	// Token: 0x06000F78 RID: 3960 RVA: 0x0004AB31 File Offset: 0x00048D31
	public void RemoveInvulnerabilitySource(object source)
	{
		if (this.invulnerabilitySources.Remove(source))
		{
			this.invulnerableCount = this.invulnerabilitySources.Count;
		}
	}

	// Token: 0x04000EB9 RID: 3769
	public bool facingRight;

	// Token: 0x04000EBA RID: 3770
	public bool onGround;

	// Token: 0x04000EBB RID: 3771
	public bool jumping;

	// Token: 0x04000EBC RID: 3772
	public bool shuttleCock;

	// Token: 0x04000EBD RID: 3773
	public bool floating;

	// Token: 0x04000EBE RID: 3774
	public bool wallJumping;

	// Token: 0x04000EBF RID: 3775
	public bool doubleJumping;

	// Token: 0x04000EC0 RID: 3776
	public bool nailCharging;

	// Token: 0x04000EC1 RID: 3777
	public bool shadowDashing;

	// Token: 0x04000EC2 RID: 3778
	public bool swimming;

	// Token: 0x04000EC3 RID: 3779
	public bool falling;

	// Token: 0x04000EC4 RID: 3780
	public bool dashing;

	// Token: 0x04000EC5 RID: 3781
	public bool isSprinting;

	// Token: 0x04000EC6 RID: 3782
	public bool isBackSprinting;

	// Token: 0x04000EC7 RID: 3783
	public bool isBackScuttling;

	// Token: 0x04000EC8 RID: 3784
	public bool airDashing;

	// Token: 0x04000EC9 RID: 3785
	public bool superDashing;

	// Token: 0x04000ECA RID: 3786
	public bool superDashOnWall;

	// Token: 0x04000ECB RID: 3787
	public bool backDashing;

	// Token: 0x04000ECC RID: 3788
	public bool touchingWall;

	// Token: 0x04000ECD RID: 3789
	public bool wallSliding;

	// Token: 0x04000ECE RID: 3790
	public bool wallClinging;

	// Token: 0x04000ECF RID: 3791
	public bool wallScrambling;

	// Token: 0x04000ED0 RID: 3792
	public bool transitioning;

	// Token: 0x04000ED1 RID: 3793
	public bool attacking;

	// Token: 0x04000ED2 RID: 3794
	public int attackCount;

	// Token: 0x04000ED3 RID: 3795
	public bool lookingUp;

	// Token: 0x04000ED4 RID: 3796
	public bool lookingDown;

	// Token: 0x04000ED5 RID: 3797
	public bool lookingUpRing;

	// Token: 0x04000ED6 RID: 3798
	public bool lookingDownRing;

	// Token: 0x04000ED7 RID: 3799
	public bool lookingUpAnim;

	// Token: 0x04000ED8 RID: 3800
	public bool lookingDownAnim;

	// Token: 0x04000ED9 RID: 3801
	public bool altAttack;

	// Token: 0x04000EDA RID: 3802
	public bool upAttacking;

	// Token: 0x04000EDB RID: 3803
	public bool downAttacking;

	// Token: 0x04000EDC RID: 3804
	public bool downTravelling;

	// Token: 0x04000EDD RID: 3805
	public bool downSpikeAntic;

	// Token: 0x04000EDE RID: 3806
	public bool downSpiking;

	// Token: 0x04000EDF RID: 3807
	public bool downSpikeBouncing;

	// Token: 0x04000EE0 RID: 3808
	public bool downSpikeBouncingShort;

	// Token: 0x04000EE1 RID: 3809
	public bool downSpikeRecovery;

	// Token: 0x04000EE2 RID: 3810
	public bool bouncing;

	// Token: 0x04000EE3 RID: 3811
	public bool shroomBouncing;

	// Token: 0x04000EE4 RID: 3812
	public bool recoilingRight;

	// Token: 0x04000EE5 RID: 3813
	public bool recoilingLeft;

	// Token: 0x04000EE6 RID: 3814
	public bool recoilingDrill;

	// Token: 0x04000EE7 RID: 3815
	public bool dead;

	// Token: 0x04000EE8 RID: 3816
	public bool isFrostDeath;

	// Token: 0x04000EE9 RID: 3817
	public bool hazardDeath;

	// Token: 0x04000EEA RID: 3818
	public bool hazardRespawning;

	// Token: 0x04000EEB RID: 3819
	public bool willHardLand;

	// Token: 0x04000EEC RID: 3820
	public bool recoilFrozen;

	// Token: 0x04000EED RID: 3821
	public bool recoiling;

	// Token: 0x04000EEE RID: 3822
	public bool invulnerable;

	// Token: 0x04000EEF RID: 3823
	private int invulnerableCount;

	// Token: 0x04000EF0 RID: 3824
	public bool casting;

	// Token: 0x04000EF1 RID: 3825
	public bool castRecoiling;

	// Token: 0x04000EF2 RID: 3826
	public bool preventDash;

	// Token: 0x04000EF3 RID: 3827
	public bool preventBackDash;

	// Token: 0x04000EF4 RID: 3828
	public bool dashCooldown;

	// Token: 0x04000EF5 RID: 3829
	public bool backDashCooldown;

	// Token: 0x04000EF6 RID: 3830
	public bool nearBench;

	// Token: 0x04000EF7 RID: 3831
	public bool inWalkZone;

	// Token: 0x04000EF8 RID: 3832
	public bool isPaused;

	// Token: 0x04000EF9 RID: 3833
	public bool onConveyor;

	// Token: 0x04000EFA RID: 3834
	public bool onConveyorV;

	// Token: 0x04000EFB RID: 3835
	public bool inConveyorZone;

	// Token: 0x04000EFC RID: 3836
	public bool spellQuake;

	// Token: 0x04000EFD RID: 3837
	public bool freezeCharge;

	// Token: 0x04000EFE RID: 3838
	public bool focusing;

	// Token: 0x04000EFF RID: 3839
	public bool inAcid;

	// Token: 0x04000F00 RID: 3840
	public bool touchingNonSlider;

	// Token: 0x04000F01 RID: 3841
	public bool wasOnGround;

	// Token: 0x04000F02 RID: 3842
	public bool parrying;

	// Token: 0x04000F03 RID: 3843
	public bool parryAttack;

	// Token: 0x04000F04 RID: 3844
	public bool mantling;

	// Token: 0x04000F05 RID: 3845
	public bool mantleRecovery;

	// Token: 0x04000F06 RID: 3846
	public bool inUpdraft;

	// Token: 0x04000F07 RID: 3847
	public int downspikeInvulnerabilitySteps;

	// Token: 0x04000F08 RID: 3848
	public bool isToolThrowing;

	// Token: 0x04000F09 RID: 3849
	public int toolThrowCount;

	// Token: 0x04000F0A RID: 3850
	public int throwingToolVertical;

	// Token: 0x04000F0B RID: 3851
	public bool isInCancelableFSMMove;

	// Token: 0x04000F0C RID: 3852
	public bool inWindRegion;

	// Token: 0x04000F0D RID: 3853
	public bool isMaggoted;

	// Token: 0x04000F0E RID: 3854
	public bool inFrostRegion;

	// Token: 0x04000F0F RID: 3855
	public bool isFrosted;

	// Token: 0x04000F10 RID: 3856
	public bool isTouchingSlopeLeft;

	// Token: 0x04000F11 RID: 3857
	public bool isTouchingSlopeRight;

	// Token: 0x04000F12 RID: 3858
	public bool isBinding;

	// Token: 0x04000F13 RID: 3859
	public bool needolinPlayingMemory;

	// Token: 0x04000F14 RID: 3860
	public bool isScrewDownAttacking;

	// Token: 0x04000F15 RID: 3861
	public bool evading;

	// Token: 0x04000F16 RID: 3862
	public bool whipLashing;

	// Token: 0x04000F17 RID: 3863
	public bool fakeHurt;

	// Token: 0x04000F18 RID: 3864
	public bool isInCutsceneMovement;

	// Token: 0x04000F19 RID: 3865
	public bool isTriggerEventsPaused;

	// Token: 0x04000F1A RID: 3866
	private static BoolFieldAccessOptimizer<HeroControllerStates> boolFieldAccessOptimizer;

	// Token: 0x04000F1B RID: 3867
	private static Dictionary<string, FieldInfo> fieldCache = new Dictionary<string, FieldInfo>();

	// Token: 0x04000F1C RID: 3868
	private HashSet<object> invulnerabilitySources = new HashSet<object>();
}
