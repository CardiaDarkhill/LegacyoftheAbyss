using System;
using UnityEngine;

// Token: 0x0200077F RID: 1919
[RequireComponent(typeof(Collider2D))]
public class RegionDebugger : MonoBehaviour
{
	// Token: 0x0600441E RID: 17438 RVA: 0x0012AFE6 File Offset: 0x001291E6
	private void Start()
	{
		Debug.LogErrorFormat(this, "Region debugger is exists in scene! These should be removed before release.", Array.Empty<object>());
	}
}
