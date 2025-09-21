using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CB7 RID: 3255
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForSilkSpecial : FsmStateAction
	{
		// Token: 0x06006153 RID: 24915 RVA: 0x001ED408 File Offset: 0x001EB608
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

		// Token: 0x06006154 RID: 24916 RVA: 0x001ED447 File Offset: 0x001EB647
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

		// Token: 0x06006155 RID: 24917 RVA: 0x001ED479 File Offset: 0x001EB679
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x001ED484 File Offset: 0x001EB684
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.QuickCast.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
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
				}
			}
		}

		// Token: 0x04005F58 RID: 24408
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005F59 RID: 24409
		public FsmEvent wasPressed;

		// Token: 0x04005F5A RID: 24410
		public FsmEvent wasReleased;

		// Token: 0x04005F5B RID: 24411
		public FsmEvent isPressed;

		// Token: 0x04005F5C RID: 24412
		public FsmEvent isNotPressed;

		// Token: 0x04005F5D RID: 24413
		public FsmBool isPressedBool;

		// Token: 0x04005F5E RID: 24414
		public FsmFloat pressedTimer;

		// Token: 0x04005F5F RID: 24415
		public FsmBool activeBool;

		// Token: 0x04005F60 RID: 24416
		public bool stateEntryOnly;

		// Token: 0x04005F61 RID: 24417
		private GameManager gm;

		// Token: 0x04005F62 RID: 24418
		private InputHandler inputHandler;
	}
}
