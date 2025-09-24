using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GlobalSettings
{
	// Token: 0x020008C9 RID: 2249
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Effects Settings")]
	public class Effects : GlobalSettingsBase<Effects>
	{
		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x06004DDD RID: 19933 RVA: 0x0016CE90 File Offset: 0x0016B090
		public static GameObject BloodParticlePrefab
		{
			get
			{
				return Effects.Get().bloodParticlePrefab;
			}
		}

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x06004DDE RID: 19934 RVA: 0x0016CE9C File Offset: 0x0016B09C
		public static GameObject RageHitEffectPrefab
		{
			get
			{
				return Effects.Get().rageHitEffectPrefab;
			}
		}

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x06004DDF RID: 19935 RVA: 0x0016CEA8 File Offset: 0x0016B0A8
		public static GameObject RageHitHealthEffectPrefab
		{
			get
			{
				return Effects.Get().rageHitHealthEffectPrefab;
			}
		}

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06004DE0 RID: 19936 RVA: 0x0016CEB4 File Offset: 0x0016B0B4
		public static AudioEvent EnemyCoalHurtSound
		{
			get
			{
				return Effects.Get().enemyCoalHurtSound;
			}
		}

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06004DE1 RID: 19937 RVA: 0x0016CEC0 File Offset: 0x0016B0C0
		public static GameObject WeakHitEffectPrefab
		{
			get
			{
				return Effects.Get().weakHitEffectPrefab;
			}
		}

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06004DE2 RID: 19938 RVA: 0x0016CECC File Offset: 0x0016B0CC
		public static CameraShakeTarget WeakHitEffectShake
		{
			get
			{
				return Effects.Get().weakHitEffectShake;
			}
		}

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06004DE3 RID: 19939 RVA: 0x0016CED8 File Offset: 0x0016B0D8
		public static GameObject EnemyWitchPoisonHitEffectPrefab
		{
			get
			{
				return Effects.Get().enemyWitchPoisonHitEffectPrefab;
			}
		}

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06004DE4 RID: 19940 RVA: 0x0016CEE4 File Offset: 0x0016B0E4
		public static GameObject EnemyWitchPoisonHurtEffectPrefab
		{
			get
			{
				return Effects.Get().enemyWitchPoisonHurtEffectPrefab;
			}
		}

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06004DE5 RID: 19941 RVA: 0x0016CEF0 File Offset: 0x0016B0F0
		public static BloodSpawner.GeneralConfig EnemyWitchPoisonBloodBurst
		{
			get
			{
				return Effects.Get().enemyWitchPoisonBloodBurst;
			}
		}

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06004DE6 RID: 19942 RVA: 0x0016CEFC File Offset: 0x0016B0FC
		public static EnemyHitEffectsProfile LightningHitEffects
		{
			get
			{
				return Effects.Get().lightningHitEffects;
			}
		}

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06004DE7 RID: 19943 RVA: 0x0016CF08 File Offset: 0x0016B108
		public static GameObject ReapHitEffectPrefab
		{
			get
			{
				return Effects.Get().reapHitEffectPrefab;
			}
		}

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x06004DE8 RID: 19944 RVA: 0x0016CF14 File Offset: 0x0016B114
		public static GameObject SpikeSlashEffectPrefab
		{
			get
			{
				return Effects.Get().spikeSlashEffectPrefab;
			}
		}

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x06004DE9 RID: 19945 RVA: 0x0016CF20 File Offset: 0x0016B120
		public static NailImbuementConfig FireNail
		{
			get
			{
				return Effects.Get().fireNail;
			}
		}

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x06004DEA RID: 19946 RVA: 0x0016CF2C File Offset: 0x0016B12C
		public static CameraShakeTarget NailClashTinkShake
		{
			get
			{
				return Effects.Get().nailClashTinkShake;
			}
		}

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x06004DEB RID: 19947 RVA: 0x0016CF38 File Offset: 0x0016B138
		public static AudioEvent NailClashParrySound
		{
			get
			{
				return Effects.Get().nailClashParrySound;
			}
		}

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x06004DEC RID: 19948 RVA: 0x0016CF44 File Offset: 0x0016B144
		public static GameObject NailClashParryEffect
		{
			get
			{
				return Effects.Get().nailClashParryEffect;
			}
		}

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06004DED RID: 19949 RVA: 0x0016CF50 File Offset: 0x0016B150
		public static GameObject NailClashParryEffectSmall
		{
			get
			{
				return Effects.Get().nailClashParryEffectSmall;
			}
		}

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x06004DEE RID: 19950 RVA: 0x0016CF5C File Offset: 0x0016B15C
		public static GameObject EnemyNailTerrainThunk
		{
			get
			{
				return Effects.Get().enemyNailTerrainThunk;
			}
		}

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x06004DEF RID: 19951 RVA: 0x0016CF68 File Offset: 0x0016B168
		public static GameObject TinkEffectDullPrefab
		{
			get
			{
				return Effects.Get().tinkEffectDullPrefab;
			}
		}

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x06004DF0 RID: 19952 RVA: 0x0016CF74 File Offset: 0x0016B174
		public static CameraShakeTarget BlockedHitShake
		{
			get
			{
				return Effects.Get().blockedHitShake;
			}
		}

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x06004DF1 RID: 19953 RVA: 0x0016CF80 File Offset: 0x0016B180
		public static CameraShakeTarget BlockedHitShakeNoFreeze
		{
			get
			{
				return Effects.Get().blockedHitShakeNoFreeze;
			}
		}

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x06004DF2 RID: 19954 RVA: 0x0016CF8C File Offset: 0x0016B18C
		public static GameObject SilkPossesionObjSing
		{
			get
			{
				return Effects.Get().silkPossesionObjSing;
			}
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06004DF3 RID: 19955 RVA: 0x0016CF98 File Offset: 0x0016B198
		public static GameObject SilkPossesionObjSingNoPuppet
		{
			get
			{
				return Effects.Get().silkPossesionObjSingNoPuppet;
			}
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06004DF4 RID: 19956 RVA: 0x0016CFA4 File Offset: 0x0016B1A4
		public static GameObject SilkPossesionObjSingEnd
		{
			get
			{
				return Effects.Get().silkPossesionObjSingEnd;
			}
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06004DF5 RID: 19957 RVA: 0x0016CFB0 File Offset: 0x0016B1B0
		public static Color LifebloodTintColour
		{
			get
			{
				return Effects.Get().lifebloodTintColour;
			}
		}

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x06004DF6 RID: 19958 RVA: 0x0016CFBC File Offset: 0x0016B1BC
		public static GameObject LifebloodEffectPrefab
		{
			get
			{
				return Effects.Get().lifebloodEffectPrefab;
			}
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x06004DF7 RID: 19959 RVA: 0x0016CFC8 File Offset: 0x0016B1C8
		public static GameObject LifebloodHealEffect
		{
			get
			{
				return Effects.Get().lifebloodHealEffect;
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x06004DF8 RID: 19960 RVA: 0x0016CFD4 File Offset: 0x0016B1D4
		public static LifebloodGlob LifebloodGlob
		{
			get
			{
				return Effects.Get().lifebloodGlob;
			}
		}

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x06004DF9 RID: 19961 RVA: 0x0016CFE0 File Offset: 0x0016B1E0
		public static Quest LifeBloodQuest
		{
			get
			{
				return Effects.Get().lifeBloodQuest;
			}
		}

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x06004DFA RID: 19962 RVA: 0x0016CFEC File Offset: 0x0016B1EC
		public static GameObject EnemyPhysicalPusher
		{
			get
			{
				return Effects.Get().enemyPhysicalPusher;
			}
		}

		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x06004DFB RID: 19963 RVA: 0x0016CFF8 File Offset: 0x0016B1F8
		public static RandomAudioClipTable EnemyDamageTickSoundTable
		{
			get
			{
				return Effects.Get().enemyDamageTickSoundTable;
			}
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x06004DFC RID: 19964 RVA: 0x0016D004 File Offset: 0x0016B204
		public static GameObject BlackThreadEnemyStartEffect
		{
			get
			{
				return Effects.Get().blackThreadEnemyStartEffect;
			}
		}

		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06004DFD RID: 19965 RVA: 0x0016D010 File Offset: 0x0016B210
		public static GameObject BlackThreadEnemyEffect
		{
			get
			{
				return Effects.Get().blackThreadEnemyEffect;
			}
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06004DFE RID: 19966 RVA: 0x0016D01C File Offset: 0x0016B21C
		public static GameObject BlackThreadEnemyDeathEffect
		{
			get
			{
				return Effects.Get().blackThreadEnemyDeathEffect;
			}
		}

		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06004DFF RID: 19967 RVA: 0x0016D028 File Offset: 0x0016B228
		public static AnimationCurve BlackThreadEnemyAttackTintCurve
		{
			get
			{
				return Effects.Get().blackThreadEnemyAttackTintCurve;
			}
		}

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x06004E00 RID: 19968 RVA: 0x0016D034 File Offset: 0x0016B234
		public static float BlackThreadEnemyAttackTintDuration
		{
			get
			{
				return Effects.Get().blackThreadEnemyAttackTintDuration;
			}
		}

		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x06004E01 RID: 19969 RVA: 0x0016D040 File Offset: 0x0016B240
		public static AnimationCurve BlackThreadEnemyPulseTintCurve
		{
			get
			{
				return Effects.Get().blackThreadEnemyPulseTintCurve;
			}
		}

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x06004E02 RID: 19970 RVA: 0x0016D04C File Offset: 0x0016B24C
		public static float BlackThreadEnemyPulseTintDuration
		{
			get
			{
				return Effects.Get().blackThreadEnemyPulseTintDuration;
			}
		}

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x06004E03 RID: 19971 RVA: 0x0016D058 File Offset: 0x0016B258
		public static GameObject BlackThreadPooledEffect
		{
			get
			{
				return Effects.Get().blackThreadPooledEffect;
			}
		}

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x06004E04 RID: 19972 RVA: 0x0016D064 File Offset: 0x0016B264
		public static BlackThreadAttack[] BlackThreadAttacksDefault
		{
			get
			{
				return Effects.Get().blackThreadAttacksDefault;
			}
		}

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x06004E05 RID: 19973 RVA: 0x0016D070 File Offset: 0x0016B270
		public static AudioMixerGroup BlackThreadVoiceMixerGroup
		{
			get
			{
				return Effects.Get().blackThreadVoiceMixerGroup;
			}
		}

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x06004E06 RID: 19974 RVA: 0x0016D07C File Offset: 0x0016B27C
		public static Material CutoutSpriteMaterial
		{
			get
			{
				return Effects.Get().cutoutSpriteMaterial;
			}
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x06004E07 RID: 19975 RVA: 0x0016D088 File Offset: 0x0016B288
		public static Color MossEffectsTintDust
		{
			get
			{
				return Effects.Get().mossEffectsTintDust;
			}
		}

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x06004E08 RID: 19976 RVA: 0x0016D094 File Offset: 0x0016B294
		public static Material DefaultLitMaterial
		{
			get
			{
				return Effects.Get().defaultLitMaterial;
			}
		}

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x06004E09 RID: 19977 RVA: 0x0016D0A0 File Offset: 0x0016B2A0
		public static Material DefaultUnlitMaterial
		{
			get
			{
				return Effects.Get().defaultUnlitMaterial;
			}
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x06004E0A RID: 19978 RVA: 0x0016D0AC File Offset: 0x0016B2AC
		public static AnimationCurve GlowResponsePulseCurve
		{
			get
			{
				return Effects.Get().glowResponsePulseCurve;
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x06004E0B RID: 19979 RVA: 0x0016D0B8 File Offset: 0x0016B2B8
		public static float GlowResponsePulseDuration
		{
			get
			{
				return Effects.Get().glowResponsePulseDuration;
			}
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x06004E0C RID: 19980 RVA: 0x0016D0C4 File Offset: 0x0016B2C4
		public static float GlowResponsePulseFrameRate
		{
			get
			{
				return Effects.Get().glowResponsePulseFrameRate;
			}
		}

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x06004E0D RID: 19981 RVA: 0x0016D0D0 File Offset: 0x0016B2D0
		public static AudioEvent BeginMaggotedSound
		{
			get
			{
				return Effects.Get().beginMaggotedSound;
			}
		}

		// Token: 0x06004E0E RID: 19982 RVA: 0x0016D0DC File Offset: 0x0016B2DC
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Effects>.StartPreloadAddressable("Global Effects Settings");
		}

		// Token: 0x06004E0F RID: 19983 RVA: 0x0016D0E8 File Offset: 0x0016B2E8
		public static void Unload()
		{
			GlobalSettingsBase<Effects>.StartUnload();
		}

		// Token: 0x06004E10 RID: 19984 RVA: 0x0016D0EF File Offset: 0x0016B2EF
		private static Effects Get()
		{
			return GlobalSettingsBase<Effects>.Get("Global Effects Settings");
		}

		// Token: 0x06004E11 RID: 19985 RVA: 0x0016D0FC File Offset: 0x0016B2FC
		public static void DoBlackThreadHit(GameObject gameObject, HitInstance hitInstance, Vector3 effectOrigin)
		{
			Effects effects = Effects.Get();
			Vector3 position = gameObject.transform.TransformPoint(effectOrigin);
			effects.blackThreadHitSound.SpawnAndPlayOneShot(position, null);
			GameObject gameObject2 = (hitInstance.HitEffectsType == EnemyHitEffectsProfile.EffectsTypes.Full) ? effects.hitShade.Spawn(position) : effects.hitShadeMinimal.Spawn(position);
			Transform transform = gameObject2.transform;
			Vector3 eulerAngles;
			switch (DirectionUtils.GetCardinalDirection(hitInstance.Direction))
			{
			case 0:
				eulerAngles = new Vector3(0f, 90f, 0f);
				break;
			case 1:
				eulerAngles = new Vector3(-90f, 90f, 0f);
				break;
			case 2:
				eulerAngles = new Vector3(0f, -90f, 0f);
				break;
			case 3:
				eulerAngles = new Vector3(-90f, 90f, 0f);
				break;
			default:
				eulerAngles = gameObject2.transform.eulerAngles;
				break;
			}
			transform.eulerAngles = eulerAngles;
		}

		// Token: 0x04004E97 RID: 20119
		[Header("Hit Effects")]
		[SerializeField]
		private GameObject bloodParticlePrefab;

		// Token: 0x04004E98 RID: 20120
		[SerializeField]
		private GameObject rageHitEffectPrefab;

		// Token: 0x04004E99 RID: 20121
		[SerializeField]
		private GameObject rageHitHealthEffectPrefab;

		// Token: 0x04004E9A RID: 20122
		[SerializeField]
		private AudioEvent enemyCoalHurtSound;

		// Token: 0x04004E9B RID: 20123
		[SerializeField]
		private GameObject weakHitEffectPrefab;

		// Token: 0x04004E9C RID: 20124
		[SerializeField]
		private CameraShakeTarget weakHitEffectShake;

		// Token: 0x04004E9D RID: 20125
		[Space]
		[SerializeField]
		private GameObject enemyWitchPoisonHitEffectPrefab;

		// Token: 0x04004E9E RID: 20126
		[SerializeField]
		private GameObject enemyWitchPoisonHurtEffectPrefab;

		// Token: 0x04004E9F RID: 20127
		[SerializeField]
		private BloodSpawner.GeneralConfig enemyWitchPoisonBloodBurst;

		// Token: 0x04004EA0 RID: 20128
		[Space]
		[SerializeField]
		private EnemyHitEffectsProfile lightningHitEffects;

		// Token: 0x04004EA1 RID: 20129
		[Space]
		[SerializeField]
		private GameObject reapHitEffectPrefab;

		// Token: 0x04004EA2 RID: 20130
		[Space]
		[SerializeField]
		private GameObject spikeSlashEffectPrefab;

		// Token: 0x04004EA3 RID: 20131
		[Space]
		[SerializeField]
		private NailImbuementConfig fireNail;

		// Token: 0x04004EA4 RID: 20132
		[Header("Nail Clash Tink")]
		[SerializeField]
		private CameraShakeTarget nailClashTinkShake;

		// Token: 0x04004EA5 RID: 20133
		[SerializeField]
		private AudioEvent nailClashParrySound;

		// Token: 0x04004EA6 RID: 20134
		[SerializeField]
		private GameObject nailClashParryEffect;

		// Token: 0x04004EA7 RID: 20135
		[SerializeField]
		private GameObject nailClashParryEffectSmall;

		// Token: 0x04004EA8 RID: 20136
		[Space]
		[SerializeField]
		private GameObject enemyNailTerrainThunk;

		// Token: 0x04004EA9 RID: 20137
		[Space]
		[SerializeField]
		private GameObject tinkEffectDullPrefab;

		// Token: 0x04004EAA RID: 20138
		[Header("Camera Shakes")]
		[SerializeField]
		private CameraShakeTarget blockedHitShake;

		// Token: 0x04004EAB RID: 20139
		[SerializeField]
		private CameraShakeTarget blockedHitShakeNoFreeze;

		// Token: 0x04004EAC RID: 20140
		[Header("Enemies")]
		[SerializeField]
		private GameObject silkPossesionObjSing;

		// Token: 0x04004EAD RID: 20141
		[SerializeField]
		private GameObject silkPossesionObjSingNoPuppet;

		// Token: 0x04004EAE RID: 20142
		[SerializeField]
		private GameObject silkPossesionObjSingEnd;

		// Token: 0x04004EAF RID: 20143
		[Space]
		[SerializeField]
		private Color lifebloodTintColour;

		// Token: 0x04004EB0 RID: 20144
		[SerializeField]
		private GameObject lifebloodEffectPrefab;

		// Token: 0x04004EB1 RID: 20145
		[SerializeField]
		private GameObject lifebloodHealEffect;

		// Token: 0x04004EB2 RID: 20146
		[SerializeField]
		private LifebloodGlob lifebloodGlob;

		// Token: 0x04004EB3 RID: 20147
		[SerializeField]
		private Quest lifeBloodQuest;

		// Token: 0x04004EB4 RID: 20148
		[Space]
		[SerializeField]
		private GameObject enemyPhysicalPusher;

		// Token: 0x04004EB5 RID: 20149
		[SerializeField]
		private RandomAudioClipTable enemyDamageTickSoundTable;

		// Token: 0x04004EB6 RID: 20150
		[Header("Black Thread")]
		[SerializeField]
		private GameObject blackThreadEnemyStartEffect;

		// Token: 0x04004EB7 RID: 20151
		[SerializeField]
		private GameObject blackThreadEnemyEffect;

		// Token: 0x04004EB8 RID: 20152
		[SerializeField]
		private GameObject blackThreadEnemyDeathEffect;

		// Token: 0x04004EB9 RID: 20153
		[SerializeField]
		private AnimationCurve blackThreadEnemyAttackTintCurve;

		// Token: 0x04004EBA RID: 20154
		[SerializeField]
		private float blackThreadEnemyAttackTintDuration;

		// Token: 0x04004EBB RID: 20155
		[SerializeField]
		private AnimationCurve blackThreadEnemyPulseTintCurve;

		// Token: 0x04004EBC RID: 20156
		[SerializeField]
		private float blackThreadEnemyPulseTintDuration;

		// Token: 0x04004EBD RID: 20157
		[SerializeField]
		private GameObject blackThreadPooledEffect;

		// Token: 0x04004EBE RID: 20158
		[SerializeField]
		private AudioEvent blackThreadHitSound;

		// Token: 0x04004EBF RID: 20159
		[SerializeField]
		private GameObject hitFlashBlack;

		// Token: 0x04004EC0 RID: 20160
		[SerializeField]
		private GameObject hitShade;

		// Token: 0x04004EC1 RID: 20161
		[SerializeField]
		private GameObject hitShadeMinimal;

		// Token: 0x04004EC2 RID: 20162
		[SerializeField]
		private BlackThreadAttack[] blackThreadAttacksDefault;

		// Token: 0x04004EC3 RID: 20163
		[SerializeField]
		private AudioMixerGroup blackThreadVoiceMixerGroup;

		// Token: 0x04004EC4 RID: 20164
		[Header("Masking")]
		[SerializeField]
		private Material cutoutSpriteMaterial;

		// Token: 0x04004EC5 RID: 20165
		[Header("Environment")]
		[SerializeField]
		private Color mossEffectsTintDust;

		// Token: 0x04004EC6 RID: 20166
		[Space]
		[SerializeField]
		private Material defaultLitMaterial;

		// Token: 0x04004EC7 RID: 20167
		[SerializeField]
		private Material defaultUnlitMaterial;

		// Token: 0x04004EC8 RID: 20168
		[Header("Glow Response")]
		[SerializeField]
		private AnimationCurve glowResponsePulseCurve;

		// Token: 0x04004EC9 RID: 20169
		[SerializeField]
		private float glowResponsePulseDuration;

		// Token: 0x04004ECA RID: 20170
		[SerializeField]
		private float glowResponsePulseFrameRate;

		// Token: 0x04004ECB RID: 20171
		[Header("Maggots")]
		[SerializeField]
		private AudioEvent beginMaggotedSound;
	}
}
