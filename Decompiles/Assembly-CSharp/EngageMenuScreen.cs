using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000646 RID: 1606
public sealed class EngageMenuScreen : MonoBehaviour
{
	// Token: 0x17000689 RID: 1673
	// (get) Token: 0x06003997 RID: 14743 RVA: 0x000FCDB9 File Offset: 0x000FAFB9
	// (set) Token: 0x06003998 RID: 14744 RVA: 0x000FCDC1 File Offset: 0x000FAFC1
	private EngageMenuScreen.Transition QueuedTransition
	{
		get
		{
			return this.queuedTransition;
		}
		set
		{
			if (this.transition == null)
			{
				this.transition = value;
				return;
			}
			this.queuedTransition = value;
		}
	}

	// Token: 0x06003999 RID: 14745 RVA: 0x000FCDDA File Offset: 0x000FAFDA
	private void OnEnable()
	{
		this.hasPlatform = Platform.Current;
		if (this.hasPlatform)
		{
			this.SetEngagementStateInstant(Platform.Current.EngagementState);
		}
	}

	// Token: 0x0600399A RID: 14746 RVA: 0x000FCE04 File Offset: 0x000FB004
	private void OnDisable()
	{
		this.transition = null;
		this.QueuedTransition = null;
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x000FCE14 File Offset: 0x000FB014
	private void Update()
	{
		if (this.hasPlatform)
		{
			this.UpdateEngagementState(Platform.Current.EngagementState);
		}
		if (this.transition != null && this.transition.Update(Time.unscaledDeltaTime))
		{
			if (this.QueuedTransition != null)
			{
				this.transition = this.QueuedTransition;
				this.queuedTransition = null;
				return;
			}
			this.transition = null;
		}
	}

	// Token: 0x0600399C RID: 14748 RVA: 0x000FCE78 File Offset: 0x000FB078
	private void SetEngagementStateInstant(Platform.EngagementStates state)
	{
		this.currentState = state;
		this.transition = null;
		this.QueuedTransition = null;
		if (state == Platform.EngagementStates.NotEngaged)
		{
			this.startGroup.alpha = 1f;
			this.pendingGroup.alpha = 0f;
			return;
		}
		if (state - Platform.EngagementStates.EngagePending > 1)
		{
			return;
		}
		this.startGroup.alpha = 0f;
		this.pendingGroup.alpha = 1f;
		this.transition = new EngageMenuScreen.PulseTransition(this.pendingGroup, this.pulseDuration, this.minPause, this.maxPause, this.pulseRange);
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x000FCF10 File Offset: 0x000FB110
	private void UpdateEngagementState(Platform.EngagementStates state)
	{
		if (this.currentState != state)
		{
			this.currentState = state;
			if (state == Platform.EngagementStates.NotEngaged)
			{
				float fadeRate = EngageMenuScreen.GetFadeRate();
				this.transition = new EngageMenuScreen.CrossFadeTransition(this.pendingGroup, this.startGroup, fadeRate, null);
				return;
			}
			if (state - Platform.EngagementStates.EngagePending > 1)
			{
				return;
			}
			float fadeRate2 = EngageMenuScreen.GetFadeRate();
			this.transition = new EngageMenuScreen.CrossFadeTransition(this.startGroup, this.pendingGroup, fadeRate2, delegate()
			{
				this.QueuedTransition = new EngageMenuScreen.PulseTransition(this.pendingGroup, this.pulseDuration, this.minPause, this.maxPause, this.pulseRange);
			});
		}
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x000FCF84 File Offset: 0x000FB184
	private static float GetFadeRate()
	{
		float result = 3.2f;
		if (UIManager.instance)
		{
			result = UIManager.instance.MENU_FADE_SPEED;
		}
		return result;
	}

	// Token: 0x04003C4E RID: 15438
	[SerializeField]
	private CanvasGroup startGroup;

	// Token: 0x04003C4F RID: 15439
	[SerializeField]
	private CanvasGroup pendingGroup;

	// Token: 0x04003C50 RID: 15440
	[Header("Pulsing Settings")]
	[SerializeField]
	private MinMaxFloat pulseRange = new MinMaxFloat(0f, 1f);

	// Token: 0x04003C51 RID: 15441
	[SerializeField]
	private float pulseDuration = 0.5f;

	// Token: 0x04003C52 RID: 15442
	[SerializeField]
	private float minPause = 0.1f;

	// Token: 0x04003C53 RID: 15443
	[SerializeField]
	private float maxPause = 0.1f;

	// Token: 0x04003C54 RID: 15444
	[Header("Debug")]
	[SerializeField]
	private bool doDebug;

	// Token: 0x04003C55 RID: 15445
	[SerializeField]
	private Platform.EngagementStates debugState;

	// Token: 0x04003C56 RID: 15446
	private const float FADE_RATE = 3.2f;

	// Token: 0x04003C57 RID: 15447
	private Platform.EngagementStates currentState;

	// Token: 0x04003C58 RID: 15448
	private bool hasPlatform;

	// Token: 0x04003C59 RID: 15449
	private EngageMenuScreen.Transition transition;

	// Token: 0x04003C5A RID: 15450
	private EngageMenuScreen.Transition queuedTransition;

	// Token: 0x02001963 RID: 6499
	private abstract class Transition
	{
		// Token: 0x06009415 RID: 37909
		public abstract bool Update(float deltaTime);
	}

	// Token: 0x02001964 RID: 6500
	private sealed class CrossFadeTransition : EngageMenuScreen.Transition
	{
		// Token: 0x14000105 RID: 261
		// (add) Token: 0x06009417 RID: 37911 RVA: 0x002A13C8 File Offset: 0x0029F5C8
		// (remove) Token: 0x06009418 RID: 37912 RVA: 0x002A1400 File Offset: 0x0029F600
		public event Action OnTransitionComplete;

		// Token: 0x06009419 RID: 37913 RVA: 0x002A1438 File Offset: 0x0029F638
		public CrossFadeTransition(CanvasGroup fadeOutGroup, CanvasGroup fadeInGroup, float rate, Action callback = null)
		{
			this.fadeOutGroup = fadeOutGroup;
			this.fadeInGroup = fadeInGroup;
			this.rate = rate;
			this.hasFadeOut = fadeOutGroup;
			this.hasFadeIn = fadeInGroup;
			if (callback != null)
			{
				this.OnTransitionComplete += callback;
			}
		}

		// Token: 0x0600941A RID: 37914 RVA: 0x002A1484 File Offset: 0x0029F684
		public override bool Update(float deltaTime)
		{
			bool result = false;
			for (;;)
			{
				switch (this.state)
				{
				case EngageMenuScreen.CrossFadeTransition.State.NotStarted:
					this.state = EngageMenuScreen.CrossFadeTransition.State.FadeOut;
					break;
				case EngageMenuScreen.CrossFadeTransition.State.FadeOut:
					if (this.hasFadeOut)
					{
						float num = Mathf.Clamp01(this.fadeOutGroup.alpha - this.rate * deltaTime);
						this.fadeOutGroup.alpha = num;
						if (num > 0f)
						{
							this.fadeOutTimer += deltaTime;
							if (this.fadeOutTimer < 2f)
							{
								return result;
							}
						}
					}
					this.state = EngageMenuScreen.CrossFadeTransition.State.FadeIn;
					break;
				case EngageMenuScreen.CrossFadeTransition.State.FadeIn:
					if (this.hasFadeIn)
					{
						float num2 = Mathf.Clamp01(this.fadeInGroup.alpha + this.rate * deltaTime);
						this.fadeInGroup.alpha = num2;
						if (num2 < 1f)
						{
							this.fadeInTimer += deltaTime;
							if (this.fadeInTimer < 2f)
							{
								return result;
							}
						}
					}
					this.state = EngageMenuScreen.CrossFadeTransition.State.Finished;
					break;
				case EngageMenuScreen.CrossFadeTransition.State.Finished:
					goto IL_EA;
				default:
					this.state = EngageMenuScreen.CrossFadeTransition.State.NotStarted;
					break;
				}
			}
			IL_EA:
			Action onTransitionComplete = this.OnTransitionComplete;
			if (onTransitionComplete != null)
			{
				onTransitionComplete();
			}
			result = true;
			return result;
		}

		// Token: 0x04009596 RID: 38294
		private CanvasGroup fadeOutGroup;

		// Token: 0x04009597 RID: 38295
		private CanvasGroup fadeInGroup;

		// Token: 0x04009598 RID: 38296
		private float rate;

		// Token: 0x04009599 RID: 38297
		private EngageMenuScreen.CrossFadeTransition.State state;

		// Token: 0x0400959B RID: 38299
		private const float MENU_FADE_FAILSAFE = 2f;

		// Token: 0x0400959C RID: 38300
		private float fadeInTimer;

		// Token: 0x0400959D RID: 38301
		private float fadeOutTimer;

		// Token: 0x0400959E RID: 38302
		private bool hasFadeOut;

		// Token: 0x0400959F RID: 38303
		private bool hasFadeIn;

		// Token: 0x02001C22 RID: 7202
		private enum State
		{
			// Token: 0x0400A018 RID: 40984
			NotStarted,
			// Token: 0x0400A019 RID: 40985
			FadeOut,
			// Token: 0x0400A01A RID: 40986
			FadeIn,
			// Token: 0x0400A01B RID: 40987
			Finished
		}
	}

	// Token: 0x02001965 RID: 6501
	private sealed class PulseTransition : EngageMenuScreen.Transition
	{
		// Token: 0x0600941B RID: 37915 RVA: 0x002A15A0 File Offset: 0x0029F7A0
		public PulseTransition(CanvasGroup canvasGroup, float pulseDuration, float minPause, float maxPause, MinMaxFloat pulseMinMax)
		{
			this.canvasGroup = canvasGroup;
			this.minPause = minPause;
			this.maxPause = maxPause;
			this.minAlpha = pulseMinMax.Start;
			this.maxAlpha = pulseMinMax.End;
			this.pauseTimer = 0f;
			if (this.minAlpha > this.maxAlpha)
			{
				float num = this.maxAlpha;
				float num2 = this.minAlpha;
				this.minAlpha = num;
				this.maxAlpha = num2;
			}
			if (pulseDuration <= 0f)
			{
				pulseDuration = 1f;
			}
			this.fadeRate = (this.maxAlpha - this.minAlpha) / pulseDuration;
			this.isValid = (this.fadeRate > 0f && canvasGroup);
		}

		// Token: 0x0600941C RID: 37916 RVA: 0x002A1658 File Offset: 0x0029F858
		public override bool Update(float deltaTime)
		{
			if (!this.isValid)
			{
				return true;
			}
			if (this.pauseTimer > 0f)
			{
				this.pauseTimer -= deltaTime;
				if (this.pauseTimer > 0f)
				{
					return false;
				}
			}
			EngageMenuScreen.PulseTransition.State state = this.state;
			if (state != EngageMenuScreen.PulseTransition.State.FadeOut)
			{
				if (state != EngageMenuScreen.PulseTransition.State.FadeIn)
				{
					this.state = EngageMenuScreen.PulseTransition.State.FadeOut;
				}
				else
				{
					float num = Mathf.Min(this.maxAlpha, this.canvasGroup.alpha + this.fadeRate * deltaTime);
					this.canvasGroup.alpha = num;
					if (num >= this.maxAlpha)
					{
						this.pauseTimer = this.maxPause;
						this.state = EngageMenuScreen.PulseTransition.State.FadeOut;
					}
				}
			}
			else
			{
				float num2 = Mathf.Max(this.minAlpha, this.canvasGroup.alpha - this.fadeRate * deltaTime);
				this.canvasGroup.alpha = num2;
				if (num2 <= this.minAlpha)
				{
					this.pauseTimer = this.minPause;
					this.state = EngageMenuScreen.PulseTransition.State.FadeIn;
				}
			}
			return false;
		}

		// Token: 0x040095A0 RID: 38304
		private CanvasGroup canvasGroup;

		// Token: 0x040095A1 RID: 38305
		private float fadeRate;

		// Token: 0x040095A2 RID: 38306
		private float minPause;

		// Token: 0x040095A3 RID: 38307
		private float maxPause;

		// Token: 0x040095A4 RID: 38308
		private float minAlpha;

		// Token: 0x040095A5 RID: 38309
		private float maxAlpha;

		// Token: 0x040095A6 RID: 38310
		private bool isValid;

		// Token: 0x040095A7 RID: 38311
		private float pauseTimer;

		// Token: 0x040095A8 RID: 38312
		private EngageMenuScreen.PulseTransition.State state;

		// Token: 0x02001C23 RID: 7203
		private enum State
		{
			// Token: 0x0400A01D RID: 40989
			FadeOut,
			// Token: 0x0400A01E RID: 40990
			FadeIn
		}
	}
}
