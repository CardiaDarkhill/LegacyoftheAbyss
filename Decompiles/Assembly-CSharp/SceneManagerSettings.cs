using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020007CE RID: 1998
[Serializable]
public class SceneManagerSettings
{
	// Token: 0x06004669 RID: 18025 RVA: 0x0013162C File Offset: 0x0012F82C
	public SceneManagerSettings(MapZone mapZone, SceneManagerSettings.Conditions condition, Color defaultColor, float defaultIntensity, float saturation, AnimationCurve redChannel, AnimationCurve greenChannel, AnimationCurve blueChannel, Color heroLightColor, float blurPlaneVibranceOffset, float heroSaturationOffset)
	{
		this.mapZone = mapZone;
		this.condition = condition;
		this.defaultColor = defaultColor;
		this.defaultIntensity = defaultIntensity;
		this.saturation = saturation;
		this.redChannel = redChannel;
		this.greenChannel = greenChannel;
		this.blueChannel = blueChannel;
		this.heroLightColor = heroLightColor;
		this.blurPlaneVibranceOffset = blurPlaneVibranceOffset;
		this.heroSaturationOffset = heroSaturationOffset;
	}

	// Token: 0x0600466A RID: 18026 RVA: 0x0013169F File Offset: 0x0012F89F
	public SceneManagerSettings()
	{
	}

	// Token: 0x040046D2 RID: 18130
	public MapZone mapZone;

	// Token: 0x040046D3 RID: 18131
	public SceneManagerSettings.Conditions condition;

	// Token: 0x040046D4 RID: 18132
	public Color defaultColor;

	// Token: 0x040046D5 RID: 18133
	public float defaultIntensity;

	// Token: 0x040046D6 RID: 18134
	public float saturation;

	// Token: 0x040046D7 RID: 18135
	public AnimationCurve redChannel;

	// Token: 0x040046D8 RID: 18136
	public AnimationCurve greenChannel;

	// Token: 0x040046D9 RID: 18137
	public AnimationCurve blueChannel;

	// Token: 0x040046DA RID: 18138
	public Color heroLightColor;

	// Token: 0x040046DB RID: 18139
	public float blurPlaneVibranceOffset = 1f;

	// Token: 0x040046DC RID: 18140
	public float heroSaturationOffset;

	// Token: 0x02001AA1 RID: 6817
	public enum Conditions
	{
		// Token: 0x04009A07 RID: 39431
		None,
		// Token: 0x04009A08 RID: 39432
		BlackThread
	}
}
