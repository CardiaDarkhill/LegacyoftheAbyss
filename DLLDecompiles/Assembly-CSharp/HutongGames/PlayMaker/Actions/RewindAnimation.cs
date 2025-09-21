using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD4 RID: 3540
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Rewinds the named animation.")]
	public class RewindAnimation : BaseAnimationAction
	{
		// Token: 0x06006682 RID: 26242 RVA: 0x00207A30 File Offset: 0x00205C30
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
		}

		// Token: 0x06006683 RID: 26243 RVA: 0x00207A40 File Offset: 0x00205C40
		public override void OnEnter()
		{
			this.DoRewindAnimation();
			base.Finish();
		}

		// Token: 0x06006684 RID: 26244 RVA: 0x00207A50 File Offset: 0x00205C50
		private void DoRewindAnimation()
		{
			if (string.IsNullOrEmpty(this.animName.Value))
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.animation.Rewind(this.animName.Value);
			}
		}

		// Token: 0x040065DE RID: 26078
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065DF RID: 26079
		[UIHint(UIHint.Animation)]
		[Tooltip("The name of the animation to rewind.")]
		public FsmString animName;
	}
}
