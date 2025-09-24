using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001352 RID: 4946
	public class AddNeedolinMsg : FsmStateAction
	{
		// Token: 0x06007FBC RID: 32700 RVA: 0x0025C601 File Offset: 0x0025A801
		public override void Reset()
		{
			this.Msg = null;
			this.DidAddTracker = null;
		}

		// Token: 0x06007FBD RID: 32701 RVA: 0x0025C614 File Offset: 0x0025A814
		public override void OnEnter()
		{
			if (!this.DidAddTracker.IsNone)
			{
				if (this.DidAddTracker.Value)
				{
					base.Finish();
					return;
				}
				this.DidAddTracker.Value = true;
			}
			LocalisedTextCollection localisedTextCollection = this.Msg.Value as LocalisedTextCollection;
			if (localisedTextCollection)
			{
				NeedolinMsgBox.AddText(localisedTextCollection, false, false);
			}
			base.Finish();
		}

		// Token: 0x04007F3F RID: 32575
		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Msg;

		// Token: 0x04007F40 RID: 32576
		[UIHint(UIHint.Variable)]
		public FsmBool DidAddTracker;
	}
}
