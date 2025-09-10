using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001CD RID: 461
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (PlayerData)")]
public class PlayerDataCollectable : FakeCollectable
{
	// Token: 0x06001201 RID: 4609 RVA: 0x00053D10 File Offset: 0x00051F10
	public override void Get(bool showPopup = true)
	{
		base.Get(showPopup);
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(this.linkedPDBool))
		{
			instance.SetVariable(this.linkedPDBool, true);
		}
		if (!string.IsNullOrEmpty(this.linkedPDInt))
		{
			int num = instance.GetVariable(this.linkedPDInt);
			num++;
			instance.SetVariable(this.linkedPDInt, num);
		}
		foreach (PlayerDataBoolOperation playerDataBoolOperation in this.setPlayerDataBools)
		{
			playerDataBoolOperation.Execute();
		}
		foreach (PlayerDataIntOperation playerDataIntOperation in this.setPlayerDataInts)
		{
			playerDataIntOperation.Execute();
		}
		if (this.isMap)
		{
			GameManager instance2 = GameManager.instance;
			instance2.UpdateGameMapWithPopup(1f);
			instance2.CheckMapAchievements();
		}
		CollectableItemManager.IncrementVersion();
		if (this.isToolPouch)
		{
			ToolItemManager.SendEquippedChangedEvent(true);
		}
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x00053DF0 File Offset: 0x00051FF0
	public override int GetSavedAmount()
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(this.linkedPDInt))
		{
			return instance.GetVariable(this.linkedPDInt);
		}
		if (!string.IsNullOrEmpty(this.linkedPDBool) && instance.GetVariable(this.linkedPDBool))
		{
			return 1;
		}
		return base.GetSavedAmount();
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x00053E40 File Offset: 0x00052040
	public override bool CanGetMore()
	{
		if (!string.IsNullOrEmpty(this.linkedPDBool))
		{
			return !PlayerData.instance.GetVariable(this.linkedPDBool);
		}
		return base.CanGetMore();
	}

	// Token: 0x040010D0 RID: 4304
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string linkedPDBool;

	// Token: 0x040010D1 RID: 4305
	[SerializeField]
	[PlayerDataField(typeof(int), false)]
	private string linkedPDInt;

	// Token: 0x040010D2 RID: 4306
	[SerializeField]
	private PlayerDataBoolOperation[] setPlayerDataBools;

	// Token: 0x040010D3 RID: 4307
	[SerializeField]
	private PlayerDataIntOperation[] setPlayerDataInts;

	// Token: 0x040010D4 RID: 4308
	[Space]
	[SerializeField]
	private bool isMap;

	// Token: 0x040010D5 RID: 4309
	[SerializeField]
	private bool isToolPouch;
}
