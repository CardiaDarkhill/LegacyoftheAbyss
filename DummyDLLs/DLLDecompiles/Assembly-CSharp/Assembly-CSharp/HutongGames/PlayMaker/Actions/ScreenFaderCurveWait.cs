using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D09 RID: 3337
	public class ScreenFaderCurveWait : FsmStateAction
	{
		// Token: 0x060062B7 RID: 25271 RVA: 0x001F34EC File Offset: 0x001F16EC
		public override void Reset()
		{
			this.StartColour = null;
			this.EndColour = null;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			this.Duration = null;
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x001F3538 File Offset: 0x001F1738
		public override void OnEnter()
		{
			this.elapsed = 0f;
			if (this.Duration.Value > 0f)
			{
				ScreenFaderUtils.SetColour(this.StartColour.Value);
				return;
			}
			ScreenFaderUtils.SetColour(this.EndColour.Value);
			base.Finish();
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x001F358C File Offset: 0x001F178C
		public override void OnUpdate()
		{
			this.elapsed += Time.deltaTime;
			float t = this.Curve.curve.Evaluate(this.elapsed / this.Duration.Value);
			ScreenFaderUtils.SetColour(Color.Lerp(this.StartColour.Value, this.EndColour.Value, t));
			if (this.elapsed >= this.Duration.Value)
			{
				ScreenFaderUtils.SetColour(this.EndColour.Value);
				base.Finish();
			}
		}

		// Token: 0x04006123 RID: 24867
		public FsmColor StartColour;

		// Token: 0x04006124 RID: 24868
		[RequiredField]
		public FsmColor EndColour;

		// Token: 0x04006125 RID: 24869
		public FsmAnimationCurve Curve;

		// Token: 0x04006126 RID: 24870
		public FsmFloat Duration;

		// Token: 0x04006127 RID: 24871
		private float elapsed;
	}
}
