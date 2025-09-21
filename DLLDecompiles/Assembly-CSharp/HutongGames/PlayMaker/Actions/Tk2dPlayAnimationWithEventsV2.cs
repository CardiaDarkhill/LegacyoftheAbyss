using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6A RID: 2922
	public sealed class Tk2dPlayAnimationWithEventsV2 : FsmStateAction
	{
		// Token: 0x06005ABD RID: 23229 RVA: 0x001CAB24 File Offset: 0x001C8D24
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005ABE RID: 23230 RVA: 0x001CAB59 File Offset: 0x001C8D59
		public override void Reset()
		{
			this.gameObject = null;
			this.clipName = null;
			this.animationTriggerEvent = null;
			this.animationCompleteEvent = null;
			this.animationInterruptedEvent = null;
		}

		// Token: 0x06005ABF RID: 23231 RVA: 0x001CAB7E File Offset: 0x001C8D7E
		public override void OnEnter()
		{
			this.hasExpectedClip = false;
			this._getSprite();
			this.DoPlayAnimationWithEvents();
		}

		// Token: 0x06005AC0 RID: 23232 RVA: 0x001CAB94 File Offset: 0x001C8D94
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
			if (!flag)
			{
				base.Finish();
			}
		}

		// Token: 0x06005AC1 RID: 23233 RVA: 0x001CAC84 File Offset: 0x001C8E84
		public override void OnUpdate()
		{
			if (this.hasExpectedClip && this.expectedClip != this._sprite.CurrentClip)
			{
				base.Fsm.Event(this.animationInterruptedEvent);
				base.Fsm.Event(this.animationTriggerEvent);
				base.Fsm.Event(this.animationCompleteEvent);
				base.Finish();
			}
		}

		// Token: 0x06005AC2 RID: 23234 RVA: 0x001CACE8 File Offset: 0x001C8EE8
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

		// Token: 0x06005AC3 RID: 23235 RVA: 0x001CAD54 File Offset: 0x001C8F54
		private void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			base.Fsm.Event(this.animationTriggerEvent);
		}

		// Token: 0x06005AC4 RID: 23236 RVA: 0x001CADAC File Offset: 0x001C8FAC
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

		// Token: 0x04005657 RID: 22103
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005658 RID: 22104
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x04005659 RID: 22105
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		// Token: 0x0400565A RID: 22106
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		// Token: 0x0400565B RID: 22107
		[Tooltip("Sent when the animation is unexpected changed.")]
		public FsmEvent animationInterruptedEvent;

		// Token: 0x0400565C RID: 22108
		private tk2dSpriteAnimator _sprite;

		// Token: 0x0400565D RID: 22109
		private bool hasExpectedClip;

		// Token: 0x0400565E RID: 22110
		private tk2dSpriteAnimationClip expectedClip;
	}
}
