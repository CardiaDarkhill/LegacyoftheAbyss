using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010BF RID: 4287
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Vector3 Variable in another FSM.")]
	public class SetFsmVector3 : FsmStateAction
	{
		// Token: 0x06007442 RID: 29762 RVA: 0x00239E09 File Offset: 0x00238009
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x06007443 RID: 29763 RVA: 0x00239E29 File Offset: 0x00238029
		public override void OnEnter()
		{
			this.DoSetFsmVector3();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007444 RID: 29764 RVA: 0x00239E40 File Offset: 0x00238040
		private void DoSetFsmVector3()
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
			FsmVector3 fsmVector = this.fsm.FsmVariables.GetFsmVector3(this.variableName.Value);
			if (fsmVector != null)
			{
				fsmVector.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007445 RID: 29765 RVA: 0x00239F3D File Offset: 0x0023813D
		public override void OnUpdate()
		{
			this.DoSetFsmVector3();
		}

		// Token: 0x04007481 RID: 29825
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007482 RID: 29826
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007483 RID: 29827
		[RequiredField]
		[UIHint(UIHint.FsmVector3)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04007484 RID: 29828
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmVector3 setValue;

		// Token: 0x04007485 RID: 29829
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007486 RID: 29830
		private GameObject goLastFrame;

		// Token: 0x04007487 RID: 29831
		private string fsmNameLastFrame;

		// Token: 0x04007488 RID: 29832
		private PlayMakerFSM fsm;
	}
}
