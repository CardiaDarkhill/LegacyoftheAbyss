using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012FE RID: 4862
	public class CheckPlatformGraphicsTier : FsmStateAction
	{
		// Token: 0x06007E75 RID: 32373 RVA: 0x0025903D File Offset: 0x0025723D
		public override void Reset()
		{
			this.CheckValue = null;
			this.EqualEvent = null;
			this.HigherEvent = null;
			this.NotEqualEvent = null;
			this.LowerEvent = null;
		}

		// Token: 0x06007E76 RID: 32374 RVA: 0x00259064 File Offset: 0x00257264
		public override void OnEnter()
		{
			Platform.GraphicsTiers graphicsTier = Platform.Current.GraphicsTier;
			Platform.GraphicsTiers graphicsTiers = (Platform.GraphicsTiers)this.CheckValue.Value;
			if (graphicsTier == graphicsTiers)
			{
				base.Fsm.Event(this.EqualEvent);
			}
			else
			{
				base.Fsm.Event((graphicsTier > graphicsTiers) ? this.HigherEvent : this.LowerEvent);
				base.Fsm.Event(this.NotEqualEvent);
			}
			base.Finish();
		}

		// Token: 0x04007E2F RID: 32303
		[ObjectType(typeof(Platform.GraphicsTiers))]
		public FsmEnum CheckValue;

		// Token: 0x04007E30 RID: 32304
		public FsmEvent EqualEvent;

		// Token: 0x04007E31 RID: 32305
		public FsmEvent HigherEvent;

		// Token: 0x04007E32 RID: 32306
		public FsmEvent LowerEvent;

		// Token: 0x04007E33 RID: 32307
		public FsmEvent NotEqualEvent;
	}
}
