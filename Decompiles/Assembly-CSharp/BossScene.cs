using System;
using UnityEngine;

// Token: 0x0200037B RID: 891
[CreateAssetMenu(fileName = "New Boss Scene", menuName = "Hollow Knight/Boss Scene")]
public class BossScene : ScriptableObject
{
	// Token: 0x17000300 RID: 768
	// (get) Token: 0x06001E67 RID: 7783 RVA: 0x0008C01A File Offset: 0x0008A21A
	public Sprite DisplayIcon
	{
		get
		{
			return this.displayIcon;
		}
	}

	// Token: 0x17000301 RID: 769
	// (get) Token: 0x06001E68 RID: 7784 RVA: 0x0008C022 File Offset: 0x0008A222
	public string Tier1Scene
	{
		get
		{
			if (!this.tier1Scene)
			{
				return this.sceneName;
			}
			return this.tier1Scene.sceneName;
		}
	}

	// Token: 0x17000302 RID: 770
	// (get) Token: 0x06001E69 RID: 7785 RVA: 0x0008C043 File Offset: 0x0008A243
	public string Tier2Scene
	{
		get
		{
			if (!this.tier2Scene)
			{
				return this.sceneName;
			}
			return this.tier2Scene.sceneName;
		}
	}

	// Token: 0x17000303 RID: 771
	// (get) Token: 0x06001E6A RID: 7786 RVA: 0x0008C064 File Offset: 0x0008A264
	public string Tier3Scene
	{
		get
		{
			if (!this.tier3Scene)
			{
				return this.sceneName;
			}
			return this.tier3Scene.sceneName;
		}
	}

	// Token: 0x06001E6B RID: 7787 RVA: 0x0008C085 File Offset: 0x0008A285
	public bool IsUnlocked(BossSceneCheckSource source)
	{
		return (source == BossSceneCheckSource.Sequence && this.baseBoss && this.baseBoss.IsUnlocked(source)) || this.IsUnlockedSelf(source);
	}

	// Token: 0x06001E6C RID: 7788 RVA: 0x0008C0B0 File Offset: 0x0008A2B0
	public bool IsUnlockedSelf(BossSceneCheckSource source)
	{
		if (GameManager.instance.playerData.unlockedBossScenes.Contains(base.name))
		{
			return true;
		}
		if (this.bossTests != null && this.bossTests.Length != 0)
		{
			BossScene.BossTest[] array = this.bossTests;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsUnlocked())
				{
					return true;
				}
			}
		}
		else if (!this.requireUnlock || source != BossSceneCheckSource.Statue)
		{
			return true;
		}
		return false;
	}

	// Token: 0x04001D6A RID: 7530
	[Tooltip("The name of the scene to load.")]
	public string sceneName;

	// Token: 0x04001D6B RID: 7531
	[Tooltip("Tests that need to succeed in order for this boss the be considered \"unlocked\". (for old save files - new saves will set each boss scene unlocked by name)")]
	public BossScene.BossTest[] bossTests;

	// Token: 0x04001D6C RID: 7532
	[Header("Sequence Only")]
	public BossScene baseBoss;

	// Token: 0x04001D6D RID: 7533
	public bool substituteBoss;

	// Token: 0x04001D6E RID: 7534
	[SerializeField]
	private Sprite displayIcon;

	// Token: 0x04001D6F RID: 7535
	[Tooltip("If this is checked this scene will not count toward overall sequence unlock, but will still only be loaded if it's unlocked.")]
	public bool isHidden;

	// Token: 0x04001D70 RID: 7536
	[Header("Boss Statue Only")]
	public bool requireUnlock;

	// Token: 0x04001D71 RID: 7537
	[SerializeField]
	private BossScene tier1Scene;

	// Token: 0x04001D72 RID: 7538
	[SerializeField]
	private BossScene tier2Scene;

	// Token: 0x04001D73 RID: 7539
	[SerializeField]
	private BossScene tier3Scene;

	// Token: 0x02001628 RID: 5672
	[Serializable]
	public class BossTest
	{
		// Token: 0x0600891D RID: 35101 RVA: 0x0027BF1C File Offset: 0x0027A11C
		public bool IsUnlocked()
		{
			bool flag = true;
			if (flag)
			{
				foreach (BossScene.BossTest.BoolTest boolTest in this.boolTests)
				{
					if (GameManager.instance.GetPlayerDataBool(boolTest.playerDataBool) != boolTest.value)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				foreach (BossScene.BossTest.IntTest intTest in this.intTests)
				{
					int playerDataInt = GameManager.instance.GetPlayerDataInt(intTest.playerDataInt);
					if (playerDataInt > -9999)
					{
						bool flag2 = false;
						switch (intTest.comparison)
						{
						case BossScene.BossTest.IntTest.Comparison.Equal:
							flag2 = (playerDataInt == intTest.value);
							break;
						case BossScene.BossTest.IntTest.Comparison.NotEqual:
							flag2 = (playerDataInt != intTest.value);
							break;
						case BossScene.BossTest.IntTest.Comparison.MoreThan:
							flag2 = (playerDataInt > intTest.value);
							break;
						case BossScene.BossTest.IntTest.Comparison.LessThan:
							flag2 = (playerDataInt < intTest.value);
							break;
						}
						if (!flag2)
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag && this.sharedData != null)
			{
				BossScene.BossTest.SharedDataTest[] array3 = this.sharedData;
				for (int i = 0; i < array3.Length; i++)
				{
					if (array3[i] == BossScene.BossTest.SharedDataTest.GGUnlock && GameManager.instance.GetStatusRecordInt("RecBossRushMode") == 1)
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		// Token: 0x040089D8 RID: 35288
		public BossScene.BossTest.BoolTest[] boolTests;

		// Token: 0x040089D9 RID: 35289
		public BossScene.BossTest.IntTest[] intTests;

		// Token: 0x040089DA RID: 35290
		public BossScene.BossTest.SharedDataTest[] sharedData;

		// Token: 0x02001C13 RID: 7187
		[Serializable]
		public struct BoolTest
		{
			// Token: 0x04009FEE RID: 40942
			public string playerDataBool;

			// Token: 0x04009FEF RID: 40943
			public bool value;
		}

		// Token: 0x02001C14 RID: 7188
		[Serializable]
		public struct IntTest
		{
			// Token: 0x04009FF0 RID: 40944
			public string playerDataInt;

			// Token: 0x04009FF1 RID: 40945
			public int value;

			// Token: 0x04009FF2 RID: 40946
			public BossScene.BossTest.IntTest.Comparison comparison;

			// Token: 0x02001C3C RID: 7228
			public enum Comparison
			{
				// Token: 0x0400A059 RID: 41049
				Equal,
				// Token: 0x0400A05A RID: 41050
				NotEqual,
				// Token: 0x0400A05B RID: 41051
				MoreThan,
				// Token: 0x0400A05C RID: 41052
				LessThan
			}
		}

		// Token: 0x02001C15 RID: 7189
		public enum SharedDataTest
		{
			// Token: 0x04009FF4 RID: 40948
			GGUnlock
		}
	}
}
