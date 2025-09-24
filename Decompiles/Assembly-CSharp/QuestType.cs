using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005AD RID: 1453
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Type")]
public class QuestType : ScriptableObject
{
	// Token: 0x170005C0 RID: 1472
	// (get) Token: 0x06003432 RID: 13362 RVA: 0x000E85EC File Offset: 0x000E67EC
	public string DisplayName
	{
		get
		{
			if (!this.displayName.IsEmpty)
			{
				return this.displayName;
			}
			return string.Empty;
		}
	}

	// Token: 0x170005C1 RID: 1473
	// (get) Token: 0x06003433 RID: 13363 RVA: 0x000E860C File Offset: 0x000E680C
	public Sprite Icon
	{
		get
		{
			return this.icon;
		}
	}

	// Token: 0x170005C2 RID: 1474
	// (get) Token: 0x06003434 RID: 13364 RVA: 0x000E8614 File Offset: 0x000E6814
	public Sprite CanCompleteIcon
	{
		get
		{
			return this.canCompleteIcon;
		}
	}

	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003435 RID: 13365 RVA: 0x000E861C File Offset: 0x000E681C
	public Color TextColor
	{
		get
		{
			return this.textColor;
		}
	}

	// Token: 0x170005C4 RID: 1476
	// (get) Token: 0x06003436 RID: 13366 RVA: 0x000E8624 File Offset: 0x000E6824
	public Sprite LargeIcon
	{
		get
		{
			return this.largeIcon;
		}
	}

	// Token: 0x170005C5 RID: 1477
	// (get) Token: 0x06003437 RID: 13367 RVA: 0x000E862C File Offset: 0x000E682C
	public Sprite LargeIconGlow
	{
		get
		{
			return this.largeIconGlow;
		}
	}

	// Token: 0x170005C6 RID: 1478
	// (get) Token: 0x06003438 RID: 13368 RVA: 0x000E8634 File Offset: 0x000E6834
	public bool IsDonateType
	{
		get
		{
			return this.isDonateType;
		}
	}

	// Token: 0x06003439 RID: 13369 RVA: 0x000E863C File Offset: 0x000E683C
	public static QuestType Create(LocalisedString displayName, Sprite icon, Color textColor, Sprite largeIcon, Sprite largeIconGlow, Sprite iconGlow = null)
	{
		QuestType questType = ScriptableObject.CreateInstance<QuestType>();
		questType.name = displayName;
		questType.displayName = displayName;
		questType.icon = icon;
		questType.canCompleteIcon = (iconGlow ? iconGlow : icon);
		questType.textColor = textColor;
		questType.largeIcon = largeIcon;
		questType.largeIconGlow = largeIconGlow;
		return questType;
	}

	// Token: 0x0600343A RID: 13370 RVA: 0x000E8692 File Offset: 0x000E6892
	public void OnQuestCompleted(FullQuestBase quest)
	{
		if (string.IsNullOrEmpty(this.removeQuestFromListOnComplete))
		{
			return;
		}
		List<string> variable = PlayerData.instance.GetVariable(this.removeQuestFromListOnComplete);
		if (variable == null)
		{
			return;
		}
		variable.Remove(quest.name);
	}

	// Token: 0x040037C2 RID: 14274
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString displayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	// Token: 0x040037C3 RID: 14275
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite icon;

	// Token: 0x040037C4 RID: 14276
	[SerializeField]
	private Sprite canCompleteIcon;

	// Token: 0x040037C5 RID: 14277
	[SerializeField]
	private Color textColor;

	// Token: 0x040037C6 RID: 14278
	[Header("Fullscreen Prompt")]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite largeIcon;

	// Token: 0x040037C7 RID: 14279
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite largeIconGlow;

	// Token: 0x040037C8 RID: 14280
	[Header("Properties")]
	[SerializeField]
	private bool isDonateType;

	// Token: 0x040037C9 RID: 14281
	[SerializeField]
	[PlayerDataField(typeof(List<string>), false)]
	private string removeQuestFromListOnComplete;
}
