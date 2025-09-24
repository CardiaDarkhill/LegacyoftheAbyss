using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010DC RID: 4316
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms a Direction from world space to a Game Object's local space. The opposite of TransformDirection.")]
	public class InverseTransformDirection : FsmStateAction
	{
		// Token: 0x060074CE RID: 29902 RVA: 0x0023BA6D File Offset: 0x00239C6D
		public override void Reset()
		{
			this.gameObject = null;
			this.worldDirection = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060074CF RID: 29903 RVA: 0x0023BA8B File Offset: 0x00239C8B
		public override void OnEnter()
		{
			this.DoInverseTransformDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074D0 RID: 29904 RVA: 0x0023BAA1 File Offset: 0x00239CA1
		public override void OnUpdate()
		{
			this.DoInverseTransformDirection();
		}

		// Token: 0x060074D1 RID: 29905 RVA: 0x0023BAAC File Offset: 0x00239CAC
		private void DoInverseTransformDirection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformDirection(this.worldDirection.Value);
		}

		// Token: 0x0400751E RID: 29982
		[RequiredField]
		[Tooltip("The game object that defines local space.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400751F RID: 29983
		[RequiredField]
		[Tooltip("The direction in world space.")]
		public FsmVector3 worldDirection;

		// Token: 0x04007520 RID: 29984
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		// Token: 0x04007521 RID: 29985
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
