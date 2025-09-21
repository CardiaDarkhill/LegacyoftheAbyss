using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001081 RID: 4225
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Add to the value of an Integer Variable in another FSM.")]
	public class FsmIntAdd : FsmStateAction
	{
		// Token: 0x0600731E RID: 29470 RVA: 0x002360CE File Offset: 0x002342CE
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.addValue = null;
		}

		// Token: 0x0600731F RID: 29471 RVA: 0x002360EE File Offset: 0x002342EE
		public override void OnEnter()
		{
			this.DoAddFsmInt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007320 RID: 29472 RVA: 0x00236104 File Offset: 0x00234304
		private void DoAddFsmInt()
		{
			if (this.addValue == null)
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
			FsmInt fsmInt = this.fsm.FsmVariables.GetFsmInt(this.variableName.Value);
			if (fsmInt != null)
			{
				fsmInt.Value += this.addValue.Value;
				return;
			}
			base.LogWarning("Could not find variable: " + this.variableName.Value);
		}

		// Token: 0x06007321 RID: 29473 RVA: 0x00236208 File Offset: 0x00234408
		public override void OnUpdate()
		{
			this.DoAddFsmInt();
		}

		// Token: 0x04007320 RID: 29472
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007321 RID: 29473
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007322 RID: 29474
		[RequiredField]
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The name of the FSM variable.")]
		public FsmString variableName;

		// Token: 0x04007323 RID: 29475
		[RequiredField]
		[Tooltip("Set the value of the variable.")]
		public FsmInt addValue;

		// Token: 0x04007324 RID: 29476
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x04007325 RID: 29477
		private GameObject goLastFrame;

		// Token: 0x04007326 RID: 29478
		private string fsmNameLastFrame;

		// Token: 0x04007327 RID: 29479
		private PlayMakerFSM fsm;
	}
}
