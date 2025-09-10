using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200132F RID: 4911
	public class AutoEquipCrest : FsmStateAction
	{
		// Token: 0x06007F21 RID: 32545 RVA: 0x0025AA7A File Offset: 0x00258C7A
		public override void Reset()
		{
			this.Crest = null;
		}

		// Token: 0x06007F22 RID: 32546 RVA: 0x0025AA83 File Offset: 0x00258C83
		public override void OnEnter()
		{
			ToolItemManager.AutoEquip(this.Crest.Value as ToolCrest, false);
			base.Finish();
		}

		// Token: 0x04007EB6 RID: 32438
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;
	}
}
