using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB6 RID: 3254
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForRsUp : FsmStateAction
	{
		// Token: 0x0600614F RID: 24911 RVA: 0x001ED280 File Offset: 0x001EB480
		public override void Reset()
		{
			this.eventTarget = null;
			this.wasPressed = null;
			this.wasReleased = null;
			this.isPressed = null;
			this.isNotPressed = null;
			this.isPressedBool = new FsmBool
			{
				UseVariable = true
			};
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06006150 RID: 24912 RVA: 0x001ED2D4 File Offset: 0x001EB4D4
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006151 RID: 24913 RVA: 0x001ED2F4 File Offset: 0x001EB4F4
		public override void OnUpdate()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.RsUp.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.RsUp.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.RsUp.IsPressed)
				{
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = true;
					}
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.RsUp.IsPressed)
				{
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = false;
					}
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F4F RID: 24399
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F50 RID: 24400
		public FsmEvent wasPressed;

		// Token: 0x04005F51 RID: 24401
		public FsmEvent wasReleased;

		// Token: 0x04005F52 RID: 24402
		public FsmEvent isPressed;

		// Token: 0x04005F53 RID: 24403
		public FsmEvent isNotPressed;

		// Token: 0x04005F54 RID: 24404
		public FsmBool isPressedBool;

		// Token: 0x04005F55 RID: 24405
		public FsmBool activeBool;

		// Token: 0x04005F56 RID: 24406
		private GameManager gm;

		// Token: 0x04005F57 RID: 24407
		private InputHandler inputHandler;
	}
}
