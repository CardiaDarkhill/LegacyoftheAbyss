using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001225 RID: 4645
	public class SetBloomForced : FsmStateAction
	{
		// Token: 0x06007B2A RID: 31530 RVA: 0x0024EEC4 File Offset: 0x0024D0C4
		public override void Reset()
		{
			this.IsBloomForced = null;
		}

		// Token: 0x06007B2B RID: 31531 RVA: 0x0024EECD File Offset: 0x0024D0CD
		public override void OnEnter()
		{
			GameCameras.instance.cameraController.IsBloomForced = this.IsBloomForced.Value;
			base.Finish();
		}

		// Token: 0x04007B7B RID: 31611
		public FsmBool IsBloomForced;
	}
}
