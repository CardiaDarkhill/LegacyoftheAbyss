using System;

// Token: 0x020005A5 RID: 1445
[Serializable]
public class QuestRumourData : SerializableNamedList<QuestRumourData.Data, QuestRumourData.NamedData>
{
	// Token: 0x020018CD RID: 6349
	[Serializable]
	public class NamedData : SerializableNamedData<QuestRumourData.Data>
	{
	}

	// Token: 0x020018CE RID: 6350
	[Serializable]
	public struct Data
	{
		// Token: 0x04009365 RID: 37733
		public bool HasBeenSeen;

		// Token: 0x04009366 RID: 37734
		public bool IsAccepted;
	}
}
