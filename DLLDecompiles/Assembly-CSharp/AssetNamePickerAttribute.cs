using System;
using UnityEngine;

// Token: 0x02000750 RID: 1872
public class AssetNamePickerAttribute : PropertyAttribute
{
	// Token: 0x17000787 RID: 1927
	// (get) Token: 0x0600428D RID: 17037 RVA: 0x0012591A File Offset: 0x00123B1A
	public string SearchFilter
	{
		get
		{
			return this.searchFilter;
		}
	}

	// Token: 0x0600428E RID: 17038 RVA: 0x00125922 File Offset: 0x00123B22
	public AssetNamePickerAttribute(string searchFilter)
	{
		this.searchFilter = searchFilter;
	}

	// Token: 0x04004413 RID: 17427
	private readonly string searchFilter;
}
