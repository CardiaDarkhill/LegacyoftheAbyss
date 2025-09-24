using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200108A RID: 4234
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Bool Variable from another FSM.")]
	public class GetFsmBool : FsmStateAction
	{
		// Token: 0x0600733B RID: 29499 RVA: 0x0023663E File Offset: 0x0023483E
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x0600733C RID: 29500 RVA: 0x0023665E File Offset: 0x0023485E
		public override void OnEnter()
		{
			this.DoGetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600733D RID: 29501 RVA: 0x00236674 File Offset: 0x00234874
		public override void OnUpdate()
		{
			this.DoGetFsmBool();
		}

		// Token: 0x0600733E RID: 29502 RVA: 0x0023667C File Offset: 0x0023487C
		private void DoGetFsmBool()
		{
			if (this.storeValue == null)
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
				return;
			}
			FsmBool fsmBool = this.fsm.FsmVariables.GetFsmBool(this.variableName.Value);
			if (fsmBool == null)
			{
				return;
			}
			this.storeValue.Value = fsmBool.Value;
		}

		// Token: 0x04007340 RID: 29504
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007341 RID: 29505
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007342 RID: 29506
		[RequiredField]
		[UIHint(UIHint.FsmBool)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x04007343 RID: 29507
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Bool variable in this FSM.")]
		public FsmBool storeValue;

		// Token: 0x04007344 RID: 29508
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007345 RID: 29509
		private GameObject goLastFrame;

		// Token: 0x04007346 RID: 29510
		private string fsmNameLastFrame;

		// Token: 0x04007347 RID: 29511
		private PlayMakerFSM fsm;
	}
}
