using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001329 RID: 4905
	public class SendToolEquipChanged : FsmStateAction
	{
		// Token: 0x06007F0D RID: 32525 RVA: 0x0025A74D File Offset: 0x0025894D
		public override void OnEnter()
		{
			ToolItemManager.SendEquippedChangedEvent(false);
			base.Finish();
		}
	}
}
