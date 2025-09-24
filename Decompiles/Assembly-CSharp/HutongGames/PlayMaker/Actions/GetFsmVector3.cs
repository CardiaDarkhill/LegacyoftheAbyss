using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200109A RID: 4250
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Vector3 Variable from another FSM.")]
	public class GetFsmVector3 : FsmStateAction
	{
		// Token: 0x0600738D RID: 29581 RVA: 0x002378D7 File Offset: 0x00235AD7
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x0600738E RID: 29582 RVA: 0x002378F7 File Offset: 0x00235AF7
		public override void OnEnter()
		{
			this.DoGetFsmVector3();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600738F RID: 29583 RVA: 0x0023790D File Offset: 0x00235B0D
		public override void OnUpdate()
		{
			this.DoGetFsmVector3();
		}

		// Token: 0x06007390 RID: 29584 RVA: 0x00237918 File Offset: 0x00235B18
		private void DoGetFsmVector3()
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
			FsmVector3 fsmVector = this.fsm.FsmVariables.GetFsmVector3(this.variableName.Value);
			if (fsmVector == null)
			{
				return;
			}
			this.storeValue.Value = fsmVector.Value;
		}

		// Token: 0x040073C0 RID: 29632
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040073C1 RID: 29633
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040073C2 RID: 29634
		[RequiredField]
		[UIHint(UIHint.FsmVector3)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x040073C3 RID: 29635
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Vector3 variable in this FSM.")]
		public FsmVector3 storeValue;

		// Token: 0x040073C4 RID: 29636
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x040073C5 RID: 29637
		private GameObject goLastFrame;

		// Token: 0x040073C6 RID: 29638
		private string fsmNameLastFrame;

		// Token: 0x040073C7 RID: 29639
		private PlayMakerFSM fsm;
	}
}
