using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	// Token: 0x02000894 RID: 2196
	[ActionCategory("Quests")]
	public class ShowQuestUpdatedStandalone : FsmStateAction
	{
		// Token: 0x06004C3A RID: 19514 RVA: 0x001674FF File Offset: 0x001656FF
		public override void Reset()
		{
			this.Quest = null;
		}

		// Token: 0x06004C3B RID: 19515 RVA: 0x00167508 File Offset: 0x00165708
		public override void OnEnter()
		{
			if (!this.Quest.IsNone)
			{
				BasicQuestBase basicQuestBase = this.Quest.Value as BasicQuestBase;
				if (basicQuestBase)
				{
					if (basicQuestBase.IsAccepted)
					{
						FullQuestBase fullQuestBase = basicQuestBase as FullQuestBase;
						if (fullQuestBase == null || !fullQuestBase.IsCompleted)
						{
							QuestManager.ShowQuestUpdatedStandalone(basicQuestBase);
						}
					}
				}
				else
				{
					Debug.LogError("Quest object is null!", base.Owner);
				}
			}
			base.Finish();
		}

		// Token: 0x04004D91 RID: 19857
		[ObjectType(typeof(BasicQuestBase))]
		public FsmObject Quest;
	}
}
