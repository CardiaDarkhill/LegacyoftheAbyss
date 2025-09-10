using System;
using UnityEngine;

namespace GlobalSettings
{
	// Token: 0x020008C5 RID: 2245
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Audio Settings")]
	public class Audio : GlobalSettingsBase<Audio>
	{
		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06004DA7 RID: 19879 RVA: 0x0016CBA2 File Offset: 0x0016ADA2
		public static float AudioEventFrequencyLimit
		{
			get
			{
				return Audio.Get().audioEventFrequencyLimit;
			}
		}

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06004DA8 RID: 19880 RVA: 0x0016CBAE File Offset: 0x0016ADAE
		public static AudioSource DefaultAudioSourcePrefab
		{
			get
			{
				return Audio.Get().defaultAudioSourcePrefab;
			}
		}

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06004DA9 RID: 19881 RVA: 0x0016CBBA File Offset: 0x0016ADBA
		public static AudioSource Default2DAudioSourcePrefab
		{
			get
			{
				return Audio.Get().default2DAudioSourcePrefab;
			}
		}

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06004DAA RID: 19882 RVA: 0x0016CBC6 File Offset: 0x0016ADC6
		public static AudioSource DefaultUIAudioSourcePrefab
		{
			get
			{
				return Audio.Get().defaultUIAudioSourcePrefab;
			}
		}

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06004DAB RID: 19883 RVA: 0x0016CBD2 File Offset: 0x0016ADD2
		public static AudioEvent InventorySelectionMoveSound
		{
			get
			{
				return Audio.Get().inventorySelectionMoveSound;
			}
		}

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06004DAC RID: 19884 RVA: 0x0016CBDE File Offset: 0x0016ADDE
		public static AudioEvent StopConfirmSound
		{
			get
			{
				return Audio.Get().stopConfirmSound;
			}
		}

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06004DAD RID: 19885 RVA: 0x0016CBEA File Offset: 0x0016ADEA
		public static RandomAudioClipTable CorpseSpikeLandAudioTable
		{
			get
			{
				return Audio.Get().corpseSpikeLandAudioTable;
			}
		}

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06004DAE RID: 19886 RVA: 0x0016CBF6 File Offset: 0x0016ADF6
		public static RandomAudioClipTable ObjectSpikeLandAudioTable
		{
			get
			{
				return Audio.Get().objectSpikeLandAudioTable;
			}
		}

		// Token: 0x06004DAF RID: 19887 RVA: 0x0016CC02 File Offset: 0x0016AE02
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Audio>.StartPreloadAddressable("Global Audio Settings");
		}

		// Token: 0x06004DB0 RID: 19888 RVA: 0x0016CC0E File Offset: 0x0016AE0E
		public static void Unload()
		{
			GlobalSettingsBase<Audio>.StartUnload();
		}

		// Token: 0x06004DB1 RID: 19889 RVA: 0x0016CC15 File Offset: 0x0016AE15
		private static Audio Get()
		{
			return GlobalSettingsBase<Audio>.Get("Global Audio Settings");
		}

		// Token: 0x04004E72 RID: 20082
		[SerializeField]
		private float audioEventFrequencyLimit = 0.02f;

		// Token: 0x04004E73 RID: 20083
		[SerializeField]
		private AudioSource defaultAudioSourcePrefab;

		// Token: 0x04004E74 RID: 20084
		[SerializeField]
		private AudioSource default2DAudioSourcePrefab;

		// Token: 0x04004E75 RID: 20085
		[SerializeField]
		private AudioSource defaultUIAudioSourcePrefab;

		// Token: 0x04004E76 RID: 20086
		[Space]
		[SerializeField]
		private AudioEvent inventorySelectionMoveSound;

		// Token: 0x04004E77 RID: 20087
		[SerializeField]
		private AudioEvent stopConfirmSound;

		// Token: 0x04004E78 RID: 20088
		[Space]
		[SerializeField]
		private RandomAudioClipTable corpseSpikeLandAudioTable;

		// Token: 0x04004E79 RID: 20089
		[SerializeField]
		private RandomAudioClipTable objectSpikeLandAudioTable;
	}
}
