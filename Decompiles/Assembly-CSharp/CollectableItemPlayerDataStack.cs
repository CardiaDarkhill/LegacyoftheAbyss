using System;
using System.Linq;
using System.Text;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001B3 RID: 435
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (PlayerData Stack)")]
public class CollectableItemPlayerDataStack : CollectableItemStack
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06001105 RID: 4357 RVA: 0x00050569 File Offset: 0x0004E769
	public override int CollectedAmount
	{
		get
		{
			return this.stackItems.Count((CollectableItemPlayerDataStack.StackItemInfo station) => station.IsUnlocked);
		}
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00050598 File Offset: 0x0004E798
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		PlayerData instance = PlayerData.instance;
		if (readSource == CollectableItem.ReadSource.GetPopup || readSource == CollectableItem.ReadSource.TakePopup)
		{
			foreach (CollectableItemPlayerDataStack.StackItemInfo stackItemInfo in this.stackItems)
			{
				if (!string.IsNullOrEmpty(stackItemInfo.PlayerDataBool) && !string.IsNullOrEmpty(instance.LastSetFieldName) && instance.LastSetFieldName == stackItemInfo.PlayerDataBool)
				{
					return stackItemInfo.Name;
				}
			}
		}
		return base.GetDisplayName(readSource);
	}

	// Token: 0x06001107 RID: 4359 RVA: 0x0005060C File Offset: 0x0004E80C
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		StringBuilder tempStringBuilder = global::Helper.GetTempStringBuilder(base.GetDescription(readSource));
		if (this.CollectedAmount > 1)
		{
			tempStringBuilder.AppendLine();
			if (!this.stackDescHeader.IsEmpty)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.Append(this.stackDescHeader);
			}
			foreach (CollectableItemPlayerDataStack.StackItemInfo stackItemInfo in this.stackItems)
			{
				if (!stackItemInfo.Name.IsEmpty && stackItemInfo.IsUnlocked)
				{
					tempStringBuilder.AppendLine();
					tempStringBuilder.AppendFormat(this.stackItemListFormat, stackItemInfo.Name);
				}
			}
		}
		return tempStringBuilder.ToString();
	}

	// Token: 0x0400101D RID: 4125
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString stackDescHeader;

	// Token: 0x0400101E RID: 4126
	[SerializeField]
	[TextArea]
	private string stackItemListFormat;

	// Token: 0x0400101F RID: 4127
	[SerializeField]
	private CollectableItemPlayerDataStack.StackItemInfo[] stackItems;

	// Token: 0x020014F2 RID: 5362
	[Serializable]
	private class StackItemInfo
	{
		// Token: 0x17000D41 RID: 3393
		// (get) Token: 0x06008553 RID: 34131 RVA: 0x0026F5F1 File Offset: 0x0026D7F1
		public bool IsUnlocked
		{
			get
			{
				return !this.Name.IsEmpty && (string.IsNullOrEmpty(this.PlayerDataBool) || PlayerData.instance.GetVariable(this.PlayerDataBool));
			}
		}

		// Token: 0x04008550 RID: 34128
		[PlayerDataField(typeof(bool), false)]
		public string PlayerDataBool;

		// Token: 0x04008551 RID: 34129
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString Name;
	}
}
