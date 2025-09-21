using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001011 RID: 4113
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Tests if a point is inside a rectangle.")]
	public class RectContains : FsmStateAction
	{
		// Token: 0x06007112 RID: 28946 RVA: 0x0022CF60 File Offset: 0x0022B160
		public override void Reset()
		{
			this.rectangle = new FsmRect
			{
				UseVariable = true
			};
			this.point = new FsmVector3
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007113 RID: 28947 RVA: 0x0022CFD1 File Offset: 0x0022B1D1
		public override void OnEnter()
		{
			this.DoRectContains();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007114 RID: 28948 RVA: 0x0022CFE7 File Offset: 0x0022B1E7
		public override void OnUpdate()
		{
			this.DoRectContains();
		}

		// Token: 0x06007115 RID: 28949 RVA: 0x0022CFF0 File Offset: 0x0022B1F0
		private void DoRectContains()
		{
			if (this.rectangle.IsNone)
			{
				return;
			}
			Vector3 value = this.point.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			bool flag = this.rectangle.Value.Contains(value);
			this.storeResult.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x040070AA RID: 28842
		[RequiredField]
		[Tooltip("Rectangle to test.")]
		public FsmRect rectangle;

		// Token: 0x040070AB RID: 28843
		[Tooltip("Point to test.")]
		public FsmVector3 point;

		// Token: 0x040070AC RID: 28844
		[Tooltip("Specify/override X value.")]
		public FsmFloat x;

		// Token: 0x040070AD RID: 28845
		[Tooltip("Specify/override Y value.")]
		public FsmFloat y;

		// Token: 0x040070AE RID: 28846
		[Tooltip("Event to send if the Point is inside the Rectangle.")]
		public FsmEvent trueEvent;

		// Token: 0x040070AF RID: 28847
		[Tooltip("Event to send if the Point is outside the Rectangle.")]
		public FsmEvent falseEvent;

		// Token: 0x040070B0 RID: 28848
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a variable.")]
		public FsmBool storeResult;

		// Token: 0x040070B1 RID: 28849
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
