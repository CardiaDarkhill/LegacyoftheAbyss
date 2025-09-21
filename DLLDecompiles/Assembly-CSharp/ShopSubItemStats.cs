using System;
using UnityEngine;

// Token: 0x02000725 RID: 1829
public class ShopSubItemStats : MonoBehaviour
{
	// Token: 0x06004134 RID: 16692 RVA: 0x0011E3BE File Offset: 0x0011C5BE
	public void Setup(ShopItem.SubItem item)
	{
		this.icon.sprite = item.Icon;
	}

	// Token: 0x04004298 RID: 17048
	[SerializeField]
	private SpriteRenderer icon;
}
