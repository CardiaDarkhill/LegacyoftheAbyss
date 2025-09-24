using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001291 RID: 4753
	[UsedImplicitly]
	public class CameraBlurPlaneFade : FsmStateAction
	{
		// Token: 0x06007CE0 RID: 31968 RVA: 0x002549F1 File Offset: 0x00252BF1
		public bool HideFrom()
		{
			return this.Duration != null && this.Duration.Value <= 0f;
		}

		// Token: 0x06007CE1 RID: 31969 RVA: 0x00254A14 File Offset: 0x00252C14
		public override void Reset()
		{
			this.From = null;
			this.To = null;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			this.Duration = null;
		}

		// Token: 0x06007CE2 RID: 31970 RVA: 0x00254A60 File Offset: 0x00252C60
		public override void OnEnter()
		{
			if (this.HideFrom())
			{
				CameraBlurPlane.Spacing = this.To.Spacing.Value;
				CameraBlurPlane.Vibrancy = this.To.Vibrancy.Value;
				FsmFloat maskLerp = this.To.MaskLerp;
				CameraBlurPlane.MaskLerp = ((maskLerp != null) ? maskLerp.Value : 0f);
				base.Finish();
				return;
			}
			CameraBlurPlane.Spacing = this.From.Spacing.Value;
			CameraBlurPlane.Vibrancy = this.From.Vibrancy.Value;
			FsmFloat maskLerp2 = this.From.MaskLerp;
			CameraBlurPlane.MaskLerp = ((maskLerp2 != null) ? maskLerp2.Value : 0f);
		}

		// Token: 0x06007CE3 RID: 31971 RVA: 0x00254B10 File Offset: 0x00252D10
		public override void OnUpdate()
		{
			float progress = this.GetProgress();
			float t = this.Curve.curve.Evaluate(progress);
			CameraBlurPlane.Spacing = Mathf.Lerp(this.From.Spacing.Value, this.To.Spacing.Value, t);
			CameraBlurPlane.Vibrancy = Mathf.Lerp(this.From.Vibrancy.Value, this.To.Vibrancy.Value, t);
			if (this.From.MaskLerp != null && this.To.MaskLerp != null)
			{
				CameraBlurPlane.MaskLerp = Mathf.Lerp(this.From.MaskLerp.Value, this.To.MaskLerp.Value, t);
			}
			if (progress >= 1f)
			{
				base.Finish();
			}
		}

		// Token: 0x06007CE4 RID: 31972 RVA: 0x00254BDF File Offset: 0x00252DDF
		public override float GetProgress()
		{
			return Mathf.Clamp01(base.State.StateTime / this.Duration.Value);
		}

		// Token: 0x04007CF0 RID: 31984
		[HideIf("HideFrom")]
		public CameraBlurPlaneFade.Config From;

		// Token: 0x04007CF1 RID: 31985
		public CameraBlurPlaneFade.Config To;

		// Token: 0x04007CF2 RID: 31986
		public FsmAnimationCurve Curve;

		// Token: 0x04007CF3 RID: 31987
		public FsmFloat Duration;

		// Token: 0x02001BEB RID: 7147
		[Serializable]
		public class Config
		{
			// Token: 0x04009F8E RID: 40846
			public FsmFloat Spacing;

			// Token: 0x04009F8F RID: 40847
			public FsmFloat Vibrancy;

			// Token: 0x04009F90 RID: 40848
			public FsmFloat MaskLerp;
		}
	}
}
