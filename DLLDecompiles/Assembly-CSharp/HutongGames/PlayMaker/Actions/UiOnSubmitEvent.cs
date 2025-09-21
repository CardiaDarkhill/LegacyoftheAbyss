using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001129 RID: 4393
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnSubmit is called on the GameObject. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnSubmitEvent : EventTriggerActionBase
	{
		// Token: 0x06007678 RID: 30328 RVA: 0x00242007 File Offset: 0x00240207
		public override void Reset()
		{
			base.Reset();
			this.onSubmitEvent = null;
		}

		// Token: 0x06007679 RID: 30329 RVA: 0x00242016 File Offset: 0x00240216
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Submit, new UnityAction<BaseEventData>(this.OnSubmitDelegate));
		}

		// Token: 0x0600767A RID: 30330 RVA: 0x0024202C File Offset: 0x0024022C
		private void OnSubmitDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onSubmitEvent);
		}

		// Token: 0x040076C9 RID: 30409
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnSubmitEvent is called")]
		public FsmEvent onSubmitEvent;
	}
}
