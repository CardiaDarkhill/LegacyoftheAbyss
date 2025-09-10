using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0C RID: 3084
	[ActionCategory(ActionCategory.Debug)]
	public class DebugPause : FsmStateAction
	{
		// Token: 0x06005E12 RID: 24082 RVA: 0x001DA67B File Offset: 0x001D887B
		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
