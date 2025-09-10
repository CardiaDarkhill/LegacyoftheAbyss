using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012CC RID: 4812
	[Tooltip("Used for stopping Walker V2 (NOT a version 2 for controlling original Walker)")]
	public class StopWalkerV2 : FSMUtility.GetComponentFsmStateAction<WalkerV2>
	{
		// Token: 0x06007DAE RID: 32174 RVA: 0x00257028 File Offset: 0x00255228
		protected override void DoAction(WalkerV2 walker)
		{
			walker.StopWalking();
		}
	}
}
