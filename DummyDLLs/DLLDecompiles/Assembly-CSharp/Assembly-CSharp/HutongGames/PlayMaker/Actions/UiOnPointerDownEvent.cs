using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001123 RID: 4387
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnPointerDown is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnPointerDownEvent : EventTriggerActionBase
	{
		// Token: 0x06007660 RID: 30304 RVA: 0x00241E44 File Offset: 0x00240044
		public override void Reset()
		{
			base.Reset();
			this.onPointerDownEvent = null;
		}

		// Token: 0x06007661 RID: 30305 RVA: 0x00241E53 File Offset: 0x00240053
		public override void OnEnter()
		{
			base.Init(EventTriggerType.PointerDown, new UnityAction<BaseEventData>(this.OnPointerDownDelegate));
		}

		// Token: 0x06007662 RID: 30306 RVA: 0x00241E68 File Offset: 0x00240068
		private void OnPointerDownDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onPointerDownEvent);
		}

		// Token: 0x040076C3 RID: 30403
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when PointerDown is called")]
		public FsmEvent onPointerDownEvent;
	}
}
