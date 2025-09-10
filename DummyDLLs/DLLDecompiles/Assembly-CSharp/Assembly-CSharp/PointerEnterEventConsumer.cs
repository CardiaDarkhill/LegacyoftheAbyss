using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000705 RID: 1797
public sealed class PointerEnterEventConsumer : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	// Token: 0x06004027 RID: 16423 RVA: 0x0011A8A7 File Offset: 0x00118AA7
	public void OnPointerEnter(PointerEventData eventData)
	{
		eventData.Use();
	}
}
