using System;
using TMProOld;
using UnityEngine;

// Token: 0x0200070E RID: 1806
public class SavedItemDisplay : MonoBehaviour
{
	// Token: 0x0600407D RID: 16509 RVA: 0x0011BB74 File Offset: 0x00119D74
	public void Setup(SavedItem item, int amount)
	{
		CollectableItem collectableItem = item as CollectableItem;
		Sprite popupIcon;
		if (collectableItem != null)
		{
			popupIcon = collectableItem.GetIcon(CollectableItem.ReadSource.TakePopup);
			if (!collectableItem.DisplayAmount)
			{
				amount = 0;
			}
		}
		else
		{
			popupIcon = item.GetPopupIcon();
		}
		this.icon.sprite = popupIcon;
		this.amountText.text = ((amount > 1) ? amount.ToString() : string.Empty);
	}

	// Token: 0x04004211 RID: 16913
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x04004212 RID: 16914
	[SerializeField]
	private TMP_Text amountText;
}
