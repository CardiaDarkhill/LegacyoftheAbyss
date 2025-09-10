using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001330 RID: 4912
	public class AutoEquipCrestV2 : FsmStateAction
	{
		// Token: 0x06007F24 RID: 32548 RVA: 0x0025AAA9 File Offset: 0x00258CA9
		public override void Reset()
		{
			this.Crest = null;
			this.SkipToAppear = null;
		}

		// Token: 0x06007F25 RID: 32549 RVA: 0x0025AAB9 File Offset: 0x00258CB9
		public override void OnEnter()
		{
			if (this.SkipToAppear.Value)
			{
				BindOrbHudFrame.SkipToNextAppear = true;
			}
			ToolItemManager.AutoEquip(this.Crest.Value as ToolCrest, false);
			BindOrbHudFrame.SkipToNextAppear = false;
			base.Finish();
		}

		// Token: 0x04007EB7 RID: 32439
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;

		// Token: 0x04007EB8 RID: 32440
		public FsmBool SkipToAppear;
	}
}
