using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ArrayForEnumAttribute : PropertyAttribute
{
	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060007C0 RID: 1984 RVA: 0x0002543C File Offset: 0x0002363C
	public bool IsValid
	{
		get
		{
			return this.EnumType != null && this.EnumType.IsEnum && this.EnumLength > 0;
		}
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00025464 File Offset: 0x00023664
	public ArrayForEnumAttribute(Type enumType)
	{
		this.EnumType = enumType;
		if (enumType != null && enumType.IsEnum)
		{
			this.EnumLength = ArrayForEnumAttribute.GetArrayLength(enumType);
			return;
		}
		this.EnumLength = 0;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00025498 File Offset: 0x00023698
	public static void EnsureArraySize<T>(ref T[] array, Type enumType)
	{
		int arrayLength = ArrayForEnumAttribute.GetArrayLength(enumType);
		if (array != null)
		{
			if (array.Length != arrayLength)
			{
				T[] array2 = array;
				array = new T[arrayLength];
				for (int i = 0; i < Mathf.Min(arrayLength, array2.Length); i++)
				{
					array[i] = array2[i];
				}
				return;
			}
		}
		else
		{
			array = new T[arrayLength];
		}
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x000254F0 File Offset: 0x000236F0
	public static int GetArrayLength(Type enumType)
	{
		int num = 0;
		Array values = Enum.GetValues(enumType);
		for (int i = 0; i < values.Length; i++)
		{
			int num2 = (int)values.GetValue(i);
			if (num2 >= 0)
			{
				int num3 = num2 + 1;
				if (num < num3)
				{
					num = num3;
				}
			}
		}
		return num;
	}

	// Token: 0x04000788 RID: 1928
	public readonly Type EnumType;

	// Token: 0x04000789 RID: 1929
	public readonly int EnumLength;
}
