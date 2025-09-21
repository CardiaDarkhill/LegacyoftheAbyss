using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F4B RID: 3915
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Tests if an FSM has a variable with the given name.")]
	public class FsmHasVariable : FsmStateAction
	{
		// Token: 0x06006CE1 RID: 27873 RVA: 0x0021ECB4 File Offset: 0x0021CEB4
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.variableName = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CE2 RID: 27874 RVA: 0x0021ECE7 File Offset: 0x0021CEE7
		public override void OnEnter()
		{
			this.DoFsmVariableTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CE3 RID: 27875 RVA: 0x0021ECFD File Offset: 0x0021CEFD
		public override void OnUpdate()
		{
			this.DoFsmVariableTest();
		}

		// Token: 0x06006CE4 RID: 27876 RVA: 0x0021ED08 File Offset: 0x0021CF08
		private void DoFsmVariableTest()
		{
			GameObject value = this.gameObject.Value;
			if (value == null)
			{
				return;
			}
			if (value != this.previousGo)
			{
				this.fsm = ActionHelpers.GetGameObjectFsm(value, this.fsmName.Value);
				this.previousGo = value;
			}
			if (this.fsm == null)
			{
				return;
			}
			bool value2 = false;
			if (this.fsm.FsmVariables.Contains(this.variableName.Value))
			{
				base.Fsm.Event(this.trueEvent);
				value2 = true;
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
			this.storeResult.Value = value2;
		}

		// Token: 0x04006C97 RID: 27799
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		// Token: 0x04006C98 RID: 27800
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of Fsm on Game Object. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;

		// Token: 0x04006C99 RID: 27801
		[RequiredField]
		[Tooltip("Check to see if the FSM has this variable.")]
		public FsmString variableName;

		// Token: 0x04006C9A RID: 27802
		[Tooltip("Event to send if the FSM has the variable.")]
		public FsmEvent trueEvent;

		// Token: 0x04006C9B RID: 27803
		[Tooltip("Event to send if the FSM does NOT have the variable.")]
		public FsmEvent falseEvent;

		// Token: 0x04006C9C RID: 27804
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of this test in a bool variable. Useful if other actions depend on this test.")]
		public FsmBool storeResult;

		// Token: 0x04006C9D RID: 27805
		[Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x04006C9E RID: 27806
		private GameObject previousGo;

		// Token: 0x04006C9F RID: 27807
		private PlayMakerFSM fsm;
	}
}
