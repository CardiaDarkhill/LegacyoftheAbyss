using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD3 RID: 3539
	[ActionCategory(ActionCategory.Animation)]
	[Tooltip("Removes a mixing transform previously added with Add Mixing Transform. If transform has been added as recursive, then it will be removed as recursive. Once you remove all mixing transforms added to animation state all curves become animated again.")]
	public class RemoveMixingTransform : BaseAnimationAction
	{
		// Token: 0x0600667E RID: 26238 RVA: 0x00207997 File Offset: 0x00205B97
		public override void Reset()
		{
			this.gameObject = null;
			this.animationName = "";
		}

		// Token: 0x0600667F RID: 26239 RVA: 0x002079B0 File Offset: 0x00205BB0
		public override void OnEnter()
		{
			this.DoRemoveMixingTransform();
			base.Finish();
		}

		// Token: 0x06006680 RID: 26240 RVA: 0x002079C0 File Offset: 0x00205BC0
		private void DoRemoveMixingTransform()
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
			Transform mix = ownerDefaultTarget.transform.Find(this.transfrom.Value);
			animationState.AddMixingTransform(mix);
		}

		// Token: 0x040065DB RID: 26075
		[RequiredField]
		[CheckForComponent(typeof(Animation))]
		[Tooltip("The GameObject playing the animation.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065DC RID: 26076
		[RequiredField]
		[Tooltip("The name of the animation.")]
		public FsmString animationName;

		// Token: 0x040065DD RID: 26077
		[RequiredField]
		[Tooltip("The mixing transform to remove. E.g., root/upper_body/left_shoulder")]
		public FsmString transfrom;
	}
}
