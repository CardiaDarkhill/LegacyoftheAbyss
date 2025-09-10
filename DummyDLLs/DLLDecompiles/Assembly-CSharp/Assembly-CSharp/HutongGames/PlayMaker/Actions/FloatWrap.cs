using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F7C RID: 3964
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Wraps the value of Float Variable so it stays in a Min/Max range.\n\nExamples:\nWrap 120 between 0 and 100 -> 20\nWrap -10 between 0 and 100 -> 90")]
	public class FloatWrap : FsmStateAction
	{
		// Token: 0x06006DCB RID: 28107 RVA: 0x002215DA File Offset: 0x0021F7DA
		public override void Reset()
		{
			this.floatVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DCC RID: 28108 RVA: 0x002215F8 File Offset: 0x0021F7F8
		public override void OnEnter()
		{
			this.DoWrap();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DCD RID: 28109 RVA: 0x0022160E File Offset: 0x0021F80E
		public override void OnUpdate()
		{
			this.DoWrap();
		}

		// Token: 0x06006DCE RID: 28110 RVA: 0x00221618 File Offset: 0x0021F818
		private void DoWrap()
		{
			float value = this.floatVariable.Value;
			float value2 = this.minValue.Value;
			float value3 = this.maxValue.Value;
			if (value < value2)
			{
				this.floatVariable.Value = value3 - (value2 - value) % (value3 - value2);
				return;
			}
			this.floatVariable.Value = value2 + (value - value2) % (value3 - value2);
		}

		// Token: 0x04006D85 RID: 28037
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to wrap.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D86 RID: 28038
		[RequiredField]
		[Tooltip("The minimum value allowed.")]
		public FsmFloat minValue;

		// Token: 0x04006D87 RID: 28039
		[RequiredField]
		[Tooltip("The maximum value allowed.")]
		public FsmFloat maxValue;

		// Token: 0x04006D88 RID: 28040
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
