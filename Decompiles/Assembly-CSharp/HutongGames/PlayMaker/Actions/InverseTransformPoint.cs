using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010DD RID: 4317
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms position from world space to a Game Object's local space. The opposite of TransformPoint.")]
	public class InverseTransformPoint : FsmStateAction
	{
		// Token: 0x060074D3 RID: 29907 RVA: 0x0023BAFE File Offset: 0x00239CFE
		public override void Reset()
		{
			this.gameObject = null;
			this.worldPosition = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060074D4 RID: 29908 RVA: 0x0023BB1C File Offset: 0x00239D1C
		public override void OnEnter()
		{
			this.DoInverseTransformPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074D5 RID: 29909 RVA: 0x0023BB32 File Offset: 0x00239D32
		public override void OnUpdate()
		{
			this.DoInverseTransformPoint();
		}

		// Token: 0x060074D6 RID: 29910 RVA: 0x0023BB3C File Offset: 0x00239D3C
		private void DoInverseTransformPoint()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformPoint(this.worldPosition.Value);
		}

		// Token: 0x04007522 RID: 29986
		[RequiredField]
		[Tooltip("The game object that defines local space.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007523 RID: 29987
		[RequiredField]
		[Tooltip("The world position vector.")]
		public FsmVector3 worldPosition;

		// Token: 0x04007524 RID: 29988
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the transformed vector in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		// Token: 0x04007525 RID: 29989
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
