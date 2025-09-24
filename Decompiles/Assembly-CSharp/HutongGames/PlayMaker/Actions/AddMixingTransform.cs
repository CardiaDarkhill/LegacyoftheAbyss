using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCB RID: 3531
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Play an animation on a subset of the hierarchy. E.g., A waving animation on the upper body.")]
	public class AddMixingTransform : BaseAnimationAction
	{
		// Token: 0x06006652 RID: 26194 RVA: 0x00206D97 File Offset: 0x00204F97
		public override void Reset()
		{
			this.gameObject = null;
			this.animationName = "";
			this.transform = "";
			this.recursive = true;
		}

		// Token: 0x06006653 RID: 26195 RVA: 0x00206DCC File Offset: 0x00204FCC
		public override void OnEnter()
		{
			this.DoAddMixingTransform();
			base.Finish();
		}

		// Token: 0x06006654 RID: 26196 RVA: 0x00206DDC File Offset: 0x00204FDC
		private void DoAddMixingTransform()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animationName.Value];
			if (animationState == null)
			{
				return;
			}
			Transform mix = ownerDefaultTarget.transform.Find(this.transform.Value);
			animationState.AddMixingTransform(mix, this.recursive.Value);
		}

		// Token: 0x040065AE RID: 26030
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065AF RID: 26031
		[RequiredField]
		[Tooltip("The name of the animation to mix. NOTE: The animation should already be added to the Animation Component on the GameObject.")]
		public FsmString animationName;

		// Token: 0x040065B0 RID: 26032
		[RequiredField]
		[Tooltip("The mixing transform. E.g., root/upper_body/left_shoulder")]
		public FsmString transform;

		// Token: 0x040065B1 RID: 26033
		[Tooltip("If recursive is true all children of the mix transform will also be animated.")]
		public FsmBool recursive;
	}
}
