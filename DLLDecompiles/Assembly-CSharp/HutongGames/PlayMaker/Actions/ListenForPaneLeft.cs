using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAE RID: 3246
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForPaneLeft : FsmStateAction
	{
		// Token: 0x0600612E RID: 24878 RVA: 0x001EC7DE File Offset: 0x001EA9DE
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x0600612F RID: 24879 RVA: 0x001EC7E7 File Offset: 0x001EA9E7
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006130 RID: 24880 RVA: 0x001EC808 File Offset: 0x001EAA08
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.inputHandler.inputActions.PaneLeft.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.PaneLeft.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.PaneLeft.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.PaneLeft.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F08 RID: 24328
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F09 RID: 24329
		public FsmEvent wasPressed;

		// Token: 0x04005F0A RID: 24330
		public FsmEvent wasReleased;

		// Token: 0x04005F0B RID: 24331
		public FsmEvent isPressed;

		// Token: 0x04005F0C RID: 24332
		public FsmEvent isNotPressed;

		// Token: 0x04005F0D RID: 24333
		private GameManager gm;

		// Token: 0x04005F0E RID: 24334
		private InputHandler inputHandler;
	}
}
