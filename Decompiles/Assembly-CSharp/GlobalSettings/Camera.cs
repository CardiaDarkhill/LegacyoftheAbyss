using System;
using UnityEngine;

namespace GlobalSettings
{
	// Token: 0x020008C6 RID: 2246
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Camera Settings")]
	public class Camera : GlobalSettingsBase<Camera>
	{
		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06004DB3 RID: 19891 RVA: 0x0016CC34 File Offset: 0x0016AE34
		public static CameraManagerReference MainCameraShakeManager
		{
			get
			{
				return Camera.Get().mainCameraShakeManager;
			}
		}

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06004DB4 RID: 19892 RVA: 0x0016CC40 File Offset: 0x0016AE40
		public static CameraShakeProfile BigShake
		{
			get
			{
				return Camera.Get().bigShake;
			}
		}

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x06004DB5 RID: 19893 RVA: 0x0016CC4C File Offset: 0x0016AE4C
		public static CameraShakeProfile BigShakeQuick
		{
			get
			{
				return Camera.Get().bigShakeQuick;
			}
		}

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x06004DB6 RID: 19894 RVA: 0x0016CC58 File Offset: 0x0016AE58
		public static CameraShakeProfile TinyShake
		{
			get
			{
				return Camera.Get().tinyShake;
			}
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x06004DB7 RID: 19895 RVA: 0x0016CC64 File Offset: 0x0016AE64
		public static CameraShakeProfile SmallShake
		{
			get
			{
				return Camera.Get().smallShake;
			}
		}

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06004DB8 RID: 19896 RVA: 0x0016CC70 File Offset: 0x0016AE70
		public static CameraShakeProfile AverageShake
		{
			get
			{
				return Camera.Get().averageShake;
			}
		}

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06004DB9 RID: 19897 RVA: 0x0016CC7C File Offset: 0x0016AE7C
		public static CameraShakeProfile AverageShakeQuick
		{
			get
			{
				return Camera.Get().averageShakeQuick;
			}
		}

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06004DBA RID: 19898 RVA: 0x0016CC88 File Offset: 0x0016AE88
		public static CameraShakeProfile EnemyKillShake
		{
			get
			{
				return Camera.Get().enemyKillShake;
			}
		}

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06004DBB RID: 19899 RVA: 0x0016CC94 File Offset: 0x0016AE94
		public static CameraShakeProfile TinyRumble
		{
			get
			{
				return Camera.Get().tinyRumble;
			}
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06004DBC RID: 19900 RVA: 0x0016CCA0 File Offset: 0x0016AEA0
		public static CameraShakeProfile SmallRumble
		{
			get
			{
				return Camera.Get().smallRumble;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06004DBD RID: 19901 RVA: 0x0016CCAC File Offset: 0x0016AEAC
		public static CameraShakeProfile MedRumble
		{
			get
			{
				return Camera.Get().medRumble;
			}
		}

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06004DBE RID: 19902 RVA: 0x0016CCB8 File Offset: 0x0016AEB8
		public static CameraShakeProfile BigRumble
		{
			get
			{
				return Camera.Get().bigRumble;
			}
		}

		// Token: 0x06004DBF RID: 19903 RVA: 0x0016CCC4 File Offset: 0x0016AEC4
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Camera>.StartPreloadAddressable("Global Camera Settings");
		}

		// Token: 0x06004DC0 RID: 19904 RVA: 0x0016CCD0 File Offset: 0x0016AED0
		public static void Unload()
		{
			GlobalSettingsBase<Camera>.StartUnload();
		}

		// Token: 0x06004DC1 RID: 19905 RVA: 0x0016CCD7 File Offset: 0x0016AED7
		private static Camera Get()
		{
			return GlobalSettingsBase<Camera>.Get("Global Camera Settings");
		}

		// Token: 0x04004E7A RID: 20090
		[SerializeField]
		private CameraManagerReference mainCameraShakeManager;

		// Token: 0x04004E7B RID: 20091
		[Header("Legacy Shake Replacements")]
		[SerializeField]
		private CameraShakeProfile bigShake;

		// Token: 0x04004E7C RID: 20092
		[SerializeField]
		private CameraShakeProfile bigShakeQuick;

		// Token: 0x04004E7D RID: 20093
		[SerializeField]
		private CameraShakeProfile tinyShake;

		// Token: 0x04004E7E RID: 20094
		[SerializeField]
		private CameraShakeProfile smallShake;

		// Token: 0x04004E7F RID: 20095
		[SerializeField]
		private CameraShakeProfile averageShake;

		// Token: 0x04004E80 RID: 20096
		[SerializeField]
		private CameraShakeProfile averageShakeQuick;

		// Token: 0x04004E81 RID: 20097
		[SerializeField]
		private CameraShakeProfile enemyKillShake;

		// Token: 0x04004E82 RID: 20098
		[SerializeField]
		private CameraShakeProfile tinyRumble;

		// Token: 0x04004E83 RID: 20099
		[SerializeField]
		private CameraShakeProfile smallRumble;

		// Token: 0x04004E84 RID: 20100
		[SerializeField]
		private CameraShakeProfile medRumble;

		// Token: 0x04004E85 RID: 20101
		[SerializeField]
		private CameraShakeProfile bigRumble;
	}
}
