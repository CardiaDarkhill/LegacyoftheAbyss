using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GlobalSettings;
using UnityEngine;

// Token: 0x020005A3 RID: 1443
public class QuestManager : MonoBehaviour
{
	// Token: 0x170005AD RID: 1453
	// (get) Token: 0x060033CB RID: 13259 RVA: 0x000E724A File Offset: 0x000E544A
	private static QuestManager Instance
	{
		get
		{
			if (!QuestManager.instance)
			{
				QuestManager.instance = Object.FindAnyObjectByType<QuestManager>();
			}
			return QuestManager.instance;
		}
	}

	// Token: 0x170005AE RID: 1454
	// (get) Token: 0x060033CC RID: 13260 RVA: 0x000E7267 File Offset: 0x000E5467
	// (set) Token: 0x060033CD RID: 13261 RVA: 0x000E7281 File Offset: 0x000E5481
	public static BasicQuestBase UpdatedQuest
	{
		get
		{
			if (!QuestManager.instance)
			{
				return null;
			}
			return QuestManager.instance.updatedQuest;
		}
		set
		{
			if (QuestManager.instance)
			{
				QuestManager.instance.updatedQuest = value;
			}
		}
	}

	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x060033CE RID: 13262 RVA: 0x000E729A File Offset: 0x000E549A
	// (set) Token: 0x060033CF RID: 13263 RVA: 0x000E72A1 File Offset: 0x000E54A1
	public static int Version { get; private set; }

	// Token: 0x060033D0 RID: 13264 RVA: 0x000E72A9 File Offset: 0x000E54A9
	public static void IncrementVersion()
	{
		QuestManager.Version++;
	}

	// Token: 0x060033D1 RID: 13265 RVA: 0x000E72B8 File Offset: 0x000E54B8
	public static void UpgradeQuests()
	{
		if (!QuestManager.instance)
		{
			return;
		}
		foreach (FullQuestBase fullQuestBase in QuestManager.GetAllFullQuests())
		{
			if (fullQuestBase.IsAccepted || fullQuestBase.IsCompleted)
			{
				fullQuestBase.SilentlyCompletePrevious();
			}
		}
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x000E7320 File Offset: 0x000E5520
	private void Awake()
	{
		QuestManager.instance = this;
		foreach (BasicQuestBase basicQuestBase in this.masterList)
		{
			basicQuestBase.Init();
		}
		QuestManager.Version++;
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x000E737C File Offset: 0x000E557C
	private void Start()
	{
		this.PreSpawnSequence(this.questAcceptedSequence, ref this.spawnedQuestAcceptedSequence);
		this.PreSpawnSequence(this.questFinishedSequence, ref this.spawnedQuestFinishedSequence);
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x000E73A4 File Offset: 0x000E55A4
	private void OnDestroy()
	{
		QuestManager.Version++;
		if (this.spawnedQuestAcceptedSequence)
		{
			Object.Destroy(this.spawnedQuestAcceptedSequence);
		}
		if (this.spawnedQuestFinishedSequence)
		{
			Object.Destroy(this.spawnedQuestFinishedSequence);
		}
		if (QuestManager.instance == this)
		{
			QuestManager._acceptedQuests = null;
			QuestManager._activeQuests = null;
			QuestManager._allFullQuests = null;
			QuestManager._fullQuestLookup = null;
			QuestManager.instance = null;
		}
	}

	// Token: 0x060033D5 RID: 13269 RVA: 0x000E7418 File Offset: 0x000E5618
	private void OnValidate()
	{
		QuestManager._allFullQuests = null;
		QuestManager._fullQuestLookup = null;
	}

	// Token: 0x060033D6 RID: 13270 RVA: 0x000E7426 File Offset: 0x000E5626
	private void PreSpawnSequence(GameObject original, ref GameObject target)
	{
		if (!original)
		{
			return;
		}
		target = Object.Instantiate<GameObject>(original);
		target.SetActive(false);
		Transform transform = target.transform;
		transform.SetParent(base.transform, true);
		transform.SetParent(null, true);
	}

	// Token: 0x060033D7 RID: 13271 RVA: 0x000E745C File Offset: 0x000E565C
	public static IEnumerable<BasicQuestBase> GetAllQuests()
	{
		if (QuestManager.Instance && QuestManager.Instance.masterList)
		{
			return QuestManager.Instance.masterList;
		}
		return Enumerable.Empty<BasicQuestBase>();
	}

	// Token: 0x060033D8 RID: 13272 RVA: 0x000E748C File Offset: 0x000E568C
	public static IEnumerable<BasicQuestBase> GetAcceptedQuests()
	{
		if (QuestManager._acceptedQuests == null)
		{
			QuestManager._acceptedQuests = new ObjectCache<IEnumerable<BasicQuestBase>>();
		}
		if (QuestManager._acceptedQuests.ShouldUpdate(QuestManager.Version))
		{
			QuestManager._acceptedQuests.UpdateCache((from quest in QuestManager.GetAllQuests()
			where quest.IsAccepted
			select quest).ToArray<BasicQuestBase>(), QuestManager.Version);
		}
		return QuestManager._acceptedQuests.Value;
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x000E7504 File Offset: 0x000E5704
	public static IEnumerable<FullQuestBase> GetActiveQuests()
	{
		if (QuestManager._activeQuests == null)
		{
			QuestManager._activeQuests = new ObjectCache<IEnumerable<FullQuestBase>>();
		}
		if (QuestManager._activeQuests.ShouldUpdate(QuestManager.Version))
		{
			QuestManager._activeQuests.UpdateCache((from quest in QuestManager.GetAcceptedQuests().OfType<FullQuestBase>()
			where !quest.IsCompleted
			select quest).ToArray<FullQuestBase>(), QuestManager.Version);
		}
		return QuestManager._activeQuests.Value;
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x000E7580 File Offset: 0x000E5780
	public static FullQuestBase GetQuest(string questName)
	{
		FullQuestBase result;
		if (QuestManager.TryGetFullQuestBase(questName, out result))
		{
			return result;
		}
		Debug.LogError("Couldn't get quest: " + questName);
		return null;
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x000E75AC File Offset: 0x000E57AC
	private static IEnumerable<FullQuestBase> GetAllFullQuests()
	{
		if (QuestManager._allFullQuests == null)
		{
			QuestManager questManager = QuestManager.Instance;
			if (!questManager || !questManager.masterList)
			{
				return ArraySegment<FullQuestBase>.Empty;
			}
			QuestManager._allFullQuests = questManager.masterList.OfType<FullQuestBase>().ToArray<FullQuestBase>();
		}
		return QuestManager._allFullQuests;
	}

	// Token: 0x060033DC RID: 13276 RVA: 0x000E7604 File Offset: 0x000E5804
	private static bool TryGetFullQuestBase(string questName, out FullQuestBase fullQuestBase)
	{
		if (QuestManager._fullQuestLookup == null)
		{
			IEnumerable<FullQuestBase> allFullQuests = QuestManager.GetAllFullQuests();
			if (allFullQuests == null || !allFullQuests.Any<FullQuestBase>())
			{
				fullQuestBase = null;
				return false;
			}
			QuestManager._fullQuestLookup = new Dictionary<string, FullQuestBase>();
			foreach (FullQuestBase fullQuestBase2 in allFullQuests)
			{
				if (!(fullQuestBase2 == null))
				{
					QuestManager._fullQuestLookup[fullQuestBase2.name] = fullQuestBase2;
				}
			}
		}
		return QuestManager._fullQuestLookup.TryGetValue(questName, out fullQuestBase);
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x000E7694 File Offset: 0x000E5894
	public static bool IsAnyQuestVisible()
	{
		return GameManager.instance.playerData.QuestCompletionData.GetValidNames((QuestCompletionData.Completion completion) => completion.IsAccepted || completion.IsCompleted).Count > 0;
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x000E76D4 File Offset: 0x000E58D4
	public static bool IsQuestInList(BasicQuestBase quest)
	{
		QuestManager questManager = QuestManager.Instance;
		return questManager && questManager.masterList && questManager.masterList.Contains(quest);
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x000E770A File Offset: 0x000E590A
	public static void ShowQuestAccepted(FullQuestBase quest, Action afterPrompt)
	{
		if (!QuestManager.Instance)
		{
			return;
		}
		QuestManager.Instance.ShowQuestPromptInternal(quest, QuestManager.Instance.spawnedQuestAcceptedSequence, afterPrompt);
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x000E772F File Offset: 0x000E592F
	public static void ShowQuestCompleted(FullQuestBase quest, Action afterPrompt)
	{
		if (!QuestManager.Instance)
		{
			return;
		}
		QuestManager.Instance.ShowQuestPromptInternal(quest, QuestManager.Instance.spawnedQuestFinishedSequence, afterPrompt);
	}

	// Token: 0x060033E1 RID: 13281 RVA: 0x000E7754 File Offset: 0x000E5954
	private void ShowQuestPromptInternal(FullQuestBase quest, GameObject prompt, Action afterPrompt)
	{
		if (!prompt)
		{
			Debug.LogError("No quest accepted object had been previously spawned!", this);
			return;
		}
		prompt.SetActive(true);
		QuestIconDisplay component = prompt.GetComponent<QuestIconDisplay>();
		if (component)
		{
			component.SetQuest(quest);
		}
		if (!this.sequenceEndEvent)
		{
			return;
		}
		Action temp = null;
		temp = delegate()
		{
			this.sequenceEndEvent.ReceivedEvent -= temp;
			if (afterPrompt != null)
			{
				afterPrompt();
			}
		};
		this.sequenceEndEvent.ReceivedEvent += temp;
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x000E77E0 File Offset: 0x000E59E0
	public static bool MaybeShowQuestUpdated(QuestTargetCounter item, CollectableUIMsg itemUiMsg)
	{
		CollectableItem collectableItem = item as CollectableItem;
		FullQuestBase quest;
		if (collectableItem != null)
		{
			quest = collectableItem.UseQuestForCap;
		}
		else
		{
			quest = null;
		}
		return QuestManager.MaybeShowQuestUpdated(quest, item, itemUiMsg);
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x000E780C File Offset: 0x000E5A0C
	public static bool MaybeShowQuestUpdated(FullQuestBase quest, QuestTargetCounter item, CollectableUIMsg itemUiMsg)
	{
		BasicQuestBase basicQuestBase;
		if (quest)
		{
			if (!quest.IsAccepted || quest.IsCompleted || !quest.CanComplete)
			{
				return false;
			}
			basicQuestBase = quest;
		}
		else
		{
			basicQuestBase = null;
			Func<ValueTuple<FullQuestBase.QuestTarget, int>, bool> <>9__0;
			foreach (FullQuestBase fullQuestBase in QuestManager.GetActiveQuests())
			{
				MainQuest mainQuest = fullQuestBase as MainQuest;
				if (mainQuest)
				{
					foreach (SubQuest subQuest in mainQuest.SubQuests)
					{
						do
						{
							if (QuestManager.IsQuestForItem(subQuest.TargetCounter, item) && subQuest.CanShowUpdated(false) && subQuest.CanComplete)
							{
								basicQuestBase = subQuest;
							}
							else
							{
								subQuest = subQuest.GetNext();
							}
						}
						while (subQuest && !basicQuestBase);
						if (basicQuestBase)
						{
							break;
						}
					}
					if (basicQuestBase)
					{
						break;
					}
					using (IEnumerator<MainQuest.AltQuestTarget> enumerator3 = mainQuest.AltTargets.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (QuestManager.IsQuestForItem(enumerator3.Current.Counter, item))
							{
								basicQuestBase = mainQuest;
								break;
							}
						}
					}
				}
				IEnumerable<ValueTuple<FullQuestBase.QuestTarget, int>> targetsAndCounters = fullQuestBase.TargetsAndCounters;
				Func<ValueTuple<FullQuestBase.QuestTarget, int>, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = (([TupleElementNames(new string[]
					{
						"target",
						"count"
					})] ValueTuple<FullQuestBase.QuestTarget, int> pair) => pair.Item1.Counter != item || pair.Item2 != pair.Item1.Count));
				}
				if (!targetsAndCounters.All(predicate))
				{
					basicQuestBase = fullQuestBase;
					break;
				}
			}
		}
		if (!basicQuestBase)
		{
			return false;
		}
		if (itemUiMsg)
		{
			QuestManager.ShowQuestUpdatedForItemMsg(itemUiMsg, basicQuestBase);
		}
		else
		{
			QuestManager.ShowQuestUpdatedStandalone(basicQuestBase);
		}
		return true;
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x000E7A08 File Offset: 0x000E5C08
	private static bool IsQuestForItem(QuestTargetCounter questItem, QuestTargetCounter item)
	{
		return questItem && (questItem == item || questItem.EnumerateSubTargets().Contains(item));
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x000E7A2C File Offset: 0x000E5C2C
	public static void ShowQuestUpdatedForItemMsg(CollectableUIMsg itemUiMsg, BasicQuestBase quest)
	{
		itemUiMsg.Replace(UI.ItemQuestMaxPopupDelay, new UIMsgDisplay
		{
			Name = UI.ItemQuestMaxPopup,
			Icon = quest.GetPopupIcon(),
			IconScale = 1f,
			RepresentingObject = quest
		});
		BasicQuestBase.SetInventoryNewItem(quest);
	}

	// Token: 0x060033E6 RID: 13286 RVA: 0x000E7A8C File Offset: 0x000E5C8C
	public static void ShowQuestUpdatedStandalone(BasicQuestBase quest)
	{
		bool flag = false;
		bool flag2 = false;
		if (quest is MainQuest)
		{
			flag = true;
		}
		if (!flag)
		{
			SubQuest subQuest = quest as SubQuest;
			if (subQuest != null)
			{
				if (!subQuest.CanShowUpdated(true))
				{
					return;
				}
				flag2 = true;
			}
		}
		CollectableUIMsg collectableUIMsg = CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = ((flag || flag2) ? UI.MainQuestProgressPopup : UI.ItemQuestMaxPopup),
			Icon = quest.GetPopupIcon(),
			IconScale = 1f,
			RepresentingObject = quest
		}, null, false);
		if (collectableUIMsg)
		{
			collectableUIMsg.DoReplacingEffects();
		}
		BasicQuestBase.SetInventoryNewItem(quest);
	}

	// Token: 0x0400378F RID: 14223
	private static QuestManager instance;

	// Token: 0x04003790 RID: 14224
	[SerializeField]
	private QuestList masterList;

	// Token: 0x04003791 RID: 14225
	[Space]
	[SerializeField]
	private GameObject questAcceptedSequence;

	// Token: 0x04003792 RID: 14226
	[SerializeField]
	private GameObject questFinishedSequence;

	// Token: 0x04003793 RID: 14227
	[SerializeField]
	private EventRegister sequenceEndEvent;

	// Token: 0x04003794 RID: 14228
	private GameObject spawnedQuestAcceptedSequence;

	// Token: 0x04003795 RID: 14229
	private GameObject spawnedQuestFinishedSequence;

	// Token: 0x04003796 RID: 14230
	private BasicQuestBase updatedQuest;

	// Token: 0x04003798 RID: 14232
	private static ObjectCache<IEnumerable<BasicQuestBase>> _acceptedQuests;

	// Token: 0x04003799 RID: 14233
	private static ObjectCache<IEnumerable<FullQuestBase>> _activeQuests;

	// Token: 0x0400379A RID: 14234
	private static FullQuestBase[] _allFullQuests;

	// Token: 0x0400379B RID: 14235
	private static Dictionary<string, FullQuestBase> _fullQuestLookup;
}
