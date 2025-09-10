using System;
using System.Runtime.CompilerServices;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000408 RID: 1032
public sealed class ChangeScaleByLanguage : ChangeByLanguageOld<ChangeScaleByLanguage.ScaleOverride>
{
	// Token: 0x06002311 RID: 8977 RVA: 0x000A081C File Offset: 0x0009EA1C
	protected override void RecordOriginalValues()
	{
		if (this.recorded)
		{
			return;
		}
		this.recorded = true;
		this.originalScale = base.transform.localScale;
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000A083F File Offset: 0x0009EA3F
	protected override void DoRevertValues()
	{
		base.transform.localScale = this.originalScale;
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000A0852 File Offset: 0x0009EA52
	public override void ApplyHandHeld()
	{
		this.ApplySetting(this.handHeldOverrides);
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000A0860 File Offset: 0x0009EA60
	private void ApplySetting(ChangeScaleByLanguage.HandHeldOverrides setting)
	{
		Vector3 localScale = base.transform.localScale;
		if (setting.xScale.IsEnabled)
		{
			localScale.x = setting.xScale.Value;
		}
		if (setting.yScale.IsEnabled)
		{
			localScale.y = setting.yScale.Value;
		}
		if (setting.zScale.IsEnabled)
		{
			localScale.z = setting.zScale.Value;
		}
		base.transform.localScale = localScale;
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000A08E4 File Offset: 0x0009EAE4
	protected override void ApplySetting(ChangeScaleByLanguage.ScaleOverride setting)
	{
		ChangeScaleByLanguage.<>c__DisplayClass8_0 CS$<>8__locals1;
		CS$<>8__locals1.scale = base.transform.localScale;
		if (setting.xScale.IsEnabled)
		{
			CS$<>8__locals1.scale.x = setting.xScale.Value;
		}
		if (setting.yScale.IsEnabled)
		{
			CS$<>8__locals1.scale.y = setting.yScale.Value;
		}
		if (setting.zScale.IsEnabled)
		{
			CS$<>8__locals1.scale.z = setting.zScale.Value;
		}
		if (base.ShouldApplyHandHeld())
		{
			ChangeScaleByLanguage.<ApplySetting>g__ApplySetting|8_0(setting.handHeldOverrides, ref CS$<>8__locals1);
		}
		base.transform.localScale = CS$<>8__locals1.scale;
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000A09A8 File Offset: 0x0009EBA8
	[CompilerGenerated]
	internal static void <ApplySetting>g__ApplySetting|8_0(ChangeScaleByLanguage.HandHeldOverrides setting, ref ChangeScaleByLanguage.<>c__DisplayClass8_0 A_1)
	{
		if (setting.xScale.IsEnabled)
		{
			A_1.scale.x = setting.xScale.Value;
		}
		if (setting.yScale.IsEnabled)
		{
			A_1.scale.y = setting.yScale.Value;
		}
		if (setting.zScale.IsEnabled)
		{
			A_1.scale.z = setting.zScale.Value;
		}
	}

	// Token: 0x040021C6 RID: 8646
	[SerializeField]
	private ChangeScaleByLanguage.HandHeldOverrides handHeldOverrides = new ChangeScaleByLanguage.HandHeldOverrides();

	// Token: 0x040021C7 RID: 8647
	private Vector3 originalScale;

	// Token: 0x0200169D RID: 5789
	[Serializable]
	public sealed class ScaleOverride : ChangeByLanguageOld<ChangeScaleByLanguage.ScaleOverride>.LanguageOverride
	{
		// Token: 0x04008B87 RID: 35719
		public OverrideFloat xScale;

		// Token: 0x04008B88 RID: 35720
		public OverrideFloat yScale;

		// Token: 0x04008B89 RID: 35721
		public OverrideFloat zScale;

		// Token: 0x04008B8A RID: 35722
		public ChangeScaleByLanguage.HandHeldOverrides handHeldOverrides;
	}

	// Token: 0x0200169E RID: 5790
	[Serializable]
	public sealed class HandHeldOverrides
	{
		// Token: 0x04008B8B RID: 35723
		public OverrideFloat xScale;

		// Token: 0x04008B8C RID: 35724
		public OverrideFloat yScale;

		// Token: 0x04008B8D RID: 35725
		public OverrideFloat zScale;
	}
}
