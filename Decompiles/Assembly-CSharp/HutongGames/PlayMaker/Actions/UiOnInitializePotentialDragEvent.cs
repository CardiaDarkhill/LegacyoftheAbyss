using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001120 RID: 4384
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when Called by the EventSystem when a drag has been found, but before it is valid to begin the drag.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnInitializePotentialDragEvent : EventTriggerActionBase
	{
		// Token: 0x06007654 RID: 30292 RVA: 0x00241D61 File Offset: 0x0023FF61
		public override void Reset()
		{
			base.Reset();
			this.onInitializePotentialDragEvent = null;
		}

		// Token: 0x06007655 RID: 30293 RVA: 0x00241D70 File Offset: 0x0023FF70
		public override void OnEnter()
		{
			base.Init(EventTriggerType.InitializePotentialDrag, new UnityAction<BaseEventData>(this.OnInitializePotentialDragDelegate));
		}

		// Token: 0x06007656 RID: 30294 RVA: 0x00241D86 File Offset: 0x0023FF86
		private void OnInitializePotentialDragDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onInitializePotentialDragEvent);
		}

		// Token: 0x040076C0 RID: 30400
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnInitializePotentialDrag is called")]
		public FsmEvent onInitializePotentialDragEvent;
	}
}
