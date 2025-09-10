using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020006B7 RID: 1719
public class InventorySubMenuButton : InventoryItemSelectableDirectional
{
	// Token: 0x1700071A RID: 1818
	// (get) Token: 0x06003DFC RID: 15868 RVA: 0x00110403 File Offset: 0x0010E603
	public override Color? CursorColor
	{
		get
		{
			return new Color?(this.cursorColor);
		}
	}

	// Token: 0x06003DFD RID: 15869 RVA: 0x00110410 File Offset: 0x0010E610
	protected override void Awake()
	{
		base.Awake();
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		this.invFsm = FSMUtility.LocateFSM(this.paneList.gameObject, "Inventory Control");
		this.invFsmPaneVar = this.invFsm.FsmVariables.FindFsmGameObject("Target Pane");
	}

	// Token: 0x06003DFE RID: 15870 RVA: 0x00110465 File Offset: 0x0010E665
	public override bool Submit()
	{
		this.invFsmPaneVar.Value = this.targetPane.gameObject;
		this.invFsm.SendEvent("FADE TO TARGET");
		return true;
	}

	// Token: 0x04003F9A RID: 16282
	[Space]
	[SerializeField]
	private Color cursorColor;

	// Token: 0x04003F9B RID: 16283
	[SerializeField]
	private InventoryPane targetPane;

	// Token: 0x04003F9C RID: 16284
	private InventoryPaneList paneList;

	// Token: 0x04003F9D RID: 16285
	private PlayMakerFSM invFsm;

	// Token: 0x04003F9E RID: 16286
	private FsmGameObject invFsmPaneVar;
}
