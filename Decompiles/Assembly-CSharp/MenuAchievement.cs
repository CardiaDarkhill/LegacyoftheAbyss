using System;
using GlobalEnums;
using PolyAndCode.UI;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D7 RID: 1751
public class MenuAchievement : MonoBehaviour, ICell
{
	// Token: 0x06003F53 RID: 16211 RVA: 0x0011794D File Offset: 0x00115B4D
	private void OnEnable()
	{
		this.gm = GameManager.instance;
		if (this.gm)
		{
			this.gm.RefreshLanguageText += this.Refresh;
		}
		this.Refresh();
	}

	// Token: 0x06003F54 RID: 16212 RVA: 0x00117984 File Offset: 0x00115B84
	private void OnDisable()
	{
		if (this.gm == null)
		{
			return;
		}
		this.gm.RefreshLanguageText -= this.Refresh;
		this.gm = null;
	}

	// Token: 0x06003F55 RID: 16213 RVA: 0x001179B3 File Offset: 0x00115BB3
	public void ConfigureCell(Achievement ach, int cellIndex)
	{
		this.achievement = ach;
		this.Refresh();
	}

	// Token: 0x06003F56 RID: 16214 RVA: 0x001179C4 File Offset: 0x00115BC4
	public void Refresh()
	{
		if (!this.gm || this.achievement == null)
		{
			return;
		}
		try
		{
			if (this.gm.IsAchievementAwarded(this.achievement.PlatformKey))
			{
				this.icon.sprite = this.achievement.Icon;
				this.icon.color = Color.white;
				this.title.text = Language.Get(this.achievement.TitleCell, "Achievements");
				this.text.text = Language.Get(this.achievement.DescriptionCell, "Achievements");
			}
			else if (this.achievement.Type == AchievementType.Normal)
			{
				this.icon.sprite = this.achievement.Icon;
				this.icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
				this.title.text = Language.Get(this.achievement.TitleCell, "Achievements");
				this.text.text = Language.Get(this.achievement.DescriptionCell, "Achievements");
			}
			else
			{
				this.icon.sprite = this.hiddenIconSprite;
				this.icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
				this.title.text = Language.Get("HIDDEN_ACHIEVEMENT_TITLE", "Achievements");
				this.text.text = Language.Get((Application.platform == RuntimePlatform.Switch) ? "HIDDEN_ACHIEVEMENT_ALT" : "HIDDEN_ACHIEVEMENT", "Achievements");
			}
		}
		catch (Exception)
		{
			if (this.achievement.Type == AchievementType.Normal)
			{
				this.icon.sprite = this.achievement.Icon;
				this.icon.color = new Color(0.57f, 0.57f, 0.57f, 0.57f);
				this.title.text = Language.Get(this.achievement.TitleCell, "Achievements");
				this.text.text = Language.Get(this.achievement.DescriptionCell, "Achievements");
			}
			else
			{
				this.icon.sprite = this.hiddenIconSprite;
				this.title.text = Language.Get("HIDDEN_ACHIEVEMENT_TITLE", "Achievements");
				this.text.text = Language.Get("HIDDEN_ACHIEVEMENT", "Achievements");
			}
		}
	}

	// Token: 0x0400411D RID: 16669
	[SerializeField]
	private Image icon;

	// Token: 0x0400411E RID: 16670
	[SerializeField]
	private Sprite hiddenIconSprite;

	// Token: 0x0400411F RID: 16671
	[SerializeField]
	private Text title;

	// Token: 0x04004120 RID: 16672
	[SerializeField]
	private Text text;

	// Token: 0x04004121 RID: 16673
	private Achievement achievement;

	// Token: 0x04004122 RID: 16674
	private GameManager gm;
}
