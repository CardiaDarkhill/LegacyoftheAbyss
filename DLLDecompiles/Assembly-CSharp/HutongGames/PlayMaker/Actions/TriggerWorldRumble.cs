using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F9 RID: 4857
	public class TriggerWorldRumble : FsmStateAction
	{
		// Token: 0x06007E67 RID: 32359 RVA: 0x00258DD0 File Offset: 0x00256FD0
		public override void OnEnter()
		{
			WorldRumbleManager worldRumbleManager = GameCameras.instance.worldRumbleManager;
			if (worldRumbleManager)
			{
				worldRumbleManager.ForceRumble();
			}
			base.Finish();
		}
	}
}
