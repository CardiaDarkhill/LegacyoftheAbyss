using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001090 RID: 4240
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Material Variable from another FSM.")]
	public class GetFsmMaterial : FsmStateAction
	{
		// Token: 0x06007359 RID: 29529 RVA: 0x00236C9F File Offset: 0x00234E9F
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600735A RID: 29530 RVA: 0x00236CD6 File Offset: 0x00234ED6
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600735B RID: 29531 RVA: 0x00236CEC File Offset: 0x00234EEC
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600735C RID: 29532 RVA: 0x00236CF4 File Offset: 0x00234EF4
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
			FsmMaterial fsmMaterial = this.fsm.FsmVariables.GetFsmMaterial(this.variableName.Value);
			if (fsmMaterial != null)
			{
				this.storeValue.Value = fsmMaterial.Value;
			}
		}

		// Token: 0x04007370 RID: 29552
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007371 RID: 29553
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007372 RID: 29554
		[RequiredField]
		[UIHint(UIHint.FsmMaterial)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x04007373 RID: 29555
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Material variable in this FSM.")]
		public FsmMaterial storeValue;

		// Token: 0x04007374 RID: 29556
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007375 RID: 29557
		private GameObject goLastFrame;

		// Token: 0x04007376 RID: 29558
		private string fsmNameLastFrame;

		// Token: 0x04007377 RID: 29559
		protected PlayMakerFSM fsm;
	}
}
