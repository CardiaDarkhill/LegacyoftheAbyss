using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA2 RID: 3234
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForInventory : FsmStateAction
	{
		// Token: 0x060060FE RID: 24830 RVA: 0x001EBADC File Offset: 0x001E9CDC
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x060060FF RID: 24831 RVA: 0x001EBAE5 File Offset: 0x001E9CE5
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x001EBB04 File Offset: 0x001E9D04
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.inputHandler.inputActions.OpenInventory.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.OpenInventory.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.OpenInventory.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.OpenInventory.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005EB8 RID: 24248
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005EB9 RID: 24249
		public FsmEvent wasPressed;

		// Token: 0x04005EBA RID: 24250
		public FsmEvent wasReleased;

		// Token: 0x04005EBB RID: 24251
		public FsmEvent isPressed;

		// Token: 0x04005EBC RID: 24252
		public FsmEvent isNotPressed;

		// Token: 0x04005EBD RID: 24253
		private GameManager gm;

		// Token: 0x04005EBE RID: 24254
		private InputHandler inputHandler;
	}
}
