using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B3 RID: 4275
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of an Enum Variable in another FSM.")]
	public class SetFsmEnum : FsmStateAction
	{
		// Token: 0x06007406 RID: 29702 RVA: 0x00238E39 File Offset: 0x00237039
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x06007407 RID: 29703 RVA: 0x00238E59 File Offset: 0x00237059
		public override void OnEnter()
		{
			this.DoSetFsmEnum();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007408 RID: 29704 RVA: 0x00238E70 File Offset: 0x00237070
		private void DoSetFsmEnum()
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
			FsmEnum fsmEnum = this.fsm.FsmVariables.GetFsmEnum(this.variableName.Value);
			if (fsmEnum != null)
			{
				fsmEnum.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007409 RID: 29705 RVA: 0x00238F6D File Offset: 0x0023716D
		public override void OnUpdate()
		{
			this.DoSetFsmEnum();
		}

		// Token: 0x0400741F RID: 29727
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007420 RID: 29728
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object.")]
		public FsmString fsmName;

		// Token: 0x04007421 RID: 29729
		[RequiredField]
		[UIHint(UIHint.FsmEnum)]
		[Tooltip("Enum variable name needs to match the FSM variable name on Game Object.")]
		public FsmString variableName;

		// Token: 0x04007422 RID: 29730
		[RequiredField]
		[Tooltip("Set the value of the Enum Variable.")]
		public FsmEnum setValue;

		// Token: 0x04007423 RID: 29731
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007424 RID: 29732
		private GameObject goLastFrame;

		// Token: 0x04007425 RID: 29733
		private string fsmNameLastFrame;

		// Token: 0x04007426 RID: 29734
		private PlayMakerFSM fsm;
	}
}
