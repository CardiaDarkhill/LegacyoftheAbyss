using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E0F RID: 3599
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the layer's current weight")]
	public class SetAnimatorLayerWeight : ComponentAction<Animator>
	{
		// Token: 0x060067A2 RID: 26530 RVA: 0x0020AABB File Offset: 0x00208CBB
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.layerWeight = null;
			this.everyFrame = false;
		}

		// Token: 0x060067A3 RID: 26531 RVA: 0x0020AAD9 File Offset: 0x00208CD9
		public override void OnEnter()
		{
			this.DoLayerWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067A4 RID: 26532 RVA: 0x0020AAEF File Offset: 0x00208CEF
		public override void OnUpdate()
		{
			this.DoLayerWeight();
		}

		// Token: 0x060067A5 RID: 26533 RVA: 0x0020AAF7 File Offset: 0x00208CF7
		private void DoLayerWeight()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.SetLayerWeight(this.layerIndex.Value, this.layerWeight.Value);
			}
		}

		// Token: 0x040066E4 RID: 26340
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066E5 RID: 26341
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x040066E6 RID: 26342
		[RequiredField]
		[Tooltip("Sets the layer's current weight")]
		public FsmFloat layerWeight;

		// Token: 0x040066E7 RID: 26343
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;
	}
}
