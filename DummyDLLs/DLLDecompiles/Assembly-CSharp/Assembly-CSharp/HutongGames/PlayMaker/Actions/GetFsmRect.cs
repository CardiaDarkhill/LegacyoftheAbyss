using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001093 RID: 4243
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Rect Variable from another FSM.")]
	public class GetFsmRect : FsmStateAction
	{
		// Token: 0x06007368 RID: 29544 RVA: 0x00237009 File Offset: 0x00235209
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.variableName = "";
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007369 RID: 29545 RVA: 0x00237040 File Offset: 0x00235240
		public override void OnEnter()
		{
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600736A RID: 29546 RVA: 0x00237056 File Offset: 0x00235256
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600736B RID: 29547 RVA: 0x00237060 File Offset: 0x00235260
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
			FsmRect fsmRect = this.fsm.FsmVariables.GetFsmRect(this.variableName.Value);
			if (fsmRect != null)
			{
				this.storeValue.Value = fsmRect.Value;
			}
		}

		// Token: 0x04007388 RID: 29576
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007389 RID: 29577
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x0400738A RID: 29578
		[RequiredField]
		[UIHint(UIHint.FsmRect)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x0400738B RID: 29579
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Rect variable in this FSM.")]
		public FsmRect storeValue;

		// Token: 0x0400738C RID: 29580
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x0400738D RID: 29581
		private GameObject goLastFrame;

		// Token: 0x0400738E RID: 29582
		private string fsmNameLastFrame;

		// Token: 0x0400738F RID: 29583
		protected PlayMakerFSM fsm;
	}
}
