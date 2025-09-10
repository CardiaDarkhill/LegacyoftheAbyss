using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200093A RID: 2362
	[Serializable]
	public struct OptionalInputDeviceDriverType
	{
		// Token: 0x060053C1 RID: 21441 RVA: 0x0017F345 File Offset: 0x0017D545
		public OptionalInputDeviceDriverType(InputDeviceDriverType value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x17000B87 RID: 2951
		// (get) Token: 0x060053C2 RID: 21442 RVA: 0x0017F355 File Offset: 0x0017D555
		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		// Token: 0x17000B88 RID: 2952
		// (get) Token: 0x060053C3 RID: 21443 RVA: 0x0017F35D File Offset: 0x0017D55D
		public bool HasNoValue
		{
			get
			{
				return !this.hasValue;
			}
		}

		// Token: 0x17000B89 RID: 2953
		// (get) Token: 0x060053C4 RID: 21444 RVA: 0x0017F368 File Offset: 0x0017D568
		// (set) Token: 0x060053C5 RID: 21445 RVA: 0x0017F383 File Offset: 0x0017D583
		public InputDeviceDriverType Value
		{
			get
			{
				if (!this.hasValue)
				{
					throw new OptionalTypeHasNoValueException("Trying to get a value from an OptionalInputDeviceDriverType that has no value.");
				}
				return this.value;
			}
			set
			{
				this.value = value;
				this.hasValue = true;
			}
		}

		// Token: 0x060053C6 RID: 21446 RVA: 0x0017F393 File Offset: 0x0017D593
		public void Clear()
		{
			this.value = InputDeviceDriverType.Unknown;
			this.hasValue = false;
		}

		// Token: 0x060053C7 RID: 21447 RVA: 0x0017F3A3 File Offset: 0x0017D5A3
		public InputDeviceDriverType GetValueOrDefault(InputDeviceDriverType defaultValue)
		{
			if (!this.hasValue)
			{
				return defaultValue;
			}
			return this.value;
		}

		// Token: 0x060053C8 RID: 21448 RVA: 0x0017F3B5 File Offset: 0x0017D5B5
		public InputDeviceDriverType GetValueOrZero()
		{
			if (!this.hasValue)
			{
				return InputDeviceDriverType.Unknown;
			}
			return this.value;
		}

		// Token: 0x060053C9 RID: 21449 RVA: 0x0017F3C7 File Offset: 0x0017D5C7
		public void SetValue(InputDeviceDriverType value)
		{
			this.value = value;
			this.hasValue = true;
		}

		// Token: 0x060053CA RID: 21450 RVA: 0x0017F3D7 File Offset: 0x0017D5D7
		public override bool Equals(object other)
		{
			return (other == null && !this.hasValue) || this.value.Equals(other);
		}

		// Token: 0x060053CB RID: 21451 RVA: 0x0017F3F8 File Offset: 0x0017D5F8
		public bool Equals(OptionalInputDeviceDriverType other)
		{
			return this.hasValue && other.hasValue && this.value == other.value;
		}

		// Token: 0x060053CC RID: 21452 RVA: 0x0017F41A File Offset: 0x0017D61A
		public bool Equals(InputDeviceDriverType other)
		{
			return this.hasValue && this.value == other;
		}

		// Token: 0x060053CD RID: 21453 RVA: 0x0017F42F File Offset: 0x0017D62F
		public static bool operator ==(OptionalInputDeviceDriverType a, OptionalInputDeviceDriverType b)
		{
			return a.hasValue && b.hasValue && a.value == b.value;
		}

		// Token: 0x060053CE RID: 21454 RVA: 0x0017F451 File Offset: 0x0017D651
		public static bool operator !=(OptionalInputDeviceDriverType a, OptionalInputDeviceDriverType b)
		{
			return !(a == b);
		}

		// Token: 0x060053CF RID: 21455 RVA: 0x0017F45D File Offset: 0x0017D65D
		public static bool operator ==(OptionalInputDeviceDriverType a, InputDeviceDriverType b)
		{
			return a.hasValue && a.value == b;
		}

		// Token: 0x060053D0 RID: 21456 RVA: 0x0017F472 File Offset: 0x0017D672
		public static bool operator !=(OptionalInputDeviceDriverType a, InputDeviceDriverType b)
		{
			return !a.hasValue || a.value != b;
		}

		// Token: 0x060053D1 RID: 21457 RVA: 0x0017F48A File Offset: 0x0017D68A
		private static int CombineHashCodes(int h1, int h2)
		{
			return (h1 << 5) + h1 ^ h2;
		}

		// Token: 0x060053D2 RID: 21458 RVA: 0x0017F493 File Offset: 0x0017D693
		public override int GetHashCode()
		{
			return OptionalInputDeviceDriverType.CombineHashCodes(this.hasValue.GetHashCode(), this.value.GetHashCode());
		}

		// Token: 0x060053D3 RID: 21459 RVA: 0x0017F4B6 File Offset: 0x0017D6B6
		public override string ToString()
		{
			if (!this.hasValue)
			{
				return "";
			}
			return this.value.ToString();
		}

		// Token: 0x060053D4 RID: 21460 RVA: 0x0017F4D7 File Offset: 0x0017D6D7
		public static implicit operator OptionalInputDeviceDriverType(InputDeviceDriverType value)
		{
			return new OptionalInputDeviceDriverType(value);
		}

		// Token: 0x060053D5 RID: 21461 RVA: 0x0017F4DF File Offset: 0x0017D6DF
		public static explicit operator InputDeviceDriverType(OptionalInputDeviceDriverType optional)
		{
			return optional.Value;
		}

		// Token: 0x04005391 RID: 21393
		[SerializeField]
		private bool hasValue;

		// Token: 0x04005392 RID: 21394
		[SerializeField]
		private InputDeviceDriverType value;
	}
}
