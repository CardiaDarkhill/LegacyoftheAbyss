using System;
using UnityEngine;

// Token: 0x0200043C RID: 1084
public class ManagerSingleton<T> : MonoBehaviour where T : ManagerSingleton<T>
{
	// Token: 0x170003DD RID: 989
	// (get) Token: 0x06002557 RID: 9559 RVA: 0x000AB25A File Offset: 0x000A945A
	public static T Instance
	{
		get
		{
			return ManagerSingleton<T>.SilentInstance;
		}
	}

	// Token: 0x170003DE RID: 990
	// (get) Token: 0x06002558 RID: 9560 RVA: 0x000AB261 File Offset: 0x000A9461
	public static T SilentInstance
	{
		get
		{
			if (ManagerSingleton<T>.UnsafeInstance == null)
			{
				ManagerSingleton<T>.UnsafeInstance = Object.FindAnyObjectByType<T>();
			}
			return ManagerSingleton<T>.UnsafeInstance;
		}
	}

	// Token: 0x170003DF RID: 991
	// (get) Token: 0x06002559 RID: 9561 RVA: 0x000AB284 File Offset: 0x000A9484
	// (set) Token: 0x0600255A RID: 9562 RVA: 0x000AB28B File Offset: 0x000A948B
	public static T UnsafeInstance { get; private set; }

	// Token: 0x0600255B RID: 9563 RVA: 0x000AB293 File Offset: 0x000A9493
	protected virtual void Awake()
	{
		if (ManagerSingleton<T>.UnsafeInstance == null)
		{
			ManagerSingleton<T>.UnsafeInstance = (T)((object)this);
			return;
		}
		if (ManagerSingleton<T>.UnsafeInstance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000AB2CC File Offset: 0x000A94CC
	protected virtual void OnDestroy()
	{
		if (ManagerSingleton<T>.UnsafeInstance == this)
		{
			ManagerSingleton<T>.UnsafeInstance = default(T);
		}
	}
}
