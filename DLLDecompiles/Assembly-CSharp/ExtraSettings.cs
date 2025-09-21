using System;
using UnityEngine;

// Token: 0x020007B9 RID: 1977
public sealed class ExtraSettings : MonoBehaviour
{
	// Token: 0x060045BD RID: 17853 RVA: 0x0012F864 File Offset: 0x0012DA64
	private void Awake()
	{
		if (this.target == null)
		{
			this.target = base.gameObject;
		}
		IApplyExtraLoadSettings[] components = this.target.GetComponents<IApplyExtraLoadSettings>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].ApplyExtraLoadSettings += this.ApplyExtraLoadSettings;
		}
	}

	// Token: 0x060045BE RID: 17854 RVA: 0x0012F8B9 File Offset: 0x0012DAB9
	private void OnValidate()
	{
		if (this.target == null)
		{
			this.target = base.gameObject;
		}
	}

	// Token: 0x060045BF RID: 17855 RVA: 0x0012F8D5 File Offset: 0x0012DAD5
	private void ApplyExtraLoadSettings()
	{
		if (this.appliedSettings)
		{
			return;
		}
		this.ApplySettings(this.settings);
		this.appliedSettings = true;
	}

	// Token: 0x060045C0 RID: 17856 RVA: 0x0012F8F4 File Offset: 0x0012DAF4
	private void ApplySettings(ExtraSettings.ExtraSettingFlags flags)
	{
		HeroController instance = HeroController.instance;
		if ((flags & ExtraSettings.ExtraSettingFlags.SuppressNextLevelReadyRegainControl) == ExtraSettings.ExtraSettingFlags.SuppressNextLevelReadyRegainControl)
		{
			GameManager.SuppressRegainControl = true;
		}
		if (instance != null)
		{
			if ((flags & ExtraSettings.ExtraSettingFlags.RelinquishControl) == ExtraSettings.ExtraSettingFlags.RelinquishControl)
			{
				instance.RelinquishControl();
			}
			if ((flags & ExtraSettings.ExtraSettingFlags.StopPlayingAudio) == ExtraSettings.ExtraSettingFlags.StopPlayingAudio)
			{
				instance.StopPlayingAudio();
			}
			if ((flags & ExtraSettings.ExtraSettingFlags.StopForceWalkingSound) == ExtraSettings.ExtraSettingFlags.StopForceWalkingSound)
			{
				instance.ForceRunningSound = false;
				instance.ForceWalkingSound = false;
			}
			if (flags.HasFlag(ExtraSettings.ExtraSettingFlags.BlockFootstepAudio))
			{
				instance.SetBlockFootstepAudio(true);
			}
			if (flags.HasFlag(ExtraSettings.ExtraSettingFlags.HeroEnterWithoutInput))
			{
				instance.enterWithoutInput = true;
			}
			if (flags.HasFlag(ExtraSettings.ExtraSettingFlags.HeroSkipNormalEntry))
			{
				instance.skipNormalEntry = true;
			}
		}
	}

	// Token: 0x04004673 RID: 18035
	[SerializeField]
	private GameObject target;

	// Token: 0x04004674 RID: 18036
	[SerializeField]
	private ExtraSettings.ExtraSettingFlags settings;

	// Token: 0x04004675 RID: 18037
	private bool appliedSettings;

	// Token: 0x02001A8B RID: 6795
	[Flags]
	private enum ExtraSettingFlags
	{
		// Token: 0x040099DB RID: 39387
		None = 0,
		// Token: 0x040099DC RID: 39388
		RelinquishControl = 1,
		// Token: 0x040099DD RID: 39389
		StopPlayingAudio = 2,
		// Token: 0x040099DE RID: 39390
		StopForceWalkingSound = 4,
		// Token: 0x040099DF RID: 39391
		SuppressNextLevelReadyRegainControl = 8,
		// Token: 0x040099E0 RID: 39392
		BlockFootstepAudio = 16,
		// Token: 0x040099E1 RID: 39393
		HeroEnterWithoutInput = 32,
		// Token: 0x040099E2 RID: 39394
		HeroSkipNormalEntry = 64
	}
}
