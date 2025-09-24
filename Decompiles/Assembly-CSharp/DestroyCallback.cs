using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004CC RID: 1228
public sealed class DestroyCallback : MonoBehaviour
{
	// Token: 0x1400008D RID: 141
	// (add) Token: 0x06002C35 RID: 11317 RVA: 0x000C1A0C File Offset: 0x000BFC0C
	// (remove) Token: 0x06002C36 RID: 11318 RVA: 0x000C1A44 File Offset: 0x000BFC44
	public event Action OnDestroyed;

	// Token: 0x06002C37 RID: 11319 RVA: 0x000C1A79 File Offset: 0x000BFC79
	private void Awake()
	{
		DestroyCallback.instances[base.gameObject] = this;
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x000C1A8C File Offset: 0x000BFC8C
	private void OnDestroy()
	{
		DestroyCallback.instances.Remove(base.gameObject);
		this.DoCallback();
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x000C1AA5 File Offset: 0x000BFCA5
	private void DoCallback()
	{
		if (this.hasSentCallback)
		{
			return;
		}
		this.hasSentCallback = true;
		Action onDestroyed = this.OnDestroyed;
		if (onDestroyed == null)
		{
			return;
		}
		onDestroyed();
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x000C1AC8 File Offset: 0x000BFCC8
	public static void AddCallback(GameObject gameObject, Action callback)
	{
		if (gameObject == null)
		{
			return;
		}
		DestroyCallback destroyCallback;
		if (!DestroyCallback.instances.TryGetValue(gameObject, out destroyCallback))
		{
			destroyCallback = (DestroyCallback.instances[gameObject] = gameObject.AddComponentIfNotPresent<DestroyCallback>());
		}
		destroyCallback.OnDestroyed += callback;
	}

	// Token: 0x04002DA1 RID: 11681
	private bool hasSentCallback;

	// Token: 0x04002DA2 RID: 11682
	private static Dictionary<GameObject, DestroyCallback> instances = new Dictionary<GameObject, DestroyCallback>();
}
