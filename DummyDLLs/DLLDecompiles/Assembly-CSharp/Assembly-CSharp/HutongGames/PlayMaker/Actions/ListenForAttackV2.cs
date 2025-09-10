using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C99 RID: 3225
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForAttackV2 : FsmStateAction
	{
		// Token: 0x060060D4 RID: 24788 RVA: 0x001EAE24 File Offset: 0x001E9024
		public override void Reset()
		{
			this.EventTarget = null;
			this.WasPressed = null;
			this.WasReleased = null;
			this.IsPressed = null;
			this.IsNotPressed = null;
			this.DelayBeforeActive = null;
			this.IsActive = true;
			this.queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x001EAE79 File Offset: 0x001E9079
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.DelayBeforeActive.Value;
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x001EAEA8 File Offset: 0x001E90A8
		public override void OnUpdate()
		{
			if (this.gm.isPaused)
			{
				return;
			}
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			if (!this.IsActive.Value && !this.IsActive.IsNone)
			{
				return;
			}
			if (this.inputHandler.inputActions.Attack.WasPressed)
			{
				base.Fsm.Event(this.WasPressed);
				if (!this.queueBool.IsNone)
				{
					this.queueBool.Value = true;
				}
			}
			if (this.inputHandler.inputActions.Attack.WasReleased)
			{
				base.Fsm.Event(this.WasReleased);
			}
			if (this.inputHandler.inputActions.Attack.IsPressed)
			{
				base.Fsm.Event(this.IsPressed);
			}
			if (!this.inputHandler.inputActions.Attack.IsPressed)
			{
				base.Fsm.Event(this.IsNotPressed);
				if (!this.queueBool.IsNone)
				{
					this.queueBool.Value = false;
				}
			}
		}

		// Token: 0x04005E64 RID: 24164
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		// Token: 0x04005E65 RID: 24165
		public FsmEvent WasPressed;

		// Token: 0x04005E66 RID: 24166
		public FsmEvent WasReleased;

		// Token: 0x04005E67 RID: 24167
		public FsmEvent IsPressed;

		// Token: 0x04005E68 RID: 24168
		public FsmEvent IsNotPressed;

		// Token: 0x04005E69 RID: 24169
		public FsmBool queueBool;

		// Token: 0x04005E6A RID: 24170
		public FsmFloat DelayBeforeActive;

		// Token: 0x04005E6B RID: 24171
		public FsmBool IsActive;

		// Token: 0x04005E6C RID: 24172
		private GameManager gm;

		// Token: 0x04005E6D RID: 24173
		private InputHandler inputHandler;

		// Token: 0x04005E6E RID: 24174
		private float timer;
	}
}
