using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD1 RID: 3537
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Plays an Animation on a Game Object. You can add named animation clips to the object in the Unity editor, or with the Add Animation Clip action. NOTE: The game object must have an Animation component.")]
	public class PlayAnimation : BaseAnimationAction
	{
		// Token: 0x0600666F RID: 26223 RVA: 0x002074D8 File Offset: 0x002056D8
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.playMode = PlayMode.StopAll;
			this.blendTime = 0.3f;
			this.finishEvent = null;
			this.loopEvent = null;
			this.stopOnExit = false;
		}

		// Token: 0x06006670 RID: 26224 RVA: 0x00207514 File Offset: 0x00205714
		public override void OnEnter()
		{
			this.DoPlayAnimation();
		}

		// Token: 0x06006671 RID: 26225 RVA: 0x0020751C File Offset: 0x0020571C
		private void DoPlayAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
				return;
			}
			if (string.IsNullOrEmpty(this.animName.Value))
			{
				base.LogWarning("Missing animName!");
				base.Finish();
				return;
			}
			this.anim = base.animation[this.animName.Value];
			if (this.anim == null)
			{
				base.LogWarning("Missing animation: " + this.animName.Value);
				base.Finish();
				return;
			}
			float value = this.blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(this.animName.Value, this.playMode);
			}
			else
			{
				base.animation.CrossFade(this.animName.Value, value, this.playMode);
			}
			this.prevAnimtTime = this.anim.time;
		}

		// Token: 0x06006672 RID: 26226 RVA: 0x0020761C File Offset: 0x0020581C
		public override void OnUpdate()
		{
			if (base.Fsm.GetOwnerDefaultTarget(this.gameObject) == null || this.anim == null)
			{
				return;
			}
			if (!this.anim.enabled || (this.anim.wrapMode == WrapMode.ClampForever && this.anim.time > this.anim.length))
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
			if (this.anim.wrapMode != WrapMode.ClampForever && this.anim.time > this.anim.length && this.prevAnimtTime < this.anim.length)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x06006673 RID: 26227 RVA: 0x002076E5 File Offset: 0x002058E5
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				this.StopAnimation();
			}
		}

		// Token: 0x06006674 RID: 26228 RVA: 0x002076F5 File Offset: 0x002058F5
		private void StopAnimation()
		{
			if (base.animation != null)
			{
				base.animation.Stop(this.animName.Value);
			}
		}

		// Token: 0x040065C8 RID: 26056
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object to play the animation on. NOTE: Must have an Animation Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065C9 RID: 26057
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation to play. Use the browse button to find animations on the specified Game Object.")]
		public FsmString animName;

		// Token: 0x040065CA RID: 26058
		[Tooltip("Whether to stop all currently playing animations, or just the animations on the same layer as this animation.")]
		public PlayMode playMode;

		// Token: 0x040065CB RID: 26059
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to cross-fade between animations (seconds).")]
		public FsmFloat blendTime;

		// Token: 0x040065CC RID: 26060
		[Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent finishEvent;

		// Token: 0x040065CD RID: 26061
		[Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent loopEvent;

		// Token: 0x040065CE RID: 26062
		[Tooltip("Stop playing the animation when this state is exited.")]
		public bool stopOnExit;

		// Token: 0x040065CF RID: 26063
		private AnimationState anim;

		// Token: 0x040065D0 RID: 26064
		private float prevAnimtTime;
	}
}
