using System;
using UnityEngine;

// Token: 0x020004A0 RID: 1184
public interface IBreakerBreakable
{
	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x06002B08 RID: 11016
	BreakableBreaker.BreakableTypes BreakableType { get; }

	// Token: 0x06002B09 RID: 11017
	void BreakFromBreaker(BreakableBreaker breaker);

	// Token: 0x06002B0A RID: 11018
	void HitFromBreaker(BreakableBreaker breaker);

	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x06002B0B RID: 11019
	GameObject gameObject { get; }
}
