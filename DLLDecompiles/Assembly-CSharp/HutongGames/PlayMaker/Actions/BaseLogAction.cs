using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E75 RID: 3701
	public abstract class BaseLogAction : FsmStateAction
	{
		// Token: 0x0600697B RID: 27003 RVA: 0x00210AF3 File Offset: 0x0020ECF3
		public override void Reset()
		{
			this.sendToUnityLog = false;
		}

		// Token: 0x040068B7 RID: 26807
		[Tooltip("Also send to the Unity Log.")]
		public bool sendToUnityLog;
	}
}
