using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBC RID: 3260
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForTaunt : FsmStateAction
	{
		// Token: 0x06006169 RID: 24937 RVA: 0x001EDBA4 File Offset: 0x001EBDA4
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x0600616A RID: 24938 RVA: 0x001EDBAD File Offset: 0x001EBDAD
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.delayBeforeActive.Value;
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x001EDBDC File Offset: 0x001EBDDC
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.timer > 0f)
				{
					this.timer -= Time.deltaTime;
					return;
				}
				if (this.inputHandler.inputActions.Taunt.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Taunt.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Taunt.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.Taunt.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005F87 RID: 24455
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F88 RID: 24456
		public FsmEvent wasPressed;

		// Token: 0x04005F89 RID: 24457
		public FsmEvent wasReleased;

		// Token: 0x04005F8A RID: 24458
		public FsmEvent isPressed;

		// Token: 0x04005F8B RID: 24459
		public FsmEvent isNotPressed;

		// Token: 0x04005F8C RID: 24460
		public FsmFloat delayBeforeActive;

		// Token: 0x04005F8D RID: 24461
		private GameManager gm;

		// Token: 0x04005F8E RID: 24462
		private InputHandler inputHandler;

		// Token: 0x04005F8F RID: 24463
		private float timer;
	}
}
