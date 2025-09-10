using System;
using System.Text;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001B8 RID: 440
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (States)")]
public class CollectableItemStates : CollectableItem
{
	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x0600112A RID: 4394 RVA: 0x00050BDD File Offset: 0x0004EDDD
	public override bool DisplayAmount
	{
		get
		{
			return !this.overridesCollected && this.CollectedAmount > 1;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x0600112B RID: 4395 RVA: 0x00050BF4 File Offset: 0x0004EDF4
	public override int CollectedAmount
	{
		get
		{
			if (!this.overridesCollected)
			{
				return base.CollectedAmount;
			}
			CollectableItemStates.ItemState[] array = this.states;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Test.IsFulfilled)
				{
					return 1;
				}
			}
			return 0;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x0600112C RID: 4396 RVA: 0x00050C3C File Offset: 0x0004EE3C
	protected override int IsSeenIndex
	{
		get
		{
			int currentStateIndex = this.GetCurrentStateIndex();
			if (currentStateIndex == 0 && !this.states[0].Test.IsDefined)
			{
				return -1;
			}
			return currentStateIndex;
		}
	}

	// Token: 0x0600112D RID: 4397 RVA: 0x00050C6E File Offset: 0x0004EE6E
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		return this.GetCurrentState().DisplayName;
	}

	// Token: 0x0600112E RID: 4398 RVA: 0x00050C80 File Offset: 0x0004EE80
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		CollectableItemStates.ItemState currentState = this.GetCurrentState();
		StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(currentState.Description);
		if (!currentState.DescriptionExtra.IsEmpty)
		{
			tempStringBuilder.AppendLine();
			tempStringBuilder.AppendLine();
			tempStringBuilder.AppendLine(currentState.DescriptionExtra);
		}
		if (currentState.HideAppends)
		{
			return tempStringBuilder.ToString();
		}
		if (this.appends.Length == 0)
		{
			return tempStringBuilder.ToString();
		}
		tempStringBuilder.AppendLine();
		foreach (CollectableItemStates.AppendDesc appendDesc in this.appends)
		{
			if (appendDesc.Condition.IsFulfilled)
			{
				string value = string.IsNullOrEmpty(appendDesc.Format) ? appendDesc.Text : string.Format(appendDesc.Format, appendDesc.Text);
				tempStringBuilder.AppendLine();
				tempStringBuilder.Append(value);
			}
		}
		return tempStringBuilder.ToString();
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x00050D6D File Offset: 0x0004EF6D
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		return this.GetCurrentState().Icon;
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x00050D7C File Offset: 0x0004EF7C
	public override InventoryItemButtonPromptData[] GetButtonPromptData()
	{
		CollectableItemStates.ItemState currentState = this.GetCurrentState();
		if (currentState.DisplayButtonPrompt)
		{
			return currentState.ButtonPromptData;
		}
		return null;
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x00050DA0 File Offset: 0x0004EFA0
	private CollectableItemStates.ItemState GetCurrentState()
	{
		int currentStateIndex = this.GetCurrentStateIndex();
		if (currentStateIndex >= 0)
		{
			return this.states[currentStateIndex];
		}
		return default(CollectableItemStates.ItemState);
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x00050DD0 File Offset: 0x0004EFD0
	private int GetCurrentStateIndex()
	{
		if (!Application.isPlaying)
		{
			return 0;
		}
		int num = -1;
		for (int i = 0; i < this.states.Length; i++)
		{
			if (this.states[i].Test.IsFulfilled)
			{
				num = i;
			}
		}
		if (num < 0)
		{
			Debug.LogError("Item state was less than 0");
		}
		return num;
	}

	// Token: 0x04001038 RID: 4152
	[Space]
	[SerializeField]
	private bool overridesCollected;

	// Token: 0x04001039 RID: 4153
	[Space]
	[SerializeField]
	private CollectableItemStates.ItemState[] states;

	// Token: 0x0400103A RID: 4154
	[Space]
	[SerializeField]
	private CollectableItemStates.AppendDesc[] appends;

	// Token: 0x020014FC RID: 5372
	[Serializable]
	private struct ItemState
	{
		// Token: 0x04008568 RID: 34152
		public LocalisedString DisplayName;

		// Token: 0x04008569 RID: 34153
		public LocalisedString Description;

		// Token: 0x0400856A RID: 34154
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString DescriptionExtra;

		// Token: 0x0400856B RID: 34155
		public Sprite Icon;

		// Token: 0x0400856C RID: 34156
		public PlayerDataTest Test;

		// Token: 0x0400856D RID: 34157
		public bool HideAppends;

		// Token: 0x0400856E RID: 34158
		public bool DisplayButtonPrompt;

		// Token: 0x0400856F RID: 34159
		public InventoryItemButtonPromptData[] ButtonPromptData;
	}

	// Token: 0x020014FD RID: 5373
	[Serializable]
	private class AppendDesc
	{
		// Token: 0x04008570 RID: 34160
		public LocalisedString Text;

		// Token: 0x04008571 RID: 34161
		public string Format;

		// Token: 0x04008572 RID: 34162
		public PlayerDataTest Condition;
	}
}
