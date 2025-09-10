using System;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000624 RID: 1572
public abstract class CurrencyCounterBase : MonoBehaviour
{
	// Token: 0x1700065D RID: 1629
	// (get) Token: 0x060037D9 RID: 14297 RVA: 0x000F6540 File Offset: 0x000F4740
	private bool IsForcedActive
	{
		get
		{
			return this.showCount > 0;
		}
	}

	// Token: 0x1700065E RID: 1630
	// (get) Token: 0x060037DA RID: 14298 RVA: 0x000F654B File Offset: 0x000F474B
	// (set) Token: 0x060037DB RID: 14299 RVA: 0x000F6554 File Offset: 0x000F4754
	private protected bool IsActive
	{
		protected get
		{
			return this.isActive;
		}
		private set
		{
			bool flag = this.isActive;
			this.isActive = value;
			if (value)
			{
				Action appeared = this.Appeared;
				if (appeared != null)
				{
					appeared();
				}
				if (this.geoChange == 0 && this.rollerFade)
				{
					this.rollerFade.FadeToZero(this.fadeOutTime);
				}
				this.isFadingOut = false;
				return;
			}
			Action disappeared = this.Disappeared;
			if (disappeared == null)
			{
				return;
			}
			disappeared();
		}
	}

	// Token: 0x1700065F RID: 1631
	// (get) Token: 0x060037DC RID: 14300 RVA: 0x000F65C3 File Offset: 0x000F47C3
	protected bool IsActiveOrQueued
	{
		get
		{
			return this.isActive || this.queuedAction > CurrencyCounterBase.QueuedAction.None;
		}
	}

	// Token: 0x17000660 RID: 1632
	// (get) Token: 0x060037DD RID: 14301 RVA: 0x000F65D8 File Offset: 0x000F47D8
	private bool IsRolling
	{
		get
		{
			return this.addRollerState == CurrencyCounterBase.RollerState.Rolling || this.addRollerStartTimer > 0f || this.takeRollerState == CurrencyCounterBase.RollerState.Rolling || this.takeRollerStartTimer > 0f;
		}
	}

	// Token: 0x17000661 RID: 1633
	// (get) Token: 0x060037DE RID: 14302 RVA: 0x000F6608 File Offset: 0x000F4808
	public static bool IsAnyCounterRolling
	{
		get
		{
			using (List<CurrencyCounterBase>.Enumerator enumerator = CurrencyCounterBase._allCounters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsRolling)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x17000662 RID: 1634
	// (get) Token: 0x060037DF RID: 14303 RVA: 0x000F6660 File Offset: 0x000F4860
	// (set) Token: 0x060037E0 RID: 14304 RVA: 0x000F6668 File Offset: 0x000F4868
	protected CurrencyCounterIcon IconOverride
	{
		get
		{
			return this.iconOverride;
		}
		set
		{
			this.iconOverride = value;
			this.icon.gameObject.SetActive(!this.iconOverride);
		}
	}

	// Token: 0x17000663 RID: 1635
	// (get) Token: 0x060037E1 RID: 14305 RVA: 0x000F668F File Offset: 0x000F488F
	private CurrencyCounterIcon CurrentIcon
	{
		get
		{
			if (!this.iconOverride)
			{
				return this.icon;
			}
			return this.iconOverride;
		}
	}

	// Token: 0x17000664 RID: 1636
	// (get) Token: 0x060037E2 RID: 14306
	protected abstract int Count { get; }

	// Token: 0x17000665 RID: 1637
	// (get) Token: 0x060037E3 RID: 14307 RVA: 0x000F66AB File Offset: 0x000F48AB
	// (set) Token: 0x060037E4 RID: 14308 RVA: 0x000F66B3 File Offset: 0x000F48B3
	public int StackOrder { get; private set; }

	// Token: 0x060037E5 RID: 14309 RVA: 0x000F66BC File Offset: 0x000F48BC
	protected virtual void Awake()
	{
		CurrencyCounterBase._allCounters.Add(this);
		if (this.addTextMesh)
		{
			this.initialAddTextX = this.addTextMesh.transform.localPosition.x;
		}
		if (this.subTextMesh)
		{
			this.initialSubTextX = this.subTextMesh.transform.localPosition.x;
		}
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x000F6724 File Offset: 0x000F4924
	protected virtual void Start()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = 0f;
		}
		GameManager instance = GameManager.instance;
		instance.GamePausedChange += this.OnGamePauseChanged;
		instance.OnBeforeFinishedSceneTransition += this.OnLevelLoaded;
	}

	// Token: 0x060037E7 RID: 14311 RVA: 0x000F6778 File Offset: 0x000F4978
	private void OnDisable()
	{
		this.addRollerState = CurrencyCounterBase.RollerState.Down;
		this.addRollerStartTimer = 0f;
		this.takeRollerState = CurrencyCounterBase.RollerState.Down;
		this.takeRollerStartTimer = 0f;
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
		}
		Action action = this.onAfterDelay;
		if (action != null)
		{
			action();
		}
		Action action2 = this.onTimerEnd;
		if (action2 == null)
		{
			return;
		}
		action2();
	}

	// Token: 0x060037E8 RID: 14312 RVA: 0x000F67E0 File Offset: 0x000F49E0
	protected virtual void OnDestroy()
	{
		CurrencyCounterBase._allCounters.Remove(this);
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		silentInstance.GamePausedChange -= this.OnGamePauseChanged;
		silentInstance.OnBeforeFinishedSceneTransition -= this.OnLevelLoaded;
	}

	// Token: 0x060037E9 RID: 14313 RVA: 0x000F682C File Offset: 0x000F4A2C
	public static void FadeInIfActive()
	{
		for (int i = CurrencyCounterBase._allCounters.Count - 1; i >= 0; i--)
		{
			CurrencyCounterBase._allCounters[i].InternalFadeInIfActive();
		}
		CurrencyCounterBase.orderCounter = 0;
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x000F6868 File Offset: 0x000F4A68
	public static void HideAllInstant()
	{
		for (int i = CurrencyCounterBase._allCounters.Count - 1; i >= 0; i--)
		{
			CurrencyCounterBase._allCounters[i].HideInstant();
		}
	}

	// Token: 0x060037EB RID: 14315 RVA: 0x000F689C File Offset: 0x000F4A9C
	private void OnLevelLoaded()
	{
		if (CheatManager.ForceCurrencyCountersAppear)
		{
			return;
		}
		this.ResetState();
		this.HideIconInstant();
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
		}
		if (this.fadeGroup)
		{
			this.fadeGroup.FadeTo(0f, 0f, null, true, null);
		}
		this.StopRollSound();
		this.IsActive = false;
	}

	// Token: 0x060037EC RID: 14316 RVA: 0x000F6904 File Offset: 0x000F4B04
	protected void UpdateCounterStart()
	{
		this.counterCurrent = this.Count;
		this.geoTextMesh.Text = this.counterCurrent.ToString();
		this.initialCounter = this.geoTextMesh.Text.Length;
		if (this.addTextMesh)
		{
			this.addTextMesh.Text = string.Empty;
		}
		if (this.subTextMesh)
		{
			this.subTextMesh.Text = string.Empty;
		}
		this.ResetState();
		this.RefreshText(false);
		this.UpdateRectLayout();
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x000F6998 File Offset: 0x000F4B98
	private void ResetState()
	{
		if (this.addCounter > 0 || this.takeCounter > 0)
		{
			this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.Stopped);
		}
		this.geoChange = 0;
		this.addCounter = 0;
		this.takeCounter = 0;
		this.addRollerStartTimer = 0f;
		this.addRollerState = CurrencyCounterBase.RollerState.Down;
		this.takeRollerStartTimer = 0f;
		this.takeRollerState = CurrencyCounterBase.RollerState.Down;
		this.digitChangeTimer = 0f;
		this.toZero = false;
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x000F6A0C File Offset: 0x000F4C0C
	public void UpdateValue()
	{
		int num = this.Count - this.counterCurrent;
		if (num > 0)
		{
			this.InternalAdd(num);
			return;
		}
		if (num < 0)
		{
			this.InternalTake(-num);
		}
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x000F6A3F File Offset: 0x000F4C3F
	private void UpdateRectLayout()
	{
		if (!this.amountLayoutGroup)
		{
			return;
		}
		this.amountLayoutGroup.ForceUpdateLayoutNoCanvas();
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x000F6A5A File Offset: 0x000F4C5A
	protected virtual void RefreshText(bool isCountingUp)
	{
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x000F6A5C File Offset: 0x000F4C5C
	private void DoChange()
	{
		this.UpdateLayout();
		this.FadeIn();
		this.IsActive = true;
		if (this.geoChange == 0)
		{
			this.RefreshText(true);
		}
		this.StopRollSound();
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
		}
		float toAlpha = 1f;
		if (this.rollerFade)
		{
			if (this.geoChange == 0)
			{
				toAlpha = 0f;
				this.CurrentIcon.GetSingle();
			}
			if (this.rollerFadeOut != null)
			{
				base.StopCoroutine(this.rollerFadeOut);
				this.rollerFadeOut = null;
			}
			this.rollerFade.FadeTo(toAlpha, 0f, null, false, null);
		}
		Action<float> timer = null;
		if (this.geoChange != 0)
		{
			this.PlayIdle();
		}
		this.onAfterDelay = delegate()
		{
			this.wantsToHide = true;
			this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.FadeDelayElapsed);
			this.onAfterDelay = null;
		};
		this.onTimerEnd = delegate()
		{
			this.onTimerEnd = null;
		};
		float y = (this.geoChange > 0) ? ((float)this.geoChange / (float)this.changePerTick * 0.0125f + 0.3f) : 0f;
		this.targetHideTime = Time.realtimeSinceStartup + MathF.Max(this.fadeOutDelay, y);
		this.fadeOutRoutine = this.StartTimerRoutine(this.fadeOutDelay, this.fadeOutTime, delegate(float time)
		{
			Action<float> timer = timer;
			if (timer == null)
			{
				return;
			}
			timer(time);
		}, this.onAfterDelay, this.onTimerEnd, true);
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x000F6BC0 File Offset: 0x000F4DC0
	private void Update()
	{
		if (this.toZero)
		{
			if (this.digitChangeTimer >= 0f)
			{
				this.digitChangeTimer -= Time.unscaledDeltaTime;
				return;
			}
			if (this.counterCurrent > 0)
			{
				this.counterCurrent += this.changePerTick;
				if (this.counterCurrent <= 0)
				{
					this.takeRollerState = CurrencyCounterBase.RollerState.Down;
					this.counterCurrent = 0;
					this.toZero = false;
					this.StopRollSound();
					if (this.IsForcedActive)
					{
						this.FadeRollers(null);
					}
					else
					{
						this.DelayedHide();
					}
				}
				else if (this.takeRollerState != CurrencyCounterBase.RollerState.Rolling)
				{
					this.takeRollerState = CurrencyCounterBase.RollerState.Rolling;
					this.PlayRollSound();
				}
				this.geoTextMesh.Text = this.counterCurrent.ToString();
				this.digitChangeTimer += 0.0125f;
				this.RefreshText(false);
				this.UpdateRectLayout();
				return;
			}
		}
		else
		{
			if (this.addRollerState == CurrencyCounterBase.RollerState.Up)
			{
				if (this.addRollerStartTimer > 0f)
				{
					this.addRollerStartTimer -= Time.unscaledDeltaTime;
				}
				else
				{
					this.addRollerState = CurrencyCounterBase.RollerState.Rolling;
					this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.StartAdd);
				}
			}
			if (this.addRollerState == CurrencyCounterBase.RollerState.Rolling)
			{
				if (this.addCounter > 0)
				{
					this.CurrentIcon.Get();
					if (this.digitChangeTimer < 0f)
					{
						this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.RollerTicked);
						this.addCounter -= this.changePerTick;
						this.counterCurrent += this.changePerTick;
						this.geoTextMesh.Text = this.counterCurrent.ToString();
						if (this.addTextMesh != null)
						{
							this.addTextMesh.Text = "+ " + this.addCounter.ToString();
						}
						if (this.addCounter <= 0)
						{
							this.PlayIdle();
							this.addCounter = 0;
							if (this.addTextMesh)
							{
								this.addTextMesh.Text = "+ 0";
							}
							this.addRollerState = CurrencyCounterBase.RollerState.Down;
							this.counterCurrent = this.targetEndCount;
							this.geoTextMesh.Text = this.counterCurrent.ToString();
							this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.Stopped);
							if (!this.IsForcedActive)
							{
								this.wantsToHide = true;
							}
						}
						this.digitChangeTimer += 0.0125f;
						this.RefreshText(true);
						this.UpdateRectLayout();
					}
					else
					{
						this.digitChangeTimer -= Time.unscaledDeltaTime;
					}
				}
				else
				{
					this.PlayIdle();
					this.addCounter = 0;
					if (this.addTextMesh)
					{
						this.addTextMesh.Text = "+ 0";
					}
					this.addRollerState = CurrencyCounterBase.RollerState.Down;
					this.counterCurrent = this.targetEndCount;
					this.geoTextMesh.Text = this.counterCurrent.ToString();
					this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.Stopped);
					if (!this.IsForcedActive)
					{
						this.wantsToHide = true;
					}
				}
			}
			if (this.takeRollerState == CurrencyCounterBase.RollerState.Up)
			{
				if (this.takeRollerStartTimer > 0f)
				{
					this.takeRollerStartTimer -= Time.unscaledDeltaTime;
				}
				else
				{
					this.takeRollerState = CurrencyCounterBase.RollerState.Rolling;
					this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.StartTake);
				}
			}
			if (this.takeRollerState == CurrencyCounterBase.RollerState.Rolling)
			{
				if (this.takeCounter < 0)
				{
					this.CurrentIcon.Take();
					if (this.digitChangeTimer < 0f)
					{
						this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.RollerTicked);
						this.takeCounter -= this.changePerTick;
						this.counterCurrent += this.changePerTick;
						this.geoTextMesh.Text = this.counterCurrent.ToString();
						if (this.subTextMesh != null)
						{
							this.subTextMesh.Text = "- " + (-this.takeCounter).ToString();
						}
						if (this.takeCounter >= 0)
						{
							this.PlayIdle();
							this.takeCounter = 0;
							if (this.subTextMesh)
							{
								this.subTextMesh.Text = "- 0";
							}
							this.takeRollerState = CurrencyCounterBase.RollerState.Down;
							this.counterCurrent = this.targetEndCount;
							this.geoTextMesh.Text = this.counterCurrent.ToString();
							this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.Stopped);
							if (!this.IsForcedActive)
							{
								this.wantsToHide = true;
							}
						}
						this.digitChangeTimer += 0.0125f;
						this.RefreshText(false);
						this.UpdateRectLayout();
					}
					else
					{
						this.digitChangeTimer -= Time.unscaledDeltaTime;
					}
				}
				else
				{
					this.PlayIdle();
					this.takeCounter = 0;
					if (this.subTextMesh)
					{
						this.subTextMesh.Text = "- 0";
					}
					this.takeRollerState = CurrencyCounterBase.RollerState.Down;
					this.counterCurrent = this.targetEndCount;
					this.geoTextMesh.Text = this.counterCurrent.ToString();
					this.SendStateChangedEvent(CurrencyCounterBase.StateEvents.Stopped);
					if (!this.IsForcedActive)
					{
						this.wantsToHide = true;
					}
				}
			}
			if (!this.toZero && this.addRollerState == CurrencyCounterBase.RollerState.Down && this.takeRollerState <= CurrencyCounterBase.RollerState.Down && this.wantsToHide && this.isVisible && !this.IsForcedActive && Time.realtimeSinceStartup >= this.targetHideTime && !CurrencyCounterBase.IsAnyCounterRolling)
			{
				this.DelayedHide();
			}
		}
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x000F70D0 File Offset: 0x000F52D0
	private void LateUpdate()
	{
		switch (this.queuedAction)
		{
		case CurrencyCounterBase.QueuedAction.Add:
			this.InternalAdd(this.queuedValue);
			this.queuedAction = CurrencyCounterBase.QueuedAction.None;
			break;
		case CurrencyCounterBase.QueuedAction.Take:
			if (this.queuedValue > 0)
			{
				this.InternalTake(this.queuedValue);
			}
			this.queuedAction = CurrencyCounterBase.QueuedAction.None;
			break;
		case CurrencyCounterBase.QueuedAction.Zero:
			if (this.counterCurrent > 0)
			{
				this.InternalToZero();
			}
			this.queuedAction = CurrencyCounterBase.QueuedAction.None;
			break;
		case CurrencyCounterBase.QueuedAction.Popup:
			this.InternalPopup();
			this.queuedAction = CurrencyCounterBase.QueuedAction.None;
			break;
		}
		if (this.hideQueued)
		{
			this.hideQueued = false;
			if (this.showCount <= 0)
			{
				this.FadeOut(delegate(bool finished)
				{
					if (finished)
					{
						this.IsActive = false;
					}
				});
			}
		}
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x000F7184 File Offset: 0x000F5384
	public float FadeIn()
	{
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
			this.fadeOutRoutine = null;
		}
		this.isFadingOut = false;
		this.ShowIcon();
		if (this.fadeGroup)
		{
			return this.fadeGroup.FadeTo(1f, this.fadeInTime, null, true, null);
		}
		return 0f;
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x000F71E5 File Offset: 0x000F53E5
	public float FadeOut()
	{
		return this.FadeOut(null);
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x000F71F0 File Offset: 0x000F53F0
	public float FadeOut(Action<bool> callback)
	{
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
			this.fadeOutRoutine = null;
		}
		this.isFadingOut = true;
		this.HideIcon();
		if (this.fadeGroup != null)
		{
			return this.fadeGroup.FadeTo(0f, this.fadeOutTime, null, true, callback);
		}
		this.FadeRollers(callback);
		return 0f;
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x000F725C File Offset: 0x000F545C
	[ContextMenu("Do Hide")]
	private void DelayedHide()
	{
		this.wantsToHide = false;
		this.hideQueued = false;
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
		}
		if (CheatManager.ForceCurrencyCountersAppear)
		{
			return;
		}
		this.fadeOutRoutine = this.StartTimerRoutine(this.completedFadeOutDelay, 0f, null, delegate
		{
			this.FadeOut(delegate(bool finished)
			{
				if (finished && this.showCount <= 0)
				{
					this.IsActive = false;
				}
			});
			this.fadeOutRoutine = null;
		}, null, true);
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x000F72B9 File Offset: 0x000F54B9
	private void ShowIcon()
	{
		this.wantsToHide = false;
		this.hideQueued = false;
		if (!this.isVisible)
		{
			this.isVisible = true;
			this.CurrentIcon.Appear();
		}
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x000F72E3 File Offset: 0x000F54E3
	private void HideIcon()
	{
		this.wantsToHide = false;
		this.hideQueued = false;
		this.isVisible = false;
		this.CurrentIcon.Disappear();
	}

	// Token: 0x060037FA RID: 14330 RVA: 0x000F7305 File Offset: 0x000F5505
	private void HideIconInstant()
	{
		this.IsActive = false;
		this.wantsToHide = false;
		this.hideQueued = false;
		this.isVisible = false;
		this.CurrentIcon.HideInstant();
	}

	// Token: 0x060037FB RID: 14331 RVA: 0x000F732E File Offset: 0x000F552E
	public void HideInstant()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.FadeTo(0f, 0f, null, true, null);
		}
		this.HideIconInstant();
	}

	// Token: 0x060037FC RID: 14332 RVA: 0x000F735C File Offset: 0x000F555C
	private void InternalFadeInIfActive()
	{
		if (this.isVisible || this.isFadingOut)
		{
			return;
		}
		if (!this.IsActive)
		{
			if (this.showCount <= 0)
			{
				return;
			}
			this.IsActive = true;
		}
		this.FadeIn();
	}

	// Token: 0x060037FD RID: 14333 RVA: 0x000F738F File Offset: 0x000F558F
	private void PlayIdle()
	{
		if (!this.isVisible)
		{
			return;
		}
		this.CurrentIcon.Idle();
	}

	// Token: 0x060037FE RID: 14334 RVA: 0x000F73A8 File Offset: 0x000F55A8
	private void UpdateLayout()
	{
		int num = this.counterCurrent.ToString().Length - this.initialCounter.ToString().Length;
		if (this.subTextMesh)
		{
			this.subTextMesh.transform.SetLocalPositionX(this.initialSubTextX + (float)num * this.characterOffset);
		}
		if (this.addTextMesh)
		{
			this.addTextMesh.transform.SetLocalPositionX(this.initialAddTextX + (float)num * this.characterOffset);
		}
	}

	// Token: 0x060037FF RID: 14335 RVA: 0x000F7431 File Offset: 0x000F5631
	protected void QueueAdd(int geo)
	{
		this.queuedAction = CurrencyCounterBase.QueuedAction.Add;
		this.queuedValue = geo;
	}

	// Token: 0x06003800 RID: 14336 RVA: 0x000F7441 File Offset: 0x000F5641
	protected void QueueTake(int geo)
	{
		this.queuedAction = CurrencyCounterBase.QueuedAction.Take;
		this.queuedValue = geo;
	}

	// Token: 0x06003801 RID: 14337 RVA: 0x000F7454 File Offset: 0x000F5654
	protected void QueueToValue(int geo)
	{
		int num = geo - this.counterCurrent;
		if (num > 0)
		{
			this.queuedAction = CurrencyCounterBase.QueuedAction.Add;
			this.queuedValue = num;
			return;
		}
		this.queuedAction = CurrencyCounterBase.QueuedAction.Take;
		this.queuedValue = -num;
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x000F748C File Offset: 0x000F568C
	protected void QueueToZero()
	{
		this.queuedAction = CurrencyCounterBase.QueuedAction.Zero;
		this.queuedValue = 0;
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x000F749C File Offset: 0x000F569C
	protected void QueuePopup()
	{
		this.queuedAction = CurrencyCounterBase.QueuedAction.Popup;
		this.queuedValue = 0;
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x000F74AC File Offset: 0x000F56AC
	protected void InternalAdd(int geo)
	{
		if (this.subTextMesh)
		{
			this.subTextMesh.Text = string.Empty;
		}
		this.targetEndCount = this.Count;
		if (this.takeRollerState > CurrencyCounterBase.RollerState.Down)
		{
			this.geoChange = geo;
			this.addCounter = this.geoChange;
			this.takeRollerState = CurrencyCounterBase.RollerState.Down;
			this.counterCurrent = this.Count - this.addCounter;
			this.geoTextMesh.Text = this.counterCurrent.ToString();
		}
		if (this.skipRoller)
		{
			this.counterCurrent = this.Count;
			this.geoTextMesh.Text = this.counterCurrent.ToString();
			return;
		}
		if (this.addRollerState == CurrencyCounterBase.RollerState.Down)
		{
			this.geoChange = geo;
			this.addCounter = this.geoChange;
			this.counterCurrent = this.Count - this.addCounter;
			if (this.addTextMesh)
			{
				this.addTextMesh.Text = "+ " + this.addCounter.ToString();
			}
			this.addRollerStartTimer = 1f;
			this.addRollerState = CurrencyCounterBase.RollerState.Up;
		}
		else if (this.addRollerState == CurrencyCounterBase.RollerState.Up)
		{
			this.geoChange = this.Count - this.counterCurrent;
			this.addCounter = this.geoChange;
			if (this.addTextMesh)
			{
				this.addTextMesh.Text = "+ " + this.addCounter.ToString();
			}
			this.addRollerStartTimer = 1f;
		}
		else if (this.addRollerState == CurrencyCounterBase.RollerState.Rolling)
		{
			this.geoChange = geo;
			this.addCounter = this.geoChange;
			this.counterCurrent = this.Count - this.geoChange;
			this.geoTextMesh.Text = this.counterCurrent.ToString();
			if (this.addTextMesh)
			{
				this.addTextMesh.Text = "+ " + this.addCounter.ToString();
			}
			this.addRollerState = CurrencyCounterBase.RollerState.Up;
			this.addRollerStartTimer = 1f;
		}
		this.changePerTick = (int)((double)((float)this.addCounter * 0.0125f) * 1.75);
		this.changePerTick = Mathf.Max(1, this.changePerTick);
		this.DoChange();
	}

	// Token: 0x06003805 RID: 14341 RVA: 0x000F76EC File Offset: 0x000F58EC
	protected void InternalTake(int geo)
	{
		this.ResetState();
		this.targetEndCount = this.Count;
		if (this.addTextMesh)
		{
			this.addTextMesh.Text = string.Empty;
		}
		if (this.addRollerState > CurrencyCounterBase.RollerState.Down)
		{
			this.geoChange = -geo;
			this.takeCounter = this.geoChange;
			this.addRollerState = CurrencyCounterBase.RollerState.Down;
			this.counterCurrent = this.Count + -this.takeCounter;
			this.geoTextMesh.Text = this.counterCurrent.ToString();
		}
		if (this.skipRoller)
		{
			this.counterCurrent = this.Count;
			this.geoTextMesh.Text = this.counterCurrent.ToString();
			return;
		}
		if (this.takeRollerState == CurrencyCounterBase.RollerState.Down)
		{
			this.geoChange = -geo;
			this.takeCounter = this.geoChange;
			this.counterCurrent = this.Count + geo;
			if (this.isVisible)
			{
				this.geoTextMesh.Text = this.counterCurrent.ToString();
			}
			if (this.subTextMesh)
			{
				this.subTextMesh.Text = "- " + (-this.takeCounter).ToString();
			}
			this.takeRollerStartTimer = 1f;
			this.takeRollerState = CurrencyCounterBase.RollerState.Up;
		}
		else if (this.takeRollerState == CurrencyCounterBase.RollerState.Up)
		{
			this.geoChange = -geo;
			this.takeCounter += this.geoChange;
			if (this.subTextMesh)
			{
				this.subTextMesh.Text = "- " + (-this.takeCounter).ToString();
			}
			this.takeRollerStartTimer = 1f;
		}
		else if (this.takeRollerState == CurrencyCounterBase.RollerState.Rolling)
		{
			this.geoChange = -geo;
			this.takeCounter = this.geoChange;
			this.counterCurrent = this.Count;
			this.geoTextMesh.Text = this.counterCurrent.ToString();
			if (this.subTextMesh)
			{
				this.subTextMesh.Text = "- " + (-this.takeCounter).ToString();
			}
			this.takeRollerState = CurrencyCounterBase.RollerState.Up;
			this.takeRollerStartTimer = 1f;
		}
		this.changePerTick = (int)((double)((float)this.takeCounter * 0.0125f) * 1.75);
		this.changePerTick = Mathf.Min(-1, this.changePerTick);
		this.DoChange();
	}

	// Token: 0x06003806 RID: 14342 RVA: 0x000F7954 File Offset: 0x000F5B54
	protected void InternalToZero()
	{
		this.ResetState();
		if (this.addTextMesh)
		{
			this.addTextMesh.Text = string.Empty;
		}
		if (this.subTextMesh)
		{
			this.subTextMesh.Text = string.Empty;
		}
		if (this.counterCurrent != 0)
		{
			this.changePerTick = -(int)((float)this.counterCurrent * 0.0125f * 1.75f);
			this.changePerTick = Mathf.Min(-1, this.changePerTick);
			this.toZero = true;
		}
		this.DoChange();
	}

	// Token: 0x06003807 RID: 14343 RVA: 0x000F79E4 File Offset: 0x000F5BE4
	private void InternalPopup()
	{
		this.ResetState();
		this.geoTextMesh.Text = string.Empty;
		if (this.addTextMesh)
		{
			this.addTextMesh.Text = string.Empty;
		}
		if (this.subTextMesh)
		{
			this.subTextMesh.Text = string.Empty;
		}
		this.changePerTick = 0;
		this.counterCurrent = 0;
		this.DoChange();
	}

	// Token: 0x06003808 RID: 14344 RVA: 0x000F7A58 File Offset: 0x000F5C58
	protected void InternalShow()
	{
		if (this.showCount < 0)
		{
			this.showCount = 0;
		}
		this.showCount++;
		if (this.showCount > 1)
		{
			return;
		}
		this.StackOrder += ++CurrencyCounterBase.orderCounter;
		if (this.addRollerState == CurrencyCounterBase.RollerState.Down && this.takeRollerState == CurrencyCounterBase.RollerState.Down)
		{
			if (this.addTextMesh)
			{
				this.addTextMesh.Text = string.Empty;
			}
			if (this.subTextMesh)
			{
				this.subTextMesh.Text = string.Empty;
			}
			this.FadeIn();
		}
		else
		{
			this.wantsToHide = false;
			if (!this.isVisible)
			{
				this.FadeIn();
			}
		}
		this.IsActive = true;
	}

	// Token: 0x06003809 RID: 14345 RVA: 0x000F7B18 File Offset: 0x000F5D18
	protected void InternalHide(bool forced = false)
	{
		this.showCount--;
		if (CheatManager.ForceCurrencyCountersAppear)
		{
			return;
		}
		if (forced)
		{
			this.showCount = 0;
		}
		if (!forced && this.showCount != 0)
		{
			return;
		}
		this.StackOrder = 0;
		if (!forced && this.IsRolling)
		{
			this.wantsToHide = true;
			return;
		}
		this.hideQueued = true;
	}

	// Token: 0x0600380A RID: 14346 RVA: 0x000F7B72 File Offset: 0x000F5D72
	protected void InternalFail()
	{
		if (this.failAnimator)
		{
			this.failAnimator.SetTrigger(CurrencyCounterBase._failedPropId);
		}
	}

	// Token: 0x0600380B RID: 14347 RVA: 0x000F7B94 File Offset: 0x000F5D94
	protected virtual void SendStateChangedEvent(CurrencyCounterBase.StateEvents stateEvent)
	{
		switch (stateEvent)
		{
		case CurrencyCounterBase.StateEvents.StartTake:
		case CurrencyCounterBase.StateEvents.StartAdd:
			if (this.geoChange != 0)
			{
				this.PlayRollSound();
				return;
			}
			break;
		case CurrencyCounterBase.StateEvents.Stopped:
			this.StopRollSound();
			if (this.IsForcedActive)
			{
				this.FadeRollers(null);
				return;
			}
			break;
		case CurrencyCounterBase.StateEvents.RollerTicked:
			this.UpdateLayout();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600380C RID: 14348 RVA: 0x000F7BE4 File Offset: 0x000F5DE4
	private void FadeRollers(Action<bool> callback = null)
	{
		if (this.rollerFade)
		{
			if (this.rollerFadeOut != null)
			{
				base.StopCoroutine(this.rollerFadeOut);
			}
			this.rollerFadeOut = this.StartTimerRoutine(this.completedFadeOutDelay, 0f, null, delegate
			{
				this.rollerFade.FadeTo(0f, this.fadeOutTime, null, true, callback);
				this.rollerFadeOut = null;
			}, null, true);
		}
	}

	// Token: 0x0600380D RID: 14349 RVA: 0x000F7C4C File Offset: 0x000F5E4C
	private void PlayRollSound()
	{
		if (!this.isPlayingRollSound && this.rollSound)
		{
			this.isPlayingRollSound = true;
			this.rollSound.Play();
		}
	}

	// Token: 0x0600380E RID: 14350 RVA: 0x000F7C78 File Offset: 0x000F5E78
	private void StopRollSound()
	{
		if (this.isPlayingRollSound && this.rollSound)
		{
			this.isPlayingRollSound = false;
			this.rollSound.Stop();
			if (this.rollEndSound)
			{
				this.rollSound.PlayOneShot(this.rollEndSound);
			}
		}
	}

	// Token: 0x0600380F RID: 14351 RVA: 0x000F7CCC File Offset: 0x000F5ECC
	private void OnGamePauseChanged(bool isPaused)
	{
		if (PlayerData.instance.isInventoryOpen)
		{
			this.OnLevelLoaded();
			return;
		}
		if (!isPaused && this.showCount > 0)
		{
			if (!this.isVisible)
			{
				this.FadeIn();
			}
			this.IsActive = true;
		}
		if (!this.isPlayingRollSound || !this.rollSound)
		{
			return;
		}
		if (isPaused)
		{
			this.rollSound.Pause();
			return;
		}
		this.rollSound.UnPause();
	}

	// Token: 0x04003AD9 RID: 15065
	private const float ROLLER_START_PAUSE = 1f;

	// Token: 0x04003ADA RID: 15066
	private const float DIGIT_CHANGE_TIME = 0.0125f;

	// Token: 0x04003ADB RID: 15067
	public Action Appeared;

	// Token: 0x04003ADC RID: 15068
	public Action Disappeared;

	// Token: 0x04003ADD RID: 15069
	[Space]
	[SerializeField]
	private CurrencyCounterIcon icon;

	// Token: 0x04003ADE RID: 15070
	[Space]
	[SerializeField]
	protected TextBridge geoTextMesh;

	// Token: 0x04003ADF RID: 15071
	[SerializeField]
	private TextBridge subTextMesh;

	// Token: 0x04003AE0 RID: 15072
	private float initialSubTextX;

	// Token: 0x04003AE1 RID: 15073
	[SerializeField]
	private TextBridge addTextMesh;

	// Token: 0x04003AE2 RID: 15074
	private float initialAddTextX;

	// Token: 0x04003AE3 RID: 15075
	[SerializeField]
	private float characterOffset;

	// Token: 0x04003AE4 RID: 15076
	private int initialCounter;

	// Token: 0x04003AE5 RID: 15077
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04003AE6 RID: 15078
	[SerializeField]
	private NestedFadeGroupBase rollerFade;

	// Token: 0x04003AE7 RID: 15079
	[SerializeField]
	private float fadeInTime = 0.1f;

	// Token: 0x04003AE8 RID: 15080
	[SerializeField]
	private float fadeOutDelay = 5f;

	// Token: 0x04003AE9 RID: 15081
	[SerializeField]
	private float completedFadeOutDelay = 0.5f;

	// Token: 0x04003AEA RID: 15082
	private Coroutine rollerFadeOut;

	// Token: 0x04003AEB RID: 15083
	private Coroutine fadeOutRoutine;

	// Token: 0x04003AEC RID: 15084
	private Action onAfterDelay;

	// Token: 0x04003AED RID: 15085
	private Action onTimerEnd;

	// Token: 0x04003AEE RID: 15086
	[SerializeField]
	private float fadeOutTime = 0.5f;

	// Token: 0x04003AEF RID: 15087
	[SerializeField]
	private LayoutGroup amountLayoutGroup;

	// Token: 0x04003AF0 RID: 15088
	[SerializeField]
	private AudioSource rollSound;

	// Token: 0x04003AF1 RID: 15089
	[SerializeField]
	private AudioClip rollEndSound;

	// Token: 0x04003AF2 RID: 15090
	[SerializeField]
	private bool skipRoller;

	// Token: 0x04003AF3 RID: 15091
	[Space]
	[SerializeField]
	private Animator failAnimator;

	// Token: 0x04003AF4 RID: 15092
	private int counterCurrent;

	// Token: 0x04003AF5 RID: 15093
	private int geoChange;

	// Token: 0x04003AF6 RID: 15094
	private int addCounter;

	// Token: 0x04003AF7 RID: 15095
	private int takeCounter;

	// Token: 0x04003AF8 RID: 15096
	private CurrencyCounterBase.RollerState addRollerState;

	// Token: 0x04003AF9 RID: 15097
	private CurrencyCounterBase.RollerState takeRollerState;

	// Token: 0x04003AFA RID: 15098
	private int changePerTick;

	// Token: 0x04003AFB RID: 15099
	private bool isPlayingRollSound;

	// Token: 0x04003AFC RID: 15100
	private float addRollerStartTimer;

	// Token: 0x04003AFD RID: 15101
	private float takeRollerStartTimer;

	// Token: 0x04003AFE RID: 15102
	private float digitChangeTimer;

	// Token: 0x04003AFF RID: 15103
	private bool toZero;

	// Token: 0x04003B00 RID: 15104
	private int showCount;

	// Token: 0x04003B01 RID: 15105
	private bool isActive;

	// Token: 0x04003B02 RID: 15106
	private bool wantsToHide;

	// Token: 0x04003B03 RID: 15107
	private bool isVisible;

	// Token: 0x04003B04 RID: 15108
	private float targetHideTime;

	// Token: 0x04003B05 RID: 15109
	private CurrencyCounterIcon iconOverride;

	// Token: 0x04003B06 RID: 15110
	private CurrencyCounterBase.QueuedAction queuedAction;

	// Token: 0x04003B07 RID: 15111
	private int queuedValue;

	// Token: 0x04003B08 RID: 15112
	private static readonly List<CurrencyCounterBase> _allCounters = new List<CurrencyCounterBase>();

	// Token: 0x04003B09 RID: 15113
	private static readonly int _failedPropId = Animator.StringToHash("Failed");

	// Token: 0x04003B0A RID: 15114
	private bool isFadingOut;

	// Token: 0x04003B0B RID: 15115
	private int targetEndCount;

	// Token: 0x04003B0C RID: 15116
	private static int orderCounter;

	// Token: 0x04003B0E RID: 15118
	private bool hideQueued;

	// Token: 0x02001933 RID: 6451
	private enum RollerState
	{
		// Token: 0x040094CC RID: 38092
		Down,
		// Token: 0x040094CD RID: 38093
		Up,
		// Token: 0x040094CE RID: 38094
		Rolling
	}

	// Token: 0x02001934 RID: 6452
	public enum StateEvents
	{
		// Token: 0x040094D0 RID: 38096
		StartTake,
		// Token: 0x040094D1 RID: 38097
		StartAdd,
		// Token: 0x040094D2 RID: 38098
		Stopped,
		// Token: 0x040094D3 RID: 38099
		RollerTicked,
		// Token: 0x040094D4 RID: 38100
		FadeDelayElapsed
	}

	// Token: 0x02001935 RID: 6453
	private enum QueuedAction
	{
		// Token: 0x040094D6 RID: 38102
		None,
		// Token: 0x040094D7 RID: 38103
		Add,
		// Token: 0x040094D8 RID: 38104
		Take,
		// Token: 0x040094D9 RID: 38105
		Zero,
		// Token: 0x040094DA RID: 38106
		Popup
	}
}
