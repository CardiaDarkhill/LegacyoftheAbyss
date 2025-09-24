using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	// Token: 0x02000890 RID: 2192
	[ActionCategory("Quests")]
	public class ShowQuestBoard : FsmStateAction
	{
		// Token: 0x06004C2D RID: 19501 RVA: 0x00167305 File Offset: 0x00165505
		public override void Reset()
		{
			this.Target = null;
			this.AcceptedEvent = null;
			this.CanceledEvent = null;
			this.AcceptedQuests = null;
		}

		// Token: 0x06004C2E RID: 19502 RVA: 0x00167324 File Offset: 0x00165524
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.board = safe.GetComponent<QuestItemBoard>();
				if (this.board)
				{
					this.board.BoardClosed += this.OnBoardClosed;
					this.board.OpenPane();
					return;
				}
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x06004C2F RID: 19503 RVA: 0x00167388 File Offset: 0x00165588
		private void OnBoardClosed(List<BasicQuestBase> quests)
		{
			this.board.BoardClosed -= this.OnBoardClosed;
			if (quests != null && quests.Count > 0)
			{
				if (!this.AcceptedQuests.IsNone)
				{
					FsmArray acceptedQuests = this.AcceptedQuests;
					object[] values = quests.ToArray();
					acceptedQuests.Values = values;
				}
				base.Fsm.Event(this.AcceptedEvent);
				return;
			}
			base.Fsm.Event(this.CanceledEvent);
		}

		// Token: 0x04004D85 RID: 19845
		public FsmOwnerDefault Target;

		// Token: 0x04004D86 RID: 19846
		public FsmEvent CanceledEvent;

		// Token: 0x04004D87 RID: 19847
		public FsmEvent AcceptedEvent;

		// Token: 0x04004D88 RID: 19848
		[ArrayEditor(typeof(Quest), "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray AcceptedQuests;

		// Token: 0x04004D89 RID: 19849
		private QuestItemBoard board;
	}
}
