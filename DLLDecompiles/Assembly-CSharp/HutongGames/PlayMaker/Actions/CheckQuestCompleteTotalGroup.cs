using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200131B RID: 4891
	public class CheckQuestCompleteTotalGroup : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007EE4 RID: 32484 RVA: 0x0025A103 File Offset: 0x00258303
		public override void Reset()
		{
			base.Reset();
			this.TotalGroup = null;
		}

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x06007EE5 RID: 32485 RVA: 0x0025A114 File Offset: 0x00258314
		public override bool IsTrue
		{
			get
			{
				QuestCompleteTotalGroup questCompleteTotalGroup = this.TotalGroup.Value as QuestCompleteTotalGroup;
				return questCompleteTotalGroup && questCompleteTotalGroup.IsFulfilled;
			}
		}

		// Token: 0x04007E8D RID: 32397
		[RequiredField]
		[ObjectType(typeof(QuestCompleteTotalGroup))]
		public FsmObject TotalGroup;
	}
}
