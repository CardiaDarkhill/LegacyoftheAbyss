using System;
using System.Runtime.CompilerServices;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001B7 RID: 439
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Stack)")]
public class CollectableItemStack : CollectableItem
{
	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06001120 RID: 4384 RVA: 0x000509D7 File Offset: 0x0004EBD7
	public override int CollectedAmount
	{
		get
		{
			return base.CollectedAmount + (this.isAlwaysUnlocked ? 1 : 0);
		}
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x000509EC File Offset: 0x0004EBEC
	protected virtual void OnValidate()
	{
		if (this.stackVariations == null || this.stackVariations.Length == 0)
		{
			this.stackVariations = new CollectableItemStack.StackVariation[1];
		}
		if (this.singleIcon)
		{
			this.singleIcons = new Sprite[]
			{
				this.singleIcon
			};
			this.singleIcon = null;
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06001122 RID: 4386 RVA: 0x00050A3F File Offset: 0x0004EC3F
	public override bool DisplayAmount
	{
		get
		{
			return this.displayAmount && this.CollectedAmount > 1;
		}
	}

	// Token: 0x06001123 RID: 4387 RVA: 0x00050A54 File Offset: 0x0004EC54
	private ValueTuple<LocalisedString, LocalisedString> GetNameDescPair(CollectableItem.ReadSource readSource)
	{
		bool flag = this.CollectedAmount >= this.singleIcons.Length;
		if (flag && readSource == CollectableItem.ReadSource.TakePopup)
		{
			this.<GetNameDescPair>g__GetAllPair|17_0();
		}
		if (readSource != CollectableItem.ReadSource.Inventory)
		{
			return new ValueTuple<LocalisedString, LocalisedString>(this.singleDisplayName, this.singleDescription);
		}
		if (flag)
		{
			return this.<GetNameDescPair>g__GetAllPair|17_0();
		}
		if (this.CollectedAmount <= 1)
		{
			return new ValueTuple<LocalisedString, LocalisedString>(this.singleDisplayName, this.singleDescription);
		}
		return new ValueTuple<LocalisedString, LocalisedString>(this.pluralDisplayName, this.pluralDescription);
	}

	// Token: 0x06001124 RID: 4388 RVA: 0x00050ACE File Offset: 0x0004ECCE
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		return this.GetNameDescPair(readSource).Item1;
	}

	// Token: 0x06001125 RID: 4389 RVA: 0x00050AE1 File Offset: 0x0004ECE1
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		return this.GetNameDescPair(readSource).Item2;
	}

	// Token: 0x06001126 RID: 4390 RVA: 0x00050AF4 File Offset: 0x0004ECF4
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		if (readSource == CollectableItem.ReadSource.Inventory || readSource == CollectableItem.ReadSource.Shop || (readSource == CollectableItem.ReadSource.TakePopup && this.CollectedAmount >= this.singleIcons.Length))
		{
			return this.GetCurrentStackVariation().Icon;
		}
		int num = this.CollectedAmount - 1;
		if (num >= this.singleIcons.Length)
		{
			num = this.singleIcons.Length - 1;
		}
		else if (num < 0)
		{
			num = 0;
		}
		return this.singleIcons[num];
	}

	// Token: 0x06001127 RID: 4391 RVA: 0x00050B58 File Offset: 0x0004ED58
	private CollectableItemStack.StackVariation GetCurrentStackVariation()
	{
		int num = this.CollectedAmount - 1;
		if (num >= this.stackVariations.Length)
		{
			num = this.stackVariations.Length - 1;
		}
		else if (num < 0)
		{
			num = 0;
		}
		return this.stackVariations[num];
	}

	// Token: 0x06001129 RID: 4393 RVA: 0x00050BA0 File Offset: 0x0004EDA0
	[CompilerGenerated]
	private ValueTuple<LocalisedString, LocalisedString> <GetNameDescPair>g__GetAllPair|17_0()
	{
		return new ValueTuple<LocalisedString, LocalisedString>(this.allDisplayName.IsEmpty ? this.pluralDisplayName : this.allDisplayName, this.allDescription.IsEmpty ? this.pluralDescription : this.allDescription);
	}

	// Token: 0x0400102D RID: 4141
	[Space]
	[SerializeField]
	private LocalisedString singleDisplayName;

	// Token: 0x0400102E RID: 4142
	[SerializeField]
	private LocalisedString singleDescription;

	// Token: 0x0400102F RID: 4143
	[Space]
	[SerializeField]
	private LocalisedString pluralDisplayName;

	// Token: 0x04001030 RID: 4144
	[SerializeField]
	private LocalisedString pluralDescription;

	// Token: 0x04001031 RID: 4145
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString allDisplayName;

	// Token: 0x04001032 RID: 4146
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString allDescription;

	// Token: 0x04001033 RID: 4147
	[Space]
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private Sprite singleIcon;

	// Token: 0x04001034 RID: 4148
	[SerializeField]
	private Sprite[] singleIcons;

	// Token: 0x04001035 RID: 4149
	[SerializeField]
	private CollectableItemStack.StackVariation[] stackVariations;

	// Token: 0x04001036 RID: 4150
	[SerializeField]
	private bool isAlwaysUnlocked;

	// Token: 0x04001037 RID: 4151
	[SerializeField]
	private bool displayAmount;

	// Token: 0x020014FB RID: 5371
	[Serializable]
	private struct StackVariation
	{
		// Token: 0x04008567 RID: 34151
		public Sprite Icon;
	}
}
