using System;
using UnityEngine;

// Token: 0x020003DB RID: 987
public interface ICurrencyLimitRegion
{
	// Token: 0x17000382 RID: 898
	// (get) Token: 0x060021CE RID: 8654
	CurrencyType CurrencyType { get; }

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x060021CF RID: 8655
	int Limit { get; }

	// Token: 0x060021D0 RID: 8656
	bool IsInsideLimitRegion(Vector2 point);
}
