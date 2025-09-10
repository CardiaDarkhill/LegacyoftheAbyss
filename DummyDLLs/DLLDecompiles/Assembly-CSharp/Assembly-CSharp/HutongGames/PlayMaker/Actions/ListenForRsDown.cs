using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB5 RID: 3253
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForRsDown : FsmStateAction
	{
		// Token: 0x0600614B RID: 24907 RVA: 0x001ED0F8 File Offset: 0x001EB2F8
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

		// Token: 0x0600614C RID: 24908 RVA: 0x001ED14C File Offset: 0x001EB34C
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x0600614D RID: 24909 RVA: 0x001ED16C File Offset: 0x001EB36C
		public override void OnUpdate()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.RsDown.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.RsDown.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.RsDown.IsPressed)
				{
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = true;
					}
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.RsDown.IsPressed)
				{
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = false;
					}
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F46 RID: 24390
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F47 RID: 24391
		public FsmEvent wasPressed;

		// Token: 0x04005F48 RID: 24392
		public FsmEvent wasReleased;

		// Token: 0x04005F49 RID: 24393
		public FsmEvent isPressed;

		// Token: 0x04005F4A RID: 24394
		public FsmEvent isNotPressed;

		// Token: 0x04005F4B RID: 24395
		public FsmBool isPressedBool;

		// Token: 0x04005F4C RID: 24396
		public FsmBool activeBool;

		// Token: 0x04005F4D RID: 24397
		private GameManager gm;

		// Token: 0x04005F4E RID: 24398
		private InputHandler inputHandler;
	}
}
