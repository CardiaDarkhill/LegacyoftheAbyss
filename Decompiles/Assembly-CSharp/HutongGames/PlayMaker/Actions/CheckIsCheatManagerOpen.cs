using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200122F RID: 4655
	public class CheckIsCheatManagerOpen : FSMUtility.CheckFsmStateEveryFrameAction
	{
		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x06007B55 RID: 31573 RVA: 0x0024F5F7 File Offset: 0x0024D7F7
		public override bool IsTrue
		{
			get
			{
				return CheatManager.IsOpen;
			}
		}
	}
}
