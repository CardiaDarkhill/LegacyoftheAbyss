using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB4 RID: 3252
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForRight : FsmStateAction
	{
		// Token: 0x06006146 RID: 24902 RVA: 0x001ECEF0 File Offset: 0x001EB0F0
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

		// Token: 0x06006147 RID: 24903 RVA: 0x001ECF2F File Offset: 0x001EB12F
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

		// Token: 0x06006148 RID: 24904 RVA: 0x001ECF61 File Offset: 0x001EB161
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x06006149 RID: 24905 RVA: 0x001ECF6C File Offset: 0x001EB16C
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (!this.ignoreIfLeftPressed || !this.inputHandler.inputActions.Left.IsPressed) && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Right.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Right.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Right.IsPressed)
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
				if (!this.inputHandler.inputActions.Right.IsPressed)
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

		// Token: 0x04005F3A RID: 24378
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F3B RID: 24379
		public FsmEvent wasPressed;

		// Token: 0x04005F3C RID: 24380
		public FsmEvent wasReleased;

		// Token: 0x04005F3D RID: 24381
		public FsmEvent isPressed;

		// Token: 0x04005F3E RID: 24382
		public FsmEvent isNotPressed;

		// Token: 0x04005F3F RID: 24383
		public FsmBool isPressedBool;

		// Token: 0x04005F40 RID: 24384
		public FsmFloat pressedTimer;

		// Token: 0x04005F41 RID: 24385
		public FsmBool activeBool;

		// Token: 0x04005F42 RID: 24386
		public bool stateEntryOnly;

		// Token: 0x04005F43 RID: 24387
		public bool ignoreIfLeftPressed;

		// Token: 0x04005F44 RID: 24388
		private GameManager gm;

		// Token: 0x04005F45 RID: 24389
		private InputHandler inputHandler;
	}
}
