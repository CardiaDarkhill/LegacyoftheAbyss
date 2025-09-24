using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B69 RID: 2921
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nCan receive animation events and animation complete event. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationWithEvents : FsmStateAction
	{
		// Token: 0x06005AB4 RID: 23220 RVA: 0x001CA81C File Offset: 0x001C8A1C
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AB5 RID: 23221 RVA: 0x001CA851 File Offset: 0x001C8A51
		public override void Reset()
		{
			this.gameObject = null;
			this.clipName = null;
			this.animationTriggerEvent = null;
			this.animationCompleteEvent = null;
		}

		// Token: 0x06005AB6 RID: 23222 RVA: 0x001CA86F File Offset: 0x001C8A6F
		public override void OnEnter()
		{
			this.hasExpectedClip = false;
			this._getSprite();
			this.DoPlayAnimationWithEvents();
		}

		// Token: 0x06005AB7 RID: 23223 RVA: 0x001CA884 File Offset: 0x001C8A84
		private void DoPlayAnimationWithEvents()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
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
			if (!this.hasExpectedClip && flag)
			{
				base.Fsm.Event(this.animationTriggerEvent);
				base.Fsm.Event(this.animationCompleteEvent);
			}
			if (!flag || !this.hasExpectedClip)
			{
				base.Finish();
			}
		}

		// Token: 0x06005AB8 RID: 23224 RVA: 0x001CA9A4 File Offset: 0x001C8BA4
		public override void OnUpdate()
		{
			if (this.hasExpectedClip && this.expectedClip != this._sprite.CurrentClip)
			{
				base.Fsm.Event(this.animationTriggerEvent);
				base.Fsm.Event(this.animationCompleteEvent);
				base.Finish();
			}
		}

		// Token: 0x06005AB9 RID: 23225 RVA: 0x001CA9F4 File Offset: 0x001C8BF4
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

		// Token: 0x06005ABA RID: 23226 RVA: 0x001CAA60 File Offset: 0x001C8C60
		private void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			base.Fsm.Event(this.animationTriggerEvent);
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x001CAAB8 File Offset: 0x001C8CB8
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

		// Token: 0x04005650 RID: 22096
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005651 RID: 22097
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;

		// Token: 0x04005652 RID: 22098
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		// Token: 0x04005653 RID: 22099
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		// Token: 0x04005654 RID: 22100
		private tk2dSpriteAnimator _sprite;

		// Token: 0x04005655 RID: 22101
		private bool hasExpectedClip;

		// Token: 0x04005656 RID: 22102
		private tk2dSpriteAnimationClip expectedClip;
	}
}
