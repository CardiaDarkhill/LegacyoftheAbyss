using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200108B RID: 4235
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Color Variable from another FSM.")]
	public class GetFsmColor : FsmStateAction
	{
		// Token: 0x06007340 RID: 29504 RVA: 0x0023674B File Offset: 0x0023494B
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06007341 RID: 29505 RVA: 0x0023676B File Offset: 0x0023496B
		public override void OnEnter()
		{
			this.DoGetFsmColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007342 RID: 29506 RVA: 0x00236781 File Offset: 0x00234981
		public override void OnUpdate()
		{
			this.DoGetFsmColor();
		}

		// Token: 0x06007343 RID: 29507 RVA: 0x0023678C File Offset: 0x0023498C
		private void DoGetFsmColor()
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
			FsmColor fsmColor = this.fsm.FsmVariables.GetFsmColor(this.variableName.Value);
			if (fsmColor == null)
			{
				return;
			}
			this.storeValue.Value = fsmColor.Value;
		}

		// Token: 0x04007348 RID: 29512
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007349 RID: 29513
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400734A RID: 29514
		[RequiredField]
		[UIHint(UIHint.FsmColor)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x0400734B RID: 29515
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Color variable in this FSM.")]
		public FsmColor storeValue;

		// Token: 0x0400734C RID: 29516
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400734D RID: 29517
		private GameObject goLastFrame;

		// Token: 0x0400734E RID: 29518
		private string fsmNameLastFrame;

		// Token: 0x0400734F RID: 29519
		private PlayMakerFSM fsm;
	}
}
