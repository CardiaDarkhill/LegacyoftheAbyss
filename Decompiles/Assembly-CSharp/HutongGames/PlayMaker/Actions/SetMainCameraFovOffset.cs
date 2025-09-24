using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200122E RID: 4654
	public class SetMainCameraFovOffset : FsmStateAction
	{
		// Token: 0x06007B52 RID: 31570 RVA: 0x0024F57D File Offset: 0x0024D77D
		public override void Reset()
		{
			this.FovOffset = null;
			this.TransitionTime = null;
			this.TransitionCurve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
		}

		// Token: 0x06007B53 RID: 31571 RVA: 0x0024F5B7 File Offset: 0x0024D7B7
		public override void OnEnter()
		{
			GameCameras.instance.forceCameraAspect.SetFovOffset(this.FovOffset.Value, this.TransitionTime.Value, this.TransitionCurve.curve);
			base.Finish();
		}

		// Token: 0x04007B9A RID: 31642
		public FsmFloat FovOffset;

		// Token: 0x04007B9B RID: 31643
		public FsmFloat TransitionTime;

		// Token: 0x04007B9C RID: 31644
		public FsmAnimationCurve TransitionCurve;
	}
}
