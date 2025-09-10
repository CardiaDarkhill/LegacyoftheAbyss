using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200138B RID: 5003
	public class RemoveUsingSilk : FsmStateAction
	{
		// Token: 0x06008091 RID: 32913 RVA: 0x0025EAD2 File Offset: 0x0025CCD2
		public override void Reset()
		{
			this.Amount = 1;
			this.UsingType = SilkSpool.SilkUsingFlags.Normal;
			this.DidAddTracker = null;
		}

		// Token: 0x06008092 RID: 32914 RVA: 0x0025EAF8 File Offset: 0x0025CCF8
		public override void OnEnter()
		{
			if (this.DidAddTracker.Value)
			{
				SilkSpool.Instance.RemoveUsing((SilkSpool.SilkUsingFlags)this.UsingType.Value, this.Amount.Value);
				this.DidAddTracker.Value = false;
			}
			base.Finish();
		}

		// Token: 0x04007FE2 RID: 32738
		public FsmInt Amount;

		// Token: 0x04007FE3 RID: 32739
		[ObjectType(typeof(SilkSpool.SilkUsingFlags))]
		public FsmEnum UsingType;

		// Token: 0x04007FE4 RID: 32740
		[UIHint(UIHint.Variable)]
		public FsmBool DidAddTracker;
	}
}
