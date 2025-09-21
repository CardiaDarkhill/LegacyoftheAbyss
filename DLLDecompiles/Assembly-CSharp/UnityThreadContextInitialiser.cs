using System;
using System.Threading;
using UnityEngine;

// Token: 0x0200046F RID: 1135
public sealed class UnityThreadContextInitialiser : MonoBehaviour
{
	// Token: 0x060028A3 RID: 10403 RVA: 0x000B2E83 File Offset: 0x000B1083
	private void Awake()
	{
		this.threadId = Thread.CurrentThread.ManagedThreadId;
		Action<int> action = this.initialisationCallback;
		if (action != null)
		{
			action(this.threadId);
		}
		this.initialisationCallback = null;
		this.init = true;
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000B2EBA File Offset: 0x000B10BA
	public void SetCallback(Action<int> callback)
	{
		if (!this.init)
		{
			this.initialisationCallback = (Action<int>)Delegate.Combine(this.initialisationCallback, callback);
			return;
		}
		if (callback != null)
		{
			callback(this.threadId);
		}
	}

	// Token: 0x040024B1 RID: 9393
	private bool init;

	// Token: 0x040024B2 RID: 9394
	private int threadId;

	// Token: 0x040024B3 RID: 9395
	private Action<int> initialisationCallback;
}
