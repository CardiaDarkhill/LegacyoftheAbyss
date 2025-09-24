using System;

// Token: 0x020005F4 RID: 1524
[Serializable]
public class ToolItemLiquidsData : SerializableNamedList<ToolItemLiquidsData.Data, ToolItemLiquidsData.NamedData>
{
	// Token: 0x020018FE RID: 6398
	[Serializable]
	public struct Data
	{
		// Token: 0x04009411 RID: 37905
		public int RefillsLeft;

		// Token: 0x04009412 RID: 37906
		public bool SeenEmptyState;

		// Token: 0x04009413 RID: 37907
		public bool UsedExtra;
	}

	// Token: 0x020018FF RID: 6399
	[Serializable]
	public class NamedData : SerializableNamedData<ToolItemLiquidsData.Data>
	{
	}
}
