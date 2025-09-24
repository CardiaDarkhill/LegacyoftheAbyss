using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AEC RID: 2796
	[AddComponentMenu("PlayMaker/UI/UI Drop Event")]
	public class PlayMakerUiDropEvent : PlayMakerUiEventBase, IDropHandler, IEventSystemHandler
	{
		// Token: 0x060058D2 RID: 22738 RVA: 0x001C32E2 File Offset: 0x001C14E2
		public void OnDrop(PointerEventData eventData)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = eventData;
			base.SendEvent(FsmEvent.UiDrop);
		}
	}
}
