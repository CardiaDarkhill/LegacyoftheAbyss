using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E4 RID: 4836
	public class GetWasButtonPressedQueued : FsmStateAction
	{
		// Token: 0x06007E1F RID: 32287 RVA: 0x002583CC File Offset: 0x002565CC
		public override void Reset()
		{
			this.Button = null;
			this.Consume = null;
			this.StoreWasPressed = null;
			this.PressedEvent = null;
			this.NotPressedEvent = null;
			this.IsActive = true;
			this.EveryFrame = true;
		}

		// Token: 0x06007E20 RID: 32288 RVA: 0x00258409 File Offset: 0x00256609
		public override void OnEnter()
		{
			this.hc = base.Owner.GetComponent<HeroController>();
			this.DoAction();
			if (!this.EveryFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06007E21 RID: 32289 RVA: 0x00258435 File Offset: 0x00256635
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007E22 RID: 32290 RVA: 0x0025843D File Offset: 0x0025663D
		public override void OnExit()
		{
			this.hc = null;
		}

		// Token: 0x06007E23 RID: 32291 RVA: 0x00258448 File Offset: 0x00256648
		private void DoAction()
		{
			if (this.hc && this.hc.IsPaused())
			{
				return;
			}
			if (!this.IsActive.Value)
			{
				return;
			}
			InputHandler instance = ManagerSingleton<InputHandler>.Instance;
			HeroActionButton heroAction = (HeroActionButton)this.Button.Value;
			bool wasButtonPressedQueued = instance.GetWasButtonPressedQueued(heroAction, this.Consume.Value);
			this.StoreWasPressed.Value = wasButtonPressedQueued;
			base.Fsm.Event(wasButtonPressedQueued ? this.PressedEvent : this.NotPressedEvent);
		}

		// Token: 0x04007DF4 RID: 32244
		[ObjectType(typeof(HeroActionButton))]
		public FsmEnum Button;

		// Token: 0x04007DF5 RID: 32245
		public FsmBool Consume;

		// Token: 0x04007DF6 RID: 32246
		[UIHint(UIHint.Variable)]
		public FsmBool StoreWasPressed;

		// Token: 0x04007DF7 RID: 32247
		public FsmEvent PressedEvent;

		// Token: 0x04007DF8 RID: 32248
		public FsmEvent NotPressedEvent;

		// Token: 0x04007DF9 RID: 32249
		public FsmBool IsActive;

		// Token: 0x04007DFA RID: 32250
		public FsmBool EveryFrame;

		// Token: 0x04007DFB RID: 32251
		private HeroController hc;
	}
}
