using System;
using UnityEngine;

// Token: 0x02000671 RID: 1649
public class GrubPin : MonoBehaviour
{
	// Token: 0x06003B27 RID: 15143 RVA: 0x00104A88 File Offset: 0x00102C88
	private void Start()
	{
		this.pd = PlayerData.instance;
	}

	// Token: 0x06003B28 RID: 15144 RVA: 0x00104A95 File Offset: 0x00102C95
	private void OnEnable()
	{
		if (this.pd == null)
		{
			this.pd = PlayerData.instance;
		}
	}

	// Token: 0x04003D74 RID: 15732
	private PlayerData pd;
}
