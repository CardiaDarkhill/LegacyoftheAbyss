using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001125 RID: 4389
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnPointerExit is called on the GameObject. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnPointerExitEvent : EventTriggerActionBase
	{
		// Token: 0x06007668 RID: 30312 RVA: 0x00241EDA File Offset: 0x002400DA
		public override void Reset()
		{
			base.Reset();
			this.onPointerExitEvent = null;
		}

		// Token: 0x06007669 RID: 30313 RVA: 0x00241EE9 File Offset: 0x002400E9
		public override void OnEnter()
		{
			base.Init(EventTriggerType.PointerExit, new UnityAction<BaseEventData>(this.OnPointerExitDelegate));
		}

		// Token: 0x0600766A RID: 30314 RVA: 0x00241EFE File Offset: 0x002400FE
		private void OnPointerExitDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onPointerExitEvent);
		}

		// Token: 0x040076C5 RID: 30405
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when PointerExit is called")]
		public FsmEvent onPointerExitEvent;
	}
}
