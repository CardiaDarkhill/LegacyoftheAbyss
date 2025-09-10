using System;
using InControl;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9F RID: 3231
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDashV3 : FsmStateAction
	{
		// Token: 0x060060F0 RID: 24816 RVA: 0x001EB5DE File Offset: 0x001E97DE
		public override void Reset()
		{
			this.EventTarget = null;
			this.IsActive = true;
			this.DelayBeforeActive = null;
			this.WasPressed = null;
			this.WasReleased = null;
			this.IsPressed = null;
			this.IsNotPressed = null;
			this.SetIsPressed = null;
		}

		// Token: 0x060060F1 RID: 24817 RVA: 0x001EB61D File Offset: 0x001E981D
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.timer = this.DelayBeforeActive.Value;
			this.CheckInput();
		}

		// Token: 0x060060F2 RID: 24818 RVA: 0x001EB652 File Offset: 0x001E9852
		public override void OnUpdate()
		{
			this.CheckInput();
		}

		// Token: 0x060060F3 RID: 24819 RVA: 0x001EB65C File Offset: 0x001E985C
		private void CheckInput()
		{
			this.SetIsPressed.Value = false;
			if (this.gm.isPaused)
			{
				return;
			}
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			if (this.IsActive.Value || this.IsActive.IsNone)
			{
				PlayerAction dash = this.inputHandler.inputActions.Dash;
				if (dash.WasPressed)
				{
					base.Fsm.Event(this.EventTarget, this.WasPressed);
				}
				if (dash.WasReleased)
				{
					base.Fsm.Event(this.EventTarget, this.WasReleased);
				}
				if (dash.IsPressed)
				{
					base.Fsm.Event(this.EventTarget, this.IsPressed);
				}
				if (!dash.IsPressed)
				{
					base.Fsm.Event(this.EventTarget, this.IsNotPressed);
				}
				this.SetIsPressed.Value = dash.IsPressed;
			}
		}

		// Token: 0x04005E99 RID: 24217
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		// Token: 0x04005E9A RID: 24218
		public FsmBool IsActive;

		// Token: 0x04005E9B RID: 24219
		public FsmFloat DelayBeforeActive;

		// Token: 0x04005E9C RID: 24220
		public FsmEvent WasPressed;

		// Token: 0x04005E9D RID: 24221
		public FsmEvent WasReleased;

		// Token: 0x04005E9E RID: 24222
		public FsmEvent IsPressed;

		// Token: 0x04005E9F RID: 24223
		public FsmEvent IsNotPressed;

		// Token: 0x04005EA0 RID: 24224
		[UIHint(UIHint.Variable)]
		public FsmBool SetIsPressed;

		// Token: 0x04005EA1 RID: 24225
		private GameManager gm;

		// Token: 0x04005EA2 RID: 24226
		private InputHandler inputHandler;

		// Token: 0x04005EA3 RID: 24227
		private float timer;
	}
}
