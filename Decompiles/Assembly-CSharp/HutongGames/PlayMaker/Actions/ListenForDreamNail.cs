using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA1 RID: 3233
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDreamNail : FsmStateAction
	{
		// Token: 0x060060FA RID: 24826 RVA: 0x001EB999 File Offset: 0x001E9B99
		public override void Reset()
		{
			this.eventTarget = null;
			this.wasPressed = null;
			this.wasReleased = null;
			this.isPressed = null;
			this.isNotPressed = null;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x060060FB RID: 24827 RVA: 0x001EB9D0 File Offset: 0x001E9BD0
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x060060FC RID: 24828 RVA: 0x001EB9F0 File Offset: 0x001E9BF0
		public override void OnUpdate()
		{
			if (this.gm.isPaused)
			{
				return;
			}
			if (!this.activeBool.Value && !this.activeBool.IsNone)
			{
				return;
			}
			if (this.inputHandler.ForceDreamNailRePress)
			{
				return;
			}
			if (this.inputHandler.inputActions.DreamNail.WasPressed)
			{
				base.Fsm.Event(this.wasPressed);
			}
			if (this.inputHandler.inputActions.DreamNail.WasReleased)
			{
				base.Fsm.Event(this.wasReleased);
			}
			if (this.inputHandler.inputActions.DreamNail.IsPressed)
			{
				base.Fsm.Event(this.isPressed);
			}
			if (!this.inputHandler.inputActions.DreamNail.IsPressed)
			{
				base.Fsm.Event(this.isNotPressed);
			}
		}

		// Token: 0x04005EB0 RID: 24240
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005EB1 RID: 24241
		public FsmEvent wasPressed;

		// Token: 0x04005EB2 RID: 24242
		public FsmEvent wasReleased;

		// Token: 0x04005EB3 RID: 24243
		public FsmEvent isPressed;

		// Token: 0x04005EB4 RID: 24244
		public FsmEvent isNotPressed;

		// Token: 0x04005EB5 RID: 24245
		public FsmBool activeBool;

		// Token: 0x04005EB6 RID: 24246
		private GameManager gm;

		// Token: 0x04005EB7 RID: 24247
		private InputHandler inputHandler;
	}
}
