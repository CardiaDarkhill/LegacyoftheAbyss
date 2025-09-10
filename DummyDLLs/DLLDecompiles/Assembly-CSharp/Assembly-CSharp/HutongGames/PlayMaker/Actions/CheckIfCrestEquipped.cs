using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001320 RID: 4896
	public class CheckIfCrestEquipped : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007EF3 RID: 32499 RVA: 0x0025A327 File Offset: 0x00258527
		public override void Reset()
		{
			base.Reset();
			this.Crest = null;
		}

		// Token: 0x17000C35 RID: 3125
		// (get) Token: 0x06007EF4 RID: 32500 RVA: 0x0025A338 File Offset: 0x00258538
		public override bool IsTrue
		{
			get
			{
				ToolCrest toolCrest = this.Crest.Value as ToolCrest;
				return toolCrest != null && toolCrest.IsEquipped;
			}
		}

		// Token: 0x04007E97 RID: 32407
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;
	}
}
