using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA7 RID: 3239
	[ActionCategory("Controls")]
	[Tooltip("Listens for menu actions, and safely disambiguates jump/submit/attack/cancel/cast.")]
	public class ListenForMenuActions : FsmStateAction
	{
		// Token: 0x06006115 RID: 24853 RVA: 0x001EC239 File Offset: 0x001EA439
		public override void Reset()
		{
			this.eventTarget = null;
		}

		// Token: 0x06006116 RID: 24854 RVA: 0x001EC244 File Offset: 0x001EA444
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

		// Token: 0x06006117 RID: 24855 RVA: 0x001EC2A0 File Offset: 0x001EA4A0
		public override void OnUpdate()
		{
			if (this.gm == null || this.gm.isPaused || this.inputHandler == null)
			{
				return;
			}
			HeroActions inputActions = this.inputHandler.inputActions;
			Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions, this.ignoreAttack.Value, false);
			if (menuAction == Platform.MenuActions.Submit)
			{
				base.Fsm.Event(this.eventTarget, this.submitPressed);
				return;
			}
			if (menuAction != Platform.MenuActions.Cancel)
			{
				return;
			}
			base.Fsm.Event(this.eventTarget, this.cancelPressed);
		}

		// Token: 0x04005EE7 RID: 24295
		public FsmEventTarget eventTarget;

		// Token: 0x04005EE8 RID: 24296
		public FsmEvent submitPressed;

		// Token: 0x04005EE9 RID: 24297
		public FsmEvent cancelPressed;

		// Token: 0x04005EEA RID: 24298
		public FsmBool ignoreAttack;

		// Token: 0x04005EEB RID: 24299
		private GameManager gm;

		// Token: 0x04005EEC RID: 24300
		private InputHandler inputHandler;
	}
}
