using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F8 RID: 4344
	[ActionCategory(ActionCategory.Tween)]
	public abstract class TweenActionBase : BaseUpdateAction
	{
		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x06007573 RID: 30067 RVA: 0x0023E0CC File Offset: 0x0023C2CC
		public EasingFunction.Function easingFunction
		{
			get
			{
				EasingFunction.Ease ease = (EasingFunction.Ease)this.easeType.Value;
				if (this.cachedEase != ease || this.func == null)
				{
					this.func = EasingFunction.GetEasingFunction(ease);
					this.cachedEase = ease;
				}
				return this.func;
			}
		}

		// Token: 0x06007574 RID: 30068 RVA: 0x0023E114 File Offset: 0x0023C314
		public override void Reset()
		{
			base.Reset();
			this.startDelay = null;
			this.easeType = null;
			this.time = 1f;
			this.realTime = false;
			this.finishEvent = null;
			this.loopType = LoopType.None;
		}

		// Token: 0x06007575 RID: 30069 RVA: 0x0023E154 File Offset: 0x0023C354
		public override void OnEnter()
		{
			this.currentTime = 0f;
			this.normalizedTime = 0f;
			this.tweenFinished = false;
			this.tweenStarted = false;
			this.everyFrame = true;
			this.reverse = false;
		}

		// Token: 0x06007576 RID: 30070 RVA: 0x0023E188 File Offset: 0x0023C388
		public override void OnActionUpdate()
		{
			float num = this.realTime.Value ? Time.unscaledDeltaTime : Time.deltaTime;
			this.currentTime += num;
			if (!this.tweenStarted)
			{
				if (this.currentTime < this.startDelay.Value)
				{
					return;
				}
				this.tweenStarted = true;
				this.currentTime -= this.startDelay.Value;
			}
			if (this.currentTime > this.time.Value)
			{
				switch (this.loopType)
				{
				case LoopType.None:
					this.tweenFinished = true;
					this.currentTime = this.time.Value;
					break;
				case LoopType.Loop:
					this.currentTime -= this.time.Value;
					break;
				case LoopType.PingPong:
					this.currentTime -= this.time.Value;
					this.reverse = !this.reverse;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			if (!this.reverse)
			{
				this.normalizedTime = this.currentTime / this.time.Value;
			}
			else
			{
				this.normalizedTime = 1f - this.currentTime / this.time.Value;
			}
			EasingFunction.AnimationCurve = this.customCurve.curve;
			this.DoTween();
			if (this.tweenFinished)
			{
				base.Finish();
				base.Fsm.Event(this.finishEvent);
			}
		}

		// Token: 0x06007577 RID: 30071
		protected abstract void DoTween();

		// Token: 0x040075DE RID: 30174
		[ActionSection("Easing")]
		[Tooltip("Delay before starting the tween.")]
		public FsmFloat startDelay;

		// Token: 0x040075DF RID: 30175
		[Tooltip("The type of easing to apply.")]
		[ObjectType(typeof(EasingFunction.Ease))]
		[PreviewField("DrawPreview")]
		public FsmEnum easeType;

		// Token: 0x040075E0 RID: 30176
		[Tooltip("Custom tween curve. Note: Typically you would use the 0-1 range.")]
		[HideIf("HideCustomCurve")]
		public FsmAnimationCurve customCurve;

		// Token: 0x040075E1 RID: 30177
		[Tooltip("Length of tween in seconds.")]
		public FsmFloat time;

		// Token: 0x040075E2 RID: 30178
		[Tooltip("Ignore any time scaling.")]
		public FsmBool realTime;

		// Token: 0x040075E3 RID: 30179
		[Tooltip("Looping options.")]
		public LoopType loopType;

		// Token: 0x040075E4 RID: 30180
		[Tooltip("Event to send when tween is finished.")]
		public FsmEvent finishEvent;

		// Token: 0x040075E5 RID: 30181
		[NonSerialized]
		public float normalizedTime;

		// Token: 0x040075E6 RID: 30182
		protected bool tweenStarted;

		// Token: 0x040075E7 RID: 30183
		protected bool tweenFinished;

		// Token: 0x040075E8 RID: 30184
		protected float currentTime;

		// Token: 0x040075E9 RID: 30185
		protected bool playPreview;

		// Token: 0x040075EA RID: 30186
		private EasingFunction.Ease cachedEase;

		// Token: 0x040075EB RID: 30187
		private EasingFunction.Function func;

		// Token: 0x040075EC RID: 30188
		private static bool showPreviewCurve;

		// Token: 0x040075ED RID: 30189
		private bool reverse;
	}
}
