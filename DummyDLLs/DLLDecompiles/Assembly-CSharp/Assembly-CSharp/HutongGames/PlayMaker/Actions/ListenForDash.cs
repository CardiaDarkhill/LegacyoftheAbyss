using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9C RID: 3228
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDash : FsmStateAction
	{
		// Token: 0x060060E1 RID: 24801 RVA: 0x001EB297 File Offset: 0x001E9497
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x001EB2A0 File Offset: 0x001E94A0
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.delayBeforeActive.Value;
			this.CheckInput();
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x001EB2D5 File Offset: 0x001E94D5
		public override void OnUpdate()
		{
			this.CheckInput();
		}

		// Token: 0x060060E4 RID: 24804 RVA: 0x001EB2E0 File Offset: 0x001E94E0
		private void CheckInput()
		{
			if (!this.gm.isPaused)
			{
				if (this.timer > 0f)
				{
					this.timer -= Time.deltaTime;
					return;
				}
				if (this.inputHandler.inputActions.Dash.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Dash.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Dash.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.Dash.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005E81 RID: 24193
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005E82 RID: 24194
		public FsmEvent wasPressed;

		// Token: 0x04005E83 RID: 24195
		public FsmEvent wasReleased;

		// Token: 0x04005E84 RID: 24196
		public FsmEvent isPressed;

		// Token: 0x04005E85 RID: 24197
		public FsmEvent isNotPressed;

		// Token: 0x04005E86 RID: 24198
		public FsmFloat delayBeforeActive;

		// Token: 0x04005E87 RID: 24199
		private GameManager gm;

		// Token: 0x04005E88 RID: 24200
		private InputHandler inputHandler;

		// Token: 0x04005E89 RID: 24201
		private float timer;
	}
}
