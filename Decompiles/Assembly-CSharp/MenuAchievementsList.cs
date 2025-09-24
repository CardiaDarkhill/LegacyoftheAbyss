using System;
using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;

// Token: 0x020006D8 RID: 1752
public class MenuAchievementsList : MonoBehaviour, IRecyclableScrollRectDataSource
{
	// Token: 0x06003F58 RID: 16216 RVA: 0x00117C68 File Offset: 0x00115E68
	public void PreInit()
	{
		GameManager instance = GameManager.instance;
		this.achievements = instance.achievementHandler.AchievementsList.Achievements;
		this.scrollRect.DataSource = this;
		this.scrollRect.PreInit();
	}

	// Token: 0x06003F59 RID: 16217 RVA: 0x00117CA8 File Offset: 0x00115EA8
	private void OnEnable()
	{
		if (this.achievements == null)
		{
			Debug.LogError("ERROR: Can't load achievements if they haven't been fetched yet!");
			return;
		}
		this.scrollRect.ReloadData();
	}

	// Token: 0x06003F5A RID: 16218 RVA: 0x00117CC8 File Offset: 0x00115EC8
	public int GetItemCount()
	{
		return this.achievements.Count;
	}

	// Token: 0x06003F5B RID: 16219 RVA: 0x00117CD5 File Offset: 0x00115ED5
	public void SetCell(ICell cell, int index)
	{
		((MenuAchievement)cell).ConfigureCell(this.achievements[index], index);
	}

	// Token: 0x04004123 RID: 16675
	[SerializeField]
	private RecyclableScrollRect scrollRect;

	// Token: 0x04004124 RID: 16676
	private IReadOnlyList<Achievement> achievements;
}
