using System;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class EnemyWakeRegion : MonoBehaviour
{
	// Token: 0x06001A30 RID: 6704 RVA: 0x00078A4B File Offset: 0x00076C4B
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.fsm.SendEvent(this.enterEvent);
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x00078A5E File Offset: 0x00076C5E
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.fsm.SendEvent(this.exitEvent);
	}

	// Token: 0x04001923 RID: 6435
	public PlayMakerFSM fsm;

	// Token: 0x04001924 RID: 6436
	public string enterEvent = "WAKE";

	// Token: 0x04001925 RID: 6437
	public string exitEvent = "SLEEP";
}
