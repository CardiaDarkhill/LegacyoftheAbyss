using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6C RID: 2924
	public sealed class Tk2dPlayRandomAnimationWithEvents : FsmStateAction
	{
		// Token: 0x06005ACF RID: 23247 RVA: 0x001CB134 File Offset: 0x001C9334
		private void _getSprite()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this._sprite = ownerDefaultTarget.GetComponent<tk2dSpriteAnimator>();
		}

		// Token: 0x06005AD0 RID: 23248 RVA: 0x001CB169 File Offset: 0x001C9369
		public override void Reset()
		{
			this.gameObject = null;
			this.clipNames = null;
			this.animationTriggerEvent = null;
			this.animationCompleteEvent = null;
			this.animationInterruptedEvent = null;
		}

		// Token: 0x06005AD1 RID: 23249 RVA: 0x001CB18E File Offset: 0x001C938E
		public override void OnEnter()
		{
			this.hasExpectedClip = false;
			this._getSprite();
			this.DoPlayAnimationWithEvents();
		}

		// Token: 0x06005AD2 RID: 23250 RVA: 0x001CB1A4 File Offset: 0x001C93A4
		private void DoPlayAnimationWithEvents()
		{
			if (this._sprite == null)
			{
				base.LogWarning("Missing tk2dSpriteAnimator component");
				base.Finish();
				return;
			}
			string text = this.clipNames.stringValues[Random.Range(0, this.clipNames.Length)];
			IHeroAnimationController component = this._sprite.GetComponent<IHeroAnimationController>();
			this.expectedClip = ((component != null) ? component.GetClip(text) : this._sprite.GetClipByName(text));
			this.hasExpectedClip = (this.expectedClip != null);
			this._sprite.Play(this.expectedClip);
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

		// Token: 0x06005AD3 RID: 23251 RVA: 0x001CB294 File Offset: 0x001C9494
		public override void OnUpdate()
		{
			if (!this.hasExpectedClip)
			{
				return;
			}
			if (this.expectedClip == this._sprite.CurrentClip)
			{
				return;
			}
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

		// Token: 0x06005AD4 RID: 23252 RVA: 0x001CB318 File Offset: 0x001C9518
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

		// Token: 0x06005AD5 RID: 23253 RVA: 0x001CB384 File Offset: 0x001C9584
		private void AnimationEventDelegate(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip, int frameNum)
		{
			tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
			Fsm.EventData.IntData = frame.eventInt;
			Fsm.EventData.StringData = frame.eventInfo;
			Fsm.EventData.FloatData = frame.eventFloat;
			base.Fsm.Event(this.animationTriggerEvent);
		}

		// Token: 0x06005AD6 RID: 23254 RVA: 0x001CB3DC File Offset: 0x001C95DC
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

		// Token: 0x04005668 RID: 22120
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dSpriteAnimator component attached.")]
		[CheckForComponent(typeof(tk2dSpriteAnimator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005669 RID: 22121
		[RequiredField]
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray clipNames;

		// Token: 0x0400566A RID: 22122
		[Tooltip("Trigger event defined in the clip. The event holds the following triggers infos: the eventInt, eventInfo and eventFloat properties")]
		public FsmEvent animationTriggerEvent;

		// Token: 0x0400566B RID: 22123
		[Tooltip("Animation complete event. The event holds the clipId reference")]
		public FsmEvent animationCompleteEvent;

		// Token: 0x0400566C RID: 22124
		[Tooltip("Sent when the animation is unexpected changed.")]
		public FsmEvent animationInterruptedEvent;

		// Token: 0x0400566D RID: 22125
		public FsmBool replayOnChanged;

		// Token: 0x0400566E RID: 22126
		private tk2dSpriteAnimator _sprite;

		// Token: 0x0400566F RID: 22127
		private bool hasExpectedClip;

		// Token: 0x04005670 RID: 22128
		private tk2dSpriteAnimationClip expectedClip;
	}
}
