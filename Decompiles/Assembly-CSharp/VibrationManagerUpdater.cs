using System;
using UnityEngine;

// Token: 0x020007B1 RID: 1969
public sealed class VibrationManagerUpdater : MonoBehaviour
{
	// Token: 0x0600458C RID: 17804 RVA: 0x0012F078 File Offset: 0x0012D278
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x0600458D RID: 17805 RVA: 0x0012F085 File Offset: 0x0012D285
	private void Update()
	{
		if (!VibrationManager.Update())
		{
			base.enabled = false;
		}
	}
}
