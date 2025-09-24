using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000594 RID: 1428
[CreateAssetMenu(menuName = "Hornet/Quests/Main Quest")]
public class MainQuest : FullQuestBase
{
	// Token: 0x17000598 RID: 1432
	// (get) Token: 0x06003356 RID: 13142 RVA: 0x000E46E4 File Offset: 0x000E28E4
	public override QuestType QuestType
	{
		get
		{
			return this.questType;
		}
	}

	// Token: 0x17000599 RID: 1433
	// (get) Token: 0x06003357 RID: 13143 RVA: 0x000E46EC File Offset: 0x000E28EC
	public IReadOnlyList<SubQuest> SubQuests
	{
		get
		{
			return this.subQuests;
		}
	}

	// Token: 0x1700059A RID: 1434
	// (get) Token: 0x06003358 RID: 13144 RVA: 0x000E46F4 File Offset: 0x000E28F4
	public IReadOnlyList<MainQuest.AltQuestTarget> AltTargets
	{
		get
		{
			return this.altTargets;
		}
	}

	// Token: 0x1700059B RID: 1435
	// (get) Token: 0x06003359 RID: 13145 RVA: 0x000E46FC File Offset: 0x000E28FC
	public override bool CanComplete
	{
		get
		{
			if (this.subQuests == null)
			{
				return base.CanComplete;
			}
			int num;
			int num2;
			this.GetCompletionCount(out num, out num2);
			num += num2;
			if (this.subQuestsRequired > 0)
			{
				if (num < this.subQuestsRequired)
				{
					return false;
				}
			}
			else if (num < this.subQuests.Length)
			{
				return false;
			}
			return base.CanComplete;
		}
	}

	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x0600335A RID: 13146 RVA: 0x000E4750 File Offset: 0x000E2950
	public bool IsAnyAltTargetsComplete
	{
		get
		{
			if (!this.CanComplete)
			{
				return false;
			}
			int num;
			int num2;
			this.GetCompletionCount(out num, out num2);
			return num2 > 0;
		}
	}

	// Token: 0x1700059D RID: 1437
	// (get) Token: 0x0600335B RID: 13147 RVA: 0x000E4778 File Offset: 0x000E2978
	public override bool IsMapMarkerVisible
	{
		get
		{
			if (this.subQuests == null)
			{
				return base.IsMapMarkerVisible;
			}
			SubQuest[] array = this.subQuests;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsMapMarkerVisible)
				{
					return false;
				}
			}
			return base.IsMapMarkerVisible;
		}
	}

	// Token: 0x1700059E RID: 1438
	// (get) Token: 0x0600335C RID: 13148 RVA: 0x000E47BB File Offset: 0x000E29BB
	public bool WouldMapMarkerBeVisible
	{
		get
		{
			return base.IsMapMarkerVisible && !this.IsAnyAltTargetsComplete;
		}
	}

	// Token: 0x0600335D RID: 13149 RVA: 0x000E47D0 File Offset: 0x000E29D0
	protected override void OnEnable()
	{
		base.OnEnable();
		this.questType = QuestType.Create(this.typeDisplayName, this.typeIcon, this.typeColor, this.typeLargeIcon, this.typeLargeIconGlow, null);
	}

	// Token: 0x0600335E RID: 13150 RVA: 0x000E4802 File Offset: 0x000E2A02
	protected override void OnDisable()
	{
		base.OnDisable();
		Object.DestroyImmediate(this.questType);
		this.questType = null;
	}

	// Token: 0x0600335F RID: 13151 RVA: 0x000E481C File Offset: 0x000E2A1C
	protected override void ShowQuestAccepted(Action afterPrompt, bool hasPreviousQuest)
	{
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = (hasPreviousQuest ? UI.MainQuestProgressPopup : UI.MainQuestBeginPopup),
			Icon = this.typeIcon,
			IconScale = 1f,
			RepresentingObject = this
		}, null, true);
		UI.QuestContinuePopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero, null);
		if (afterPrompt != null)
		{
			afterPrompt();
		}
	}

	// Token: 0x06003360 RID: 13152 RVA: 0x000E48A0 File Offset: 0x000E2AA0
	protected override void ShowQuestCompleted(Action afterPrompt)
	{
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = UI.MainQuestCompletePopup,
			Icon = this.typeIcon,
			IconScale = 1f,
			RepresentingObject = this
		}, null, true);
		UI.QuestContinuePopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero, null);
		if (afterPrompt != null)
		{
			afterPrompt();
		}
	}

	// Token: 0x06003361 RID: 13153 RVA: 0x000E4918 File Offset: 0x000E2B18
	protected override bool ConsumeTargets()
	{
		bool result = base.ConsumeTargets();
		if (this.subQuests == null)
		{
			return result;
		}
		foreach (SubQuest subQuest in this.subQuests)
		{
			QuestTargetCounter targetCounter = subQuest.TargetCounter;
			if (!(targetCounter == null))
			{
				targetCounter.Consume(subQuest.TargetCount, false);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x06003362 RID: 13154 RVA: 0x000E4974 File Offset: 0x000E2B74
	public void GetCompletionCount(out int subQuestsComplete, out int altTargetsComplete)
	{
		subQuestsComplete = 0;
		altTargetsComplete = 0;
		SubQuest[] array = this.subQuests;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].GetCurrent().CanGetMore())
			{
				subQuestsComplete++;
			}
		}
		foreach (MainQuest.AltQuestTarget altQuestTarget in this.altTargets)
		{
			int num;
			if (altQuestTarget.AltTest.IsDefined && altQuestTarget.AltTest.IsFulfilled)
			{
				num = altQuestTarget.Count;
			}
			else
			{
				num = altQuestTarget.Counter.GetCompletionAmount(default(QuestCompletionData.Completion));
			}
			if (num >= altQuestTarget.Count)
			{
				altTargetsComplete++;
			}
		}
	}

	// Token: 0x04003727 RID: 14119
	[Header("- Main Quest")]
	[SerializeField]
	private LocalisedString typeDisplayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	// Token: 0x04003728 RID: 14120
	[SerializeField]
	private Color typeColor = new Color(0.5960785f, 0.5960785f, 0.5960785f, 1f);

	// Token: 0x04003729 RID: 14121
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIcon;

	// Token: 0x0400372A RID: 14122
	[SerializeField]
	private Sprite typeLargeIcon;

	// Token: 0x0400372B RID: 14123
	[SerializeField]
	private Sprite typeLargeIconGlow;

	// Token: 0x0400372C RID: 14124
	[Space]
	[SerializeField]
	private SubQuest[] subQuests;

	// Token: 0x0400372D RID: 14125
	[SerializeField]
	private MainQuest.AltQuestTarget[] altTargets;

	// Token: 0x0400372E RID: 14126
	[SerializeField]
	private int subQuestsRequired;

	// Token: 0x0400372F RID: 14127
	private QuestType questType;

	// Token: 0x020018B5 RID: 6325
	[Serializable]
	public struct AltQuestTarget
	{
		// Token: 0x0400931F RID: 37663
		public QuestTargetCounter Counter;

		// Token: 0x04009320 RID: 37664
		public int Count;

		// Token: 0x04009321 RID: 37665
		public PlayerDataTest AltTest;
	}
}
