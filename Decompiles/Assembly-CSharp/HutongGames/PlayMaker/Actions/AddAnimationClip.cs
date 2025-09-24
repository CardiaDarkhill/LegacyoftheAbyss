using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCA RID: 3530
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Adds an Animation Clip to a Game Object. Optionally trim the clip.")]
	public class AddAnimationClip : FsmStateAction
	{
		// Token: 0x0600664E RID: 26190 RVA: 0x00206C84 File Offset: 0x00204E84
		public override void Reset()
		{
			this.gameObject = null;
			this.animationClip = null;
			this.animationName = "";
			this.firstFrame = 0;
			this.lastFrame = 0;
			this.addLoopFrame = false;
		}

		// Token: 0x0600664F RID: 26191 RVA: 0x00206CD3 File Offset: 0x00204ED3
		public override void OnEnter()
		{
			this.DoAddAnimationClip();
			base.Finish();
		}

		// Token: 0x06006650 RID: 26192 RVA: 0x00206CE4 File Offset: 0x00204EE4
		private void DoAddAnimationClip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AnimationClip animationClip = this.animationClip.Value as AnimationClip;
			if (animationClip == null)
			{
				return;
			}
			Animation component = ownerDefaultTarget.GetComponent<Animation>();
			if (this.firstFrame.Value == 0 && this.lastFrame.Value == 0)
			{
				component.AddClip(animationClip, this.animationName.Value);
				return;
			}
			component.AddClip(animationClip, this.animationName.Value, this.firstFrame.Value, this.lastFrame.Value, this.addLoopFrame.Value);
		}

		// Token: 0x040065A8 RID: 26024
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The Game Object to add the Animation Clip to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065A9 RID: 26025
		[RequiredField]
		[ObjectType(typeof(AnimationClip))]
		[Tooltip("The animation clip to add. NOTE: Make sure the clip is compatible with the object's hierarchy.")]
		public FsmObject animationClip;

		// Token: 0x040065AA RID: 26026
		[RequiredField]
		[Tooltip("Optionally give the animation a new name. Can be used by other Animation actions.")]
		public FsmString animationName;

		// Token: 0x040065AB RID: 26027
		[ActionSection("Trimming")]
		[Tooltip("Optionally trim the animation by specifying a first and last frame.")]
		public FsmInt firstFrame;

		// Token: 0x040065AC RID: 26028
		[Tooltip("Set the last frame of the trimmed animation. 0 means no trimming.")]
		public FsmInt lastFrame;

		// Token: 0x040065AD RID: 26029
		[Tooltip("Add a frame at the end of the trimmed clip that's the same as the first frame so it loops nicely.")]
		public FsmBool addLoopFrame;
	}
}
