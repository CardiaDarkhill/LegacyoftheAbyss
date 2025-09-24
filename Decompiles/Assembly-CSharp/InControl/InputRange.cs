using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008F9 RID: 2297
	[Serializable]
	public struct InputRange
	{
		// Token: 0x06005072 RID: 20594 RVA: 0x00172D19 File Offset: 0x00170F19
		private InputRange(float value0, float value1, InputRangeType type)
		{
			this.Value0 = value0;
			this.Value1 = value1;
			this.Type = type;
		}

		// Token: 0x06005073 RID: 20595 RVA: 0x00172D30 File Offset: 0x00170F30
		public InputRange(InputRangeType type)
		{
			this.Value0 = InputRange.typeToRange[(int)type].Value0;
			this.Value1 = InputRange.typeToRange[(int)type].Value1;
			this.Type = type;
		}

		// Token: 0x06005074 RID: 20596 RVA: 0x00172D65 File Offset: 0x00170F65
		public bool Includes(float value)
		{
			return !this.Excludes(value);
		}

		// Token: 0x06005075 RID: 20597 RVA: 0x00172D71 File Offset: 0x00170F71
		private bool Excludes(float value)
		{
			return this.Type == InputRangeType.None || value < Mathf.Min(this.Value0, this.Value1) || value > Mathf.Max(this.Value0, this.Value1);
		}

		// Token: 0x06005076 RID: 20598 RVA: 0x00172DA7 File Offset: 0x00170FA7
		public static bool Excludes(InputRangeType rangeType, float value)
		{
			return InputRange.typeToRange[(int)rangeType].Excludes(value);
		}

		// Token: 0x06005077 RID: 20599 RVA: 0x00172DBC File Offset: 0x00170FBC
		private static float Remap(float value, InputRange sourceRange, InputRange targetRange)
		{
			if (sourceRange.Excludes(value))
			{
				return 0f;
			}
			float t = Mathf.InverseLerp(sourceRange.Value0, sourceRange.Value1, value);
			return Mathf.Lerp(targetRange.Value0, targetRange.Value1, t);
		}

		// Token: 0x06005078 RID: 20600 RVA: 0x00172E00 File Offset: 0x00171000
		public static float Remap(float value, InputRangeType sourceRangeType, InputRangeType targetRangeType)
		{
			InputRange sourceRange = InputRange.typeToRange[(int)sourceRangeType];
			InputRange targetRange = InputRange.typeToRange[(int)targetRangeType];
			return InputRange.Remap(value, sourceRange, targetRange);
		}

		// Token: 0x04005170 RID: 20848
		public static readonly InputRange None = new InputRange(0f, 0f, InputRangeType.None);

		// Token: 0x04005171 RID: 20849
		public static readonly InputRange MinusOneToOne = new InputRange(-1f, 1f, InputRangeType.MinusOneToOne);

		// Token: 0x04005172 RID: 20850
		public static readonly InputRange OneToMinusOne = new InputRange(1f, -1f, InputRangeType.OneToMinusOne);

		// Token: 0x04005173 RID: 20851
		public static readonly InputRange ZeroToOne = new InputRange(0f, 1f, InputRangeType.ZeroToOne);

		// Token: 0x04005174 RID: 20852
		public static readonly InputRange ZeroToMinusOne = new InputRange(0f, -1f, InputRangeType.ZeroToMinusOne);

		// Token: 0x04005175 RID: 20853
		public static readonly InputRange OneToZero = new InputRange(1f, 0f, InputRangeType.OneToZero);

		// Token: 0x04005176 RID: 20854
		public static readonly InputRange MinusOneToZero = new InputRange(-1f, 0f, InputRangeType.MinusOneToZero);

		// Token: 0x04005177 RID: 20855
		private static readonly InputRange[] typeToRange = new InputRange[]
		{
			InputRange.None,
			InputRange.MinusOneToOne,
			InputRange.OneToMinusOne,
			InputRange.ZeroToOne,
			InputRange.ZeroToMinusOne,
			InputRange.OneToZero,
			InputRange.MinusOneToZero
		};

		// Token: 0x04005178 RID: 20856
		public readonly float Value0;

		// Token: 0x04005179 RID: 20857
		public readonly float Value1;

		// Token: 0x0400517A RID: 20858
		public readonly InputRangeType Type;
	}
}
