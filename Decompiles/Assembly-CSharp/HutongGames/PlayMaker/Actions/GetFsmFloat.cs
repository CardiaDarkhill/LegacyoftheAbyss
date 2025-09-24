using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200108D RID: 4237
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Float Variable from another FSM.")]
	public class GetFsmFloat : FsmStateAction
	{
		// Token: 0x0600734A RID: 29514 RVA: 0x0023696B File Offset: 0x00234B6B
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x0600734B RID: 29515 RVA: 0x0023698B File Offset: 0x00234B8B
		public override void OnEnter()
		{
			this.DoGetFsmFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600734C RID: 29516 RVA: 0x002369A1 File Offset: 0x00234BA1
		public override void OnUpdate()
		{
			this.DoGetFsmFloat();
		}

		// Token: 0x0600734D RID: 29517 RVA: 0x002369AC File Offset: 0x00234BAC
		private void DoGetFsmFloat()
		{
			if (this.storeValue.IsNone)
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
			FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
			if (fsmFloat == null)
			{
				return;
			}
			this.storeValue.Value = fsmFloat.Value;
		}

		// Token: 0x04007358 RID: 29528
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007359 RID: 29529
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400735A RID: 29530
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x0400735B RID: 29531
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Float variable in this FSM.")]
		public FsmFloat storeValue;

		// Token: 0x0400735C RID: 29532
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400735D RID: 29533
		private GameObject goLastFrame;

		// Token: 0x0400735E RID: 29534
		private string fsmNameLastFrame;

		// Token: 0x0400735F RID: 29535
		private PlayMakerFSM fsm;
	}
}
