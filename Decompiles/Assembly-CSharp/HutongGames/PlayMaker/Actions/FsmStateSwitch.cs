using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F4C RID: 3916
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName", false)]
	[Tooltip("Sends Events based on the current State of an FSM.")]
	public class FsmStateSwitch : FsmStateAction
	{
		// Token: 0x06006CE6 RID: 27878 RVA: 0x0021EDBF File Offset: 0x0021CFBF
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006CE7 RID: 27879 RVA: 0x0021EDEE File Offset: 0x0021CFEE
		public override void OnEnter()
		{
			this.DoFsmStateSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CE8 RID: 27880 RVA: 0x0021EE04 File Offset: 0x0021D004
		public override void OnUpdate()
		{
			this.DoFsmStateSwitch();
		}

		// Token: 0x06006CE9 RID: 27881 RVA: 0x0021EE0C File Offset: 0x0021D00C
		private void DoFsmStateSwitch()
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
			string activeStateName = this.fsm.ActiveStateName;
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (activeStateName == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04006CA0 RID: 27808
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmGameObject gameObject;

		// Token: 0x04006CA1 RID: 27809
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of Fsm on GameObject. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;

		// Token: 0x04006CA2 RID: 27810
		[CompoundArray("State Switches", "Compare State", "Send Event")]
		[Tooltip("Compare the current State to this State.")]
		public FsmString[] compareTo;

		// Token: 0x04006CA3 RID: 27811
		[Tooltip("Send this event if the same.")]
		public FsmEvent[] sendEvent;

		// Token: 0x04006CA4 RID: 27812
		[Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x04006CA5 RID: 27813
		private GameObject previousGo;

		// Token: 0x04006CA6 RID: 27814
		private PlayMakerFSM fsm;
	}
}
