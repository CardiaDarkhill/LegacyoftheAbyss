using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200108F RID: 4239
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of an Integer Variable from another FSM.")]
	public class GetFsmInt : FsmStateAction
	{
		// Token: 0x06007354 RID: 29524 RVA: 0x00236B8F File Offset: 0x00234D8F
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06007355 RID: 29525 RVA: 0x00236BAF File Offset: 0x00234DAF
		public override void OnEnter()
		{
			this.DoGetFsmInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007356 RID: 29526 RVA: 0x00236BC5 File Offset: 0x00234DC5
		public override void OnUpdate()
		{
			this.DoGetFsmInt();
		}

		// Token: 0x06007357 RID: 29527 RVA: 0x00236BD0 File Offset: 0x00234DD0
		private void DoGetFsmInt()
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
			FsmInt fsmInt = this.fsm.FsmVariables.GetFsmInt(this.variableName.Value);
			if (fsmInt == null)
			{
				return;
			}
			this.storeValue.Value = fsmInt.Value;
		}

		// Token: 0x04007368 RID: 29544
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007369 RID: 29545
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400736A RID: 29546
		[RequiredField]
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x0400736B RID: 29547
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in an Int variable in this FSM.")]
		public FsmInt storeValue;

		// Token: 0x0400736C RID: 29548
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400736D RID: 29549
		private GameObject goLastFrame;

		// Token: 0x0400736E RID: 29550
		private string fsmNameLastFrame;

		// Token: 0x0400736F RID: 29551
		private PlayMakerFSM fsm;
	}
}
