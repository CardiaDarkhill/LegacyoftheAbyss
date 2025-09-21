using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF4 RID: 3060
	[ActionCategory(ActionCategory.Math)]
	public class ClampAngle : FsmStateAction
	{
		// Token: 0x06005DA2 RID: 23970 RVA: 0x001D8648 File Offset: 0x001D6848
		public override void Reset()
		{
			this.angleVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DA3 RID: 23971 RVA: 0x001D8668 File Offset: 0x001D6868
		public override void OnEnter()
		{
			this.angleVariable.Value = this.DoClamp(this.angleVariable.Value, this.minValue.Value, this.maxValue.Value);
			if (this.angleVariable.Value >= 360f)
			{
				this.angleVariable.Value -= 360f;
			}
			if (this.angleVariable.Value < 0f)
			{
				this.angleVariable.Value += 360f;
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DA4 RID: 23972 RVA: 0x001D8708 File Offset: 0x001D6908
		public override void OnUpdate()
		{
			this.angleVariable.Value = this.DoClamp(this.angleVariable.Value, this.minValue.Value, this.maxValue.Value);
			if (this.angleVariable.Value >= 360f)
			{
				this.angleVariable.Value -= 360f;
			}
			if (this.angleVariable.Value < 0f)
			{
				this.angleVariable.Value += 360f;
			}
		}

		// Token: 0x06005DA5 RID: 23973 RVA: 0x001D879C File Offset: 0x001D699C
		private float DoClamp(float angle, float min, float max)
		{
			if (min < 0f && max > 0f && (angle > max || angle < min))
			{
				angle -= 360f;
				if (angle > max || angle < min)
				{
					if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)))
					{
						return min;
					}
					return max;
				}
			}
			else if (min > 0f && (angle > max || angle < min))
			{
				angle += 360f;
				if (angle > max || angle < min)
				{
					if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max)))
					{
						return min;
					}
					return max;
				}
			}
			if (angle < min)
			{
				return min;
			}
			if (angle > max)
			{
				return max;
			}
			return angle;
		}

		// Token: 0x040059F1 RID: 23025
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat angleVariable;

		// Token: 0x040059F2 RID: 23026
		[RequiredField]
		[Tooltip("The minimum value.")]
		public FsmFloat minValue;

		// Token: 0x040059F3 RID: 23027
		[RequiredField]
		[Tooltip("The maximum value.")]
		public FsmFloat maxValue;

		// Token: 0x040059F4 RID: 23028
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
