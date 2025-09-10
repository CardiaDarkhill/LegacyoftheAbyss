using System;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public sealed class InventoryCollectableItemSelectionHelper : MonoBehaviour
{
	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x0600117A RID: 4474 RVA: 0x00051887 File Offset: 0x0004FA87
	// (set) Token: 0x0600117B RID: 4475 RVA: 0x0005188E File Offset: 0x0004FA8E
	public static InventoryCollectableItemSelectionHelper.SelectionType LastSelectionUpdate
	{
		get
		{
			return InventoryCollectableItemSelectionHelper.lastSelectionUpdate;
		}
		set
		{
			if (value != InventoryCollectableItemSelectionHelper.SelectionType.None)
			{
				CollectableItemManager.CollectedItem = null;
				InventoryPaneList.SetNextOpen("Inv");
			}
			InventoryCollectableItemSelectionHelper.lastSelectionUpdate = value;
		}
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x000518A9 File Offset: 0x0004FAA9
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<InventoryItemSelectable>(ref this.selectables, typeof(InventoryCollectableItemSelectionHelper.SelectionType));
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x000518C0 File Offset: 0x0004FAC0
	private void OnDestroy()
	{
		InventoryCollectableItemSelectionHelper.LastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.None;
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x000518C8 File Offset: 0x0004FAC8
	public bool TryGetSelectable(out InventoryItemSelectable selectable)
	{
		if (InventoryCollectableItemSelectionHelper.lastSelectionUpdate == InventoryCollectableItemSelectionHelper.SelectionType.None)
		{
			selectable = null;
			return false;
		}
		try
		{
			selectable = this.selectables[(int)InventoryCollectableItemSelectionHelper.lastSelectionUpdate];
		}
		catch (Exception)
		{
			selectable = null;
		}
		InventoryCollectableItemSelectionHelper.lastSelectionUpdate = InventoryCollectableItemSelectionHelper.SelectionType.None;
		return true;
	}

	// Token: 0x04001069 RID: 4201
	[ArrayForEnum(typeof(InventoryCollectableItemSelectionHelper.SelectionType))]
	[SerializeField]
	private InventoryItemSelectable[] selectables = new InventoryItemSelectable[0];

	// Token: 0x0400106A RID: 4202
	private static InventoryCollectableItemSelectionHelper.SelectionType lastSelectionUpdate;

	// Token: 0x02001504 RID: 5380
	[Serializable]
	public enum SelectionType
	{
		// Token: 0x0400858E RID: 34190
		None,
		// Token: 0x0400858F RID: 34191
		Needle,
		// Token: 0x04008590 RID: 34192
		MaskShard,
		// Token: 0x04008591 RID: 34193
		SpoolPiece,
		// Token: 0x04008592 RID: 34194
		Silk,
		// Token: 0x04008593 RID: 34195
		Needolin,
		// Token: 0x04008594 RID: 34196
		Sprint,
		// Token: 0x04008595 RID: 34197
		HarpoonDash,
		// Token: 0x04008596 RID: 34198
		EvaHeal,
		// Token: 0x04008597 RID: 34199
		SuperJump,
		// Token: 0x04008598 RID: 34200
		WallJump
	}
}
