using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF9 RID: 3577
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the name of a layer from its index")]
	public class GetAnimatorLayerName : ComponentAction<Animator>
	{
		// Token: 0x06006732 RID: 26418 RVA: 0x0020981E File Offset: 0x00207A1E
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.layerName = null;
		}

		// Token: 0x06006733 RID: 26419 RVA: 0x00209838 File Offset: 0x00207A38
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.layerName.Value = this.cachedComponent.GetLayerName(this.layerIndex.Value);
			}
			base.Finish();
		}

		// Token: 0x04006683 RID: 26243
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006684 RID: 26244
		[RequiredField]
		[Tooltip("The layer index")]
		public FsmInt layerIndex;

		// Token: 0x04006685 RID: 26245
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer name")]
		public FsmString layerName;
	}
}
