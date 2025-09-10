using System;
using UnityEngine;

// Token: 0x0200035D RID: 861
public class PermadeathUnlock : MonoBehaviour
{
	// Token: 0x06001DCF RID: 7631 RVA: 0x00089CCB File Offset: 0x00087ECB
	private void Start()
	{
		PermadeathUnlock.Unlock();
	}

	// Token: 0x06001DD0 RID: 7632 RVA: 0x00089CD2 File Offset: 0x00087ED2
	public static void Unlock()
	{
		GameManager.instance.SetStatusRecordInt("RecPermadeathMode", 1);
		GameManager.instance.SaveStatusRecords();
	}
}
