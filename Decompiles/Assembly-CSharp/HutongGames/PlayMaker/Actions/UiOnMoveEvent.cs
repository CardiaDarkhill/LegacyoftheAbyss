using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001121 RID: 4385
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnMoveEvent is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnMoveEvent : EventTriggerActionBase
	{
		// Token: 0x06007658 RID: 30296 RVA: 0x00241DAD File Offset: 0x0023FFAD
		public override void Reset()
		{
			base.Reset();
			this.onMoveEvent = null;
		}

		// Token: 0x06007659 RID: 30297 RVA: 0x00241DBC File Offset: 0x0023FFBC
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Move, new UnityAction<BaseEventData>(this.OnMoveDelegate));
		}

		// Token: 0x0600765A RID: 30298 RVA: 0x00241DD2 File Offset: 0x0023FFD2
		private void OnMoveDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onMoveEvent);
		}

		// Token: 0x040076C1 RID: 30401
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnMoveEvent is called")]
		public FsmEvent onMoveEvent;
	}
}
