using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001192 RID: 4498
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Performs most possible operations on 2 Vector2: Dot product, Distance, Angle, Add, Subtract, Multiply, Divide, Min, Max")]
	public class Vector2Operator : FsmStateAction
	{
		// Token: 0x06007874 RID: 30836 RVA: 0x00247CFF File Offset: 0x00245EFF
		public override void Reset()
		{
			this.vector1 = null;
			this.vector2 = null;
			this.operation = Vector2Operator.Vector2Operation.Add;
			this.storeVector2Result = null;
			this.storeFloatResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06007875 RID: 30837 RVA: 0x00247D2B File Offset: 0x00245F2B
		public override void OnEnter()
		{
			this.DoVector2Operator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007876 RID: 30838 RVA: 0x00247D41 File Offset: 0x00245F41
		public override void OnUpdate()
		{
			this.DoVector2Operator();
		}

		// Token: 0x06007877 RID: 30839 RVA: 0x00247D4C File Offset: 0x00245F4C
		private void DoVector2Operator()
		{
			Vector2 value = this.vector1.Value;
			Vector2 value2 = this.vector2.Value;
			switch (this.operation)
			{
			case Vector2Operator.Vector2Operation.DotProduct:
				this.storeFloatResult.Value = Vector2.Dot(value, value2);
				return;
			case Vector2Operator.Vector2Operation.Distance:
				this.storeFloatResult.Value = Vector2.Distance(value, value2);
				return;
			case Vector2Operator.Vector2Operation.Angle:
				this.storeFloatResult.Value = Vector2.Angle(value, value2);
				return;
			case Vector2Operator.Vector2Operation.Add:
				this.storeVector2Result.Value = value + value2;
				return;
			case Vector2Operator.Vector2Operation.Subtract:
				this.storeVector2Result.Value = value - value2;
				return;
			case Vector2Operator.Vector2Operation.Multiply:
			{
				Vector2 zero = Vector2.zero;
				zero.x = value.x * value2.x;
				zero.y = value.y * value2.y;
				this.storeVector2Result.Value = zero;
				return;
			}
			case Vector2Operator.Vector2Operation.Divide:
			{
				Vector2 zero2 = Vector2.zero;
				zero2.x = value.x / value2.x;
				zero2.y = value.y / value2.y;
				this.storeVector2Result.Value = zero2;
				return;
			}
			case Vector2Operator.Vector2Operation.Min:
				this.storeVector2Result.Value = Vector2.Min(value, value2);
				return;
			case Vector2Operator.Vector2Operation.Max:
				this.storeVector2Result.Value = Vector2.Max(value, value2);
				return;
			default:
				return;
			}
		}

		// Token: 0x040078E3 RID: 30947
		[RequiredField]
		[Tooltip("The first vector")]
		public FsmVector2 vector1;

		// Token: 0x040078E4 RID: 30948
		[RequiredField]
		[Tooltip("The second vector")]
		public FsmVector2 vector2;

		// Token: 0x040078E5 RID: 30949
		[Tooltip("The operation")]
		public Vector2Operator.Vector2Operation operation = Vector2Operator.Vector2Operation.Add;

		// Token: 0x040078E6 RID: 30950
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2 result when it applies.")]
		public FsmVector2 storeVector2Result;

		// Token: 0x040078E7 RID: 30951
		[UIHint(UIHint.Variable)]
		[Tooltip("The float result when it applies")]
		public FsmFloat storeFloatResult;

		// Token: 0x040078E8 RID: 30952
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x02001BD4 RID: 7124
		public enum Vector2Operation
		{
			// Token: 0x04009EFF RID: 40703
			DotProduct,
			// Token: 0x04009F00 RID: 40704
			Distance,
			// Token: 0x04009F01 RID: 40705
			Angle,
			// Token: 0x04009F02 RID: 40706
			Add,
			// Token: 0x04009F03 RID: 40707
			Subtract,
			// Token: 0x04009F04 RID: 40708
			Multiply,
			// Token: 0x04009F05 RID: 40709
			Divide,
			// Token: 0x04009F06 RID: 40710
			Min,
			// Token: 0x04009F07 RID: 40711
			Max
		}
	}
}
