using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200111F RID: 4383
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event Called by the EventSystem once dragging ends.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnEndDragEvent : EventTriggerActionBase
	{
		// Token: 0x06007650 RID: 30288 RVA: 0x00241D15 File Offset: 0x0023FF15
		public override void Reset()
		{
			base.Reset();
			this.onEndDragEvent = null;
		}

		// Token: 0x06007651 RID: 30289 RVA: 0x00241D24 File Offset: 0x0023FF24
		public override void OnEnter()
		{
			base.Init(EventTriggerType.EndDrag, new UnityAction<BaseEventData>(this.OnEndDragDelegate));
		}

		// Token: 0x06007652 RID: 30290 RVA: 0x00241D3A File Offset: 0x0023FF3A
		private void OnEndDragDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onEndDragEvent);
		}

		// Token: 0x040076BF RID: 30399
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnEndDrag is called")]
		public FsmEvent onEndDragEvent;
	}
}
