using System;
using UnityEngine;

// Token: 0x02000200 RID: 512
public class DeactivatePlayerDataTest : MonoBehaviour
{
	// Token: 0x0600136D RID: 4973 RVA: 0x00058A5C File Offset: 0x00056C5C
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
	}

	// Token: 0x0600136E RID: 4974 RVA: 0x00058A6B File Offset: 0x00056C6B
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		if (this.test.IsFulfilled)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040011DA RID: 4570
	[SerializeField]
	private PlayerDataTest test;

	// Token: 0x040011DB RID: 4571
	private bool hasStarted;
}
