using System;
using UnityEngine;

namespace GlobalSettings
{
	// Token: 0x020008C8 RID: 2248
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Demo Settings")]
	public class Demo : GlobalSettingsBase<Demo>
	{
		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06004DD6 RID: 19926 RVA: 0x0016CDC6 File Offset: 0x0016AFC6
		public static int MaxDeathCount
		{
			get
			{
				return Demo.Get().maxDeathCount;
			}
		}

		// Token: 0x06004DD7 RID: 19927 RVA: 0x0016CDD2 File Offset: 0x0016AFD2
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Demo>.StartPreloadAddressable("Global Demo Settings");
		}

		// Token: 0x06004DD8 RID: 19928 RVA: 0x0016CDDE File Offset: 0x0016AFDE
		public static void Unload()
		{
			GlobalSettingsBase<Demo>.StartUnload();
		}

		// Token: 0x06004DD9 RID: 19929 RVA: 0x0016CDE5 File Offset: 0x0016AFE5
		private static Demo Get()
		{
			return GlobalSettingsBase<Demo>.Get("Global Demo Settings");
		}

		// Token: 0x06004DDA RID: 19930 RVA: 0x0016CDF4 File Offset: 0x0016AFF4
		private void OnValidate()
		{
			if (this.saveFileOverrides.Length != 4)
			{
				Demo.SaveFileData[] array = new Demo.SaveFileData[4];
				for (int i = 0; i < Mathf.Min(array.Length, this.saveFileOverrides.Length); i++)
				{
					array[i] = this.saveFileOverrides[i];
				}
				this.saveFileOverrides = array;
			}
		}

		// Token: 0x06004DDB RID: 19931 RVA: 0x0016CE48 File Offset: 0x0016B048
		public static Demo.SaveFileData GetSaveFileOverride(int index)
		{
			Demo demo = Demo.Get();
			if (index < 0 || demo.saveFileOverrides == null || index >= demo.saveFileOverrides.Length)
			{
				return default(Demo.SaveFileData);
			}
			return demo.saveFileOverrides[index];
		}

		// Token: 0x04004E95 RID: 20117
		[SerializeField]
		private Demo.SaveFileData[] saveFileOverrides;

		// Token: 0x04004E96 RID: 20118
		[SerializeField]
		private int maxDeathCount;

		// Token: 0x02001B51 RID: 6993
		[Serializable]
		public struct SaveFileData
		{
			// Token: 0x04009C2F RID: 39983
			[Tooltip("Insert an UNENCRYPTED .txt save file.")]
			public TextAsset SaveFile;

			// Token: 0x04009C30 RID: 39984
			[Tooltip("Is this save file for display purposes only? Will run regular game start when selected.")]
			public bool IsDummySave;
		}
	}
}
