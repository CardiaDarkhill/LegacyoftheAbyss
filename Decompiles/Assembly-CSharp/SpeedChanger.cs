using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public abstract class SpeedChanger : MonoBehaviour
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x0600053B RID: 1339 RVA: 0x0001AB14 File Offset: 0x00018D14
	// (remove) Token: 0x0600053C RID: 1340 RVA: 0x0001AB4C File Offset: 0x00018D4C
	public event Action<float> SpeedChanged;

	// Token: 0x0600053D RID: 1341 RVA: 0x0001AB81 File Offset: 0x00018D81
	protected void CallSpeedChangedEvent(float speed)
	{
		if (this.SpeedChanged != null)
		{
			this.SpeedChanged(speed);
		}
	}
}
