using System;

// Token: 0x02000599 RID: 1433
[Serializable]
public class QuestCompletionData : SerializableNamedList<QuestCompletionData.Completion, QuestCompletionData.NamedCompletion>
{
	// Token: 0x170005A8 RID: 1448
	// (get) Token: 0x06003385 RID: 13189 RVA: 0x000E5830 File Offset: 0x000E3A30
	public static QuestCompletionData.Completion Accepted
	{
		get
		{
			return new QuestCompletionData.Completion
			{
				IsAccepted = true
			};
		}
	}

	// Token: 0x170005A9 RID: 1449
	// (get) Token: 0x06003386 RID: 13190 RVA: 0x000E5850 File Offset: 0x000E3A50
	public static QuestCompletionData.Completion Completed
	{
		get
		{
			return new QuestCompletionData.Completion
			{
				IsAccepted = true,
				IsCompleted = true
			};
		}
	}

	// Token: 0x020018C0 RID: 6336
	[Serializable]
	public class NamedCompletion : SerializableNamedData<QuestCompletionData.Completion>
	{
	}

	// Token: 0x020018C1 RID: 6337
	[Serializable]
	public struct Completion
	{
		// Token: 0x06009236 RID: 37430 RVA: 0x0029B79D File Offset: 0x0029999D
		public void SetCompleted()
		{
			this.IsCompleted = true;
			this.WasEverCompleted = true;
		}

		// Token: 0x0400933A RID: 37690
		public bool HasBeenSeen;

		// Token: 0x0400933B RID: 37691
		public bool IsAccepted;

		// Token: 0x0400933C RID: 37692
		public int CompletedCount;

		// Token: 0x0400933D RID: 37693
		public bool IsCompleted;

		// Token: 0x0400933E RID: 37694
		public bool WasEverCompleted;
	}
}
