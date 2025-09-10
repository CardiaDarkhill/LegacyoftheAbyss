using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA5 RID: 3237
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForJumpV2 : FsmStateAction
	{
		// Token: 0x0600610B RID: 24843 RVA: 0x001EBE58 File Offset: 0x001EA058
		public override void Reset()
		{
			this.eventTarget = null;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
			this.queueBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x0600610C RID: 24844 RVA: 0x001EBE88 File Offset: 0x001EA088
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.delayTimer = this.delayBeforeActive;
			if (this.delayBeforeActive == 0f)
			{
				this.CheckInput();
			}
			if (this.stateEntryOnly)
			{
				base.Finish();
			}
		}

		// Token: 0x0600610D RID: 24845 RVA: 0x001EBEDE File Offset: 0x001EA0DE
		public override void OnUpdate()
		{
			if (this.delayTimer > 0f)
			{
				this.delayTimer -= Time.deltaTime;
				return;
			}
			this.CheckInput();
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x001EBF08 File Offset: 0x001EA108
		private void CheckInput()
		{
			if (!this.gm.isPaused && (this.activeBool.IsNone || this.activeBool.Value))
			{
				if (this.inputHandler.inputActions.Jump.WasPressed)
				{
					base.Fsm.Event(this.wasPressed);
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = true;
					}
				}
				if (this.inputHandler.inputActions.Jump.WasReleased)
				{
					base.Fsm.Event(this.wasReleased);
				}
				if (this.inputHandler.inputActions.Jump.IsPressed)
				{
					base.Fsm.Event(this.isPressed);
					this.isPressedBool.Value = true;
				}
				if (!this.inputHandler.inputActions.Jump.IsPressed)
				{
					base.Fsm.Event(this.isNotPressed);
					this.isPressedBool.Value = false;
					if (!this.queueBool.IsNone)
					{
						this.queueBool.Value = false;
					}
				}
			}
		}

		// Token: 0x04005ECE RID: 24270
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04005ECF RID: 24271
		public FsmEvent wasPressed;

		// Token: 0x04005ED0 RID: 24272
		public FsmEvent wasReleased;

		// Token: 0x04005ED1 RID: 24273
		public FsmEvent isPressed;

		// Token: 0x04005ED2 RID: 24274
		public FsmEvent isNotPressed;

		// Token: 0x04005ED3 RID: 24275
		public FsmBool isPressedBool;

		// Token: 0x04005ED4 RID: 24276
		public FsmBool queueBool;

		// Token: 0x04005ED5 RID: 24277
		public FsmBool activeBool;

		// Token: 0x04005ED6 RID: 24278
		public bool stateEntryOnly;

		// Token: 0x04005ED7 RID: 24279
		public float delayBeforeActive;

		// Token: 0x04005ED8 RID: 24280
		private GameManager gm;

		// Token: 0x04005ED9 RID: 24281
		private InputHandler inputHandler;

		// Token: 0x04005EDA RID: 24282
		private float delayTimer;
	}
}
