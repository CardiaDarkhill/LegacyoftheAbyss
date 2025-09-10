using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001327 RID: 4903
	public class SetToolLocked : FsmStateAction
	{
		// Token: 0x06007F07 RID: 32519 RVA: 0x0025A690 File Offset: 0x00258890
		public override void Reset()
		{
			this.Tool = null;
			this.ShowPopup = true;
		}

		// Token: 0x06007F08 RID: 32520 RVA: 0x0025A6A8 File Offset: 0x002588A8
		public override void OnEnter()
		{
			if (!this.Tool.IsNone && this.Tool.Value)
			{
				ToolItem toolItem = (ToolItem)this.Tool.Value;
				toolItem.Lock();
				if (this.ShowPopup.Value)
				{
					CollectableUIMsg.ShowTakeMsg(toolItem, TakeItemTypes.Taken);
				}
			}
			base.Finish();
		}

		// Token: 0x04007EA1 RID: 32417
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		// Token: 0x04007EA2 RID: 32418
		public FsmBool ShowPopup;
	}
}
