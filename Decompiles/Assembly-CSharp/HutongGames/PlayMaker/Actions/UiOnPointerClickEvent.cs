using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001122 RID: 4386
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnPointerClick is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnPointerClickEvent : EventTriggerActionBase
	{
		// Token: 0x0600765C RID: 30300 RVA: 0x00241DF9 File Offset: 0x0023FFF9
		public override void Reset()
		{
			base.Reset();
			this.onPointerClickEvent = null;
		}

		// Token: 0x0600765D RID: 30301 RVA: 0x00241E08 File Offset: 0x00240008
		public override void OnEnter()
		{
			base.Init(EventTriggerType.PointerClick, new UnityAction<BaseEventData>(this.OnPointerClickDelegate));
		}

		// Token: 0x0600765E RID: 30302 RVA: 0x00241E1D File Offset: 0x0024001D
		private void OnPointerClickDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onPointerClickEvent);
		}

		// Token: 0x040076C2 RID: 30402
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when PointerClick is called")]
		public FsmEvent onPointerClickEvent;
	}
}
