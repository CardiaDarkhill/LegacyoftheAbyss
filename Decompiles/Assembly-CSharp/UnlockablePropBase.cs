using System;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public abstract class UnlockablePropBase : MonoBehaviour
{
	// Token: 0x06002D04 RID: 11524 RVA: 0x000C4A3F File Offset: 0x000C2C3F
	[ContextMenu("Test Unlock")]
	private void TestOpen()
	{
		this.Open();
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x000C4A47 File Offset: 0x000C2C47
	[ContextMenu("Test Unlock", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x06002D06 RID: 11526
	public abstract void Open();

	// Token: 0x06002D07 RID: 11527
	public abstract void Opened();
}
