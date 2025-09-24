using System;

// Token: 0x020001E7 RID: 487
[Serializable]
public class CollectableMementosData : SerializableNamedList<CollectableMementosData.Data, CollectableMementosData.NamedData>
{
	// Token: 0x02001526 RID: 5414
	[Serializable]
	public struct Data
	{
		// Token: 0x0400861F RID: 34335
		public bool IsDeposited;

		// Token: 0x04008620 RID: 34336
		public bool HasSeenInRelicBoard;
	}

	// Token: 0x02001527 RID: 5415
	[Serializable]
	public class NamedData : SerializableNamedData<CollectableMementosData.Data>
	{
	}
}
