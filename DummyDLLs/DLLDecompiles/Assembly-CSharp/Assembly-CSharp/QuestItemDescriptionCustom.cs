using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class QuestItemDescriptionCustom : MonoBehaviour
{
	// Token: 0x060033BD RID: 13245 RVA: 0x000E6DD8 File Offset: 0x000E4FD8
	private void OnEnable()
	{
		if (this.targetsList == null)
		{
			this.targetsList = new List<ValueTuple<FullQuestBase.QuestTarget, int>>(this.quest.TargetsAndCounters);
		}
		else
		{
			this.targetsList.AddRange(this.quest.TargetsAndCounters);
		}
		int num = 0;
		int num2 = 0;
		foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple in this.targetsList)
		{
			FullQuestBase.QuestTarget item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			num2 += this.quest.GetCollectedCountOverride(item, item2);
			num += item.Count;
		}
		this.targetsList.Clear();
		this.rangeDisplay.MaxValue = (this.hideMax ? num2 : num);
		this.rangeDisplay.CurrentValue = num2;
	}

	// Token: 0x04003780 RID: 14208
	[SerializeField]
	private FullQuestBase quest;

	// Token: 0x04003781 RID: 14209
	[Space]
	[SerializeField]
	private IconCounter rangeDisplay;

	// Token: 0x04003782 RID: 14210
	[SerializeField]
	private bool hideMax;

	// Token: 0x04003783 RID: 14211
	[TupleElementNames(new string[]
	{
		"target",
		"count"
	})]
	private List<ValueTuple<FullQuestBase.QuestTarget, int>> targetsList;
}
