using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200111C RID: 4380
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnDeselect is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnDeselectEvent : EventTriggerActionBase
	{
		// Token: 0x06007644 RID: 30276 RVA: 0x00241C33 File Offset: 0x0023FE33
		public override void Reset()
		{
			base.Reset();
			this.onDeselectEvent = null;
		}

		// Token: 0x06007645 RID: 30277 RVA: 0x00241C42 File Offset: 0x0023FE42
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Deselect, new UnityAction<BaseEventData>(this.OnDeselectDelegate));
		}

		// Token: 0x06007646 RID: 30278 RVA: 0x00241C58 File Offset: 0x0023FE58
		private void OnDeselectDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onDeselectEvent);
		}

		// Token: 0x040076BC RID: 30396
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnDeselectEvent is called")]
		public FsmEvent onDeselectEvent;
	}
}
