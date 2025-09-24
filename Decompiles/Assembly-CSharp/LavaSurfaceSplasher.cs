using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000510 RID: 1296
public sealed class LavaSurfaceSplasher : MonoBehaviour
{
	// Token: 0x06002E3C RID: 11836 RVA: 0x000CB0CC File Offset: 0x000C92CC
	private void OnEnable()
	{
		LavaSurfaceSplasher.splashers[base.gameObject] = this;
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000CB0DF File Offset: 0x000C92DF
	private void OnDisable()
	{
		LavaSurfaceSplasher.splashers.Remove(base.gameObject);
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000CB0F2 File Offset: 0x000C92F2
	public static bool TryGetSplasher(GameObject go, out LavaSurfaceSplasher splasher)
	{
		return LavaSurfaceSplasher.splashers.TryGetValue(go, out splasher);
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000CB100 File Offset: 0x000C9300
	public static bool TrySplash(GameObject go)
	{
		LavaSurfaceSplasher lavaSurfaceSplasher;
		if (LavaSurfaceSplasher.TryGetSplasher(go, out lavaSurfaceSplasher))
		{
			lavaSurfaceSplasher.DoSplash();
			return true;
		}
		return false;
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000CB120 File Offset: 0x000C9320
	public void DoSplash()
	{
		UnityEvent unityEvent = this.onSplash;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		if (this.recycleOnSplash)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x0400307F RID: 12415
	[SerializeField]
	private bool recycleOnSplash;

	// Token: 0x04003080 RID: 12416
	public UnityEvent onSplash;

	// Token: 0x04003081 RID: 12417
	private static Dictionary<GameObject, LavaSurfaceSplasher> splashers = new Dictionary<GameObject, LavaSurfaceSplasher>();
}
