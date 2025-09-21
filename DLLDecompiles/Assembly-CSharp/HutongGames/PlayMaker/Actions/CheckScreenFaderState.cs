using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F8 RID: 4856
	[Tooltip("Checks the alpha value of the screen fader and triggers events based on comparison.")]
	public class CheckScreenFaderState : FsmStateAction
	{
		// Token: 0x06007E61 RID: 32353 RVA: 0x00258CEF File Offset: 0x00256EEF
		public override void Reset()
		{
			this.targetAlpha = 0f;
			this.comparisonMethod = CheckScreenFaderState.CompareMethod.Equal;
			this.trueEvent = null;
			this.falseEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007E62 RID: 32354 RVA: 0x00258D22 File Offset: 0x00256F22
		public override void OnEnter()
		{
			this.CheckAlpha();
			if (!this.everyFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x06007E63 RID: 32355 RVA: 0x00258D3D File Offset: 0x00256F3D
		public override void OnUpdate()
		{
			if (this.everyFrame.Value)
			{
				this.CheckAlpha();
			}
		}

		// Token: 0x06007E64 RID: 32356 RVA: 0x00258D54 File Offset: 0x00256F54
		private void CheckAlpha()
		{
			float alpha = ScreenFaderState.Alpha;
			bool flag = this.Compare(alpha, this.targetAlpha.Value, this.comparisonMethod);
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x06007E65 RID: 32357 RVA: 0x00258D9C File Offset: 0x00256F9C
		private bool Compare(float value1, float value2, CheckScreenFaderState.CompareMethod method)
		{
			switch (method)
			{
			case CheckScreenFaderState.CompareMethod.Equal:
				return Mathf.Approximately(value1, value2);
			case CheckScreenFaderState.CompareMethod.LessThan:
				return value1 < value2;
			case CheckScreenFaderState.CompareMethod.GreaterThan:
				return value1 > value2;
			default:
				return false;
			}
		}

		// Token: 0x04007E20 RID: 32288
		[Tooltip("The target alpha value to compare against.")]
		public FsmFloat targetAlpha;

		// Token: 0x04007E21 RID: 32289
		[Tooltip("Comparison method: Equal, LessThan, GreaterThan.")]
		public CheckScreenFaderState.CompareMethod comparisonMethod;

		// Token: 0x04007E22 RID: 32290
		[Tooltip("Event to send if the comparison is true.")]
		public FsmEvent trueEvent;

		// Token: 0x04007E23 RID: 32291
		[Tooltip("Event to send if the comparison is false.")]
		public FsmEvent falseEvent;

		// Token: 0x04007E24 RID: 32292
		[Tooltip("Whether to check every frame.")]
		public FsmBool everyFrame;

		// Token: 0x02001BF0 RID: 7152
		public enum CompareMethod
		{
			// Token: 0x04009FA1 RID: 40865
			Equal,
			// Token: 0x04009FA2 RID: 40866
			LessThan,
			// Token: 0x04009FA3 RID: 40867
			GreaterThan
		}
	}
}
