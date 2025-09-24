using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA6 RID: 3238
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForLeft : FsmStateAction
	{
		// Token: 0x06006110 RID: 24848 RVA: 0x001EC034 File Offset: 0x001EA234
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

		// Token: 0x06006111 RID: 24849 RVA: 0x001EC073 File Offset: 0x001EA273
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

		// Token: 0x06006112 RID: 24850 RVA: 0x001EC0A5 File Offset: 0x001EA2A5
		public override void OnUpdate()
		{
			this.CheckForInput();
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x001EC0B0 File Offset: 0x001EA2B0
		public void CheckForInput()
		{
			if (!this.gm.isPaused && (!this.ignoreIfRightPressed || !this.inputHandler.inputActions.Right.IsPressed) && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Left.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
				}
				if (this.inputHandler.inputActions.Left.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Left.IsPressed)
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
				if (!this.inputHandler.inputActions.Left.IsPressed)
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

		// Token: 0x04005EDB RID: 24283
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005EDC RID: 24284
		public FsmEvent wasPressed;

		// Token: 0x04005EDD RID: 24285
		public FsmEvent wasReleased;

		// Token: 0x04005EDE RID: 24286
		public FsmEvent isPressed;

		// Token: 0x04005EDF RID: 24287
		public FsmEvent isNotPressed;

		// Token: 0x04005EE0 RID: 24288
		public FsmBool isPressedBool;

		// Token: 0x04005EE1 RID: 24289
		public FsmFloat pressedTimer;

		// Token: 0x04005EE2 RID: 24290
		public FsmBool activeBool;

		// Token: 0x04005EE3 RID: 24291
		public bool stateEntryOnly;

		// Token: 0x04005EE4 RID: 24292
		public bool ignoreIfRightPressed;

		// Token: 0x04005EE5 RID: 24293
		private GameManager gm;

		// Token: 0x04005EE6 RID: 24294
		private InputHandler inputHandler;
	}
}
