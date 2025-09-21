using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF8 RID: 3576
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the Animator controller layer count")]
	public class GetAnimatorLayerCount : ComponentAction<Animator>
	{
		// Token: 0x0600672F RID: 26415 RVA: 0x002097CF File Offset: 0x002079CF
		public override void Reset()
		{
			this.gameObject = null;
			this.layerCount = null;
		}

		// Token: 0x06006730 RID: 26416 RVA: 0x002097DF File Offset: 0x002079DF
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.layerCount.Value = this.cachedComponent.layerCount;
			}
			base.Finish();
		}

		// Token: 0x04006681 RID: 26241
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006682 RID: 26242
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Animator controller layer count")]
		public FsmInt layerCount;
	}
}
