using System;
using HutongGames.PlayMaker;

// Token: 0x020006DF RID: 1759
[ActionCategory("Hollow Knight")]
public class SetMenuButtonIconAction : FsmStateAction
{
	// Token: 0x06003F70 RID: 16240 RVA: 0x00118041 File Offset: 0x00116241
	public override void Reset()
	{
		this.target = null;
		this.menuAction = null;
	}

	// Token: 0x06003F71 RID: 16241 RVA: 0x00118054 File Offset: 0x00116254
	public override void OnEnter()
	{
		if (this.target.Value)
		{
			MenuButtonIcon componentInChildren = this.target.Value.GetComponentInChildren<MenuButtonIcon>();
			if (componentInChildren)
			{
				componentInChildren.menuAction = (Platform.MenuActions)this.menuAction.Value;
				componentInChildren.RefreshButtonIcon();
			}
		}
		base.Finish();
	}

	// Token: 0x04004130 RID: 16688
	public FsmGameObject target;

	// Token: 0x04004131 RID: 16689
	[ObjectType(typeof(Platform.MenuActions))]
	public FsmEnum menuAction;
}
