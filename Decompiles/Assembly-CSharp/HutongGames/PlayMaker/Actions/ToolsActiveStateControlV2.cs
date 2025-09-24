using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001334 RID: 4916
	public class ToolsActiveStateControlV2 : FsmStateAction
	{
		// Token: 0x06007F30 RID: 32560 RVA: 0x0025ABBB File Offset: 0x00258DBB
		public override void Reset()
		{
			this.SetActiveState = null;
			this.SkipAnims = null;
		}

		// Token: 0x06007F31 RID: 32561 RVA: 0x0025ABCB File Offset: 0x00258DCB
		public override void OnEnter()
		{
			ToolItemManager.SetActiveState((ToolsActiveStates)this.SetActiveState.Value, this.SkipAnims.Value);
			base.Finish();
		}

		// Token: 0x04007EBE RID: 32446
		[ObjectType(typeof(ToolsActiveStates))]
		public FsmEnum SetActiveState;

		// Token: 0x04007EBF RID: 32447
		public FsmBool SkipAnims;
	}
}
