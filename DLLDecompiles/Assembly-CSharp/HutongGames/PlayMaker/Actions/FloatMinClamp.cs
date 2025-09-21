using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F78 RID: 3960
	[ActionCategory(ActionCategory.Math)]
	public class FloatMinClamp : FsmStateAction
	{
		// Token: 0x06006DB8 RID: 28088 RVA: 0x00221311 File Offset: 0x0021F511
		public override void Reset()
		{
			this.floatVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DB9 RID: 28089 RVA: 0x0022132F File Offset: 0x0021F52F
		public override void OnEnter()
		{
			this.DoClamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DBA RID: 28090 RVA: 0x00221345 File Offset: 0x0021F545
		public override void OnUpdate()
		{
			this.DoClamp();
		}

		// Token: 0x06006DBB RID: 28091 RVA: 0x00221350 File Offset: 0x0021F550
		private void DoClamp()
		{
			float value = this.floatVariable.Value;
			bool flag = false;
			if (value < 0f && value > this.minValue.Value)
			{
				value = this.minValue.Value;
				flag = true;
			}
			if (value > 0f && value < this.maxValue.Value)
			{
				value = this.maxValue.Value;
				flag = true;
			}
			if (flag)
			{
				this.floatVariable.Value = value;
			}
		}

		// Token: 0x04006D75 RID: 28021
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Float variable to clamp.")]
		public FsmFloat floatVariable;

		// Token: 0x04006D76 RID: 28022
		[RequiredField]
		[Tooltip("The minimum value.")]
		public FsmFloat minValue;

		// Token: 0x04006D77 RID: 28023
		[RequiredField]
		[Tooltip("The maximum value.")]
		public FsmFloat maxValue;

		// Token: 0x04006D78 RID: 28024
		[Tooltip("Repeat every frame. Useful if the float variable is changing.")]
		public bool everyFrame;
	}
}
