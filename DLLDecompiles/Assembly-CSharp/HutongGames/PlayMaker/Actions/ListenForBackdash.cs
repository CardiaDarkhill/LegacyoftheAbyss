using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9A RID: 3226
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForBackdash : FsmStateAction
	{
		// Token: 0x060060D8 RID: 24792 RVA: 0x001EAFD8 File Offset: 0x001E91D8
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x001EAFE1 File Offset: 0x001E91E1
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x001EB000 File Offset: 0x001E9200
		public override void OnUpdate()
		{
			if (this.inputHandler.inputActions.Evade.WasPressed)
			{
				base.Fsm.Event(this.wasPressed);
			}
			if (this.inputHandler.inputActions.Evade.WasReleased)
			{
				base.Fsm.Event(this.wasReleased);
			}
			if (this.inputHandler.inputActions.Evade.IsPressed)
			{
				base.Fsm.Event(this.isPressed);
			}
			if (!this.inputHandler.inputActions.Evade.IsPressed)
			{
				base.Fsm.Event(this.isNotPressed);
			}
		}

		// Token: 0x04005E6F RID: 24175
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005E70 RID: 24176
		public FsmEvent wasPressed;

		// Token: 0x04005E71 RID: 24177
		public FsmEvent wasReleased;

		// Token: 0x04005E72 RID: 24178
		public FsmEvent isPressed;

		// Token: 0x04005E73 RID: 24179
		public FsmEvent isNotPressed;

		// Token: 0x04005E74 RID: 24180
		private GameManager gm;

		// Token: 0x04005E75 RID: 24181
		private InputHandler inputHandler;
	}
}
