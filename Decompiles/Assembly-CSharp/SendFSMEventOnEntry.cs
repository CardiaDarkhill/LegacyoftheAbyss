using System;
using UnityEngine;

// Token: 0x02000316 RID: 790
public class SendFSMEventOnEntry : MonoBehaviour
{
	// Token: 0x06001BEB RID: 7147 RVA: 0x00082148 File Offset: 0x00080348
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.fsm.SendEvent(this.fsmEvent);
	}

	// Token: 0x04001AE9 RID: 6889
	public PlayMakerFSM fsm;

	// Token: 0x04001AEA RID: 6890
	public string fsmEvent;
}
