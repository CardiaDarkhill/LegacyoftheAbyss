using System;
using System.Collections.Generic;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001335 RID: 4917
	public class CountTools : FsmStateAction
	{
		// Token: 0x06007F33 RID: 32563 RVA: 0x0025ABFB File Offset: 0x00258DFB
		public override void Reset()
		{
			this.UnlockedOnly = null;
			this.DamageFlag = null;
			this.StoreCount = null;
		}

		// Token: 0x06007F34 RID: 32564 RVA: 0x0025AC14 File Offset: 0x00258E14
		public override void OnEnter()
		{
			IEnumerable<ToolItem> source = this.UnlockedOnly.Value ? ToolItemManager.GetUnlockedTools() : ToolItemManager.GetAllTools();
			ToolDamageFlags flag = (ToolDamageFlags)this.DamageFlag.Value;
			if (flag != ToolDamageFlags.None)
			{
				source = from tool in source
				where (tool.DamageFlags & flag) > ToolDamageFlags.None
				select tool;
			}
			this.StoreCount.Value = source.Count<ToolItem>();
			base.Finish();
		}

		// Token: 0x04007EC0 RID: 32448
		public FsmBool UnlockedOnly;

		// Token: 0x04007EC1 RID: 32449
		[ObjectType(typeof(ToolDamageFlags))]
		public FsmEnum DamageFlag;

		// Token: 0x04007EC2 RID: 32450
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCount;
	}
}
