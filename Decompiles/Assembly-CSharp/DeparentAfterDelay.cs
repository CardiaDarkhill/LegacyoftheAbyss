using System;
using UnityEngine;

// Token: 0x02000206 RID: 518
public class DeparentAfterDelay : MonoBehaviour
{
	// Token: 0x0600138D RID: 5005 RVA: 0x000593A4 File Offset: 0x000575A4
	private void Update()
	{
		if (!this.deparented)
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			base.gameObject.transform.parent = null;
			this.deparented = true;
			base.enabled = false;
		}
	}

	// Token: 0x040011F3 RID: 4595
	public float time;

	// Token: 0x040011F4 RID: 4596
	private float timer;

	// Token: 0x040011F5 RID: 4597
	private bool deparented;
}
