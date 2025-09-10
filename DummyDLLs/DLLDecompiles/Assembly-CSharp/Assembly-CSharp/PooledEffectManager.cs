using System;
using TeamCherry.ObjectPool;
using UnityEngine;

// Token: 0x02000264 RID: 612
public sealed class PooledEffectManager : MonoBehaviour
{
	// Token: 0x17000256 RID: 598
	// (get) Token: 0x060015F2 RID: 5618 RVA: 0x00062944 File Offset: 0x00060B44
	public static PooledEffectManager Instance
	{
		get
		{
			if (!PooledEffectManager.hasInstance)
			{
				PooledEffectManager.instance = Object.FindObjectOfType<PooledEffectManager>();
				if (PooledEffectManager.instance == null)
				{
					PooledEffectManager.instance = new GameObject("PooledEffectManager").AddComponent<PooledEffectManager>();
					Object.DontDestroyOnLoad(PooledEffectManager.instance.gameObject);
				}
				PooledEffectManager.hasInstance = true;
			}
			return PooledEffectManager.instance;
		}
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x0006299D File Offset: 0x00060B9D
	private void Awake()
	{
		if (PooledEffectManager.instance != null && PooledEffectManager.instance != this)
		{
			base.enabled = false;
			return;
		}
		PooledEffectManager.hasInstance = true;
		PooledEffectManager.instance = this;
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x000629D0 File Offset: 0x00060BD0
	private void OnDestroy()
	{
		if (PooledEffectManager.hasInstance && PooledEffectManager.instance == this)
		{
			PooledEffectManager.profiles.ReserveListUsage();
			foreach (PooledEffectProfile pooledEffectProfile in PooledEffectManager.profiles.List)
			{
				pooledEffectProfile.Clear();
			}
			PooledEffectManager.profiles.Clear();
			PooledEffectManager.profiles.ReleaseListUsage();
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x00062A58 File Offset: 0x00060C58
	private void Update()
	{
		PooledEffectManager.profiles.ReserveListUsage();
		foreach (PooledEffectProfile pooledEffectProfile in PooledEffectManager.profiles.List)
		{
			pooledEffectProfile.Update();
		}
		PooledEffectManager.profiles.ReleaseListUsage();
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x00062AC0 File Offset: 0x00060CC0
	public static PooledEffectTracker<T> InitProfile<T>(PooledEffectProfile profile) where T : PooledEffect, IPoolable<T>
	{
		PooledEffectManager.profiles.Add(profile);
		return new PooledEffectTracker<T>(profile);
	}

	// Token: 0x04001473 RID: 5235
	private static UniqueList<PooledEffectProfile> profiles = new UniqueList<PooledEffectProfile>();

	// Token: 0x04001474 RID: 5236
	private static bool hasInstance;

	// Token: 0x04001475 RID: 5237
	private static PooledEffectManager instance;
}
