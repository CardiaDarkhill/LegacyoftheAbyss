using System;

// Token: 0x020007AB RID: 1963
public abstract class VibrationMixer
{
	// Token: 0x170007BF RID: 1983
	// (get) Token: 0x06004562 RID: 17762
	// (set) Token: 0x06004563 RID: 17763
	public abstract bool IsPaused { get; set; }

	// Token: 0x06004564 RID: 17764
	public abstract VibrationEmission PlayEmission(VibrationData vibrationData, VibrationTarget vibrationTarget, bool isLooping, string tag, bool isRealtime);

	// Token: 0x06004565 RID: 17765 RVA: 0x0012EF19 File Offset: 0x0012D119
	public virtual VibrationEmission PlayEmission(VibrationEmission emission)
	{
		return emission;
	}

	// Token: 0x170007C0 RID: 1984
	// (get) Token: 0x06004566 RID: 17766
	public abstract int PlayingEmissionCount { get; }

	// Token: 0x06004567 RID: 17767
	public abstract VibrationEmission GetPlayingEmission(int playingEmissionIndex);

	// Token: 0x06004568 RID: 17768
	public abstract void StopAllEmissions();

	// Token: 0x06004569 RID: 17769
	public abstract void StopAllEmissionsWithTag(string tag);
}
