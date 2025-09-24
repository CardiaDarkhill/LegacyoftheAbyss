using System;
using System.Globalization;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200093F RID: 2367
	[Serializable]
	public struct OptionalUInt32
	{
		// Token: 0x0600542A RID: 21546 RVA: 0x0017FB4D File Offset: 0x0017DD4D
		public OptionalUInt32(uint value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x0600542B RID: 21547 RVA: 0x0017FB5D File Offset: 0x0017DD5D
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x0600542C RID: 21548 RVA: 0x0017FB65 File Offset: 0x0017DD65
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x0600542D RID: 21549 RVA: 0x0017FB70 File Offset: 0x0017DD70
		// (set) Token: 0x0600542E RID: 21550 RVA: 0x0017FB8B File Offset: 0x0017DD8B
		public uint Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalUInt32 that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x0600542F RID: 21551 RVA: 0x0017FB9B File Offset: 0x0017DD9B
		public void Clear()
		{
			this.value = 0U;
			this.hasValue = false;
		}

		// Token: 0x06005430 RID: 21552 RVA: 0x0017FBAB File Offset: 0x0017DDAB
		public uint GetValueOrDefault(uint defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x06005431 RID: 21553 RVA: 0x0017FBBD File Offset: 0x0017DDBD
		public uint GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return 0U;
			}
			return this.value;
		}

		// Token: 0x06005432 RID: 21554 RVA: 0x0017FBCF File Offset: 0x0017DDCF
		public void SetValue(uint value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x06005433 RID: 21555 RVA: 0x0017FBDF File Offset: 0x0017DDDF
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x06005434 RID: 21556 RVA: 0x0017FBFA File Offset: 0x0017DDFA
		public bool Equals(OptionalUInt32 other)
		{
			return this.hasValue && other.hasValue && this.value == other.value;
		}

		// Token: 0x06005435 RID: 21557 RVA: 0x0017FC1C File Offset: 0x0017DE1C
		public bool Equals(uint other)
		{
			return this.hasValue && this.value == other;
		}

		// Token: 0x06005436 RID: 21558 RVA: 0x0017FC31 File Offset: 0x0017DE31
		public static bool operator ==(OptionalUInt32 a, OptionalUInt32 b)
		{
			return a.hasValue && b.hasValue && a.value == b.value;
		}

		// Token: 0x06005437 RID: 21559 RVA: 0x0017FC53 File Offset: 0x0017DE53
		public static bool operator !=(OptionalUInt32 a, OptionalUInt32 b)
		{
			return !(a == b);
		}

		// Token: 0x06005438 RID: 21560 RVA: 0x0017FC5F File Offset: 0x0017DE5F
		public static bool operator ==(OptionalUInt32 a, uint b)
		{
			return a.hasValue && a.value == b;
		}

		// Token: 0x06005439 RID: 21561 RVA: 0x0017FC74 File Offset: 0x0017DE74
		public static bool operator !=(OptionalUInt32 a, uint b)
		{
			return !a.hasValue || a.value != b;
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x0017FC8C File Offset: 0x0017DE8C
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x0017FC95 File Offset: 0x0017DE95
		public override int GetHashCode()
		{
			return OptionalUInt32.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x0017FCB2 File Offset: 0x0017DEB2
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x0017FCD2 File Offset: 0x0017DED2
		public static implicit operator OptionalUInt32(uint value)
		{
			return new OptionalUInt32(value);
		}

		// Token: 0x0600543E RID: 21566 RVA: 0x0017FCDA File Offset: 0x0017DEDA
		public static explicit operator uint(OptionalUInt32 optional)
		{
			return optional.Value;
		}

		// Token: 0x0400539B RID: 21403
		[SerializeField]
		private bool hasValue;

		// Token: 0x0400539C RID: 21404
		[SerializeField]
		private uint value;
	}
}
