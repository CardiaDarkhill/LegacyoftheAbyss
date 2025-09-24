using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001094 RID: 4244
	[ActionCategory(ActionCategory.StateMachine)]
	[ActionTarget(typeof(PlayMakerFSM), "fsmComponent", false)]
	[Tooltip("Gets the name of the specified FSMs current state. Either reference the fsm component directly, or find it on a game object.")]
	public class GetFsmState : FsmStateAction
	{
		// Token: 0x0600736D RID: 29549 RVA: 0x0023712D File Offset: 0x0023532D
		public override void Reset()
		{
			this.fsmComponent = null;
			this.gameObject = null;
			this.fsmName = "";
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600736E RID: 29550 RVA: 0x0023715B File Offset: 0x0023535B
		public override void OnEnter()
		{
			this.DoGetFsmState();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600736F RID: 29551 RVA: 0x00237171 File Offset: 0x00235371
		public override void OnUpdate()
		{
			this.DoGetFsmState();
		}

		// Token: 0x06007370 RID: 29552 RVA: 0x0023717C File Offset: 0x0023537C
		private void DoGetFsmState()
		{
			if (this.fsm == null)
			{
				if (this.fsmComponent != null)
				{
					this.fsm = this.fsmComponent;
				}
				else
				{
					GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
					if (ownerDefaultTarget != null)
					{
						this.fsm = ActionHelpers.GetGameObjectFsm(ownerDefaultTarget, this.fsmName.Value);
					}
				}
				if (this.fsm == null)
				{
					this.storeResult.Value = "";
					return;
				}
			}
			this.storeResult.Value = this.fsm.ActiveStateName;
		}

		// Token: 0x04007390 RID: 29584
		[Tooltip("Choose a PlayMakerFSM component. If you set a component here it overrides the Game Object and Fsm Name settings.")]
		public PlayMakerFSM fsmComponent;

		// Token: 0x04007391 RID: 29585
		[Tooltip("If not specifying the component above, specify the GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007392 RID: 29586
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object. If left blank it will find the first PlayMakerFSM on the GameObject.")]
		public FsmString fsmName;

		// Token: 0x04007393 RID: 29587
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the FSM's current State in a string variable.")]
		public FsmString storeResult;

		// Token: 0x04007394 RID: 29588
		[Tooltip("Repeat every frame. E.g.,  useful if you're waiting for the State to change.")]
		public bool everyFrame;

		// Token: 0x04007395 RID: 29589
		private PlayMakerFSM fsm;
	}
}
