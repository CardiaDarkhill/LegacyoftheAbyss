using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamCherry.PS5
{
	// Token: 0x020008A5 RID: 2213
	public static class PS5VibrationManager
	{
		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x06004C8E RID: 19598 RVA: 0x001683D8 File Offset: 0x001665D8
		private static Dictionary<AudioClip, PS5VibrationData> Vibrations
		{
			get
			{
				return PS5VibrationManager.vibrations;
			}
		}

		// Token: 0x06004C8F RID: 19599 RVA: 0x001683DF File Offset: 0x001665DF
		static PS5VibrationManager()
		{
			Debug.Log("Initialising Vibration Manager");
			PS5VibrationManager.vibrations = new Dictionary<AudioClip, PS5VibrationData>();
		}

		// Token: 0x06004C90 RID: 19600 RVA: 0x001683F8 File Offset: 0x001665F8
		public static AudioClip GetVibrationClip(AudioClip clip)
		{
			PS5VibrationData ps5VibrationData;
			if (PS5VibrationManager.Vibrations.TryGetValue(clip, out ps5VibrationData))
			{
				Debug.Log(string.Format("Found vibration data for {0} - {1}", clip, ps5VibrationData));
				if (ps5VibrationData.VibrationClip)
				{
					Debug.Log(string.Format("Found {0} vibration for {1} audio", ps5VibrationData.VibrationClip, clip), ps5VibrationData);
					return ps5VibrationData.VibrationClip;
				}
				Debug.LogError(string.Format("No vibration clip has been set for {0}", clip), clip);
			}
			else
			{
				Debug.LogError(string.Format("Failed to find vibration data for {0}", clip), clip);
			}
			return clip;
		}

		// Token: 0x04004DBC RID: 19900
		public const string VIBRATION_LABEL = "ps5 vibration";

		// Token: 0x04004DBD RID: 19901
		public const string MANAGER_ASSET_NAME = "PS5 Vibration Manager Asset";

		// Token: 0x04004DBE RID: 19902
		public const string PS5_VIBRATION_ASSET_PATH = "Assets/Audio/Vibration Files/PS5";

		// Token: 0x04004DBF RID: 19903
		private static readonly Dictionary<AudioClip, PS5VibrationData> vibrations;
	}
}
