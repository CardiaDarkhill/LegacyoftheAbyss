using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6B RID: 2923
	public sealed class Tk2dPlayAnimationWithEventsV3 : FsmStateAction
	{
		// Token: 0x06005AC6 RID: 23238 RVA: 0x001CAE18 File Offset: 0x001C9018
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AC7 RID: 23239 RVA: 0x001CAE4D File Offset: 0x001C904D
		public override void Reset()
		{
			this.gameObject = null;
			this.clipName = null;
			this.animationTriggerEvent = null;
			this.animationCompleteEvent = null;
			this.animationInterruptedEvent = null;
		}

		// Token: 0x06005AC8 RID: 23240 RVA: 0x001CAE72 File Offset: 0x001C9072
		public override void OnEnter()
		{
			this.hasExpectedClip = false;
			this._getSprite();
			this.DoPlayAnimationWithEvents();
		}

		// Token: 0x06005AC9 RID: 23241 RVA: 0x001CAE88 File Offset: 0x001C9088
		private void DoPlayAnimationWithEvents()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				base.Finish();
				return;
			}
			IHeroAnimationController component = this._sprite.GetComponent<IHeroAnimationController>();
			if (component != null)
			{
				this.expectedClip = component.GetClip(this.clipName.Value);
				this._sprite.Play(this.expectedClip);
			}
			else
			{
				this.expectedClip = this._sprite.GetClipByName(this.clipName.Value);
				this._sprite.Play(this.expectedClip);
			}
			this.hasExpectedClip = (this.expectedClip != null);
			bool flag = false;
			if (this.animationTriggerEvent != null)
			{
				this._sprite.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventDelegate);
				flag = true;
			}
			if (this.animationCompleteEvent != null)
			{
				this._sprite.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
				flag = true;
			}
			if (!flag && !this.replayOnChanged.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06005ACA RID: 23242 RVA: 0x001CAF84 File Offset: 0x001C9184
		public override void OnUpdate()
		{
			if (this.hasExpectedClip && this.expectedClip != this._sprite.CurrentClip)
			{
				base.Fsm.Event(this.animationInterruptedEvent);
				base.Fsm.Event(this.animationTriggerEvent);
				base.Fsm.Event(this.animationCompleteEvent);
				if (this.replayOnChanged.Value)
				{
					this._sprite.Play(this.expectedClip);
					return;
				}
				base.Finish();
			}
		}

		// Token: 0x06005ACB RID: 23243 RVA: 0x001CB004 File Offset: 0x001C9204
		public override void OnExit()
		{
			if (this._sprite == null)
			{
				return;
			}
			tk2dSpriteAnimator sprite = this._sprite;
			sprite.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(sprite.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventDelegate));
			tk2dSpriteAnimator sprite2 = this._sprite;
			sprite2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(sprite2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate));
		}

		// Token: 0x06005ACC RID: 23244 RVA: 0x001CB070 File Offset: 0x001C9270
		private void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			base.Fsm.Event(this.animationTriggerEvent);
		}

		// Token: 0x06005ACD RID: 23245 RVA: 0x001CB0C8 File Offset: 0x001C92C8
		private void AnimationCompleteDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
		{
			int intData = -1;
			tk2dSpriteAnimationClip[] array = (sprite.Library != null) ? sprite.Library.clips : null;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == clip)
					{
						intData = i;
						break;
					}
				}
			}
			Fsm.EventData.IntData = intData;
			base.Fsm.Event(this.animationCompleteEvent);
		}

		// Token: 0x0400565F RID: 22111
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005660 RID: 22112
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x04005661 RID: 22113
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		// Token: 0x04005662 RID: 22114
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		// Token: 0x04005663 RID: 22115
		[Tooltip("Sent when the animation is unexpected changed.")]
		public FsmEvent animationInterruptedEvent;

		// Token: 0x04005664 RID: 22116
		public FsmBool replayOnChanged;

		// Token: 0x04005665 RID: 22117
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005666 RID: 22118
		private bool hasExpectedClip;

		// Token: 0x04005667 RID: 22119
		private tk2dSpriteAnimationClip expectedClip;
	}
}
