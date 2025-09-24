using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200132E RID: 4910
	public class AutoEquipTool : FsmStateAction
	{
		// Token: 0x06007F1E RID: 32542 RVA: 0x0025AA2D File Offset: 0x00258C2D
		public override void Reset()
		{
			this.Tool = null;
		}

		// Token: 0x06007F1F RID: 32543 RVA: 0x0025AA36 File Offset: 0x00258C36
		public override void OnEnter()
		{
			if (!this.Tool.IsNone && this.Tool.Value)
			{
				ToolItemManager.AutoEquip((ToolItem)this.Tool.Value);
			}
			base.Finish();
		}

		// Token: 0x04007EB5 RID: 32437
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;
	}
}
