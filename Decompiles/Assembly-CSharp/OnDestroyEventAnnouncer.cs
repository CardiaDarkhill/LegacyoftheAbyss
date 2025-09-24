using System;
using UnityEngine;

// Token: 0x0200035A RID: 858
public class OnDestroyEventAnnouncer : MonoBehaviour
{
	// Token: 0x14000055 RID: 85
	// (add) Token: 0x06001DA2 RID: 7586 RVA: 0x00088B50 File Offset: 0x00086D50
	// (remove) Token: 0x06001DA3 RID: 7587 RVA: 0x00088B88 File Offset: 0x00086D88
	public event Action<OnDestroyEventAnnouncer> OnDestroyEvent;

	// Token: 0x06001DA4 RID: 7588 RVA: 0x00088BBD File Offset: 0x00086DBD
	private void OnDestroy()
	{
		Action<OnDestroyEventAnnouncer> onDestroyEvent = this.OnDestroyEvent;
		if (onDestroyEvent != null)
		{
			onDestroyEvent(this);
		}
		this.OnDestroyEvent = null;
	}
}
