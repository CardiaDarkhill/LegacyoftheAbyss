using System;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x02000729 RID: 1833
public class SimpleShopItemDisplay : MonoBehaviour
{
	// Token: 0x0600417B RID: 16763 RVA: 0x001202C4 File Offset: 0x0011E4C4
	private void OnDisable()
	{
		if (this.titleText)
		{
			this.titleText.transform.localPosition = this.initialTextPos;
		}
	}

	// Token: 0x0600417C RID: 16764 RVA: 0x001202F0 File Offset: 0x0011E4F0
	public void SetItem(ISimpleShopItem item)
	{
		if (!this.isSetup)
		{
			if (this.titleText)
			{
				this.initialTextPos = this.titleText.transform.localPosition;
			}
			this.isSetup = true;
		}
		bool flag = false;
		if (this.itemIcon)
		{
			Sprite icon = item.GetIcon();
			this.itemIcon.sprite = icon;
			flag = icon;
		}
		if (this.titleText)
		{
			this.titleText.text = item.GetDisplayName();
			if (flag)
			{
				this.titleText.transform.SetLocalPosition2D(this.initialTextPos + this.textOffsetWithIcon);
			}
			else
			{
				this.titleText.transform.SetLocalPosition2D(this.initialTextPos);
			}
		}
		this.cost = item.GetCost();
		if (this.cost > 0)
		{
			if (this.costGroup)
			{
				this.costGroup.SetActive(true);
			}
			if (this.costText)
			{
				this.costText.text = this.cost.ToString();
			}
		}
		else if (this.costGroup)
		{
			this.costGroup.SetActive(false);
		}
		this.RefreshCostOpacity();
	}

	// Token: 0x0600417D RID: 16765 RVA: 0x00120429 File Offset: 0x0011E629
	public void RefreshCostOpacity()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = ((PlayerData.instance.geo < this.cost) ? this.disabledOpacity : 1f);
		}
	}

	// Token: 0x040042F9 RID: 17145
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x040042FA RID: 17146
	[SerializeField]
	private SpriteRenderer itemIcon;

	// Token: 0x040042FB RID: 17147
	[SerializeField]
	private Vector2 textOffsetWithIcon;

	// Token: 0x040042FC RID: 17148
	[SerializeField]
	private GameObject costGroup;

	// Token: 0x040042FD RID: 17149
	[SerializeField]
	private TMP_Text costText;

	// Token: 0x040042FE RID: 17150
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x040042FF RID: 17151
	[SerializeField]
	private float disabledOpacity = 0.5f;

	// Token: 0x04004300 RID: 17152
	private bool isSetup;

	// Token: 0x04004301 RID: 17153
	private Vector2 initialTextPos;

	// Token: 0x04004302 RID: 17154
	private int cost;
}
