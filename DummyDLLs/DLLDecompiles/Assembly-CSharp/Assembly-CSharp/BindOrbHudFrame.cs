using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200060B RID: 1547
public class BindOrbHudFrame : MonoBehaviour
{
	// Token: 0x17000656 RID: 1622
	// (get) Token: 0x06003734 RID: 14132 RVA: 0x000F38EA File Offset: 0x000F1AEA
	// (set) Token: 0x06003735 RID: 14133 RVA: 0x000F38F1 File Offset: 0x000F1AF1
	public static bool SkipToNextAppear { get; set; }

	// Token: 0x17000657 RID: 1623
	// (get) Token: 0x06003736 RID: 14134 RVA: 0x000F38F9 File Offset: 0x000F1AF9
	// (set) Token: 0x06003737 RID: 14135 RVA: 0x000F3900 File Offset: 0x000F1B00
	public static bool ForceNextInstant { get; set; }

	// Token: 0x06003738 RID: 14136 RVA: 0x000F3908 File Offset: 0x000F1B08
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.animProxy = base.GetComponent<SteelSoulAnimProxy>();
		EventRegister.GetRegisterGuaranteed(base.gameObject, "POST TOOL EQUIPS CHANGED").ReceivedEvent += delegate()
		{
			this.Refresh(false, false);
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOLMASTER QUICK CRAFTING").ReceivedEvent += delegate()
		{
			this.queuedToolmasterSpin = true;
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HEALTH UPDATE").ReceivedEvent += this.RefreshLifebloodTint;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "CHARM INDICATOR CHECK").ReceivedEvent += this.RefreshLifebloodTint;
	}

	// Token: 0x06003739 RID: 14137 RVA: 0x000F39B4 File Offset: 0x000F1BB4
	private void OnEnable()
	{
		BindOrbHudFrame.MeterAnims[] array = this.warriorMeterAnims;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (GameObject gameObject in array[i].IncreaseEffects)
			{
				if (gameObject)
				{
					gameObject.SetActive(false);
				}
			}
		}
		this.hunterV2Bar.gameObject.SetActive(false);
		this.hunterV3BarA.gameObject.SetActive(false);
		this.hunterV3BarB.gameObject.SetActive(false);
		this.hunterV3ExtraHitEffect.SetActive(false);
	}

	// Token: 0x0600373A RID: 14138 RVA: 0x000F3A42 File Offset: 0x000F1C42
	private void OnDisable()
	{
		if (this.animRoutine != null)
		{
			base.StopCoroutine(this.animRoutine);
			this.animRoutine = null;
		}
		this.isActive = false;
	}

	// Token: 0x0600373B RID: 14139 RVA: 0x000F3A66 File Offset: 0x000F1C66
	public void FirstAppear()
	{
		if (this.isActive || this.animRoutine != null)
		{
			return;
		}
		this.isActive = true;
		this.Refresh(false, true);
	}

	// Token: 0x0600373C RID: 14140 RVA: 0x000F3A88 File Offset: 0x000F1C88
	public void AlreadyAppeared()
	{
		if (this.isActive || this.animRoutine != null)
		{
			return;
		}
		this.isActive = true;
		this.Refresh(true, false);
	}

	// Token: 0x0600373D RID: 14141 RVA: 0x000F3AAA File Offset: 0x000F1CAA
	public void Disappeared()
	{
		this.isActive = false;
		if (this.animRoutine != null)
		{
			base.StopCoroutine(this.animRoutine);
			this.animRoutine = null;
		}
	}

	// Token: 0x0600373E RID: 14142 RVA: 0x000F3ACE File Offset: 0x000F1CCE
	private void Refresh(bool isInstant, bool isFirst)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (!this.DoChangeFrame(isInstant, isFirst))
		{
			this.ChangeEnded();
		}
	}

	// Token: 0x0600373F RID: 14143 RVA: 0x000F3AEC File Offset: 0x000F1CEC
	private void RefreshLifebloodTint()
	{
		tk2dSprite tk2dSprite = this.animator.Sprite as tk2dSprite;
		if (!tk2dSprite)
		{
			return;
		}
		if (HeroController.instance.IsInLifebloodState)
		{
			tk2dSprite.color = this.lifebloodTint;
			tk2dSprite.EnableKeyword("RECOLOUR");
			return;
		}
		this.animator.Sprite.color = Color.white;
		tk2dSprite.DisableKeyword("RECOLOUR");
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x000F3B58 File Offset: 0x000F1D58
	private bool DoChangeFrame(bool isInstant, bool isFirst)
	{
		BindOrbHudFrame.<>c__DisplayClass68_0 CS$<>8__locals1 = new BindOrbHudFrame.<>c__DisplayClass68_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.isInstant = isInstant;
		CS$<>8__locals1.isFirst = isFirst;
		if (!this.isActive && !CS$<>8__locals1.isFirst)
		{
			return false;
		}
		if (!CS$<>8__locals1.isInstant && BindOrbHudFrame.ForceNextInstant)
		{
			CS$<>8__locals1.isInstant = true;
		}
		ToolCrest hunterCrest = Gameplay.HunterCrest;
		ToolCrest hunterCrest2 = Gameplay.HunterCrest2;
		ToolCrest hunterCrest3 = Gameplay.HunterCrest3;
		ToolCrest cloaklessCrest = Gameplay.CloaklessCrest;
		ToolCrest warriorCrest = Gameplay.WarriorCrest;
		ToolCrest reaperCrest = Gameplay.ReaperCrest;
		ToolCrest wandererCrest = Gameplay.WandererCrest;
		ToolCrest cursedCrest = Gameplay.CursedCrest;
		ToolCrest witchCrest = Gameplay.WitchCrest;
		ToolCrest toolmasterCrest = Gameplay.ToolmasterCrest;
		ToolCrest spellCrest = Gameplay.SpellCrest;
		CS$<>8__locals1.newFrameAnims = null;
		CS$<>8__locals1.customAnimRoutine = null;
		this.isCursed = false;
		if (hunterCrest.IsEquipped)
		{
			if (this.currentFrameCrest == hunterCrest)
			{
				return false;
			}
			this.currentFrameCrest = hunterCrest;
			CS$<>8__locals1.newFrameAnims = this.defaultFrameAnims;
		}
		else if (hunterCrest2.IsEquipped)
		{
			if (this.currentFrameCrest == hunterCrest2)
			{
				return false;
			}
			this.currentFrameCrest = hunterCrest2;
			CS$<>8__locals1.newFrameAnims = this.hunterV2FrameAnims;
			CS$<>8__locals1.customAnimRoutine = new BindOrbHudFrame.CoroutineFunction(this.HunterCrestV2Routine);
		}
		else if (hunterCrest3.IsEquipped)
		{
			if (this.currentFrameCrest == hunterCrest3)
			{
				return false;
			}
			this.currentFrameCrest = hunterCrest3;
			CS$<>8__locals1.newFrameAnims = this.hunterV3FrameAnims;
			CS$<>8__locals1.customAnimRoutine = new BindOrbHudFrame.CoroutineFunction(this.HunterCrestV3Routine);
		}
		else if (cloaklessCrest.IsEquipped)
		{
			if (this.currentFrameCrest == cloaklessCrest)
			{
				return false;
			}
			this.currentFrameCrest = cloaklessCrest;
			CS$<>8__locals1.newFrameAnims = this.cloaklessFrameAnims;
		}
		else if (warriorCrest.IsEquipped)
		{
			if (this.currentFrameCrest == warriorCrest)
			{
				return false;
			}
			this.currentFrameCrest = warriorCrest;
			CS$<>8__locals1.newFrameAnims = this.warriorFrameAnims;
			CS$<>8__locals1.customAnimRoutine = new BindOrbHudFrame.CoroutineFunction(this.WarriorCrestRoutine);
		}
		else if (reaperCrest.IsEquipped)
		{
			if (this.currentFrameCrest == reaperCrest)
			{
				return false;
			}
			this.currentFrameCrest = reaperCrest;
			CS$<>8__locals1.newFrameAnims = this.reaperFrameAnims;
			CS$<>8__locals1.customAnimRoutine = new BindOrbHudFrame.CoroutineFunction(this.ReaperCrestRoutine);
		}
		else if (wandererCrest.IsEquipped)
		{
			if (this.currentFrameCrest == wandererCrest)
			{
				return false;
			}
			this.currentFrameCrest = wandererCrest;
			CS$<>8__locals1.newFrameAnims = this.wandererFrameAnims;
			CS$<>8__locals1.customAnimRoutine = new BindOrbHudFrame.CoroutineFunction(this.WandererCrestRoutine);
		}
		else if (cursedCrest.IsEquipped)
		{
			if (!CS$<>8__locals1.isFirst && this.currentFrameCrest == cursedCrest)
			{
				return false;
			}
			this.currentFrameCrest = cursedCrest;
			CS$<>8__locals1.newFrameAnims = this.cursedV1FrameAnims;
			this.isCursed = true;
		}
		else if (witchCrest.IsEquipped)
		{
			if (this.currentFrameCrest == witchCrest)
			{
				return false;
			}
			this.currentFrameCrest = witchCrest;
			CS$<>8__locals1.newFrameAnims = this.witchFrameAnims;
		}
		else if (toolmasterCrest.IsEquipped)
		{
			if (this.currentFrameCrest == toolmasterCrest)
			{
				return false;
			}
			this.currentFrameCrest = toolmasterCrest;
			CS$<>8__locals1.newFrameAnims = this.toolmasterFrameAnims;
			CS$<>8__locals1.customAnimRoutine = new BindOrbHudFrame.CoroutineFunction(this.ToolmasterCrestRoutine);
		}
		else if (spellCrest.IsEquipped)
		{
			if (this.currentFrameCrest == spellCrest)
			{
				return false;
			}
			this.currentFrameCrest = spellCrest;
			CS$<>8__locals1.newFrameAnims = this.spellFrameAnims;
		}
		else
		{
			if (!CS$<>8__locals1.isFirst && this.currentFrameCrest == null)
			{
				return false;
			}
			this.currentFrameCrest = null;
			CS$<>8__locals1.newFrameAnims = this.defaultFrameAnims;
		}
		if (this.activateEventsTarget)
		{
			this.activateEventsTarget.enabled = true;
			this.activateEventsTarget.SendEvent("DEACTIVATE");
		}
		BindOrbHudFrame.BasicFrameAnims basicFrameAnims = this.currentFrameAnims;
		this.currentFrameAnims = CS$<>8__locals1.newFrameAnims;
		if (!CS$<>8__locals1.isFirst && basicFrameAnims != null && this.currentFrameAnims != null && basicFrameAnims.Idle == this.currentFrameAnims.Idle && CS$<>8__locals1.customAnimRoutine == null)
		{
			return false;
		}
		if (this.animRoutine != null)
		{
			base.StopCoroutine(this.animRoutine);
		}
		if (CS$<>8__locals1.isInstant | CS$<>8__locals1.isFirst)
		{
			CS$<>8__locals1.<DoChangeFrame>g__StartNextFrameAnims|0();
		}
		else if (this.onEndFrameAnim != null)
		{
			this.onEndFrameAnim(new Action(CS$<>8__locals1.<DoChangeFrame>g__StartNextFrameAnims|0));
		}
		else
		{
			this.animRoutine = base.StartCoroutine(this.FrameDisappear(null, new Action(CS$<>8__locals1.<DoChangeFrame>g__StartNextFrameAnims|0)));
		}
		return true;
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x000F3FAF File Offset: 0x000F21AF
	private IEnumerator FrameAppear(BindOrbHudFrame.BasicFrameAnims frameAnims, BindOrbHudFrame.CoroutineFunction customAnimRoutine, bool isFirst)
	{
		string text;
		if (isFirst)
		{
			text = ((!string.IsNullOrEmpty(frameAnims.AppearFromNone)) ? frameAnims.AppearFromNone : frameAnims.Appear);
			if (GameCameras.instance.IsHudVisible)
			{
				(Gameplay.CursedCrest.IsEquipped ? this.hudChangeCursedAudio : this.hudAppearAudio).SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			}
		}
		else
		{
			text = frameAnims.Appear;
		}
		if (!string.IsNullOrEmpty(text))
		{
			tk2dSpriteAnimationClip clip = this.animProxy.GetClip(text);
			if (clip != null)
			{
				float seconds = this.animator.PlayAnimGetTime(clip);
				this.animator.PlayFromFrame(0);
				yield return new WaitForSeconds(seconds);
			}
		}
		this.PlayFrameAnim(frameAnims.Idle, 0);
		if (isFirst)
		{
			this.isActive = true;
		}
		this.ChangeEnded();
		if (customAnimRoutine != null)
		{
			this.animRoutine = base.StartCoroutine(customAnimRoutine());
		}
		yield break;
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x000F3FD3 File Offset: 0x000F21D3
	private IEnumerator FrameDisappear(BindOrbHudFrame.BasicFrameAnims frameAnims, Action startNextFrameAnims)
	{
		if (BindOrbHudFrame.SkipToNextAppear)
		{
			this.PlayChangeEffects();
		}
		else
		{
			if (this.refreshDelay > 0f)
			{
				yield return new WaitForSeconds(this.refreshDelay);
			}
			if (!this.currentFrameCrest)
			{
				this.PlayChangeEffects();
			}
			if (frameAnims != null && !string.IsNullOrEmpty(frameAnims.Disappear))
			{
				tk2dSpriteAnimationClip clip = this.animProxy.GetClip(frameAnims.Disappear);
				if (clip != null)
				{
					float seconds = this.animator.PlayAnimGetTime(clip);
					this.animator.PlayFromFrame(0);
					yield return new WaitForSeconds(seconds);
				}
			}
			if (this.currentFrameCrest)
			{
				this.PlayChangeEffects();
			}
		}
		startNextFrameAnims();
		yield break;
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x000F3FF0 File Offset: 0x000F21F0
	private void PlayChangeEffects()
	{
		if (this.changeParticle)
		{
			if (this.changeParticle.IsAlive(true))
			{
				this.changeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			this.changeParticle.Play(true);
		}
		if (!GameCameras.instance.IsHudVisible)
		{
			return;
		}
		(this.isCursed ? this.hudChangeCursedAudio : this.hudChangeAudio).SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x000F4070 File Offset: 0x000F2270
	private void PlayFrameAnim(string animName, int frame = 0)
	{
		if (string.IsNullOrEmpty(animName))
		{
			return;
		}
		tk2dSpriteAnimationClip clip = this.animProxy.GetClip(animName);
		if (clip != null)
		{
			this.animator.PlayFromFrame(clip, frame);
		}
		this.RefreshLifebloodTint();
	}

	// Token: 0x06003745 RID: 14149 RVA: 0x000F40A9 File Offset: 0x000F22A9
	private IEnumerator WarriorCrestRoutine()
	{
		HeroController hc = HeroController.instance;
		bool wasInRageMode = false;
		for (;;)
		{
			if (!hc.IsPaused())
			{
				HeroController.WarriorCrestStateInfo warriorState = hc.WarriorState;
				if (warriorState.IsInRageMode)
				{
					if (!wasInRageMode)
					{
						this.PlayFrameAnim(this.warriorRageAnim, 0);
					}
				}
				else if (wasInRageMode)
				{
					this.PlayFrameAnim(this.warriorRageEndAnim, 0);
				}
				wasInRageMode = warriorState.IsInRageMode;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06003746 RID: 14150 RVA: 0x000F40B8 File Offset: 0x000F22B8
	private IEnumerator ReaperCrestRoutine()
	{
		HeroController hc = HeroController.instance;
		bool wasInReaperMode = false;
		float reaperEffectTimeLeft = 0f;
		for (;;)
		{
			if (!hc.IsPaused())
			{
				HeroController.ReaperCrestStateInfo reaperState = hc.ReaperState;
				if (reaperState.IsInReaperMode)
				{
					if (!wasInReaperMode)
					{
						this.PlayFrameAnim(this.reaperModeBeginAnim, 0);
						if (this.reaperModeEffect)
						{
							this.reaperModeEffect.gameObject.SetActive(false);
							this.reaperModeEffect.gameObject.SetActive(true);
							this.reaperModeEffect.AlphaSelf = 1f;
							reaperEffectTimeLeft = 0f;
						}
					}
				}
				else if (wasInReaperMode)
				{
					this.PlayFrameAnim(this.reaperModeEndAnim, 0);
					if (this.reaperModeEffect)
					{
						reaperEffectTimeLeft = this.reaperModeEffect.FadeTo(0f, this.reaperModeEffectFadeOutTime, null, false, null);
					}
				}
				if (reaperEffectTimeLeft > 0f)
				{
					reaperEffectTimeLeft -= Time.deltaTime;
					if (reaperEffectTimeLeft <= 0f)
					{
						this.reaperModeEffect.gameObject.SetActive(false);
					}
				}
				wasInReaperMode = reaperState.IsInReaperMode;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06003747 RID: 14151 RVA: 0x000F40C7 File Offset: 0x000F22C7
	private IEnumerator WandererCrestRoutine()
	{
		HeroController hc = HeroController.instance;
		bool wasLucky = false;
		for (;;)
		{
			if (!hc.IsPaused())
			{
				bool isWandererLucky = hc.IsWandererLucky;
				if (isWandererLucky && !wasLucky)
				{
					this.PlayFrameAnim(this.wandererFullAnim, 0);
					if (!hc.IsRefillSoundsSuppressed && HudCanvas.IsVisible && ScreenFaderState.Alpha < 0.5f)
					{
						this.wandererHarpAppearAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
					}
				}
				else if (!isWandererLucky && wasLucky)
				{
					this.PlayFrameAnim(this.wandererFullEndAnim, 0);
					if (!hc.IsRefillSoundsSuppressed && HudCanvas.IsVisible && ScreenFaderState.Alpha < 0.5f)
					{
						this.wandererHarpDisappearAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
					}
				}
				wasLucky = isWandererLucky;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06003748 RID: 14152 RVA: 0x000F40D6 File Offset: 0x000F22D6
	private IEnumerator ToolmasterCrestRoutine()
	{
		PlayerData pd = PlayerData.instance;
		HeroController hc = HeroController.instance;
		bool couldBind = (float)pd.silk >= SilkSpool.BindCost;
		for (;;)
		{
			if (!hc.IsPaused())
			{
				bool flag = (float)pd.silk >= SilkSpool.BindCost;
				if (flag != couldBind || this.queuedToolmasterSpin)
				{
					this.PlayFrameAnim(this.toolmasterSilkGetAnim, 0);
				}
				couldBind = flag;
				this.queuedToolmasterSpin = false;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06003749 RID: 14153 RVA: 0x000F40E5 File Offset: 0x000F22E5
	private IEnumerator HunterCrestV2Routine()
	{
		return this.HunterCrestUpgradedRoutine(Gameplay.HunterComboHits, 0, this.hunterV2Bar, null, this.hunterV2FullAnim, null, this.hunterV2FrameAnims.Idle);
	}

	// Token: 0x0600374A RID: 14154 RVA: 0x000F410C File Offset: 0x000F230C
	private IEnumerator HunterCrestV3Routine()
	{
		return this.HunterCrestUpgradedRoutine(Gameplay.HunterCombo2Hits, Gameplay.HunterCombo2ExtraHits, this.hunterV3BarA, this.hunterV3BarB, this.hunterV3FullAnimA, this.hunterV3FullAnimB, this.hunterV3FrameAnims.Idle);
	}

	// Token: 0x0600374B RID: 14155 RVA: 0x000F4144 File Offset: 0x000F2344
	private IEnumerator HunterCrestUpgradedRoutine(int maxHits, int extraMaxHits, UiProgressBar bar, UiProgressBar extraBar, string fullAnimA, string fullAnimB, string idleAnim)
	{
		HeroController hc = HeroController.instance;
		bar.Value = 0f;
		bar.gameObject.SetActive(true);
		if (extraBar != null)
		{
			extraBar.Value = 0f;
			extraBar.gameObject.SetActive(true);
		}
		int previousHits = -1;
		bool wasFull = false;
		bool wasFullExtra = false;
		for (;;)
		{
			if (!hc.IsPaused())
			{
				HeroController.HunterUpgCrestStateInfo hunterUpgState = hc.HunterUpgState;
				bool flag = hunterUpgState.CurrentMeterHits >= maxHits;
				int num = hunterUpgState.CurrentMeterHits - maxHits;
				bool flag2 = extraMaxHits > 0 && num >= extraMaxHits;
				if (num > 0 && extraMaxHits > 0 && hunterUpgState.CurrentMeterHits != previousHits)
				{
					this.hunterV3ExtraHitEffect.SetActive(false);
					this.hunterV3ExtraHitEffect.SetActive(true);
				}
				if (flag)
				{
					if (!wasFull)
					{
						bar.gameObject.SetActive(false);
						this.PlayFrameAnim(fullAnimA, 0);
						this.hunterV2ChargedAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
					}
					if (flag2)
					{
						if (!wasFullExtra)
						{
							if (extraBar)
							{
								extraBar.gameObject.SetActive(false);
							}
							this.PlayFrameAnim(fullAnimB, 0);
							this.hunterV3ChargedAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
						}
					}
					else if (extraBar != null)
					{
						if (!extraBar.gameObject.activeSelf)
						{
							extraBar.gameObject.SetActive(true);
						}
						if (hunterUpgState.CurrentMeterHits != previousHits)
						{
							extraBar.Value = (float)num / (float)extraMaxHits;
						}
					}
				}
				else
				{
					if (wasFull)
					{
						this.PlayFrameAnim(idleAnim, 0);
						if (extraBar != null)
						{
							extraBar.SetValueInstant(0f);
						}
					}
					if (hunterUpgState.CurrentMeterHits > previousHits)
					{
						bar.Value = (float)hunterUpgState.CurrentMeterHits / (float)maxHits;
					}
					else if (hunterUpgState.CurrentMeterHits < previousHits)
					{
						bar.SetValueInstant(0f);
					}
					if (wasFull)
					{
						bar.gameObject.SetActive(true);
					}
				}
				previousHits = hunterUpgState.CurrentMeterHits;
				wasFull = flag;
				wasFullExtra = flag2;
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x0600374C RID: 14156 RVA: 0x000F4194 File Offset: 0x000F2394
	private void ChangeEnded()
	{
		EventRegister.SendEvent(EventRegisterEvents.HudFrameChanged, null);
		if (this.activateEventsTarget && this.currentFrameAnims != null && !string.IsNullOrEmpty(this.currentFrameAnims.ActivateEvent))
		{
			this.activateEventsTarget.enabled = true;
			this.activateEventsTarget.SendEvent(this.currentFrameAnims.ActivateEvent);
		}
	}

	// Token: 0x04003A07 RID: 14855
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims defaultFrameAnims;

	// Token: 0x04003A08 RID: 14856
	[SerializeField]
	private AudioEvent hudAppearAudio;

	// Token: 0x04003A09 RID: 14857
	[Header("Common")]
	[SerializeField]
	private float refreshDelay;

	// Token: 0x04003A0A RID: 14858
	[SerializeField]
	private ParticleSystem changeParticle;

	// Token: 0x04003A0B RID: 14859
	[SerializeField]
	private PlayMakerFSM activateEventsTarget;

	// Token: 0x04003A0C RID: 14860
	[SerializeField]
	private AudioEvent hudChangeAudio;

	// Token: 0x04003A0D RID: 14861
	[SerializeField]
	private Color lifebloodTint;

	// Token: 0x04003A0E RID: 14862
	[Header("Cloakless")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims cloaklessFrameAnims;

	// Token: 0x04003A0F RID: 14863
	[Header("Hunter")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims hunterV2FrameAnims;

	// Token: 0x04003A10 RID: 14864
	[SerializeField]
	private string hunterV2FullAnim;

	// Token: 0x04003A11 RID: 14865
	[SerializeField]
	private UiProgressBar hunterV2Bar;

	// Token: 0x04003A12 RID: 14866
	[SerializeField]
	private AudioEvent hunterV2ChargedAudio;

	// Token: 0x04003A13 RID: 14867
	[Space]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims hunterV3FrameAnims;

	// Token: 0x04003A14 RID: 14868
	[SerializeField]
	private string hunterV3FullAnimA;

	// Token: 0x04003A15 RID: 14869
	[SerializeField]
	private string hunterV3FullAnimB;

	// Token: 0x04003A16 RID: 14870
	[SerializeField]
	private UiProgressBar hunterV3BarA;

	// Token: 0x04003A17 RID: 14871
	[SerializeField]
	private UiProgressBar hunterV3BarB;

	// Token: 0x04003A18 RID: 14872
	[SerializeField]
	private GameObject hunterV3ExtraHitEffect;

	// Token: 0x04003A19 RID: 14873
	[SerializeField]
	private AudioEvent hunterV3ChargedAudio;

	// Token: 0x04003A1A RID: 14874
	[Header("Warrior")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims warriorFrameAnims;

	// Token: 0x04003A1B RID: 14875
	[SerializeField]
	private BindOrbHudFrame.MeterAnims[] warriorMeterAnims;

	// Token: 0x04003A1C RID: 14876
	[SerializeField]
	private string warriorRageAnim;

	// Token: 0x04003A1D RID: 14877
	[SerializeField]
	private string warriorRageEndAnim;

	// Token: 0x04003A1E RID: 14878
	[Header("Reaper")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims reaperFrameAnims;

	// Token: 0x04003A1F RID: 14879
	[SerializeField]
	private string reaperModeBeginAnim;

	// Token: 0x04003A20 RID: 14880
	[SerializeField]
	private string reaperModeEndAnim;

	// Token: 0x04003A21 RID: 14881
	[SerializeField]
	private NestedFadeGroupBase reaperModeEffect;

	// Token: 0x04003A22 RID: 14882
	[SerializeField]
	private float reaperModeEffectFadeOutTime;

	// Token: 0x04003A23 RID: 14883
	[Header("Wanderer")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims wandererFrameAnims;

	// Token: 0x04003A24 RID: 14884
	[SerializeField]
	private string wandererFullAnim;

	// Token: 0x04003A25 RID: 14885
	[SerializeField]
	private string wandererFullEndAnim;

	// Token: 0x04003A26 RID: 14886
	[SerializeField]
	private AudioEvent wandererHarpAppearAudio;

	// Token: 0x04003A27 RID: 14887
	[SerializeField]
	private AudioEvent wandererHarpDisappearAudio;

	// Token: 0x04003A28 RID: 14888
	[Header("Witch")]
	[SerializeField]
	private AudioEvent hudChangeCursedAudio;

	// Token: 0x04003A29 RID: 14889
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims cursedV1FrameAnims;

	// Token: 0x04003A2A RID: 14890
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims witchFrameAnims;

	// Token: 0x04003A2B RID: 14891
	[Header("Toolmaster")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims toolmasterFrameAnims;

	// Token: 0x04003A2C RID: 14892
	[SerializeField]
	private string toolmasterSilkGetAnim;

	// Token: 0x04003A2D RID: 14893
	[Header("Spell")]
	[SerializeField]
	private BindOrbHudFrame.BasicFrameAnims spellFrameAnims;

	// Token: 0x04003A2E RID: 14894
	private bool queuedToolmasterSpin;

	// Token: 0x04003A2F RID: 14895
	private bool isActive;

	// Token: 0x04003A30 RID: 14896
	private bool isCursed;

	// Token: 0x04003A31 RID: 14897
	private BindOrbHudFrame.BasicFrameAnims currentFrameAnims;

	// Token: 0x04003A32 RID: 14898
	private ToolCrest currentFrameCrest;

	// Token: 0x04003A33 RID: 14899
	private Coroutine animRoutine;

	// Token: 0x04003A34 RID: 14900
	private BindOrbHudFrame.FrameAnimEndDelegate onEndFrameAnim;

	// Token: 0x04003A35 RID: 14901
	private tk2dSpriteAnimator animator;

	// Token: 0x04003A36 RID: 14902
	private SteelSoulAnimProxy animProxy;

	// Token: 0x02001919 RID: 6425
	[Serializable]
	private class BasicFrameAnims
	{
		// Token: 0x04009456 RID: 37974
		public string AppearFromNone;

		// Token: 0x04009457 RID: 37975
		public string Appear;

		// Token: 0x04009458 RID: 37976
		public string Idle;

		// Token: 0x04009459 RID: 37977
		public string Disappear;

		// Token: 0x0400945A RID: 37978
		public string ActivateEvent;
	}

	// Token: 0x0200191A RID: 6426
	[Serializable]
	private class MeterAnims
	{
		// Token: 0x0400945B RID: 37979
		public GameObject[] IncreaseEffects;
	}

	// Token: 0x0200191B RID: 6427
	// (Invoke) Token: 0x0600932F RID: 37679
	private delegate void FrameAnimEndDelegate(Action onFrameEnded);

	// Token: 0x0200191C RID: 6428
	// (Invoke) Token: 0x06009333 RID: 37683
	private delegate IEnumerator CoroutineFunction();
}
