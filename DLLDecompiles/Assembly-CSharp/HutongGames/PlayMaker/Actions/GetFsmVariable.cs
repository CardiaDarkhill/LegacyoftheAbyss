using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001097 RID: 4247
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the value of a variable in another FSM and store it in a variable of the same name in this FSM.")]
	public class GetFsmVariable : FsmStateAction
	{
		// Token: 0x0600737C RID: 29564 RVA: 0x00237451 File Offset: 0x00235651
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.storeValue = new FsmVar();
		}

		// Token: 0x0600737D RID: 29565 RVA: 0x00237475 File Offset: 0x00235675
		public override void OnEnter()
		{
			this.InitFsmVar();
			this.DoGetFsmVariable();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600737E RID: 29566 RVA: 0x00237491 File Offset: 0x00235691
		public override void OnUpdate()
		{
			this.DoGetFsmVariable();
		}

		// Token: 0x0600737F RID: 29567 RVA: 0x0023749C File Offset: 0x0023569C
		private void InitFsmVar()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGO || this.cachedFsmName != this.fsmName.Value)
			{
				this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
				this.sourceVariable = this.sourceFsm.FsmVariables.GetVariable(this.storeValue.variableName);
				this.targetVariable = base.Fsm.Variables.GetVariable(this.storeValue.variableName);
				this.storeValue.Type = this.targetVariable.VariableType;
				if (!string.IsNullOrEmpty(this.storeValue.variableName) && this.sourceVariable == null)
				{
					base.LogWarning("Missing Variable: " + this.storeValue.variableName);
				}
				this.cachedGO = ownerDefaultTarget;
				this.cachedFsmName = this.fsmName.Value;
			}
		}

		// Token: 0x06007380 RID: 29568 RVA: 0x002375AA File Offset: 0x002357AA
		private void DoGetFsmVariable()
		{
			if (this.storeValue.IsNone)
			{
				return;
			}
			this.InitFsmVar();
			this.storeValue.GetValueFrom(this.sourceVariable);
			this.storeValue.ApplyValueTo(this.targetVariable);
		}

		// Token: 0x040073A6 RID: 29606
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040073A7 RID: 29607
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040073A8 RID: 29608
		[RequiredField]
		[HideTypeFilter]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value of the FsmVariable")]
		public FsmVar storeValue;

		// Token: 0x040073A9 RID: 29609
		[Tooltip("Repeat every frame. Useful if the value is changing.")]
		public bool everyFrame;

		// Token: 0x040073AA RID: 29610
		private GameObject cachedGO;

		// Token: 0x040073AB RID: 29611
		private string cachedFsmName;

		// Token: 0x040073AC RID: 29612
		private PlayMakerFSM sourceFsm;

		// Token: 0x040073AD RID: 29613
		private INamedVariable sourceVariable;

		// Token: 0x040073AE RID: 29614
		private NamedVariable targetVariable;
	}
}
