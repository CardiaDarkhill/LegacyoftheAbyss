using System;
using System.Text;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001B0 RID: 432
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Memento)")]
public class CollectableItemMemento : CollectableItemBasic, ICollectionViewerItem
{
	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x060010D3 RID: 4307 RVA: 0x0004FA08 File Offset: 0x0004DC08
	public bool CanDeposit
	{
		get
		{
			return !this.requireCompletedQuest || this.requireCompletedQuest.IsCompleted;
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x060010D4 RID: 4308 RVA: 0x0004FA24 File Offset: 0x0004DC24
	public Object CountKey
	{
		get
		{
			if (!this.countKey)
			{
				return this;
			}
			return this.countKey;
		}
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x060010D5 RID: 4309 RVA: 0x0004FA3B File Offset: 0x0004DC3B
	public bool IsSeenOverridden
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x060010D6 RID: 4310 RVA: 0x0004FA3E File Offset: 0x0004DC3E
	// (set) Token: 0x060010D7 RID: 4311 RVA: 0x0004FA5C File Offset: 0x0004DC5C
	public bool IsSeenOverrideValue
	{
		get
		{
			return PlayerData.instance.MementosDeposited.GetData(base.name).HasSeenInRelicBoard;
		}
		set
		{
			CollectableMementosData mementosDeposited = PlayerData.instance.MementosDeposited;
			CollectableMementosData.Data data = mementosDeposited.GetData(base.name);
			data.HasSeenInRelicBoard = value;
			mementosDeposited.SetData(base.name, data);
		}
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0004FA94 File Offset: 0x0004DC94
	public bool IsListedInCollection()
	{
		return this.CollectedAmount > 0 || this.IsVisibleInCollection();
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x0004FAA7 File Offset: 0x0004DCA7
	public override bool IsVisibleInCollection()
	{
		return PlayerData.instance.MementosDeposited.GetData(base.name).IsDeposited;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x0004FAC4 File Offset: 0x0004DCC4
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		string description = base.GetDescription(readSource);
		if (readSource == CollectableItem.ReadSource.Shop)
		{
			return description;
		}
		if (this.requireCompletedQuest && !this.requireCompletedQuest.IsCompleted)
		{
			return description;
		}
		StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(description);
		tempStringBuilder.AppendLine();
		tempStringBuilder.AppendLine();
		tempStringBuilder.Append(this.extraDesc);
		return tempStringBuilder.ToString();
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x0004FB2E File Offset: 0x0004DD2E
	string ICollectionViewerItem.get_name()
	{
		return base.name;
	}

	// Token: 0x04000FF9 RID: 4089
	[Header("Memento")]
	[SerializeField]
	private FullQuestBase requireCompletedQuest;

	// Token: 0x04000FFA RID: 4090
	[SerializeField]
	private Object countKey;

	// Token: 0x04000FFB RID: 4091
	[SerializeField]
	private LocalisedString extraDesc;
}
