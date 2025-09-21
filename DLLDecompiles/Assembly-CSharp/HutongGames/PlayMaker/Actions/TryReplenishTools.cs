using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001328 RID: 4904
	public class TryReplenishTools : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007F0A RID: 32522 RVA: 0x0025A70D File Offset: 0x0025890D
		public override void Reset()
		{
			base.Reset();
			this.DoReplenish = null;
			this.Method = null;
		}

		// Token: 0x17000C38 RID: 3128
		// (get) Token: 0x06007F0B RID: 32523 RVA: 0x0025A723 File Offset: 0x00258923
		public override bool IsTrue
		{
			get
			{
				return ToolItemManager.TryReplenishTools(this.DoReplenish.Value, (ToolItemManager.ReplenishMethod)this.Method.Value);
			}
		}

		// Token: 0x04007EA3 RID: 32419
		public FsmBool DoReplenish;

		// Token: 0x04007EA4 RID: 32420
		[ObjectType(typeof(ToolItemManager.ReplenishMethod))]
		public FsmEnum Method;
	}
}
