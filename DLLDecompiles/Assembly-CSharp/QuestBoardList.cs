using System;
using UnityEngine;

// Token: 0x02000597 RID: 1431
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Board List")]
public class QuestBoardList : NamedScriptableObjectList<QuestGroupBase>
{
	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x0600337E RID: 13182 RVA: 0x000E5608 File Offset: 0x000E3808
	public int CompletedQuestCount
	{
		get
		{
			int num = 0;
			foreach (QuestGroupBase questGroupBase in this)
			{
				if (questGroupBase)
				{
					foreach (BasicQuestBase basicQuestBase in questGroupBase.GetQuests())
					{
						IQuestWithCompletion questWithCompletion = basicQuestBase as IQuestWithCompletion;
						if (questWithCompletion != null && questWithCompletion.IsCompleted)
						{
							num++;
						}
					}
				}
			}
			return num;
		}
	}
}
