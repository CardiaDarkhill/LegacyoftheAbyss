using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5E RID: 3166
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class GetCameraPixelDimensions : FsmStateAction
	{
		// Token: 0x06005FC7 RID: 24519 RVA: 0x001E5C2D File Offset: 0x001E3E2D
		public override void Reset()
		{
			this.cameraWidth = null;
			this.cameraHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06005FC8 RID: 24520 RVA: 0x001E5C44 File Offset: 0x001E3E44
		public override void OnEnter()
		{
			this.DoGetCamera();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FC9 RID: 24521 RVA: 0x001E5C5A File Offset: 0x001E3E5A
		public override void OnUpdate()
		{
			this.DoGetCamera();
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x001E5C62 File Offset: 0x001E3E62
		private void DoGetCamera()
		{
		}

		// Token: 0x04005D1D RID: 23837
		[UIHint(UIHint.Variable)]
		public FsmFloat cameraWidth;

		// Token: 0x04005D1E RID: 23838
		[UIHint(UIHint.Variable)]
		public FsmFloat cameraHeight;

		// Token: 0x04005D1F RID: 23839
		public bool everyFrame;
	}
}
