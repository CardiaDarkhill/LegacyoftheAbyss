using System;
using UnityEngine;

// Token: 0x02000526 RID: 1318
public class NonThunker : MonoBehaviour
{
	// Token: 0x06002F64 RID: 12132 RVA: 0x000D0D6F File Offset: 0x000CEF6F
	public void SetActive(bool value)
	{
		this.active = value;
	}

	// Token: 0x04003221 RID: 12833
	public bool active = true;

	// Token: 0x04003222 RID: 12834
	public bool doRecoil;
}
