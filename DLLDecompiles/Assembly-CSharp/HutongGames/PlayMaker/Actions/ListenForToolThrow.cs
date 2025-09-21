using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBD RID: 3261
	[ActionCategory("Controls")]
	[Tooltip("Listens for an action button press (using HeroActions InControl mappings).")]
	public class ListenForToolThrow : FsmStateAction
	{
		// Token: 0x0600616D RID: 24941 RVA: 0x001EDCC4 File Offset: 0x001EBEC4
		public override void Reset()
		{
			this.EventTarget = null;
			this.WasPressed = null;
			this.WasReleased = null;
			this.IsPressed = null;
			this.IsNotPressed = null;
			this.RequireToolToThrow = true;
			this.IsActive = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x0600616E RID: 24942 RVA: 0x001EDD12 File Offset: 0x001EBF12
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			this.hc = HeroController.instance;
		}

		// Token: 0x0600616F RID: 24943 RVA: 0x001EDD3C File Offset: 0x001EBF3C
		public override void OnUpdate()
		{
			if (this.hc.IsPaused())
			{
				return;
			}
			if (!this.IsActive.IsNone && !this.IsActive.Value)
			{
				return;
			}
			if (this.inputHandler.inputActions.QuickCast.WasPressed && this.CanDo(true))
			{
				base.Fsm.Event(this.WasPressed);
			}
			if (this.inputHandler.inputActions.QuickCast.WasReleased)
			{
				base.Fsm.Event(this.WasReleased);
			}
			if (this.inputHandler.inputActions.QuickCast.IsPressed && this.CanDo(false))
			{
				base.Fsm.Event(this.IsPressed);
			}
			if (!this.inputHandler.inputActions.QuickCast.IsPressed)
			{
				base.Fsm.Event(this.IsNotPressed);
			}
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x001EDE24 File Offset: 0x001EC024
		private bool CanDo(bool reportFailure)
		{
			return !this.RequireToolToThrow.Value || HeroController.instance.GetWillThrowTool(reportFailure);
		}

		// Token: 0x04005F90 RID: 24464
		[Tooltip("Where to send the event.")]
		public FsmEventTarget EventTarget;

		// Token: 0x04005F91 RID: 24465
		public FsmEvent WasPressed;

		// Token: 0x04005F92 RID: 24466
		public FsmEvent WasReleased;

		// Token: 0x04005F93 RID: 24467
		public FsmEvent IsPressed;

		// Token: 0x04005F94 RID: 24468
		public FsmEvent IsNotPressed;

		// Token: 0x04005F95 RID: 24469
		public FsmBool RequireToolToThrow;

		// Token: 0x04005F96 RID: 24470
		public FsmBool IsActive;

		// Token: 0x04005F97 RID: 24471
		private HeroController hc;

		// Token: 0x04005F98 RID: 24472
		private GameManager gm;

		// Token: 0x04005F99 RID: 24473
		private InputHandler inputHandler;
	}
}
