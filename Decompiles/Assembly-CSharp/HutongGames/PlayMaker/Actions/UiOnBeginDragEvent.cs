using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200111A RID: 4378
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when user starts to drag a GameObject.\n Use GetLastPointerDataInfo action to get info from the event")]
	public class UiOnBeginDragEvent : EventTriggerActionBase
	{
		// Token: 0x0600763C RID: 30268 RVA: 0x00241B9A File Offset: 0x0023FD9A
		public override void Reset()
		{
			base.Reset();
			this.onBeginDragEvent = null;
		}

		// Token: 0x0600763D RID: 30269 RVA: 0x00241BA9 File Offset: 0x0023FDA9
		public override void OnEnter()
		{
			base.Init(EventTriggerType.BeginDrag, new UnityAction<BaseEventData>(this.OnBeginDragDelegate));
		}

		// Token: 0x0600763E RID: 30270 RVA: 0x00241BBF File Offset: 0x0023FDBF
		private void OnBeginDragDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			base.SendEvent(this.eventTarget, this.onBeginDragEvent);
		}

		// Token: 0x040076BA RID: 30394
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnBeginDrag is called")]
		public FsmEvent onBeginDragEvent;
	}
}
