using System;
using UnityEngine;

// Token: 0x020005D5 RID: 1493
public class DisableAtRuntime : MonoBehaviour
{
	// Token: 0x0600350F RID: 13583 RVA: 0x000EB7CA File Offset: 0x000E99CA
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}
}
