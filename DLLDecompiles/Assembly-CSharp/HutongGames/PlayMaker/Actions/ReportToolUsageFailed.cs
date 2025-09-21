using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200132D RID: 4909
	public class ReportToolUsageFailed : FsmStateAction
	{
		// Token: 0x06007F1B RID: 32539 RVA: 0x0025A9D2 File Offset: 0x00258BD2
		public override void Reset()
		{
			this.Tool = null;
		}

		// Token: 0x06007F1C RID: 32540 RVA: 0x0025A9DC File Offset: 0x00258BDC
		public override void OnEnter()
		{
			ToolItem toolItem = this.Tool.Value as ToolItem;
			if (toolItem != null)
			{
				AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(toolItem);
				if (attackToolBinding != null)
				{
					ToolItemManager.ReportBoundAttackToolFailed(attackToolBinding.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007EB4 RID: 32436
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;
	}
}
