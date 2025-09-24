using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001091 RID: 4241
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of an Object Variable from another FSM.")]
	public class GetFsmObject : FsmStateAction
	{
		// Token: 0x0600735E RID: 29534 RVA: 0x00236DC1 File Offset: 0x00234FC1
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600735F RID: 29535 RVA: 0x00236DF8 File Offset: 0x00234FF8
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007360 RID: 29536 RVA: 0x00236E0E File Offset: 0x0023500E
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x06007361 RID: 29537 RVA: 0x00236E18 File Offset: 0x00235018
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
			FsmObject fsmObject = this.fsm.FsmVariables.GetFsmObject(this.variableName.Value);
			if (fsmObject != null)
			{
				this.storeValue.Value = fsmObject.Value;
			}
		}

		// Token: 0x04007378 RID: 29560
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007379 RID: 29561
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400737A RID: 29562
		[RequiredField]
		[UIHint(UIHint.FsmObject)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x0400737B RID: 29563
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in an Object variable in this FSM.")]
		public FsmObject storeValue;

		// Token: 0x0400737C RID: 29564
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400737D RID: 29565
		private GameObject goLastFrame;

		// Token: 0x0400737E RID: 29566
		private string fsmNameLastFrame;

		// Token: 0x0400737F RID: 29567
		protected PlayMakerFSM fsm;
	}
}
