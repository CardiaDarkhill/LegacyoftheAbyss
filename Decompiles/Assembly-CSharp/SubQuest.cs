using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005AE RID: 1454
[CreateAssetMenu(menuName = "Hornet/Quests/Sub Quest")]
public class SubQuest : BasicQuestBase, ICollectableUIMsgItem, IUIMsgPopupItem, IQuestWithCompletion, IMasterListExclude
{
	// Token: 0x170005C7 RID: 1479
	// (get) Token: 0x0600343C RID: 13372 RVA: 0x000E86FE File Offset: 0x000E68FE
	public override QuestType QuestType
	{
		get
		{
			return this.questType;
		}
	}

	// Token: 0x170005C8 RID: 1480
	// (get) Token: 0x0600343D RID: 13373 RVA: 0x000E8706 File Offset: 0x000E6906
	public QuestTargetCounter TargetCounter
	{
		get
		{
			return this.targetCounter;
		}
	}

	// Token: 0x170005C9 RID: 1481
	// (get) Token: 0x0600343E RID: 13374 RVA: 0x000E870E File Offset: 0x000E690E
	public int TargetCount
	{
		get
		{
			return this.targetCount;
		}
	}

	// Token: 0x170005CA RID: 1482
	// (get) Token: 0x0600343F RID: 13375 RVA: 0x000E8716 File Offset: 0x000E6916
	public override bool IsAvailable
	{
		get
		{
			return !this.CanGetMore();
		}
	}

	// Token: 0x170005CB RID: 1483
	// (get) Token: 0x06003440 RID: 13376 RVA: 0x000E8721 File Offset: 0x000E6921
	public override bool IsAccepted
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170005CC RID: 1484
	// (get) Token: 0x06003441 RID: 13377 RVA: 0x000E8724 File Offset: 0x000E6924
	public override bool IsHidden
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170005CD RID: 1485
	// (get) Token: 0x06003442 RID: 13378 RVA: 0x000E8727 File Offset: 0x000E6927
	public bool CanComplete
	{
		get
		{
			return !this.CanGetMore();
		}
	}

	// Token: 0x170005CE RID: 1486
	// (get) Token: 0x06003443 RID: 13379 RVA: 0x000E8732 File Offset: 0x000E6932
	public bool IsCompleted
	{
		get
		{
			return !this.CanGetMore();
		}
	}

	// Token: 0x170005CF RID: 1487
	// (get) Token: 0x06003444 RID: 13380 RVA: 0x000E873D File Offset: 0x000E693D
	// (set) Token: 0x06003445 RID: 13381 RVA: 0x000E875E File Offset: 0x000E695E
	public override bool HasBeenSeen
	{
		get
		{
			return string.IsNullOrEmpty(this.seenBool) || PlayerData.instance.GetVariable(this.seenBool);
		}
		set
		{
			if (!string.IsNullOrEmpty(this.seenBool))
			{
				PlayerData.instance.SetVariable(this.seenBool, value);
			}
		}
	}

	// Token: 0x170005D0 RID: 1488
	// (get) Token: 0x06003446 RID: 13382 RVA: 0x000E877E File Offset: 0x000E697E
	public override bool IsMapMarkerVisible
	{
		get
		{
			return !this.CanComplete && this.FindActiveMainQuest();
		}
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x000E879A File Offset: 0x000E699A
	private void OnEnable()
	{
		this.questType = QuestType.Create(this.typeDisplayName, this.typeIcon, new Color(0.6f, 0.6f, 0.6f), null, null, this.typeIconGlow);
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x000E87CF File Offset: 0x000E69CF
	private void OnDisable()
	{
		Object.DestroyImmediate(this.questType);
		this.questType = null;
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x000E87E3 File Offset: 0x000E69E3
	public override string GetDescription(BasicQuestBase.ReadSource readSource)
	{
		if (!this.inventoryCompletableDescription.IsEmpty && this.CanComplete)
		{
			return this.inventoryCompletableDescription;
		}
		return this.inventoryDescription;
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x000E8814 File Offset: 0x000E6A14
	public override void Get(bool showPopup = true)
	{
		QuestManager.IncrementVersion();
		if (!string.IsNullOrEmpty(this.linkedBool))
		{
			PlayerData.instance.SetVariable(this.linkedBool, true);
		}
		if (this.targetCounter)
		{
			this.targetCounter.Get(showPopup);
		}
		else if (showPopup)
		{
			QuestManager.ShowQuestUpdatedStandalone(this);
		}
		GameManager instance = GameManager.instance;
		instance.CheckSubQuestAchievements();
		if (this.queueSaveGame)
		{
			instance.QueueSaveGame();
		}
		if (this.queueAutoSaveOnGet > AutoSaveName.NONE)
		{
			instance.QueueAutoSave(this.queueAutoSaveOnGet);
		}
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x000E8898 File Offset: 0x000E6A98
	public override bool CanGetMore()
	{
		foreach (BasicQuestBase basicQuestBase in QuestManager.GetAllQuests())
		{
			MainQuest mainQuest = basicQuestBase as MainQuest;
			if (mainQuest)
			{
				foreach (SubQuest subQuest in mainQuest.SubQuests)
				{
					while (subQuest)
					{
						if (!(subQuest != this) && mainQuest.IsCompleted)
						{
							return false;
						}
						subQuest = subQuest.GetNext();
					}
				}
			}
		}
		return !this.IsLinkedBoolComplete || (this.targetCounter && this.targetCounter.GetCompletionAmount(default(QuestCompletionData.Completion)) < this.targetCount);
	}

	// Token: 0x170005D1 RID: 1489
	// (get) Token: 0x0600344C RID: 13388 RVA: 0x000E8980 File Offset: 0x000E6B80
	public bool IsLinkedBoolComplete
	{
		get
		{
			return string.IsNullOrEmpty(this.linkedBool) || PlayerData.instance.GetVariable(this.linkedBool);
		}
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x000E89A1 File Offset: 0x000E6BA1
	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		yield break;
	}

	// Token: 0x0600344E RID: 13390 RVA: 0x000E89AA File Offset: 0x000E6BAA
	public Object GetRepresentingObject()
	{
		return this;
	}

	// Token: 0x0600344F RID: 13391 RVA: 0x000E89AD File Offset: 0x000E6BAD
	public Sprite GetUIMsgSprite()
	{
		return this.typeIcon;
	}

	// Token: 0x06003450 RID: 13392 RVA: 0x000E89B5 File Offset: 0x000E6BB5
	public string GetUIMsgName()
	{
		return base.DisplayName;
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x000E89C2 File Offset: 0x000E6BC2
	public float GetUIMsgIconScale()
	{
		return 1f;
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x000E89C9 File Offset: 0x000E6BC9
	public SubQuest GetCurrent()
	{
		if (this.nextSubQuest && (this.CanComplete || this.nextSubQuest.CanComplete))
		{
			return this.nextSubQuest.GetCurrent();
		}
		return this;
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x000E89FA File Offset: 0x000E6BFA
	public SubQuest GetNext()
	{
		return this.nextSubQuest;
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x000E8A02 File Offset: 0x000E6C02
	public bool CanShowUpdated(bool isStandalone)
	{
		return (isStandalone || this.showQuestUpdated) && this.FindActiveMainQuest();
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x000E8A1C File Offset: 0x000E6C1C
	private MainQuest FindActiveMainQuest()
	{
		foreach (MainQuest mainQuest in QuestManager.GetActiveQuests().OfType<MainQuest>())
		{
			if (mainQuest.WouldMapMarkerBeVisible)
			{
				foreach (SubQuest subQuest in mainQuest.SubQuests)
				{
					while (subQuest)
					{
						if (subQuest.nextSubQuest == this && subQuest.GetCurrent() != this)
						{
							return null;
						}
						if (!(subQuest != this))
						{
							return mainQuest;
						}
						subQuest = subQuest.GetNext();
					}
				}
			}
		}
		return null;
	}

	// Token: 0x040037CA RID: 14282
	[Header("- Sub Quest")]
	[SerializeField]
	private LocalisedString typeDisplayName = new LocalisedString
	{
		Sheet = "Quests",
		Key = "TYPE_"
	};

	// Token: 0x040037CB RID: 14283
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIcon;

	// Token: 0x040037CC RID: 14284
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Sprite typeIconGlow;

	// Token: 0x040037CD RID: 14285
	[SerializeField]
	private LocalisedString inventoryDescription;

	// Token: 0x040037CE RID: 14286
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString inventoryCompletableDescription;

	// Token: 0x040037CF RID: 14287
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string linkedBool;

	// Token: 0x040037D0 RID: 14288
	[SerializeField]
	private int targetCount;

	// Token: 0x040037D1 RID: 14289
	[SerializeField]
	private QuestTargetCounter targetCounter;

	// Token: 0x040037D2 RID: 14290
	[SerializeField]
	private bool showQuestUpdated = true;

	// Token: 0x040037D3 RID: 14291
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string seenBool;

	// Token: 0x040037D4 RID: 14292
	[SerializeField]
	private AutoSaveName queueAutoSaveOnGet;

	// Token: 0x040037D5 RID: 14293
	[SerializeField]
	private bool queueSaveGame;

	// Token: 0x040037D6 RID: 14294
	[Space]
	[SerializeField]
	private SubQuest nextSubQuest;

	// Token: 0x040037D7 RID: 14295
	private QuestType questType;
}
