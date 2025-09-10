using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000590 RID: 1424
public abstract class FullQuestBase : BasicQuestBase, IQuestWithCompletion
{
	// Token: 0x17000574 RID: 1396
	// (get) Token: 0x060032E6 RID: 13030 RVA: 0x000E2301 File Offset: 0x000E0501
	public LocalisedString GiveNameOverride
	{
		get
		{
			return this.giveNameOverride;
		}
	}

	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x060032E7 RID: 13031 RVA: 0x000E2309 File Offset: 0x000E0509
	public LocalisedString InvItemAppendDesc
	{
		get
		{
			return this.invItemAppendDesc;
		}
	}

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x060032E8 RID: 13032 RVA: 0x000E2311 File Offset: 0x000E0511
	public SavedItem RewardItem
	{
		get
		{
			return this.rewardItem;
		}
	}

	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x060032E9 RID: 13033 RVA: 0x000E2319 File Offset: 0x000E0519
	public int RewardCount
	{
		get
		{
			return this.rewardCount;
		}
	}

	// Token: 0x17000578 RID: 1400
	// (get) Token: 0x060032EA RID: 13034 RVA: 0x000E2321 File Offset: 0x000E0521
	public Sprite RewardIcon
	{
		get
		{
			if (this.rewardIconType == FullQuestBase.IconTypes.None)
			{
				return null;
			}
			if (this.rewardIcon)
			{
				return this.rewardIcon;
			}
			if (!this.rewardItem)
			{
				return null;
			}
			return this.rewardItem.GetPopupIcon();
		}
	}

	// Token: 0x17000579 RID: 1401
	// (get) Token: 0x060032EB RID: 13035 RVA: 0x000E235C File Offset: 0x000E055C
	public FullQuestBase.IconTypes RewardIconType
	{
		get
		{
			return this.rewardIconType;
		}
	}

	// Token: 0x1700057A RID: 1402
	// (get) Token: 0x060032EC RID: 13036 RVA: 0x000E2364 File Offset: 0x000E0564
	public IReadOnlyList<FullQuestBase.QuestTarget> Targets
	{
		get
		{
			return this.targets ?? Array.Empty<FullQuestBase.QuestTarget>();
		}
	}

	// Token: 0x1700057B RID: 1403
	// (get) Token: 0x060032ED RID: 13037 RVA: 0x000E2375 File Offset: 0x000E0575
	public bool ConsumeTargetIfApplicable
	{
		get
		{
			return this.consumeTargetIfApplicable;
		}
	}

	// Token: 0x1700057C RID: 1404
	// (get) Token: 0x060032EE RID: 13038 RVA: 0x000E2380 File Offset: 0x000E0580
	public FullQuestBase.DescCounterTypes DescCounterType
	{
		get
		{
			if (this.hideCountersWhenCompletable && this.CanComplete)
			{
				return FullQuestBase.DescCounterTypes.None;
			}
			if (this.hideDescCounterForLangs != null)
			{
				LanguageCode languageCode = Language.CurrentLanguage();
				LanguageCode[] array = this.hideDescCounterForLangs;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == languageCode)
					{
						return FullQuestBase.DescCounterTypes.None;
					}
				}
			}
			return this.descCounterType;
		}
	}

	// Token: 0x1700057D RID: 1405
	// (get) Token: 0x060032EF RID: 13039 RVA: 0x000E23D0 File Offset: 0x000E05D0
	public bool IsDescCounterTypeCustom
	{
		get
		{
			return this.DescCounterType == FullQuestBase.DescCounterTypes.Custom;
		}
	}

	// Token: 0x1700057E RID: 1406
	// (get) Token: 0x060032F0 RID: 13040 RVA: 0x000E23DB File Offset: 0x000E05DB
	public GameObject CustomDescPrefab
	{
		get
		{
			return this.customDescPrefab;
		}
	}

	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x060032F1 RID: 13041 RVA: 0x000E23E3 File Offset: 0x000E05E3
	public Color ProgressBarTint
	{
		get
		{
			return this.progressBarTint;
		}
	}

	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x060032F2 RID: 13042 RVA: 0x000E23EB File Offset: 0x000E05EB
	public float CounterIconScale
	{
		get
		{
			return this.counterIconScale;
		}
	}

	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x060032F3 RID: 13043 RVA: 0x000E23F3 File Offset: 0x000E05F3
	public Vector2 CounterIconPadding
	{
		get
		{
			return this.counterIconPadding;
		}
	}

	// Token: 0x17000582 RID: 1410
	// (get) Token: 0x060032F4 RID: 13044 RVA: 0x000E23FB File Offset: 0x000E05FB
	public FullQuestBase.ListCounterTypes ListCounterType
	{
		get
		{
			if (!this.hideCountersWhenCompletable || !this.CanComplete)
			{
				return this.listCounterType;
			}
			return FullQuestBase.ListCounterTypes.None;
		}
	}

	// Token: 0x17000583 RID: 1411
	// (get) Token: 0x060032F5 RID: 13045 RVA: 0x000E2415 File Offset: 0x000E0615
	public bool HideMax
	{
		get
		{
			return this.hideMax;
		}
	}

	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x060032F6 RID: 13046 RVA: 0x000E241D File Offset: 0x000E061D
	public OverrideFloat OverrideFontSize
	{
		get
		{
			return this.overrideFontSize;
		}
	}

	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x060032F7 RID: 13047 RVA: 0x000E2428 File Offset: 0x000E0628
	public OverrideFloat OverrideParagraphSpacing
	{
		get
		{
			LanguageCode languageCode = Language.CurrentLanguage();
			OverrideFloat result;
			if (languageCode == LanguageCode.DE || languageCode == LanguageCode.FR)
			{
				result = this.overrideParagraphSpacingShort;
			}
			else
			{
				result = this.overrideParagraphSpacing;
			}
			return result;
		}
	}

	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x060032F8 RID: 13048 RVA: 0x000E2458 File Offset: 0x000E0658
	public override bool IsAvailable
	{
		get
		{
			foreach (FullQuestBase fullQuestBase in this.requiredCompleteQuests)
			{
				if (fullQuestBase && !fullQuestBase.IsCompleted)
				{
					return false;
				}
			}
			foreach (ToolItem toolItem in this.requiredUnlockedTools)
			{
				if (toolItem && !toolItem.IsUnlocked)
				{
					return false;
				}
			}
			foreach (QuestCompleteTotalGroup questCompleteTotalGroup in this.requiredCompleteTotalGroups)
			{
				if (questCompleteTotalGroup && !questCompleteTotalGroup.IsFulfilled)
				{
					return false;
				}
			}
			return this.playerDataTest.IsFulfilled;
		}
	}

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x060032F9 RID: 13049 RVA: 0x000E24FC File Offset: 0x000E06FC
	public IEnumerable<int> Counters
	{
		get
		{
			if (this.IsCompleted)
			{
				return from target in this.Targets
				select target.Count;
			}
			return this.Targets.Select(delegate(FullQuestBase.QuestTarget target)
			{
				if (target.AltTest.IsDefined && target.AltTest.IsFulfilled)
				{
					return target.Count;
				}
				if (!target.Counter)
				{
					return this.Completion.CompletedCount;
				}
				return target.Counter.GetCompletionAmount(this.Completion);
			});
		}
	}

	// Token: 0x17000588 RID: 1416
	// (get) Token: 0x060032FA RID: 13050 RVA: 0x000E2553 File Offset: 0x000E0753
	[TupleElementNames(new string[]
	{
		"target",
		"count"
	})]
	public IEnumerable<ValueTuple<FullQuestBase.QuestTarget, int>> TargetsAndCountersNotHidden
	{
		[return: TupleElementNames(new string[]
		{
			"target",
			"count"
		})]
		get
		{
			return this.Targets.Zip(this.Counters, (FullQuestBase.QuestTarget target, int count) => new ValueTuple<FullQuestBase.QuestTarget, int>(target, count));
		}
	}

	// Token: 0x17000589 RID: 1417
	// (get) Token: 0x060032FB RID: 13051 RVA: 0x000E2585 File Offset: 0x000E0785
	[TupleElementNames(new string[]
	{
		"target",
		"count"
	})]
	public IEnumerable<ValueTuple<FullQuestBase.QuestTarget, int>> TargetsAndCounters
	{
		[return: TupleElementNames(new string[]
		{
			"target",
			"count"
		})]
		get
		{
			return from target in this.TargetsAndCountersNotHidden
			where !target.Item1.HideInCount
			select target;
		}
	}

	// Token: 0x1700058A RID: 1418
	// (get) Token: 0x060032FC RID: 13052 RVA: 0x000E25B1 File Offset: 0x000E07B1
	public bool IsDonateType
	{
		get
		{
			return this.QuestType && this.QuestType.IsDonateType;
		}
	}

	// Token: 0x1700058B RID: 1419
	// (get) Token: 0x060032FD RID: 13053 RVA: 0x000E25CD File Offset: 0x000E07CD
	public virtual bool CanComplete
	{
		get
		{
			return this.TargetsAndCounters.All(([TupleElementNames(new string[]
			{
				"target",
				"count"
			})] ValueTuple<FullQuestBase.QuestTarget, int> pair) => pair.Item2 >= pair.Item1.Count);
		}
	}

	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x060032FE RID: 13054 RVA: 0x000E25F9 File Offset: 0x000E07F9
	public override bool IsAccepted
	{
		get
		{
			return this.Completion.IsAccepted;
		}
	}

	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x060032FF RID: 13055 RVA: 0x000E2606 File Offset: 0x000E0806
	public bool IsCompleted
	{
		get
		{
			return this.Completion.IsCompleted;
		}
	}

	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x06003300 RID: 13056 RVA: 0x000E2613 File Offset: 0x000E0813
	public bool WasEverCompleted
	{
		get
		{
			return this.Completion.WasEverCompleted;
		}
	}

	// Token: 0x1700058F RID: 1423
	// (get) Token: 0x06003301 RID: 13057 RVA: 0x000E2620 File Offset: 0x000E0820
	// (set) Token: 0x06003302 RID: 13058 RVA: 0x000E2630 File Offset: 0x000E0830
	public override bool HasBeenSeen
	{
		get
		{
			return this.Completion.HasBeenSeen;
		}
		set
		{
			QuestCompletionData.Completion completion = this.Completion;
			completion.HasBeenSeen = value;
			this.Completion = completion;
		}
	}

	// Token: 0x17000590 RID: 1424
	// (get) Token: 0x06003303 RID: 13059 RVA: 0x000E2654 File Offset: 0x000E0854
	public override bool IsHidden
	{
		get
		{
			if (this.nextQuestStep && this.nextQuestStep.IsAccepted && this.nextQuestStep.DisplayName == base.DisplayName)
			{
				return true;
			}
			foreach (FullQuestBase fullQuestBase in this.hideIfComplete)
			{
				if (fullQuestBase && fullQuestBase.IsCompleted)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x17000591 RID: 1425
	// (get) Token: 0x06003304 RID: 13060 RVA: 0x000E26CB File Offset: 0x000E08CB
	public override bool IsMapMarkerVisible
	{
		get
		{
			return this.IsAccepted && !this.IsCompleted && !this.IsHidden;
		}
	}

	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x06003305 RID: 13061 RVA: 0x000E26E8 File Offset: 0x000E08E8
	// (set) Token: 0x06003306 RID: 13062 RVA: 0x000E2704 File Offset: 0x000E0904
	private QuestCompletionData.Completion Completion
	{
		get
		{
			return GameManager.instance.playerData.QuestCompletionData.GetData(base.name);
		}
		set
		{
			GameManager.instance.playerData.QuestCompletionData.SetData(base.name, value);
			QuestManager.IncrementVersion();
		}
	}

	// Token: 0x06003307 RID: 13063 RVA: 0x000E2726 File Offset: 0x000E0926
	private bool ShowCustomPickupDisplay()
	{
		return this.Targets.Any((FullQuestBase.QuestTarget target) => target.Counter == null && target.Count > 0);
	}

	// Token: 0x06003308 RID: 13064 RVA: 0x000E2752 File Offset: 0x000E0952
	private bool ShowCounterIconOverride()
	{
		return this.HasCounterIcon() && this.descCounterType == FullQuestBase.DescCounterTypes.Icons;
	}

	// Token: 0x06003309 RID: 13065 RVA: 0x000E2768 File Offset: 0x000E0968
	private bool HasCounterIcon()
	{
		FullQuestBase.DescCounterTypes descCounterTypes = this.descCounterType;
		return descCounterTypes == FullQuestBase.DescCounterTypes.Icons || descCounterTypes == FullQuestBase.DescCounterTypes.Text;
	}

	// Token: 0x0600330A RID: 13066 RVA: 0x000E278C File Offset: 0x000E098C
	private bool? RewardIconValidation()
	{
		if (this.rewardIconType == FullQuestBase.IconTypes.None)
		{
			return null;
		}
		if (this.rewardItem && this.RewardIcon && this.rewardIcon == null)
		{
			return null;
		}
		return new bool?(this.rewardIcon);
	}

	// Token: 0x0600330B RID: 13067 RVA: 0x000E27EE File Offset: 0x000E09EE
	private bool ShowRewardCount()
	{
		return this.rewardItem != null && this.rewardItem.CanGetMultipleAtOnce;
	}

	// Token: 0x0600330C RID: 13068 RVA: 0x000E280B File Offset: 0x000E0A0B
	private bool ShowTurnInAtBoard()
	{
		FullQuestBase.QuestTarget[] array = this.targets;
		return ((array != null) ? array.Length : 0) > 0;
	}

	// Token: 0x0600330D RID: 13069 RVA: 0x000E2820 File Offset: 0x000E0A20
	private void OnValidate()
	{
		if (this.oldPreviousQuestStep && this.oldPreviousQuestStep.nextQuestStep == this)
		{
			this.oldPreviousQuestStep.nextQuestStep = null;
		}
		if (this.previousQuestStep)
		{
			this.previousQuestStep.nextQuestStep = this;
		}
		if (this.rewardCount <= 0)
		{
			this.rewardCount = 1;
		}
		this.oldPreviousQuestStep = this.previousQuestStep;
		if (this.targetCounter != null || this.targetCount > 0)
		{
			this.targets = new FullQuestBase.QuestTarget[]
			{
				new FullQuestBase.QuestTarget
				{
					Counter = this.targetCounter,
					Count = this.targetCount
				}
			};
			this.targetCounter = null;
			this.targetCount = 0;
		}
		if (this.descAppendItemList)
		{
			this.descAppendItemList = false;
			this.descAppendBehaviour = FullQuestBase.AppendDescBehaviour.Append;
		}
	}

	// Token: 0x0600330E RID: 13070 RVA: 0x000E28FF File Offset: 0x000E0AFF
	protected override void DoInit()
	{
		this.OnValidate();
	}

	// Token: 0x0600330F RID: 13071 RVA: 0x000E2907 File Offset: 0x000E0B07
	protected virtual void OnEnable()
	{
		base.Init();
		QuestTargetCounter.OnIncrement += this.IncrementCounterHandler;
	}

	// Token: 0x06003310 RID: 13072 RVA: 0x000E2920 File Offset: 0x000E0B20
	protected virtual void OnDisable()
	{
		QuestTargetCounter.OnIncrement -= this.IncrementCounterHandler;
	}

	// Token: 0x06003311 RID: 13073 RVA: 0x000E2934 File Offset: 0x000E0B34
	public Sprite GetCounterSpriteOverride(FullQuestBase.QuestTarget forTarget, int index)
	{
		if (this.counterIconOverride)
		{
			return this.counterIconOverride;
		}
		if (forTarget.AltSprite)
		{
			return forTarget.AltSprite;
		}
		if (!forTarget.Counter)
		{
			return this.customPickupDisplay.Icon;
		}
		return forTarget.Counter.GetQuestCounterSprite(index);
	}

	// Token: 0x06003312 RID: 13074 RVA: 0x000E2990 File Offset: 0x000E0B90
	public override string GetDescription(BasicQuestBase.ReadSource readSource)
	{
		if (readSource != BasicQuestBase.ReadSource.Inventory)
		{
			if (readSource != BasicQuestBase.ReadSource.QuestBoard)
			{
				throw new ArgumentOutOfRangeException("readSource", readSource, null);
			}
			return this.wallDescription;
		}
		else if (this.IsCompleted)
		{
			if (this.inventoryCompletedDescription.IsEmpty)
			{
				return this.<GetDescription>g__MaybeAppendItemList|126_0(this.inventoryDescription);
			}
			return this.inventoryCompletedDescription;
		}
		else
		{
			if (!this.inventoryCompletableDescription.IsEmpty && this.CanComplete)
			{
				return this.inventoryCompletableDescription;
			}
			return this.<GetDescription>g__MaybeAppendItemList|126_0(this.inventoryDescription);
		}
	}

	// Token: 0x06003313 RID: 13075 RVA: 0x000E2A2C File Offset: 0x000E0C2C
	public void IncrementCounterHandler(QuestTargetCounter counter)
	{
		bool flag = false;
		foreach (FullQuestBase.QuestTarget questTarget in this.targets)
		{
			if (questTarget.Counter && questTarget.Counter.ShouldIncrementQuestCounter(counter))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		if (this.IsAccepted && !this.IsCompleted)
		{
			this.IncrementQuestCounter();
		}
	}

	// Token: 0x06003314 RID: 13076 RVA: 0x000E2A94 File Offset: 0x000E0C94
	public void BeginQuest(Action afterPrompt, bool showPrompt = true)
	{
		if (!QuestManager.IsQuestInList(this))
		{
			return;
		}
		bool hasPreviousQuest = this.previousQuestStep && this.previousQuestStep.IsAccepted;
		this.SilentlyCompletePrevious();
		QuestCompletionData.Completion completion = this.Completion;
		completion.IsAccepted = true;
		if (completion.IsCompleted && !completion.WasEverCompleted)
		{
			completion.WasEverCompleted = true;
		}
		completion.IsCompleted = false;
		completion.HasBeenSeen = false;
		this.Completion = completion;
		BasicQuestBase.SetInventoryNewItem(this);
		if (showPrompt)
		{
			this.ShowQuestAccepted(afterPrompt, hasPreviousQuest);
			return;
		}
		if (afterPrompt != null)
		{
			afterPrompt();
		}
	}

	// Token: 0x06003315 RID: 13077 RVA: 0x000E2B28 File Offset: 0x000E0D28
	public void SilentlyCompletePrevious()
	{
		if (this.previousQuestStep)
		{
			this.previousQuestStep.SilentlyComplete();
		}
		if (this.markCompleted != null)
		{
			foreach (FullQuestBase fullQuestBase in this.markCompleted)
			{
				if (fullQuestBase)
				{
					fullQuestBase.SilentlyComplete();
				}
			}
		}
		if (this.cancelIfIncomplete != null)
		{
			foreach (FullQuestBase fullQuestBase2 in this.cancelIfIncomplete)
			{
				if (fullQuestBase2 && !fullQuestBase2.IsCompleted)
				{
					QuestCompletionData.Completion completion = fullQuestBase2.Completion;
					completion.IsAccepted = false;
					fullQuestBase2.Completion = completion;
				}
			}
		}
	}

	// Token: 0x06003316 RID: 13078 RVA: 0x000E2BC8 File Offset: 0x000E0DC8
	public void SilentlyComplete()
	{
		QuestCompletionData.Completion completion = this.Completion;
		if (completion.IsCompleted)
		{
			return;
		}
		completion.IsAccepted = true;
		completion.SetCompleted();
		this.Completion = completion;
		this.SilentlyCompletePrevious();
	}

	// Token: 0x06003317 RID: 13079 RVA: 0x000E2C04 File Offset: 0x000E0E04
	protected virtual void ShowQuestAccepted(Action afterPrompt, bool hasPreviousQuest)
	{
		if (hasPreviousQuest)
		{
			CollectableUIMsg.Spawn(new FullQuestBase.UIMsgDisplay
			{
				Name = UI.QuestContinuePopup,
				Icon = this.QuestType.Icon,
				IconScale = 1f,
				RepresentingObject = this
			}, null, true);
			UI.QuestContinuePopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero, null);
			if (afterPrompt != null)
			{
				afterPrompt();
				return;
			}
		}
		else
		{
			QuestManager.ShowQuestAccepted(this, afterPrompt);
		}
	}

	// Token: 0x06003318 RID: 13080 RVA: 0x000E2C86 File Offset: 0x000E0E86
	public bool ConsumeTarget()
	{
		return this.consumeTargetIfApplicable && this.ConsumeTargets();
	}

	// Token: 0x06003319 RID: 13081 RVA: 0x000E2C98 File Offset: 0x000E0E98
	protected virtual bool ConsumeTargets()
	{
		bool result = false;
		foreach (FullQuestBase.QuestTarget questTarget in this.targets)
		{
			if (questTarget.Counter && questTarget.Counter.CanConsume)
			{
				questTarget.Counter.Consume(questTarget.Count, true);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600331A RID: 13082 RVA: 0x000E2CF4 File Offset: 0x000E0EF4
	public bool TryEndQuest(Action afterPrompt, bool consumeCurrency, bool forceEnd = false, bool showPrompt = true)
	{
		QuestCompletionData.Completion completion = this.Completion;
		if (!forceEnd && !this.CanComplete)
		{
			if (afterPrompt != null)
			{
				afterPrompt();
			}
			return false;
		}
		FullQuestBase fullQuestBase = this.previousQuestStep;
		while (fullQuestBase)
		{
			fullQuestBase.SilentlyComplete();
			fullQuestBase = fullQuestBase.previousQuestStep;
		}
		if (this.QuestType.IsDonateType)
		{
			completion.IsAccepted = true;
		}
		completion.SetCompleted();
		this.Completion = completion;
		if (consumeCurrency)
		{
			this.ConsumeTarget();
		}
		InventoryPaneList.SetNextOpen("Quests");
		QuestManager.UpdatedQuest = this;
		GameManager instance = GameManager.instance;
		if (showPrompt && completion.IsAccepted)
		{
			if (!(this is MainQuest))
			{
				instance.QueueAchievement("FIRST_WISH");
			}
			if (!string.IsNullOrWhiteSpace(this.awardAchievementOnComplete))
			{
				instance.QueueAchievement(this.awardAchievementOnComplete);
			}
			MateriumItemManager.CheckAchievements(true);
			this.ShowQuestCompleted(afterPrompt);
		}
		else
		{
			if (afterPrompt != null)
			{
				afterPrompt();
			}
			if (!string.IsNullOrWhiteSpace(this.awardAchievementOnComplete))
			{
				instance.AwardAchievement(this.awardAchievementOnComplete);
			}
			MateriumItemManager.CheckAchievements();
			if (!(this is MainQuest))
			{
				instance.AwardAchievement("FIRST_WISH");
			}
		}
		instance.QueueSaveGame();
		if (this.QuestType)
		{
			this.QuestType.OnQuestCompleted(this);
		}
		return true;
	}

	// Token: 0x0600331B RID: 13083 RVA: 0x000E2E20 File Offset: 0x000E1020
	protected virtual void ShowQuestCompleted(Action afterPrompt)
	{
		QuestManager.ShowQuestCompleted(this, afterPrompt);
	}

	// Token: 0x0600331C RID: 13084 RVA: 0x000E2E2C File Offset: 0x000E102C
	public void IncrementQuestCounter()
	{
		QuestCompletionData.Completion completion = this.Completion;
		completion.CompletedCount++;
		this.Completion = completion;
	}

	// Token: 0x0600331D RID: 13085 RVA: 0x000E2E54 File Offset: 0x000E1054
	public override void Get(bool showPopup = true)
	{
		foreach (FullQuestBase.QuestTarget questTarget in this.targets)
		{
			if (questTarget.Counter)
			{
				questTarget.Counter.Get(showPopup);
			}
		}
		this.IncrementQuestCounter();
		this.customPickupDisplay.RepresentingObject = this;
		if (!showPopup || !this.ShowCustomPickupDisplay())
		{
			return;
		}
		CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(this.customPickupDisplay, this.CanComplete ? UI.MaxItemsTextColor : Color.white, null, false);
		if (this.CanComplete)
		{
			QuestManager.ShowQuestUpdatedForItemMsg(itemUiMsg, this);
		}
	}

	// Token: 0x0600331E RID: 13086 RVA: 0x000E2EEC File Offset: 0x000E10EC
	public override bool CanGetMore()
	{
		if (!this.IsAccepted || this.IsCompleted)
		{
			return false;
		}
		if (!this.getTargetCondition.IsFulfilled)
		{
			return false;
		}
		bool flag = false;
		foreach (FullQuestBase.QuestTarget questTarget in this.targets)
		{
			if (!questTarget.Counter)
			{
				if (this.Completion.CompletedCount >= questTarget.Count)
				{
					return false;
				}
			}
			else
			{
				if (questTarget.Counter.CanGetMore())
				{
					return true;
				}
				flag = true;
			}
		}
		return !flag;
	}

	// Token: 0x0600331F RID: 13087 RVA: 0x000E2F70 File Offset: 0x000E1170
	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		yield return this;
		yield break;
	}

	// Token: 0x06003320 RID: 13088 RVA: 0x000E2F80 File Offset: 0x000E1180
	public bool GetIsReadyToTurnIn(bool atQuestBoard)
	{
		return !this.nextQuestStep && (!atQuestBoard || (this.canTurnInAtBoard && this.ShowTurnInAtBoard())) && this.IsAccepted && !this.IsCompleted && this.CanComplete;
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x000E2FBE File Offset: 0x000E11BE
	public int GetCollectedCountOverride(FullQuestBase.QuestTarget target, int baseCount)
	{
		return Mathf.Clamp(baseCount, 0, target.Count);
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x000E3054 File Offset: 0x000E1254
	[CompilerGenerated]
	private string <GetDescription>g__MaybeAppendItemList|126_0(string desc)
	{
		StringBuilder tempStringBuilder;
		bool flag;
		bool flag2;
		switch (this.descAppendBehaviour)
		{
		case FullQuestBase.AppendDescBehaviour.None:
			return desc;
		case FullQuestBase.AppendDescBehaviour.Append:
			tempStringBuilder = global::Helper.GetTempStringBuilder(desc);
			flag = false;
			flag2 = false;
			break;
		case FullQuestBase.AppendDescBehaviour.Prepend:
			tempStringBuilder = global::Helper.GetTempStringBuilder();
			flag = true;
			flag2 = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		FullQuestBase.AppendDescFormat appendDescFormat = this.descAppendFormat;
		string text;
		if (appendDescFormat != FullQuestBase.AppendDescFormat.None)
		{
			if (appendDescFormat != FullQuestBase.AppendDescFormat.Bullet)
			{
				throw new ArgumentOutOfRangeException();
			}
			text = "• ";
		}
		else
		{
			text = string.Empty;
		}
		string text2 = text;
		string value = "<alpha=#55>" + text2 + "<s>";
		bool isCompleted = this.IsCompleted;
		foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple in this.TargetsAndCounters)
		{
			FullQuestBase.QuestTarget item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			if (flag2)
			{
				flag2 = false;
			}
			else
			{
				tempStringBuilder.AppendLine();
			}
			LocalisedString itemName = item.ItemName;
			string value2;
			if (!itemName.IsEmpty)
			{
				value2 = item.ItemName;
			}
			else
			{
				if (!item.Counter)
				{
					continue;
				}
				value2 = item.Counter.GetPopupName();
			}
			if (!isCompleted && item2 >= item.Count)
			{
				tempStringBuilder.Append(value);
				tempStringBuilder.Append(value2);
				tempStringBuilder.Append("</s><alpha=#FF>");
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(text2))
				{
					tempStringBuilder.Append(text2);
				}
				tempStringBuilder.Append(value2);
			}
		}
		if (flag)
		{
			tempStringBuilder.AppendLine();
			tempStringBuilder.AppendLine(desc);
		}
		return tempStringBuilder.ToString();
	}

	// Token: 0x040036C7 RID: 14023
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private int targetCount;

	// Token: 0x040036C8 RID: 14024
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private QuestTargetCounter targetCounter;

	// Token: 0x040036C9 RID: 14025
	[Header("- Full Quest Base")]
	[SerializeField]
	private FullQuestBase.QuestTarget[] targets;

	// Token: 0x040036CA RID: 14026
	[SerializeField]
	private bool consumeTargetIfApplicable;

	// Token: 0x040036CB RID: 14027
	[SerializeField]
	private PlayerDataTest getTargetCondition;

	// Token: 0x040036CC RID: 14028
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowTurnInAtBoard", true, true, false)]
	private bool canTurnInAtBoard = true;

	// Token: 0x040036CD RID: 14029
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString giveNameOverride;

	// Token: 0x040036CE RID: 14030
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString invItemAppendDesc;

	// Token: 0x040036CF RID: 14031
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowCustomPickupDisplay", true, true, false)]
	private FullQuestBase.UIMsgDisplay customPickupDisplay;

	// Token: 0x040036D0 RID: 14032
	[Space]
	[SerializeField]
	private SavedItem rewardItem;

	// Token: 0x040036D1 RID: 14033
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowRewardCount", true, true, false)]
	private int rewardCount;

	// Token: 0x040036D2 RID: 14034
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("RewardIconValidation")]
	private Sprite rewardIcon;

	// Token: 0x040036D3 RID: 14035
	[SerializeField]
	private FullQuestBase.IconTypes rewardIconType;

	// Token: 0x040036D4 RID: 14036
	[Space]
	[SerializeField]
	private string awardAchievementOnComplete;

	// Token: 0x040036D5 RID: 14037
	[Header("Inventory")]
	[SerializeField]
	private LocalisedString inventoryDescription;

	// Token: 0x040036D6 RID: 14038
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private bool descAppendItemList;

	// Token: 0x040036D7 RID: 14039
	[SerializeField]
	private FullQuestBase.AppendDescBehaviour descAppendBehaviour;

	// Token: 0x040036D8 RID: 14040
	[SerializeField]
	private FullQuestBase.AppendDescFormat descAppendFormat = FullQuestBase.AppendDescFormat.Bullet;

	// Token: 0x040036D9 RID: 14041
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString inventoryCompletableDescription;

	// Token: 0x040036DA RID: 14042
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString inventoryCompletedDescription;

	// Token: 0x040036DB RID: 14043
	[Space]
	[SerializeField]
	[FormerlySerializedAs("counterDisplayType")]
	private FullQuestBase.DescCounterTypes descCounterType;

	// Token: 0x040036DC RID: 14044
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsDescCounterTypeCustom", true, false, false)]
	private GameObject customDescPrefab;

	// Token: 0x040036DD RID: 14045
	[SerializeField]
	private Color progressBarTint = Color.white;

	// Token: 0x040036DE RID: 14046
	[SerializeField]
	private FullQuestBase.ListCounterTypes listCounterType;

	// Token: 0x040036DF RID: 14047
	[SerializeField]
	private bool hideMax;

	// Token: 0x040036E0 RID: 14048
	[SerializeField]
	private bool hideCountersWhenCompletable;

	// Token: 0x040036E1 RID: 14049
	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowCounterIconOverride", true, true, false)]
	private Sprite counterIconOverride;

	// Token: 0x040036E2 RID: 14050
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasCounterIcon", true, true, false)]
	private float counterIconScale = 1f;

	// Token: 0x040036E3 RID: 14051
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasCounterIcon", true, true, false)]
	private Vector2 counterIconPadding;

	// Token: 0x040036E4 RID: 14052
	[Space]
	[SerializeField]
	private OverrideFloat overrideFontSize;

	// Token: 0x040036E5 RID: 14053
	[SerializeField]
	private OverrideFloat overrideParagraphSpacing;

	// Token: 0x040036E6 RID: 14054
	[SerializeField]
	private OverrideFloat overrideParagraphSpacingShort;

	// Token: 0x040036E7 RID: 14055
	[SerializeField]
	private LanguageCode[] hideDescCounterForLangs;

	// Token: 0x040036E8 RID: 14056
	[Header("Quest Board")]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString wallDescription;

	// Token: 0x040036E9 RID: 14057
	[Space]
	[SerializeField]
	private PlayerDataTest playerDataTest;

	// Token: 0x040036EA RID: 14058
	[SerializeField]
	private FullQuestBase[] requiredCompleteQuests;

	// Token: 0x040036EB RID: 14059
	[SerializeField]
	private ToolItem[] requiredUnlockedTools;

	// Token: 0x040036EC RID: 14060
	[SerializeField]
	private QuestCompleteTotalGroup[] requiredCompleteTotalGroups;

	// Token: 0x040036ED RID: 14061
	[Header("Progress")]
	[SerializeField]
	[Tooltip("Previous step will be hidden when this quest is shown.")]
	private FullQuestBase previousQuestStep;

	// Token: 0x040036EE RID: 14062
	[NonSerialized]
	private FullQuestBase oldPreviousQuestStep;

	// Token: 0x040036EF RID: 14063
	[NonSerialized]
	private FullQuestBase nextQuestStep;

	// Token: 0x040036F0 RID: 14064
	[SerializeField]
	[Tooltip("Will be silently marked as completed when this quest begins, but will not be hidden.")]
	private FullQuestBase[] markCompleted;

	// Token: 0x040036F1 RID: 14065
	[SerializeField]
	private FullQuestBase[] cancelIfIncomplete;

	// Token: 0x040036F2 RID: 14066
	[Space]
	[SerializeField]
	[Tooltip("Will silently hide this quest if any of these are complete, without marking as completed.")]
	private FullQuestBase[] hideIfComplete;

	// Token: 0x020018A5 RID: 6309
	public enum DescCounterTypes
	{
		// Token: 0x040092DE RID: 37598
		Icons,
		// Token: 0x040092DF RID: 37599
		Text,
		// Token: 0x040092E0 RID: 37600
		ProgressBar,
		// Token: 0x040092E1 RID: 37601
		None,
		// Token: 0x040092E2 RID: 37602
		Custom
	}

	// Token: 0x020018A6 RID: 6310
	public enum ListCounterTypes
	{
		// Token: 0x040092E4 RID: 37604
		Dots,
		// Token: 0x040092E5 RID: 37605
		Bar,
		// Token: 0x040092E6 RID: 37606
		None
	}

	// Token: 0x020018A7 RID: 6311
	public enum IconTypes
	{
		// Token: 0x040092E8 RID: 37608
		None = -1,
		// Token: 0x040092E9 RID: 37609
		Image,
		// Token: 0x040092EA RID: 37610
		Silhouette
	}

	// Token: 0x020018A8 RID: 6312
	private enum AppendDescBehaviour
	{
		// Token: 0x040092EC RID: 37612
		None,
		// Token: 0x040092ED RID: 37613
		Append,
		// Token: 0x040092EE RID: 37614
		Prepend
	}

	// Token: 0x020018A9 RID: 6313
	private enum AppendDescFormat
	{
		// Token: 0x040092F0 RID: 37616
		None,
		// Token: 0x040092F1 RID: 37617
		Bullet
	}

	// Token: 0x020018AA RID: 6314
	[Serializable]
	private struct UIMsgDisplay : ICollectableUIMsgItem, IUIMsgPopupItem
	{
		// Token: 0x060091F2 RID: 37362 RVA: 0x0029AFF7 File Offset: 0x002991F7
		public float GetUIMsgIconScale()
		{
			return this.IconScale;
		}

		// Token: 0x060091F3 RID: 37363 RVA: 0x0029AFFF File Offset: 0x002991FF
		public bool HasUpgradeIcon()
		{
			return false;
		}

		// Token: 0x060091F4 RID: 37364 RVA: 0x0029B002 File Offset: 0x00299202
		public string GetUIMsgName()
		{
			return this.Name;
		}

		// Token: 0x060091F5 RID: 37365 RVA: 0x0029B00F File Offset: 0x0029920F
		public Sprite GetUIMsgSprite()
		{
			return this.Icon;
		}

		// Token: 0x060091F6 RID: 37366 RVA: 0x0029B017 File Offset: 0x00299217
		public Object GetRepresentingObject()
		{
			return this.RepresentingObject;
		}

		// Token: 0x040092F2 RID: 37618
		public LocalisedString Name;

		// Token: 0x040092F3 RID: 37619
		public Sprite Icon;

		// Token: 0x040092F4 RID: 37620
		public float IconScale;

		// Token: 0x040092F5 RID: 37621
		[NonSerialized]
		public Object RepresentingObject;
	}

	// Token: 0x020018AB RID: 6315
	[Serializable]
	public struct QuestTarget
	{
		// Token: 0x040092F6 RID: 37622
		public QuestTargetCounter Counter;

		// Token: 0x040092F7 RID: 37623
		public int Count;

		// Token: 0x040092F8 RID: 37624
		public PlayerDataTest AltTest;

		// Token: 0x040092F9 RID: 37625
		public Sprite AltSprite;

		// Token: 0x040092FA RID: 37626
		[LocalisedString.NotRequiredAttribute]
		[ModifiableProperty]
		[Conditional("Counter", false, false, false)]
		public LocalisedString ItemName;

		// Token: 0x040092FB RID: 37627
		public bool HideInCount;
	}
}
