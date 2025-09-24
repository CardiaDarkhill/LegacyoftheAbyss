using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001294 RID: 4756
	public class CameraBlurPlaneFadeToV3 : FsmStateAction
	{
		// Token: 0x06007CF0 RID: 31984 RVA: 0x00254F60 File Offset: 0x00253160
		public override void Reset()
		{
			this.Spacing = null;
			this.Vibrancy = null;
			this.MaskLerp = null;
			this.MaskScale = 1f;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			this.Duration = null;
		}

		// Token: 0x06007CF1 RID: 31985 RVA: 0x00254FC4 File Offset: 0x002531C4
		public override void OnEnter()
		{
			if (this.Duration.Value <= 0f)
			{
				if (!this.Spacing.IsNone)
				{
					CameraBlurPlane.Spacing = this.Spacing.Value;
				}
				if (!this.Vibrancy.IsNone)
				{
					CameraBlurPlane.Vibrancy = this.Vibrancy.Value;
				}
				if (!this.MaskLerp.IsNone)
				{
					CameraBlurPlane.MaskLerp = this.MaskLerp.Value;
				}
				if (!this.MaskScale.IsNone)
				{
					CameraBlurPlane.MaskScale = this.MaskScale.Value;
				}
				base.Finish();
			}
			this.fromSpacing = CameraBlurPlane.Spacing;
			this.fromVibrancy = CameraBlurPlane.Vibrancy;
			this.fromMaskLerp = CameraBlurPlane.MaskLerp;
			this.fromMaskScale = CameraBlurPlane.MaskScale;
		}

		// Token: 0x06007CF2 RID: 31986 RVA: 0x0025508C File Offset: 0x0025328C
		public override void OnUpdate()
		{
			float progress = this.GetProgress();
			float t = this.Curve.curve.Evaluate(progress);
			if (!this.Spacing.IsNone)
			{
				CameraBlurPlane.Spacing = Mathf.Lerp(this.fromSpacing, this.Spacing.Value, t);
			}
			if (!this.Vibrancy.IsNone)
			{
				CameraBlurPlane.Vibrancy = Mathf.Lerp(this.fromVibrancy, this.Vibrancy.Value, t);
			}
			if (!this.MaskLerp.IsNone)
			{
				CameraBlurPlane.MaskLerp = Mathf.Lerp(this.fromMaskLerp, this.MaskLerp.Value, t);
			}
			if (!this.MaskScale.IsNone)
			{
				CameraBlurPlane.MaskScale = Mathf.Lerp(this.fromMaskScale, this.MaskScale.Value, t);
			}
			if (progress >= 1f)
			{
				base.Finish();
			}
		}

		// Token: 0x06007CF3 RID: 31987 RVA: 0x00255164 File Offset: 0x00253364
		public override float GetProgress()
		{
			return Mathf.Clamp01(base.State.StateTime / this.Duration.Value);
		}

		// Token: 0x04007D02 RID: 32002
		public FsmFloat Spacing;

		// Token: 0x04007D03 RID: 32003
		public FsmFloat Vibrancy;

		// Token: 0x04007D04 RID: 32004
		public FsmFloat MaskLerp;

		// Token: 0x04007D05 RID: 32005
		public FsmFloat MaskScale;

		// Token: 0x04007D06 RID: 32006
		public FsmAnimationCurve Curve;

		// Token: 0x04007D07 RID: 32007
		public FsmFloat Duration;

		// Token: 0x04007D08 RID: 32008
		private float fromSpacing;

		// Token: 0x04007D09 RID: 32009
		private float fromVibrancy;

		// Token: 0x04007D0A RID: 32010
		private float fromMaskLerp;

		// Token: 0x04007D0B RID: 32011
		private float fromMaskScale;
	}
}
