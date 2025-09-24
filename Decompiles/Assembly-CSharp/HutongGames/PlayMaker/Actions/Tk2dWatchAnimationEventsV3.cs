using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B72 RID: 2930
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Receive animation events and animation complete event of the current animation playing. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dWatchAnimationEventsV3 : FsmStateAction
	{
		// Token: 0x06005AF8 RID: 23288 RVA: 0x001CBAA0 File Offset: 0x001C9CA0
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AF9 RID: 23289 RVA: 0x001CBAD5 File Offset: 0x001C9CD5
		public override void Reset()
		{
			this.gameObject = null;
			this.eventInfo = null;
			this.animationTriggerEvent = null;
			this.animationCompleteEvent = null;
			this.sendTriggerEventIfAnimationChanged = null;
			this.sendCompleteEventIfAnimationChanged = null;
			this.animationInterruptedEvent = null;
		}

		// Token: 0x06005AFA RID: 23290 RVA: 0x001CBB08 File Offset: 0x001C9D08
		public override void OnEnter()
		{
			this._getSprite();
			this.DoWatchAnimationWithEvents();
		}

		// Token: 0x06005AFB RID: 23291 RVA: 0x001CBB18 File Offset: 0x001C9D18
		public override void OnUpdate()
		{
			if (!this._sprite.Playing)
			{
				base.Fsm.Event(this.animationCompleteEvent);
				base.Finish();
				return;
			}
			if (this.checkChange && this._sprite.CurrentClip != this._currentClip)
			{
				base.Fsm.Event(this.animationInterruptedEvent);
				if (this.sendTriggerEventIfAnimationChanged.Value)
				{
					base.Fsm.Event(this.animationTriggerEvent);
				}
				if (this.sendCompleteEventIfAnimationChanged.Value)
				{
					base.Fsm.Event(this.animationCompleteEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x06005AFC RID: 23292 RVA: 0x001CBBB8 File Offset: 0x001C9DB8
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
			this.checkChange = (this.animationCompleteEvent != null || this.sendTriggerEventIfAnimationChanged.Value || this.sendCompleteEventIfAnimationChanged.Value);
			this._currentClip = this._sprite.CurrentClip;
		}

		// Token: 0x06005AFD RID: 23293 RVA: 0x001CBC58 File Offset: 0x001C9E58
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

		// Token: 0x06005AFE RID: 23294 RVA: 0x001CBCD4 File Offset: 0x001C9ED4
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

		// Token: 0x04005682 RID: 22146
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005683 RID: 22147
		public FsmString eventInfo;

		// Token: 0x04005684 RID: 22148
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		// Token: 0x04005685 RID: 22149
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		// Token: 0x04005686 RID: 22150
		public FsmBool sendTriggerEventIfAnimationChanged;

		// Token: 0x04005687 RID: 22151
		public FsmBool sendCompleteEventIfAnimationChanged;

		// Token: 0x04005688 RID: 22152
		public FsmEvent animationInterruptedEvent;

		// Token: 0x04005689 RID: 22153
		private tk2dSpriteAnimator _sprite;

		// Token: 0x0400568A RID: 22154
		private tk2dSpriteAnimationClip _currentClip;

		// Token: 0x0400568B RID: 22155
		private bool checkChange;
	}
}
