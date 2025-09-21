using System;

// Token: 0x020001ED RID: 493
[Serializable]
public class CollectableRelicsData : SerializableNamedList<CollectableRelicsData.Data, CollectableRelicsData.NamedData>
{
	// Token: 0x0200152A RID: 5418
	[Serializable]
	public struct Data
	{
		// Token: 0x04008627 RID: 34343
		public bool IsCollected;

		// Token: 0x04008628 RID: 34344
		public bool IsDeposited;

		// Token: 0x04008629 RID: 34345
		public bool HasSeenInRelicBoard;
	}

	// Token: 0x0200152B RID: 5419
	[Serializable]
	public class NamedData : SerializableNamedData<CollectableRelicsData.Data>
	{
	}
}
