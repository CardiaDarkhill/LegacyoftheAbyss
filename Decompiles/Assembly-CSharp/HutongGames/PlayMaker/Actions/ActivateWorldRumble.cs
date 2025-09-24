using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012FA RID: 4858
	public class ActivateWorldRumble : FsmStateAction
	{
		// Token: 0x06007E69 RID: 32361 RVA: 0x00258E04 File Offset: 0x00257004
		public override void Reset()
		{
			this.SetActive = null;
		}

		// Token: 0x06007E6A RID: 32362 RVA: 0x00258E10 File Offset: 0x00257010
		public override void OnEnter()
		{
			WorldRumbleManager worldRumbleManager = GameCameras.instance.worldRumbleManager;
			if (this.SetActive.Value)
			{
				worldRumbleManager.AllowRumbles();
			}
			else
			{
				worldRumbleManager.PreventRumbles();
			}
			base.Finish();
		}

		// Token: 0x04007E25 RID: 32293
		public FsmBool SetActive;
	}
}
