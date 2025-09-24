using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000598 RID: 1432
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Complete Total Group")]
public class QuestCompleteTotalGroup : ScriptableObject
{
	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x06003380 RID: 13184 RVA: 0x000E56A8 File Offset: 0x000E38A8
	public bool IsFulfilled
	{
		get
		{
			if (!this.additionalTest.IsFulfilled)
			{
				return false;
			}
			foreach (QuestCompleteTotalGroup.CompleteQuest completeQuest in this.Quests)
			{
				if (!completeQuest.Quest)
				{
					Debug.LogError("Skipping null quest", this);
				}
				else if (completeQuest.IsRequired && !completeQuest.Quest.IsCompleted)
				{
					return false;
				}
			}
			return this.CurrentValueCount >= this.target;
		}
	}

	// Token: 0x170005A6 RID: 1446
	// (get) Token: 0x06003381 RID: 13185 RVA: 0x000E5744 File Offset: 0x000E3944
	public float CurrentValueCount
	{
		get
		{
			float num = 0f;
			foreach (QuestCompleteTotalGroup.CompleteQuest completeQuest in this.Quests)
			{
				if (completeQuest.Quest.IsCompleted)
				{
					num += completeQuest.Value;
				}
			}
			return num;
		}
	}

	// Token: 0x170005A7 RID: 1447
	// (get) Token: 0x06003382 RID: 13186 RVA: 0x000E57A8 File Offset: 0x000E39A8
	public IEnumerable<QuestCompleteTotalGroup.CompleteQuest> Quests
	{
		get
		{
			return from c in this.quests
			where c.Quest
			select c;
		}
	}

	// Token: 0x06003383 RID: 13187 RVA: 0x000E57D4 File Offset: 0x000E39D4
	private void OnValidate()
	{
		for (int i = 0; i < this.quests.Length; i++)
		{
			QuestCompleteTotalGroup.CompleteQuest completeQuest = this.quests[i];
			if (completeQuest.Value < 0f)
			{
				completeQuest.Value = 0f;
				this.quests[i] = completeQuest;
			}
		}
	}

	// Token: 0x04003743 RID: 14147
	[SerializeField]
	private QuestCompleteTotalGroup.CompleteQuest[] quests;

	// Token: 0x04003744 RID: 14148
	[SerializeField]
	private float target;

	// Token: 0x04003745 RID: 14149
	[Space]
	[SerializeField]
	private PlayerDataTest additionalTest;

	// Token: 0x020018BE RID: 6334
	[Serializable]
	public struct CompleteQuest
	{
		// Token: 0x04009335 RID: 37685
		public FullQuestBase Quest;

		// Token: 0x04009336 RID: 37686
		public float Value;

		// Token: 0x04009337 RID: 37687
		public bool IsRequired;
	}
}
