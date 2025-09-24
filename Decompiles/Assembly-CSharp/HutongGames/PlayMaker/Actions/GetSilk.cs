using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200123B RID: 4667
	[ActionCategory("Hollow Knight")]
	public class GetSilk : FsmStateAction
	{
		// Token: 0x06007B84 RID: 31620 RVA: 0x0024FCCF File Offset: 0x0024DECF
		public override void Reset()
		{
			this.StoreAmount = null;
		}

		// Token: 0x06007B85 RID: 31621 RVA: 0x0024FCD8 File Offset: 0x0024DED8
		public override void OnEnter()
		{
			this.StoreAmount.Value = PlayerData.instance.silk;
			base.Finish();
		}

		// Token: 0x04007BBC RID: 31676
		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmount;
	}
}
