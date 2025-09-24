using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AEB RID: 2795
	[AddComponentMenu("PlayMaker/UI/UI Drag Events")]
	public class PlayMakerUiDragEvents : PlayMakerUiEventBase, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler
	{
		// Token: 0x060058CE RID: 22734 RVA: 0x001C32A1 File Offset: 0x001C14A1
		public void OnBeginDrag(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiBeginDrag);
		}

		// Token: 0x060058CF RID: 22735 RVA: 0x001C32B4 File Offset: 0x001C14B4
		public void OnDrag(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiDrag);
		}

		// Token: 0x060058D0 RID: 22736 RVA: 0x001C32C7 File Offset: 0x001C14C7
		public void OnEndDrag(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiEndDrag);
		}
	}
}
