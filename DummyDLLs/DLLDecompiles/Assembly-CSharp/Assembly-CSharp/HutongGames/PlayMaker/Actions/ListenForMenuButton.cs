using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAC RID: 3244
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public abstract class ListenForMenuButton : FsmStateAction
	{
		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x06006127 RID: 24871
		protected abstract Platform.MenuActions MenuAction { get; }

		// Token: 0x06006128 RID: 24872 RVA: 0x001EC6A8 File Offset: 0x001EA8A8
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x06006129 RID: 24873 RVA: 0x001EC6B1 File Offset: 0x001EA8B1
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.wasPreviouslyPressed = (Platform.Current.GetMenuAction(this.inputHandler.inputActions, false, true) == Platform.MenuActions.Submit);
		}

		// Token: 0x0600612A RID: 24874 RVA: 0x001EC6F0 File Offset: 0x001EA8F0
		public override void OnUpdate()
		{
			if (this.gm == null || this.gm.isPaused || this.inputHandler == null)
			{
				return;
			}
			HeroActions inputActions = this.inputHandler.inputActions;
			Platform platform = Platform.Current;
			Platform.MenuActions menuAction = this.MenuAction;
			if (platform.GetMenuAction(inputActions, false, true) == menuAction)
			{
				if (platform.GetMenuAction(inputActions, false, false) == menuAction)
				{
					base.Fsm.Event(this.eventTarget, this.wasPressed);
				}
				base.Fsm.Event(this.eventTarget, this.isPressed);
				this.wasPreviouslyPressed = true;
				return;
			}
			if (this.wasPreviouslyPressed)
			{
				base.Fsm.Event(this.eventTarget, this.wasReleased);
			}
			base.Fsm.Event(this.eventTarget, this.isNotPressed);
			this.wasPreviouslyPressed = false;
		}

		// Token: 0x04005F00 RID: 24320
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F01 RID: 24321
		public FsmEvent wasPressed;

		// Token: 0x04005F02 RID: 24322
		public FsmEvent wasReleased;

		// Token: 0x04005F03 RID: 24323
		public FsmEvent isPressed;

		// Token: 0x04005F04 RID: 24324
		public FsmEvent isNotPressed;

		// Token: 0x04005F05 RID: 24325
		private bool wasPreviouslyPressed;

		// Token: 0x04005F06 RID: 24326
		private GameManager gm;

		// Token: 0x04005F07 RID: 24327
		private InputHandler inputHandler;
	}
}
