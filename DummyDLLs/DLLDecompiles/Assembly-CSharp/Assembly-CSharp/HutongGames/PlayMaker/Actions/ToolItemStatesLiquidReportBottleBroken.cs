using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001337 RID: 4919
	public class ToolItemStatesLiquidReportBottleBroken : FsmStateAction
	{
		// Token: 0x06007F39 RID: 32569 RVA: 0x0025ACD7 File Offset: 0x00258ED7
		public override void Reset()
		{
			this.Tool = null;
		}

		// Token: 0x06007F3A RID: 32570 RVA: 0x0025ACE0 File Offset: 0x00258EE0
		public override void OnEnter()
		{
			ToolItemStatesLiquid toolItemStatesLiquid = this.Tool.Value as ToolItemStatesLiquid;
			if (toolItemStatesLiquid)
			{
				ToolItemStatesLiquid.ReportBottleBroken(toolItemStatesLiquid);
			}
			base.Finish();
		}

		// Token: 0x04007EC4 RID: 32452
		[ObjectType(typeof(ToolItemStatesLiquid))]
		public FsmObject Tool;
	}
}
