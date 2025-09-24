using System;
using UnityEngine;

// Token: 0x020007A9 RID: 1961
public class VibrationEffect : MonoBehaviour
{
	// Token: 0x06004552 RID: 17746 RVA: 0x0012EBC7 File Offset: 0x0012CDC7
	protected void OnEnable()
	{
		VibrationManager.PlayVibrationClipOneShot(this.vibrationData, new VibrationTarget?(this.vibrationSource), false, "", false);
	}

	// Token: 0x0400461C RID: 17948
	[SerializeField]
	private VibrationData vibrationData;

	// Token: 0x0400461D RID: 17949
	[SerializeField]
	private VibrationTarget vibrationSource;
}
