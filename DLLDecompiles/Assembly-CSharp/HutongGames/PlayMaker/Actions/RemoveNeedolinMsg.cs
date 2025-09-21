using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001353 RID: 4947
	public class RemoveNeedolinMsg : FsmStateAction
	{
		// Token: 0x06007FBF RID: 32703 RVA: 0x0025C67D File Offset: 0x0025A87D
		public override void Reset()
		{
			this.Msg = null;
			this.DidAddTracker = null;
		}

		// Token: 0x06007FC0 RID: 32704 RVA: 0x0025C690 File Offset: 0x0025A890
		public override void OnEnter()
		{
			if (!this.DidAddTracker.IsNone)
			{
				if (!this.DidAddTracker.Value)
				{
					base.Finish();
					return;
				}
				this.DidAddTracker.Value = false;
			}
			LocalisedTextCollection localisedTextCollection = this.Msg.Value as LocalisedTextCollection;
			if (localisedTextCollection)
			{
				NeedolinMsgBox.RemoveText(localisedTextCollection);
			}
			base.Finish();
		}

		// Token: 0x04007F41 RID: 32577
		[ObjectType(typeof(LocalisedTextCollection))]
		public FsmObject Msg;

		// Token: 0x04007F42 RID: 32578
		[UIHint(UIHint.Variable)]
		public FsmBool DidAddTracker;
	}
}
