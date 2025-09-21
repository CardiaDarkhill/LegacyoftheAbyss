using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200108C RID: 4236
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of an Enum Variable from another FSM.")]
	public class GetFsmEnum : FsmStateAction
	{
		// Token: 0x06007345 RID: 29509 RVA: 0x0023685B File Offset: 0x00234A5B
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06007346 RID: 29510 RVA: 0x0023687B File Offset: 0x00234A7B
		public override void OnEnter()
		{
			this.DoGetFsmEnum();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007347 RID: 29511 RVA: 0x00236891 File Offset: 0x00234A91
		public override void OnUpdate()
		{
			this.DoGetFsmEnum();
		}

		// Token: 0x06007348 RID: 29512 RVA: 0x0023689C File Offset: 0x00234A9C
		private void DoGetFsmEnum()
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
			FsmEnum fsmEnum = this.fsm.FsmVariables.GetFsmEnum(this.variableName.Value);
			if (fsmEnum == null)
			{
				return;
			}
			this.storeValue.Value = fsmEnum.Value;
		}

		// Token: 0x04007350 RID: 29520
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007351 RID: 29521
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007352 RID: 29522
		[RequiredField]
		[UIHint(UIHint.FsmEnum)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x04007353 RID: 29523
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in an Enum variable in this FSM.")]
		public FsmEnum storeValue;

		// Token: 0x04007354 RID: 29524
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007355 RID: 29525
		private GameObject goLastFrame;

		// Token: 0x04007356 RID: 29526
		private string fsmNameLastFrame;

		// Token: 0x04007357 RID: 29527
		private PlayMakerFSM fsm;
	}
}
