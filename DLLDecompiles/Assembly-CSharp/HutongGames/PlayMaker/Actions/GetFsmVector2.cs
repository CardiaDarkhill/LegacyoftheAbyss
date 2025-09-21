using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001099 RID: 4249
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a Vector2 Variable from another FSM.")]
	public class GetFsmVector2 : FsmStateAction
	{
		// Token: 0x06007388 RID: 29576 RVA: 0x002377C8 File Offset: 0x002359C8
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = null;
		}

		// Token: 0x06007389 RID: 29577 RVA: 0x002377E8 File Offset: 0x002359E8
		public override void OnEnter()
		{
			this.DoGetFsmVector2();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600738A RID: 29578 RVA: 0x002377FE File Offset: 0x002359FE
		public override void OnUpdate()
		{
			this.DoGetFsmVector2();
		}

		// Token: 0x0600738B RID: 29579 RVA: 0x00237808 File Offset: 0x00235A08
		private void DoGetFsmVector2()
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
			FsmVector2 fsmVector = this.fsm.FsmVariables.GetFsmVector2(this.variableName.Value);
			if (fsmVector == null)
			{
				return;
			}
			this.storeValue.Value = fsmVector.Value;
		}

		// Token: 0x040073B8 RID: 29624
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040073B9 RID: 29625
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040073BA RID: 29626
		[RequiredField]
		[UIHint(UIHint.FsmVector2)]
		[Tooltip("The name of the FSM variable to get.")]
		public FsmString variableName;

		// Token: 0x040073BB RID: 29627
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Vector2 variable in this FSM.")]
		public FsmVector2 storeValue;

		// Token: 0x040073BC RID: 29628
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x040073BD RID: 29629
		private GameObject goLastFrame;

		// Token: 0x040073BE RID: 29630
		private string fsmNameLastFrame;

		// Token: 0x040073BF RID: 29631
		private PlayMakerFSM fsm;
	}
}
