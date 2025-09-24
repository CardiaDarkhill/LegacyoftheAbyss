using System;
using TeamCherry.ObjectPool;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class PooledEffectTracker<T> where T : PooledEffect, IPoolable<T>
{
	// Token: 0x06001604 RID: 5636 RVA: 0x00062C28 File Offset: 0x00060E28
	public PooledEffectTracker(PooledEffectProfile profile)
	{
		this.profile = profile;
		this.recycleQueue = new UniqueList<T>();
		this.effectPool = new ObjectPool<T>(new ObjectPool<T>.ObjectReturnDelegate(this.CreateNewAction), new ObjectPool<T>.ObjectPassDelegate(this.OnGet), new ObjectPool<T>.ObjectPassDelegate(this.OnRelease), new ObjectPool<T>.ObjectPassDelegate(this.OnDestroy), profile.InitialSize);
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x00062C90 File Offset: 0x00060E90
	private T CreateNewAction()
	{
		T t = this.profile.EffectPrefab as T;
		if (t != null)
		{
			T t2 = Object.Instantiate<T>(t, PooledEffectManager.Instance.transform);
			t2.SetReleaser(this.profile);
			return t2;
		}
		return default(T);
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x00062CE6 File Offset: 0x00060EE6
	private void OnGet(T element)
	{
		this.spawnedCount++;
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x00062CF6 File Offset: 0x00060EF6
	private void OnRelease(T element)
	{
		this.ReduceSpawnCount();
		element.OnRelease();
		element.transform.SetParent(PooledEffectManager.Instance.transform, false);
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x00062D24 File Offset: 0x00060F24
	public void OnDestroy(T element)
	{
		if (element != null)
		{
			this.recycleQueue.Remove(element);
			element.ClearReleaser();
			this.ReduceSpawnCount();
		}
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x00062D54 File Offset: 0x00060F54
	public void ReduceSpawnCount()
	{
		int a = 0;
		int b = this.spawnedCount - 1;
		this.spawnedCount = b;
		this.spawnedCount = Mathf.Max(a, b);
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x00062D80 File Offset: 0x00060F80
	public void Update()
	{
		this.recycleQueue.ReserveListUsage();
		foreach (T t in this.recycleQueue.List)
		{
			if (t.CanRelease())
			{
				this.ReleaseEffect(t);
			}
		}
		this.recycleQueue.ReleaseListUsage();
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x00062DFC File Offset: 0x00060FFC
	public void ReleaseEffect(T effect)
	{
		this.recycleQueue.Remove(effect);
		this.effectPool.Release(effect);
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x00062E17 File Offset: 0x00061017
	public bool EnqueueRelease(T effect)
	{
		return this.recycleQueue.Add(effect);
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x00062E25 File Offset: 0x00061025
	public bool TryGet(out T effect)
	{
		if (this.spawnedCount >= this.profile.PoolCapacity)
		{
			effect = default(T);
			return false;
		}
		effect = this.effectPool.Get();
		return true;
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x00062E55 File Offset: 0x00061055
	public void Clear()
	{
		ObjectPool<T> objectPool = this.effectPool;
		if (objectPool == null)
		{
			return;
		}
		objectPool.Clear();
	}

	// Token: 0x0400147A RID: 5242
	private int spawnedCount;

	// Token: 0x0400147B RID: 5243
	private PooledEffectProfile profile;

	// Token: 0x0400147C RID: 5244
	private ObjectPool<T> effectPool;

	// Token: 0x0400147D RID: 5245
	private UniqueList<T> recycleQueue;
}
