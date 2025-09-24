using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9E RID: 3230
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDashV2 : FsmStateAction
	{
		// Token: 0x060060EB RID: 24811 RVA: 0x001EB466 File Offset: 0x001E9666
		public override void Reset()
		{
			this.EventTarget = null;
			this.WasPressed = null;
			this.WasReleased = null;
			this.IsPressed = null;
			this.IsNotPressed = null;
			this.DelayBeforeActive = null;
			this.IsActive = true;
		}

		// Token: 0x060060EC RID: 24812 RVA: 0x001EB49E File Offset: 0x001E969E
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.DelayBeforeActive.Value;
			this.CheckInput();
		}

		// Token: 0x060060ED RID: 24813 RVA: 0x001EB4D3 File Offset: 0x001E96D3
		public override void OnUpdate()
		{
			this.CheckInput();
		}

		// Token: 0x060060EE RID: 24814 RVA: 0x001EB4DC File Offset: 0x001E96DC
		private void CheckInput()
		{
			if (!this.gm.isPaused)
			{
				if (this.timer > 0f)
				{
					this.timer -= Time.deltaTime;
					return;
				}
				if (this.IsActive.Value || this.IsActive.IsNone)
				{
					if (this.inputHandler.inputActions.Dash.WasPressed)
					{
						base.Fsm.Event(this.WasPressed);
					}
					if (this.inputHandler.inputActions.Dash.WasReleased)
					{
						base.Fsm.Event(this.WasReleased);
					}
					if (this.inputHandler.inputActions.Dash.IsPressed)
					{
						base.Fsm.Event(this.IsPressed);
					}
					if (!this.inputHandler.inputActions.Dash.IsPressed)
					{
						base.Fsm.Event(this.IsNotPressed);
					}
				}
			}
		}

		// Token: 0x04005E8F RID: 24207
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		// Token: 0x04005E90 RID: 24208
		public FsmEvent WasPressed;

		// Token: 0x04005E91 RID: 24209
		public FsmEvent WasReleased;

		// Token: 0x04005E92 RID: 24210
		public FsmEvent IsPressed;

		// Token: 0x04005E93 RID: 24211
		public FsmEvent IsNotPressed;

		// Token: 0x04005E94 RID: 24212
		public FsmFloat DelayBeforeActive;

		// Token: 0x04005E95 RID: 24213
		public FsmBool IsActive;

		// Token: 0x04005E96 RID: 24214
		private GameManager gm;

		// Token: 0x04005E97 RID: 24215
		private InputHandler inputHandler;

		// Token: 0x04005E98 RID: 24216
		private float timer;
	}
}
