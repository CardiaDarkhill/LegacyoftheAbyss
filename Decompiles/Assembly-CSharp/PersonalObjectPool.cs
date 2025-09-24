using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200035E RID: 862
public class PersonalObjectPool : MonoBehaviour, IInitialisable
{
	// Token: 0x170002FB RID: 763
	// (get) Token: 0x06001DD2 RID: 7634 RVA: 0x00089CF6 File Offset: 0x00087EF6
	public GameObject StealFromParent
	{
		get
		{
			return this.stealFromParent;
		}
	}

	// Token: 0x06001DD3 RID: 7635 RVA: 0x00089CFE File Offset: 0x00087EFE
	public PersonalObjectPool(bool createdStartupPools)
	{
		this.createdStartupPools = createdStartupPools;
	}

	// Token: 0x06001DD4 RID: 7636 RVA: 0x00089D10 File Offset: 0x00087F10
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.sceneName = base.gameObject.scene.name;
		PersonalObjectPool._activePoolManagers.Add(this);
		if (this.startupPool == null)
		{
			this.startupPool = new List<StartupPool>();
		}
		if (this.stealFromParent)
		{
			List<StartupPool> list = new List<StartupPool>();
			PersonalObjectPool[] componentsInChildren = this.stealFromParent.GetComponentsInChildren<PersonalObjectPool>(true);
			for (int i = componentsInChildren.Length - 1; i >= 0; i--)
			{
				PersonalObjectPool personalObjectPool = componentsInChildren[i];
				if (!(personalObjectPool == this) && personalObjectPool.startupPool != null)
				{
					for (int j = personalObjectPool.startupPool.Count - 1; j >= 0; j--)
					{
						StartupPool startupPool = personalObjectPool.startupPool[j];
						if (startupPool.SpawnedCount <= 0)
						{
							list.Add(startupPool);
							personalObjectPool.startupPool.RemoveAt(j);
						}
					}
				}
			}
			IEnumerable<StartupPool> collection = (from pool in list
			group pool by pool.prefab).SelectMany((IGrouping<GameObject, StartupPool> group) => (from pool in @group
			orderby pool.size descending
			select pool).Take(1)).Select(delegate(StartupPool pool)
			{
				pool.size *= 2;
				return pool;
			});
			this.startupPool.AddRange(collection);
		}
		for (int k = this.startupPool.Count - 1; k >= 0; k--)
		{
			if (!this.startupPool[k].prefab)
			{
				this.startupPool.RemoveAt(k);
			}
		}
		return true;
	}

	// Token: 0x06001DD5 RID: 7637 RVA: 0x00089EBF File Offset: 0x000880BF
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		this.CreateStartupPools();
		this.startFinished = true;
		return true;
	}

	// Token: 0x06001DD6 RID: 7638 RVA: 0x00089EE7 File Offset: 0x000880E7
	protected void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06001DD7 RID: 7639 RVA: 0x00089EF0 File Offset: 0x000880F0
	private void Start()
	{
		this.OnStart();
	}

	// Token: 0x06001DD8 RID: 7640 RVA: 0x00089EF9 File Offset: 0x000880F9
	private void OnDestroy()
	{
		if (!this.createdStartupPools)
		{
			PersonalObjectPool._activePoolManagers.Remove(this);
		}
	}

	// Token: 0x06001DD9 RID: 7641 RVA: 0x00089F10 File Offset: 0x00088110
	public static void PreUnloadingScene(string unloadingSceneName)
	{
		int i = PersonalObjectPool._activePoolManagers.Count - 1;
		while (i >= 0)
		{
			PersonalObjectPool personalObjectPool = PersonalObjectPool._activePoolManagers[i];
			if (!personalObjectPool)
			{
				goto IL_46;
			}
			string baseSceneName = GameManager.GetBaseSceneName(personalObjectPool.sceneName);
			if (!(unloadingSceneName != baseSceneName) || !(unloadingSceneName != personalObjectPool.sceneName))
			{
				goto IL_46;
			}
			IL_5C:
			i--;
			continue;
			IL_46:
			PersonalObjectPool._activePoolManagers.RemoveAt(i);
			PersonalObjectPool._inactivePoolManagers.Add(personalObjectPool);
			goto IL_5C;
		}
	}

	// Token: 0x06001DDA RID: 7642 RVA: 0x00089F84 File Offset: 0x00088184
	public static void UnloadingScene(string unloadingSceneName)
	{
		foreach (PersonalObjectPool personalObjectPool in PersonalObjectPool._inactivePoolManagers)
		{
			personalObjectPool.DestroyMyPooledObjects();
		}
		PersonalObjectPool._inactivePoolManagers.Clear();
	}

	// Token: 0x06001DDB RID: 7643 RVA: 0x00089FE0 File Offset: 0x000881E0
	public static void ForceReleasePoolManagers()
	{
		foreach (PersonalObjectPool personalObjectPool in PersonalObjectPool._activePoolManagers)
		{
			personalObjectPool.DestroyMyPooledObjects();
		}
		foreach (PersonalObjectPool personalObjectPool2 in PersonalObjectPool._inactivePoolManagers)
		{
			personalObjectPool2.DestroyMyPooledObjects();
		}
		PersonalObjectPool._activePoolManagers.Clear();
		PersonalObjectPool._inactivePoolManagers.Clear();
	}

	// Token: 0x06001DDC RID: 7644 RVA: 0x0008A084 File Offset: 0x00088284
	public void CreateStartupPools()
	{
		this.createdStartupPools = true;
		List<StartupPool> list = this.startupPool;
		if (list == null || list.Count <= 0)
		{
			return;
		}
		this.poolsAdded = false;
		for (int i = 0; i < this.startupPool.Count; i++)
		{
			StartupPool startupPool = this.startupPool[i];
			GameObject prefab = startupPool.prefab;
			if (prefab)
			{
				int num = startupPool.size - startupPool.SpawnedCount;
				if (num > 0)
				{
					bool flag = PersonalObjectPool._extraSpawnedCount.ContainsKey(prefab);
					if (flag)
					{
						int num2 = PersonalObjectPool._extraSpawnedCount[prefab];
						while (num > 0 && num2 > 0)
						{
							num--;
							num2--;
						}
					}
					if (num > 0)
					{
						ObjectPool.GetPooled(prefab, PersonalObjectPool._tempList, true);
						int count = PersonalObjectPool._tempList.Count;
						PersonalObjectPool._tempList.Clear();
						this.CreatePool(prefab, num, startupPool.initialiseSpawnedObjects, startupPool.shared);
						ObjectPool.GetPooled(prefab, PersonalObjectPool._tempList, true);
						int count2 = PersonalObjectPool._tempList.Count;
						PersonalObjectPool._tempList.Clear();
						int num3 = count2 - count;
						startupPool.SpawnedCount += num3;
						this.startupPool[i] = startupPool;
						if (!flag)
						{
							PersonalObjectPool._extraSpawnedCount.Add(prefab, 0);
						}
					}
				}
			}
		}
	}

	// Token: 0x06001DDD RID: 7645 RVA: 0x0008A1CC File Offset: 0x000883CC
	public void CreatePool(GameObject prefab, int initialPoolSize, bool runInitialisation, bool shared)
	{
		int num = initialPoolSize;
		if (num <= 0)
		{
			return;
		}
		if (!shared)
		{
			ObjectPool.GetPooled(prefab, PersonalObjectPool._tempList, true);
			num += PersonalObjectPool._tempList.Count;
		}
		ObjectPool.CreatePool(prefab, num, runInitialisation);
		PersonalObjectPool._tempList.Clear();
	}

	// Token: 0x06001DDE RID: 7646 RVA: 0x0008A210 File Offset: 0x00088410
	public void DestroyMyPooledObjects()
	{
		if (!this.createdStartupPools)
		{
			return;
		}
		this.createdStartupPools = false;
		for (int i = 0; i < this.startupPool.Count; i++)
		{
			StartupPool startupPool = this.startupPool[i];
			if (startupPool.SpawnedCount > 0)
			{
				for (int j = 0; j < 2; j++)
				{
					int num = 0;
					do
					{
						num = 0;
						bool flag = false;
						foreach (PersonalObjectPool personalObjectPool in PersonalObjectPool._activePoolManagers)
						{
							for (int k = 0; k < personalObjectPool.startupPool.Count; k++)
							{
								StartupPool startupPool2 = personalObjectPool.startupPool[k];
								if (!(startupPool2.prefab != startupPool.prefab) && (j != 0 || startupPool2.SpawnedCount < startupPool2.size))
								{
									num++;
									startupPool2.SpawnedCount++;
									startupPool.SpawnedCount--;
									personalObjectPool.startupPool[k] = startupPool2;
									this.startupPool[i] = startupPool;
									if (startupPool.SpawnedCount == 0)
									{
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								break;
							}
						}
					}
					while (startupPool.SpawnedCount > 0 && num > 0);
				}
			}
		}
		List<GameObject> list = new List<GameObject>();
		if (this.startupPool != null && this.startupPool.Count > 0)
		{
			foreach (StartupPool startupPool3 in this.startupPool)
			{
				if (startupPool3.SpawnedCount > 0 && startupPool3.prefab)
				{
					ObjectPool.DestroyPooled(startupPool3.prefab, startupPool3.SpawnedCount);
					if (!list.Contains(startupPool3.prefab))
					{
						list.Add(startupPool3.prefab);
					}
				}
			}
		}
		foreach (GameObject gameObject in list)
		{
			if (PersonalObjectPool._extraSpawnedCount.ContainsKey(gameObject))
			{
				int num2 = PersonalObjectPool._extraSpawnedCount[gameObject];
				if (num2 > 0)
				{
					ObjectPool.DestroyPooled(gameObject, num2);
				}
				PersonalObjectPool._extraSpawnedCount.Remove(gameObject);
			}
		}
	}

	// Token: 0x06001DDF RID: 7647 RVA: 0x0008A48C File Offset: 0x0008868C
	public static void RegisterNewSpawned(GameObject prefab, int amount)
	{
		if (PersonalObjectPool._extraSpawnedCount.ContainsKey(prefab))
		{
			Dictionary<GameObject, int> extraSpawnedCount = PersonalObjectPool._extraSpawnedCount;
			extraSpawnedCount[prefab] += amount;
		}
	}

	// Token: 0x06001DE0 RID: 7648 RVA: 0x0008A4C0 File Offset: 0x000886C0
	public static void EnsurePooledInScene(GameObject ownerObj, GameObject prefab, int poolAmount, bool finished = true, bool initialiseSpawned = false, bool shared = false)
	{
		GameManager.instance.EnsureGlobalPool();
		int num = ObjectPool.CountPooled(prefab);
		int size = Mathf.Max(0, poolAmount - num);
		PersonalObjectPool personalObjectPool = ownerObj.AddComponentIfNotPresent<PersonalObjectPool>();
		personalObjectPool.OnAwake();
		if (personalObjectPool.startupPool.All((StartupPool pool) => pool.prefab != prefab))
		{
			personalObjectPool.startupPool.Add(new StartupPool(size, prefab, initialiseSpawned, shared));
			personalObjectPool.poolsAdded = true;
		}
		if (finished && personalObjectPool.startFinished)
		{
			personalObjectPool.CreateStartupPools();
		}
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x0008A554 File Offset: 0x00088754
	public static void EnsurePooledInSceneFinished(GameObject ownerObj)
	{
		PersonalObjectPool component = ownerObj.GetComponent<PersonalObjectPool>();
		if (component)
		{
			component.CreateStartupPools();
		}
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x0008A578 File Offset: 0x00088778
	public static void CreateIfRequired(GameObject gameObject, bool forced = false)
	{
		PersonalObjectPool component = gameObject.GetComponent<PersonalObjectPool>();
		if (component != null && component.poolsAdded)
		{
			if (component.startFinished)
			{
				component.CreateStartupPools();
				return;
			}
			if (forced)
			{
				component.OnStart();
			}
		}
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x0008A5E0 File Offset: 0x000887E0
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04001CFD RID: 7421
	public List<StartupPool> startupPool;

	// Token: 0x04001CFE RID: 7422
	[SerializeField]
	[Tooltip("Will steal and aggregate all child pools. Good for optimising pooling by reducing the amount of objects pooled, assuming each pool wont need all at once.")]
	private GameObject stealFromParent;

	// Token: 0x04001CFF RID: 7423
	private bool createdStartupPools;

	// Token: 0x04001D00 RID: 7424
	private bool startFinished;

	// Token: 0x04001D01 RID: 7425
	private bool poolsAdded;

	// Token: 0x04001D02 RID: 7426
	private string sceneName;

	// Token: 0x04001D03 RID: 7427
	public static readonly List<PersonalObjectPool> _activePoolManagers = new List<PersonalObjectPool>();

	// Token: 0x04001D04 RID: 7428
	private static readonly List<PersonalObjectPool> _inactivePoolManagers = new List<PersonalObjectPool>();

	// Token: 0x04001D05 RID: 7429
	private static readonly Dictionary<GameObject, int> _extraSpawnedCount = new Dictionary<GameObject, int>();

	// Token: 0x04001D06 RID: 7430
	private static readonly List<GameObject> _tempList = new List<GameObject>();

	// Token: 0x04001D07 RID: 7431
	private bool hasAwaken;

	// Token: 0x04001D08 RID: 7432
	private bool hasStarted;
}
