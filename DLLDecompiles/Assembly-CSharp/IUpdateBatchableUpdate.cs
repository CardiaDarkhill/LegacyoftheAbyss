using System;

// Token: 0x02000798 RID: 1944
public interface IUpdateBatchableUpdate
{
	// Token: 0x170007B0 RID: 1968
	// (get) Token: 0x060044B1 RID: 17585
	bool ShouldUpdate { get; }

	// Token: 0x060044B2 RID: 17586
	void BatchedUpdate();
}
