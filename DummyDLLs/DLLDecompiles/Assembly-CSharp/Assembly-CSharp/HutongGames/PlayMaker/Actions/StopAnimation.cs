using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD8 RID: 3544
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Stops all playing Animations on a Game Object. Optionally, specify a single Animation to Stop.")]
	public class StopAnimation : BaseAnimationAction
	{
		// Token: 0x06006695 RID: 26261 RVA: 0x00207E00 File Offset: 0x00206000
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
		}

		// Token: 0x06006696 RID: 26262 RVA: 0x00207E10 File Offset: 0x00206010
		public override void OnEnter()
		{
			this.DoStopAnimation();
			base.Finish();
		}

		// Token: 0x06006697 RID: 26263 RVA: 0x00207E20 File Offset: 0x00206020
		private void DoStopAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (FsmString.IsNullOrEmpty(this.animName))
			{
				base.animation.Stop();
				return;
			}
			base.animation.Stop(this.animName.Value);
		}

		// Token: 0x040065ED RID: 26093
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065EE RID: 26094
		[Tooltip("The name of the animation to stop. Leave empty to stop all playing animations.")]
		[UIHint(UIHint.Animation)]
		public FsmString animName;
	}
}
