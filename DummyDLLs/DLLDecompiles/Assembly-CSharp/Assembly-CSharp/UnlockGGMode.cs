using System;
using UnityEngine;

// Token: 0x020003B3 RID: 947
public class UnlockGGMode : MonoBehaviour
{
	// Token: 0x06001FC0 RID: 8128 RVA: 0x00091215 File Offset: 0x0008F415
	private void Start()
	{
		GameManager.instance.SetStatusRecordInt("RecBossRushMode", 1);
		GameManager.instance.SaveStatusRecords();
	}
}
