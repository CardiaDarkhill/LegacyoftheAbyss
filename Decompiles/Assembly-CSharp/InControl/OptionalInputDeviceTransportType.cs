using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200093B RID: 2363
	[Serializable]
	public struct OptionalInputDeviceTransportType
	{
		// Token: 0x060053D6 RID: 21462 RVA: 0x0017F4E8 File Offset: 0x0017D6E8
		public OptionalInputDeviceTransportType(InputDeviceTransportType value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B8A RID: 2954
		// (get) Token: 0x060053D7 RID: 21463 RVA: 0x0017F4F8 File Offset: 0x0017D6F8
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B8B RID: 2955
		// (get) Token: 0x060053D8 RID: 21464 RVA: 0x0017F500 File Offset: 0x0017D700
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B8C RID: 2956
		// (get) Token: 0x060053D9 RID: 21465 RVA: 0x0017F50B File Offset: 0x0017D70B
		// (set) Token: 0x060053DA RID: 21466 RVA: 0x0017F526 File Offset: 0x0017D726
		public InputDeviceTransportType Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInputDeviceTransportType that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x060053DB RID: 21467 RVA: 0x0017F536 File Offset: 0x0017D736
		public void Clear()
		{
			this.value = InputDeviceTransportType.Unknown;
			this.hasValue = false;
		}

		// Token: 0x060053DC RID: 21468 RVA: 0x0017F546 File Offset: 0x0017D746
		public InputDeviceTransportType GetValueOrDefault(InputDeviceTransportType defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x060053DD RID: 21469 RVA: 0x0017F558 File Offset: 0x0017D758
		public InputDeviceTransportType GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return InputDeviceTransportType.Unknown;
			}
			return this.value;
		}

		// Token: 0x060053DE RID: 21470 RVA: 0x0017F56A File Offset: 0x0017D76A
		public void SetValue(InputDeviceTransportType value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x060053DF RID: 21471 RVA: 0x0017F57A File Offset: 0x0017D77A
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x060053E0 RID: 21472 RVA: 0x0017F59B File Offset: 0x0017D79B
		public bool Equals(OptionalInputDeviceTransportType other)
		{
			return this.hasValue && other.hasValue && this.value == other.value;
		}

		// Token: 0x060053E1 RID: 21473 RVA: 0x0017F5BD File Offset: 0x0017D7BD
		public bool Equals(InputDeviceTransportType other)
		{
			return this.hasValue && this.value == other;
		}

		// Token: 0x060053E2 RID: 21474 RVA: 0x0017F5D2 File Offset: 0x0017D7D2
		public static bool operator ==(OptionalInputDeviceTransportType a, OptionalInputDeviceTransportType b)
		{
			return a.hasValue && b.hasValue && a.value == b.value;
		}

		// Token: 0x060053E3 RID: 21475 RVA: 0x0017F5F4 File Offset: 0x0017D7F4
		public static bool operator !=(OptionalInputDeviceTransportType a, OptionalInputDeviceTransportType b)
		{
			return !(a == b);
		}

		// Token: 0x060053E4 RID: 21476 RVA: 0x0017F600 File Offset: 0x0017D800
		public static bool operator ==(OptionalInputDeviceTransportType a, InputDeviceTransportType b)
		{
			return a.hasValue && a.value == b;
		}

		// Token: 0x060053E5 RID: 21477 RVA: 0x0017F615 File Offset: 0x0017D815
		public static bool operator !=(OptionalInputDeviceTransportType a, InputDeviceTransportType b)
		{
			return !a.hasValue || a.value != b;
		}

		// Token: 0x060053E6 RID: 21478 RVA: 0x0017F62D File Offset: 0x0017D82D
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x060053E7 RID: 21479 RVA: 0x0017F636 File Offset: 0x0017D836
		public override int GetHashCode()
		{
			return OptionalInputDeviceTransportType.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x060053E8 RID: 21480 RVA: 0x0017F659 File Offset: 0x0017D859
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString();
		}

		// Token: 0x060053E9 RID: 21481 RVA: 0x0017F67A File Offset: 0x0017D87A
		public static implicit operator OptionalInputDeviceTransportType(InputDeviceTransportType value)
		{
			return new OptionalInputDeviceTransportType(value);
		}

		// Token: 0x060053EA RID: 21482 RVA: 0x0017F682 File Offset: 0x0017D882
		public static explicit operator InputDeviceTransportType(OptionalInputDeviceTransportType optional)
		{
			return optional.Value;
		}

		// Token: 0x04005393 RID: 21395
		[SerializeField]
		private bool hasValue;

		// Token: 0x04005394 RID: 21396
		[SerializeField]
		private InputDeviceTransportType value;
	}
}
