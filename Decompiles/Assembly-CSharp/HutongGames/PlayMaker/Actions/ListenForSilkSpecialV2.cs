using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB8 RID: 3256
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSilkSpecialV2 : FsmStateAction
	{
		// Token: 0x06006158 RID: 24920 RVA: 0x001ED5EC File Offset: 0x001EB7EC
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
			this.queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06006159 RID: 24921 RVA: 0x001ED648 File Offset: 0x001EB848
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

		// Token: 0x0600615A RID: 24922 RVA: 0x001ED67A File Offset: 0x001EB87A
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x0600615B RID: 24923 RVA: 0x001ED684 File Offset: 0x001EB884
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.QuickCast.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = true;
					}
				}
				if (this.inputHandler.inputActions.QuickCast.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.QuickCast.IsPressed)
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
				if (!this.inputHandler.inputActions.QuickCast.IsPressed)
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
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = false;
					}
				}
			}
		}

		// Token: 0x04005F63 RID: 24419
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F64 RID: 24420
		public FsmEvent wasPressed;

		// Token: 0x04005F65 RID: 24421
		public FsmEvent wasReleased;

		// Token: 0x04005F66 RID: 24422
		public FsmEvent isPressed;

		// Token: 0x04005F67 RID: 24423
		public FsmEvent isNotPressed;

		// Token: 0x04005F68 RID: 24424
		public FsmBool isPressedBool;

		// Token: 0x04005F69 RID: 24425
		public FsmFloat pressedTimer;

		// Token: 0x04005F6A RID: 24426
		public FsmBool queueBool;

		// Token: 0x04005F6B RID: 24427
		public FsmBool activeBool;

		// Token: 0x04005F6C RID: 24428
		public bool stateEntryOnly;

		// Token: 0x04005F6D RID: 24429
		private GameManager gm;

		// Token: 0x04005F6E RID: 24430
		private InputHandler inputHandler;
	}
}
