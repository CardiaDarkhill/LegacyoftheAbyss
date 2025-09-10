using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000612 RID: 1554
public class CaptureMouseEvents : UIBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06003774 RID: 14196 RVA: 0x000F4965 File Offset: 0x000F2B65
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.forwardTarget)
		{
			this.forwardTarget.OnPointerEnter(eventData);
		}
	}

	// Token: 0x06003775 RID: 14197 RVA: 0x000F4980 File Offset: 0x000F2B80
	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.forwardTarget)
		{
			this.forwardTarget.OnPointerExit(eventData);
		}
	}

	// Token: 0x04003A5F RID: 14943
	public Selectable forwardTarget;
}
