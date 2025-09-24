using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001126 RID: 4390
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnPointerUp is called on the GameObject. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnPointerUpEvent : EventTriggerActionBase
	{
		// Token: 0x0600766C RID: 30316 RVA: 0x00241F25 File Offset: 0x00240125
		public override void Reset()
		{
			base.Reset();
			this.onPointerUpEvent = null;
		}

		// Token: 0x0600766D RID: 30317 RVA: 0x00241F34 File Offset: 0x00240134
		public override void OnEnter()
		{
			base.Init(EventTriggerType.PointerUp, new UnityAction<BaseEventData>(this.OnPointerUpDelegate));
		}

		// Token: 0x0600766E RID: 30318 RVA: 0x00241F49 File Offset: 0x00240149
		private void OnPointerUpDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onPointerUpEvent);
		}

		// Token: 0x040076C6 RID: 30406
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when PointerUp is called")]
		public FsmEvent onPointerUpEvent;
	}
}
