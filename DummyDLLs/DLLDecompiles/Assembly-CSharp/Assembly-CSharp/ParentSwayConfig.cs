using System;
using UnityEngine;

// Token: 0x02000528 RID: 1320
public class ParentSwayConfig : MonoBehaviour
{
	// Token: 0x17000542 RID: 1346
	// (get) Token: 0x06002F76 RID: 12150 RVA: 0x000D10D9 File Offset: 0x000CF2D9
	public bool ApplyMapZoneSway
	{
		get
		{
			return this.applyMapZoneSway;
		}
	}

	// Token: 0x17000543 RID: 1347
	// (get) Token: 0x06002F77 RID: 12151 RVA: 0x000D10E1 File Offset: 0x000CF2E1
	public bool HasIdleSway
	{
		get
		{
			return true;
		}
	}

	// Token: 0x04003238 RID: 12856
	[SerializeField]
	private bool applyMapZoneSway = true;
}
