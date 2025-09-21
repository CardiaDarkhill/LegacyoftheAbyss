using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200123C RID: 4668
	[ActionCategory("Hollow Knight")]
	public class GetSilkV2 : FsmStateAction
	{
		// Token: 0x06007B87 RID: 31623 RVA: 0x0024FCFD File Offset: 0x0024DEFD
		public override void Reset()
		{
			this.StoreAmount = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007B88 RID: 31624 RVA: 0x0024FD0D File Offset: 0x0024DF0D
		public override void OnEnter()
		{
			this.playerData = PlayerData.instance;
			this.StoreAmount.Value = this.playerData.silk;
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007B89 RID: 31625 RVA: 0x0024FD3E File Offset: 0x0024DF3E
		public override void OnUpdate()
		{
			this.StoreAmount.Value = this.playerData.silk;
		}

		// Token: 0x04007BBD RID: 31677
		[UIHint(UIHint.Variable)]
		public FsmInt StoreAmount;

		// Token: 0x04007BBE RID: 31678
		public bool EveryFrame;

		// Token: 0x04007BBF RID: 31679
		private PlayerData playerData;
	}
}
