using System;

// Token: 0x020001EF RID: 495
[Serializable]
public class MateriumItemsData : SerializableNamedList<MateriumItemsData.Data, MateriumItemsData.NamedData>
{
	// Token: 0x0200152C RID: 5420
	[Serializable]
	public struct Data
	{
		// Token: 0x0400862A RID: 34346
		public bool IsCollected;

		// Token: 0x0400862B RID: 34347
		public bool HasSeenInRelicBoard;
	}

	// Token: 0x0200152D RID: 5421
	[Serializable]
	public class NamedData : SerializableNamedData<MateriumItemsData.Data>
	{
	}
}
