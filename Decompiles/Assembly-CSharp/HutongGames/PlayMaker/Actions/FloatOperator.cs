using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F7A RID: 3962
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Performs math operations on 2 Floats: Add, Subtract, Multiply, Divide, Min, Max.")]
	public class FloatOperator : FsmStateAction
	{
		// Token: 0x06006DC1 RID: 28097 RVA: 0x00221435 File Offset: 0x0021F635
		public override void Reset()
		{
			this.float1 = null;
			this.float2 = null;
			this.operation = FloatOperator.Operation.Add;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DC2 RID: 28098 RVA: 0x0022145A File Offset: 0x0021F65A
		public override void OnEnter()
		{
			this.DoFloatOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DC3 RID: 28099 RVA: 0x00221470 File Offset: 0x0021F670
		public override void OnUpdate()
		{
			this.DoFloatOperator();
		}

		// Token: 0x06006DC4 RID: 28100 RVA: 0x00221478 File Offset: 0x0021F678
		private void DoFloatOperator()
		{
			float value = this.float1.Value;
			float value2 = this.float2.Value;
			switch (this.operation)
			{
			case FloatOperator.Operation.Add:
				this.storeResult.Value = value + value2;
				return;
			case FloatOperator.Operation.Subtract:
				this.storeResult.Value = value - value2;
				return;
			case FloatOperator.Operation.Multiply:
				this.storeResult.Value = value * value2;
				return;
			case FloatOperator.Operation.Divide:
				this.storeResult.Value = value / value2;
				return;
			case FloatOperator.Operation.Min:
				this.storeResult.Value = Mathf.Min(value, value2);
				return;
			case FloatOperator.Operation.Max:
				this.storeResult.Value = Mathf.Max(value, value2);
				return;
			case FloatOperator.Operation.Modulus:
				this.storeResult.Value = value % value2;
				return;
			default:
				return;
			}
		}

		// Token: 0x04006D7C RID: 28028
		[RequiredField]
		[Tooltip("The first float.")]
		public FsmFloat float1;

		// Token: 0x04006D7D RID: 28029
		[RequiredField]
		[Tooltip("The second float.")]
		public FsmFloat float2;

		// Token: 0x04006D7E RID: 28030
		[Tooltip("The math operation to perform on the floats.")]
		public FloatOperator.Operation operation;

		// Token: 0x04006D7F RID: 28031
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the operation in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x04006D80 RID: 28032
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		// Token: 0x02001BAF RID: 7087
		public enum Operation
		{
			// Token: 0x04009E36 RID: 40502
			Add,
			// Token: 0x04009E37 RID: 40503
			Subtract,
			// Token: 0x04009E38 RID: 40504
			Multiply,
			// Token: 0x04009E39 RID: 40505
			Divide,
			// Token: 0x04009E3A RID: 40506
			Min,
			// Token: 0x04009E3B RID: 40507
			Max,
			// Token: 0x04009E3C RID: 40508
			Modulus
		}
	}
}
