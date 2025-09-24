using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F42 RID: 3906
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Performs boolean operations on 2 Bool Variables.")]
	public class BoolOperator : FsmStateAction
	{
		// Token: 0x06006CB3 RID: 27827 RVA: 0x0021E4E7 File Offset: 0x0021C6E7
		public override void Reset()
		{
			this.bool1 = false;
			this.bool2 = false;
			this.operation = BoolOperator.Operation.AND;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CB4 RID: 27828 RVA: 0x0021E516 File Offset: 0x0021C716
		public override void OnEnter()
		{
			this.DoBoolOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CB5 RID: 27829 RVA: 0x0021E52C File Offset: 0x0021C72C
		public override void OnUpdate()
		{
			this.DoBoolOperator();
		}

		// Token: 0x06006CB6 RID: 27830 RVA: 0x0021E534 File Offset: 0x0021C734
		private void DoBoolOperator()
		{
			bool value = this.bool1.Value;
			bool value2 = this.bool2.Value;
			switch (this.operation)
			{
			case BoolOperator.Operation.AND:
				this.storeResult.Value = (value && value2);
				return;
			case BoolOperator.Operation.NAND:
				this.storeResult.Value = (!value || !value2);
				return;
			case BoolOperator.Operation.OR:
				this.storeResult.Value = (value || value2);
				return;
			case BoolOperator.Operation.XOR:
				this.storeResult.Value = (value ^ value2);
				return;
			default:
				return;
			}
		}

		// Token: 0x04006C6B RID: 27755
		[RequiredField]
		[Tooltip("The first Bool variable.")]
		public FsmBool bool1;

		// Token: 0x04006C6C RID: 27756
		[RequiredField]
		[Tooltip("The second Bool variable.")]
		public FsmBool bool2;

		// Token: 0x04006C6D RID: 27757
		[Tooltip("Boolean Operation.")]
		public BoolOperator.Operation operation;

		// Token: 0x04006C6E RID: 27758
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable.")]
		public FsmBool storeResult;

		// Token: 0x04006C6F RID: 27759
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x02001BAE RID: 7086
		public enum Operation
		{
			// Token: 0x04009E31 RID: 40497
			AND,
			// Token: 0x04009E32 RID: 40498
			NAND,
			// Token: 0x04009E33 RID: 40499
			OR,
			// Token: 0x04009E34 RID: 40500
			XOR
		}
	}
}
