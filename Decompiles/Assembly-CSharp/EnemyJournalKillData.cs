using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020002F1 RID: 753
[JsonObject(MemberSerialization.OptIn)]
[Serializable]
public class EnemyJournalKillData : ISerializationCallbackReceiver
{
	// Token: 0x06001AE9 RID: 6889 RVA: 0x0007D88A File Offset: 0x0007BA8A
	public EnemyJournalKillData()
	{
		this.list = new List<EnemyJournalKillData.NamedKillData>();
		this.dictionary = new Dictionary<string, EnemyJournalKillData.KillData>();
	}

	// Token: 0x06001AEA RID: 6890 RVA: 0x0007D8A8 File Offset: 0x0007BAA8
	public EnemyJournalKillData(List<EnemyJournalKillData.NamedKillData> startingList)
	{
		this.list = startingList;
		this.OnAfterDeserialize();
	}

	// Token: 0x170002C1 RID: 705
	// (get) Token: 0x06001AEB RID: 6891 RVA: 0x0007D8BD File Offset: 0x0007BABD
	public Dictionary<string, EnemyJournalKillData.KillData> Dictionary
	{
		get
		{
			return this.dictionary;
		}
	}

	// Token: 0x06001AEC RID: 6892 RVA: 0x0007D8C5 File Offset: 0x0007BAC5
	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		this.OnAfterDeserialize();
	}

	// Token: 0x06001AED RID: 6893 RVA: 0x0007D8D0 File Offset: 0x0007BAD0
	public void OnAfterDeserialize()
	{
		this.dictionary = (from namedRecord in this.list
		group namedRecord by namedRecord.Name).ToDictionary((IGrouping<string, EnemyJournalKillData.NamedKillData> group) => group.Key, (IGrouping<string, EnemyJournalKillData.NamedKillData> group) => group.FirstOrDefault<EnemyJournalKillData.NamedKillData>().Record);
	}

	// Token: 0x06001AEE RID: 6894 RVA: 0x0007D950 File Offset: 0x0007BB50
	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		this.OnBeforeSerialize();
	}

	// Token: 0x06001AEF RID: 6895 RVA: 0x0007D958 File Offset: 0x0007BB58
	public void OnBeforeSerialize()
	{
		this.list = (from pair in this.dictionary
		select new EnemyJournalKillData.NamedKillData
		{
			Name = pair.Key,
			Record = pair.Value
		}).ToList<EnemyJournalKillData.NamedKillData>();
	}

	// Token: 0x06001AF0 RID: 6896 RVA: 0x0007D98F File Offset: 0x0007BB8F
	public void RecordKillData(string journalRecordName, EnemyJournalKillData.KillData killData)
	{
		this.dictionary[journalRecordName] = killData;
	}

	// Token: 0x06001AF1 RID: 6897 RVA: 0x0007D9A0 File Offset: 0x0007BBA0
	public EnemyJournalKillData.KillData GetKillData(string journalRecordName)
	{
		EnemyJournalKillData.KillData result;
		if (!this.dictionary.TryGetValue(journalRecordName, out result))
		{
			return default(EnemyJournalKillData.KillData);
		}
		return result;
	}

	// Token: 0x04001A02 RID: 6658
	[SerializeField]
	[JsonProperty]
	private List<EnemyJournalKillData.NamedKillData> list;

	// Token: 0x04001A03 RID: 6659
	[NonSerialized]
	private Dictionary<string, EnemyJournalKillData.KillData> dictionary;

	// Token: 0x020015DC RID: 5596
	[Serializable]
	public struct KillData
	{
		// Token: 0x040088E1 RID: 35041
		public int Kills;

		// Token: 0x040088E2 RID: 35042
		public bool HasBeenSeen;
	}

	// Token: 0x020015DD RID: 5597
	[Serializable]
	public struct NamedKillData
	{
		// Token: 0x040088E3 RID: 35043
		public string Name;

		// Token: 0x040088E4 RID: 35044
		public EnemyJournalKillData.KillData Record;
	}
}
