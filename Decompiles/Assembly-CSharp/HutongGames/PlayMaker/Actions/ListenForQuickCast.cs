using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB0 RID: 3248
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForQuickCast : FsmStateAction
	{
		// Token: 0x06006136 RID: 24886 RVA: 0x001EC9B9 File Offset: 0x001EABB9
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x06006137 RID: 24887 RVA: 0x001EC9C2 File Offset: 0x001EABC2
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006138 RID: 24888 RVA: 0x001EC9E0 File Offset: 0x001EABE0
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.inputHandler.inputActions.QuickCast.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.QuickCast.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.QuickCast.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.QuickCast.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F16 RID: 24342
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F17 RID: 24343
		public FsmEvent wasPressed;

		// Token: 0x04005F18 RID: 24344
		public FsmEvent wasReleased;

		// Token: 0x04005F19 RID: 24345
		public FsmEvent isPressed;

		// Token: 0x04005F1A RID: 24346
		public FsmEvent isNotPressed;

		// Token: 0x04005F1B RID: 24347
		private GameManager gm;

		// Token: 0x04005F1C RID: 24348
		private InputHandler inputHandler;
	}
}
