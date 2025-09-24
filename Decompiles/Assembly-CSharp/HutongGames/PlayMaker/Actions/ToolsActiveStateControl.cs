using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001333 RID: 4915
	public class ToolsActiveStateControl : FsmStateAction
	{
		// Token: 0x06007F2D RID: 32557 RVA: 0x0025AB8D File Offset: 0x00258D8D
		public override void Reset()
		{
			this.SetActiveState = null;
		}

		// Token: 0x06007F2E RID: 32558 RVA: 0x0025AB96 File Offset: 0x00258D96
		public override void OnEnter()
		{
			ToolItemManager.SetActiveState((ToolsActiveStates)this.SetActiveState.Value);
			base.Finish();
		}

		// Token: 0x04007EBD RID: 32445
		[ObjectType(typeof(ToolsActiveStates))]
		public FsmEnum SetActiveState;
	}
}
