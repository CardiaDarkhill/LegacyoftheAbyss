using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A9 RID: 4265
	[Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event to another Fsm after an optional delay. Specify an Fsm Name or use the first Fsm on the object.")]
	public class SendEventToFsm : FsmStateAction
	{
		// Token: 0x060073DD RID: 29661 RVA: 0x0023858E File Offset: 0x0023678E
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.sendEvent = null;
			this.delay = null;
			this.requireReceiver = false;
		}

		// Token: 0x060073DE RID: 29662 RVA: 0x002385B4 File Offset: 0x002367B4
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(this.go, this.fsmName.Value);
			if (gameObjectFsm == null)
			{
				if (this.requireReceiver)
				{
					base.LogError("GameObject doesn't have FsmComponent: " + this.go.name + " " + this.fsmName.Value);
				}
				return;
			}
			if ((double)this.delay.Value < 0.001)
			{
				gameObjectFsm.Fsm.Event(this.sendEvent.Value);
				base.Finish();
				return;
			}
			this.delayedEvent = gameObjectFsm.Fsm.DelayedEvent(FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
		}

		// Token: 0x060073DF RID: 29663 RVA: 0x002386A1 File Offset: 0x002368A1
		public override void OnUpdate()
		{
			if (DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}

		// Token: 0x040073ED RID: 29677
		[RequiredField]
		[Tooltip("The game object that owns the other FSM.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040073EE RID: 29678
		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		// Token: 0x040073EF RID: 29679
		[RequiredField]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The Event to send.")]
		public FsmString sendEvent;

		// Token: 0x040073F0 RID: 29680
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Optional delay in seconds.")]
		public FsmFloat delay;

		// Token: 0x040073F1 RID: 29681
		private bool requireReceiver;

		// Token: 0x040073F2 RID: 29682
		private GameObject go;

		// Token: 0x040073F3 RID: 29683
		private DelayedEvent delayedEvent;
	}
}
