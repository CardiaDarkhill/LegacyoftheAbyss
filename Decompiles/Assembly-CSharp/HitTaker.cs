using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000031 RID: 49
public static class HitTaker
{
	// Token: 0x06000171 RID: 369 RVA: 0x00008373 File Offset: 0x00006573
	public static void Hit(GameObject targetGameObject, HitInstance damageInstance, int recursionDepth = 3)
	{
		HitTaker.Hit(targetGameObject, damageInstance, recursionDepth, null);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x0000837E File Offset: 0x0000657E
	public static void Hit(GameObject targetGameObject, HitInstance damageInstance, HashSet<IHitResponder> blackList)
	{
		HitTaker.Hit(targetGameObject, damageInstance, 3, blackList);
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000838C File Offset: 0x0000658C
	public static void Hit(GameObject targetGameObject, HitInstance damageInstance, int recursionDepth, HashSet<IHitResponder> blackList)
	{
		foreach (IHitResponder hitResponder in HitTaker.GetHitResponders(targetGameObject, recursionDepth, blackList))
		{
			hitResponder.Hit(damageInstance);
		}
	}

	// Token: 0x06000174 RID: 372 RVA: 0x000083E0 File Offset: 0x000065E0
	public static List<IHitResponder> GetHitResponders(GameObject targetGameObject, HashSet<IHitResponder> blackList)
	{
		return HitTaker.GetHitResponders(targetGameObject, 3, blackList);
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000083EA File Offset: 0x000065EA
	public static List<IHitResponder> GetHitResponders(GameObject targetGameObject, int recursionDepth, HashSet<IHitResponder> blackList)
	{
		List<IHitResponder> list = new List<IHitResponder>();
		HitTaker.GetHitResponders(list, targetGameObject, recursionDepth, blackList);
		return list;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x000083FA File Offset: 0x000065FA
	public static void GetHitResponders(List<IHitResponder> storeList, GameObject targetGameObject, HashSet<IHitResponder> blackList)
	{
		HitTaker.GetHitResponders(storeList, targetGameObject, 3, blackList);
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00008408 File Offset: 0x00006608
	public static void GetHitResponders(List<IHitResponder> storeList, GameObject targetGameObject, int recursionDepth, HashSet<IHitResponder> blackList)
	{
		if (targetGameObject == null)
		{
			return;
		}
		Transform transform = targetGameObject.transform;
		bool flag = blackList != null;
		try
		{
			for (int i = 0; i < recursionDepth; i++)
			{
				HitTaker._tempHitResponders.Clear();
				transform.GetComponents<IHitResponder>(HitTaker._tempHitResponders);
				bool flag2 = false;
				foreach (IHitResponder hitResponder in HitTaker._tempHitResponders)
				{
					if (!flag)
					{
						storeList.Add(hitResponder);
					}
					else if (!blackList.Contains(hitResponder))
					{
						storeList.Add(hitResponder);
					}
					if (!hitResponder.HitRecurseUpwards)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					break;
				}
				if (transform.GetComponent<Rigidbody2D>())
				{
					break;
				}
				transform = transform.parent;
				if (transform == null)
				{
					break;
				}
			}
		}
		finally
		{
			HitTaker._tempHitResponders.Clear();
		}
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00008500 File Offset: 0x00006700
	public static bool TryGetHealthManager(GameObject targetGameObject, out HealthManager healthManager)
	{
		return HitTaker.TryGetHealthManager(targetGameObject, 3, out healthManager);
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000850C File Offset: 0x0000670C
	public static bool TryGetHealthManager(GameObject targetGameObject, int recursionDepth, out HealthManager healthManager)
	{
		if (targetGameObject == null)
		{
			healthManager = null;
			return false;
		}
		Transform transform = targetGameObject.transform;
		try
		{
			for (int i = 0; i < recursionDepth; i++)
			{
				HitTaker._tempHitResponders.Clear();
				transform.GetComponents<IHitResponder>(HitTaker._tempHitResponders);
				bool flag = false;
				foreach (IHitResponder hitResponder in HitTaker._tempHitResponders)
				{
					HealthManager healthManager2 = hitResponder as HealthManager;
					if (healthManager2 != null)
					{
						healthManager = healthManager2;
						return true;
					}
					if (!hitResponder.HitRecurseUpwards)
					{
						flag = true;
					}
				}
				if (flag)
				{
					break;
				}
				if (transform.GetComponent<Rigidbody2D>())
				{
					break;
				}
				transform = transform.parent;
				if (transform == null)
				{
					break;
				}
			}
		}
		finally
		{
			HitTaker._tempHitResponders.Clear();
		}
		healthManager = null;
		return false;
	}

	// Token: 0x04000109 RID: 265
	private const int DefaultRecursionDepth = 3;

	// Token: 0x0400010A RID: 266
	private static readonly List<IHitResponder> _tempHitResponders = new List<IHitResponder>();
}
