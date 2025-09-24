using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001092 RID: 4242
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Quaternion Variable from another FSM.")]
	public class GetFsmQuaternion : FsmStateAction
	{
		// Token: 0x06007363 RID: 29539 RVA: 0x00236EE5 File Offset: 0x002350E5
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007364 RID: 29540 RVA: 0x00236F1C File Offset: 0x0023511C
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007365 RID: 29541 RVA: 0x00236F32 File Offset: 0x00235132
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x06007366 RID: 29542 RVA: 0x00236F3C File Offset: 0x0023513C
		private void DoGetFsmVariable()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.goLastFrame || this.fsmName.Value != this.fsmNameLastFrame)
			{
				this.goLastFrame = ownerDefaultTarget;
				this.fsmNameLastFrame = this.fsmName.Value;
				this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
			}
			if (this.fsm == null || this.storeValue == null)
			{
				return;
			}
			FsmQuaternion fsmQuaternion = this.fsm.FsmVariables.GetFsmQuaternion(this.variableName.Value);
			if (fsmQuaternion != null)
			{
				this.storeValue.Value = fsmQuaternion.Value;
			}
		}

		// Token: 0x04007380 RID: 29568
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007381 RID: 29569
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007382 RID: 29570
		[RequiredField]
		[UIHint(UIHint.FsmQuaternion)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x04007383 RID: 29571
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Quaternion variable in this FSM.")]
		public FsmQuaternion storeValue;

		// Token: 0x04007384 RID: 29572
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007385 RID: 29573
		private GameObject goLastFrame;

		// Token: 0x04007386 RID: 29574
		private string fsmNameLastFrame;

		// Token: 0x04007387 RID: 29575
		protected PlayMakerFSM fsm;
	}
}
