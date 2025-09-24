using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200138D RID: 5005
	public class CheckSimpleShopMenuHasStock : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x0600809B RID: 32923 RVA: 0x0025EC8E File Offset: 0x0025CE8E
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
		}

		// Token: 0x17000C43 RID: 3139
		// (get) Token: 0x0600809C RID: 32924 RVA: 0x0025EC9D File Offset: 0x0025CE9D
		public override bool IsTrue
		{
			get
			{
				return this.Target.GetSafe(this).GetComponent<SimpleShopMenuOwner>().HasStockLeft();
			}
		}

		// Token: 0x04007FEA RID: 32746
		[CheckForComponent(typeof(SimpleShopMenuOwner))]
		public FsmOwnerDefault Target;
	}
}
