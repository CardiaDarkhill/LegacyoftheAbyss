using System;
using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200131D RID: 4893
	public class GetQuestGroupInfo : FsmStateAction
	{
		// Token: 0x06007EEA RID: 32490 RVA: 0x0025A1D5 File Offset: 0x002583D5
		public override void Reset()
		{
			this.QuestGroup = null;
			this.StoreCompleteCount = null;
			this.StoreTotalCount = null;
		}

		// Token: 0x06007EEB RID: 32491 RVA: 0x0025A1EC File Offset: 0x002583EC
		public override void OnEnter()
		{
			QuestGroup questGroup = this.QuestGroup.Value as QuestGroup;
			if (questGroup != null)
			{
				IEnumerable<FullQuestBase> fullQuests = questGroup.GetFullQuests();
				this.StoreCompleteCount.Value = fullQuests.Count((FullQuestBase quest) => quest.IsCompleted);
				this.StoreTotalCount.Value = fullQuests.Count<FullQuestBase>();
			}
			else
			{
				this.StoreCompleteCount.Value = 0;
				this.StoreTotalCount.Value = 0;
			}
			base.Finish();
		}

		// Token: 0x04007E91 RID: 32401
		[ObjectType(typeof(QuestGroup))]
		public FsmObject QuestGroup;

		// Token: 0x04007E92 RID: 32402
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCompleteCount;

		// Token: 0x04007E93 RID: 32403
		[UIHint(UIHint.Variable)]
		public FsmInt StoreTotalCount;
	}
}
