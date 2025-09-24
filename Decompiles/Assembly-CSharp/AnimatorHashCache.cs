using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
[Serializable]
public class AnimatorHashCache
{
	// Token: 0x06000284 RID: 644 RVA: 0x0000EC3D File Offset: 0x0000CE3D
	public AnimatorHashCache(string name)
	{
		this.name = name;
		this.cachedHash = Animator.StringToHash(name);
		this.valid = true;
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000285 RID: 645 RVA: 0x0000EC5F File Offset: 0x0000CE5F
	public int Hash
	{
		get
		{
			this.Update();
			return this.cachedHash;
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x06000286 RID: 646 RVA: 0x0000EC6D File Offset: 0x0000CE6D
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000EC75 File Offset: 0x0000CE75
	public void Update()
	{
		if (this.valid)
		{
			return;
		}
		this.cachedHash = Animator.StringToHash(this.name);
		this.valid = true;
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000EC98 File Offset: 0x0000CE98
	public void Dirty()
	{
		this.valid = false;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000ECA1 File Offset: 0x0000CEA1
	public static implicit operator int(AnimatorHashCache cache)
	{
		if (cache == null)
		{
			return 0;
		}
		return cache.Hash;
	}

	// Token: 0x0400022A RID: 554
	[SerializeField]
	private string name;

	// Token: 0x0400022B RID: 555
	private int cachedHash;

	// Token: 0x0400022C RID: 556
	private bool valid;
}
