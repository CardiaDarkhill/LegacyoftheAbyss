using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	// Token: 0x02000895 RID: 2197
	[ActionCategory("Quests")]
	public class QuestConsumeTargetTake : FsmStateAction
	{
		// Token: 0x06004C3D RID: 19517 RVA: 0x0016757B File Offset: 0x0016577B
		public override void Reset()
		{
			this.Quest = null;
			this.TakeDisplay = TakeItemTypes.Taken;
		}

		// Token: 0x06004C3E RID: 19518 RVA: 0x00167598 File Offset: 0x00165798
		public override void OnEnter()
		{
			if (!this.Quest.IsNone)
			{
				FullQuestBase fullQuestBase = this.Quest.Value as FullQuestBase;
				if (fullQuestBase)
				{
					this.DoAction(fullQuestBase);
				}
				else
				{
					Debug.LogError("Quest object is null!", base.Owner);
				}
			}
			base.Finish();
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x001675EC File Offset: 0x001657EC
		private void DoAction(FullQuestBase quest)
		{
			if (!quest.IsAccepted || quest.IsCompleted)
			{
				return;
			}
			foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple in quest.TargetsAndCountersNotHidden)
			{
				FullQuestBase.QuestTarget item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				if (item.Counter)
				{
					item.Counter.Consume(item2, false);
					CollectableUIMsg.ShowTakeMsg(item.Counter, (TakeItemTypes)this.TakeDisplay.Value);
				}
			}
		}

		// Token: 0x04004D92 RID: 19858
		[ObjectType(typeof(FullQuestBase))]
		public FsmObject Quest;

		// Token: 0x04004D93 RID: 19859
		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;
	}
}
