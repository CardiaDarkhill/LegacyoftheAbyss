using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	// Token: 0x02000880 RID: 2176
	public abstract class QuestFsmAction : FsmStateAction
	{
		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x06004BF7 RID: 19447 RVA: 0x00166CFC File Offset: 0x00164EFC
		protected virtual bool CustomFinish
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06004BF8 RID: 19448 RVA: 0x00166CFF File Offset: 0x00164EFF
		public override void Reset()
		{
			this.Quest = null;
		}

		// Token: 0x06004BF9 RID: 19449 RVA: 0x00166D08 File Offset: 0x00164F08
		public override void OnEnter()
		{
			if (!this.Quest.IsNone)
			{
				FullQuestBase fullQuestBase = this.Quest.Value as FullQuestBase;
				if (fullQuestBase)
				{
					this.DoQuestAction(fullQuestBase);
				}
				else
				{
					Debug.LogError("Quest object is null!", base.Owner);
				}
			}
			if (!this.CustomFinish)
			{
				base.Finish();
			}
		}

		// Token: 0x06004BFA RID: 19450
		protected abstract void DoQuestAction(FullQuestBase quest);

		// Token: 0x04004D65 RID: 19813
		[ObjectType(typeof(FullQuestBase))]
		public FsmObject Quest;
	}
}
