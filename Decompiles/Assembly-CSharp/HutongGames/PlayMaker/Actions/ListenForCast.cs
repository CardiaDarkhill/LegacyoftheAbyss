using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9B RID: 3227
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForCast : FsmStateAction
	{
		// Token: 0x060060DC RID: 24796 RVA: 0x001EB0B5 File Offset: 0x001E92B5
		public override void Reset()
		{
			this.eventTarget = null;
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
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x001EB0F4 File Offset: 0x001E92F4
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

		// Token: 0x060060DE RID: 24798 RVA: 0x001EB126 File Offset: 0x001E9326
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x060060DF RID: 24799 RVA: 0x001EB130 File Offset: 0x001E9330
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Cast.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Cast.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Cast.IsPressed)
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
				if (!this.inputHandler.inputActions.Cast.IsPressed)
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

		// Token: 0x04005E76 RID: 24182
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005E77 RID: 24183
		public FsmEvent wasPressed;

		// Token: 0x04005E78 RID: 24184
		public FsmEvent wasReleased;

		// Token: 0x04005E79 RID: 24185
		public FsmEvent isPressed;

		// Token: 0x04005E7A RID: 24186
		public FsmEvent isNotPressed;

		// Token: 0x04005E7B RID: 24187
		public FsmBool isPressedBool;

		// Token: 0x04005E7C RID: 24188
		public FsmFloat pressedTimer;

		// Token: 0x04005E7D RID: 24189
		public FsmBool activeBool;

		// Token: 0x04005E7E RID: 24190
		public bool stateEntryOnly;

		// Token: 0x04005E7F RID: 24191
		private GameManager gm;

		// Token: 0x04005E80 RID: 24192
		private InputHandler inputHandler;
	}
}
