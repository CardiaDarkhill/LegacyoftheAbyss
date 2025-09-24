using System;
using System.Collections.Generic;

// Token: 0x0200059B RID: 1435
public abstract class QuestGroupBase : SavedItem
{
	// Token: 0x0600338E RID: 13198
	public abstract IEnumerable<BasicQuestBase> GetQuests();
}
