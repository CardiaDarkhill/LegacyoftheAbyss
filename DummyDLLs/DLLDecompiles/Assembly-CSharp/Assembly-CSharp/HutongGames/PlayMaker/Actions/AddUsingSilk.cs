using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200138A RID: 5002
	public class AddUsingSilk : FsmStateAction
	{
		// Token: 0x0600808E RID: 32910 RVA: 0x0025EA6C File Offset: 0x0025CC6C
		public override void Reset()
		{
			this.Amount = 1;
			this.UsingType = SilkSpool.SilkUsingFlags.Normal;
			this.DidAddTracker = null;
		}

		// Token: 0x0600808F RID: 32911 RVA: 0x0025EA92 File Offset: 0x0025CC92
		public override void OnEnter()
		{
			this.DidAddTracker.Value = SilkSpool.Instance.AddUsing((SilkSpool.SilkUsingFlags)this.UsingType.Value, this.Amount.Value);
			base.Finish();
		}

		// Token: 0x04007FDF RID: 32735
		public FsmInt Amount;

		// Token: 0x04007FE0 RID: 32736
		[ObjectType(typeof(SilkSpool.SilkUsingFlags))]
		public FsmEnum UsingType;

		// Token: 0x04007FE1 RID: 32737
		[UIHint(UIHint.Variable)]
		public FsmBool DidAddTracker;
	}
}
