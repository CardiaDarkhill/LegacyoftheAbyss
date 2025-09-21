using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001201 RID: 4609
	public class CheckIsTeleportBlocked : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x06007AA8 RID: 31400 RVA: 0x0024D009 File Offset: 0x0024B209
		public override bool IsTrue
		{
			get
			{
				return GameManager.instance.IsMemoryScene();
			}
		}
	}
}
