using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000682 RID: 1666
public class InventoryItemBasic : InventoryItemUpdateable
{
	// Token: 0x170006B9 RID: 1721
	// (get) Token: 0x06003B96 RID: 15254 RVA: 0x001063A7 File Offset: 0x001045A7
	public override string DisplayName
	{
		get
		{
			return this.displayName;
		}
	}

	// Token: 0x170006BA RID: 1722
	// (get) Token: 0x06003B97 RID: 15255 RVA: 0x001063B4 File Offset: 0x001045B4
	public override string Description
	{
		get
		{
			return this.description;
		}
	}

	// Token: 0x170006BB RID: 1723
	// (get) Token: 0x06003B98 RID: 15256 RVA: 0x001063C1 File Offset: 0x001045C1
	// (set) Token: 0x06003B99 RID: 15257 RVA: 0x001063E2 File Offset: 0x001045E2
	protected override bool IsSeen
	{
		get
		{
			return string.IsNullOrEmpty(this.hasSeenPdBool) || PlayerData.instance.GetVariable(this.hasSeenPdBool);
		}
		set
		{
			if (string.IsNullOrEmpty(this.hasSeenPdBool))
			{
				return;
			}
			PlayerData.instance.SetVariable(this.hasSeenPdBool, value);
		}
	}

	// Token: 0x06003B9A RID: 15258 RVA: 0x00106403 File Offset: 0x00104603
	public void SetDisplayProperties(LocalisedString newDisplayName, LocalisedString newDescription)
	{
		this.displayName = newDisplayName;
		this.description = newDescription;
	}

	// Token: 0x04003DCA RID: 15818
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string hasSeenPdBool;

	// Token: 0x04003DCB RID: 15819
	[Space]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x04003DCC RID: 15820
	[SerializeField]
	private LocalisedString description;
}
