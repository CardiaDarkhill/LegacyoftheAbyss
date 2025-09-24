using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001326 RID: 4902
	public class SetToolUnlocked : FsmStateAction
	{
		// Token: 0x06007F04 RID: 32516 RVA: 0x0025A60C File Offset: 0x0025880C
		public override void Reset()
		{
			this.Tool = null;
			this.WaitForTutorialMsgEnd = null;
		}

		// Token: 0x06007F05 RID: 32517 RVA: 0x0025A61C File Offset: 0x0025881C
		public override void OnEnter()
		{
			if (!this.Tool.IsNone && this.Tool.Value)
			{
				ToolItem toolItem = (ToolItem)this.Tool.Value;
				if (this.WaitForTutorialMsgEnd.Value)
				{
					toolItem.Unlock(new Action(base.Finish), ToolItem.PopupFlags.Default);
					return;
				}
				toolItem.Unlock(null, ToolItem.PopupFlags.Default);
			}
			base.Finish();
		}

		// Token: 0x04007E9F RID: 32415
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		// Token: 0x04007EA0 RID: 32416
		public FsmBool WaitForTutorialMsgEnd;
	}
}
