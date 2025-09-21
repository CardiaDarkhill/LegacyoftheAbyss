using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020007D7 RID: 2007
[JsonObject(MemberSerialization.OptIn)]
[Serializable]
public class WrappedList<T>
{
	// Token: 0x170007FC RID: 2044
	// (get) Token: 0x0600469A RID: 18074 RVA: 0x0013AF78 File Offset: 0x00139178
	public List<T> List
	{
		get
		{
			return this.list;
		}
	}

	// Token: 0x04004703 RID: 18179
	[SerializeField]
	[JsonProperty]
	private List<T> list = new List<T>();
}
