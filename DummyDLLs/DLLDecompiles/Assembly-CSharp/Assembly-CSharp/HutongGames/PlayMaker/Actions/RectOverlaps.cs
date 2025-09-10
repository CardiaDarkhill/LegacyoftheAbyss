using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001012 RID: 4114
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Tests if 2 Rects overlap.")]
	public class RectOverlaps : FsmStateAction
	{
		// Token: 0x06007117 RID: 28951 RVA: 0x0022D09C File Offset: 0x0022B29C
		public override void Reset()
		{
			this.rect1 = new FsmRect
			{
				UseVariable = true
			};
			this.rect2 = new FsmRect
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007118 RID: 28952 RVA: 0x0022D0E9 File Offset: 0x0022B2E9
		public override void OnEnter()
		{
			this.DoRectOverlap();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007119 RID: 28953 RVA: 0x0022D0FF File Offset: 0x0022B2FF
		public override void OnUpdate()
		{
			this.DoRectOverlap();
		}

		// Token: 0x0600711A RID: 28954 RVA: 0x0022D108 File Offset: 0x0022B308
		private void DoRectOverlap()
		{
			if (this.rect1.IsNone || this.rect2.IsNone)
			{
				return;
			}
			bool flag = RectOverlaps.Intersect(this.rect1.Value, this.rect2.Value);
			this.storeResult.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x0600711B RID: 28955 RVA: 0x0022D174 File Offset: 0x0022B374
		public static bool Intersect(Rect a, Rect b)
		{
			RectOverlaps.FlipNegative(ref a);
			RectOverlaps.FlipNegative(ref b);
			bool flag = a.xMin < b.xMax;
			bool flag2 = a.xMax > b.xMin;
			bool flag3 = a.yMin < b.yMax;
			bool flag4 = a.yMax > b.yMin;
			return flag && flag2 && flag3 && flag4;
		}

		// Token: 0x0600711C RID: 28956 RVA: 0x0022D1D8 File Offset: 0x0022B3D8
		public static void FlipNegative(ref Rect r)
		{
			if (r.width < 0f)
			{
				r.x -= (r.width *= -1f);
			}
			if (r.height < 0f)
			{
				r.y -= (r.height *= -1f);
			}
		}

		// Token: 0x040070B2 RID: 28850
		[RequiredField]
		[Tooltip("First Rectangle.")]
		public FsmRect rect1;

		// Token: 0x040070B3 RID: 28851
		[RequiredField]
		[Tooltip("Second Rectangle.")]
		public FsmRect rect2;

		// Token: 0x040070B4 RID: 28852
		[Tooltip("Event to send if the Rects overlap.")]
		public FsmEvent trueEvent;

		// Token: 0x040070B5 RID: 28853
		[Tooltip("Event to send if the Rects do not overlap.")]
		public FsmEvent falseEvent;

		// Token: 0x040070B6 RID: 28854
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a variable.")]
		public FsmBool storeResult;

		// Token: 0x040070B7 RID: 28855
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
