using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA9 RID: 3241
	public class ListenForMenuActionsContinuous : FsmStateAction
	{
		// Token: 0x0600611D RID: 24861 RVA: 0x001EC49F File Offset: 0x001EA69F
		public override void Reset()
		{
			this.EventTarget = null;
			this.SubmitPressed = null;
			this.CancelPressed = null;
			this.NonePressed = null;
			this.IgnoreAttack = null;
		}

		// Token: 0x0600611E RID: 24862 RVA: 0x001EC4C4 File Offset: 0x001EA6C4
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

		// Token: 0x0600611F RID: 24863 RVA: 0x001EC520 File Offset: 0x001EA720
		public override void OnUpdate()
		{
			if (this.gm == null || this.gm.isPaused || this.inputHandler == null)
			{
				return;
			}
			HeroActions inputActions = this.inputHandler.inputActions;
			Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions, this.IgnoreAttack.Value, true);
			if (menuAction == Platform.MenuActions.Submit)
			{
				base.Fsm.Event(this.EventTarget, this.SubmitPressed);
				return;
			}
			if (menuAction != Platform.MenuActions.Cancel)
			{
				base.Fsm.Event(this.EventTarget, this.NonePressed);
				return;
			}
			base.Fsm.Event(this.EventTarget, this.CancelPressed);
		}

		// Token: 0x04005EF5 RID: 24309
		public FsmEventTarget EventTarget;

		// Token: 0x04005EF6 RID: 24310
		public FsmEvent SubmitPressed;

		// Token: 0x04005EF7 RID: 24311
		public FsmEvent CancelPressed;

		// Token: 0x04005EF8 RID: 24312
		public FsmEvent NonePressed;

		// Token: 0x04005EF9 RID: 24313
		public FsmBool IgnoreAttack;

		// Token: 0x04005EFA RID: 24314
		private GameManager gm;

		// Token: 0x04005EFB RID: 24315
		private InputHandler inputHandler;
	}
}
