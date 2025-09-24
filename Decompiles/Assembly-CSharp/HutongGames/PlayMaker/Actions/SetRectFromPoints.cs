using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001014 RID: 4116
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Sets a Rect's value using Vector2 points.")]
	public class SetRectFromPoints : FsmStateAction
	{
		// Token: 0x06007123 RID: 28963 RVA: 0x0022D388 File Offset: 0x0022B588
		public override void Reset()
		{
			this.rectangle = null;
			this.point1 = new FsmVector2
			{
				UseVariable = true
			};
			this.point2 = new FsmVector2
			{
				UseVariable = true
			};
			this.positiveDimensions = new FsmBool
			{
				Value = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06007124 RID: 28964 RVA: 0x0022D3D9 File Offset: 0x0022B5D9
		public override void OnEnter()
		{
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007125 RID: 28965 RVA: 0x0022D3EF File Offset: 0x0022B5EF
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x06007126 RID: 28966 RVA: 0x0022D3F8 File Offset: 0x0022B5F8
		private void DoSetValue()
		{
			if (this.rectangle.IsNone)
			{
				return;
			}
			if (this.positiveDimensions.Value)
			{
				Rect value = new Rect
				{
					x = Mathf.Min(this.point1.Value.x, this.point2.Value.x),
					y = Mathf.Min(this.point1.Value.y, this.point2.Value.y),
					width = Mathf.Abs(this.point2.Value.x - this.point1.Value.x),
					height = Mathf.Abs(this.point2.Value.y - this.point1.Value.y)
				};
				this.rectangle.Value = value;
				return;
			}
			this.rectangle.Value = new Rect
			{
				min = this.point1.Value,
				max = this.point2.Value
			};
		}

		// Token: 0x040070BE RID: 28862
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Rectangle to set.")]
		public FsmRect rectangle;

		// Token: 0x040070BF RID: 28863
		[Tooltip("First point.")]
		public FsmVector2 point1;

		// Token: 0x040070C0 RID: 28864
		[Tooltip("Second point.")]
		public FsmVector2 point2;

		// Token: 0x040070C1 RID: 28865
		[Tooltip("Avoid negative width and height values. This is useful for UI rects that don't draw if they have negative dimensions.")]
		public FsmBool positiveDimensions;

		// Token: 0x040070C2 RID: 28866
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
