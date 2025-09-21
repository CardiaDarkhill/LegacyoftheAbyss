using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB2 RID: 3250
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForQuickMap : FsmStateAction
	{
		// Token: 0x0600613E RID: 24894 RVA: 0x001ECC5C File Offset: 0x001EAE5C
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x0600613F RID: 24895 RVA: 0x001ECC65 File Offset: 0x001EAE65
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006140 RID: 24896 RVA: 0x001ECC84 File Offset: 0x001EAE84
		public override void OnUpdate()
		{
			if (this.inputHandler.inputActions.QuickMap.WasPressed)
			{
				base.Fsm.Event(this.wasPressed);
			}
			if (this.inputHandler.inputActions.QuickMap.WasReleased)
			{
				base.Fsm.Event(this.wasReleased);
			}
			if (this.inputHandler.inputActions.QuickMap.IsPressed)
			{
				base.Fsm.Event(this.isPressed);
			}
			if (!this.inputHandler.inputActions.QuickMap.IsPressed)
			{
				base.Fsm.Event(this.isNotPressed);
			}
		}

		// Token: 0x04005F28 RID: 24360
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F29 RID: 24361
		public FsmEvent wasPressed;

		// Token: 0x04005F2A RID: 24362
		public FsmEvent wasReleased;

		// Token: 0x04005F2B RID: 24363
		public FsmEvent isPressed;

		// Token: 0x04005F2C RID: 24364
		public FsmEvent isNotPressed;

		// Token: 0x04005F2D RID: 24365
		private GameManager gm;

		// Token: 0x04005F2E RID: 24366
		private InputHandler inputHandler;
	}
}
