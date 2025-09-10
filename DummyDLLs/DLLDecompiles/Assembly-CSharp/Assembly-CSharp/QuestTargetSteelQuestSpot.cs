using System;
using UnityEngine;

// Token: 0x020005AB RID: 1451
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Steel Quest Spot")]
public class QuestTargetSteelQuestSpot : QuestTargetCounter
{
	// Token: 0x0600342B RID: 13355 RVA: 0x000E83FD File Offset: 0x000E65FD
	public override bool CanGetMore()
	{
		SteelSoulQuestSpot.Spot spot = QuestTargetSteelQuestSpot.GetSpot();
		return spot == null || !spot.IsSeen;
	}

	// Token: 0x0600342C RID: 13356 RVA: 0x000E8414 File Offset: 0x000E6614
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		SteelSoulQuestSpot.Spot[] steelQuestSpots = PlayerData.instance.SteelQuestSpots;
		if (steelQuestSpots == null)
		{
			return 0;
		}
		int num = 0;
		SteelSoulQuestSpot.Spot[] array = steelQuestSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsSeen)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600342D RID: 13357 RVA: 0x000E8452 File Offset: 0x000E6652
	public override string GetPopupName()
	{
		return string.Empty;
	}

	// Token: 0x0600342E RID: 13358 RVA: 0x000E8459 File Offset: 0x000E6659
	public override Sprite GetPopupIcon()
	{
		return this.icon;
	}

	// Token: 0x0600342F RID: 13359 RVA: 0x000E8464 File Offset: 0x000E6664
	private static SteelSoulQuestSpot.Spot GetSpot()
	{
		PlayerData instance = PlayerData.instance;
		SteelSoulQuestSpot.Spot[] steelQuestSpots = instance.SteelQuestSpots;
		if (steelQuestSpots == null)
		{
			return null;
		}
		string sceneNameString = GameManager.instance.GetSceneNameString();
		for (int i = 0; i < steelQuestSpots.Length; i++)
		{
			if (!(steelQuestSpots[i].SceneName != sceneNameString))
			{
				return instance.SteelQuestSpots[i];
			}
		}
		return null;
	}

	// Token: 0x040037B3 RID: 14259
	[SerializeField]
	private QuestTargetSteelQuestSpot.SpotInfo[] spotInfos;

	// Token: 0x040037B4 RID: 14260
	[Space]
	[SerializeField]
	private Sprite icon;

	// Token: 0x020018D1 RID: 6353
	[Serializable]
	private struct SpotInfo
	{
		// Token: 0x0400936C RID: 37740
		public string SceneName;
	}
}
