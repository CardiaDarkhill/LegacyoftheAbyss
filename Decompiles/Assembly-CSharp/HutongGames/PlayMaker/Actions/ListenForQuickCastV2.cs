using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB1 RID: 3249
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForQuickCastV2 : FsmStateAction
	{
		// Token: 0x0600613A RID: 24890 RVA: 0x001ECAA8 File Offset: 0x001EACA8
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

		// Token: 0x0600613B RID: 24891 RVA: 0x001ECAFD File Offset: 0x001EACFD
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.DelayBeforeActive.Value;
		}

		// Token: 0x0600613C RID: 24892 RVA: 0x001ECB2C File Offset: 0x001EAD2C
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
			if (this.inputHandler.inputActions.QuickCast.WasPressed)
			{
				base.Fsm.Event(this.WasPressed);
				if (!this.queueBool.IsNone)
				{
					this.queueBool.Value = true;
				}
			}
			if (this.inputHandler.inputActions.QuickCast.WasReleased)
			{
				base.Fsm.Event(this.WasReleased);
			}
			if (this.inputHandler.inputActions.QuickCast.IsPressed)
			{
				base.Fsm.Event(this.IsPressed);
			}
			if (!this.inputHandler.inputActions.QuickCast.IsPressed)
			{
				base.Fsm.Event(this.IsNotPressed);
				if (!this.queueBool.IsNone)
				{
					this.queueBool.Value = false;
				}
			}
		}

		// Token: 0x04005F1D RID: 24349
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		// Token: 0x04005F1E RID: 24350
		public FsmEvent WasPressed;

		// Token: 0x04005F1F RID: 24351
		public FsmEvent WasReleased;

		// Token: 0x04005F20 RID: 24352
		public FsmEvent IsPressed;

		// Token: 0x04005F21 RID: 24353
		public FsmEvent IsNotPressed;

		// Token: 0x04005F22 RID: 24354
		public FsmBool queueBool;

		// Token: 0x04005F23 RID: 24355
		public FsmFloat DelayBeforeActive;

		// Token: 0x04005F24 RID: 24356
		public FsmBool IsActive;

		// Token: 0x04005F25 RID: 24357
		private GameManager gm;

		// Token: 0x04005F26 RID: 24358
		private InputHandler inputHandler;

		// Token: 0x04005F27 RID: 24359
		private float timer;
	}
}
