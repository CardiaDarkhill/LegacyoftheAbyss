using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x02000810 RID: 2064
	internal static class SetPropertyUtility
	{
		// Token: 0x060048E7 RID: 18663 RVA: 0x00154444 File Offset: 0x00152644
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060048E8 RID: 18664 RVA: 0x00154493 File Offset: 0x00152693
		public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060048E9 RID: 18665 RVA: 0x001544AE File Offset: 0x001526AE
		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		// Token: 0x060048EA RID: 18666 RVA: 0x001544D0 File Offset: 0x001526D0
		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}
	}
}
