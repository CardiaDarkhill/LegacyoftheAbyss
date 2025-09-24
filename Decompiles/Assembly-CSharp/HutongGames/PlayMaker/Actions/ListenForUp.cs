using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBE RID: 3262
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForUp : FsmStateAction
	{
		// Token: 0x06006172 RID: 24946 RVA: 0x001EDE48 File Offset: 0x001EC048
		public override void Reset()
		{
			this.eventTarget = null;
			this.wasPressed = null;
			this.isPressed = null;
			this.isNotPressed = null;
			this.isPressedBool = new FsmBool
			{
				UseVariable = true
			};
			this.pressedTimer = new FsmFloat
			{
				UseVariable = true
			};
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
			this.stateEntryOnly = false;
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x001EDEAE File Offset: 0x001EC0AE
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.CheckForInput();
			if (this.stateEntryOnly)
			{
				base.Finish();
			}
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x001EDEE0 File Offset: 0x001EC0E0
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x001EDEE8 File Offset: 0x001EC0E8
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Up.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Up.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Up.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = true;
					}
					if (!this.pressedTimer.IsNone)
					{
						this.pressedTimer.Value += Time.deltaTime;
					}
				}
				if (!this.inputHandler.inputActions.Up.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = false;
					}
					if (!this.pressedTimer.IsNone && this.pressedTimer.Value != 0f)
					{
						this.pressedTimer.Value = 0f;
					}
				}
			}
		}

		// Token: 0x04005F9A RID: 24474
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F9B RID: 24475
		public FsmEvent wasPressed;

		// Token: 0x04005F9C RID: 24476
		public FsmEvent wasReleased;

		// Token: 0x04005F9D RID: 24477
		public FsmEvent isPressed;

		// Token: 0x04005F9E RID: 24478
		public FsmEvent isNotPressed;

		// Token: 0x04005F9F RID: 24479
		public FsmBool isPressedBool;

		// Token: 0x04005FA0 RID: 24480
		public FsmFloat pressedTimer;

		// Token: 0x04005FA1 RID: 24481
		public FsmBool activeBool;

		// Token: 0x04005FA2 RID: 24482
		public bool stateEntryOnly;

		// Token: 0x04005FA3 RID: 24483
		private GameManager gm;

		// Token: 0x04005FA4 RID: 24484
		private InputHandler inputHandler;
	}
}
