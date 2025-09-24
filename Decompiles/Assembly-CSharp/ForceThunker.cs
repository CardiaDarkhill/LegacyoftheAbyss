using System;
using UnityEngine;

// Token: 0x020004E5 RID: 1253
public class ForceThunker : MonoBehaviour
{
	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x06002CFA RID: 11514 RVA: 0x000C48EA File Offset: 0x000C2AEA
	public bool PreventDownBounce
	{
		get
		{
			return this.preventDownBounce;
		}
	}

	// Token: 0x04002EA5 RID: 11941
	[SerializeField]
	private bool preventDownBounce;
}
