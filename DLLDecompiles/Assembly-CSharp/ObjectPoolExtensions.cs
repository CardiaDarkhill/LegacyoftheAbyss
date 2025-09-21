using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001C RID: 28
public static class ObjectPoolExtensions
{
	// Token: 0x060000F5 RID: 245 RVA: 0x000067D8 File Offset: 0x000049D8
	public static void CreatePool<T>(this T prefab) where T : Component
	{
		ObjectPool.CreatePool<T>(prefab, 0, false);
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x000067E2 File Offset: 0x000049E2
	public static void CreatePool<T>(this T prefab, int initialPoolSize) where T : Component
	{
		ObjectPool.CreatePool<T>(prefab, initialPoolSize, false);
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x000067EC File Offset: 0x000049EC
	public static void CreatePool(this GameObject prefab)
	{
		ObjectPool.CreatePool(prefab, 0, false);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x000067F6 File Offset: 0x000049F6
	public static void CreatePool(this GameObject prefab, int initialPoolSize)
	{
		ObjectPool.CreatePool(prefab, initialPoolSize, false);
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00006800 File Offset: 0x00004A00
	public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn<T>(prefab, parent, position, rotation);
	}

	// Token: 0x060000FA RID: 250 RVA: 0x0000680B File Offset: 0x00004A0B
	public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn<T>(prefab, null, position, rotation);
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00006816 File Offset: 0x00004A16
	public static T Spawn<T>(this T prefab, Transform parent, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn<T>(prefab, parent, position, Quaternion.identity);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00006825 File Offset: 0x00004A25
	public static T Spawn<T>(this T prefab, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn<T>(prefab, null, position, Quaternion.identity);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00006834 File Offset: 0x00004A34
	public static T Spawn<T>(this T prefab, Transform parent) where T : Component
	{
		return ObjectPool.Spawn<T>(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00006847 File Offset: 0x00004A47
	public static T Spawn<T>(this T prefab) where T : Component
	{
		return ObjectPool.Spawn<T>(prefab, null, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x060000FF RID: 255 RVA: 0x0000685A File Offset: 0x00004A5A
	public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		return ObjectPool.Spawn(prefab, parent, position, rotation);
	}

	// Token: 0x06000100 RID: 256 RVA: 0x00006865 File Offset: 0x00004A65
	public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return ObjectPool.Spawn(prefab, null, position, rotation);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00006870 File Offset: 0x00004A70
	public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position)
	{
		return ObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
	}

	// Token: 0x06000102 RID: 258 RVA: 0x0000687F File Offset: 0x00004A7F
	public static GameObject Spawn(this GameObject prefab, Vector3 position)
	{
		return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
	}

	// Token: 0x06000103 RID: 259 RVA: 0x0000688E File Offset: 0x00004A8E
	public static GameObject Spawn(this GameObject prefab, Transform parent)
	{
		return ObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06000104 RID: 260 RVA: 0x000068A1 File Offset: 0x00004AA1
	public static GameObject Spawn(this GameObject prefab)
	{
		return ObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x06000105 RID: 261 RVA: 0x000068B4 File Offset: 0x00004AB4
	private static GameObject[] SpawnAll(this GameObject[] array, Action<GameObject> onSpawn)
	{
		GameObject[] array2 = new GameObject[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			if (!(array[i] == null))
			{
				array2[i] = array[i].Spawn();
				if (onSpawn != null)
				{
					onSpawn(array2[i]);
				}
			}
		}
		return array2;
	}

	// Token: 0x06000106 RID: 262 RVA: 0x000068FB File Offset: 0x00004AFB
	public static GameObject[] SpawnAll(this GameObject[] array)
	{
		return array.SpawnAll(null);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00006904 File Offset: 0x00004B04
	public static GameObject[] SpawnAll(this GameObject[] array, Vector3 position)
	{
		return array.SpawnAll(delegate(GameObject obj)
		{
			obj.transform.position = position;
		});
	}

	// Token: 0x06000108 RID: 264 RVA: 0x00006930 File Offset: 0x00004B30
	public static GameObject[] SpawnAll(this GameObject[] array, Vector2 position)
	{
		return array.SpawnAll(delegate(GameObject obj)
		{
			obj.transform.SetPosition2D(position);
		});
	}

	// Token: 0x06000109 RID: 265 RVA: 0x0000695C File Offset: 0x00004B5C
	public static void Recycle<T>(this T obj) where T : Component
	{
		ObjectPool.Recycle<T>(obj);
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00006964 File Offset: 0x00004B64
	public static void Recycle(this GameObject obj)
	{
		ObjectPool.Recycle(obj);
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000696C File Offset: 0x00004B6C
	public static void RecycleAll<T>(this T prefab) where T : Component
	{
		ObjectPool.RecycleAll<T>(prefab);
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00006974 File Offset: 0x00004B74
	public static void RecycleAll(this GameObject prefab)
	{
		ObjectPool.RecycleAll(prefab);
	}

	// Token: 0x0600010D RID: 269 RVA: 0x0000697C File Offset: 0x00004B7C
	public static int CountPooled<T>(this T prefab) where T : Component
	{
		return ObjectPool.CountPooled<T>(prefab);
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00006984 File Offset: 0x00004B84
	public static int CountPooled(this GameObject prefab)
	{
		return ObjectPool.CountPooled(prefab);
	}

	// Token: 0x0600010F RID: 271 RVA: 0x0000698C File Offset: 0x00004B8C
	public static int CountSpawned<T>(this T prefab) where T : Component
	{
		return ObjectPool.CountSpawned<T>(prefab);
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00006994 File Offset: 0x00004B94
	public static int CountSpawned(this GameObject prefab)
	{
		return ObjectPool.CountSpawned(prefab);
	}

	// Token: 0x06000111 RID: 273 RVA: 0x0000699C File Offset: 0x00004B9C
	public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list, bool appendList)
	{
		return ObjectPool.GetSpawned(prefab, list, appendList);
	}

	// Token: 0x06000112 RID: 274 RVA: 0x000069A6 File Offset: 0x00004BA6
	public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list)
	{
		return ObjectPool.GetSpawned(prefab, list, false);
	}

	// Token: 0x06000113 RID: 275 RVA: 0x000069B0 File Offset: 0x00004BB0
	public static List<GameObject> GetSpawned(this GameObject prefab)
	{
		return ObjectPool.GetSpawned(prefab, null, false);
	}

	// Token: 0x06000114 RID: 276 RVA: 0x000069BA File Offset: 0x00004BBA
	public static List<T> GetSpawned<T>(this T prefab, List<T> list, bool appendList) where T : Component
	{
		return ObjectPool.GetSpawned<T>(prefab, list, appendList);
	}

	// Token: 0x06000115 RID: 277 RVA: 0x000069C4 File Offset: 0x00004BC4
	public static List<T> GetSpawned<T>(this T prefab, List<T> list) where T : Component
	{
		return ObjectPool.GetSpawned<T>(prefab, list, false);
	}

	// Token: 0x06000116 RID: 278 RVA: 0x000069CE File Offset: 0x00004BCE
	public static List<T> GetSpawned<T>(this T prefab) where T : Component
	{
		return ObjectPool.GetSpawned<T>(prefab, null, false);
	}

	// Token: 0x06000117 RID: 279 RVA: 0x000069D8 File Offset: 0x00004BD8
	public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list, bool appendList)
	{
		return ObjectPool.GetPooled(prefab, list, appendList);
	}

	// Token: 0x06000118 RID: 280 RVA: 0x000069E2 File Offset: 0x00004BE2
	public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list)
	{
		return ObjectPool.GetPooled(prefab, list, false);
	}

	// Token: 0x06000119 RID: 281 RVA: 0x000069EC File Offset: 0x00004BEC
	public static List<GameObject> GetPooled(this GameObject prefab)
	{
		return ObjectPool.GetPooled(prefab, null, false);
	}

	// Token: 0x0600011A RID: 282 RVA: 0x000069F6 File Offset: 0x00004BF6
	public static List<T> GetPooled<T>(this T prefab, List<T> list, bool appendList) where T : Component
	{
		return ObjectPool.GetPooled<T>(prefab, list, appendList);
	}

	// Token: 0x0600011B RID: 283 RVA: 0x00006A00 File Offset: 0x00004C00
	public static List<T> GetPooled<T>(this T prefab, List<T> list) where T : Component
	{
		return ObjectPool.GetPooled<T>(prefab, list, false);
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00006A0A File Offset: 0x00004C0A
	public static List<T> GetPooled<T>(this T prefab) where T : Component
	{
		return ObjectPool.GetPooled<T>(prefab, null, false);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00006A14 File Offset: 0x00004C14
	public static void DestroyPooled(this GameObject prefab)
	{
		ObjectPool.DestroyPooled(prefab);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00006A1C File Offset: 0x00004C1C
	public static void DestroyPooled<T>(this T prefab) where T : Component
	{
		ObjectPool.DestroyPooled(prefab.gameObject);
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00006A2E File Offset: 0x00004C2E
	public static void DestroyAll(this GameObject prefab)
	{
		ObjectPool.DestroyAll(prefab);
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00006A36 File Offset: 0x00004C36
	public static void DestroyAll<T>(this T prefab) where T : Component
	{
		ObjectPool.DestroyAll(prefab.gameObject);
	}
}
