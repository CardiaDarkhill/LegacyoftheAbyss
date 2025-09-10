using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010BA RID: 4282
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Rect Variable in another FSM.")]
	public class SetFsmRect : FsmStateAction
	{
		// Token: 0x06007429 RID: 29737 RVA: 0x00239731 File Offset: 0x00237931
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600742A RID: 29738 RVA: 0x00239768 File Offset: 0x00237968
		public override void OnEnter()
		{
			this.DoSetFsmBool();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600742B RID: 29739 RVA: 0x00239780 File Offset: 0x00237980
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
			FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
			if (fsmRect != null)
			{
				fsmRect.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x0600742C RID: 29740 RVA: 0x0023987D File Offset: 0x00237A7D
		public override void OnUpdate()
		{
			this.DoSetFsmBool();
		}

		// Token: 0x04007457 RID: 29783
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007458 RID: 29784
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007459 RID: 29785
		[RequiredField]
		[UIHint(UIHint.FsmRect)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400745A RID: 29786
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmRect setValue;

		// Token: 0x0400745B RID: 29787
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400745C RID: 29788
		private GameObject goLastFrame;

		// Token: 0x0400745D RID: 29789
		private string fsmNameLastFrame;

		// Token: 0x0400745E RID: 29790
		private PlayMakerFSM fsm;
	}
}
