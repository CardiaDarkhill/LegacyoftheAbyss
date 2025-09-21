using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200128A RID: 4746
	public class ShowRelicBoard : FsmStateAction
	{
		// Token: 0x06007CC3 RID: 31939 RVA: 0x00254541 File Offset: 0x00252741
		public override void Reset()
		{
			this.Target = null;
			this.ClosedEvent = null;
		}

		// Token: 0x06007CC4 RID: 31940 RVA: 0x00254554 File Offset: 0x00252754
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				RelicBoardOwner component = safe.GetComponent<RelicBoardOwner>();
				if (component)
				{
					this.board = component.RelicBoard;
					if (this.board)
					{
						this.board.BoardClosed += this.OnBoardClosed;
						this.board.OpenBoard(component);
						return;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06007CC5 RID: 31941 RVA: 0x002545C8 File Offset: 0x002527C8
		public override void OnExit()
		{
			if (this.board)
			{
				this.board.CloseBoard();
				this.board.BoardClosed -= this.OnBoardClosed;
				this.board = null;
			}
		}

		// Token: 0x06007CC6 RID: 31942 RVA: 0x00254600 File Offset: 0x00252800
		private void OnBoardClosed()
		{
			if (this.board)
			{
				this.board.BoardClosed -= this.OnBoardClosed;
				this.board = null;
			}
			base.Fsm.Event(this.ClosedEvent);
		}

		// Token: 0x04007CDD RID: 31965
		[CheckForComponent(typeof(RelicBoardOwner))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007CDE RID: 31966
		public FsmEvent ClosedEvent;

		// Token: 0x04007CDF RID: 31967
		private CollectableRelicBoard board;
	}
}
