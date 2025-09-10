using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C98 RID: 3224
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForAttack : FsmStateAction
	{
		// Token: 0x060060D0 RID: 24784 RVA: 0x001EAD01 File Offset: 0x001E8F01
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x001EAD0A File Offset: 0x001E8F0A
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.delayBeforeActive.Value;
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x001EAD3C File Offset: 0x001E8F3C
		public override void OnUpdate()
		{
			if (!this.gm.isPaused)
			{
				if (this.timer > 0f)
				{
					this.timer -= Time.deltaTime;
					return;
				}
				if (this.inputHandler.inputActions.Attack.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Attack.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Attack.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
				}
				if (!this.inputHandler.inputActions.Attack.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
				}
			}
		}

		// Token: 0x04005E5B RID: 24155
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005E5C RID: 24156
		public FsmEvent wasPressed;

		// Token: 0x04005E5D RID: 24157
		public FsmEvent wasReleased;

		// Token: 0x04005E5E RID: 24158
		public FsmEvent isPressed;

		// Token: 0x04005E5F RID: 24159
		public FsmEvent isNotPressed;

		// Token: 0x04005E60 RID: 24160
		public FsmFloat delayBeforeActive;

		// Token: 0x04005E61 RID: 24161
		private GameManager gm;

		// Token: 0x04005E62 RID: 24162
		private InputHandler inputHandler;

		// Token: 0x04005E63 RID: 24163
		private float timer;
	}
}
