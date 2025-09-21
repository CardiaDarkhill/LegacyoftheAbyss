using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001331 RID: 4913
	public class AutoEquipCrestV3 : FsmStateAction
	{
		// Token: 0x06007F27 RID: 32551 RVA: 0x0025AAF8 File Offset: 0x00258CF8
		public override void Reset()
		{
			this.Crest = null;
			this.SkipToAppear = null;
			this.IsTemp = null;
		}

		// Token: 0x06007F28 RID: 32552 RVA: 0x0025AB10 File Offset: 0x00258D10
		public override void OnEnter()
		{
			if (this.SkipToAppear.Value)
			{
				BindOrbHudFrame.SkipToNextAppear = true;
			}
			ToolItemManager.AutoEquip(this.Crest.Value as ToolCrest, this.IsTemp.Value);
			BindOrbHudFrame.SkipToNextAppear = false;
			base.Finish();
		}

		// Token: 0x04007EB9 RID: 32441
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		// Token: 0x04007EBA RID: 32442
		public FsmBool SkipToAppear;

		// Token: 0x04007EBB RID: 32443
		public FsmBool IsTemp;
	}
}
