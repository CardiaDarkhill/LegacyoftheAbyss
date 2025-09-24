using System;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public interface ICollectionViewerItem
{
	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x06001243 RID: 4675
	string name { get; }

	// Token: 0x170001F6 RID: 502
	// (get) Token: 0x06001244 RID: 4676
	// (set) Token: 0x06001245 RID: 4677
	bool IsSeen { get; set; }

	// Token: 0x170001F7 RID: 503
	// (get) Token: 0x06001246 RID: 4678 RVA: 0x000552F3 File Offset: 0x000534F3
	bool IsSeenOverridden
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001F8 RID: 504
	// (get) Token: 0x06001247 RID: 4679 RVA: 0x000552F6 File Offset: 0x000534F6
	// (set) Token: 0x06001248 RID: 4680 RVA: 0x000552F9 File Offset: 0x000534F9
	bool IsSeenOverrideValue
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	// Token: 0x170001F9 RID: 505
	// (get) Token: 0x06001249 RID: 4681 RVA: 0x000552FB File Offset: 0x000534FB
	bool CanDeposit
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600124A RID: 4682
	string GetCollectionName();

	// Token: 0x0600124B RID: 4683
	string GetCollectionDesc();

	// Token: 0x0600124C RID: 4684
	Sprite GetCollectionIcon();

	// Token: 0x0600124D RID: 4685 RVA: 0x000552FE File Offset: 0x000534FE
	bool IsListedInCollection()
	{
		return this.IsVisibleInCollection();
	}

	// Token: 0x0600124E RID: 4686
	bool IsVisibleInCollection();

	// Token: 0x0600124F RID: 4687
	bool IsRequiredInCollection();
}
