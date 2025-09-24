using System;
using TeamCherry.ObjectPool;
using UnityEngine;

// Token: 0x02000263 RID: 611
public class PooledEffect : MonoBehaviour, IPoolable<PooledEffect>
{
	// Token: 0x060015E9 RID: 5609 RVA: 0x0006282D File Offset: 0x00060A2D
	private void OnDestroy()
	{
		if (this.pooledEffectProfile != null)
		{
			this.pooledEffectProfile.NotifyDestroyed(this);
			this.pooledEffectProfile = null;
		}
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x00062850 File Offset: 0x00060A50
	private void OnDisable()
	{
		this.Release();
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x00062858 File Offset: 0x00060A58
	public void SetReleaser(IPoolReleaser<PooledEffect> releaser)
	{
		PooledEffectProfile pooledEffectProfile = releaser as PooledEffectProfile;
		if (pooledEffectProfile != null)
		{
			this.pooledEffectProfile = pooledEffectProfile;
		}
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x00062876 File Offset: 0x00060A76
	public void ClearReleaser()
	{
		this.pooledEffectProfile = null;
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x0006287F File Offset: 0x00060A7F
	public void Release()
	{
		if (!this.spawned)
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
			return;
		}
		if (this.pooledEffectProfile)
		{
			this.pooledEffectProfile.Release(this);
		}
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x000628BC File Offset: 0x00060ABC
	public virtual void OnSpawn()
	{
		this.spawned = true;
		this.releaseState = 0;
		base.gameObject.SetActive(true);
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x000628D8 File Offset: 0x00060AD8
	public virtual void OnRelease()
	{
		this.spawned = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x000628F0 File Offset: 0x00060AF0
	public virtual bool CanRelease()
	{
		if (this.releaseState >= 2)
		{
			return true;
		}
		Transform parent = base.transform.parent;
		if (parent == null || parent.gameObject.activeInHierarchy)
		{
			return true;
		}
		this.releaseState++;
		return false;
	}

	// Token: 0x04001470 RID: 5232
	private PooledEffectProfile pooledEffectProfile;

	// Token: 0x04001471 RID: 5233
	private bool spawned;

	// Token: 0x04001472 RID: 5234
	private int releaseState;
}
