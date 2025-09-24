using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200093D RID: 2365
	[Serializable]
	public struct OptionalInt32
	{
		// Token: 0x06005400 RID: 21504 RVA: 0x0017F821 File Offset: 0x0017DA21
		public OptionalInt32(int value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x06005401 RID: 21505 RVA: 0x0017F831 File Offset: 0x0017DA31
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x06005402 RID: 21506 RVA: 0x0017F839 File Offset: 0x0017DA39
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x06005403 RID: 21507 RVA: 0x0017F844 File Offset: 0x0017DA44
		// (set) Token: 0x06005404 RID: 21508 RVA: 0x0017F85F File Offset: 0x0017DA5F
		public int Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInt32 that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x06005405 RID: 21509 RVA: 0x0017F86F File Offset: 0x0017DA6F
		public void Clear()
		{
			this.value = 0;
			this.hasValue = false;
		}

		// Token: 0x06005406 RID: 21510 RVA: 0x0017F87F File Offset: 0x0017DA7F
		public int GetValueOrDefault(int defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x06005407 RID: 21511 RVA: 0x0017F891 File Offset: 0x0017DA91
		public int GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return 0;
			}
			return this.value;
		}

		// Token: 0x06005408 RID: 21512 RVA: 0x0017F8A3 File Offset: 0x0017DAA3
		public void SetValue(int value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x06005409 RID: 21513 RVA: 0x0017F8B3 File Offset: 0x0017DAB3
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x0600540A RID: 21514 RVA: 0x0017F8CE File Offset: 0x0017DACE
		public bool Equals(OptionalInt32 other)
		{
			return this.hasValue && other.hasValue && this.value == other.value;
		}

		// Token: 0x0600540B RID: 21515 RVA: 0x0017F8F0 File Offset: 0x0017DAF0
		public bool Equals(int other)
		{
			return this.hasValue && this.value == other;
		}

		// Token: 0x0600540C RID: 21516 RVA: 0x0017F905 File Offset: 0x0017DB05
		public static bool operator ==(OptionalInt32 a, OptionalInt32 b)
		{
			return a.hasValue && b.hasValue && a.value == b.value;
		}

		// Token: 0x0600540D RID: 21517 RVA: 0x0017F927 File Offset: 0x0017DB27
		public static bool operator !=(OptionalInt32 a, OptionalInt32 b)
		{
			return !(a == b);
		}

		// Token: 0x0600540E RID: 21518 RVA: 0x0017F933 File Offset: 0x0017DB33
		public static bool operator ==(OptionalInt32 a, int b)
		{
			return a.hasValue && a.value == b;
		}

		// Token: 0x0600540F RID: 21519 RVA: 0x0017F948 File Offset: 0x0017DB48
		public static bool operator !=(OptionalInt32 a, int b)
		{
			return !a.hasValue || a.value != b;
		}

		// Token: 0x06005410 RID: 21520 RVA: 0x0017F960 File Offset: 0x0017DB60
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x06005411 RID: 21521 RVA: 0x0017F969 File Offset: 0x0017DB69
		public override int GetHashCode()
		{
			return OptionalInt32.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x06005412 RID: 21522 RVA: 0x0017F986 File Offset: 0x0017DB86
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06005413 RID: 21523 RVA: 0x0017F9A6 File Offset: 0x0017DBA6
		public static implicit operator OptionalInt32(int value)
		{
			return new OptionalInt32(value);
		}

		// Token: 0x06005414 RID: 21524 RVA: 0x0017F9AE File Offset: 0x0017DBAE
		public static explicit operator int(OptionalInt32 optional)
		{
			return optional.Value;
		}

		// Token: 0x04005397 RID: 21399
		[SerializeField]
		private bool hasValue;

		// Token: 0x04005398 RID: 21400
		[SerializeField]
		private int value;
	}
}
