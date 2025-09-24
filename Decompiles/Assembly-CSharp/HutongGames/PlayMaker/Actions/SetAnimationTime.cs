using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD6 RID: 3542
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Sets the current Time of an Animation. Useful for manually controlling playback of an animation. Check Every Frame to update the time continuously.")]
	public class SetAnimationTime : BaseAnimationAction
	{
		// Token: 0x0600668B RID: 26251 RVA: 0x00207BB4 File Offset: 0x00205DB4
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.time = null;
			this.normalized = false;
			this.everyFrame = false;
		}

		// Token: 0x0600668C RID: 26252 RVA: 0x00207BD9 File Offset: 0x00205DD9
		public override void OnEnter()
		{
			this.DoSetAnimationTime((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600668D RID: 26253 RVA: 0x00207C14 File Offset: 0x00205E14
		public override void OnUpdate()
		{
			this.DoSetAnimationTime((this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value);
		}

		// Token: 0x0600668E RID: 26254 RVA: 0x00207C44 File Offset: 0x00205E44
		private void DoSetAnimationTime(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			base.animation.Play(this.animName.Value);
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				base.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			if (this.normalized)
			{
				animationState.normalizedTime = this.time.Value;
			}
			else
			{
				animationState.time = this.time.Value;
			}
			if (this.everyFrame)
			{
				animationState.speed = 0f;
			}
		}

		// Token: 0x040065E4 RID: 26084
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065E5 RID: 26085
		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation.")]
		public FsmString animName;

		// Token: 0x040065E6 RID: 26086
		[Tooltip("The time to set the animation to.")]
		public FsmFloat time;

		// Token: 0x040065E7 RID: 26087
		[Tooltip("Use normalized time: 0 = start ; 1 = end. Useful if you don't care about the length of the exact length of the animation.")]
		public bool normalized;

		// Token: 0x040065E8 RID: 26088
		[Tooltip("Set time every frame. Useful if you're using a variable as Time.")]
		public bool everyFrame;
	}
}
