using System;
using UnityEngine;

// Token: 0x0200077E RID: 1918
public static class RandomTable
{
	// Token: 0x0600441C RID: 17436 RVA: 0x0012AF1C File Offset: 0x0012911C
	public static bool TrySelectValue<Ty>(this Ty[] items, out Ty value) where Ty : WeightedItem
	{
		if (items.Length == 0)
		{
			value = default(Ty);
			return false;
		}
		float num = 0f;
		foreach (Ty ty in items)
		{
			num += ty.Weight;
		}
		float num2 = Random.Range(0f, num);
		float num3 = 0f;
		for (int j = 0; j < items.Length - 1; j++)
		{
			Ty ty2 = items[j];
			num3 += ty2.Weight;
			if (num2 < num3)
			{
				value = ty2;
				return true;
			}
		}
		value = items[items.Length - 1];
		return true;
	}

	// Token: 0x0600441D RID: 17437 RVA: 0x0012AFC4 File Offset: 0x001291C4
	public static Ty SelectValue<Ty>(this Ty[] items) where Ty : WeightedItem
	{
		Ty result;
		if (!items.TrySelectValue(out result))
		{
			return default(Ty);
		}
		return result;
	}
}
