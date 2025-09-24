using System;
using UnityEngine;

// Token: 0x020003EA RID: 1002
public class PreSpawnedItem : IDisposable
{
	// Token: 0x1700038E RID: 910
	// (get) Token: 0x0600223C RID: 8764 RVA: 0x0009DE8B File Offset: 0x0009C08B
	public GameObject SpawnedObject
	{
		get
		{
			return this.spawnedObject;
		}
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x0009DE93 File Offset: 0x0009C093
	public PreSpawnedItem(GameObject spawnedObject, bool recycle)
	{
		this.spawnedObject = spawnedObject;
		this.recycle = recycle;
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x0009DEAC File Offset: 0x0009C0AC
	~PreSpawnedItem()
	{
		this.Dispose();
	}

	// Token: 0x0600223F RID: 8767 RVA: 0x0009DED8 File Offset: 0x0009C0D8
	public void Dispose()
	{
		if (this.disposed)
		{
			return;
		}
		this.disposed = true;
		if (this.spawnedObject != null)
		{
			if (this.recycle)
			{
				this.spawnedObject.Recycle();
				return;
			}
			this.spawnedObject.SetActive(false);
		}
	}

	// Token: 0x06002240 RID: 8768 RVA: 0x0009DF18 File Offset: 0x0009C118
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		if (this.spawnedObject != null)
		{
			this.children = this.spawnedObject.GetComponentsInChildren<IInitialisable>(true);
			IInitialisable[] array = this.children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnAwake();
			}
		}
		return true;
	}

	// Token: 0x06002241 RID: 8769 RVA: 0x0009DF78 File Offset: 0x0009C178
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		if (this.children != null)
		{
			IInitialisable[] array = this.children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnStart();
			}
		}
		return true;
	}

	// Token: 0x04002109 RID: 8457
	private GameObject spawnedObject;

	// Token: 0x0400210A RID: 8458
	private bool recycle;

	// Token: 0x0400210B RID: 8459
	private bool disposed;

	// Token: 0x0400210C RID: 8460
	private bool hasAwaken;

	// Token: 0x0400210D RID: 8461
	private bool hasStarted;

	// Token: 0x0400210E RID: 8462
	private IInitialisable[] children;
}
