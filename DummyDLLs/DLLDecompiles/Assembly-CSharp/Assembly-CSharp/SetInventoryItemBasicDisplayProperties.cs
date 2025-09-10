using System;
using HutongGames.PlayMaker;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000683 RID: 1667
public class SetInventoryItemBasicDisplayProperties : FsmStateAction
{
	// Token: 0x06003B9C RID: 15260 RVA: 0x0010641B File Offset: 0x0010461B
	public override void Reset()
	{
		this.Target = null;
		this.DisplayNameKey = null;
		this.DisplayNameSheet = null;
		this.DescriptionSheet = null;
		this.DescriptionKey = null;
	}

	// Token: 0x06003B9D RID: 15261 RVA: 0x00106440 File Offset: 0x00104640
	public override void OnEnter()
	{
		GameObject safe = this.Target.GetSafe(this);
		if (safe)
		{
			InventoryItemBasic component = safe.GetComponent<InventoryItemBasic>();
			if (component)
			{
				component.SetDisplayProperties(new LocalisedString
				{
					Sheet = this.DisplayNameSheet.Value,
					Key = this.DisplayNameKey.Value
				}, new LocalisedString
				{
					Sheet = this.DescriptionSheet.Value,
					Key = this.DescriptionKey.Value
				});
			}
		}
		base.Finish();
	}

	// Token: 0x04003DCD RID: 15821
	public FsmOwnerDefault Target;

	// Token: 0x04003DCE RID: 15822
	public FsmString DisplayNameSheet;

	// Token: 0x04003DCF RID: 15823
	public FsmString DisplayNameKey;

	// Token: 0x04003DD0 RID: 15824
	public FsmString DescriptionSheet;

	// Token: 0x04003DD1 RID: 15825
	public FsmString DescriptionKey;
}
