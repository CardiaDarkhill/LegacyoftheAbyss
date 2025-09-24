using System;
using UnityEngine;

// Token: 0x02000354 RID: 852
public interface IInitialisable
{
	// Token: 0x06001D8C RID: 7564
	bool OnAwake();

	// Token: 0x06001D8D RID: 7565
	bool OnStart();

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001D8E RID: 7566
	GameObject gameObject { get; }

	// Token: 0x06001D8F RID: 7567 RVA: 0x000886E4 File Offset: 0x000868E4
	public static void DoFullInit(GameObject gameObject)
	{
		foreach (IInitialisable initialisable in gameObject.GetComponentsInChildren<IInitialisable>(true))
		{
			initialisable.OnAwake();
			initialisable.OnStart();
		}
		PersonalObjectPool.CreateIfRequired(gameObject, !gameObject.activeSelf);
	}

	// Token: 0x06001D90 RID: 7568 RVA: 0x00088728 File Offset: 0x00086928
	public static void DoFullInitForcePool(GameObject gameObject)
	{
		foreach (IInitialisable initialisable in gameObject.GetComponentsInChildren<IInitialisable>(true))
		{
			initialisable.OnAwake();
			initialisable.OnStart();
		}
		PersonalObjectPool.CreateIfRequired(gameObject, true);
	}
}
