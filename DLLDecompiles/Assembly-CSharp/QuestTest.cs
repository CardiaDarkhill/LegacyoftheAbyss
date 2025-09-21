using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005AC RID: 1452
[Serializable]
public struct QuestTest
{
	// Token: 0x170005BF RID: 1471
	// (get) Token: 0x06003431 RID: 13361 RVA: 0x000E84C0 File Offset: 0x000E66C0
	public bool IsFulfilled
	{
		get
		{
			if (!this.Quest)
			{
				Debug.LogError("Quest test has a null quest!");
				return false;
			}
			if (this.CheckAvailable && this.Quest.IsAvailable != this.IsAvailable)
			{
				return false;
			}
			if (this.CheckAccepted && this.Quest.IsAccepted != this.IsAccepted)
			{
				return false;
			}
			if (this.CheckCompletedAmount)
			{
				Debug.LogWarning("Checking completed amount for " + this.Quest.name + ". NOTE: this will check all quest targets and fail if any are below the desired amount (legacy behaviour)");
				int completedAmount = this.CompletedAmount;
				using (IEnumerator<int> enumerator = this.Quest.Counters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current < completedAmount)
						{
							return false;
						}
					}
				}
			}
			return (!this.CheckCompletable || this.Quest.CanComplete == this.IsCompletable) && (!this.CheckCompleted || this.Quest.IsCompleted == this.IsCompleted) && (!this.CheckWasEverCompleted || this.Quest.WasEverCompleted == this.WasEverCompleted);
		}
	}

	// Token: 0x040037B5 RID: 14261
	public FullQuestBase Quest;

	// Token: 0x040037B6 RID: 14262
	[Space]
	public bool CheckAvailable;

	// Token: 0x040037B7 RID: 14263
	[ModifiableProperty]
	[Conditional("CheckAvailable", true, false, false)]
	public bool IsAvailable;

	// Token: 0x040037B8 RID: 14264
	[Space]
	public bool CheckAccepted;

	// Token: 0x040037B9 RID: 14265
	[ModifiableProperty]
	[Conditional("CheckAccepted", true, false, false)]
	public bool IsAccepted;

	// Token: 0x040037BA RID: 14266
	[Space]
	public bool CheckCompletedAmount;

	// Token: 0x040037BB RID: 14267
	[ModifiableProperty]
	[Conditional("CheckCompletedAmount", true, false, false)]
	public int CompletedAmount;

	// Token: 0x040037BC RID: 14268
	[Space]
	public bool CheckCompletable;

	// Token: 0x040037BD RID: 14269
	[ModifiableProperty]
	[Conditional("CheckCompletable", true, false, false)]
	public bool IsCompletable;

	// Token: 0x040037BE RID: 14270
	[Space]
	public bool CheckCompleted;

	// Token: 0x040037BF RID: 14271
	[ModifiableProperty]
	[Conditional("CheckCompleted", true, false, false)]
	public bool IsCompleted;

	// Token: 0x040037C0 RID: 14272
	[Space]
	public bool CheckWasEverCompleted;

	// Token: 0x040037C1 RID: 14273
	[ModifiableProperty]
	[Conditional("CheckWasEverCompleted", true, false, false)]
	public bool WasEverCompleted;
}
