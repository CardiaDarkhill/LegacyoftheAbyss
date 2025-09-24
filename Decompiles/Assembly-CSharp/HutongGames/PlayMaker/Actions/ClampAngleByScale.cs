using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF5 RID: 3061
	[ActionCategory(ActionCategory.Math)]
	public class ClampAngleByScale : FsmStateAction
	{
		// Token: 0x06005DA7 RID: 23975 RVA: 0x001D8844 File Offset: 0x001D6A44
		public override void Reset()
		{
			this.angleVariable = null;
			this.positiveMinValue = null;
			this.positiveMaxValue = null;
			this.negativeMinValue = null;
			this.negativeMaxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DA8 RID: 23976 RVA: 0x001D8870 File Offset: 0x001D6A70
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (((this.space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale).x > 0f)
			{
				this.minValue = this.positiveMinValue.Value;
				this.maxValue = this.positiveMaxValue.Value;
			}
			else
			{
				this.minValue = this.negativeMinValue.Value;
				this.maxValue = this.negativeMaxValue.Value;
			}
			this.angleVariable.Value = this.DoClamp(this.angleVariable.Value, this.minValue, this.maxValue);
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

		// Token: 0x06005DA9 RID: 23977 RVA: 0x001D8994 File Offset: 0x001D6B94
		public override void OnUpdate()
		{
			this.angleVariable.Value = this.DoClamp(this.angleVariable.Value, this.minValue, this.maxValue);
			if (this.angleVariable.Value >= 360f)
			{
				this.angleVariable.Value -= 360f;
			}
			if (this.angleVariable.Value < 0f)
			{
				this.angleVariable.Value += 360f;
			}
		}

		// Token: 0x06005DAA RID: 23978 RVA: 0x001D8A1C File Offset: 0x001D6C1C
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

		// Token: 0x040059F5 RID: 23029
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059F6 RID: 23030
		public Space space;

		// Token: 0x040059F7 RID: 23031
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat angleVariable;

		// Token: 0x040059F8 RID: 23032
		[RequiredField]
		public FsmFloat positiveMinValue;

		// Token: 0x040059F9 RID: 23033
		[RequiredField]
		public FsmFloat positiveMaxValue;

		// Token: 0x040059FA RID: 23034
		[RequiredField]
		public FsmFloat negativeMinValue;

		// Token: 0x040059FB RID: 23035
		[RequiredField]
		public FsmFloat negativeMaxValue;

		// Token: 0x040059FC RID: 23036
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;

		// Token: 0x040059FD RID: 23037
		private float minValue;

		// Token: 0x040059FE RID: 23038
		private float maxValue;
	}
}
