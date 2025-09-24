using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200132C RID: 4908
	public class CustomToolUsage : FsmStateAction
	{
		// Token: 0x06007F18 RID: 32536 RVA: 0x0025A977 File Offset: 0x00258B77
		public override void Reset()
		{
			this.Tool = null;
			this.UseAmount = 1;
		}

		// Token: 0x06007F19 RID: 32537 RVA: 0x0025A98C File Offset: 0x00258B8C
		public override void OnEnter()
		{
			ToolItem toolItem = this.Tool.Value as ToolItem;
			if (toolItem != null)
			{
				toolItem.CustomUsage(this.UseAmount.Value);
			}
			base.Finish();
		}

		// Token: 0x04007EB2 RID: 32434
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		// Token: 0x04007EB3 RID: 32435
		public FsmInt UseAmount;
	}
}
