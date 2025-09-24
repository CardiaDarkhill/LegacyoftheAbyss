using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001098 RID: 4248
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Get the values of multiple variables in another FSM and store in variables of the same name in this FSM.")]
	public class GetFsmVariables : FsmStateAction
	{
		// Token: 0x06007382 RID: 29570 RVA: 0x002375EA File Offset: 0x002357EA
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = "";
			this.getVariables = null;
		}

		// Token: 0x06007383 RID: 29571 RVA: 0x0023760C File Offset: 0x0023580C
		private void InitFsmVars()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.cachedGO || this.cachedFsmName != this.fsmName.Value)
			{
				this.sourceVariables = new INamedVariable[this.getVariables.Length];
				this.targetVariables = new NamedVariable[this.getVariables.Length];
				for (int i = 0; i < this.getVariables.Length; i++)
				{
					string variableName = this.getVariables[i].variableName;
					this.sourceFsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
					this.sourceVariables[i] = this.sourceFsm.FsmVariables.GetVariable(variableName);
					this.targetVariables[i] = base.Fsm.Variables.GetVariable(variableName);
					this.getVariables[i].Type = this.targetVariables[i].VariableType;
					if (!string.IsNullOrEmpty(variableName) && this.sourceVariables[i] == null)
					{
						base.LogWarning("Missing Variable: " + variableName);
					}
					this.cachedGO = ownerDefaultTarget;
					this.cachedFsmName = this.fsmName.Value;
				}
			}
		}

		// Token: 0x06007384 RID: 29572 RVA: 0x00237749 File Offset: 0x00235949
		public override void OnEnter()
		{
			this.InitFsmVars();
			this.DoGetFsmVariables();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007385 RID: 29573 RVA: 0x00237765 File Offset: 0x00235965
		public override void OnUpdate()
		{
			this.DoGetFsmVariables();
		}

		// Token: 0x06007386 RID: 29574 RVA: 0x00237770 File Offset: 0x00235970
		private void DoGetFsmVariables()
		{
			this.InitFsmVars();
			for (int i = 0; i < this.getVariables.Length; i++)
			{
				this.getVariables[i].GetValueFrom(this.sourceVariables[i]);
				this.getVariables[i].ApplyValueTo(this.targetVariables[i]);
			}
		}

		// Token: 0x040073AF RID: 29615
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040073B0 RID: 29616
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040073B1 RID: 29617
		[RequiredField]
		[HideTypeFilter]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the values of the FsmVariables")]
		public FsmVar[] getVariables;

		// Token: 0x040073B2 RID: 29618
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040073B3 RID: 29619
		private GameObject cachedGO;

		// Token: 0x040073B4 RID: 29620
		private string cachedFsmName;

		// Token: 0x040073B5 RID: 29621
		private PlayMakerFSM sourceFsm;

		// Token: 0x040073B6 RID: 29622
		private INamedVariable[] sourceVariables;

		// Token: 0x040073B7 RID: 29623
		private NamedVariable[] targetVariables;
	}
}
