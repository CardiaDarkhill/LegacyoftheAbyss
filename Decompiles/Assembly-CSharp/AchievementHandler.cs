using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000411 RID: 1041
public class AchievementHandler : MonoBehaviour
{
	// Token: 0x1400006B RID: 107
	// (add) Token: 0x0600233D RID: 9021 RVA: 0x000A1268 File Offset: 0x0009F468
	// (remove) Token: 0x0600233E RID: 9022 RVA: 0x000A12A0 File Offset: 0x0009F4A0
	public event AchievementHandler.AchievementAwarded AwardAchievementEvent;

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x0600233F RID: 9023 RVA: 0x000A12D5 File Offset: 0x0009F4D5
	public List<string> QueuedAchievements
	{
		get
		{
			return this.queuedAchievements;
		}
	}

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002340 RID: 9024 RVA: 0x000A12DD File Offset: 0x0009F4DD
	public AchievementsList AchievementsList
	{
		get
		{
			return this.achievementsList;
		}
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000A12E5 File Offset: 0x0009F4E5
	private void Start()
	{
		this.gm = GameManager.instance;
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000A12F4 File Offset: 0x0009F4F4
	public void AwardAchievementToPlayer(string key)
	{
		if (DemoHelper.IsDemoMode)
		{
			return;
		}
		if (this.AchievementsList.FindAchievement(key) != null && this.CanAwardAchievement(key) && (CheatManager.AlwaysAwardAchievement || !Platform.Current.IsAchievementUnlocked(key).GetValueOrDefault()))
		{
			Platform.Current.PushAchievementUnlock(key);
			if (this.gm.gameSettings.showNativeAchievementPopups == 1 && this.AwardAchievementEvent != null)
			{
				this.AwardAchievementEvent(key);
			}
		}
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000A136E File Offset: 0x0009F56E
	public void UpdateAchievementProgress(string key, int value, int max)
	{
		Platform.Current.UpdateAchievementProgress(key, value, max);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000A1380 File Offset: 0x0009F580
	public bool AchievementWasAwarded(string key)
	{
		return this.AchievementsList.FindAchievement(key) != null && Platform.Current.IsAchievementUnlocked(key).GetValueOrDefault();
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000A13B0 File Offset: 0x0009F5B0
	public void ResetAllAchievements()
	{
		Platform.Current.ResetAchievements();
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000A13BC File Offset: 0x0009F5BC
	public void FlushRecordsToDisk()
	{
		Platform.Current.RoamingSharedData.Save();
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000A13CD File Offset: 0x0009F5CD
	public void QueueAchievement(string key)
	{
		if (!this.QueuedAchievements.Contains(key))
		{
			this.QueuedAchievements.Add(key);
		}
	}

	// Token: 0x06002348 RID: 9032 RVA: 0x000A13EC File Offset: 0x0009F5EC
	public void QueueAchievementProgress(string key, int value, int max)
	{
		AchievementHandler.QueuedAchievementProgress queuedAchievementProgress = new AchievementHandler.QueuedAchievementProgress(key, value, max);
		int num = this.queuedAchievementProgressList.IndexOf(queuedAchievementProgress);
		if (num < 0)
		{
			this.queuedAchievementProgressList.Add(queuedAchievementProgress);
			return;
		}
		this.queuedAchievementProgressList[num] = queuedAchievementProgress;
	}

	// Token: 0x06002349 RID: 9033 RVA: 0x000A1430 File Offset: 0x0009F630
	public void AwardQueuedAchievements()
	{
		foreach (string key in this.QueuedAchievements)
		{
			this.AwardAchievementToPlayer(key);
		}
		this.QueuedAchievements.Clear();
		if (this.gm)
		{
			foreach (AchievementHandler.QueuedAchievementProgress queuedAchievementProgress in this.queuedAchievementProgressList)
			{
				this.gm.UpdateAchievementProgress(queuedAchievementProgress.key, queuedAchievementProgress.value, queuedAchievementProgress.maxValue);
			}
			this.queuedAchievementProgressList.Clear();
		}
	}

	// Token: 0x0600234A RID: 9034 RVA: 0x000A1500 File Offset: 0x0009F700
	public void AwardAllAchievements()
	{
		foreach (Achievement achievement in this.AchievementsList.Achievements)
		{
			this.AwardAchievementToPlayer(achievement.PlatformKey);
		}
	}

	// Token: 0x0600234B RID: 9035 RVA: 0x000A1558 File Offset: 0x0009F758
	[ContextMenu("Award Random Achievement", true)]
	private bool CanAwardRandomAchievement()
	{
		return Application.isPlaying;
	}

	// Token: 0x0600234C RID: 9036 RVA: 0x000A1560 File Offset: 0x0009F760
	[ContextMenu("Award Random Achievement")]
	private void AwardRandomAchievement()
	{
		Achievement achievement = this.AchievementsList.Achievements[Random.Range(0, this.AchievementsList.Achievements.Count)];
		this.AwardAchievementToPlayer(achievement.PlatformKey);
	}

	// Token: 0x0600234D RID: 9037 RVA: 0x000A15A0 File Offset: 0x0009F7A0
	private bool CanAwardAchievement(string key)
	{
		if (GameManager.instance)
		{
			string currentMapZone = GameManager.instance.GetCurrentMapZone();
			if (this.achievementWhiteLists.ContainsKey(currentMapZone))
			{
				return this.achievementWhiteLists[currentMapZone].Contains(key);
			}
		}
		return true;
	}

	// Token: 0x040021E6 RID: 8678
	private GameManager gm;

	// Token: 0x040021E7 RID: 8679
	[SerializeField]
	private AchievementsList achievementsList;

	// Token: 0x040021E9 RID: 8681
	private readonly List<string> queuedAchievements = new List<string>();

	// Token: 0x040021EA RID: 8682
	private readonly List<AchievementHandler.QueuedAchievementProgress> queuedAchievementProgressList = new List<AchievementHandler.QueuedAchievementProgress>();

	// Token: 0x040021EB RID: 8683
	private readonly Dictionary<string, List<string>> achievementWhiteLists = new Dictionary<string, List<string>>
	{
		{
			"GODS_GLORY",
			new List<string>
			{
				"PANTHEON1",
				"PANTHEON2",
				"PANTHEON3",
				"PANTHEON4",
				"ENDINGD",
				"COMPLETIONGG"
			}
		}
	};

	// Token: 0x020016A5 RID: 5797
	// (Invoke) Token: 0x06008AA1 RID: 35489
	public delegate void AchievementAwarded(string key);

	// Token: 0x020016A6 RID: 5798
	private struct QueuedAchievementProgress : IEquatable<AchievementHandler.QueuedAchievementProgress>
	{
		// Token: 0x06008AA4 RID: 35492 RVA: 0x00280B16 File Offset: 0x0027ED16
		public QueuedAchievementProgress(string key, int value, int maxValue)
		{
			this.key = key;
			this.value = value;
			this.maxValue = maxValue;
		}

		// Token: 0x06008AA5 RID: 35493 RVA: 0x00280B2D File Offset: 0x0027ED2D
		public bool Equals(AchievementHandler.QueuedAchievementProgress other)
		{
			return string.Equals(this.key, other.key);
		}

		// Token: 0x06008AA6 RID: 35494 RVA: 0x00280B40 File Offset: 0x0027ED40
		public override bool Equals(object obj)
		{
			if (obj is AchievementHandler.QueuedAchievementProgress)
			{
				AchievementHandler.QueuedAchievementProgress other = (AchievementHandler.QueuedAchievementProgress)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06008AA7 RID: 35495 RVA: 0x00280B65 File Offset: 0x0027ED65
		public override int GetHashCode()
		{
			if (this.key == null)
			{
				return 0;
			}
			return this.key.GetHashCode();
		}

		// Token: 0x06008AA8 RID: 35496 RVA: 0x00280B7C File Offset: 0x0027ED7C
		public static bool operator ==(AchievementHandler.QueuedAchievementProgress left, AchievementHandler.QueuedAchievementProgress right)
		{
			return left.Equals(right);
		}

		// Token: 0x06008AA9 RID: 35497 RVA: 0x00280B86 File Offset: 0x0027ED86
		public static bool operator !=(AchievementHandler.QueuedAchievementProgress left, AchievementHandler.QueuedAchievementProgress right)
		{
			return !(left == right);
		}

		// Token: 0x04008B9C RID: 35740
		public string key;

		// Token: 0x04008B9D RID: 35741
		public int value;

		// Token: 0x04008B9E RID: 35742
		public int maxValue;
	}
}
