using System;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class InventoryPaneListItem : MonoBehaviour
{
	// Token: 0x17000717 RID: 1815
	// (get) Token: 0x06003DCF RID: 15823 RVA: 0x0010F9F9 File Offset: 0x0010DBF9
	// (set) Token: 0x06003DD0 RID: 15824 RVA: 0x0010FA19 File Offset: 0x0010DC19
	public float Alpha
	{
		get
		{
			if (!this.group)
			{
				return 0f;
			}
			return this.group.AlphaSelf;
		}
		set
		{
			if (this.group)
			{
				this.group.AlphaSelf = value;
			}
		}
	}

	// Token: 0x06003DD1 RID: 15825 RVA: 0x0010FA34 File Offset: 0x0010DC34
	private void Awake()
	{
		if (this.newOrb)
		{
			this.newOrbInitialScale = this.newOrb.transform.localScale;
		}
	}

	// Token: 0x06003DD2 RID: 15826 RVA: 0x0010FA5C File Offset: 0x0010DC5C
	public void UpdateValues(InventoryPane pane, bool isSelected)
	{
		if (this.currentPane)
		{
			this.currentPane.NewItemsUpdated -= this.OnNewItemsUpdated;
		}
		this.currentPane = pane;
		this.icon.sprite = pane.ListIcon;
		this.icon.color = (isSelected ? this.selectedColor : this.normalColor);
		float num = isSelected ? this.selectedScale : 1f;
		base.transform.ScaleTo(this, new Vector3(num, num, 1f), this.scaleDuration, 0f, false, true, null);
		pane.NewItemsUpdated += this.OnNewItemsUpdated;
		if (this.newOrb)
		{
			if (pane.IsAnyUpdates)
			{
				this.newOrb.SetActive(true);
				this.newOrb.transform.localScale = this.newOrbInitialScale;
				return;
			}
			this.newOrb.SetActive(false);
		}
	}

	// Token: 0x06003DD3 RID: 15827 RVA: 0x0010FB50 File Offset: 0x0010DD50
	private void OnNewItemsUpdated(bool isAnyNewItems)
	{
		if (!this.newOrb)
		{
			return;
		}
		if (isAnyNewItems)
		{
			this.newOrb.SetActive(true);
			return;
		}
		this.newOrb.transform.ScaleTo(this, Vector3.zero, UI.NewDotScaleTime, UI.NewDotScaleDelay, false, true, null);
	}

	// Token: 0x04003F6E RID: 16238
	[SerializeField]
	private NestedFadeGroupBase group;

	// Token: 0x04003F6F RID: 16239
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x04003F70 RID: 16240
	[SerializeField]
	private Color selectedColor;

	// Token: 0x04003F71 RID: 16241
	[SerializeField]
	private Color normalColor;

	// Token: 0x04003F72 RID: 16242
	[SerializeField]
	private float selectedScale;

	// Token: 0x04003F73 RID: 16243
	[SerializeField]
	private float scaleDuration;

	// Token: 0x04003F74 RID: 16244
	[SerializeField]
	private GameObject newOrb;

	// Token: 0x04003F75 RID: 16245
	private InventoryPane currentPane;

	// Token: 0x04003F76 RID: 16246
	private Vector3 newOrbInitialScale;
}
