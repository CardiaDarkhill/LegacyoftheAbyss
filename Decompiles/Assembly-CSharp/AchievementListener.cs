using System;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x0200033E RID: 830
public class AchievementListener : MonoBehaviour
{
	// Token: 0x06001CF1 RID: 7409 RVA: 0x00086772 File Offset: 0x00084972
	private void OnEnable()
	{
		if (!this.gm)
		{
			this.gm = GameManager.instance;
		}
		this.gm.achievementHandler.AwardAchievementEvent += this.CaptureAchievementEvent;
	}

	// Token: 0x06001CF2 RID: 7410 RVA: 0x000867A8 File Offset: 0x000849A8
	private void OnDisable()
	{
		this.gm.achievementHandler.AwardAchievementEvent -= this.CaptureAchievementEvent;
	}

	// Token: 0x06001CF3 RID: 7411 RVA: 0x000867C8 File Offset: 0x000849C8
	private void CaptureAchievementEvent(string achievementKey)
	{
		Achievement achievement = this.gm.achievementHandler.AchievementsList.FindAchievement(achievementKey);
		this.icon.sprite = achievement.Icon;
		this.title.text = Language.Get(achievement.TitleCell, "Achievements");
		this.text.text = Language.Get(achievement.DescriptionCell, "Achievements");
		if (this.fsmToSendEvent && !string.IsNullOrEmpty(this.eventName))
		{
			this.fsmToSendEvent.SendEvent(this.eventName);
		}
	}

	// Token: 0x04001C4A RID: 7242
	private GameManager gm;

	// Token: 0x04001C4B RID: 7243
	public SpriteRenderer icon;

	// Token: 0x04001C4C RID: 7244
	public TextMeshPro title;

	// Token: 0x04001C4D RID: 7245
	public TextMeshPro text;

	// Token: 0x04001C4E RID: 7246
	public PlayMakerFSM fsmToSendEvent;

	// Token: 0x04001C4F RID: 7247
	public string eventName;
}
