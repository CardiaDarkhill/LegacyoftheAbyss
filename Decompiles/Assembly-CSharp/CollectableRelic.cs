using System;
using System.Runtime.CompilerServices;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x020001E8 RID: 488
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Relics/Collectable Relic")]
public class CollectableRelic : QuestTargetCounter, ICollectionViewerItem
{
	// Token: 0x17000213 RID: 531
	// (get) Token: 0x060012C5 RID: 4805 RVA: 0x00056B64 File Offset: 0x00054D64
	public string DisplayName
	{
		get
		{
			CollectableItemRelicType relicType = this.RelicType;
			if (relicType == null)
			{
				return null;
			}
			return relicType.GetDisplayName(CollectableItem.ReadSource.Inventory);
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x060012C6 RID: 4806 RVA: 0x00056B78 File Offset: 0x00054D78
	public string Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x060012C7 RID: 4807 RVA: 0x00056B85 File Offset: 0x00054D85
	// (set) Token: 0x060012C8 RID: 4808 RVA: 0x00056B8D File Offset: 0x00054D8D
	public CollectableItemRelicType RelicType { get; set; }

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x060012C9 RID: 4809 RVA: 0x00056B96 File Offset: 0x00054D96
	public bool PlaySyncedAudioSource
	{
		get
		{
			return this.playSyncedAudioSource;
		}
	}

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x060012CA RID: 4810 RVA: 0x00056B9E File Offset: 0x00054D9E
	// (set) Token: 0x060012CB RID: 4811 RVA: 0x00056BA6 File Offset: 0x00054DA6
	public CollectableRelicsData.Data SavedData
	{
		get
		{
			return CollectableRelicManager.GetRelicData(this);
		}
		set
		{
			CollectableRelicManager.SetRelicData(this, value);
		}
	}

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x060012CC RID: 4812 RVA: 0x00056BB0 File Offset: 0x00054DB0
	public bool IsInInventory
	{
		get
		{
			CollectableRelicsData.Data savedData = this.SavedData;
			return savedData.IsCollected && !savedData.IsDeposited;
		}
	}

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x060012CD RID: 4813 RVA: 0x00056BD7 File Offset: 0x00054DD7
	public bool IsPlayable
	{
		get
		{
			return this.RelicType && this.RelicType.RelicPlayType > CollectableItemRelicType.RelicPlayTypes.None;
		}
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x060012CE RID: 4814 RVA: 0x00056BF6 File Offset: 0x00054DF6
	public bool IsLoading
	{
		get
		{
			return this.loadWaitingCount > 0;
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x060012CF RID: 4815 RVA: 0x00056C01 File Offset: 0x00054E01
	public AudioClip GramaphoneClip
	{
		get
		{
			if (this.IsGramaphoneRelicPlayType())
			{
				return this.gramaphoneClip;
			}
			return null;
		}
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x060012D0 RID: 4816 RVA: 0x00056C13 File Offset: 0x00054E13
	public AudioClip NeedolinClip
	{
		get
		{
			if (this.IsGramaphoneRelicPlayType())
			{
				return this.needolinClip;
			}
			return null;
		}
	}

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x060012D1 RID: 4817 RVA: 0x00056C25 File Offset: 0x00054E25
	public AudioMixerGroup MixerOverride
	{
		get
		{
			if (this.IsGramaphoneRelicPlayType())
			{
				return this.mixerOverride;
			}
			return null;
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x060012D2 RID: 4818 RVA: 0x00056C37 File Offset: 0x00054E37
	public bool WillSendPlayEvent
	{
		get
		{
			return !string.IsNullOrEmpty(this.playEventRegister) && (!this.eventConditionItem || this.eventConditionItem.CanGetMore());
		}
	}

	// Token: 0x060012D3 RID: 4819 RVA: 0x00056C63 File Offset: 0x00054E63
	private bool IsGramaphoneRelicPlayType()
	{
		return this.RelicType && this.RelicType.RelicPlayType == CollectableItemRelicType.RelicPlayTypes.Gramaphone;
	}

	// Token: 0x060012D4 RID: 4820 RVA: 0x00056C84 File Offset: 0x00054E84
	public override void Get(bool showPopup = true)
	{
		CollectableRelicsData.Data savedData = this.SavedData;
		savedData.IsCollected = true;
		this.SavedData = savedData;
		if (this.RelicType)
		{
			bool flag = true;
			if (showPopup)
			{
				CollectableUIMsg itemUiMsg = CollectableUIMsg.Spawn(this, null, false);
				if (QuestManager.MaybeShowQuestUpdated(this, itemUiMsg))
				{
					flag = false;
				}
			}
			if (flag)
			{
				CollectableItemManager.CollectedItem = this.RelicType;
				InventoryPaneList.SetNextOpen("Inv");
				PlayerData.instance.InvPaneHasNew = true;
			}
		}
		CollectableItemHeroReaction.DoReaction();
	}

	// Token: 0x060012D5 RID: 4821 RVA: 0x00056CF5 File Offset: 0x00054EF5
	public override bool CanGetMore()
	{
		return !this.SavedData.IsCollected;
	}

	// Token: 0x060012D6 RID: 4822 RVA: 0x00056D08 File Offset: 0x00054F08
	public void Deposit()
	{
		CollectableRelicsData.Data savedData = this.SavedData;
		if (!savedData.IsCollected)
		{
			Debug.LogError("Can't deposit a relic that hasn't been collected!", this);
			return;
		}
		savedData.IsDeposited = true;
		this.SavedData = savedData;
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x00056D3F File Offset: 0x00054F3F
	public override Sprite GetPopupIcon()
	{
		if (!this.RelicType)
		{
			return null;
		}
		return this.RelicType.GetUIMsgSprite();
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x00056D5B File Offset: 0x00054F5B
	public override string GetPopupName()
	{
		if (!this.RelicType)
		{
			return "!!NO_RELIC_TYPE!!";
		}
		return this.RelicType.GetDisplayName(CollectableItem.ReadSource.Inventory);
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x00056D7C File Offset: 0x00054F7C
	public override float GetUIMsgIconScale()
	{
		if (!this.RelicType)
		{
			return base.GetUIMsgIconScale();
		}
		return this.RelicType.GetUIMsgIconScale();
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x060012DA RID: 4826 RVA: 0x00056D9D File Offset: 0x00054F9D
	// (set) Token: 0x060012DB RID: 4827 RVA: 0x00056DAC File Offset: 0x00054FAC
	public bool IsSeen
	{
		get
		{
			return this.SavedData.HasSeenInRelicBoard;
		}
		set
		{
			CollectableRelicsData.Data savedData = this.SavedData;
			savedData.HasSeenInRelicBoard = value;
			this.SavedData = savedData;
		}
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x00056DCF File Offset: 0x00054FCF
	public string GetCollectionName()
	{
		return this.DisplayName;
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x00056DD7 File Offset: 0x00054FD7
	public string GetCollectionDesc()
	{
		return this.Description;
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x00056DDF File Offset: 0x00054FDF
	public Sprite GetCollectionIcon()
	{
		return this.RelicType.GetIcon(CollectableItem.ReadSource.Inventory);
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x00056DED File Offset: 0x00054FED
	public bool IsVisibleInCollection()
	{
		return this.SavedData.IsDeposited;
	}

	// Token: 0x060012E0 RID: 4832 RVA: 0x00056DFA File Offset: 0x00054FFA
	public bool IsRequiredInCollection()
	{
		return true;
	}

	// Token: 0x060012E1 RID: 4833 RVA: 0x00056DFD File Offset: 0x00054FFD
	public void OnPlayedEvent()
	{
		EventRegister.SendEvent(this.playEventRegister, null);
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x00056E0B File Offset: 0x0005500B
	public override Sprite GetQuestCounterSprite(int index)
	{
		return this.GetCollectionIcon();
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x00056E13 File Offset: 0x00055013
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		if (!this.SavedData.IsCollected)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x060012E4 RID: 4836 RVA: 0x00056E28 File Offset: 0x00055028
	public void LoadClips()
	{
		this.loadCount++;
		if (this.loadCount > 1)
		{
			return;
		}
		Debug.Log("Started loading clips for " + base.name);
		if (this.gramaphoneClipRef.RuntimeKeyIsValid())
		{
			this.loadWaitingCount++;
			this.gramaphoneClipRef.LoadAssetAsync().Completed += this.<LoadClips>g__OnCompletedGramaphoneClip|57_0;
		}
		if (this.needolinClipRef.RuntimeKeyIsValid())
		{
			this.loadWaitingCount++;
			this.needolinClipRef.LoadAssetAsync().Completed += this.<LoadClips>g__OnCompletedNeedolinClip|57_1;
		}
	}

	// Token: 0x060012E5 RID: 4837 RVA: 0x00056ED8 File Offset: 0x000550D8
	public void FreeClips()
	{
		if (this.loadCount <= 0)
		{
			return;
		}
		this.loadCount--;
		if (this.loadCount > 0)
		{
			return;
		}
		Debug.Log("Freed clips for " + base.name);
		if (this.gramaphoneClipRef.IsValid())
		{
			this.gramaphoneClipRef.ReleaseAsset();
		}
		if (this.needolinClipRef.IsValid())
		{
			this.needolinClipRef.ReleaseAsset();
		}
		this.loadWaitingCount = 0;
	}

	// Token: 0x060012E7 RID: 4839 RVA: 0x00056F5B File Offset: 0x0005515B
	string ICollectionViewerItem.get_name()
	{
		return base.name;
	}

	// Token: 0x060012E8 RID: 4840 RVA: 0x00056F64 File Offset: 0x00055164
	[CompilerGenerated]
	private void <LoadClips>g__OnCompletedGramaphoneClip|57_0(AsyncOperationHandle<AudioClip> op)
	{
		this.gramaphoneClip = op.Result;
		this.loadWaitingCount--;
		Debug.Log("Finished loading gramaphone clip for " + base.name);
		op.Completed -= this.<LoadClips>g__OnCompletedGramaphoneClip|57_0;
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x00056FB4 File Offset: 0x000551B4
	[CompilerGenerated]
	private void <LoadClips>g__OnCompletedNeedolinClip|57_1(AsyncOperationHandle<AudioClip> op)
	{
		this.needolinClip = op.Result;
		this.loadWaitingCount--;
		Debug.Log("Finished loading needolin clip for " + base.name);
		op.Completed -= this.<LoadClips>g__OnCompletedNeedolinClip|57_1;
	}

	// Token: 0x0400117B RID: 4475
	[Space]
	[SerializeField]
	private LocalisedString description;

	// Token: 0x0400117C RID: 4476
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsGramaphoneRelicPlayType", true, true, false)]
	private AssetReferenceT<AudioClip> gramaphoneClipRef;

	// Token: 0x0400117D RID: 4477
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsGramaphoneRelicPlayType", true, true, false)]
	private AssetReferenceT<AudioClip> needolinClipRef;

	// Token: 0x0400117E RID: 4478
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsGramaphoneRelicPlayType", true, true, false)]
	private AudioMixerGroup mixerOverride;

	// Token: 0x0400117F RID: 4479
	[SerializeField]
	private bool playSyncedAudioSource;

	// Token: 0x04001180 RID: 4480
	[Space]
	[SerializeField]
	private string playEventRegister;

	// Token: 0x04001181 RID: 4481
	[SerializeField]
	private SavedItem eventConditionItem;

	// Token: 0x04001182 RID: 4482
	[NonSerialized]
	private CollectableItemRelicType previousRelicType;

	// Token: 0x04001183 RID: 4483
	private int loadCount;

	// Token: 0x04001184 RID: 4484
	private int loadWaitingCount;

	// Token: 0x04001185 RID: 4485
	private AudioClip gramaphoneClip;

	// Token: 0x04001186 RID: 4486
	private AudioClip needolinClip;
}
