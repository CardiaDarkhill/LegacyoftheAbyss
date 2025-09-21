using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x0200059D RID: 1437
public class QuestItemBoard : QuestItemManager
{
	// Token: 0x140000A2 RID: 162
	// (add) Token: 0x06003393 RID: 13203 RVA: 0x000E5B90 File Offset: 0x000E3D90
	// (remove) Token: 0x06003394 RID: 13204 RVA: 0x000E5BC8 File Offset: 0x000E3DC8
	public event Action QuestAccepted;

	// Token: 0x140000A3 RID: 163
	// (add) Token: 0x06003395 RID: 13205 RVA: 0x000E5C00 File Offset: 0x000E3E00
	// (remove) Token: 0x06003396 RID: 13206 RVA: 0x000E5C38 File Offset: 0x000E3E38
	public event Action<List<BasicQuestBase>> BoardClosed;

	// Token: 0x140000A4 RID: 164
	// (add) Token: 0x06003397 RID: 13207 RVA: 0x000E5C70 File Offset: 0x000E3E70
	// (remove) Token: 0x06003398 RID: 13208 RVA: 0x000E5CA8 File Offset: 0x000E3EA8
	public event Action<FullQuestBase> DonateQuestAccepted;

	// Token: 0x170005AA RID: 1450
	// (get) Token: 0x06003399 RID: 13209 RVA: 0x000E5CDD File Offset: 0x000E3EDD
	public int AvailableQuestsCount
	{
		get
		{
			return this.GetItems().Count;
		}
	}

	// Token: 0x170005AB RID: 1451
	// (get) Token: 0x0600339A RID: 13210 RVA: 0x000E5CEA File Offset: 0x000E3EEA
	public List<BasicQuestBase> Quests
	{
		get
		{
			return this.GetItems();
		}
	}

	// Token: 0x170005AC RID: 1452
	// (get) Token: 0x0600339B RID: 13211 RVA: 0x000E5CF2 File Offset: 0x000E3EF2
	private bool InputBlocked
	{
		get
		{
			return this.currentState != QuestItemBoard.States.DisplayQuests || this.fadeStateRoutine != null;
		}
	}

	// Token: 0x0600339C RID: 13212 RVA: 0x000E5D08 File Offset: 0x000E3F08
	protected override void Awake()
	{
		base.Awake();
		this.pane = base.GetComponent<InventoryPaneStandalone>();
		if (this.pane)
		{
			this.pane.PaneOpenedAnimEnd += delegate()
			{
				if (this.cursor)
				{
					this.cursor.Activate();
				}
				base.IsActionsBlocked = false;
				base.SetSelected(InventoryItemManager.SelectedActionType.Default, false);
			};
			this.pane.PaneClosedAnimEnd += delegate()
			{
				if (this.BoardClosed != null)
				{
					this.BoardClosed(this.acceptedQuests);
				}
				GameCameras.instance.HUDIn();
			};
		}
		base.transform.SetPosition2D(this.worldPosition);
		if (this.yesButton)
		{
			this.yesButton.InactiveConditionText = delegate()
			{
				if (!this.yesNoQuest || !this.yesNoQuest.CanComplete)
				{
					return this.notEnoughText;
				}
				return string.Empty;
			};
		}
	}

	// Token: 0x0600339D RID: 13213 RVA: 0x000E5D97 File Offset: 0x000E3F97
	protected override List<BasicQuestBase> GetItems()
	{
		if (this.GetAvailableQuestsFunc == null)
		{
			return new List<BasicQuestBase>();
		}
		return this.GetAvailableQuestsFunc().Cast<BasicQuestBase>().ToList<BasicQuestBase>();
	}

	// Token: 0x0600339E RID: 13214 RVA: 0x000E5DBC File Offset: 0x000E3FBC
	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemQuest> questItems, List<BasicQuestBase> quests)
	{
		for (int i = 0; i < questItems.Count; i++)
		{
			questItems[i].gameObject.SetActive(true);
			questItems[i].SetQuest(quests[i], false);
		}
		if (!base.IsActionsBlocked && questItems.Count > 0)
		{
			InventoryItemQuest inventoryItemQuest = base.CurrentSelected as InventoryItemQuest;
			if (!inventoryItemQuest || !questItems.Contains(inventoryItemQuest))
			{
				base.SetSelected(questItems[questItems.Count - 1], null, false);
			}
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = questItems.Cast<InventoryItemSelectableDirectional>().ToList<InventoryItemSelectableDirectional>()
			}
		};
	}

	// Token: 0x0600339F RID: 13215 RVA: 0x000E5E6E File Offset: 0x000E406E
	protected override void OnItemInstantiated(InventoryItemQuest questItem)
	{
		questItem.Submitted += this.SubmitQuestSelection;
		questItem.Canceled += this.CancelQuestSelection;
	}

	// Token: 0x060033A0 RID: 13216 RVA: 0x000E5E94 File Offset: 0x000E4094
	public void OpenPane()
	{
		base.IsActionsBlocked = true;
		if (this.cursor)
		{
			this.cursor.Deactivate();
		}
		if (this.pane)
		{
			this.pane.PaneStart();
		}
		this.SetState((this.AvailableQuestsCount > 0) ? QuestItemBoard.States.DisplayQuests : QuestItemBoard.States.Empty);
		bool flag = this.currentState != QuestItemBoard.States.Empty;
		if (this.selectGroup)
		{
			this.selectGroup.AlphaSelf = 1f;
			this.selectGroup.gameObject.SetActive(flag);
		}
		if (this.yesNoGroup)
		{
			this.yesNoGroup.AlphaSelf = 1f;
			this.yesNoGroup.gameObject.SetActive(false);
		}
		if (this.emptyGroup)
		{
			this.emptyGroup.AlphaSelf = 1f;
			this.emptyGroup.gameObject.SetActive(!flag);
		}
		if (this.acceptedQuests == null)
		{
			this.acceptedQuests = new List<BasicQuestBase>();
		}
		else
		{
			this.acceptedQuests.Clear();
		}
		this.UpdateButtonPrompts();
		this.SetDisplay(null);
		GameCameras.instance.HUDOut();
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x000E5FBC File Offset: 0x000E41BC
	public override bool MoveSelection(InventoryItemManager.SelectionDirection direction)
	{
		return this.InputBlocked || base.MoveSelection(direction);
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x000E5FD0 File Offset: 0x000E41D0
	private void SubmitQuestSelection(BasicQuestBase quest)
	{
		if (this.InputBlocked)
		{
			return;
		}
		this.acceptedQuests.Add(quest);
		this.yesNoQuest = (quest as FullQuestBase);
		if (this.yesNoQuest != null)
		{
			if (this.yesNoQuest.IsDonateType)
			{
				this.DisplayCurrencyCounters(this.yesNoQuest);
				this.FadeToState(QuestItemBoard.States.YesNo);
				if (this.yesNoQuestDisplay)
				{
					this.yesNoQuestDisplay.SetQuest(this.yesNoQuest);
				}
				this.selectAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
				return;
			}
			this.SetDisplay(null);
			if (this.cursor)
			{
				this.cursor.Deactivate();
			}
			Action action = this.GetPromptBlocker();
			action = (Action)Delegate.Combine(action, new Action(delegate()
			{
				if (this.cursor)
				{
					this.cursor.Activate();
				}
				this.QuestActioned();
			}));
			this.yesNoQuest.BeginQuest(action, true);
		}
		else
		{
			quest.Get(true);
			this.QuestActioned();
		}
		Action questAccepted = this.QuestAccepted;
		if (questAccepted == null)
		{
			return;
		}
		questAccepted();
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x000E60D5 File Offset: 0x000E42D5
	private void QuestActioned()
	{
		if (this.AvailableQuestsCount > 0)
		{
			base.UpdateList();
			base.SetSelected(InventoryItemManager.SelectedActionType.Previous, false);
			return;
		}
		this.CloseBoard();
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x000E60F6 File Offset: 0x000E42F6
	private Action GetPromptBlocker()
	{
		base.IsActionsBlocked = true;
		InteractManager.IsDisabled = true;
		HeroController.instance.AddInputBlocker(this);
		this.isPromptOverTop = true;
		this.UpdateButtonPrompts();
		return delegate()
		{
			base.IsActionsBlocked = false;
			InteractManager.IsDisabled = false;
			HeroController.instance.RemoveInputBlocker(this);
			this.isPromptOverTop = false;
			this.UpdateButtonPrompts();
		};
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x000E6129 File Offset: 0x000E4329
	[UsedImplicitly]
	public void AcceptDonation()
	{
		this.CloseBoard();
		Action<FullQuestBase> donateQuestAccepted = this.DonateQuestAccepted;
		if (donateQuestAccepted == null)
		{
			return;
		}
		donateQuestAccepted(this.yesNoQuest);
	}

	// Token: 0x060033A6 RID: 13222 RVA: 0x000E6147 File Offset: 0x000E4347
	[UsedImplicitly]
	public void DeclineDonation()
	{
		if (this.AvailableQuestsCount > 0)
		{
			this.FadeToState(QuestItemBoard.States.DisplayQuests);
		}
		this.HideCurrencyCounters(this.yesNoQuest);
		this.yesNoQuest = null;
	}

	// Token: 0x060033A7 RID: 13223 RVA: 0x000E616C File Offset: 0x000E436C
	private void CancelQuestSelection()
	{
		if (this.InputBlocked)
		{
			return;
		}
		this.CloseBoard();
	}

	// Token: 0x060033A8 RID: 13224 RVA: 0x000E6180 File Offset: 0x000E4380
	private void CloseBoard()
	{
		this.Quests.ForEach(delegate(BasicQuestBase quest)
		{
			quest.HasBeenSeen = true;
		});
		if (this.pane)
		{
			this.pane.PaneEnd();
		}
		base.IsActionsBlocked = true;
	}

	// Token: 0x060033A9 RID: 13225 RVA: 0x000E61D6 File Offset: 0x000E43D6
	public override bool SubmitButtonSelected()
	{
		if (this.currentState == QuestItemBoard.States.Empty)
		{
			this.CloseBoard();
			return true;
		}
		return !this.InputBlocked && base.SubmitButtonSelected();
	}

	// Token: 0x060033AA RID: 13226 RVA: 0x000E61F9 File Offset: 0x000E43F9
	public override bool CancelButtonSelected()
	{
		if (this.currentState == QuestItemBoard.States.Empty)
		{
			this.CloseBoard();
			return true;
		}
		return !this.InputBlocked && base.CancelButtonSelected();
	}

	// Token: 0x060033AB RID: 13227 RVA: 0x000E621C File Offset: 0x000E441C
	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		InventoryItemQuest inventoryItemQuest = selectable as InventoryItemQuest;
		if (inventoryItemQuest == null)
		{
			return;
		}
		this.fullQuest = (inventoryItemQuest.Quest as FullQuestBase);
		this.UpdateButtonPrompts();
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x000E6258 File Offset: 0x000E4458
	private void UpdateButtonPrompts()
	{
		if (this.currentState == QuestItemBoard.States.DisplayQuests)
		{
			bool flag = this.fullQuest != null && this.fullQuest.IsDonateType;
			if (this.submitText)
			{
				this.submitText.text = (flag ? this.donateLabel : this.takeLabel);
			}
			if (this.submitPrompt)
			{
				this.submitPrompt.AlphaSelf = 1f;
			}
		}
		else
		{
			if (this.submitText)
			{
				this.submitText.text = this.confirmLabel;
			}
			if (this.submitPrompt)
			{
				this.submitPrompt.AlphaSelf = ((this.currentState == QuestItemBoard.States.Empty) ? 0f : 1f);
			}
		}
		if (this.isPromptOverTop)
		{
			if (this.submitPrompt)
			{
				this.submitPrompt.AlphaSelf = 0f;
			}
			if (this.cancelPrompt)
			{
				this.cancelPrompt.AlphaSelf = 0f;
				return;
			}
		}
		else if (this.cancelPrompt)
		{
			this.cancelPrompt.AlphaSelf = 1f;
		}
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x000E6388 File Offset: 0x000E4588
	private void SetState(QuestItemBoard.States newState)
	{
		NestedFadeGroupBase groupForState = this.GetGroupForState(this.currentState);
		if (groupForState)
		{
			groupForState.gameObject.SetActive(false);
		}
		this.currentState = newState;
		NestedFadeGroupBase groupForState2 = this.GetGroupForState(this.currentState);
		if (groupForState2)
		{
			groupForState2.AlphaSelf = 1f;
			groupForState2.gameObject.SetActive(true);
		}
		this.UpdateButtonPrompts();
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x000E63EF File Offset: 0x000E45EF
	private void FadeToState(QuestItemBoard.States newState)
	{
		if (this.fadeStateRoutine != null)
		{
			base.StopCoroutine(this.fadeStateRoutine);
		}
		this.fadeStateRoutine = base.StartCoroutine(this.FadeStateRoutine(newState));
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x000E6418 File Offset: 0x000E4618
	private IEnumerator FadeStateRoutine(QuestItemBoard.States newState)
	{
		base.IsActionsBlocked = true;
		if (newState == QuestItemBoard.States.YesNo && this.yesNoList)
		{
			this.yesNoList.SetActive(false);
		}
		NestedFadeGroupBase previousGroup = this.GetGroupForState(this.currentState);
		if (previousGroup)
		{
			for (float elapsed = 0f; elapsed < this.groupFadeDuration; elapsed += Time.deltaTime)
			{
				float num = elapsed / this.groupFadeDuration;
				previousGroup.AlphaSelf = 1f - num;
				yield return null;
			}
			previousGroup.gameObject.SetActive(false);
		}
		this.currentState = newState;
		this.UpdateButtonPrompts();
		NestedFadeGroupBase currentGroup = this.GetGroupForState(this.currentState);
		if (currentGroup)
		{
			currentGroup.gameObject.SetActive(true);
			for (float elapsed = 0f; elapsed < this.groupFadeDuration; elapsed += Time.deltaTime)
			{
				float alphaSelf = elapsed / this.groupFadeDuration;
				currentGroup.AlphaSelf = alphaSelf;
				yield return null;
			}
		}
		if (newState == QuestItemBoard.States.YesNo && this.yesNoList)
		{
			this.yesNoList.SetActive(true);
		}
		base.IsActionsBlocked = false;
		this.fadeStateRoutine = null;
		yield break;
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x000E642E File Offset: 0x000E462E
	private NestedFadeGroupBase GetGroupForState(QuestItemBoard.States state)
	{
		switch (state)
		{
		case QuestItemBoard.States.None:
			return null;
		case QuestItemBoard.States.DisplayQuests:
			return this.selectGroup;
		case QuestItemBoard.States.YesNo:
			return this.yesNoGroup;
		case QuestItemBoard.States.Empty:
			return this.emptyGroup;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x000E6464 File Offset: 0x000E4664
	public void DisplayCurrencyCounters(FullQuestBase quest)
	{
		foreach (FullQuestBase.QuestTarget questTarget in quest.Targets)
		{
			if (!questTarget.Counter)
			{
				break;
			}
			QuestTargetCurrency questTargetCurrency = questTarget.Counter as QuestTargetCurrency;
			if (questTargetCurrency != null)
			{
				CurrencyCounter.Show(questTargetCurrency.CurrencyType, false);
			}
			else
			{
				CollectableItem collectableItem = questTarget.Counter as CollectableItem;
				if (collectableItem != null)
				{
					ItemCurrencyCounter.Show(collectableItem);
				}
				else
				{
					Debug.LogErrorFormat(this, "Could not get counter item for quest: {0}", new object[]
					{
						quest.name
					});
				}
			}
		}
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x000E6514 File Offset: 0x000E4714
	public void HideCurrencyCounters(FullQuestBase quest)
	{
		foreach (FullQuestBase.QuestTarget questTarget in quest.Targets)
		{
			if (!questTarget.Counter)
			{
				break;
			}
			QuestTargetCurrency questTargetCurrency = questTarget.Counter as QuestTargetCurrency;
			if (questTargetCurrency != null)
			{
				CurrencyCounter.Hide(questTargetCurrency.CurrencyType);
			}
			else
			{
				CollectableItem collectableItem = questTarget.Counter as CollectableItem;
				if (collectableItem != null)
				{
					ItemCurrencyCounter.Hide(collectableItem);
				}
				else
				{
					Debug.LogErrorFormat(this, "Could not get counter item for quest: {0}", new object[]
					{
						quest.name
					});
				}
			}
		}
	}

	// Token: 0x04003753 RID: 14163
	[Header("Quest Board")]
	[SerializeField]
	private Vector2 worldPosition;

	// Token: 0x04003754 RID: 14164
	[Space]
	[SerializeField]
	private NestedFadeGroup selectGroup;

	// Token: 0x04003755 RID: 14165
	[SerializeField]
	private NestedFadeGroup yesNoGroup;

	// Token: 0x04003756 RID: 14166
	[SerializeField]
	private UISelectionList yesNoList;

	// Token: 0x04003757 RID: 14167
	[SerializeField]
	private QuestIconDisplay yesNoQuestDisplay;

	// Token: 0x04003758 RID: 14168
	[SerializeField]
	private NestedFadeGroup emptyGroup;

	// Token: 0x04003759 RID: 14169
	[SerializeField]
	private float groupFadeDuration;

	// Token: 0x0400375A RID: 14170
	[SerializeField]
	private AudioEvent selectAudio;

	// Token: 0x0400375B RID: 14171
	[Space]
	[SerializeField]
	private NestedFadeGroupBase submitPrompt;

	// Token: 0x0400375C RID: 14172
	[SerializeField]
	private TMP_Text submitText;

	// Token: 0x0400375D RID: 14173
	[SerializeField]
	private NestedFadeGroupBase cancelPrompt;

	// Token: 0x0400375E RID: 14174
	[SerializeField]
	private LocalisedString takeLabel;

	// Token: 0x0400375F RID: 14175
	[SerializeField]
	private LocalisedString donateLabel;

	// Token: 0x04003760 RID: 14176
	[SerializeField]
	private LocalisedString confirmLabel;

	// Token: 0x04003761 RID: 14177
	[Space]
	[SerializeField]
	private UISelectionListItem yesButton;

	// Token: 0x04003762 RID: 14178
	[SerializeField]
	private LocalisedString notEnoughText;

	// Token: 0x04003763 RID: 14179
	private List<BasicQuestBase> acceptedQuests;

	// Token: 0x04003764 RID: 14180
	private QuestItemBoard.States currentState;

	// Token: 0x04003765 RID: 14181
	private bool isPromptOverTop;

	// Token: 0x04003766 RID: 14182
	private FullQuestBase yesNoQuest;

	// Token: 0x04003767 RID: 14183
	private FullQuestBase fullQuest;

	// Token: 0x04003768 RID: 14184
	private Coroutine fadeStateRoutine;

	// Token: 0x04003769 RID: 14185
	private InventoryPaneStandalone pane;

	// Token: 0x0400376A RID: 14186
	public Func<IReadOnlyCollection<QuestGroupBase>> GetAvailableQuestsFunc;

	// Token: 0x020018C4 RID: 6340
	private enum States
	{
		// Token: 0x04009348 RID: 37704
		None,
		// Token: 0x04009349 RID: 37705
		DisplayQuests,
		// Token: 0x0400934A RID: 37706
		YesNo,
		// Token: 0x0400934B RID: 37707
		Empty
	}
}
