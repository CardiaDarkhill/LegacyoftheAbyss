using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAA RID: 3242
	public class ListenForPromptContinue : FsmStateAction
	{
		// Token: 0x06006121 RID: 24865 RVA: 0x001EC5D4 File Offset: 0x001EA7D4
		public override void Reset()
		{
			this.EventTarget = null;
			this.ContinuePressed = null;
		}

		// Token: 0x06006122 RID: 24866 RVA: 0x001EC5E4 File Offset: 0x001EA7E4
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			if (this.gm == null)
			{
				base.LogError("Cannot listen for buttons without game manager.");
				return;
			}
			this.inputHandler = this.gm.inputHandler;
			if (this.inputHandler == null)
			{
				base.LogError("Cannot listen for buttons without input handler.");
			}
		}

		// Token: 0x06006123 RID: 24867 RVA: 0x001EC640 File Offset: 0x001EA840
		public override void OnUpdate()
		{
			if (this.gm == null || this.gm.isPaused || this.inputHandler == null)
			{
				return;
			}
			if (this.inputHandler.WasSkipButtonPressed)
			{
				base.Fsm.Event(this.ContinuePressed);
			}
		}

		// Token: 0x04005EFC RID: 24316
		public FsmEventTarget EventTarget;

		// Token: 0x04005EFD RID: 24317
		public FsmEvent ContinuePressed;

		// Token: 0x04005EFE RID: 24318
		private GameManager gm;

		// Token: 0x04005EFF RID: 24319
		private InputHandler inputHandler;
	}
}
