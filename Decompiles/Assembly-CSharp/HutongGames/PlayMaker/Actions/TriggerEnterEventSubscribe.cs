using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200133F RID: 4927
	[ActionCategory("Hollow Knight")]
	public class TriggerEnterEventSubscribe : FsmStateAction
	{
		// Token: 0x06007F5E RID: 32606 RVA: 0x0025B208 File Offset: 0x00259408
		public override void Reset()
		{
			this.trigger = null;
			this.triggerEnteredEvent = null;
			this.triggerExitedEvent = null;
			this.triggerStayedEvent = null;
		}

		// Token: 0x06007F5F RID: 32607 RVA: 0x0025B228 File Offset: 0x00259428
		public override void OnEnter()
		{
			if (!this.trigger.IsNone)
			{
				TriggerEnterEvent triggerEnterEvent = (TriggerEnterEvent)this.trigger.Value;
				triggerEnterEvent.OnTriggerEntered += this.SendEnteredEvent;
				triggerEnterEvent.OnTriggerExited += this.SendExitedEvent;
				if (this.triggerStayedEvent != null && !string.IsNullOrEmpty(this.triggerStayedEvent.Name))
				{
					this.subbedStayEvent = true;
					triggerEnterEvent.OnTriggerStayed += this.SendStayedEvent;
				}
			}
			base.Finish();
		}

		// Token: 0x06007F60 RID: 32608 RVA: 0x0025B2B0 File Offset: 0x002594B0
		public override void OnExit()
		{
			if (this.trigger.IsNone)
			{
				return;
			}
			TriggerEnterEvent triggerEnterEvent = (TriggerEnterEvent)this.trigger.Value;
			triggerEnterEvent.OnTriggerEntered -= this.SendEnteredEvent;
			triggerEnterEvent.OnTriggerExited -= this.SendExitedEvent;
			if (this.subbedStayEvent)
			{
				this.subbedStayEvent = false;
				triggerEnterEvent.OnTriggerStayed -= this.SendStayedEvent;
			}
		}

		// Token: 0x06007F61 RID: 32609 RVA: 0x0025B321 File Offset: 0x00259521
		private void SendEnteredEvent(Collider2D collider, GameObject sender)
		{
			base.Fsm.Event(this.triggerEnteredEvent);
		}

		// Token: 0x06007F62 RID: 32610 RVA: 0x0025B334 File Offset: 0x00259534
		private void SendExitedEvent(Collider2D collider, GameObject sender)
		{
			base.Fsm.Event(this.triggerExitedEvent);
		}

		// Token: 0x06007F63 RID: 32611 RVA: 0x0025B347 File Offset: 0x00259547
		private void SendStayedEvent(Collider2D collider, GameObject sender)
		{
			base.Fsm.Event(this.triggerStayedEvent);
		}

		// Token: 0x04007EE2 RID: 32482
		[ObjectType(typeof(TriggerEnterEvent))]
		public FsmObject trigger;

		// Token: 0x04007EE3 RID: 32483
		public FsmEvent triggerEnteredEvent;

		// Token: 0x04007EE4 RID: 32484
		public FsmEvent triggerExitedEvent;

		// Token: 0x04007EE5 RID: 32485
		public FsmEvent triggerStayedEvent;

		// Token: 0x04007EE6 RID: 32486
		private bool subbedStayEvent;
	}
}
