using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC6 RID: 3014
	[ActionCategory("Game Manager")]
	[Tooltip("Perform a generic scene transition.")]
	public class CheckGameState : FsmStateAction
	{
		// Token: 0x06005CA2 RID: 23714 RVA: 0x001D2972 File Offset: 0x001D0B72
		public override void Reset()
		{
			this.playing = null;
			this.otherwise = null;
		}

		// Token: 0x06005CA3 RID: 23715 RVA: 0x001D2982 File Offset: 0x001D0B82
		public override void OnEnter()
		{
			this.Check();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005CA4 RID: 23716 RVA: 0x001D2998 File Offset: 0x001D0B98
		private void Check()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				base.Fsm.Event(this.otherwise);
				base.LogError("Cannot CheckGameState() before the game manager is loaded.");
				return;
			}
			if (unsafeInstance.GameState == GameState.PLAYING)
			{
				base.Fsm.Event(this.playing);
				return;
			}
			base.Fsm.Event(this.otherwise);
		}

		// Token: 0x06005CA5 RID: 23717 RVA: 0x001D29FD File Offset: 0x001D0BFD
		public override void OnUpdate()
		{
			base.OnUpdate();
			this.Check();
		}

		// Token: 0x0400583B RID: 22587
		public FsmEvent playing;

		// Token: 0x0400583C RID: 22588
		public FsmEvent otherwise;

		// Token: 0x0400583D RID: 22589
		public bool everyFrame;
	}
}
