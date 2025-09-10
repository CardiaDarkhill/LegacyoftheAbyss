using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA8 RID: 3240
	public class ListenForMenuActionsV2 : FsmStateAction
	{
		// Token: 0x06006119 RID: 24857 RVA: 0x001EC33B File Offset: 0x001EA53B
		public override void Reset()
		{
			this.EventTarget = null;
			this.SubmitPressed = null;
			this.CancelPressed = null;
			this.IgnoreAttack = null;
			this.ExtraPressed = null;
			this.SuperPressed = null;
		}

		// Token: 0x0600611A RID: 24858 RVA: 0x001EC368 File Offset: 0x001EA568
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			if (this.gm == null)
			{
				base.LogError("Cannot listen for buttons without game manager.");
				return;
			}
			this.inputHandler = this.gm.inputHandler;
			if (this.inputHandler == null)
			{
				base.LogError("Cannot listen for buttons without input handler.");
			}
		}

		// Token: 0x0600611B RID: 24859 RVA: 0x001EC3C4 File Offset: 0x001EA5C4
		public override void OnUpdate()
		{
			if (this.gm == null || this.gm.isPaused || this.inputHandler == null)
			{
				return;
			}
			HeroActions inputActions = this.inputHandler.inputActions;
			switch (Platform.Current.GetMenuAction(inputActions, this.IgnoreAttack.Value, false))
			{
			case Platform.MenuActions.Submit:
				base.Fsm.Event(this.EventTarget, this.SubmitPressed);
				return;
			case Platform.MenuActions.Cancel:
				base.Fsm.Event(this.EventTarget, this.CancelPressed);
				return;
			case Platform.MenuActions.Extra:
				base.Fsm.Event(this.EventTarget, this.ExtraPressed);
				return;
			case Platform.MenuActions.Super:
				base.Fsm.Event(this.EventTarget, this.SuperPressed);
				return;
			default:
				return;
			}
		}

		// Token: 0x04005EED RID: 24301
		public FsmEventTarget EventTarget;

		// Token: 0x04005EEE RID: 24302
		public FsmEvent SubmitPressed;

		// Token: 0x04005EEF RID: 24303
		public FsmEvent CancelPressed;

		// Token: 0x04005EF0 RID: 24304
		public FsmEvent ExtraPressed;

		// Token: 0x04005EF1 RID: 24305
		public FsmEvent SuperPressed;

		// Token: 0x04005EF2 RID: 24306
		public FsmBool IgnoreAttack;

		// Token: 0x04005EF3 RID: 24307
		private GameManager gm;

		// Token: 0x04005EF4 RID: 24308
		private InputHandler inputHandler;
	}
}
