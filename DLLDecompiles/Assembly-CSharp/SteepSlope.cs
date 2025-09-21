using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000562 RID: 1378
public class SteepSlope : MonoBehaviour
{
	// Token: 0x06003135 RID: 12597 RVA: 0x000DA7F0 File Offset: 0x000D89F0
	private void Awake()
	{
		GameObject gameObject = base.gameObject;
		gameObject.AddComponentIfNotPresent<NonSlider>();
		SteepSlope.STEEP_SLOPES.Add(gameObject);
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x000DA817 File Offset: 0x000D8A17
	private void OnDestroy()
	{
		SteepSlope.STEEP_SLOPES.Remove(base.gameObject);
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x000DA82A File Offset: 0x000D8A2A
	public static bool IsSteepSlope(Collider2D collider2D)
	{
		return SteepSlope.STEEP_SLOPES.Contains(collider2D.gameObject);
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x000DA83C File Offset: 0x000D8A3C
	public static bool IsSteepSlope(GameObject gameObject)
	{
		return SteepSlope.STEEP_SLOPES.Contains(gameObject);
	}

	// Token: 0x04003498 RID: 13464
	private static readonly HashSet<GameObject> STEEP_SLOPES = new HashSet<GameObject>();
}
