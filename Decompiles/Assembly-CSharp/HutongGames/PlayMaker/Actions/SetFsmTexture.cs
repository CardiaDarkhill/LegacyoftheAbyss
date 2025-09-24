using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010BC RID: 4284
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a Texture Variable in another FSM.")]
	public class SetFsmTexture : FsmStateAction
	{
		// Token: 0x06007433 RID: 29747 RVA: 0x002399D1 File Offset: 0x00237BD1
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.setValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007434 RID: 29748 RVA: 0x00239A08 File Offset: 0x00237C08
		public override void OnEnter()
		{
			this.DoSetFsmTexture();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007435 RID: 29749 RVA: 0x00239A20 File Offset: 0x00237C20
		private void DoSetFsmTexture()
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
			FsmTexture fsmTexture = this.fsm.FsmVariables.FindFsmTexture(this.variableName.Value);
			if (fsmTexture != null)
			{
				fsmTexture.Value = this.setValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007436 RID: 29750 RVA: 0x00239B1D File Offset: 0x00237D1D
		public override void OnUpdate()
		{
			this.DoSetFsmTexture();
		}

		// Token: 0x04007467 RID: 29799
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007468 RID: 29800
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007469 RID: 29801
		[RequiredField]
		[UIHint(UIHint.FsmTexture)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x0400746A RID: 29802
		[Tooltip("Set the value of the variable.")]
		public FsmTexture setValue;

		// Token: 0x0400746B RID: 29803
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400746C RID: 29804
		private GameObject goLastFrame;

		// Token: 0x0400746D RID: 29805
		private string fsmNameLastFrame;

		// Token: 0x0400746E RID: 29806
		private PlayMakerFSM fsm;
	}
}
