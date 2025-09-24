using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001CC RID: 460
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (PlayerData Bool)")]
public class PlayerDataBoolCollectable : FakeCollectable
{
	// Token: 0x060011FE RID: 4606 RVA: 0x00053CD1 File Offset: 0x00051ED1
	public override bool CanGetMore()
	{
		return !PlayerData.instance.GetVariable(this.boolName);
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x00053CE6 File Offset: 0x00051EE6
	public override void Get(bool showPopup = true)
	{
		base.Get(showPopup);
		PlayerData.instance.SetVariable(this.boolName, true);
		CollectableItemManager.IncrementVersion();
	}

	// Token: 0x040010CF RID: 4303
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string boolName;
}
