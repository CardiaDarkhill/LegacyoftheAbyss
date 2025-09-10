using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001336 RID: 4918
	public class ToolItemStatesLiquidRefillRefills : FsmStateAction
	{
		// Token: 0x06007F36 RID: 32566 RVA: 0x0025AC91 File Offset: 0x00258E91
		public override void Reset()
		{
			this.Tool = null;
		}

		// Token: 0x06007F37 RID: 32567 RVA: 0x0025AC9C File Offset: 0x00258E9C
		public override void OnEnter()
		{
			ToolItemStatesLiquid toolItemStatesLiquid = this.Tool.Value as ToolItemStatesLiquid;
			if (toolItemStatesLiquid)
			{
				toolItemStatesLiquid.RefillRefills(true);
			}
			base.Finish();
		}

		// Token: 0x04007EC3 RID: 32451
		[ObjectType(typeof(ToolItemStatesLiquid))]
		public FsmObject Tool;
	}
}
