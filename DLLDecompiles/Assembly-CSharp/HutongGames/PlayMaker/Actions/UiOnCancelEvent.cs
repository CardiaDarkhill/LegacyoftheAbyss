using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200111B RID: 4379
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnCancel is called on the GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnCancelEvent : EventTriggerActionBase
	{
		// Token: 0x06007640 RID: 30272 RVA: 0x00241BE6 File Offset: 0x0023FDE6
		public override void Reset()
		{
			this.gameObject = null;
			this.onCancelEvent = null;
		}

		// Token: 0x06007641 RID: 30273 RVA: 0x00241BF6 File Offset: 0x0023FDF6
		public override void OnEnter()
		{
			base.Init(EventTriggerType.Cancel, new UnityAction<BaseEventData>(this.OnCancelDelegate));
		}

		// Token: 0x06007642 RID: 30274 RVA: 0x00241C0C File Offset: 0x0023FE0C
		private void OnCancelDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onCancelEvent);
		}

		// Token: 0x040076BB RID: 30395
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnCancelEvent is called")]
		public FsmEvent onCancelEvent;
	}
}
