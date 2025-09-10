using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001332 RID: 4914
	public class ToolsCutsceneControl : FsmStateAction
	{
		// Token: 0x06007F2A RID: 32554 RVA: 0x0025AB64 File Offset: 0x00258D64
		public override void Reset()
		{
			this.SetInCutscene = null;
		}

		// Token: 0x06007F2B RID: 32555 RVA: 0x0025AB6D File Offset: 0x00258D6D
		public override void OnEnter()
		{
			ToolItemManager.SetIsInCutscene(this.SetInCutscene.Value);
			base.Finish();
		}

		// Token: 0x04007EBC RID: 32444
		public FsmBool SetInCutscene;
	}
}
