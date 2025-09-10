using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000939 RID: 2361
	[Serializable]
	public struct OptionalFloat
	{
		// Token: 0x060053AA RID: 21418 RVA: 0x0017F138 File Offset: 0x0017D338
		public OptionalFloat(float value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B84 RID: 2948
		// (get) Token: 0x060053AB RID: 21419 RVA: 0x0017F148 File Offset: 0x0017D348
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B85 RID: 2949
		// (get) Token: 0x060053AC RID: 21420 RVA: 0x0017F150 File Offset: 0x0017D350
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B86 RID: 2950
		// (get) Token: 0x060053AD RID: 21421 RVA: 0x0017F15B File Offset: 0x0017D35B
		// (set) Token: 0x060053AE RID: 21422 RVA: 0x0017F176 File Offset: 0x0017D376
		public float Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalFloat that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x060053AF RID: 21423 RVA: 0x0017F186 File Offset: 0x0017D386
		public void Clear()
		{
			this.value = 0f;
			this.hasValue = false;
		}

		// Token: 0x060053B0 RID: 21424 RVA: 0x0017F19A File Offset: 0x0017D39A
		public float GetValueOrDefault(float defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x060053B1 RID: 21425 RVA: 0x0017F1AC File Offset: 0x0017D3AC
		public float GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return 0f;
			}
			return this.value;
		}

		// Token: 0x060053B2 RID: 21426 RVA: 0x0017F1C2 File Offset: 0x0017D3C2
		public void SetValue(float value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x060053B3 RID: 21427 RVA: 0x0017F1D2 File Offset: 0x0017D3D2
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x060053B4 RID: 21428 RVA: 0x0017F1ED File Offset: 0x0017D3ED
		public bool Equals(OptionalFloat other)
		{
			return this.hasValue && other.hasValue && OptionalFloat.IsApproximatelyEqual(this.value, other.value);
		}

		// Token: 0x060053B5 RID: 21429 RVA: 0x0017F212 File Offset: 0x0017D412
		public bool Equals(float other)
		{
			return this.hasValue && OptionalFloat.IsApproximatelyEqual(this.value, other);
		}

		// Token: 0x060053B6 RID: 21430 RVA: 0x0017F22A File Offset: 0x0017D42A
		public static bool operator ==(OptionalFloat a, OptionalFloat b)
		{
			return a.hasValue && b.hasValue && OptionalFloat.IsApproximatelyEqual(a.value, b.value);
		}

		// Token: 0x060053B7 RID: 21431 RVA: 0x0017F24F File Offset: 0x0017D44F
		public static bool operator !=(OptionalFloat a, OptionalFloat b)
		{
			return !(a == b);
		}

		// Token: 0x060053B8 RID: 21432 RVA: 0x0017F25B File Offset: 0x0017D45B
		public static bool operator ==(OptionalFloat a, float b)
		{
			return a.hasValue && OptionalFloat.IsApproximatelyEqual(a.value, b);
		}

		// Token: 0x060053B9 RID: 21433 RVA: 0x0017F273 File Offset: 0x0017D473
		public static bool operator !=(OptionalFloat a, float b)
		{
			return !a.hasValue || !OptionalFloat.IsApproximatelyEqual(a.value, b);
		}

		// Token: 0x060053BA RID: 21434 RVA: 0x0017F28E File Offset: 0x0017D48E
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x060053BB RID: 21435 RVA: 0x0017F297 File Offset: 0x0017D497
		public override int GetHashCode()
		{
			return OptionalFloat.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x060053BC RID: 21436 RVA: 0x0017F2B4 File Offset: 0x0017D4B4
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060053BD RID: 21437 RVA: 0x0017F2D4 File Offset: 0x0017D4D4
		public static implicit operator OptionalFloat(float value)
		{
			return new OptionalFloat(value);
		}

		// Token: 0x060053BE RID: 21438 RVA: 0x0017F2DC File Offset: 0x0017D4DC
		public static explicit operator float(OptionalFloat optional)
		{
			return optional.Value;
		}

		// Token: 0x060053BF RID: 21439 RVA: 0x0017F2E8 File Offset: 0x0017D4E8
		private static bool IsApproximatelyEqual(float a, float b)
		{
			float num = a - b;
			return num >= -1E-07f && num <= 1E-07f;
		}

		// Token: 0x060053C0 RID: 21440 RVA: 0x0017F310 File Offset: 0x0017D510
		public bool ApproximatelyEquals(float other)
		{
			if (!this.hasValue)
			{
				return false;
			}
			float num = this.value - other;
			return num >= -1E-07f && num <= 1E-07f;
		}

		// Token: 0x0400538E RID: 21390
		[SerializeField]
		private bool hasValue;

		// Token: 0x0400538F RID: 21391
		[SerializeField]
		private float value;

		// Token: 0x04005390 RID: 21392
		private const float epsilon = 1E-07f;
	}
}
