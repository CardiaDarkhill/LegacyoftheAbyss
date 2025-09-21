using System;
using UnityEngine;

// Token: 0x02000595 RID: 1429
[CreateAssetMenu(menuName = "Hornet/Quests/Quest")]
public class Quest : FullQuestBase
{
	// Token: 0x1700059F RID: 1439
	// (get) Token: 0x06003364 RID: 13156 RVA: 0x000E4A75 File Offset: 0x000E2C75
	public override QuestType QuestType
	{
		get
		{
			return this.questType;
		}
	}

	// Token: 0x04003730 RID: 14128
	[Header("Quest")]
	[SerializeField]
	private QuestType questType;
}
