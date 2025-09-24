using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200112A RID: 4394
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when Called by the EventSystem when the object associated with this EventTrigger is updated.\nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnUpdateSelectedEvent : EventTriggerActionBase
	{
		// Token: 0x0600767C RID: 30332 RVA: 0x00242053 File Offset: 0x00240253
		public override void Reset()
		{
			base.Reset();
			this.onUpdateSelectedEvent = null;
		}

		// Token: 0x0600767D RID: 30333 RVA: 0x00242062 File Offset: 0x00240262
		public override void OnEnter()
		{
			base.Init(EventTriggerType.UpdateSelected, new UnityAction<BaseEventData>(this.OnUpdateSelectedDelegate));
		}

		// Token: 0x0600767E RID: 30334 RVA: 0x00242077 File Offset: 0x00240277
		private void OnUpdateSelectedDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onUpdateSelectedEvent);
		}

		// Token: 0x040076CA RID: 30410
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnUpdateSelected is called")]
		public FsmEvent onUpdateSelectedEvent;
	}
}
