using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200111E RID: 4382
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnDrop is called on the GameObject. Warning this event is sent everyframe while dragging.\n Use GetLastPointerDataInfo action to get info from the event.")]
	public class UiOnDropEvent : EventTriggerActionBase
	{
		// Token: 0x0600764C RID: 30284 RVA: 0x00241CCA File Offset: 0x0023FECA
		public override void Reset()
		{
			base.Reset();
			this.onDropEvent = null;
		}

		// Token: 0x0600764D RID: 30285 RVA: 0x00241CD9 File Offset: 0x0023FED9
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Drop, new UnityAction<BaseEventData>(this.OnDropDelegate));
		}

		// Token: 0x0600764E RID: 30286 RVA: 0x00241CEE File Offset: 0x0023FEEE
		private void OnDropDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onDropEvent);
		}

		// Token: 0x040076BE RID: 30398
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnDrop is called")]
		public FsmEvent onDropEvent;
	}
}
