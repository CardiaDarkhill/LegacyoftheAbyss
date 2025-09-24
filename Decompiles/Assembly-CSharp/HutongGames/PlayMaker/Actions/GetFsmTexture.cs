using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001096 RID: 4246
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Texture Variable from another FSM.")]
	public class GetFsmTexture : FsmStateAction
	{
		// Token: 0x06007377 RID: 29559 RVA: 0x0023732F File Offset: 0x0023552F
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007378 RID: 29560 RVA: 0x00237366 File Offset: 0x00235566
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007379 RID: 29561 RVA: 0x0023737C File Offset: 0x0023557C
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600737A RID: 29562 RVA: 0x00237384 File Offset: 0x00235584
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
			FsmTexture fsmTexture = this.fsm.FsmVariables.GetFsmTexture(this.variableName.Value);
			if (fsmTexture != null)
			{
				this.storeValue.Value = fsmTexture.Value;
			}
		}

		// Token: 0x0400739E RID: 29598
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400739F RID: 29599
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040073A0 RID: 29600
		[RequiredField]
		[UIHint(UIHint.FsmTexture)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x040073A1 RID: 29601
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Texture variable in this FSM.")]
		public FsmTexture storeValue;

		// Token: 0x040073A2 RID: 29602
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x040073A3 RID: 29603
		private GameObject goLastFrame;

		// Token: 0x040073A4 RID: 29604
		private string fsmNameLastFrame;

		// Token: 0x040073A5 RID: 29605
		protected PlayMakerFSM fsm;
	}
}
