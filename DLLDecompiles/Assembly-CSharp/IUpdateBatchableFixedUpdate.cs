using System;

// Token: 0x02000799 RID: 1945
public interface IUpdateBatchableFixedUpdate
{
	// Token: 0x170007B1 RID: 1969
	// (get) Token: 0x060044B3 RID: 17587
	bool ShouldUpdate { get; }

	// Token: 0x060044B4 RID: 17588
	void BatchedFixedUpdate();
}
