using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x020003DA RID: 986
public class CurrencyObjectLimitRegion : MonoBehaviour, ICurrencyLimitRegion
{
	// Token: 0x17000380 RID: 896
	// (get) Token: 0x060021C3 RID: 8643 RVA: 0x0009BEC2 File Offset: 0x0009A0C2
	public CurrencyType CurrencyType
	{
		get
		{
			return this.currencyType;
		}
	}

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x060021C4 RID: 8644 RVA: 0x0009BECA File Offset: 0x0009A0CA
	public int Limit
	{
		get
		{
			return this.limit;
		}
	}

	// Token: 0x060021C5 RID: 8645 RVA: 0x0009BED4 File Offset: 0x0009A0D4
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Bounds bounds = new Bounds
		{
			min = this.min,
			max = this.max
		};
		Gizmos.DrawWireCube(bounds.center, bounds.size);
	}

	// Token: 0x060021C6 RID: 8646 RVA: 0x0009BF32 File Offset: 0x0009A132
	private void OnEnable()
	{
		CurrencyObjectLimitRegion._activeRegions.Add(this);
	}

	// Token: 0x060021C7 RID: 8647 RVA: 0x0009BF40 File Offset: 0x0009A140
	private void OnDisable()
	{
		CurrencyObjectLimitRegion._activeRegions.Remove(this);
	}

	// Token: 0x060021C8 RID: 8648 RVA: 0x0009BF4E File Offset: 0x0009A14E
	public static void AddRegion(ICurrencyLimitRegion limitRegion)
	{
		CurrencyObjectLimitRegion._activeRegions.Add(limitRegion);
	}

	// Token: 0x060021C9 RID: 8649 RVA: 0x0009BF5C File Offset: 0x0009A15C
	public static void RemoveRegion(ICurrencyLimitRegion limitRegion)
	{
		CurrencyObjectLimitRegion._activeRegions.Remove(limitRegion);
	}

	// Token: 0x060021CA RID: 8650 RVA: 0x0009BF6C File Offset: 0x0009A16C
	public static int GetLimit(Vector2 pos, CurrencyType limitedType)
	{
		int maxCurrencyObjects = Gameplay.GetMaxCurrencyObjects(limitedType);
		foreach (ICurrencyLimitRegion currencyLimitRegion in CurrencyObjectLimitRegion._activeRegions)
		{
			if (currencyLimitRegion.CurrencyType == limitedType && currencyLimitRegion.Limit <= maxCurrencyObjects && currencyLimitRegion.IsInsideLimitRegion(pos))
			{
				maxCurrencyObjects = currencyLimitRegion.Limit;
			}
		}
		return maxCurrencyObjects;
	}

	// Token: 0x060021CB RID: 8651 RVA: 0x0009BFE4 File Offset: 0x0009A1E4
	public bool IsInsideLimitRegion(Vector2 point)
	{
		Transform transform = base.transform;
		Vector3 vector = transform.TransformPoint(this.min);
		Vector3 vector2 = transform.TransformPoint(this.max);
		return point.x >= vector.x && point.y >= vector.y && point.x <= vector2.x && point.y <= vector2.y;
	}

	// Token: 0x0400208C RID: 8332
	[SerializeField]
	private Vector2 min;

	// Token: 0x0400208D RID: 8333
	[SerializeField]
	private Vector2 max;

	// Token: 0x0400208E RID: 8334
	[SerializeField]
	private CurrencyType currencyType;

	// Token: 0x0400208F RID: 8335
	[SerializeField]
	private int limit;

	// Token: 0x04002090 RID: 8336
	private static readonly HashSet<ICurrencyLimitRegion> _activeRegions = new HashSet<ICurrencyLimitRegion>();
}
