using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F82 RID: 3970
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
	public class IntOperator : FsmStateAction
	{
		// Token: 0x06006DE6 RID: 28134 RVA: 0x0022196E File Offset: 0x0021FB6E
		public override void Reset()
		{
			this.integer1 = null;
			this.integer2 = null;
			this.operation = IntOperator.Operation.Add;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DE7 RID: 28135 RVA: 0x00221993 File Offset: 0x0021FB93
		public override void OnEnter()
		{
			this.DoIntOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DE8 RID: 28136 RVA: 0x002219A9 File Offset: 0x0021FBA9
		public override void OnUpdate()
		{
			this.DoIntOperator();
		}

		// Token: 0x06006DE9 RID: 28137 RVA: 0x002219B4 File Offset: 0x0021FBB4
		private void DoIntOperator()
		{
			int value = this.integer1.Value;
			int value2 = this.integer2.Value;
			switch (this.operation)
			{
			case IntOperator.Operation.Add:
				this.storeResult.Value = value + value2;
				return;
			case IntOperator.Operation.Subtract:
				this.storeResult.Value = value - value2;
				return;
			case IntOperator.Operation.Multiply:
				this.storeResult.Value = value * value2;
				return;
			case IntOperator.Operation.Divide:
				this.storeResult.Value = value / value2;
				return;
			case IntOperator.Operation.Min:
				this.storeResult.Value = Mathf.Min(value, value2);
				return;
			case IntOperator.Operation.Max:
				this.storeResult.Value = Mathf.Max(value, value2);
				return;
			default:
				return;
			}
		}

		// Token: 0x04006D99 RID: 28057
		[RequiredField]
		[Tooltip("The first integer.")]
		public FsmInt integer1;

		// Token: 0x04006D9A RID: 28058
		[RequiredField]
		[Tooltip("The second integer.")]
		public FsmInt integer2;

		// Token: 0x04006D9B RID: 28059
		[Tooltip("The operation to perform on the 2 integers.")]
		public IntOperator.Operation operation;

		// Token: 0x04006D9C RID: 28060
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Integer Variable.")]
		public FsmInt storeResult;

		// Token: 0x04006D9D RID: 28061
		[Tooltip("Perform this action every frame. Useful if you're using variables that are changing.")]
		public bool everyFrame;

		// Token: 0x02001BB0 RID: 7088
		public enum Operation
		{
			// Token: 0x04009E3E RID: 40510
			Add,
			// Token: 0x04009E3F RID: 40511
			Subtract,
			// Token: 0x04009E40 RID: 40512
			Multiply,
			// Token: 0x04009E41 RID: 40513
			Divide,
			// Token: 0x04009E42 RID: 40514
			Min,
			// Token: 0x04009E43 RID: 40515
			Max
		}
	}
}
