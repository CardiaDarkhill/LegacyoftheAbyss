using System;
using UnityEngine;

// Token: 0x020007A5 RID: 1957
public sealed class HeroVibrationRegionSyncer : MonoBehaviour
{
	// Token: 0x0600452A RID: 17706 RVA: 0x0012E5CC File Offset: 0x0012C7CC
	private void Start()
	{
		if (this.heroVibrationRegion)
		{
			this.heroVibrationRegion.MainEmissionStarted += this.HeroVibrationRegionOnMainEmissionStarted;
		}
		else
		{
			Debug.LogError(string.Format("{0} is missing vibration region.", this));
		}
		if (this.audioSource == null)
		{
			Debug.LogError(string.Format("{0} is missing audio source. Emission Sync will fail.", this));
		}
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x0012E62D File Offset: 0x0012C82D
	private void OnValidate()
	{
		if (!this.heroVibrationRegion)
		{
			this.heroVibrationRegion = base.GetComponent<HeroVibrationRegion>();
		}
	}

	// Token: 0x0600452C RID: 17708 RVA: 0x0012E648 File Offset: 0x0012C848
	private void HeroVibrationRegionOnMainEmissionStarted(VibrationEmission emission)
	{
		AudioVibrationSyncer.StartSyncedEmission(emission, this.audioSource);
	}

	// Token: 0x04004607 RID: 17927
	[SerializeField]
	private HeroVibrationRegion heroVibrationRegion;

	// Token: 0x04004608 RID: 17928
	[SerializeField]
	private AudioSource audioSource;
}
