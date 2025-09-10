using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB3 RID: 3251
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForQuickMapV2 : FsmStateAction
	{
		// Token: 0x06006142 RID: 24898 RVA: 0x001ECD3C File Offset: 0x001EAF3C
		public override void Reset()
		{
			this.eventTarget = null;
			this.wasPressed = null;
			this.wasReleased = null;
			this.isPressed = null;
			this.isNotPressed = null;
			this.DelayBeforeActive = null;
			this.IsActive = true;
			this.queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06006143 RID: 24899 RVA: 0x001ECD91 File Offset: 0x001EAF91
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.DelayBeforeActive.Value;
		}

		// Token: 0x06006144 RID: 24900 RVA: 0x001ECDC0 File Offset: 0x001EAFC0
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
			if (this.inputHandler.inputActions.QuickMap.WasPressed)
			{
				base.Fsm.Event(this.wasPressed);
				if (!this.queueBool.IsNone)
				{
					this.queueBool.Value = true;
				}
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
				if (!this.queueBool.IsNone)
				{
					this.queueBool.Value = false;
				}
			}
		}

		// Token: 0x04005F2F RID: 24367
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F30 RID: 24368
		public FsmEvent wasPressed;

		// Token: 0x04005F31 RID: 24369
		public FsmEvent wasReleased;

		// Token: 0x04005F32 RID: 24370
		public FsmEvent isPressed;

		// Token: 0x04005F33 RID: 24371
		public FsmEvent isNotPressed;

		// Token: 0x04005F34 RID: 24372
		public FsmBool queueBool;

		// Token: 0x04005F35 RID: 24373
		public FsmFloat DelayBeforeActive;

		// Token: 0x04005F36 RID: 24374
		public FsmBool IsActive;

		// Token: 0x04005F37 RID: 24375
		private GameManager gm;

		// Token: 0x04005F38 RID: 24376
		private InputHandler inputHandler;

		// Token: 0x04005F39 RID: 24377
		private float timer;
	}
}
