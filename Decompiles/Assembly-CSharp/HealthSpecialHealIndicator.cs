using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000088 RID: 136
public class HealthSpecialHealIndicator : MonoBehaviour
{
	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060003CB RID: 971 RVA: 0x00012E84 File Offset: 0x00011084
	private int CurrentHealCap
	{
		get
		{
			HealthSpecialHealIndicator.HealCapTypes healCapTypes = this.healCapType;
			int result;
			if (healCapTypes != HealthSpecialHealIndicator.HealCapTypes.Warrior)
			{
				if (healCapTypes != HealthSpecialHealIndicator.HealCapTypes.Witch)
				{
					throw new ArgumentOutOfRangeException();
				}
				result = this.hc.GetWitchHealCap();
			}
			else
			{
				result = this.hc.GetRageModeHealCap() - this.hc.WarriorState.RageModeHealCount;
			}
			return result;
		}
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060003CC RID: 972 RVA: 0x00012ED8 File Offset: 0x000110D8
	private bool ShouldBeUp
	{
		get
		{
			if (!this.isStateActive)
			{
				return false;
			}
			PlayerData playerData = this.hc.playerData;
			int health = playerData.health;
			int currentMaxHealth = playerData.CurrentMaxHealth;
			if (this.healthNumber <= health || this.healthNumber > currentMaxHealth)
			{
				return false;
			}
			HealthSpecialHealIndicator.HealCapTypes healCapTypes = this.healCapType;
			int num;
			if (healCapTypes != HealthSpecialHealIndicator.HealCapTypes.Warrior)
			{
				if (healCapTypes != HealthSpecialHealIndicator.HealCapTypes.Witch)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = this.frozenHealTarget;
			}
			else
			{
				num = health + this.CurrentHealCap;
			}
			int num2 = num;
			return this.healthNumber <= num2;
		}
	}

	// Token: 0x060003CD RID: 973 RVA: 0x00012F58 File Offset: 0x00011158
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshRenderer.enabled = false;
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		FsmInt fsmInt = FSMUtility.LocateFSM(base.transform.parent.gameObject, "health_display").FsmVariables.FindFsmInt("Health Number");
		this.healthNumber = fsmInt.Value;
		this.hc = HeroController.instance;
		this.hc.OnTakenDamage += this.OnUpdateEvent;
		this.appearEvent.ReceivedEvent += delegate()
		{
			if (this.healCapType == HealthSpecialHealIndicator.HealCapTypes.Witch && this.hc.cState.isMaggoted)
			{
				return;
			}
			this.isStateActive = true;
			this.frozenHealTarget = this.hc.playerData.health + this.CurrentHealCap;
			this.OnUpdateEvent();
		};
		EventRegister[] array = this.updateEvents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ReceivedEvent += this.OnUpdateEvent;
		}
		this.disappearEvent.ReceivedEvent += delegate()
		{
			this.isStateActive = false;
			this.OnUpdateEvent();
		};
	}

	// Token: 0x060003CE RID: 974 RVA: 0x0001305F File Offset: 0x0001125F
	private void OnDestroy()
	{
		if (this.hc)
		{
			this.hc.OnTakenDamage -= this.OnUpdateEvent;
			this.hc = null;
		}
	}

	// Token: 0x060003CF RID: 975 RVA: 0x0001308C File Offset: 0x0001128C
	private void OnUpdateEvent()
	{
		if (this.ShouldBeUp)
		{
			if (!this.isUp)
			{
				this.isUp = true;
				this.meshRenderer.enabled = true;
				this.animator.Play(this.upAnim);
				return;
			}
		}
		else if (this.isUp)
		{
			this.isUp = false;
			this.animator.Play(this.downAnim);
		}
	}

	// Token: 0x060003D0 RID: 976 RVA: 0x000130EE File Offset: 0x000112EE
	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == this.downAnim)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x0400036A RID: 874
	[SerializeField]
	private EventRegister appearEvent;

	// Token: 0x0400036B RID: 875
	[SerializeField]
	private EventRegister[] updateEvents;

	// Token: 0x0400036C RID: 876
	[SerializeField]
	private EventRegister disappearEvent;

	// Token: 0x0400036D RID: 877
	[SerializeField]
	private HealthSpecialHealIndicator.HealCapTypes healCapType;

	// Token: 0x0400036E RID: 878
	[SerializeField]
	private string upAnim;

	// Token: 0x0400036F RID: 879
	[SerializeField]
	private string downAnim;

	// Token: 0x04000370 RID: 880
	private MeshRenderer meshRenderer;

	// Token: 0x04000371 RID: 881
	private tk2dSpriteAnimator animator;

	// Token: 0x04000372 RID: 882
	private int healthNumber;

	// Token: 0x04000373 RID: 883
	private HeroController hc;

	// Token: 0x04000374 RID: 884
	private bool isStateActive;

	// Token: 0x04000375 RID: 885
	private bool isUp;

	// Token: 0x04000376 RID: 886
	private int frozenHealTarget;

	// Token: 0x020013FE RID: 5118
	private enum HealCapTypes
	{
		// Token: 0x04008172 RID: 33138
		Warrior,
		// Token: 0x04008173 RID: 33139
		Witch
	}
}
