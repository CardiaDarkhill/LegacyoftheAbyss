using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001293 RID: 4755
	public class CameraBlurPlaneFadeToV2 : FsmStateAction
	{
		// Token: 0x06007CEB RID: 31979 RVA: 0x00254D8C File Offset: 0x00252F8C
		public override void Reset()
		{
			this.Spacing = null;
			this.Vibrancy = null;
			this.MaskLerp = null;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			this.Duration = null;
		}

		// Token: 0x06007CEC RID: 31980 RVA: 0x00254DE0 File Offset: 0x00252FE0
		public override void OnEnter()
		{
			CameraBlurPlane.MaskScale = 1f;
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
				base.Finish();
			}
			this.fromSpacing = CameraBlurPlane.Spacing;
			this.fromVibrancy = CameraBlurPlane.Vibrancy;
			this.fromMaskLerp = CameraBlurPlane.MaskLerp;
		}

		// Token: 0x06007CED RID: 31981 RVA: 0x00254E88 File Offset: 0x00253088
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
			if (progress >= 1f)
			{
				base.Finish();
			}
		}

		// Token: 0x06007CEE RID: 31982 RVA: 0x00254F37 File Offset: 0x00253137
		public override float GetProgress()
		{
			return Mathf.Clamp01(base.State.StateTime / this.Duration.Value);
		}

		// Token: 0x04007CFA RID: 31994
		public FsmFloat Spacing;

		// Token: 0x04007CFB RID: 31995
		public FsmFloat Vibrancy;

		// Token: 0x04007CFC RID: 31996
		public FsmFloat MaskLerp;

		// Token: 0x04007CFD RID: 31997
		public FsmAnimationCurve Curve;

		// Token: 0x04007CFE RID: 31998
		public FsmFloat Duration;

		// Token: 0x04007CFF RID: 31999
		private float fromSpacing;

		// Token: 0x04007D00 RID: 32000
		private float fromVibrancy;

		// Token: 0x04007D01 RID: 32001
		private float fromMaskLerp;
	}
}
