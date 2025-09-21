using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E09 RID: 3593
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Controls culling of this Animator component.")]
	public class SetAnimatorCullingMode : ComponentAction<Animator>
	{
		// Token: 0x06006786 RID: 26502 RVA: 0x0020A524 File Offset: 0x00208724
		public override void Reset()
		{
			this.gameObject = null;
			this.alwaysAnimate = null;
			this.cullCompletely = null;
		}

		// Token: 0x06006787 RID: 26503 RVA: 0x0020A53C File Offset: 0x0020873C
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.cullingMode = (this.alwaysAnimate.Value ? AnimatorCullingMode.AlwaysAnimate : AnimatorCullingMode.CullUpdateTransforms);
				if (this.cullCompletely.Value)
				{
					this.cachedComponent.cullingMode = AnimatorCullingMode.CullCompletely;
				}
			}
			base.Finish();
		}

		// Token: 0x040066C8 RID: 26312
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066C9 RID: 26313
		[Tooltip("If true, always animate the entire character. If false, animation updates are disabled when renderers are not visible")]
		public FsmBool alwaysAnimate;

		// Token: 0x040066CA RID: 26314
		[Tooltip("If true, animation is completely disabled when renderers are not visible")]
		public FsmBool cullCompletely;
	}
}
