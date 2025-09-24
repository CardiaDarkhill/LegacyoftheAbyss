using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F6 RID: 4854
	[Tooltip("Checks Visible State Of Hud")]
	public sealed class CheckHudVisible : FsmStateAction
	{
		// Token: 0x06007E5B RID: 32347 RVA: 0x00258BC7 File Offset: 0x00256DC7
		public override void Reset()
		{
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeValue = null;
			this.everyFrame = null;
		}

		// Token: 0x06007E5C RID: 32348 RVA: 0x00258BE8 File Offset: 0x00256DE8
		public override void OnEnter()
		{
			if (HudCanvas.IsVisible)
			{
				base.Fsm.Event(this.trueEvent);
				this.storeValue.Value = true;
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
				this.storeValue.Value = false;
			}
			if (!this.everyFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x04007E18 RID: 32280
		public FsmEvent trueEvent;

		// Token: 0x04007E19 RID: 32281
		public FsmEvent falseEvent;

		// Token: 0x04007E1A RID: 32282
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		// Token: 0x04007E1B RID: 32283
		public FsmBool everyFrame;
	}
}
