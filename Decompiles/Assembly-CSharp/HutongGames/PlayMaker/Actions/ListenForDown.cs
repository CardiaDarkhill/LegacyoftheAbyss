using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA0 RID: 3232
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForDown : FsmStateAction
	{
		// Token: 0x060060F5 RID: 24821 RVA: 0x001EB768 File Offset: 0x001E9968
		public override void Reset()
		{
			this.eventTarget = null;
			this.isPressedBool = new FsmBool
			{
				UseVariable = true
			};
			this.queueBool = new FsmBool
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
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x001EB7C4 File Offset: 0x001E99C4
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

		// Token: 0x060060F7 RID: 24823 RVA: 0x001EB7F6 File Offset: 0x001E99F6
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x001EB800 File Offset: 0x001E9A00
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Down.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = true;
					}
				}
				if (this.inputHandler.inputActions.Down.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Down.IsPressed)
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
				if (!this.inputHandler.inputActions.Down.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
					if (!this.isPressedBool.IsNone)
					{
						this.isPressedBool.Value = false;
					}
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = false;
					}
					if (!this.pressedTimer.IsNone && this.pressedTimer.Value != 0f)
					{
						this.pressedTimer.Value = 0f;
					}
				}
			}
		}

		// Token: 0x04005EA4 RID: 24228
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005EA5 RID: 24229
		public FsmEvent wasPressed;

		// Token: 0x04005EA6 RID: 24230
		public FsmEvent wasReleased;

		// Token: 0x04005EA7 RID: 24231
		public FsmEvent isPressed;

		// Token: 0x04005EA8 RID: 24232
		public FsmEvent isNotPressed;

		// Token: 0x04005EA9 RID: 24233
		public FsmBool isPressedBool;

		// Token: 0x04005EAA RID: 24234
		public FsmBool queueBool;

		// Token: 0x04005EAB RID: 24235
		public FsmFloat pressedTimer;

		// Token: 0x04005EAC RID: 24236
		public FsmBool activeBool;

		// Token: 0x04005EAD RID: 24237
		public bool stateEntryOnly;

		// Token: 0x04005EAE RID: 24238
		private GameManager gm;

		// Token: 0x04005EAF RID: 24239
		private InputHandler inputHandler;
	}
}
