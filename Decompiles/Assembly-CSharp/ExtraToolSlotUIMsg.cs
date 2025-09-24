using System;
using GlobalSettings;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x02000648 RID: 1608
public class ExtraToolSlotUIMsg : UIMsgBase<ToolItemType>
{
	// Token: 0x060039A2 RID: 14754 RVA: 0x000FD020 File Offset: 0x000FB220
	public static void Spawn(ToolItemType slotType, GameObject prefab, Action afterMsg = null)
	{
		ExtraToolSlotUIMsg component = prefab.GetComponent<ExtraToolSlotUIMsg>();
		if (!component)
		{
			return;
		}
		UIMsgBase<ToolItemType>.Spawn(slotType, component, afterMsg);
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x000FD048 File Offset: 0x000FB248
	protected override void Setup(ToolItemType slotType)
	{
		base.transform.position = this.spawnPosition;
		this.defendSlot.gameObject.SetActive(false);
		this.exploreSlot.gameObject.SetActive(false);
		if (slotType != ToolItemType.Blue)
		{
			if (slotType != ToolItemType.Yellow)
			{
				throw new NotImplementedException();
			}
			this.exploreSlot.gameObject.SetActive(true);
			this.nameText.text = this.exploreSlotName;
			this.exploreSlot.color = UI.GetToolTypeColor(ToolItemType.Yellow);
		}
		else
		{
			this.defendSlot.gameObject.SetActive(true);
			this.nameText.text = this.defendSlotName;
			this.defendSlot.color = UI.GetToolTypeColor(ToolItemType.Blue);
		}
		this.nameText.text = this.defendSlotName;
	}

	// Token: 0x04003C5B RID: 15451
	[SerializeField]
	private SpriteRenderer defendSlot;

	// Token: 0x04003C5C RID: 15452
	[SerializeField]
	private LocalisedString defendSlotName;

	// Token: 0x04003C5D RID: 15453
	[SerializeField]
	private SpriteRenderer exploreSlot;

	// Token: 0x04003C5E RID: 15454
	[SerializeField]
	private LocalisedString exploreSlotName;

	// Token: 0x04003C5F RID: 15455
	[Space]
	[SerializeField]
	private TMP_Text nameText;

	// Token: 0x04003C60 RID: 15456
	[SerializeField]
	private Vector3 spawnPosition;
}
