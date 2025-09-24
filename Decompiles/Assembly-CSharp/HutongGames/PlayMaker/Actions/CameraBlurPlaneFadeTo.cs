using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001292 RID: 4754
	public class CameraBlurPlaneFadeTo : FsmStateAction
	{
		// Token: 0x06007CE6 RID: 31974 RVA: 0x00254C08 File Offset: 0x00252E08
		public override void Reset()
		{
			this.Spacing = null;
			this.Vibrancy = null;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			this.Duration = null;
		}

		// Token: 0x06007CE7 RID: 31975 RVA: 0x00254C54 File Offset: 0x00252E54
		public override void OnEnter()
		{
			CameraBlurPlane.MaskLerp = 0f;
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
				base.Finish();
			}
			this.fromSpacing = CameraBlurPlane.Spacing;
			this.fromVibrancy = CameraBlurPlane.Vibrancy;
		}

		// Token: 0x06007CE8 RID: 31976 RVA: 0x00254CE0 File Offset: 0x00252EE0
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
			if (progress >= 1f)
			{
				base.Finish();
			}
		}

		// Token: 0x06007CE9 RID: 31977 RVA: 0x00254D66 File Offset: 0x00252F66
		public override float GetProgress()
		{
			return Mathf.Clamp01(base.State.StateTime / this.Duration.Value);
		}

		// Token: 0x04007CF4 RID: 31988
		public FsmFloat Spacing;

		// Token: 0x04007CF5 RID: 31989
		public FsmFloat Vibrancy;

		// Token: 0x04007CF6 RID: 31990
		public FsmAnimationCurve Curve;

		// Token: 0x04007CF7 RID: 31991
		public FsmFloat Duration;

		// Token: 0x04007CF8 RID: 31992
		private float fromSpacing;

		// Token: 0x04007CF9 RID: 31993
		private float fromVibrancy;
	}
}
