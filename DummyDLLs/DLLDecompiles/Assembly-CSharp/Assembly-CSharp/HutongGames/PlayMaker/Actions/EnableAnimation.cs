using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD0 RID: 3536
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Enables/Disables an Animation on a GameObject.\nAnimation time is paused while disabled. Animation must also have a non zero weight to play.")]
	public class EnableAnimation : BaseAnimationAction
	{
		// Token: 0x0600666A RID: 26218 RVA: 0x002073FB File Offset: 0x002055FB
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.enable = true;
			this.resetOnExit = false;
		}

		// Token: 0x0600666B RID: 26219 RVA: 0x00207423 File Offset: 0x00205623
		public override void OnEnter()
		{
			this.DoEnableAnimation(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x0600666C RID: 26220 RVA: 0x00207444 File Offset: 0x00205644
		private void DoEnableAnimation(GameObject go)
		{
			if (base.UpdateCache(go))
			{
				this.anim = base.animation[this.animName.Value];
				if (this.anim != null)
				{
					this.anim.enabled = this.enable.Value;
				}
			}
		}

		// Token: 0x0600666D RID: 26221 RVA: 0x0020749A File Offset: 0x0020569A
		public override void OnExit()
		{
			if (this.resetOnExit.Value && this.anim != null)
			{
				this.anim.enabled = !this.enable.Value;
			}
		}

		// Token: 0x040065C3 RID: 26051
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065C4 RID: 26052
		[RequiredField]
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation to enable/disable.")]
		public FsmString animName;

		// Token: 0x040065C5 RID: 26053
		[RequiredField]
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		// Token: 0x040065C6 RID: 26054
		[Tooltip("Reset the initial enabled state when exiting the state.")]
		public FsmBool resetOnExit;

		// Token: 0x040065C7 RID: 26055
		private AnimationState anim;
	}
}
