using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBA RID: 3258
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSuperdashV2 : FsmStateAction
	{
		// Token: 0x06006161 RID: 24929 RVA: 0x001ED909 File Offset: 0x001EBB09
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

		// Token: 0x06006162 RID: 24930 RVA: 0x001ED940 File Offset: 0x001EBB40
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x001ED960 File Offset: 0x001EBB60
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
			}
			if (!this.inputHandler.inputActions.SuperDash.IsPressed)
			{
				base.Fsm.Event(this.IsNotPressed);
			}
		}

		// Token: 0x04005F76 RID: 24438
		[Tooltip("Where to send the event.")]
		public FsmEventTarget Target;

		// Token: 0x04005F77 RID: 24439
		public FsmEvent WasPressed;

		// Token: 0x04005F78 RID: 24440
		public FsmEvent WasReleased;

		// Token: 0x04005F79 RID: 24441
		public FsmEvent IsPressed;

		// Token: 0x04005F7A RID: 24442
		public FsmEvent IsNotPressed;

		// Token: 0x04005F7B RID: 24443
		public FsmBool ActiveBool;

		// Token: 0x04005F7C RID: 24444
		private GameManager gm;

		// Token: 0x04005F7D RID: 24445
		private InputHandler inputHandler;
	}
}
