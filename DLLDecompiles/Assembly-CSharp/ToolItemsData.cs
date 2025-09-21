using System;
using Newtonsoft.Json;

// Token: 0x020005F0 RID: 1520
[Serializable]
public class ToolItemsData : SerializableNamedList<ToolItemsData.Data, ToolItemsData.NamedData>
{
	// Token: 0x020018FB RID: 6395
	[JsonObject(MemberSerialization.Fields)]
	[Serializable]
	public struct Data : ISerializableNamedDataRedundancy
	{
		// Token: 0x17001054 RID: 4180
		// (get) Token: 0x060092D0 RID: 37584 RVA: 0x0029C999 File Offset: 0x0029AB99
		public bool IsDataRedundant
		{
			get
			{
				return !this.IsUnlocked && this.AmountLeft <= 0;
			}
		}

		// Token: 0x04009405 RID: 37893
		public bool IsUnlocked;

		// Token: 0x04009406 RID: 37894
		public bool IsHidden;

		// Token: 0x04009407 RID: 37895
		public bool HasBeenSeen;

		// Token: 0x04009408 RID: 37896
		public bool HasBeenSelected;

		// Token: 0x04009409 RID: 37897
		public int AmountLeft;
	}

	// Token: 0x020018FC RID: 6396
	[Serializable]
	public class NamedData : SerializableNamedData<ToolItemsData.Data>
	{
	}
}
