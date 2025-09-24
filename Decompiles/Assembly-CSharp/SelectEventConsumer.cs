using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x0200070B RID: 1803
public sealed class SelectEventConsumer : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	// Token: 0x06004064 RID: 16484 RVA: 0x0011B51A File Offset: 0x0011971A
	public void OnSelect(BaseEventData eventData)
	{
		eventData.Use();
	}
}
