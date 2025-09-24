using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004F3 RID: 1267
public class HarpoonHook : MonoBehaviour
{
	// Token: 0x06002D61 RID: 11617 RVA: 0x000C6147 File Offset: 0x000C4347
	public void HookStart()
	{
		this.OnHookStart.Invoke();
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x000C6154 File Offset: 0x000C4354
	public void HookEnd()
	{
		this.OnHookEnd.Invoke();
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x000C6161 File Offset: 0x000C4361
	public void HookCancel()
	{
		this.OnHookCancel.Invoke();
	}

	// Token: 0x04002F13 RID: 12051
	public UnityEvent OnHookStart;

	// Token: 0x04002F14 RID: 12052
	public UnityEvent OnHookEnd;

	// Token: 0x04002F15 RID: 12053
	public UnityEvent OnHookCancel;
}
