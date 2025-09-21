using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B4 RID: 4276
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Float Variable in another FSM.")]
	public class SetFsmFloat : FsmStateAction
	{
		// Token: 0x0600740B RID: 29707 RVA: 0x00238F7D File Offset: 0x0023717D
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x0600740C RID: 29708 RVA: 0x00238F9D File Offset: 0x0023719D
		public override void OnEnter()
		{
			this.DoSetFsmFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600740D RID: 29709 RVA: 0x00238FB4 File Offset: 0x002371B4
		private void DoSetFsmFloat()
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
				return;
			}
			FsmFloat fsmFloat = this.fsm.FsmVariables.GetFsmFloat(this.variableName.Value);
			if (fsmFloat != null)
			{
				fsmFloat.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x0600740E RID: 29710 RVA: 0x00239096 File Offset: 0x00237296
		public override void OnUpdate()
		{
			this.DoSetFsmFloat();
		}

		// Token: 0x04007427 RID: 29735
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007428 RID: 29736
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007429 RID: 29737
		[RequiredField]
		[UIHint(UIHint.FsmFloat)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400742A RID: 29738
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmFloat setValue;

		// Token: 0x0400742B RID: 29739
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400742C RID: 29740
		private GameObject goLastFrame;

		// Token: 0x0400742D RID: 29741
		private string fsmNameLastFrame;

		// Token: 0x0400742E RID: 29742
		private PlayMakerFSM fsm;
	}
}
