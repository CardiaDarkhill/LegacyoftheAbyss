using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010BE RID: 4286
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Vector2 Variable in another FSM.")]
	public class SetFsmVector2 : FsmStateAction
	{
		// Token: 0x0600743D RID: 29757 RVA: 0x00239CC5 File Offset: 0x00237EC5
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = null;
		}

		// Token: 0x0600743E RID: 29758 RVA: 0x00239CE5 File Offset: 0x00237EE5
		public override void OnEnter()
		{
			this.DoSetFsmVector2();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600743F RID: 29759 RVA: 0x00239CFC File Offset: 0x00237EFC
		private void DoSetFsmVector2()
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
			FsmVector2 fsmVector = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
			if (fsmVector != null)
			{
				fsmVector.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007440 RID: 29760 RVA: 0x00239DF9 File Offset: 0x00237FF9
		public override void OnUpdate()
		{
			this.DoSetFsmVector2();
		}

		// Token: 0x04007479 RID: 29817
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400747A RID: 29818
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400747B RID: 29819
		[RequiredField]
		[UIHint(UIHint.FsmVector2)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400747C RID: 29820
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmVector2 setValue;

		// Token: 0x0400747D RID: 29821
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400747E RID: 29822
		private GameObject goLastFrame;

		// Token: 0x0400747F RID: 29823
		private string fsmNameLastFrame;

		// Token: 0x04007480 RID: 29824
		private PlayMakerFSM fsm;
	}
}
