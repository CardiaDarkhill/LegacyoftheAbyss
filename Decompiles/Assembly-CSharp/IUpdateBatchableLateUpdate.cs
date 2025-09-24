using System;

// Token: 0x02000797 RID: 1943
public interface IUpdateBatchableLateUpdate
{
	// Token: 0x170007AF RID: 1967
	// (get) Token: 0x060044AF RID: 17583
	bool ShouldUpdate { get; }

	// Token: 0x060044B0 RID: 17584
	void BatchedLateUpdate();
}
