using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001127 RID: 4391
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnScroll is called on the GameObject. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnScrollEvent : EventTriggerActionBase
	{
		// Token: 0x06007670 RID: 30320 RVA: 0x00241F70 File Offset: 0x00240170
		public override void Reset()
		{
			base.Reset();
			this.onScrollEvent = null;
		}

		// Token: 0x06007671 RID: 30321 RVA: 0x00241F7F File Offset: 0x0024017F
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Scroll, new UnityAction<BaseEventData>(this.OnScrollDelegate));
		}

		// Token: 0x06007672 RID: 30322 RVA: 0x00241F94 File Offset: 0x00240194
		private void OnScrollDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onScrollEvent);
		}

		// Token: 0x040076C7 RID: 30407
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnScroll is called")]
		public FsmEvent onScrollEvent;
	}
}
