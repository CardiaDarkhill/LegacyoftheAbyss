using System;
using System.Collections.Generic;

// Token: 0x020005E1 RID: 1505
[Serializable]
public class ToolCrestsData : SerializableNamedList<ToolCrestsData.Data, ToolCrestsData.NamedData>
{
	// Token: 0x020018E7 RID: 6375
	[Serializable]
	public struct SlotData
	{
		// Token: 0x040093AD RID: 37805
		public string EquippedTool;

		// Token: 0x040093AE RID: 37806
		public bool IsUnlocked;
	}

	// Token: 0x020018E8 RID: 6376
	[Serializable]
	public struct Data
	{
		// Token: 0x040093AF RID: 37807
		public bool IsUnlocked;

		// Token: 0x040093B0 RID: 37808
		public List<ToolCrestsData.SlotData> Slots;

		// Token: 0x040093B1 RID: 37809
		public bool DisplayNewIndicator;
	}

	// Token: 0x020018E9 RID: 6377
	[Serializable]
	public class NamedData : SerializableNamedData<ToolCrestsData.Data>
	{
	}
}
