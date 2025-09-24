using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB9 RID: 3257
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSuperdash : FsmStateAction
	{
		// Token: 0x0600615D RID: 24925 RVA: 0x001ED81D File Offset: 0x001EBA1D
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x0600615E RID: 24926 RVA: 0x001ED826 File Offset: 0x001EBA26
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x0600615F RID: 24927 RVA: 0x001ED844 File Offset: 0x001EBA44
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.inputHandler.inputActions.SuperDash.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.SuperDash.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.SuperDash.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.SuperDash.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F6F RID: 24431
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F70 RID: 24432
		public FsmEvent wasPressed;

		// Token: 0x04005F71 RID: 24433
		public FsmEvent wasReleased;

		// Token: 0x04005F72 RID: 24434
		public FsmEvent isPressed;

		// Token: 0x04005F73 RID: 24435
		public FsmEvent isNotPressed;

		// Token: 0x04005F74 RID: 24436
		private GameManager gm;

		// Token: 0x04005F75 RID: 24437
		private InputHandler inputHandler;
	}
}
