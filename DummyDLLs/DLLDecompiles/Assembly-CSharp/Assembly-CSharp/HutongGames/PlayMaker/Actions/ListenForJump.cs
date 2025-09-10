using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA4 RID: 3236
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForJump : FsmStateAction
	{
		// Token: 0x06006106 RID: 24838 RVA: 0x001EBCDD File Offset: 0x001E9EDD
		public override void Reset()
		{
			this.eventTarget = null;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
			this.queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06006107 RID: 24839 RVA: 0x001EBD0A File Offset: 0x001E9F0A
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.CheckInput();
			if (this.stateEntryOnly)
			{
				base.Finish();
			}
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x001EBD3C File Offset: 0x001E9F3C
		public override void OnUpdate()
		{
			this.CheckInput();
		}

		// Token: 0x06006109 RID: 24841 RVA: 0x001EBD44 File Offset: 0x001E9F44
		private void CheckInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Jump.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = true;
					}
				}
				if (this.inputHandler.inputActions.Jump.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Jump.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.Jump.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = false;
					}
				}
			}
		}

		// Token: 0x04005EC4 RID: 24260
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005EC5 RID: 24261
		public FsmEvent wasPressed;

		// Token: 0x04005EC6 RID: 24262
		public FsmEvent wasReleased;

		// Token: 0x04005EC7 RID: 24263
		public FsmEvent isPressed;

		// Token: 0x04005EC8 RID: 24264
		public FsmEvent isNotPressed;

		// Token: 0x04005EC9 RID: 24265
		public FsmBool queueBool;

		// Token: 0x04005ECA RID: 24266
		public FsmBool activeBool;

		// Token: 0x04005ECB RID: 24267
		public bool stateEntryOnly;

		// Token: 0x04005ECC RID: 24268
		private GameManager gm;

		// Token: 0x04005ECD RID: 24269
		private InputHandler inputHandler;
	}
}
