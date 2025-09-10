using System;
using UnityEngine;

// Token: 0x0200029F RID: 671
public class BlackThreadDeathAltProxy : MonoBehaviour
{
	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001771 RID: 6001 RVA: 0x00069B96 File Offset: 0x00067D96
	public GameObject AltPrefab
	{
		get
		{
			return this.altPrefab;
		}
	}

	// Token: 0x04001615 RID: 5653
	[SerializeField]
	private GameObject altPrefab;
}
