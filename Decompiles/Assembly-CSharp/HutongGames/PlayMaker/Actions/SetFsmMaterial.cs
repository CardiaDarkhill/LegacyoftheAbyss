using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010B7 RID: 4279
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Material Variable in another FSM.")]
	public class SetFsmMaterial : FsmStateAction
	{
		// Token: 0x0600741A RID: 29722 RVA: 0x0023931D File Offset: 0x0023751D
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600741B RID: 29723 RVA: 0x00239354 File Offset: 0x00237554
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600741C RID: 29724 RVA: 0x0023936C File Offset: 0x0023756C
		private void DoSetFsmBool()
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
			FsmMaterial fsmMaterial = this.fsm.FsmVariables.GetFsmMaterial(this.variableName.Value);
			if (fsmMaterial != null)
			{
				fsmMaterial.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x0600741D RID: 29725 RVA: 0x00239469 File Offset: 0x00237669
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x0400743F RID: 29759
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007440 RID: 29760
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007441 RID: 29761
		[RequiredField]
		[UIHint(UIHint.FsmMaterial)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04007442 RID: 29762
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmMaterial setValue;

		// Token: 0x04007443 RID: 29763
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007444 RID: 29764
		private GameObject goLastFrame;

		// Token: 0x04007445 RID: 29765
		private string fsmNameLastFrame;

		// Token: 0x04007446 RID: 29766
		private PlayMakerFSM fsm;
	}
}
