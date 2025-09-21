using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000738 RID: 1848
public class ToolCrestUIMsg : UIMsgBase<ToolCrest>
{
	// Token: 0x06004201 RID: 16897 RVA: 0x00122548 File Offset: 0x00120748
	public static void Spawn(ToolCrest crest, GameObject prefab, Action afterMsg = null)
	{
		ToolCrestUIMsg component = prefab.GetComponent<ToolCrestUIMsg>();
		if (!component)
		{
			return;
		}
		UIMsgBase<ToolCrest>.Spawn(crest, component, afterMsg);
	}

	// Token: 0x06004202 RID: 16898 RVA: 0x00122570 File Offset: 0x00120770
	protected override void Setup(ToolCrest crest)
	{
		if (this.crestDisplay)
		{
			this.crestDisplay.sprite = crest.CrestSprite;
		}
		if (this.nameText)
		{
			this.nameText.text = crest.DisplayName;
		}
		if (this.descText)
		{
			this.descText.text = crest.GetPromptDesc;
		}
		if (this.itemPrefixText)
		{
			this.itemPrefixText.text = crest.ItemNamePrefix;
		}
		if (this.equipText)
		{
			this.equipText.text = crest.EquipText;
		}
	}

	// Token: 0x04004384 RID: 17284
	[SerializeField]
	private SpriteRenderer crestDisplay;

	// Token: 0x04004385 RID: 17285
	[SerializeField]
	private TMP_Text nameText;

	// Token: 0x04004386 RID: 17286
	[SerializeField]
	private TMP_Text descText;

	// Token: 0x04004387 RID: 17287
	[SerializeField]
	private TMP_Text itemPrefixText;

	// Token: 0x04004388 RID: 17288
	[SerializeField]
	private TMP_Text equipText;
}
