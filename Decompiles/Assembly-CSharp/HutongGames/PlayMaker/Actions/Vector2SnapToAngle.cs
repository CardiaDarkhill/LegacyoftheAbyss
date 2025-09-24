using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001195 RID: 4501
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Snap a Vector2 to an angle increment while maintaining length.")]
	public class Vector2SnapToAngle : FsmStateAction
	{
		// Token: 0x06007881 RID: 30849 RVA: 0x00248064 File Offset: 0x00246264
		public override void Reset()
		{
			this.vector2Variable = null;
			this.snapAngle = 15f;
			this.everyFrame = false;
		}

		// Token: 0x06007882 RID: 30850 RVA: 0x00248084 File Offset: 0x00246284
		public override void OnEnter()
		{
			this.DoSnapToAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007883 RID: 30851 RVA: 0x0024809A File Offset: 0x0024629A
		public override void OnUpdate()
		{
			this.DoSnapToAngle();
		}

		// Token: 0x06007884 RID: 30852 RVA: 0x002480A4 File Offset: 0x002462A4
		private void DoSnapToAngle()
		{
			float value = this.snapAngle.Value;
			if (value <= 0f)
			{
				return;
			}
			Vector2 value2 = this.vector2Variable.Value;
			float magnitude = value2.magnitude;
			float num = Mathf.Atan2(value2.y, value2.y);
			float f = 0.017453292f * Mathf.Round(num / value) * value;
			this.vector2Variable.Value = new Vector2(Mathf.Cos(f) * magnitude, Mathf.Sin(f) * magnitude);
		}

		// Token: 0x040078F0 RID: 30960
		private static bool showPreview;

		// Token: 0x040078F1 RID: 30961
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector to snap to an angle.")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078F2 RID: 30962
		[PreviewField("DrawPreview")]
		[Tooltip("Angle increment to snap to.")]
		public FsmFloat snapAngle;

		// Token: 0x040078F3 RID: 30963
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
