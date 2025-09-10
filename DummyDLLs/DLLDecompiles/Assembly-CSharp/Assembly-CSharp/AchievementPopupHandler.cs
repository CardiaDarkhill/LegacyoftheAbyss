using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000604 RID: 1540
public class AchievementPopupHandler : MonoBehaviour
{
	// Token: 0x060036FC RID: 14076 RVA: 0x000F28BE File Offset: 0x000F0ABE
	private void Awake()
	{
		AchievementPopupHandler.Instance = this;
		if (this.template)
		{
			this.popups.Add(this.template);
			this.template.gameObject.SetActive(false);
		}
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x000F28F5 File Offset: 0x000F0AF5
	private void Start()
	{
		this.achievements = GameManager.instance.achievementHandler;
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x000F2907 File Offset: 0x000F0B07
	public void Setup(AchievementHandler handler)
	{
		this.achievements = handler;
		if (!Platform.Current.HasNativeAchievementsDialog)
		{
			this.achievements.AwardAchievementEvent += this.HandleAchievementEvent;
		}
	}

	// Token: 0x060036FF RID: 14079 RVA: 0x000F2933 File Offset: 0x000F0B33
	private void OnDestroy()
	{
		if (!Platform.Current.HasNativeAchievementsDialog)
		{
			this.achievements.AwardAchievementEvent -= this.HandleAchievementEvent;
		}
	}

	// Token: 0x06003700 RID: 14080 RVA: 0x000F2958 File Offset: 0x000F0B58
	private void HandleAchievementEvent(string key)
	{
		Achievement achievement = this.achievements.AchievementsList.FindAchievement(key);
		Sprite icon = achievement.Icon;
		string name = Language.Get(achievement.TitleCell, "Achievements");
		string description = Language.Get(achievement.DescriptionCell, "Achievements");
		AchievementPopup pooledPopup = this.GetPooledPopup();
		if (pooledPopup)
		{
			pooledPopup.Setup(icon, name, description);
			this.lastPopup = pooledPopup;
			pooledPopup.OnFinish += this.DisableAll;
			return;
		}
		Debug.LogError("Could not get achievement popup!");
	}

	// Token: 0x06003701 RID: 14081 RVA: 0x000F29DC File Offset: 0x000F0BDC
	private AchievementPopup GetPooledPopup()
	{
		AchievementPopup achievementPopup = null;
		foreach (AchievementPopup achievementPopup2 in this.popups)
		{
			if (!achievementPopup2.gameObject.activeSelf)
			{
				achievementPopup = achievementPopup2;
				break;
			}
		}
		if (achievementPopup == null && this.template)
		{
			achievementPopup = Object.Instantiate<GameObject>(this.template.gameObject, this.template.transform.parent).GetComponent<AchievementPopup>();
			this.popups.Add(achievementPopup);
		}
		if (this.reverseOrder)
		{
			achievementPopup.transform.SetAsFirstSibling();
		}
		else
		{
			achievementPopup.transform.SetAsLastSibling();
		}
		achievementPopup.gameObject.SetActive(true);
		return achievementPopup;
	}

	// Token: 0x06003702 RID: 14082 RVA: 0x000F2AB0 File Offset: 0x000F0CB0
	private void DisableAll(AchievementPopup sender)
	{
		sender.OnFinish -= this.DisableAll;
		if (sender == this.lastPopup)
		{
			foreach (AchievementPopup achievementPopup in this.popups)
			{
				achievementPopup.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x040039C9 RID: 14793
	public static AchievementPopupHandler Instance;

	// Token: 0x040039CA RID: 14794
	public AchievementPopup template;

	// Token: 0x040039CB RID: 14795
	private List<AchievementPopup> popups = new List<AchievementPopup>();

	// Token: 0x040039CC RID: 14796
	public bool reverseOrder;

	// Token: 0x040039CD RID: 14797
	private AchievementHandler achievements;

	// Token: 0x040039CE RID: 14798
	private AchievementPopup lastPopup;
}
