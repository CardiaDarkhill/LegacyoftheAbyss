using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200131C RID: 4892
	public class EvaluateQuestGroup : FsmStateAction
	{
		// Token: 0x06007EE7 RID: 32487 RVA: 0x0025A14A File Offset: 0x0025834A
		public override void Reset()
		{
			this.QuestGroup = null;
			this.StoreQuest = null;
			this.StoreIndex = null;
		}

		// Token: 0x06007EE8 RID: 32488 RVA: 0x0025A164 File Offset: 0x00258364
		public override void OnEnter()
		{
			QuestGroup questGroup = this.QuestGroup.Value as QuestGroup;
			if (questGroup != null)
			{
				FullQuestBase value;
				int value2;
				questGroup.Evaluate(out value, out value2);
				this.StoreQuest.Value = value;
				this.StoreIndex.Value = value2;
			}
			else
			{
				this.StoreQuest.Value = null;
				this.StoreIndex.Value = -1;
			}
			base.Finish();
		}

		// Token: 0x04007E8E RID: 32398
		[ObjectType(typeof(QuestGroup))]
		public FsmObject QuestGroup;

		// Token: 0x04007E8F RID: 32399
		[ObjectType(typeof(FullQuestBase))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreQuest;

		// Token: 0x04007E90 RID: 32400
		[UIHint(UIHint.Variable)]
		public FsmInt StoreIndex;
	}
}
