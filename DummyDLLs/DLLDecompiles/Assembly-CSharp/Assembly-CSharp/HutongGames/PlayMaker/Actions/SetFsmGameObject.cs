using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B5 RID: 4277
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Game Object Variable in another FSM. Accept null reference")]
	public class SetFsmGameObject : FsmStateAction
	{
		// Token: 0x06007410 RID: 29712 RVA: 0x002390A6 File Offset: 0x002372A6
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007411 RID: 29713 RVA: 0x002390CD File Offset: 0x002372CD
		public override void OnEnter()
		{
			this.DoSetFsmGameObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007412 RID: 29714 RVA: 0x002390E4 File Offset: 0x002372E4
		private void DoSetFsmGameObject()
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
			if (this.fsm == null)
			{
				return;
			}
			FsmGameObject fsmGameObject = this.fsm.FsmVariables.FindFsmGameObject(this.variableName.Value);
			if (fsmGameObject != null)
			{
				fsmGameObject.Value = ((this.setValue == null) ? null : this.setValue.Value);
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007413 RID: 29715 RVA: 0x002391C8 File Offset: 0x002373C8
		public override void OnUpdate()
		{
			this.DoSetFsmGameObject();
		}

		// Token: 0x0400742F RID: 29743
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007430 RID: 29744
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007431 RID: 29745
		[RequiredField]
		[UIHint(UIHint.FsmGameObject)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04007432 RID: 29746
		[Tooltip("Set the value of the variable.")]
		public FsmGameObject setValue;

		// Token: 0x04007433 RID: 29747
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007434 RID: 29748
		private GameObject goLastFrame;

		// Token: 0x04007435 RID: 29749
		private string fsmNameLastFrame;

		// Token: 0x04007436 RID: 29750
		private PlayMakerFSM fsm;
	}
}
