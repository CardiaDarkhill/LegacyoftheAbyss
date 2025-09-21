using System;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public class SendPlaymakerEventOnEnable : MonoBehaviour
{
	// Token: 0x0600348B RID: 13451 RVA: 0x000E972B File Offset: 0x000E792B
	private void OnEnable()
	{
		if (this.eventName != "")
		{
			PlayMakerFSM.BroadcastEvent(this.eventName);
		}
	}

	// Token: 0x040037FB RID: 14331
	public string eventName = "";
}
