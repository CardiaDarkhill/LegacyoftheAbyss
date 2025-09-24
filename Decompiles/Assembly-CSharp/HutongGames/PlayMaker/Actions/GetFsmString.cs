using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001095 RID: 4245
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a String Variable from another FSM.")]
	public class GetFsmString : FsmStateAction
	{
		// Token: 0x06007372 RID: 29554 RVA: 0x00237222 File Offset: 0x00235422
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06007373 RID: 29555 RVA: 0x00237242 File Offset: 0x00235442
		public override void OnEnter()
		{
			this.DoGetFsmString();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007374 RID: 29556 RVA: 0x00237258 File Offset: 0x00235458
		public override void OnUpdate()
		{
			this.DoGetFsmString();
		}

		// Token: 0x06007375 RID: 29557 RVA: 0x00237260 File Offset: 0x00235460
		private void DoGetFsmString()
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
			FsmString fsmString = this.fsm.FsmVariables.GetFsmString(this.variableName.Value);
			if (fsmString == null)
			{
				return;
			}
			this.storeValue.Value = fsmString.Value;
		}

		// Token: 0x04007396 RID: 29590
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007397 RID: 29591
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007398 RID: 29592
		[RequiredField]
		[UIHint(UIHint.FsmString)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x04007399 RID: 29593
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a String variable in this FSM.")]
		public FsmString storeValue;

		// Token: 0x0400739A RID: 29594
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400739B RID: 29595
		private GameObject goLastFrame;

		// Token: 0x0400739C RID: 29596
		private string fsmNameLastFrame;

		// Token: 0x0400739D RID: 29597
		private PlayMakerFSM fsm;
	}
}
