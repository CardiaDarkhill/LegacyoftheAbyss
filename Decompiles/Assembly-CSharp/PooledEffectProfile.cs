using System;
using TeamCherry.ObjectPool;
using UnityEngine;

// Token: 0x02000265 RID: 613
[CreateAssetMenu(fileName = "Pooled Effect Profile", menuName = "Pooled Effect Profile")]
public sealed class PooledEffectProfile : ScriptableObject, IPoolReleaser<PooledEffect>
{
	// Token: 0x17000257 RID: 599
	// (get) Token: 0x060015F9 RID: 5625 RVA: 0x00062AE8 File Offset: 0x00060CE8
	public PooledEffect EffectPrefab
	{
		get
		{
			return this.effectPrefab;
		}
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x060015FA RID: 5626 RVA: 0x00062AF0 File Offset: 0x00060CF0
	public int InitialSize
	{
		get
		{
			return this.initialSize;
		}
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x060015FB RID: 5627 RVA: 0x00062AF8 File Offset: 0x00060CF8
	public int PoolCapacity
	{
		get
		{
			return this.poolCapacity;
		}
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x00062B00 File Offset: 0x00060D00
	public void Init()
	{
		if (this.effectTracker == null)
		{
			this.effectTracker = PooledEffectManager.InitProfile<PooledEffect>(this);
		}
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x00062B16 File Offset: 0x00060D16
	public void Update()
	{
		PooledEffectTracker<PooledEffect> pooledEffectTracker = this.effectTracker;
		if (pooledEffectTracker == null)
		{
			return;
		}
		pooledEffectTracker.Update();
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x00062B28 File Offset: 0x00060D28
	public void Release(PooledEffect element)
	{
		if (element == null)
		{
			return;
		}
		if (this.effectTracker == null)
		{
			Object.Destroy(element.gameObject);
			return;
		}
		if (element.CanRelease())
		{
			this.effectTracker.ReleaseEffect(element);
			return;
		}
		this.effectTracker.EnqueueRelease(element);
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x00062B75 File Offset: 0x00060D75
	public void NotifyDestroyed(PooledEffect element)
	{
		PooledEffectTracker<PooledEffect> pooledEffectTracker = this.effectTracker;
		if (pooledEffectTracker == null)
		{
			return;
		}
		pooledEffectTracker.OnDestroy(element);
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x00062B88 File Offset: 0x00060D88
	public void Clear()
	{
		PooledEffectTracker<PooledEffect> pooledEffectTracker = this.effectTracker;
		if (pooledEffectTracker != null)
		{
			pooledEffectTracker.Clear();
		}
		this.effectTracker = null;
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x00062BA2 File Offset: 0x00060DA2
	public void SpawnEffect(Transform parent)
	{
		this.SpawnEffect(Vector3.zero, Quaternion.identity, parent);
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x00062BB8 File Offset: 0x00060DB8
	public void SpawnEffect(Vector3 position, Quaternion rotation, Transform parent)
	{
		this.Init();
		PooledEffect pooledEffect;
		if (this.effectTracker != null && this.effectTracker.TryGet(out pooledEffect))
		{
			pooledEffect.transform.SetParent(parent, false);
			pooledEffect.transform.localPosition = position;
			pooledEffect.transform.localRotation = rotation;
			pooledEffect.OnSpawn();
		}
	}

	// Token: 0x04001476 RID: 5238
	[SerializeField]
	private PooledEffect effectPrefab;

	// Token: 0x04001477 RID: 5239
	[SerializeField]
	private int initialSize = 10;

	// Token: 0x04001478 RID: 5240
	[SerializeField]
	private int poolCapacity = 10;

	// Token: 0x04001479 RID: 5241
	private PooledEffectTracker<PooledEffect> effectTracker;
}
