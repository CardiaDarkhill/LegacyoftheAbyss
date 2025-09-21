using System;
using UnityEngine;

namespace GlobalSettings
{
	// Token: 0x020008C7 RID: 2247
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Corpse Settings")]
	public class Corpse : GlobalSettingsBase<Corpse>
	{
		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x06004DC3 RID: 19907 RVA: 0x0016CCEB File Offset: 0x0016AEEB
		public static Color LandTint
		{
			get
			{
				return Corpse.Get().landTint;
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x06004DC4 RID: 19908 RVA: 0x0016CCF7 File Offset: 0x0016AEF7
		public static float LandTintFadeTime
		{
			get
			{
				return Corpse.Get().landTintFadeTime;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x06004DC5 RID: 19909 RVA: 0x0016CD03 File Offset: 0x0016AF03
		public static float LandTintWaitTime
		{
			get
			{
				return Corpse.Get().landTintWaitTime;
			}
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x06004DC6 RID: 19910 RVA: 0x0016CD0F File Offset: 0x0016AF0F
		public static AnimationCurve LandTintCurve
		{
			get
			{
				return Corpse.Get().landTintCurve;
			}
		}

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x06004DC7 RID: 19911 RVA: 0x0016CD1B File Offset: 0x0016AF1B
		public static AnimationCurve LandDesaturationCurve
		{
			get
			{
				return Corpse.Get().landDesaturationCurve;
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x06004DC8 RID: 19912 RVA: 0x0016CD27 File Offset: 0x0016AF27
		public static Color SpellBurnColor
		{
			get
			{
				return Corpse.Get().spellBurnColor;
			}
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06004DC9 RID: 19913 RVA: 0x0016CD33 File Offset: 0x0016AF33
		public static Color SpellBurnColorBlackThread
		{
			get
			{
				return Corpse.Get().spellBurnColorBlackThread;
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06004DCA RID: 19914 RVA: 0x0016CD3F File Offset: 0x0016AF3F
		public static ParticleEffectsLerpEmission SpellBurnEffect
		{
			get
			{
				return Corpse.Get().spellBurnEffect;
			}
		}

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06004DCB RID: 19915 RVA: 0x0016CD4B File Offset: 0x0016AF4B
		public static float SpellBurnDuration
		{
			get
			{
				return Corpse.Get().spellBurnDuration;
			}
		}

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06004DCC RID: 19916 RVA: 0x0016CD57 File Offset: 0x0016AF57
		public static ParticleEffectsLerpEmission FireBurnEffect
		{
			get
			{
				return Corpse.Get().fireBurnEffect;
			}
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06004DCD RID: 19917 RVA: 0x0016CD63 File Offset: 0x0016AF63
		public static ParticleEffectsLerpEmission PoisonBurnEffect
		{
			get
			{
				return Corpse.Get().poisonBurnEffect;
			}
		}

		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06004DCE RID: 19918 RVA: 0x0016CD6F File Offset: 0x0016AF6F
		public static ParticleEffectsLerpEmission SoulBurnEffect
		{
			get
			{
				return Corpse.Get().soulBurnEffect;
			}
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06004DCF RID: 19919 RVA: 0x0016CD7B File Offset: 0x0016AF7B
		public static ParticleEffectsLerpEmission ZapBurnEffect
		{
			get
			{
				return Corpse.Get().zapBurnEffect;
			}
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06004DD0 RID: 19920 RVA: 0x0016CD87 File Offset: 0x0016AF87
		public static GameObject EnemyLavaDeath
		{
			get
			{
				return Corpse.Get().enemyLavaDeath;
			}
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06004DD1 RID: 19921 RVA: 0x0016CD93 File Offset: 0x0016AF93
		public static float MinCorpseFlingMagnitudeMult
		{
			get
			{
				return Corpse.Get().minCorpseFlingMagnitudeMult;
			}
		}

		// Token: 0x06004DD2 RID: 19922 RVA: 0x0016CD9F File Offset: 0x0016AF9F
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Corpse>.StartPreloadAddressable("Global Corpse Settings");
		}

		// Token: 0x06004DD3 RID: 19923 RVA: 0x0016CDAB File Offset: 0x0016AFAB
		public static void Unload()
		{
			GlobalSettingsBase<Corpse>.StartUnload();
		}

		// Token: 0x06004DD4 RID: 19924 RVA: 0x0016CDB2 File Offset: 0x0016AFB2
		private static Corpse Get()
		{
			return GlobalSettingsBase<Corpse>.Get("Global Corpse Settings");
		}

		// Token: 0x04004E86 RID: 20102
		[SerializeField]
		private Color landTint;

		// Token: 0x04004E87 RID: 20103
		[SerializeField]
		private float landTintFadeTime;

		// Token: 0x04004E88 RID: 20104
		[SerializeField]
		private float landTintWaitTime;

		// Token: 0x04004E89 RID: 20105
		[SerializeField]
		private AnimationCurve landTintCurve;

		// Token: 0x04004E8A RID: 20106
		[SerializeField]
		private AnimationCurve landDesaturationCurve;

		// Token: 0x04004E8B RID: 20107
		[Space]
		[SerializeField]
		private Color spellBurnColor;

		// Token: 0x04004E8C RID: 20108
		[SerializeField]
		private Color spellBurnColorBlackThread;

		// Token: 0x04004E8D RID: 20109
		[SerializeField]
		private ParticleEffectsLerpEmission spellBurnEffect;

		// Token: 0x04004E8E RID: 20110
		[SerializeField]
		private float spellBurnDuration;

		// Token: 0x04004E8F RID: 20111
		[SerializeField]
		private ParticleEffectsLerpEmission fireBurnEffect;

		// Token: 0x04004E90 RID: 20112
		[SerializeField]
		private ParticleEffectsLerpEmission poisonBurnEffect;

		// Token: 0x04004E91 RID: 20113
		[SerializeField]
		private ParticleEffectsLerpEmission soulBurnEffect;

		// Token: 0x04004E92 RID: 20114
		[SerializeField]
		private ParticleEffectsLerpEmission zapBurnEffect;

		// Token: 0x04004E93 RID: 20115
		[Header("Death")]
		[SerializeField]
		private GameObject enemyLavaDeath;

		// Token: 0x04004E94 RID: 20116
		[SerializeField]
		private float minCorpseFlingMagnitudeMult;
	}
}
