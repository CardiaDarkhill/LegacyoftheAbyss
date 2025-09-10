using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005AA RID: 1450
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target PlayerData Int")]
public class QuestTargetPlayerDataInt : QuestTargetCounter
{
	// Token: 0x06003426 RID: 13350 RVA: 0x000E83A8 File Offset: 0x000E65A8
	public override bool CanGetMore()
	{
		return true;
	}

	// Token: 0x06003427 RID: 13351 RVA: 0x000E83AB File Offset: 0x000E65AB
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return PlayerData.instance.GetVariable(this.playerDataInt);
	}

	// Token: 0x06003428 RID: 13352 RVA: 0x000E83C0 File Offset: 0x000E65C0
	public override void Get(bool showPopup = true)
	{
		PlayerData instance = PlayerData.instance;
		int value = instance.GetVariable(this.playerDataInt) + 1;
		instance.SetVariable(this.playerDataInt, value);
	}

	// Token: 0x06003429 RID: 13353 RVA: 0x000E83ED File Offset: 0x000E65ED
	public override Sprite GetPopupIcon()
	{
		return this.questCounterSprite;
	}

	// Token: 0x040037B1 RID: 14257
	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string playerDataInt;

	// Token: 0x040037B2 RID: 14258
	[SerializeField]
	private Sprite questCounterSprite;
}
