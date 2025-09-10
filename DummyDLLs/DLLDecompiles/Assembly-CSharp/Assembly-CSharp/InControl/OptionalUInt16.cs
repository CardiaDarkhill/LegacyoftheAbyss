using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200093E RID: 2366
	[Serializable]
	public struct OptionalUInt16
	{
		// Token: 0x06005415 RID: 21525 RVA: 0x0017F9B7 File Offset: 0x0017DBB7
		public OptionalUInt16(ushort value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x06005416 RID: 21526 RVA: 0x0017F9C7 File Offset: 0x0017DBC7
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x06005417 RID: 21527 RVA: 0x0017F9CF File Offset: 0x0017DBCF
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x06005418 RID: 21528 RVA: 0x0017F9DA File Offset: 0x0017DBDA
		// (set) Token: 0x06005419 RID: 21529 RVA: 0x0017F9F5 File Offset: 0x0017DBF5
		public ushort Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalUInt16 that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x0600541A RID: 21530 RVA: 0x0017FA05 File Offset: 0x0017DC05
		public void Clear()
		{
			this.value = 0;
			this.hasValue = false;
		}

		// Token: 0x0600541B RID: 21531 RVA: 0x0017FA15 File Offset: 0x0017DC15
		public ushort GetValueOrDefault(ushort defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x0600541C RID: 21532 RVA: 0x0017FA27 File Offset: 0x0017DC27
		public ushort GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return 0;
			}
			return this.value;
		}

		// Token: 0x0600541D RID: 21533 RVA: 0x0017FA39 File Offset: 0x0017DC39
		public void SetValue(ushort value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x0600541E RID: 21534 RVA: 0x0017FA49 File Offset: 0x0017DC49
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x0600541F RID: 21535 RVA: 0x0017FA64 File Offset: 0x0017DC64
		public bool Equals(OptionalUInt16 other)
		{
			return this.hasValue && other.hasValue && this.value == other.value;
		}

		// Token: 0x06005420 RID: 21536 RVA: 0x0017FA86 File Offset: 0x0017DC86
		public bool Equals(ushort other)
		{
			return this.hasValue && this.value == other;
		}

		// Token: 0x06005421 RID: 21537 RVA: 0x0017FA9B File Offset: 0x0017DC9B
		public static bool operator ==(OptionalUInt16 a, OptionalUInt16 b)
		{
			return a.hasValue && b.hasValue && a.value == b.value;
		}

		// Token: 0x06005422 RID: 21538 RVA: 0x0017FABD File Offset: 0x0017DCBD
		public static bool operator !=(OptionalUInt16 a, OptionalUInt16 b)
		{
			return !(a == b);
		}

		// Token: 0x06005423 RID: 21539 RVA: 0x0017FAC9 File Offset: 0x0017DCC9
		public static bool operator ==(OptionalUInt16 a, ushort b)
		{
			return a.hasValue && a.value == b;
		}

		// Token: 0x06005424 RID: 21540 RVA: 0x0017FADE File Offset: 0x0017DCDE
		public static bool operator !=(OptionalUInt16 a, ushort b)
		{
			return !a.hasValue || a.value != b;
		}

		// Token: 0x06005425 RID: 21541 RVA: 0x0017FAF6 File Offset: 0x0017DCF6
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x06005426 RID: 21542 RVA: 0x0017FAFF File Offset: 0x0017DCFF
		public override int GetHashCode()
		{
			return OptionalUInt16.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x06005427 RID: 21543 RVA: 0x0017FB1C File Offset: 0x0017DD1C
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06005428 RID: 21544 RVA: 0x0017FB3C File Offset: 0x0017DD3C
		public static implicit operator OptionalUInt16(ushort value)
		{
			return new OptionalUInt16(value);
		}

		// Token: 0x06005429 RID: 21545 RVA: 0x0017FB44 File Offset: 0x0017DD44
		public static explicit operator ushort(OptionalUInt16 optional)
		{
			return optional.Value;
		}

		// Token: 0x04005399 RID: 21401
		[SerializeField]
		private bool hasValue;

		// Token: 0x0400539A RID: 21402
		[SerializeField]
		private ushort value;
	}
}
