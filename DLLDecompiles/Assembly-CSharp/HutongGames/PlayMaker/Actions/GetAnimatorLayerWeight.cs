using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFB RID: 3579
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the layer's current weight")]
	public class GetAnimatorLayerWeight : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x06006738 RID: 26424 RVA: 0x00209914 File Offset: 0x00207B14
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x0020991C File Offset: 0x00207B1C
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.layerWeight = null;
		}

		// Token: 0x0600673A RID: 26426 RVA: 0x00209939 File Offset: 0x00207B39
		public override void OnEnter()
		{
			this.GetLayerWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600673B RID: 26427 RVA: 0x0020994F File Offset: 0x00207B4F
		public override void OnActionUpdate()
		{
			this.GetLayerWeight();
		}

		// Token: 0x0600673C RID: 26428 RVA: 0x00209958 File Offset: 0x00207B58
		private void GetLayerWeight()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.layerWeight.Value = this.animator.GetLayerWeight(this.layerIndex.Value);
		}

		// Token: 0x0400668A RID: 26250
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400668B RID: 26251
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x0400668C RID: 26252
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's current weight")]
		public FsmFloat layerWeight;
	}
}
