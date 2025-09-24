using System;

// Token: 0x0200058F RID: 1423
public interface IQuestWithCompletion
{
	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x060032E4 RID: 13028
	bool CanComplete { get; }

	// Token: 0x17000573 RID: 1395
	// (get) Token: 0x060032E5 RID: 13029
	bool IsCompleted { get; }
}
