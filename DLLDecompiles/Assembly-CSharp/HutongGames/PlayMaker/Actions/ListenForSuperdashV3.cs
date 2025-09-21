using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBB RID: 3259
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSuperdashV3 : FsmStateAction
	{
		// Token: 0x06006165 RID: 24933 RVA: 0x001EDA3E File Offset: 0x001EBC3E
		public override void Reset()
		{
			this.Target = null;
			this.WasPressed = null;
			this.WasReleased = null;
			this.IsPressed = null;
			this.IsNotPressed = null;
			this.ActiveBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x001EDA75 File Offset: 0x001EBC75
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x001EDA94 File Offset: 0x001EBC94
		public override void OnUpdate()
		{
			if (this.gm.isPaused)
			{
				return;
			}
			if (!this.ActiveBool.IsNone && !this.ActiveBool.Value)
			{
				return;
			}
			if (this.inputHandler.inputActions.SuperDash.WasPressed)
			{
				base.Fsm.Event(this.WasPressed);
			}
			if (this.inputHandler.inputActions.SuperDash.WasReleased)
			{
				base.Fsm.Event(this.WasReleased);
			}
			if (this.inputHandler.inputActions.SuperDash.IsPressed)
			{
				base.Fsm.Event(this.IsPressed);
				if (!this.isPressedBool.IsNone)
				{
					this.isPressedBool.Value = true;
				}
			}
			if (!this.inputHandler.inputActions.SuperDash.IsPressed)
			{
				base.Fsm.Event(this.IsNotPressed);
				if (!this.isPressedBool.IsNone)
				{
					this.isPressedBool.Value = false;
				}
			}
		}

		// Token: 0x04005F7E RID: 24446
		[Tooltip("Where to send the event.")]
		public FsmEventTarget Target;

		// Token: 0x04005F7F RID: 24447
		public FsmEvent WasPressed;

		// Token: 0x04005F80 RID: 24448
		public FsmEvent WasReleased;

		// Token: 0x04005F81 RID: 24449
		public FsmEvent IsPressed;

		// Token: 0x04005F82 RID: 24450
		public FsmEvent IsNotPressed;

		// Token: 0x04005F83 RID: 24451
		public FsmBool isPressedBool;

		// Token: 0x04005F84 RID: 24452
		public FsmBool ActiveBool;

		// Token: 0x04005F85 RID: 24453
		private GameManager gm;

		// Token: 0x04005F86 RID: 24454
		private InputHandler inputHandler;
	}
}
