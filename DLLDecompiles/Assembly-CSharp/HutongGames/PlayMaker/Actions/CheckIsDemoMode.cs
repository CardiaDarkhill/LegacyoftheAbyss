using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F5 RID: 4853
	public class CheckIsDemoMode : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C2E RID: 3118
		// (get) Token: 0x06007E59 RID: 32345 RVA: 0x00258BB8 File Offset: 0x00256DB8
		public override bool IsTrue
		{
			get
			{
				return DemoHelper.IsDemoMode;
			}
		}
	}
}
