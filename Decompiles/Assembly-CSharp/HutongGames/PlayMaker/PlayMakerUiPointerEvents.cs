using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF1 RID: 2801
	[AddComponentMenu("PlayMaker/UI/UI Pointer Events")]
	public class PlayMakerUiPointerEvents : PlayMakerUiEventBase, IPointerClickHandler, IEventSystemHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
	{
		// Token: 0x060058E7 RID: 22759 RVA: 0x001C3664 File Offset: 0x001C1864
		public void OnPointerClick(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiPointerClick);
		}

		// Token: 0x060058E8 RID: 22760 RVA: 0x001C3677 File Offset: 0x001C1877
		public void OnPointerDown(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiPointerDown);
		}

		// Token: 0x060058E9 RID: 22761 RVA: 0x001C368A File Offset: 0x001C188A
		public void OnPointerEnter(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiPointerEnter);
		}

		// Token: 0x060058EA RID: 22762 RVA: 0x001C369D File Offset: 0x001C189D
		public void OnPointerExit(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiPointerExit);
		}

		// Token: 0x060058EB RID: 22763 RVA: 0x001C36B0 File Offset: 0x001C18B0
		public void OnPointerUp(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiPointerUp);
		}
	}
}
