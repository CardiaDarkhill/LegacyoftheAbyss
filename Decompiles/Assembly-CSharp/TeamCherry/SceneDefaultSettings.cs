using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

namespace TeamCherry
{
	// Token: 0x0200089E RID: 2206
	[Serializable]
	public class SceneDefaultSettings : ScriptableObject
	{
		// Token: 0x06004C4B RID: 19531 RVA: 0x00167759 File Offset: 0x00165959
		public void OnEnable()
		{
			if (this.settingsList == null)
			{
				this.settingsList = new List<SceneManagerSettings>();
			}
		}

		// Token: 0x06004C4C RID: 19532 RVA: 0x00167770 File Offset: 0x00165970
		public SceneManagerSettings GetMapZoneSettingsRuntime(MapZone mapZone, SceneManagerSettings.Conditions condition)
		{
			SceneManagerSettings result = null;
			foreach (SceneManagerSettings sceneManagerSettings in this.settingsList)
			{
				if (sceneManagerSettings.mapZone == mapZone)
				{
					result = sceneManagerSettings;
					if (sceneManagerSettings.condition >= condition)
					{
						return sceneManagerSettings;
					}
				}
			}
			return result;
		}

		// Token: 0x06004C4D RID: 19533 RVA: 0x001677DC File Offset: 0x001659DC
		public SceneManagerSettings GetMapZoneSettingsEdit(MapZone mapZone, SceneManagerSettings.Conditions condition)
		{
			foreach (SceneManagerSettings sceneManagerSettings in this.settingsList)
			{
				if (sceneManagerSettings.mapZone == mapZone && sceneManagerSettings.condition == condition)
				{
					return sceneManagerSettings;
				}
			}
			return null;
		}

		// Token: 0x06004C4E RID: 19534 RVA: 0x00167844 File Offset: 0x00165A44
		public void SaveSettings(SceneManagerSettings sms)
		{
			SceneManagerSettings sceneManagerSettings = null;
			foreach (SceneManagerSettings sceneManagerSettings2 in this.settingsList)
			{
				if (sceneManagerSettings2.mapZone == sms.mapZone && sceneManagerSettings2.condition == sms.condition)
				{
					sceneManagerSettings = sceneManagerSettings2;
				}
			}
			if (sceneManagerSettings != null)
			{
				sceneManagerSettings.defaultColor = new Color(sms.defaultColor.r, sms.defaultColor.g, sms.defaultColor.b, sms.defaultColor.a);
				sceneManagerSettings.defaultIntensity = sms.defaultIntensity;
				sceneManagerSettings.saturation = sms.saturation;
				sceneManagerSettings.redChannel = new AnimationCurve(sms.redChannel.keys.Clone() as Keyframe[]);
				sceneManagerSettings.greenChannel = new AnimationCurve(sms.greenChannel.keys.Clone() as Keyframe[]);
				sceneManagerSettings.blueChannel = new AnimationCurve(sms.blueChannel.keys.Clone() as Keyframe[]);
				sceneManagerSettings.heroLightColor = new Color(sms.heroLightColor.r, sms.heroLightColor.g, sms.heroLightColor.b, sms.heroLightColor.a);
				sceneManagerSettings.blurPlaneVibranceOffset = sms.blurPlaneVibranceOffset;
				sceneManagerSettings.heroSaturationOffset = sms.heroSaturationOffset;
				return;
			}
			this.settingsList.Add(new SceneManagerSettings(sms.mapZone, sms.condition, new Color(sms.defaultColor.r, sms.defaultColor.g, sms.defaultColor.b), sms.defaultIntensity, sms.saturation, new AnimationCurve(sms.redChannel.keys.Clone() as Keyframe[]), new AnimationCurve(sms.greenChannel.keys.Clone() as Keyframe[]), new AnimationCurve(sms.blueChannel.keys.Clone() as Keyframe[]), new Color(sms.heroLightColor.r, sms.heroLightColor.g, sms.heroLightColor.b, sms.heroLightColor.a), sms.blurPlaneVibranceOffset, sms.heroSaturationOffset));
		}

		// Token: 0x04004DA4 RID: 19876
		[SerializeField]
		public List<SceneManagerSettings> settingsList;
	}
}
