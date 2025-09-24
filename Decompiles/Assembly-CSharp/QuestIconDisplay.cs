using System;
using System.Linq;
using TMProOld;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class QuestIconDisplay : MonoBehaviour
{
	// Token: 0x06003390 RID: 13200 RVA: 0x000E59BB File Offset: 0x000E3BBB
	private void Awake()
	{
		if (this.hasWoken)
		{
			return;
		}
		this.hasWoken = true;
		if (this.targetCounterIcon)
		{
			this.counterIconScale = this.targetCounterIcon.transform.localScale;
		}
	}

	// Token: 0x06003391 RID: 13201 RVA: 0x000E59F0 File Offset: 0x000E3BF0
	public void SetQuest(FullQuestBase quest)
	{
		this.Awake();
		QuestType questType = quest.QuestType;
		QuestIconDisplay.IconSizes iconSizes = this.iconSize;
		if (iconSizes != QuestIconDisplay.IconSizes.Large)
		{
			if (iconSizes != QuestIconDisplay.IconSizes.Small)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (this.icon)
			{
				this.icon.sprite = questType.Icon;
			}
			SpriteRenderer[] array = this.glows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = questType.CanCompleteIcon;
			}
		}
		else
		{
			if (this.icon)
			{
				this.icon.sprite = questType.LargeIcon;
			}
			SpriteRenderer[] array = this.glows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = questType.LargeIconGlow;
			}
		}
		if (this.nameText)
		{
			this.nameText.text = quest.DisplayName;
		}
		if (this.typeText)
		{
			this.typeText.text = questType.DisplayName;
			this.typeText.color = questType.TextColor;
		}
		if (this.targetCounterIcon)
		{
			this.targetCounterIcon.sprite = quest.GetCounterSpriteOverride(quest.Targets.FirstOrDefault<FullQuestBase.QuestTarget>(), 0);
			this.targetCounterIcon.transform.localScale = this.counterIconScale.MultiplyElements(new Vector3(quest.CounterIconScale, quest.CounterIconScale, 1f));
		}
		if (this.targetCountText)
		{
			int count = quest.Targets.FirstOrDefault<FullQuestBase.QuestTarget>().Count;
			this.targetCountText.text = count.ToString();
		}
	}

	// Token: 0x04003747 RID: 14151
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x04003748 RID: 14152
	[SerializeField]
	private QuestIconDisplay.IconSizes iconSize;

	// Token: 0x04003749 RID: 14153
	[SerializeField]
	private SpriteRenderer[] glows;

	// Token: 0x0400374A RID: 14154
	[SerializeField]
	private TMP_Text nameText;

	// Token: 0x0400374B RID: 14155
	[SerializeField]
	private TMP_Text typeText;

	// Token: 0x0400374C RID: 14156
	[Space]
	[SerializeField]
	private SpriteRenderer targetCounterIcon;

	// Token: 0x0400374D RID: 14157
	[SerializeField]
	private TMP_Text targetCountText;

	// Token: 0x0400374E RID: 14158
	private bool hasWoken;

	// Token: 0x0400374F RID: 14159
	private Vector3 counterIconScale;

	// Token: 0x020018C3 RID: 6339
	public enum IconSizes
	{
		// Token: 0x04009345 RID: 37701
		Large,
		// Token: 0x04009346 RID: 37702
		Small
	}
}
