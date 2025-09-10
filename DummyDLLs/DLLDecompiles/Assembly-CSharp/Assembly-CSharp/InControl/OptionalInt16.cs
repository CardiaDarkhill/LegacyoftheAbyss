using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200093C RID: 2364
	[Serializable]
	public struct OptionalInt16
	{
		// Token: 0x060053EB RID: 21483 RVA: 0x0017F68B File Offset: 0x0017D88B
		public OptionalInt16(short value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x060053EC RID: 21484 RVA: 0x0017F69B File Offset: 0x0017D89B
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x060053ED RID: 21485 RVA: 0x0017F6A3 File Offset: 0x0017D8A3
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x060053EE RID: 21486 RVA: 0x0017F6AE File Offset: 0x0017D8AE
		// (set) Token: 0x060053EF RID: 21487 RVA: 0x0017F6C9 File Offset: 0x0017D8C9
		public short Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInt16 that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x060053F0 RID: 21488 RVA: 0x0017F6D9 File Offset: 0x0017D8D9
		public void Clear()
		{
			this.value = 0;
			this.hasValue = false;
		}

		// Token: 0x060053F1 RID: 21489 RVA: 0x0017F6E9 File Offset: 0x0017D8E9
		public short GetValueOrDefault(short defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x060053F2 RID: 21490 RVA: 0x0017F6FB File Offset: 0x0017D8FB
		public short GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return 0;
			}
			return this.value;
		}

		// Token: 0x060053F3 RID: 21491 RVA: 0x0017F70D File Offset: 0x0017D90D
		public void SetValue(short value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x060053F4 RID: 21492 RVA: 0x0017F71D File Offset: 0x0017D91D
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x060053F5 RID: 21493 RVA: 0x0017F738 File Offset: 0x0017D938
		public bool Equals(OptionalInt16 other)
		{
			return this.hasValue && other.hasValue && this.value == other.value;
		}

		// Token: 0x060053F6 RID: 21494 RVA: 0x0017F75A File Offset: 0x0017D95A
		public bool Equals(short other)
		{
			return this.hasValue && this.value == other;
		}

		// Token: 0x060053F7 RID: 21495 RVA: 0x0017F76F File Offset: 0x0017D96F
		public static bool operator ==(OptionalInt16 a, OptionalInt16 b)
		{
			return a.hasValue && b.hasValue && a.value == b.value;
		}

		// Token: 0x060053F8 RID: 21496 RVA: 0x0017F791 File Offset: 0x0017D991
		public static bool operator !=(OptionalInt16 a, OptionalInt16 b)
		{
			return !(a == b);
		}

		// Token: 0x060053F9 RID: 21497 RVA: 0x0017F79D File Offset: 0x0017D99D
		public static bool operator ==(OptionalInt16 a, short b)
		{
			return a.hasValue && a.value == b;
		}

		// Token: 0x060053FA RID: 21498 RVA: 0x0017F7B2 File Offset: 0x0017D9B2
		public static bool operator !=(OptionalInt16 a, short b)
		{
			return !a.hasValue || a.value != b;
		}

		// Token: 0x060053FB RID: 21499 RVA: 0x0017F7CA File Offset: 0x0017D9CA
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x060053FC RID: 21500 RVA: 0x0017F7D3 File Offset: 0x0017D9D3
		public override int GetHashCode()
		{
			return OptionalInt16.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x060053FD RID: 21501 RVA: 0x0017F7F0 File Offset: 0x0017D9F0
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x0017F810 File Offset: 0x0017DA10
		public static implicit operator OptionalInt16(short value)
		{
			return new OptionalInt16(value);
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x0017F818 File Offset: 0x0017DA18
		public static explicit operator short(OptionalInt16 optional)
		{
			return optional.Value;
		}

		// Token: 0x04005395 RID: 21397
		[SerializeField]
		private bool hasValue;

		// Token: 0x04005396 RID: 21398
		[SerializeField]
		private short value;
	}
}
