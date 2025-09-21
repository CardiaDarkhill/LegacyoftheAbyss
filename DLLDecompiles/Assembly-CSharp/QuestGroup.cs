using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200059A RID: 1434
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Group")]
public class QuestGroup : QuestGroupBase
{
	// Token: 0x06003388 RID: 13192 RVA: 0x000E587E File Offset: 0x000E3A7E
	public override bool CanGetMore()
	{
		Debug.LogError("CanGetMore() is not valid for this item", this);
		return false;
	}

	// Token: 0x06003389 RID: 13193 RVA: 0x000E588C File Offset: 0x000E3A8C
	public override void Get(bool showPopup = true)
	{
		Debug.LogError("Get() is not valid for this item", this);
	}

	// Token: 0x0600338A RID: 13194 RVA: 0x000E5899 File Offset: 0x000E3A99
	public override IEnumerable<BasicQuestBase> GetQuests()
	{
		return this.quests;
	}

	// Token: 0x0600338B RID: 13195 RVA: 0x000E58A1 File Offset: 0x000E3AA1
	public IEnumerable<FullQuestBase> GetFullQuests()
	{
		return this.GetQuests().OfType<FullQuestBase>();
	}

	// Token: 0x0600338C RID: 13196 RVA: 0x000E58B0 File Offset: 0x000E3AB0
	public void Evaluate(out FullQuestBase quest, out int index)
	{
		quest = null;
		index = -1;
		if (this.quests == null || this.quests.Count == 0)
		{
			return;
		}
		List<FullQuestBase> source = (from q in this.GetFullQuests()
		where (q.IsAvailable || q.IsAccepted) && !q.IsCompleted
		select q).ToList<FullQuestBase>();
		quest = source.FirstOrDefault((FullQuestBase q) => q.CanComplete);
		if (quest == null)
		{
			quest = source.FirstOrDefault((FullQuestBase q) => !q.IsAccepted);
		}
		if (quest == null)
		{
			quest = source.FirstOrDefault((FullQuestBase q) => q.IsAccepted);
		}
		if (quest != null)
		{
			index = this.quests.IndexOf(quest);
		}
	}

	// Token: 0x04003746 RID: 14150
	[SerializeField]
	private List<BasicQuestBase> quests;
}
