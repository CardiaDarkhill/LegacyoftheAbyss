using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200069D RID: 1693
public static class InventoryItemNavigationHelper
{
	// Token: 0x06003C68 RID: 15464 RVA: 0x00109A64 File Offset: 0x00107C64
	public static T GetClosestOnAxis<T>(InventoryItemManager.SelectionDirection direction, InventoryItemSelectable currentSelected, IEnumerable<T> items) where T : InventoryItemSelectable
	{
		T closest;
		if (direction == InventoryItemManager.SelectionDirection.Left || direction == InventoryItemManager.SelectionDirection.Right)
		{
			closest = InventoryItemNavigationHelper.GetClosest<T>(new Func<Vector2, float>(InventoryItemNavigationHelper.GetXAxis), new Func<Vector2, float>(InventoryItemNavigationHelper.GetYAxis), direction == InventoryItemManager.SelectionDirection.Left, currentSelected, items);
		}
		else
		{
			closest = InventoryItemNavigationHelper.GetClosest<T>(new Func<Vector2, float>(InventoryItemNavigationHelper.GetYAxis), new Func<Vector2, float>(InventoryItemNavigationHelper.GetXAxis), direction == InventoryItemManager.SelectionDirection.Down, currentSelected, items);
		}
		if (closest)
		{
			return (T)((object)closest.Get(new InventoryItemManager.SelectionDirection?(direction)));
		}
		return default(T);
	}

	// Token: 0x06003C69 RID: 15465 RVA: 0x00109AF0 File Offset: 0x00107CF0
	private static T GetClosest<T>(Func<Vector2, float> getMainAxis, Func<Vector2, float> getSecondaryAxis, bool selectPositive, InventoryItemSelectable currentSelected, IEnumerable<T> items) where T : InventoryItemSelectable
	{
		Vector2 a = Vector2.zero;
		float? num = null;
		float y = getSecondaryAxis(currentSelected.transform.TransformPoint(currentSelected.NavigationOffset));
		foreach (T t in items)
		{
			Vector3 v = t.transform.TransformPoint(t.NavigationOffset);
			float num2 = getMainAxis(v);
			if (num != null)
			{
				float? num4;
				if (selectPositive)
				{
					float num3 = num2;
					num4 = num;
					if (num3 > num4.GetValueOrDefault() & num4 != null)
					{
						goto IL_B7;
					}
				}
				if (selectPositive)
				{
					continue;
				}
				float num5 = num2;
				num4 = num;
				if (!(num5 < num4.GetValueOrDefault() & num4 != null))
				{
					continue;
				}
			}
			IL_B7:
			num = new float?(num2);
			a = v;
		}
		if (num != null)
		{
			Vector2 arg = new Vector2(num.Value, y);
			Vector2 a2 = new Vector2(getMainAxis(arg), getSecondaryAxis(arg));
			float num6 = float.MaxValue;
			float num7 = float.MaxValue;
			T result = default(T);
			foreach (T t2 in items)
			{
				Vector3 v2 = t2.transform.TransformPoint(t2.NavigationOffset);
				float num8 = Vector2.Distance(a2, v2);
				if (num8 >= num6)
				{
					if (Math.Abs(num8 - num6) >= 0.2f)
					{
						continue;
					}
					float num9 = Vector2.Distance(a, v2);
					if (num9 >= num7)
					{
						continue;
					}
					num7 = num9;
				}
				num6 = num8;
				result = t2;
			}
			return result;
		}
		return default(T);
	}

	// Token: 0x06003C6A RID: 15466 RVA: 0x00109CE0 File Offset: 0x00107EE0
	private static float GetXAxis(Vector2 vector)
	{
		return vector.x;
	}

	// Token: 0x06003C6B RID: 15467 RVA: 0x00109CE8 File Offset: 0x00107EE8
	private static float GetYAxis(Vector2 vector)
	{
		return vector.y;
	}
}
