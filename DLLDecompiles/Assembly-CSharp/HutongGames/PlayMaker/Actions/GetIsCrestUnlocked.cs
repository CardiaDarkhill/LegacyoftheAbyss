using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200131F RID: 4895
	public class GetIsCrestUnlocked : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007EF0 RID: 32496 RVA: 0x0025A2E0 File Offset: 0x002584E0
		public override void Reset()
		{
			base.Reset();
			this.Crest = null;
		}

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x06007EF1 RID: 32497 RVA: 0x0025A2F0 File Offset: 0x002584F0
		public override bool IsTrue
		{
			get
			{
				ToolCrest toolCrest = this.Crest.Value as ToolCrest;
				return toolCrest != null && toolCrest.IsUnlocked;
			}
		}

		// Token: 0x04007E96 RID: 32406
		[ObjectType(typeof(ToolCrest))]
		public FsmObject Crest;
	}
}
