using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000596 RID: 1430
public class QuestBoardInteractable : NPCControlBase
{
	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x06003366 RID: 13158 RVA: 0x000E4A88 File Offset: 0x000E2C88
	public override string InteractLabelDisplay
	{
		get
		{
			if (!this.IsAnyQuestsCompletable)
			{
				return base.InteractLabelDisplay;
			}
			return InteractableBase.PromptLabels.TurnIn.ToString();
		}
	}

	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x06003367 RID: 13159 RVA: 0x000E4AB4 File Offset: 0x000E2CB4
	public IReadOnlyCollection<QuestGroupBase> Quests
	{
		get
		{
			return this.questList;
		}
	}

	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003368 RID: 13160 RVA: 0x000E4ABC File Offset: 0x000E2CBC
	private bool IsAnyQuestsCompletable
	{
		get
		{
			return this.Quests.SelectMany((QuestGroupBase group) => group.GetQuests()).OfType<FullQuestBase>().Any((FullQuestBase quest) => quest.GetIsReadyToTurnIn(true));
		}
	}

	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x06003369 RID: 13161 RVA: 0x000E4B1C File Offset: 0x000E2D1C
	private IReadOnlyList<BasicQuestBase> CurrentQuests
	{
		get
		{
			if (!CheatManager.ShowAllQuestBoardQuest && !this.activeCondition.IsFulfilled)
			{
				return Array.Empty<BasicQuestBase>();
			}
			return this.questBoard.Quests;
		}
	}

	// Token: 0x0600336A RID: 13162 RVA: 0x000E4B54 File Offset: 0x000E2D54
	protected override void Awake()
	{
		base.Awake();
		foreach (BasicQuestBase basicQuestBase in this.Quests.SelectMany((QuestGroupBase group) => group.GetQuests()))
		{
			FullQuestBase fullQuestBase = basicQuestBase as FullQuestBase;
			if (!(fullQuestBase == null) && fullQuestBase.GetIsReadyToTurnIn(true) && !(fullQuestBase.RewardItem == null))
			{
				ISavedItemPreSpawn savedItemPreSpawn = fullQuestBase.RewardItem as ISavedItemPreSpawn;
				PreSpawnedItem preSpawnedItem;
				if (savedItemPreSpawn != null && savedItemPreSpawn.TryGetPrespawnedItem(out preSpawnedItem))
				{
					if (!this.preSpawnedItems.TryAdd(fullQuestBase, new QuestBoardInteractable.PreSpawnedItemData
					{
						preSpawnedItem = preSpawnedItem,
						itemPreSpawner = savedItemPreSpawn
					}))
					{
						preSpawnedItem.Dispose();
					}
					else
					{
						preSpawnedItem.SpawnedObject.SetActive(true);
						preSpawnedItem.OnAwake();
					}
				}
			}
		}
	}

	// Token: 0x0600336B RID: 13163 RVA: 0x000E4C4C File Offset: 0x000E2E4C
	protected override void Start()
	{
		foreach (KeyValuePair<FullQuestBase, QuestBoardInteractable.PreSpawnedItemData> keyValuePair in this.preSpawnedItems)
		{
			keyValuePair.Value.preSpawnedItem.OnStart();
			keyValuePair.Value.preSpawnedItem.SpawnedObject.SetActive(false);
		}
		if (this.questBoardPrefab)
		{
			this.questBoard = Object.Instantiate<QuestItemBoard>(this.questBoardPrefab, base.transform, true);
			this.questBoard.transform.SetParent(null, true);
			this.questBoard.GetAvailableQuestsFunc = new Func<IReadOnlyCollection<QuestGroupBase>>(this.GetDisplayedQuests);
			this.questBoard.IsActionsBlocked = true;
			this.questBoard.UpdateList();
			this.questBoard.IsActionsBlocked = false;
			this.questBoard.QuestAccepted += this.UpdateQuestDisplay;
			this.questBoard.DonateQuestAccepted += delegate(FullQuestBase quest)
			{
				this.donateQuest = quest;
				this.UpdateQuestDisplay();
			};
		}
		bool isFulfilled = this.activeCondition.IsFulfilled;
		if (!isFulfilled)
		{
			base.Deactivate(false);
		}
		this.activeObjects.SetAllActive(isFulfilled);
		this.inactiveObjects.SetAllActive(!isFulfilled);
		if (this.questBoard)
		{
			this.SetupQuestDisplay();
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "CONTINUE QUEST BOARD DEPOSIT").ReceivedEvent += delegate()
		{
			if (StaticVariableList.GetValue<bool>("IsInQuestBoardDepositSequence"))
			{
				this.ProcessQueuedCompletions();
			}
		};
	}

	// Token: 0x0600336C RID: 13164 RVA: 0x000E4DC8 File Offset: 0x000E2FC8
	private void OnDestroy()
	{
		foreach (KeyValuePair<FullQuestBase, QuestBoardInteractable.PreSpawnedItemData> keyValuePair in this.preSpawnedItems)
		{
			PreSpawnedItem preSpawnedItem = keyValuePair.Value.preSpawnedItem;
			if (preSpawnedItem != null)
			{
				preSpawnedItem.Dispose();
			}
		}
	}

	// Token: 0x0600336D RID: 13165 RVA: 0x000E4E2C File Offset: 0x000E302C
	private IReadOnlyCollection<QuestGroupBase> GetDisplayedQuests()
	{
		GameObject gameObject = base.gameObject;
		string variableName = "QuestBoardState_" + gameObject.name + "_" + gameObject.scene.name;
		IEnumerable<QuestGroupBase> source = this.Quests;
		string text = StaticVariableList.GetValue<string>(variableName, null);
		if (!string.IsNullOrEmpty(text))
		{
			QuestBoardInteractable.SerializableQuestList cachedQuestsDeserialized = JsonUtility.FromJson<QuestBoardInteractable.SerializableQuestList>(text);
			source = from quest in source
			where cachedQuestsDeserialized.QuestNames.Contains(quest.name)
			select quest;
		}
		if (CheatManager.ShowAllQuestBoardQuest)
		{
			return source.ToList<QuestGroupBase>();
		}
		List<BasicQuestBase> list = source.SelectMany((QuestGroupBase questBase) => questBase.GetQuests()).Where(delegate(BasicQuestBase quest)
		{
			if (quest != this.donateQuest && !quest.IsAccepted && quest.IsAvailable)
			{
				IQuestWithCompletion questWithCompletion = quest as IQuestWithCompletion;
				return questWithCompletion == null || !questWithCompletion.IsCompleted;
			}
			return false;
		}).Take(6).ToList<BasicQuestBase>();
		List<string> questNames = (from quest in list
		select quest.name).ToList<string>();
		text = JsonUtility.ToJson(new QuestBoardInteractable.SerializableQuestList
		{
			QuestNames = questNames
		});
		StaticVariableList.SetValue(variableName, text, 3);
		return list;
	}

	// Token: 0x0600336E RID: 13166 RVA: 0x000E4F3C File Offset: 0x000E313C
	protected override void OnStartDialogue()
	{
		this.donateQuest = null;
		if (!this.questBoard)
		{
			Debug.LogError("No quest board assigned!", this);
			return;
		}
		base.DisableInteraction();
		if (this.queuedCompletions == null)
		{
			this.queuedCompletions = new Queue<FullQuestBase>();
		}
		else
		{
			this.queuedCompletions.Clear();
		}
		bool flag = false;
		foreach (BasicQuestBase basicQuestBase in this.Quests.SelectMany((QuestGroupBase group) => group.GetQuests()))
		{
			FullQuestBase fullQuestBase = basicQuestBase as FullQuestBase;
			if (!(fullQuestBase == null) && fullQuestBase.GetIsReadyToTurnIn(true))
			{
				this.queuedCompletions.Enqueue(fullQuestBase);
				foreach (FullQuestBase.QuestTarget questTarget in fullQuestBase.Targets)
				{
					if (!questTarget.Counter || questTarget.Counter.CanConsume)
					{
						flag = true;
					}
				}
			}
		}
		if (this.queuedCompletions.Count <= 0)
		{
			this.OpenBoard();
			return;
		}
		if (this.handInSequenceFsm)
		{
			this.handInSequenceFsm.SendEvent(flag ? "QUESTS" : "QUESTS_NOCONSUME");
			return;
		}
		this.DepositQuestItems();
		this.ProcessQueuedCompletions();
	}

	// Token: 0x0600336F RID: 13167 RVA: 0x000E50B4 File Offset: 0x000E32B4
	[UsedImplicitly]
	public void DepositQuestItems()
	{
		StaticVariableList.SetValue("IsInQuestBoardDepositSequence", true, 0);
		foreach (FullQuestBase fullQuestBase in this.queuedCompletions)
		{
			if (fullQuestBase.ConsumeTarget())
			{
				foreach (FullQuestBase.QuestTarget questTarget in fullQuestBase.Targets)
				{
					CollectableUIMsg.ShowTakeMsg(questTarget.Counter, TakeItemTypes.Deposited);
				}
			}
		}
	}

	// Token: 0x06003370 RID: 13168 RVA: 0x000E5158 File Offset: 0x000E3358
	[UsedImplicitly]
	public void ProcessQueuedCompletions()
	{
		if (this.queuedCompletions.Count == 0)
		{
			StaticVariableList.SetValue("IsInQuestBoardDepositSequence", false, 0);
			this.UpdateQuestDisplay();
			base.EndDialogue();
			return;
		}
		FullQuestBase completeQuest = this.queuedCompletions.Dequeue();
		completeQuest.TryEndQuest(delegate
		{
			bool flag = false;
			QuestBoardInteractable.PreSpawnedItemData preSpawnedItemData;
			if (this.preSpawnedItems.TryGetValue(completeQuest, out preSpawnedItemData))
			{
				preSpawnedItemData.itemPreSpawner.PreSpawnGet(true);
				preSpawnedItemData.preSpawnedItem.SpawnedObject.SetActive(true);
				flag = true;
			}
			SavedItem rewardItem = completeQuest.RewardItem;
			if (rewardItem)
			{
				if (!flag)
				{
					rewardItem.Get(completeQuest.RewardCount, true);
				}
				if (rewardItem.GetTakesHeroControl())
				{
					return;
				}
			}
			this.ExecuteDelayed(1f, new Action(this.ProcessQueuedCompletions));
		}, false, true, true);
	}

	// Token: 0x06003371 RID: 13169 RVA: 0x000E51C9 File Offset: 0x000E33C9
	private void OpenBoard()
	{
		this.UpdateQuestIndicator(null, false, true);
		this.questBoard.BoardClosed += this.AcceptQuests;
		this.questBoard.OpenPane();
	}

	// Token: 0x06003372 RID: 13170 RVA: 0x000E51F6 File Offset: 0x000E33F6
	private void AcceptQuests(List<BasicQuestBase> acceptedQuests)
	{
		this.questBoard.BoardClosed -= this.AcceptQuests;
		this.UpdateQuestDisplay();
		base.EndDialogue();
	}

	// Token: 0x06003373 RID: 13171 RVA: 0x000E521B File Offset: 0x000E341B
	protected override void OnEndDialogue()
	{
		if (this.donateQuest == null)
		{
			base.EnableInteraction();
			return;
		}
		this.OnDonateQuestAccepted(this.donateQuest);
	}

	// Token: 0x06003374 RID: 13172 RVA: 0x000E5240 File Offset: 0x000E3440
	private void SetupQuestDisplay()
	{
		IReadOnlyList<BasicQuestBase> readOnlyList = this.CurrentQuests;
		if (CheatManager.ShowAllQuestBoardQuest)
		{
			readOnlyList = this.questBoard.Quests;
		}
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			QuestBoardInteractable.QuestBadge questBadge = this.questDisplays[i];
			if (i < readOnlyList.Count)
			{
				if (questBadge.Tint)
				{
					QuestBoardInteractable.QuestTypeColor questTypeColor = default(QuestBoardInteractable.QuestTypeColor);
					bool flag = false;
					foreach (QuestBoardInteractable.QuestTypeColor questTypeColor2 in this.questTypeColors)
					{
						if (!(questTypeColor2.Type != readOnlyList[i].QuestType))
						{
							questTypeColor = questTypeColor2;
							flag = true;
							break;
						}
					}
					questBadge.Tint.color = (flag ? questTypeColor.Color : Color.white);
				}
				if (questBadge.Fade)
				{
					questBadge.Fade.AlphaSelf = 1f;
				}
				this.boardQuests.Add(new QuestBoardInteractable.BoardQuest
				{
					Quest = readOnlyList[i],
					Display = questBadge
				});
			}
			else if (questBadge.Fade)
			{
				questBadge.Fade.AlphaSelf = 0f;
			}
		}
		this.UpdateQuestIndicator(readOnlyList, true, false);
	}

	// Token: 0x06003375 RID: 13173 RVA: 0x000E53A4 File Offset: 0x000E35A4
	private void UpdateQuestDisplay()
	{
		IReadOnlyList<BasicQuestBase> currentQuests = this.CurrentQuests;
		foreach (QuestBoardInteractable.BoardQuest boardQuest in this.boardQuests)
		{
			NestedFadeGroupBase fade = boardQuest.Display.Fade;
			if (fade && !currentQuests.Contains(boardQuest.Quest) && fade.AlphaSelf > 0f)
			{
				fade.FadeTo(0f, this.questDisplayFadeTime, null, false, null);
			}
		}
		this.UpdateQuestIndicator(currentQuests, false, true);
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x000E5444 File Offset: 0x000E3644
	private void UpdateQuestIndicator(IEnumerable<BasicQuestBase> quests, bool isInstant, bool forceOff)
	{
		if (this.questDisplayHidden)
		{
			return;
		}
		if (this.IsAnyQuestsCompletable)
		{
			return;
		}
		bool flag = true;
		if (!forceOff)
		{
			foreach (BasicQuestBase basicQuestBase in quests)
			{
				if (!basicQuestBase.HasBeenSeen && !basicQuestBase.IsAccepted)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag)
		{
			this.questDisplayHidden = true;
			if (this.newQuestIndicator)
			{
				this.newQuestIndicator.FadeTo(0f, isInstant ? 0f : this.questIndicatorFadeTime, null, false, null);
			}
		}
	}

	// Token: 0x06003377 RID: 13175 RVA: 0x000E54EC File Offset: 0x000E36EC
	private void OnDonateQuestAccepted(FullQuestBase quest)
	{
		if (!this.handInSequenceFsm)
		{
			return;
		}
		this.handInSequenceFsm.FsmVariables.GetFsmObject("Quest").Value = quest;
		this.handInSequenceFsm.SendEvent("DONATE");
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x000E5527 File Offset: 0x000E3727
	public void TakeDonateCurrency()
	{
		this.questBoard.HideCurrencyCounters(this.donateQuest);
		this.donateQuest.ConsumeTarget();
	}

	// Token: 0x06003379 RID: 13177 RVA: 0x000E5546 File Offset: 0x000E3746
	public void OnDonationSequenceComplete()
	{
		this.donateQuest = null;
		this.UpdateQuestDisplay();
		if (this.questBoard.AvailableQuestsCount > 0)
		{
			HeroTalkAnimation.EnterConversation(this);
			this.OpenBoard();
			return;
		}
		base.EnableInteraction();
		GameManager.instance.DoQueuedSaveGame();
	}

	// Token: 0x04003731 RID: 14129
	[SerializeField]
	private QuestItemBoard questBoardPrefab;

	// Token: 0x04003732 RID: 14130
	[SerializeField]
	private QuestBoardList questList;

	// Token: 0x04003733 RID: 14131
	[SerializeField]
	private NestedFadeGroupBase newQuestIndicator;

	// Token: 0x04003734 RID: 14132
	[SerializeField]
	private float questIndicatorFadeTime;

	// Token: 0x04003735 RID: 14133
	[SerializeField]
	private List<QuestBoardInteractable.QuestBadge> questDisplays;

	// Token: 0x04003736 RID: 14134
	[SerializeField]
	private float questDisplayFadeTime;

	// Token: 0x04003737 RID: 14135
	[SerializeField]
	private List<QuestBoardInteractable.QuestTypeColor> questTypeColors;

	// Token: 0x04003738 RID: 14136
	[Space]
	[SerializeField]
	private PlayerDataTest activeCondition;

	// Token: 0x04003739 RID: 14137
	[SerializeField]
	private GameObject[] activeObjects;

	// Token: 0x0400373A RID: 14138
	[SerializeField]
	private GameObject[] inactiveObjects;

	// Token: 0x0400373B RID: 14139
	[Space]
	[SerializeField]
	private PlayMakerFSM handInSequenceFsm;

	// Token: 0x0400373C RID: 14140
	private readonly List<QuestBoardInteractable.BoardQuest> boardQuests = new List<QuestBoardInteractable.BoardQuest>();

	// Token: 0x0400373D RID: 14141
	private Queue<FullQuestBase> queuedCompletions;

	// Token: 0x0400373E RID: 14142
	private QuestItemBoard questBoard;

	// Token: 0x0400373F RID: 14143
	private bool questDisplayHidden;

	// Token: 0x04003740 RID: 14144
	private FullQuestBase donateQuest;

	// Token: 0x04003741 RID: 14145
	private Dictionary<FullQuestBase, QuestBoardInteractable.PreSpawnedItemData> preSpawnedItems = new Dictionary<FullQuestBase, QuestBoardInteractable.PreSpawnedItemData>();

	// Token: 0x04003742 RID: 14146
	public const string IN_DEPOSIT_SEQUENCE_STATIC_BOOL = "IsInQuestBoardDepositSequence";

	// Token: 0x020018B6 RID: 6326
	private struct BoardQuest
	{
		// Token: 0x04009322 RID: 37666
		public BasicQuestBase Quest;

		// Token: 0x04009323 RID: 37667
		public QuestBoardInteractable.QuestBadge Display;
	}

	// Token: 0x020018B7 RID: 6327
	[Serializable]
	private class SerializableQuestList
	{
		// Token: 0x04009324 RID: 37668
		public List<string> QuestNames;
	}

	// Token: 0x020018B8 RID: 6328
	[Serializable]
	private struct QuestTypeColor
	{
		// Token: 0x04009325 RID: 37669
		public QuestType Type;

		// Token: 0x04009326 RID: 37670
		public Color Color;
	}

	// Token: 0x020018B9 RID: 6329
	[Serializable]
	private class QuestBadge
	{
		// Token: 0x04009327 RID: 37671
		public NestedFadeGroupBase Fade;

		// Token: 0x04009328 RID: 37672
		public SpriteRenderer Tint;
	}

	// Token: 0x020018BA RID: 6330
	private struct PreSpawnedItemData
	{
		// Token: 0x04009329 RID: 37673
		public PreSpawnedItem preSpawnedItem;

		// Token: 0x0400932A RID: 37674
		public ISavedItemPreSpawn itemPreSpawner;
	}
}
