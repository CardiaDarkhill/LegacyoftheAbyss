using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000592 RID: 1426
public class InventoryItemQuest : InventoryItemUpdateable
{
	// Token: 0x140000A0 RID: 160
	// (add) Token: 0x0600332F RID: 13103 RVA: 0x000E370C File Offset: 0x000E190C
	// (remove) Token: 0x06003330 RID: 13104 RVA: 0x000E3744 File Offset: 0x000E1944
	public event Action<BasicQuestBase> Submitted;

	// Token: 0x140000A1 RID: 161
	// (add) Token: 0x06003331 RID: 13105 RVA: 0x000E377C File Offset: 0x000E197C
	// (remove) Token: 0x06003332 RID: 13106 RVA: 0x000E37B4 File Offset: 0x000E19B4
	public event Action Canceled;

	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x06003333 RID: 13107 RVA: 0x000E37E9 File Offset: 0x000E19E9
	public override string DisplayName
	{
		get
		{
			if (!this.quest)
			{
				return string.Empty;
			}
			return this.quest.DisplayName;
		}
	}

	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x06003334 RID: 13108 RVA: 0x000E3810 File Offset: 0x000E1A10
	public override string Description
	{
		get
		{
			if (!this.quest)
			{
				return string.Empty;
			}
			InventoryItemQuest.DescriptionTypes descriptionTypes = this.descriptionType;
			string description;
			if (descriptionTypes != InventoryItemQuest.DescriptionTypes.Inventory)
			{
				if (descriptionTypes != InventoryItemQuest.DescriptionTypes.QuestBoard)
				{
					throw new ArgumentOutOfRangeException();
				}
				description = this.quest.GetDescription(BasicQuestBase.ReadSource.QuestBoard);
			}
			else
			{
				description = this.quest.GetDescription(BasicQuestBase.ReadSource.Inventory);
			}
			return description;
		}
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x06003335 RID: 13109 RVA: 0x000E3865 File Offset: 0x000E1A65
	public override Color? CursorColor
	{
		get
		{
			if (this.quest && this.quest.QuestType)
			{
				return new Color?(this.quest.QuestType.TextColor);
			}
			return base.CursorColor;
		}
	}

	// Token: 0x17000596 RID: 1430
	// (get) Token: 0x06003336 RID: 13110 RVA: 0x000E38A2 File Offset: 0x000E1AA2
	public BasicQuestBase Quest
	{
		get
		{
			return this.quest;
		}
	}

	// Token: 0x17000597 RID: 1431
	// (get) Token: 0x06003337 RID: 13111 RVA: 0x000E38AA File Offset: 0x000E1AAA
	// (set) Token: 0x06003338 RID: 13112 RVA: 0x000E38B7 File Offset: 0x000E1AB7
	protected override bool IsSeen
	{
		get
		{
			return this.quest.HasBeenSeen;
		}
		set
		{
			this.quest.HasBeenSeen = value;
		}
	}

	// Token: 0x06003339 RID: 13113 RVA: 0x000E38C5 File Offset: 0x000E1AC5
	protected override void Awake()
	{
		base.Awake();
		this.extraDesc = base.GetComponent<InventoryItemExtraDescription>();
		this.Manager = base.GetComponentInParent<QuestItemManager>();
	}

	// Token: 0x0600333A RID: 13114 RVA: 0x000E38E5 File Offset: 0x000E1AE5
	protected override void Start()
	{
		base.Start();
		if (this.quest)
		{
			this.SetQuest(this.quest, this.wasInCompletedSection);
		}
	}

	// Token: 0x0600333B RID: 13115 RVA: 0x000E390C File Offset: 0x000E1B0C
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.updateQueued)
		{
			this.updateQueued = false;
			if (Application.isPlaying)
			{
				base.StartCoroutine(this.RefreshRoutine());
			}
		}
	}

	// Token: 0x0600333C RID: 13116 RVA: 0x000E3937 File Offset: 0x000E1B37
	protected IEnumerator RefreshRoutine()
	{
		yield return null;
		if (this.typeText)
		{
			this.typeText.gameObject.SetActive(false);
			this.typeText.gameObject.SetActive(true);
		}
		if (this.nameText)
		{
			this.nameText.gameObject.SetActive(false);
			this.nameText.gameObject.SetActive(true);
		}
		yield break;
	}

	// Token: 0x0600333D RID: 13117 RVA: 0x000E3946 File Offset: 0x000E1B46
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.hasBlockedActions && this.Manager)
		{
			this.Manager.IsActionsBlocked = false;
			this.hasBlockedActions = false;
		}
	}

	// Token: 0x0600333E RID: 13118 RVA: 0x000E3978 File Offset: 0x000E1B78
	public virtual void SetQuest(BasicQuestBase newQuest, bool isInCompletedSection)
	{
		this.quest = newQuest;
		this.wasInCompletedSection = isInCompletedSection;
		if (!this.quest)
		{
			if (this.group)
			{
				this.group.AlphaSelf = 0f;
			}
			if (this.extraDesc)
			{
				this.extraDesc.ExtraDescPrefab = null;
			}
			return;
		}
		if (this.quest.QuestType)
		{
			if (this.icon)
			{
				this.icon.sprite = this.quest.QuestType.Icon;
			}
			if (this.typeText)
			{
				this.typeText.text = this.quest.QuestType.DisplayName;
				this.typeText.color = this.quest.QuestType.TextColor;
			}
		}
		if (this.nameText)
		{
			this.nameText.text = this.quest.DisplayName;
		}
		if (this.consumeEffect)
		{
			this.consumeEffect.gameObject.SetActive(false);
		}
		if (this.iconCounter)
		{
			this.iconCounter.gameObject.SetActive(false);
		}
		if (this.progressBar)
		{
			this.progressBar.gameObject.SetActive(false);
		}
		if (this.progressBarSegmented)
		{
			this.progressBarSegmented.gameObject.SetActive(false);
		}
		if (this.group)
		{
			this.group.AlphaSelf = 1f;
		}
		if (this.iconGroup)
		{
			this.iconGroup.AlphaSelf = 1f;
		}
		IQuestWithCompletion questWithCompletion = this.quest as IQuestWithCompletion;
		FullQuestBase fullQuestBase = this.quest as FullQuestBase;
		bool flag = questWithCompletion != null && questWithCompletion.CanComplete;
		if (fullQuestBase != null && !isInCompletedSection)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple in fullQuestBase.TargetsAndCounters)
			{
				FullQuestBase.QuestTarget item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				num += Mathf.Clamp(item2, 0, item.Count);
				num2 += item.Count;
				num3++;
			}
			if (num3 > 0)
			{
				switch (fullQuestBase.ListCounterType)
				{
				case FullQuestBase.ListCounterTypes.Dots:
					if (this.iconCounter)
					{
						this.iconCounter.gameObject.SetActive(true);
						this.iconCounter.MaxValue = (fullQuestBase.HideMax ? num : num2);
						this.iconCounter.CurrentValue = num;
						this.iconCounter.SetColor(fullQuestBase.ProgressBarTint);
					}
					break;
				case FullQuestBase.ListCounterTypes.Bar:
					if (num3 > 1 && this.progressBarSegmented)
					{
						this.progressBarSegmented.gameObject.SetActive(true);
						this.progressBarSegmented.SetSegmentCount(num3);
						int num4 = 0;
						using (IEnumerator<ValueTuple<FullQuestBase.QuestTarget, int>> enumerator = fullQuestBase.TargetsAndCounters.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ValueTuple<FullQuestBase.QuestTarget, int> valueTuple2 = enumerator.Current;
								FullQuestBase.QuestTarget item3 = valueTuple2.Item1;
								float progress = (float)valueTuple2.Item2 / (float)item3.Count;
								this.progressBarSegmented.SetSegmentProgress(num4, progress);
								num4++;
							}
							break;
						}
					}
					if (this.progressBar)
					{
						this.progressBar.gameObject.SetActive(true);
						this.progressBar.Value = (float)num / (float)num2;
						this.progressBar.Color = fullQuestBase.ProgressBarTint;
					}
					break;
				case FullQuestBase.ListCounterTypes.None:
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			if (this.extraDesc)
			{
				this.extraDesc.ExtraDescPrefab = (fullQuestBase.IsDescCounterTypeCustom ? fullQuestBase.CustomDescPrefab : null);
			}
		}
		else if (this.extraDesc)
		{
			this.extraDesc.ExtraDescPrefab = null;
		}
		bool flag2 = false;
		if (isInCompletedSection)
		{
			if (this.group)
			{
				this.group.AlphaSelf = this.completedAlpha;
			}
			if (this.iconGroup)
			{
				this.iconGroup.AlphaSelf = this.completedIconAlpha;
			}
		}
		else if (this.quest is SubQuest && flag)
		{
			if (this.canCompleteIcon)
			{
				this.canCompleteIcon.sprite = this.quest.QuestType.CanCompleteIcon;
			}
			flag2 = true;
		}
		if (this.animator)
		{
			this.animator.Play(flag2 ? InventoryItemQuest._canCompleteProp : InventoryItemQuest._idleProp);
		}
		this.UpdateDisplay();
	}

	// Token: 0x0600333F RID: 13119 RVA: 0x000E3E3C File Offset: 0x000E203C
	public void RegisterTextForUpdate()
	{
		this.updateQueued = true;
	}

	// Token: 0x06003340 RID: 13120 RVA: 0x000E3E48 File Offset: 0x000E2048
	public override bool Submit()
	{
		if (this.Submitted == null || this.consumeBlocked)
		{
			return false;
		}
		if (base.isActiveAndEnabled && !this.quest.QuestType.IsDonateType)
		{
			base.StartCoroutine(this.ConsumeRoutine());
		}
		else
		{
			this.Submitted(this.quest);
		}
		return true;
	}

	// Token: 0x06003341 RID: 13121 RVA: 0x000E3EA2 File Offset: 0x000E20A2
	public override bool Cancel()
	{
		if (this.Canceled == null)
		{
			return base.Cancel();
		}
		this.Canceled();
		return true;
	}

	// Token: 0x06003342 RID: 13122 RVA: 0x000E3EBF File Offset: 0x000E20BF
	public override void Select(InventoryItemManager.SelectionDirection? direction)
	{
		base.Select(direction);
		this.quest.OnSelected();
	}

	// Token: 0x06003343 RID: 13123 RVA: 0x000E3ED3 File Offset: 0x000E20D3
	private IEnumerator ConsumeRoutine()
	{
		InventoryItemQuest.<>c__DisplayClass54_0 CS$<>8__locals1 = new InventoryItemQuest.<>c__DisplayClass54_0();
		CS$<>8__locals1.<>4__this = this;
		this.consumeBlocked = true;
		this.hasBlockedActions = true;
		this.Manager.IsActionsBlocked = true;
		if (this.consumeEffect)
		{
			CS$<>8__locals1.hitTrigger = false;
			this.consumeEffect.EventFired += CS$<>8__locals1.<ConsumeRoutine>g__Temp|0;
			this.consumeEffect.gameObject.SetActive(false);
			this.consumeEffect.gameObject.SetActive(true);
			this.consumeShake.DoShake(this, true);
			this.consumeAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
			while (!CS$<>8__locals1.hitTrigger)
			{
				yield return null;
			}
		}
		if (this.group)
		{
			this.group.AlphaSelf = 0f;
			yield return new WaitForSeconds(this.wishPromptPause);
		}
		else
		{
			Debug.LogError("Nested Fade Group \"group\" needs to be assigned!", this);
			yield return new WaitForSeconds(this.wishPromptPause);
		}
		this.consumeBlocked = false;
		this.Manager.IsActionsBlocked = false;
		this.hasBlockedActions = false;
		this.Submitted(this.quest);
		yield break;
	}

	// Token: 0x040036FB RID: 14075
	[Space]
	[SerializeField]
	private InventoryItemQuest.DescriptionTypes descriptionType;

	// Token: 0x040036FC RID: 14076
	[SerializeField]
	private BasicQuestBase quest;

	// Token: 0x040036FD RID: 14077
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x040036FE RID: 14078
	[SerializeField]
	private SpriteRenderer canCompleteIcon;

	// Token: 0x040036FF RID: 14079
	[SerializeField]
	private Animator animator;

	// Token: 0x04003700 RID: 14080
	[SerializeField]
	private TextMeshPro typeText;

	// Token: 0x04003701 RID: 14081
	[SerializeField]
	private TextMeshPro nameText;

	// Token: 0x04003702 RID: 14082
	[SerializeField]
	private IconCounter iconCounter;

	// Token: 0x04003703 RID: 14083
	[SerializeField]
	private ImageSlider progressBar;

	// Token: 0x04003704 RID: 14084
	[SerializeField]
	private ProgressBarSegmented progressBarSegmented;

	// Token: 0x04003705 RID: 14085
	[SerializeField]
	private NestedFadeGroupBase group;

	// Token: 0x04003706 RID: 14086
	[SerializeField]
	private float completedAlpha = 0.7f;

	// Token: 0x04003707 RID: 14087
	[SerializeField]
	private NestedFadeGroupBase iconGroup;

	// Token: 0x04003708 RID: 14088
	[SerializeField]
	private float completedIconAlpha = 1f;

	// Token: 0x04003709 RID: 14089
	[Space]
	[SerializeField]
	private float wishPromptPause = 0.5f;

	// Token: 0x0400370A RID: 14090
	[SerializeField]
	private CaptureAnimationEvent consumeEffect;

	// Token: 0x0400370B RID: 14091
	[SerializeField]
	private CameraShakeTarget consumeShake;

	// Token: 0x0400370C RID: 14092
	[SerializeField]
	private AudioEvent consumeAudio;

	// Token: 0x0400370D RID: 14093
	private bool consumeBlocked;

	// Token: 0x0400370E RID: 14094
	private bool wasInCompletedSection;

	// Token: 0x0400370F RID: 14095
	private InventoryItemExtraDescription extraDesc;

	// Token: 0x04003710 RID: 14096
	protected QuestItemManager Manager;

	// Token: 0x04003711 RID: 14097
	private bool hasBlockedActions;

	// Token: 0x04003712 RID: 14098
	private static readonly int _canCompleteProp = Animator.StringToHash("Can Complete");

	// Token: 0x04003713 RID: 14099
	private static readonly int _idleProp = Animator.StringToHash("Idle");

	// Token: 0x04003714 RID: 14100
	private bool updateQueued;

	// Token: 0x020018AE RID: 6318
	private enum DescriptionTypes
	{
		// Token: 0x04009307 RID: 37639
		Inventory,
		// Token: 0x04009308 RID: 37640
		QuestBoard
	}
}
