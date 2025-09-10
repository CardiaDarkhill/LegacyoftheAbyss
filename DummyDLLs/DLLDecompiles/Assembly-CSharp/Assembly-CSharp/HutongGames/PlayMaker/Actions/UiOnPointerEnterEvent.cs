using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001124 RID: 4388
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnPointerEnter is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnPointerEnterEvent : EventTriggerActionBase
	{
		// Token: 0x06007664 RID: 30308 RVA: 0x00241E8F File Offset: 0x0024008F
		public override void Reset()
		{
			base.Reset();
			this.onPointerEnterEvent = null;
		}

		// Token: 0x06007665 RID: 30309 RVA: 0x00241E9E File Offset: 0x0024009E
		public override void OnEnter()
		{
			base.Init(EventTriggerType.PointerEnter, new UnityAction<BaseEventData>(this.OnPointerEnterDelegate));
		}

		// Token: 0x06007666 RID: 30310 RVA: 0x00241EB3 File Offset: 0x002400B3
		private void OnPointerEnterDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onPointerEnterEvent);
		}

		// Token: 0x040076C4 RID: 30404
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when PointerEnter is called")]
		public FsmEvent onPointerEnterEvent;
	}
}
