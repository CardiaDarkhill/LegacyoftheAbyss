using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001128 RID: 4392
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when Called by the EventSystem when a Select event occurs. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnSelectEvent : EventTriggerActionBase
	{
		// Token: 0x06007674 RID: 30324 RVA: 0x00241FBB File Offset: 0x002401BB
		public override void Reset()
		{
			base.Reset();
			this.onSelectEvent = null;
		}

		// Token: 0x06007675 RID: 30325 RVA: 0x00241FCA File Offset: 0x002401CA
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Select, new UnityAction<BaseEventData>(this.OnSelectDelegate));
		}

		// Token: 0x06007676 RID: 30326 RVA: 0x00241FE0 File Offset: 0x002401E0
		private void OnSelectDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onSelectEvent);
		}

		// Token: 0x040076C8 RID: 30408
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnSelect is called")]
		public FsmEvent onSelectEvent;
	}
}
