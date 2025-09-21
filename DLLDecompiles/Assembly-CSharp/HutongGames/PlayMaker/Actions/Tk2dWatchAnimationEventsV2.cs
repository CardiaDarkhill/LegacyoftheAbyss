using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B71 RID: 2929
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Receive animation events and animation complete event of the current animation playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dWatchAnimationEventsV2 : FsmStateAction
	{
		// Token: 0x06005AF0 RID: 23280 RVA: 0x001CB8C8 File Offset: 0x001C9AC8
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AF1 RID: 23281 RVA: 0x001CB8FD File Offset: 0x001C9AFD
		public override void Reset()
		{
			this.gameObject = null;
			this.eventInfo = null;
			this.animationTriggerEvent = null;
			this.animationCompleteEvent = null;
		}

		// Token: 0x06005AF2 RID: 23282 RVA: 0x001CB91B File Offset: 0x001C9B1B
		public override void OnEnter()
		{
			this._getSprite();
			this.DoWatchAnimationWithEvents();
		}

		// Token: 0x06005AF3 RID: 23283 RVA: 0x001CB929 File Offset: 0x001C9B29
		public override void OnUpdate()
		{
			if (!this._sprite.Playing)
			{
				base.Fsm.Event(this.animationCompleteEvent);
				base.Finish();
			}
		}

		// Token: 0x06005AF4 RID: 23284 RVA: 0x001CB950 File Offset: 0x001C9B50
		private void DoWatchAnimationWithEvents()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				return;
			}
			if (this.animationTriggerEvent != null)
			{
				this._sprite.AnimationEventTriggered = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimationEventDelegate);
			}
			if (this.animationCompleteEvent != null)
			{
				this._sprite.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleteDelegate);
			}
		}

		// Token: 0x06005AF5 RID: 23285 RVA: 0x001CB9B8 File Offset: 0x001C9BB8
		private void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			string value = this.eventInfo.Value;
			if (!string.IsNullOrEmpty(value) && !frame.eventInfo.Equals(value))
			{
				return;
			}
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			base.Fsm.Event(this.animationTriggerEvent);
		}

		// Token: 0x06005AF6 RID: 23286 RVA: 0x001CBA34 File Offset: 0x001C9C34
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

		// Token: 0x0400567D RID: 22141
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400567E RID: 22142
		public FsmString eventInfo;

		// Token: 0x0400567F RID: 22143
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		// Token: 0x04005680 RID: 22144
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		// Token: 0x04005681 RID: 22145
		private tk2dSpriteAnimator _sprite;
	}
}
