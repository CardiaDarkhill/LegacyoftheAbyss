using System;

namespace TeamCherry.Localization.Platform
{
	// Token: 0x020008AE RID: 2222
	[Serializable]
	public class AchievementEntry
	{
		// Token: 0x06004CDD RID: 19677 RVA: 0x00168F60 File Offset: 0x00167160
		public void MergeFrom(AchievementEntry other)
		{
			if (!string.IsNullOrEmpty(other.internalAchievementID))
			{
				this.internalAchievementID = other.internalAchievementID;
			}
			if (!string.IsNullOrEmpty(other.achievementId))
			{
				this.achievementId = other.achievementId;
			}
			if (!string.IsNullOrEmpty(other.achievementNameId))
			{
				this.achievementNameId = other.achievementNameId;
			}
			if (!string.IsNullOrEmpty(other.lockedDescriptionId))
			{
				this.lockedDescriptionId = other.lockedDescriptionId;
			}
			if (!string.IsNullOrEmpty(other.unlockedDescriptionId))
			{
				this.unlockedDescriptionId = other.unlockedDescriptionId;
			}
			if (!string.IsNullOrEmpty(other.iconImageId))
			{
				this.iconImageId = other.iconImageId;
			}
			this.displayOrder = other.displayOrder;
			this.gamerscore = other.gamerscore;
			this.isHidden = other.isHidden;
			this.baseAchievement = other.baseAchievement;
		}

		// Token: 0x04004DE3 RID: 19939
		public string internalAchievementID;

		// Token: 0x04004DE4 RID: 19940
		public string achievementId;

		// Token: 0x04004DE5 RID: 19941
		public string stableGuid;

		// Token: 0x04004DE6 RID: 19942
		public string achievementNameId;

		// Token: 0x04004DE7 RID: 19943
		public string lockedDescriptionId;

		// Token: 0x04004DE8 RID: 19944
		public string unlockedDescriptionId;

		// Token: 0x04004DE9 RID: 19945
		public bool isHidden;

		// Token: 0x04004DEA RID: 19946
		public int displayOrder;

		// Token: 0x04004DEB RID: 19947
		public int gamerscore;

		// Token: 0x04004DEC RID: 19948
		public bool baseAchievement;

		// Token: 0x04004DED RID: 19949
		public string iconImageId;
	}
}
