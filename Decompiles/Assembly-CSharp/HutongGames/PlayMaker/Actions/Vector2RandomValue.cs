using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F8F RID: 3983
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Sets a Vector2 Variable to a random value.")]
	public class Vector2RandomValue : FsmStateAction
	{
		// Token: 0x06006E1C RID: 28188 RVA: 0x00222178 File Offset: 0x00220378
		public override void Reset()
		{
			this.shape = Vector2RandomValue.Option.Circle;
			this.minLength = 0f;
			this.maxLength = 1f;
			this.floatParam1 = null;
			this.floatParam2 = null;
			this.yScale = 1f;
			this.storeResult = null;
		}

		// Token: 0x06006E1D RID: 28189 RVA: 0x002221D1 File Offset: 0x002203D1
		public override void OnEnter()
		{
			this.DoRandomVector2();
			this.storeResult.Value = this.v2;
			base.Finish();
		}

		// Token: 0x06006E1E RID: 28190 RVA: 0x002221F0 File Offset: 0x002203F0
		private void DoRandomVector2()
		{
			switch (this.shape)
			{
			case Vector2RandomValue.Option.Circle:
				this.v2 = Random.insideUnitCircle.normalized * Random.Range(this.minLength.Value, this.maxLength.Value);
				break;
			case Vector2RandomValue.Option.Rectangle:
			{
				float value = this.minLength.Value;
				float value2 = this.maxLength.Value;
				this.v2.x = Random.Range(value, value2);
				if (Random.Range(0, 100) < 50)
				{
					this.v2.x = -this.v2.x;
				}
				this.v2.y = Random.Range(value, value2);
				if (Random.Range(0, 100) < 50)
				{
					this.v2.y = -this.v2.y;
				}
				break;
			}
			case Vector2RandomValue.Option.InArc:
			{
				float f = 0.017453292f * Random.Range(this.floatParam1.Value, this.floatParam2.Value);
				float num = Random.Range(this.minLength.Value, this.maxLength.Value);
				this.v2.x = Mathf.Cos(f) * num;
				this.v2.y = Mathf.Sin(f) * num;
				break;
			}
			case Vector2RandomValue.Option.AtAngles:
			{
				int num2 = (int)this.floatParam1.Value;
				int maxExclusive = 360 / num2;
				int num3 = Random.Range(0, maxExclusive);
				float f2 = 0.017453292f * (float)num3 * (float)num2;
				float num = Random.Range(this.minLength.Value, this.maxLength.Value);
				this.v2.x = Mathf.Cos(f2) * num;
				this.v2.y = Mathf.Sin(f2) * num;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			this.v2.y = this.v2.y * this.yScale.Value;
		}

		// Token: 0x04006DC8 RID: 28104
		private static bool showPreview;

		// Token: 0x04006DC9 RID: 28105
		[PreviewField("DrawPreview")]
		[Tooltip("Controls the distribution of the random Vector2 values.")]
		public Vector2RandomValue.Option shape;

		// Token: 0x04006DCA RID: 28106
		[Tooltip("The minimum length for the random Vector2 value.")]
		public FsmFloat minLength;

		// Token: 0x04006DCB RID: 28107
		[Tooltip("The maximum length for the random Vector2 value.")]
		public FsmFloat maxLength;

		// Token: 0x04006DCC RID: 28108
		[Tooltip("Context sensitive parameter. Depends on the Shape.")]
		public FsmFloat floatParam1;

		// Token: 0x04006DCD RID: 28109
		[Tooltip("Context sensitive parameter. Depends on the Shape.")]
		public FsmFloat floatParam2;

		// Token: 0x04006DCE RID: 28110
		[Tooltip("Scale the vector in Y (e.g., to squash a circle into an oval)")]
		public FsmFloat yScale;

		// Token: 0x04006DCF RID: 28111
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Vector2 variable.")]
		public FsmVector2 storeResult;

		// Token: 0x04006DD0 RID: 28112
		private Vector2 v2;

		// Token: 0x02001BB1 RID: 7089
		public enum Option
		{
			// Token: 0x04009E45 RID: 40517
			Circle,
			// Token: 0x04009E46 RID: 40518
			Rectangle,
			// Token: 0x04009E47 RID: 40519
			InArc,
			// Token: 0x04009E48 RID: 40520
			AtAngles
		}
	}
}
