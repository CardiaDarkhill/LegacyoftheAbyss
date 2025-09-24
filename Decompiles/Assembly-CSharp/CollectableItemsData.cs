using System;

// Token: 0x020001B6 RID: 438
[Serializable]
public class CollectableItemsData : SerializableNamedList<CollectableItemsData.Data, CollectableItemsData.NamedData>
{
	// Token: 0x020014F9 RID: 5369
	[Serializable]
	public struct Data
	{
		// Token: 0x04008564 RID: 34148
		public int Amount;

		// Token: 0x04008565 RID: 34149
		public int IsSeenMask;

		// Token: 0x04008566 RID: 34150
		public int AmountWhileHidden;
	}

	// Token: 0x020014FA RID: 5370
	[Serializable]
	public class NamedData : SerializableNamedData<CollectableItemsData.Data>
	{
	}
}
