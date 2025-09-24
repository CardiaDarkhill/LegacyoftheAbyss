using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001B RID: 27
public sealed class ObjectPool : MonoBehaviour
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000B8 RID: 184 RVA: 0x000053ED File Offset: 0x000035ED
	// (set) Token: 0x060000B9 RID: 185 RVA: 0x000053F4 File Offset: 0x000035F4
	public static bool IsCreatingPool { get; private set; }

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000BA RID: 186 RVA: 0x000053FC File Offset: 0x000035FC
	public static ObjectPool instance
	{
		get
		{
			if (!ObjectPool.hasInstance)
			{
				ObjectPool._instance = Object.FindObjectOfType<ObjectPool>();
				ObjectPool.hasInstance = ObjectPool._instance;
				if (ObjectPool.hasInstance && ObjectPool._instance.isPrimary)
				{
					Object.DontDestroyOnLoad(ObjectPool._instance.gameObject);
				}
			}
			return ObjectPool._instance;
		}
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00005450 File Offset: 0x00003650
	private void Awake()
	{
		if (ObjectPool._instance == null)
		{
			ObjectPool._instance = this;
			if (this.isPrimary)
			{
				Object.DontDestroyOnLoad(this);
				return;
			}
		}
		else if (ObjectPool._instance != this)
		{
			if (this.isPrimary && !ObjectPool._instance.isPrimary)
			{
				Object.Destroy(ObjectPool._instance.gameObject);
				ObjectPool._instance = this;
				Object.DontDestroyOnLoad(this);
				return;
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x000054C7 File Offset: 0x000036C7
	private void OnDestroy()
	{
		if (ObjectPool._instance == this)
		{
			ObjectPool.hasInstance = false;
			ObjectPool._instance = null;
		}
	}

	// Token: 0x060000BD RID: 189 RVA: 0x000054E4 File Offset: 0x000036E4
	private void LateUpdate()
	{
		if (ObjectPool.reparentList.Count > 0)
		{
			for (int i = ObjectPool.reparentList.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = ObjectPool.reparentList[i];
				if (gameObject == null)
				{
					ObjectPool.reparentList.RemoveAt(i);
				}
				else
				{
					gameObject.transform.parent = base.transform;
					if (!(gameObject.transform.parent != base.transform))
					{
						ObjectPool.reparentList.RemoveAt(i);
					}
				}
			}
		}
	}

	// Token: 0x060000BE RID: 190 RVA: 0x0000556C File Offset: 0x0000376C
	public static void CreateStartupPools()
	{
		if (ObjectPool.instance.startupPoolsCreated)
		{
			return;
		}
		ObjectPool.instance.startupPoolsCreated = true;
		ObjectPool.StartupPool[] array = ObjectPool.instance.startupPools;
		if (array == null || array.Length <= 0)
		{
			return;
		}
		foreach (ObjectPool.StartupPool startupPool in array)
		{
			ObjectPool.CreatePool(startupPool.prefab, startupPool.size, true);
		}
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000055CC File Offset: 0x000037CC
	public static void CreatePool<T>(T prefab, int initialPoolSize, bool runInitialisation = false) where T : Component
	{
		ObjectPool.CreatePool<T>(prefab, initialPoolSize, false, Vector3.zero, Quaternion.identity, runInitialisation);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000055E1 File Offset: 0x000037E1
	public static void CreatePool<T>(T prefab, int initialPoolSize, bool setPosition, Vector3 position, Quaternion rotation, bool runInitialisation = false) where T : Component
	{
		ObjectPool.CreatePool(prefab.gameObject, initialPoolSize, setPosition, position, rotation, runInitialisation);
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x000055FA File Offset: 0x000037FA
	public static void CreatePool(GameObject prefab, int initialPoolSize, bool runInitialisation = false)
	{
		ObjectPool.CreatePool(prefab, initialPoolSize, false, Vector3.zero, Quaternion.identity, runInitialisation);
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00005610 File Offset: 0x00003810
	public static void CreatePool(GameObject prefab, int initialPoolSize, bool setPosition, Vector3 position, Quaternion rotation, bool runInitialisation = false)
	{
		ObjectPoolAuditor.RecordPoolCreated(prefab, initialPoolSize);
		if (prefab)
		{
			List<GameObject> list;
			if (!ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
			{
				list = ((ObjectPool._spareGameObjectPools.Count > 0) ? ObjectPool._spareGameObjectPools.Dequeue() : new List<GameObject>());
				ObjectPool.instance.pooledObjects.Add(prefab, list);
			}
			ObjectPool.CreatePooledObjects(prefab, initialPoolSize, list, setPosition, position, rotation);
			if (list == null)
			{
				return;
			}
			using (List<GameObject>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameObject gameObject = enumerator.Current;
					if (gameObject)
					{
						tk2dSprite[] componentsInChildren = gameObject.GetComponentsInChildren<tk2dSprite>(true);
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].ForceBuild();
						}
						if (runInitialisation)
						{
							IInitialisable.DoFullInitForcePool(gameObject);
						}
					}
				}
				return;
			}
		}
		prefab;
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000056F8 File Offset: 0x000038F8
	private static void CreatePooledObjects(GameObject prefab, int initialPoolSize, ICollection<GameObject> pooledList, bool setPosition, Vector3 position, Quaternion rotation)
	{
		if (initialPoolSize <= 0)
		{
			return;
		}
		bool flag = prefab.GetComponent<ActiveRecycler>();
		Transform transform = ObjectPool.instance.transform;
		if (!setPosition)
		{
			ObjectPool.IsCreatingPool = true;
		}
		try
		{
			while (pooledList.Count < initialPoolSize)
			{
				GameObject gameObject = setPosition ? Object.Instantiate<GameObject>(prefab, position, rotation, transform) : Object.Instantiate<GameObject>(prefab, transform, true);
				if (flag)
				{
					gameObject.SetActive(true);
					ObjectPool.SetActiveRecycled(gameObject);
				}
				else if (!setPosition)
				{
					gameObject.SetActive(false);
				}
				pooledList.Add(gameObject);
			}
		}
		finally
		{
			ObjectPool.IsCreatingPool = false;
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0000578C File Offset: 0x0000398C
	public void RevertToStartState()
	{
		this.noInstancePrefabs.Clear();
		Dictionary<GameObject, ObjectPool.PoolCounter> dictionary = new Dictionary<GameObject, ObjectPool.PoolCounter>();
		for (int i = 0; i < this.startupPools.Length; i++)
		{
			ObjectPool.StartupPool startupPool = this.startupPools[i];
			if (!(startupPool.prefab == null))
			{
				ObjectPool.PoolCounter poolCounter;
				if (!dictionary.TryGetValue(startupPool.prefab, out poolCounter))
				{
					poolCounter = (dictionary[startupPool.prefab] = new ObjectPool.PoolCounter());
				}
				poolCounter.startupCount = Mathf.Max(poolCounter.startupCount, startupPool.size);
			}
		}
		try
		{
			foreach (PersonalObjectPool personalObjectPool in PersonalObjectPool._activePoolManagers)
			{
				if (!(personalObjectPool == null) && personalObjectPool.startupPool != null)
				{
					for (int j = 0; j < personalObjectPool.startupPool.Count; j++)
					{
						global::StartupPool startupPool2 = personalObjectPool.startupPool[j];
						if (!(startupPool2.prefab == null))
						{
							ObjectPool.PoolCounter poolCounter2;
							if (!dictionary.TryGetValue(startupPool2.prefab, out poolCounter2))
							{
								poolCounter2 = (dictionary[startupPool2.prefab] = new ObjectPool.PoolCounter());
							}
							if (personalObjectPool.StealFromParent)
							{
								poolCounter2.sharedPoolCount = Mathf.Max(poolCounter2.sharedPoolCount, startupPool2.size);
							}
							else
							{
								poolCounter2.personalPoolCount += startupPool2.size;
							}
						}
					}
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		foreach (KeyValuePair<GameObject, List<GameObject>> keyValuePair in this.pooledObjects)
		{
			GameObject key = keyValuePair.Key;
			List<GameObject> value = keyValuePair.Value;
			int num = 0;
			ObjectPool.PoolCounter poolCounter3;
			if (key && dictionary.TryGetValue(key, out poolCounter3))
			{
				num = poolCounter3.RequiredCount;
			}
			for (int k = value.Count - 1; k >= 0; k--)
			{
				if (!value[k])
				{
					value.RemoveAt(k);
				}
			}
			while (value.Count > num)
			{
				Object.Destroy(value[0]);
				value.RemoveAt(0);
			}
			if (num == 0)
			{
				this.noInstancePrefabs.Add(key);
			}
			else if (value.Count < num)
			{
				ObjectPool.CreatePool(key, num - value.Count, false);
			}
		}
		foreach (GameObject key2 in this.noInstancePrefabs)
		{
			this.pooledObjects.Remove(key2);
		}
		this.noInstancePrefabs.Clear();
		ObjectPool.PurgeRecentRecycled();
		ObjectPool.AuditSpawnedDictionary();
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x00005A84 File Offset: 0x00003C84
	public static void PurgeRecentRecycled()
	{
		ObjectPool.recentlyRecycled.Clear();
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00005A90 File Offset: 0x00003C90
	public static void AuditSpawnedDictionary()
	{
		ObjectPool instance = ObjectPool.instance;
		if (instance == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>(instance.spawnedObjects.Count);
		foreach (GameObject gameObject in instance.spawnedObjects.Keys)
		{
			if (gameObject == null)
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject key in list)
		{
			instance.spawnedObjects.Remove(key);
		}
		bool flag = list.Count > 0;
		list.Clear();
		if (flag)
		{
			foreach (KeyValuePair<GameObject, List<GameObject>> keyValuePair in instance.pooledObjects)
			{
				keyValuePair.Value.RemoveAll((GameObject o) => o == null);
			}
		}
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00005BD8 File Offset: 0x00003DD8
	public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, parent, position, rotation).GetComponent<T>();
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00005BF2 File Offset: 0x00003DF2
	public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, null, position, rotation).GetComponent<T>();
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x00005C0C File Offset: 0x00003E0C
	public static T Spawn<T>(T prefab, Transform parent, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, parent, position, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060000CA RID: 202 RVA: 0x00005C2A File Offset: 0x00003E2A
	public static T Spawn<T>(T prefab, Vector3 position) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, null, position, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00005C48 File Offset: 0x00003E48
	public static T Spawn<T>(T prefab, Transform parent) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00005C6A File Offset: 0x00003E6A
	public static T Spawn<T>(T prefab) where T : Component
	{
		return ObjectPool.Spawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity).GetComponent<T>();
	}

	// Token: 0x060000CD RID: 205 RVA: 0x00005C8C File Offset: 0x00003E8C
	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
	{
		return ObjectPool.Spawn(prefab, parent, position, rotation, false);
	}

	// Token: 0x060000CE RID: 206 RVA: 0x00005C98 File Offset: 0x00003E98
	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, bool stealActiveSpawned = false)
	{
		if (prefab == null)
		{
			return null;
		}
		bool flag = false;
		bool activeRecyclerType = prefab.GetComponent<ActiveRecycler>() != null;
		if (!stealActiveSpawned && prefab.GetComponent<CurrencyObjectBase>())
		{
			stealActiveSpawned = true;
		}
		ObjectPool instance = ObjectPool.instance;
		List<GameObject> list;
		GameObject gameObject;
		if (!instance.pooledObjects.TryGetValue(prefab, out list))
		{
			ObjectPool.CreatePool(prefab.gameObject, 1, true, position, rotation, false);
			gameObject = ObjectPool.Spawn(prefab, parent, position, rotation);
			gameObject.SetActive(true);
			return gameObject;
		}
		gameObject = null;
		bool flag2 = false;
		while (list.Count > 0 && !flag2)
		{
			int index = list.Count - 1;
			gameObject = list[index];
			flag2 = gameObject;
			list.RemoveAt(index);
		}
		if (stealActiveSpawned && (list.Count <= 1 || !flag2))
		{
			Vector2 a = HeroController.instance.transform.position;
			float num = 0f;
			GameObject gameObject2 = null;
			foreach (KeyValuePair<GameObject, GameObject> keyValuePair in instance.spawnedObjects)
			{
				if (!(keyValuePair.Value != prefab))
				{
					GameObject key = keyValuePair.Key;
					if (!(key == null))
					{
						Vector3 position2 = key.transform.position;
						float num2 = Vector2.SqrMagnitude(a - new Vector2(position2.x, position2.y));
						if (num2 > num || !(gameObject2 != null))
						{
							gameObject2 = key;
							num = num2;
						}
					}
				}
			}
			if (gameObject2 != null)
			{
				if (!flag2)
				{
					ObjectPool.RecycleProcess(gameObject2);
					gameObject = gameObject2;
					flag2 = true;
					flag = true;
				}
				else
				{
					DropRecycle component = gameObject2.GetComponent<DropRecycle>();
					if (component)
					{
						component.StartDrop();
					}
				}
			}
		}
		if (flag2)
		{
			Transform transform = gameObject.transform;
			transform.parent = parent;
			transform.localPosition = position;
			transform.localRotation = rotation;
			ObjectPool.ActivateSpawningObject(gameObject, activeRecyclerType);
			if (!flag)
			{
				instance.AddSpawnedObject(gameObject, prefab);
				ObjectPoolAuditor.RecordSpawned(prefab, false);
			}
			return gameObject;
		}
		gameObject = Object.Instantiate<GameObject>(prefab, position, rotation, parent);
		Transform transform2 = gameObject.transform;
		transform2.localPosition = position;
		transform2.localRotation = rotation;
		ObjectPool.ActivateSpawningObject(gameObject, activeRecyclerType);
		instance.AddSpawnedObject(gameObject, prefab);
		ObjectPoolAuditor.RecordSpawned(prefab, true);
		PersonalObjectPool.RegisterNewSpawned(prefab, 1);
		return gameObject;
	}

	// Token: 0x060000CF RID: 207 RVA: 0x00005ED8 File Offset: 0x000040D8
	public static bool ObjectWasSpawned(GameObject gameObject)
	{
		GameObject gameObject2;
		return !(ObjectPool.instance == null) && ObjectPool.instance.spawnedObjects.TryGetValue(gameObject, out gameObject2);
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00005F06 File Offset: 0x00004106
	private static void ActivateSpawningObject(GameObject obj, bool activeRecyclerType)
	{
		if (activeRecyclerType)
		{
			FSMUtility.SendEventToGameObject(obj, "A SPAWN", false);
			return;
		}
		obj.SetActive(true);
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00005F1F File Offset: 0x0000411F
	private void AddSpawnedObject(GameObject obj, GameObject prefab)
	{
		this.spawnedObjects.TryAdd(obj, prefab);
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00005F2F File Offset: 0x0000412F
	private bool RemoveSpawnedObject(GameObject obj)
	{
		return this.spawnedObjects.Remove(obj);
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00005F3D File Offset: 0x0000413D
	public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position)
	{
		return ObjectPool.Spawn(prefab, parent, position, Quaternion.identity);
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00005F4C File Offset: 0x0000414C
	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return ObjectPool.Spawn(prefab, null, position, rotation);
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00005F57 File Offset: 0x00004157
	public static GameObject Spawn(GameObject prefab, Transform parent)
	{
		return ObjectPool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00005F6A File Offset: 0x0000416A
	public static GameObject Spawn(GameObject prefab, Vector3 position)
	{
		return ObjectPool.Spawn(prefab, null, position, Quaternion.identity);
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00005F79 File Offset: 0x00004179
	public static GameObject Spawn(GameObject prefab)
	{
		return ObjectPool.Spawn(prefab, null, Vector3.zero, Quaternion.identity);
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00005F8C File Offset: 0x0000418C
	public static void Recycle<T>(T obj) where T : Component
	{
		ObjectPool.Recycle(obj.gameObject);
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00005FA0 File Offset: 0x000041A0
	public static void Recycle(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		RecycleResetHandler component = obj.GetComponent<RecycleResetHandler>();
		if (component)
		{
			component.OnPreRecycle();
		}
		GameObject prefab;
		if (ObjectPool.instance != null && ObjectPool.instance.spawnedObjects.TryGetValue(obj, out prefab))
		{
			ObjectPool.Recycle(obj, prefab);
			return;
		}
		if (!ObjectPool.recentlyRecycled.Contains(obj))
		{
			ObjectPoolAuditor.RecordDespawned(obj, false);
			Object.Destroy(obj);
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00006010 File Offset: 0x00004210
	private static void Recycle(GameObject obj, GameObject prefab)
	{
		if (obj == null || prefab == null)
		{
			return;
		}
		bool flag = ObjectPool.instance.RemoveSpawnedObject(obj);
		List<GameObject> list;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			if (flag)
			{
				ObjectPool.recentlyRecycled.Add(obj);
				list.Add(obj);
			}
			ObjectPool.RecycleProcess(obj);
			ObjectPoolAuditor.RecordDespawned(obj, true);
			return;
		}
		ObjectPoolAuditor.RecordDespawned(obj, false);
		Object.Destroy(obj);
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00006084 File Offset: 0x00004284
	private static void RecycleProcess(GameObject obj)
	{
		ResetDynamicHierarchy component = obj.GetComponent<ResetDynamicHierarchy>();
		if (component)
		{
			component.DoReset(false);
		}
		if (!obj.activeInHierarchy)
		{
			ObjectPool.reparentList.Add(obj.gameObject);
		}
		else
		{
			obj.transform.parent = ObjectPool.instance.transform;
		}
		if (obj.GetComponent<ActiveRecycler>() != null)
		{
			ObjectPool.SetActiveRecycled(obj);
			return;
		}
		obj.SetActive(false);
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000060F2 File Offset: 0x000042F2
	private static void SetActiveRecycled(GameObject obj)
	{
		obj.transform.SetPosition2D(ObjectPool.activeStashLocation);
		FSMUtility.SendEventToGameObject(obj, "A RECYCLE", false);
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00006110 File Offset: 0x00004310
	public static void RecycleAll<T>(T prefab) where T : Component
	{
		ObjectPool.RecycleAll(prefab.gameObject);
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00006124 File Offset: 0x00004324
	public static void RecycleAll(GameObject prefab)
	{
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in ObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				ObjectPool._tempList.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < ObjectPool._tempList.Count; i++)
		{
			ObjectPool.Recycle(ObjectPool._tempList[i]);
		}
		ObjectPool._tempList.Clear();
	}

	// Token: 0x060000DF RID: 223 RVA: 0x000061C4 File Offset: 0x000043C4
	public static void RecycleAll()
	{
		if (!ObjectPool.instance)
		{
			return;
		}
		ObjectPool._tempList.AddRange(ObjectPool.instance.spawnedObjects.Keys);
		for (int i = 0; i < ObjectPool._tempList.Count; i++)
		{
			ObjectPool.Recycle(ObjectPool._tempList[i]);
		}
		ObjectPool._tempList.Clear();
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00006226 File Offset: 0x00004426
	public static bool IsSpawned(GameObject obj)
	{
		return ObjectPool.instance.spawnedObjects.ContainsKey(obj);
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00006238 File Offset: 0x00004438
	public static bool IsParentsPooledRecursive(GameObject obj)
	{
		if (ObjectPool.instance.spawnedObjects.ContainsKey(obj))
		{
			return true;
		}
		foreach (KeyValuePair<GameObject, List<GameObject>> keyValuePair in ObjectPool.instance.pooledObjects)
		{
			using (List<GameObject>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == obj)
					{
						return true;
					}
				}
			}
		}
		Transform parent = obj.transform.parent;
		return parent && ObjectPool.IsParentsPooledRecursive(parent.gameObject);
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x00006308 File Offset: 0x00004508
	public static int CountPooled<T>(T prefab) where T : Component
	{
		return ObjectPool.CountPooled(prefab.gameObject);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x0000631C File Offset: 0x0000451C
	public static int CountPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			return list.Count;
		}
		return 0;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00006345 File Offset: 0x00004545
	public static int CountSpawned<T>(T prefab) where T : Component
	{
		return ObjectPool.CountSpawned(prefab.gameObject);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00006358 File Offset: 0x00004558
	public static int CountSpawned(GameObject prefab)
	{
		int num = 0;
		foreach (GameObject y in ObjectPool.instance.spawnedObjects.Values)
		{
			if (prefab == y)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x000063C0 File Offset: 0x000045C0
	public static int CountAllPooled()
	{
		int num = 0;
		foreach (List<GameObject> list in ObjectPool.instance.pooledObjects.Values)
		{
			num += list.Count;
		}
		return num;
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00006424 File Offset: 0x00004624
	public static IEnumerable<ValueTuple<GameObject, int>> EnumerateAllPooledCounts()
	{
		foreach (KeyValuePair<GameObject, List<GameObject>> keyValuePair in ObjectPool.instance.pooledObjects)
		{
			yield return new ValueTuple<GameObject, int>(keyValuePair.Key, keyValuePair.Value.Count);
		}
		Dictionary<GameObject, List<GameObject>>.Enumerator enumerator = default(Dictionary<GameObject, List<GameObject>>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00006430 File Offset: 0x00004630
	public static int GetStartupCount(GameObject prefab)
	{
		foreach (ObjectPool.StartupPool startupPool in ObjectPool.instance.startupPools)
		{
			if (startupPool.prefab == prefab)
			{
				return startupPool.size;
			}
		}
		return 0;
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006470 File Offset: 0x00004670
	public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		List<GameObject> collection;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab, out collection))
		{
			list.AddRange(collection);
		}
		return list;
	}

	// Token: 0x060000EA RID: 234 RVA: 0x000064AC File Offset: 0x000046AC
	public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		List<GameObject> list2;
		if (ObjectPool.instance.pooledObjects.TryGetValue(prefab.gameObject, out list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				list.Add(list2[i].GetComponent<T>());
			}
		}
		return list;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00006510 File Offset: 0x00004710
	public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
	{
		if (list == null)
		{
			list = new List<GameObject>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in ObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == prefab)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00006590 File Offset: 0x00004790
	public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
	{
		if (list == null)
		{
			list = new List<T>();
		}
		if (!appendList)
		{
			list.Clear();
		}
		GameObject gameObject = prefab.gameObject;
		foreach (KeyValuePair<GameObject, GameObject> keyValuePair in ObjectPool.instance.spawnedObjects)
		{
			if (keyValuePair.Value == gameObject && !(keyValuePair.Key == null))
			{
				list.Add(keyValuePair.Key.GetComponent<T>());
			}
		}
		return list;
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00006630 File Offset: 0x00004830
	public static void DestroyPooled(GameObject prefab)
	{
		List<GameObject> list;
		if (!ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			return;
		}
		foreach (GameObject obj in list)
		{
			Object.Destroy(obj);
		}
		list.Clear();
		ObjectPool._spareGameObjectPools.Enqueue(list);
		ObjectPool.instance.pooledObjects.Remove(prefab);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000066B4 File Offset: 0x000048B4
	public static void DestroyPooled<T>(T prefab) where T : Component
	{
		ObjectPool.DestroyPooled(prefab.gameObject);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000066C8 File Offset: 0x000048C8
	public static void DestroyPooled(GameObject prefab, int amountToRemove)
	{
		ObjectPool.RecycleAll(prefab);
		List<GameObject> list;
		if (!ObjectPool.instance.pooledObjects.TryGetValue(prefab, out list))
		{
			return;
		}
		int num = 0;
		while (num < amountToRemove && list.Count > 0)
		{
			Object.Destroy(list[0]);
			list.RemoveAt(0);
			num++;
		}
		if (list.Count > 0)
		{
			return;
		}
		ObjectPool._spareGameObjectPools.Enqueue(list);
		ObjectPool.instance.pooledObjects.Remove(prefab);
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x0000673E File Offset: 0x0000493E
	public static void DestroyPooled<T>(T prefab, int amount) where T : Component
	{
		ObjectPool.DestroyPooled(prefab.gameObject, amount);
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00006751 File Offset: 0x00004951
	public static void DestroyAll(GameObject prefab)
	{
		ObjectPool.RecycleAll(prefab);
		ObjectPool.DestroyPooled(prefab);
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x0000675F File Offset: 0x0000495F
	public static void DestroyAll<T>(T prefab) where T : Component
	{
		ObjectPool.DestroyAll(prefab.gameObject);
	}

	// Token: 0x040000A5 RID: 165
	private static readonly List<GameObject> _tempList = new List<GameObject>();

	// Token: 0x040000A6 RID: 166
	private static readonly Queue<List<GameObject>> _spareGameObjectPools = new Queue<List<GameObject>>();

	// Token: 0x040000A7 RID: 167
	private readonly Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

	// Token: 0x040000A8 RID: 168
	private readonly Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

	// Token: 0x040000A9 RID: 169
	private static readonly HashSet<GameObject> recentlyRecycled = new HashSet<GameObject>();

	// Token: 0x040000AA RID: 170
	public ObjectPool.StartupPool[] startupPools;

	// Token: 0x040000AB RID: 171
	public bool isPrimary;

	// Token: 0x040000AC RID: 172
	private bool startupPoolsCreated;

	// Token: 0x040000AD RID: 173
	private static readonly Vector2 activeStashLocation = new Vector2(-20f, -20f);

	// Token: 0x040000AF RID: 175
	private static ObjectPool _instance;

	// Token: 0x040000B0 RID: 176
	private static bool hasInstance;

	// Token: 0x040000B1 RID: 177
	private readonly List<GameObject> noInstancePrefabs = new List<GameObject>();

	// Token: 0x040000B2 RID: 178
	private static List<GameObject> reparentList = new List<GameObject>();

	// Token: 0x020013C0 RID: 5056
	[Serializable]
	public class StartupPool
	{
		// Token: 0x04008096 RID: 32918
		public int size;

		// Token: 0x04008097 RID: 32919
		public GameObject prefab;
	}

	// Token: 0x020013C1 RID: 5057
	private class PoolCounter
	{
		// Token: 0x17000C4B RID: 3147
		// (get) Token: 0x06008163 RID: 33123 RVA: 0x002615C5 File Offset: 0x0025F7C5
		public int RequiredCount
		{
			get
			{
				return Mathf.Max(new int[]
				{
					this.startupCount,
					this.personalPoolCount,
					this.sharedPoolCount
				});
			}
		}

		// Token: 0x04008098 RID: 32920
		public int startupCount;

		// Token: 0x04008099 RID: 32921
		public int personalPoolCount;

		// Token: 0x0400809A RID: 32922
		public int sharedPoolCount;
	}
}
