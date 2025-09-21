using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010BD RID: 4285
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Set the value of a variable in another FSM.")]
	public class SetFsmVariable : FsmStateAction
	{
		// Token: 0x06007438 RID: 29752 RVA: 0x00239B2D File Offset: 0x00237D2D
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.setValue = new FsmVar();
		}

		// Token: 0x06007439 RID: 29753 RVA: 0x00239B51 File Offset: 0x00237D51
		public override void OnEnter()
		{
			this.DoSetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600743A RID: 29754 RVA: 0x00239B67 File Offset: 0x00237D67
		public override void OnUpdate()
		{
			this.DoSetFsmVariable();
		}

		// Token: 0x0600743B RID: 29755 RVA: 0x00239B70 File Offset: 0x00237D70
		private void DoSetFsmVariable()
		{
			if (this.setValue.IsNone || string.IsNullOrEmpty(this.variableName.Value))
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGameObject || this.fsmName.Value != this.cachedFsmName)
			{
				this.targetFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
				if (this.targetFsm == null)
				{
					return;
				}
				this.cachedGameObject = ownerDefaultTarget;
				this.cachedFsmName = this.fsmName.Value;
				this.cachedVariableName = string.Empty;
			}
			if (this.variableName.Value != this.cachedVariableName)
			{
				this.targetVariable = this.targetFsm.FsmVariables.FindVariable(this.setValue.Type, this.variableName.Value);
				this.cachedVariableName = this.variableName.Value;
			}
			if (this.targetVariable == null)
			{
				base.LogWarning("Missing Variable: " + this.variableName.Value);
				return;
			}
			this.setValue.UpdateValue();
			this.setValue.ApplyValueTo(this.targetVariable);
		}

		// Token: 0x0400746F RID: 29807
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007470 RID: 29808
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x04007471 RID: 29809
		[Tooltip("The name of the variable in the target FSM.")]
		public FsmString variableName;

		// Token: 0x04007472 RID: 29810
		[RequiredField]
		[Tooltip("Set the value.")]
		public FsmVar setValue;

		// Token: 0x04007473 RID: 29811
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007474 RID: 29812
		private PlayMakerFSM targetFsm;

		// Token: 0x04007475 RID: 29813
		private NamedVariable targetVariable;

		// Token: 0x04007476 RID: 29814
		private GameObject cachedGameObject;

		// Token: 0x04007477 RID: 29815
		private string cachedFsmName;

		// Token: 0x04007478 RID: 29816
		private string cachedVariableName;
	}
}
