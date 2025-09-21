using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B1 RID: 4273
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Bool Variable in another FSM.")]
	public class SetFsmBool : FsmStateAction
	{
		// Token: 0x060073FC RID: 29692 RVA: 0x00238BB1 File Offset: 0x00236DB1
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x060073FD RID: 29693 RVA: 0x00238BD1 File Offset: 0x00236DD1
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060073FE RID: 29694 RVA: 0x00238BE8 File Offset: 0x00236DE8
		private void DoSetFsmBool()
		{
			if (this.setValue == null)
			{
				return;
			}
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
			if (this.fsm == null)
			{
				base.LogWarning("Could not find FSM: " + this.fsmName.Value);
				return;
			}
			FsmBool fsmBool = this.fsm.FsmVariables.FindFsmBool(this.variableName.Value);
			if (fsmBool != null)
			{
				fsmBool.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x060073FF RID: 29695 RVA: 0x00238CE5 File Offset: 0x00236EE5
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x0400740F RID: 29711
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007410 RID: 29712
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007411 RID: 29713
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04007412 RID: 29714
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmBool setValue;

		// Token: 0x04007413 RID: 29715
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007414 RID: 29716
		private GameObject goLastFrame;

		// Token: 0x04007415 RID: 29717
		private string fsmNameLastFrame;

		// Token: 0x04007416 RID: 29718
		private PlayMakerFSM fsm;
	}
}
