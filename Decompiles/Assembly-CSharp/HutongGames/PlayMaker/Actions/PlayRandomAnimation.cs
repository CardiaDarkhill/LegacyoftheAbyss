using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD2 RID: 3538
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Plays a Random Animation on a Game Object. You can set the relative weight of each animation to control how often they are selected.")]
	public class PlayRandomAnimation : BaseAnimationAction
	{
		// Token: 0x06006676 RID: 26230 RVA: 0x00207724 File Offset: 0x00205924
		public override void Reset()
		{
			this.gameObject = null;
			this.animations = new FsmString[0];
			this.weights = new FsmFloat[0];
			this.playMode = PlayMode.StopAll;
			this.blendTime = 0.3f;
			this.finishEvent = null;
			this.loopEvent = null;
			this.stopOnExit = false;
		}

		// Token: 0x06006677 RID: 26231 RVA: 0x0020777C File Offset: 0x0020597C
		public override void OnEnter()
		{
			this.DoPlayRandomAnimation();
		}

		// Token: 0x06006678 RID: 26232 RVA: 0x00207784 File Offset: 0x00205984
		private void DoPlayRandomAnimation()
		{
			if (this.animations.Length != 0)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					this.DoPlayAnimation(this.animations[randomWeightedIndex].Value);
				}
			}
		}

		// Token: 0x06006679 RID: 26233 RVA: 0x002077C0 File Offset: 0x002059C0
		private void DoPlayAnimation(string animName)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				base.Finish();
				return;
			}
			if (string.IsNullOrEmpty(animName))
			{
				base.LogWarning("Missing animName!");
				base.Finish();
				return;
			}
			this.anim = base.animation[animName];
			if (this.anim == null)
			{
				base.LogWarning("Missing animation: " + animName);
				base.Finish();
				return;
			}
			float value = this.blendTime.Value;
			if (value < 0.001f)
			{
				base.animation.Play(animName, this.playMode);
			}
			else
			{
				base.animation.CrossFade(animName, value, this.playMode);
			}
			this.prevAnimtTime = this.anim.time;
		}

		// Token: 0x0600667A RID: 26234 RVA: 0x00207890 File Offset: 0x00205A90
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

		// Token: 0x0600667B RID: 26235 RVA: 0x00207959 File Offset: 0x00205B59
		public override void OnExit()
		{
			if (this.stopOnExit)
			{
				this.StopAnimation();
			}
		}

		// Token: 0x0600667C RID: 26236 RVA: 0x00207969 File Offset: 0x00205B69
		private void StopAnimation()
		{
			if (base.animation != null)
			{
				base.animation.Stop(this.anim.name);
			}
		}

		// Token: 0x040065D1 RID: 26065
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("Game Object to play the animation on.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065D2 RID: 26066
		[CompoundArray("Animations", "Animation", "Weight")]
		[UIHint(UIHint.Animation)]
		[Tooltip("An array of animations to pick randomly from.")]
		public FsmString[] animations;

		// Token: 0x040065D3 RID: 26067
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of each animation being picked. E.g. a weight of 2 is twice a likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x040065D4 RID: 26068
		[Tooltip("How to treat previously playing animations.")]
		public PlayMode playMode;

		// Token: 0x040065D5 RID: 26069
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time taken to blend to this animation.")]
		public FsmFloat blendTime;

		// Token: 0x040065D6 RID: 26070
		[Tooltip("Event to send when the animation is finished playing. NOTE: Not sent with Loop or PingPong wrap modes!")]
		public FsmEvent finishEvent;

		// Token: 0x040065D7 RID: 26071
		[Tooltip("Event to send when the animation loops. If you want to send this event to another FSM use Set Event Target. NOTE: This event is only sent with Loop and PingPong wrap modes.")]
		public FsmEvent loopEvent;

		// Token: 0x040065D8 RID: 26072
		[Tooltip("Stop playing the animation when this state is exited.")]
		public bool stopOnExit;

		// Token: 0x040065D9 RID: 26073
		private AnimationState anim;

		// Token: 0x040065DA RID: 26074
		private float prevAnimtTime;
	}
}
