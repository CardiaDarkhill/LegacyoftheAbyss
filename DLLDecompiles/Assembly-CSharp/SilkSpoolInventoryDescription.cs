using System;
using UnityEngine;

// Token: 0x020006BC RID: 1724
public class SilkSpoolInventoryDescription : MonoBehaviour
{
	// Token: 0x06003E8E RID: 16014 RVA: 0x00113A44 File Offset: 0x00111C44
	private void OnEnable()
	{
		int silkRegenMax = PlayerData.instance.silkRegenMax;
		if (silkRegenMax > 0)
		{
			if (this.silkHeartsGroup)
			{
				this.silkHeartsGroup.SetActive(true);
			}
			if (this.silkHeartsCounter)
			{
				this.silkHeartsCounter.MaxValue = Mathf.Max(3, silkRegenMax);
				this.silkHeartsCounter.CurrentValue = silkRegenMax;
				return;
			}
		}
		else if (this.silkHeartsGroup)
		{
			this.silkHeartsGroup.SetActive(false);
		}
	}

	// Token: 0x04004035 RID: 16437
	[SerializeField]
	private GameObject silkHeartsGroup;

	// Token: 0x04004036 RID: 16438
	[SerializeField]
	private IconCounter silkHeartsCounter;
}
