using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAF RID: 3247
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForPaneRight : FsmStateAction
	{
		// Token: 0x06006132 RID: 24882 RVA: 0x001EC8CD File Offset: 0x001EAACD
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x06006133 RID: 24883 RVA: 0x001EC8D6 File Offset: 0x001EAAD6
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006134 RID: 24884 RVA: 0x001EC8F4 File Offset: 0x001EAAF4
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.inputHandler.inputActions.PaneRight.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.PaneRight.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.PaneRight.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.PaneRight.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F0F RID: 24335
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F10 RID: 24336
		public FsmEvent wasPressed;

		// Token: 0x04005F11 RID: 24337
		public FsmEvent wasReleased;

		// Token: 0x04005F12 RID: 24338
		public FsmEvent isPressed;

		// Token: 0x04005F13 RID: 24339
		public FsmEvent isNotPressed;

		// Token: 0x04005F14 RID: 24340
		private GameManager gm;

		// Token: 0x04005F15 RID: 24341
		private InputHandler inputHandler;
	}
}
