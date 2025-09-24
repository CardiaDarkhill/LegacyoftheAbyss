using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200111D RID: 4381
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnDrag is called on the GameObject. Warning this event is sent every frame while dragging.\n Use GetLastPointerDataInfo action to get info from the event.")]
	public class UiOnDragEvent : EventTriggerActionBase
	{
		// Token: 0x06007648 RID: 30280 RVA: 0x00241C7F File Offset: 0x0023FE7F
		public override void Reset()
		{
			base.Reset();
			this.onDragEvent = null;
		}

		// Token: 0x06007649 RID: 30281 RVA: 0x00241C8E File Offset: 0x0023FE8E
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Drag, new UnityAction<BaseEventData>(this.OnDragDelegate));
		}

		// Token: 0x0600764A RID: 30282 RVA: 0x00241CA3 File Offset: 0x0023FEA3
		private void OnDragDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onDragEvent);
		}

		// Token: 0x040076BD RID: 30397
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnDrag is called")]
		public FsmEvent onDragEvent;
	}
}
