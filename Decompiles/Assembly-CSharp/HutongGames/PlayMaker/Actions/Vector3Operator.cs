using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A9 RID: 4521
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Performs most possible operations on 2 Vector3: Dot product, Cross product, Distance, Angle, Project, Reflect, Add, Subtract, Multiply, Divide, Min, Max")]
	public class Vector3Operator : FsmStateAction
	{
		// Token: 0x060078DB RID: 30939 RVA: 0x00248F7B File Offset: 0x0024717B
		public override void Reset()
		{
			this.vector1 = null;
			this.vector2 = null;
			this.operation = Vector3Operator.Vector3Operation.Add;
			this.storeVector3Result = null;
			this.storeFloatResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060078DC RID: 30940 RVA: 0x00248FA7 File Offset: 0x002471A7
		public override void OnEnter()
		{
			this.DoVector3Operator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078DD RID: 30941 RVA: 0x00248FBD File Offset: 0x002471BD
		public override void OnUpdate()
		{
			this.DoVector3Operator();
		}

		// Token: 0x060078DE RID: 30942 RVA: 0x00248FC8 File Offset: 0x002471C8
		private void DoVector3Operator()
		{
			Vector3 value = this.vector1.Value;
			Vector3 value2 = this.vector2.Value;
			switch (this.operation)
			{
			case Vector3Operator.Vector3Operation.DotProduct:
				this.storeFloatResult.Value = Vector3.Dot(value, value2);
				return;
			case Vector3Operator.Vector3Operation.CrossProduct:
				this.storeVector3Result.Value = Vector3.Cross(value, value2);
				return;
			case Vector3Operator.Vector3Operation.Distance:
				this.storeFloatResult.Value = Vector3.Distance(value, value2);
				return;
			case Vector3Operator.Vector3Operation.Angle:
				this.storeFloatResult.Value = Vector3.Angle(value, value2);
				return;
			case Vector3Operator.Vector3Operation.Project:
				this.storeVector3Result.Value = Vector3.Project(value, value2);
				return;
			case Vector3Operator.Vector3Operation.Reflect:
				this.storeVector3Result.Value = Vector3.Reflect(value, value2);
				return;
			case Vector3Operator.Vector3Operation.Add:
				this.storeVector3Result.Value = value + value2;
				return;
			case Vector3Operator.Vector3Operation.Subtract:
				this.storeVector3Result.Value = value - value2;
				return;
			case Vector3Operator.Vector3Operation.Multiply:
			{
				Vector3 zero = Vector3.zero;
				zero.x = value.x * value2.x;
				zero.y = value.y * value2.y;
				zero.z = value.z * value2.z;
				this.storeVector3Result.Value = zero;
				return;
			}
			case Vector3Operator.Vector3Operation.Divide:
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x = value.x / value2.x;
				zero2.y = value.y / value2.y;
				zero2.z = value.z / value2.z;
				this.storeVector3Result.Value = zero2;
				return;
			}
			case Vector3Operator.Vector3Operation.Min:
				this.storeVector3Result.Value = Vector3.Min(value, value2);
				return;
			case Vector3Operator.Vector3Operation.Max:
				this.storeVector3Result.Value = Vector3.Max(value, value2);
				return;
			default:
				return;
			}
		}

		// Token: 0x0400793B RID: 31035
		[RequiredField]
		[Tooltip("The first vector in the operation.")]
		public FsmVector3 vector1;

		// Token: 0x0400793C RID: 31036
		[RequiredField]
		[Tooltip("The second vector in the operation.")]
		public FsmVector3 vector2;

		// Token: 0x0400793D RID: 31037
		[Tooltip("The operation to perform.")]
		public Vector3Operator.Vector3Operation operation = Vector3Operator.Vector3Operation.Add;

		// Token: 0x0400793E RID: 31038
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Vector3 Variable.")]
		public FsmVector3 storeVector3Result;

		// Token: 0x0400793F RID: 31039
		[UIHint(UIHint.Variable)]
		[Tooltip("Store a float result in a Float Variable (E.g., Dot, Distance, Angle)")]
		public FsmFloat storeFloatResult;

		// Token: 0x04007940 RID: 31040
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x02001BD5 RID: 7125
		public enum Vector3Operation
		{
			// Token: 0x04009F09 RID: 40713
			DotProduct,
			// Token: 0x04009F0A RID: 40714
			CrossProduct,
			// Token: 0x04009F0B RID: 40715
			Distance,
			// Token: 0x04009F0C RID: 40716
			Angle,
			// Token: 0x04009F0D RID: 40717
			Project,
			// Token: 0x04009F0E RID: 40718
			Reflect,
			// Token: 0x04009F0F RID: 40719
			Add,
			// Token: 0x04009F10 RID: 40720
			Subtract,
			// Token: 0x04009F11 RID: 40721
			Multiply,
			// Token: 0x04009F12 RID: 40722
			Divide,
			// Token: 0x04009F13 RID: 40723
			Min,
			// Token: 0x04009F14 RID: 40724
			Max
		}
	}
}
