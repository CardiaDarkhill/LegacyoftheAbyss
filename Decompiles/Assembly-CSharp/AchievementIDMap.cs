using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200044B RID: 1099
[CreateAssetMenu(fileName = "AchievementIDMap", menuName = "Hollow Knight/Achievement ID Map", order = 1900)]
public class AchievementIDMap : ScriptableObject
{
	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x060026A3 RID: 9891 RVA: 0x000AED36 File Offset: 0x000ACF36
	public int Count
	{
		get
		{
			if (this.pairs != null)
			{
				return this.pairs.Length;
			}
			return 0;
		}
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000AED4C File Offset: 0x000ACF4C
	public int? GetServiceIdForInternalId(string internalId)
	{
		if (this.serviceIdsByInternalId == null)
		{
			this.serviceIdsByInternalId = new Dictionary<string, int>();
			for (int i = 0; i < this.pairs.Length; i++)
			{
				AchievementIDMap.AchievementIDPair achievementIDPair = this.pairs[i];
				this.serviceIdsByInternalId.Add(achievementIDPair.InternalId, achievementIDPair.ServiceId);
			}
		}
		int value;
		if (!this.serviceIdsByInternalId.TryGetValue(internalId, out value))
		{
			return null;
		}
		return new int?(value);
	}

	// Token: 0x060026A5 RID: 9893 RVA: 0x000AEDC0 File Offset: 0x000ACFC0
	public bool TryGetAchievementInformation(string internalId, out AchievementIDMap.AchievementIDPair info)
	{
		if (this.infoById == null)
		{
			this.infoById = new Dictionary<string, AchievementIDMap.AchievementIDPair>();
			for (int i = 0; i < this.pairs.Length; i++)
			{
				AchievementIDMap.AchievementIDPair achievementIDPair = this.pairs[i];
				this.infoById.Add(achievementIDPair.InternalId, achievementIDPair);
			}
		}
		return this.infoById.TryGetValue(internalId, out info);
	}

	// Token: 0x060026A6 RID: 9894 RVA: 0x000AEE1C File Offset: 0x000AD01C
	private string GetElementName(int index)
	{
		try
		{
			AchievementIDMap.AchievementIDPair achievementIDPair = this.pairs[index];
			return string.Format("{0} : {1}{2}", achievementIDPair.ServiceId, achievementIDPair.InternalId, achievementIDPair.UseCustomEvent ? (" - " + achievementIDPair.CustomEvent.statName) : string.Empty);
		}
		catch (Exception)
		{
		}
		return string.Empty;
	}

	// Token: 0x04002401 RID: 9217
	[NamedArray("GetElementName")]
	[SerializeField]
	private AchievementIDMap.AchievementIDPair[] pairs;

	// Token: 0x04002402 RID: 9218
	private Dictionary<string, int> serviceIdsByInternalId;

	// Token: 0x04002403 RID: 9219
	private Dictionary<string, AchievementIDMap.AchievementIDPair> infoById;

	// Token: 0x02001732 RID: 5938
	[Serializable]
	public class AchievementIDPair
	{
		// Token: 0x17000F09 RID: 3849
		// (get) Token: 0x06008D0A RID: 36106 RVA: 0x00288EA3 File Offset: 0x002870A3
		public string InternalId
		{
			get
			{
				return this.internalId;
			}
		}

		// Token: 0x17000F0A RID: 3850
		// (get) Token: 0x06008D0B RID: 36107 RVA: 0x00288EAB File Offset: 0x002870AB
		public int ServiceId
		{
			get
			{
				return this.serviceId;
			}
		}

		// Token: 0x17000F0B RID: 3851
		// (get) Token: 0x06008D0C RID: 36108 RVA: 0x00288EB3 File Offset: 0x002870B3
		public bool UseCustomEvent
		{
			get
			{
				return this.useCustomEvent;
			}
		}

		// Token: 0x17000F0C RID: 3852
		// (get) Token: 0x06008D0D RID: 36109 RVA: 0x00288EBB File Offset: 0x002870BB
		public AchievementIDMap.CustomEvent CustomEvent
		{
			get
			{
				return this.customEvent;
			}
		}

		// Token: 0x04008D9C RID: 36252
		[SerializeField]
		[FormerlySerializedAs("achievementId")]
		private string internalId;

		// Token: 0x04008D9D RID: 36253
		[SerializeField]
		[FormerlySerializedAs("trophyId")]
		private int serviceId;

		// Token: 0x04008D9E RID: 36254
		[SerializeField]
		private bool useCustomEvent;

		// Token: 0x04008D9F RID: 36255
		[SerializeField]
		private AchievementIDMap.CustomEvent customEvent;
	}

	// Token: 0x02001733 RID: 5939
	[Serializable]
	public struct CustomEvent
	{
		// Token: 0x04008DA0 RID: 36256
		public string statName;
	}
}
