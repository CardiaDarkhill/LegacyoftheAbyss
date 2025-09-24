using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F4D RID: 3917
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Tests if an FSM is in the specified State.")]
	public class FsmStateTest : FsmStateAction
	{
		// Token: 0x06006CEB RID: 27883 RVA: 0x0021EEBA File Offset: 0x0021D0BA
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.stateName = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CEC RID: 27884 RVA: 0x0021EEED File Offset: 0x0021D0ED
		public override void OnEnter()
		{
			this.DoFsmStateTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CED RID: 27885 RVA: 0x0021EF03 File Offset: 0x0021D103
		public override void OnUpdate()
		{
			this.DoFsmStateTest();
		}

		// Token: 0x06006CEE RID: 27886 RVA: 0x0021EF0C File Offset: 0x0021D10C
		private void DoFsmStateTest()
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
			if (this.fsm.ActiveStateName == this.stateName.Value)
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

		// Token: 0x04006CA7 RID: 27815
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		// Token: 0x04006CA8 RID: 27816
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of Fsm on Game Object. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;

		// Token: 0x04006CA9 RID: 27817
		[RequiredField]
		[Tooltip("Check to see if the FSM is in this state.")]
		public FsmString stateName;

		// Token: 0x04006CAA RID: 27818
		[Tooltip("Event to send if the FSM is in the specified state.")]
		public FsmEvent trueEvent;

		// Token: 0x04006CAB RID: 27819
		[Tooltip("Event to send if the FSM is NOT in the specified state.")]
		public FsmEvent falseEvent;

		// Token: 0x04006CAC RID: 27820
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of this test in a bool variable. Useful if other actions depend on this test.")]
		public FsmBool storeResult;

		// Token: 0x04006CAD RID: 27821
		[Tooltip("Repeat every frame. Useful if you want to wait for an FSM to be in a particular state before sending an event.")]
		public bool everyFrame;

		// Token: 0x04006CAE RID: 27822
		private GameObject previousGo;

		// Token: 0x04006CAF RID: 27823
		private PlayMakerFSM fsm;
	}
}
