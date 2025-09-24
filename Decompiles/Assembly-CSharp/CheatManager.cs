using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class CheatManager : MonoBehaviour, IOnGUI
{
	// Token: 0x17000118 RID: 280
	// (get) Token: 0x06000BDD RID: 3037 RVA: 0x00035DA3 File Offset: 0x00033FA3
	public static float Multiplier
	{
		get
		{
			if ((float)Screen.height > 1080f)
			{
				return (float)Screen.height / 1080f * 1.25f + CheatManager.MultiplierChange;
			}
			return 1f + CheatManager.MultiplierChange;
		}
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000BDE RID: 3038 RVA: 0x00035DD6 File Offset: 0x00033FD6
	// (set) Token: 0x06000BDF RID: 3039 RVA: 0x00035DDD File Offset: 0x00033FDD
	public static float MultiplierChange { get; private set; }

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x00035DE5 File Offset: 0x00033FE5
	public static float ButtonHeight
	{
		get
		{
			return 20f * CheatManager.Multiplier;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x00035DF2 File Offset: 0x00033FF2
	public static float ButtonWidth
	{
		get
		{
			return Mathf.Max(350f, CheatManager.lastMaxWidth) * CheatManager.Multiplier;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000BE2 RID: 3042 RVA: 0x00035E09 File Offset: 0x00034009
	public static float SpaceHeight
	{
		get
		{
			return 5f * CheatManager.Multiplier;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x00035E16 File Offset: 0x00034016
	public static float XIndent
	{
		get
		{
			return 15f * CheatManager.Multiplier;
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x00035E23 File Offset: 0x00034023
	public static float YIndent
	{
		get
		{
			return 15f * CheatManager.Multiplier;
		}
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00035E30 File Offset: 0x00034030
	public static int FontSize
	{
		get
		{
			return Mathf.RoundToInt(12f * CheatManager.Multiplier);
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00035E42 File Offset: 0x00034042
	public static GUIStyle LabelStyle
	{
		get
		{
			if (CheatManager.advanceButtonStyle == null)
			{
				CheatManager.advanceButtonStyle = new GUIStyle(GUI.skin.label);
			}
			CheatManager.advanceButtonStyle.fontSize = CheatManager.FontSize;
			return CheatManager.advanceButtonStyle;
		}
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00035E73 File Offset: 0x00034073
	public static bool IsCheatsEnabled
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000BE8 RID: 3048 RVA: 0x00035E76 File Offset: 0x00034076
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		if (!CheatManager.IsCheatsEnabled)
		{
			return;
		}
		Object.DontDestroyOnLoad(new GameObject("CheatManager", new Type[]
		{
			typeof(CheatManager)
		}));
		PerformanceHud.Init();
	}

	// Token: 0x06000BE9 RID: 3049 RVA: 0x00035EA7 File Offset: 0x000340A7
	public static void ReInit()
	{
		if (CheatManager._instance)
		{
			Object.Destroy(CheatManager._instance.gameObject);
		}
		CheatManager.Init();
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000BEA RID: 3050 RVA: 0x00035EC9 File Offset: 0x000340C9
	// (set) Token: 0x06000BEB RID: 3051 RVA: 0x00035ED0 File Offset: 0x000340D0
	public static bool isQuickSilkEnabled { get; set; }

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000BEC RID: 3052 RVA: 0x00035ED8 File Offset: 0x000340D8
	// (set) Token: 0x06000BED RID: 3053 RVA: 0x00035EDF File Offset: 0x000340DF
	public static CheatManager.DamageSelfStates DamageSelfState { get; set; }

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000BEE RID: 3054 RVA: 0x00035EE7 File Offset: 0x000340E7
	// (set) Token: 0x06000BEF RID: 3055 RVA: 0x00035EEE File Offset: 0x000340EE
	public static HazardType HazardType { get; set; } = HazardType.NON_HAZARD;

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x00035EF6 File Offset: 0x000340F6
	// (set) Token: 0x06000BF1 RID: 3057 RVA: 0x00035EFD File Offset: 0x000340FD
	public static CheatManager.NailDamageStates NailDamage { get; set; }

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x00035F05 File Offset: 0x00034105
	// (set) Token: 0x06000BF3 RID: 3059 RVA: 0x00035F0C File Offset: 0x0003410C
	public static CheatManager.InvincibilityStates Invincibility { get; set; }

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x00035F14 File Offset: 0x00034114
	// (set) Token: 0x06000BF5 RID: 3061 RVA: 0x00035F1B File Offset: 0x0003411B
	public static bool ForceNextHitStun { get; set; }

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x00035F23 File Offset: 0x00034123
	// (set) Token: 0x06000BF7 RID: 3063 RVA: 0x00035F2A File Offset: 0x0003412A
	public static bool ForceStun
	{
		get
		{
			return CheatManager.forceStun;
		}
		set
		{
			CheatManager.forceStun = value;
		}
	}

	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x00035F32 File Offset: 0x00034132
	// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x00035F39 File Offset: 0x00034139
	public static bool IsTextPrintSkipEnabled { get; set; }

	// Token: 0x1700012A RID: 298
	// (get) Token: 0x06000BFA RID: 3066 RVA: 0x00035F41 File Offset: 0x00034141
	// (set) Token: 0x06000BFB RID: 3067 RVA: 0x00035F48 File Offset: 0x00034148
	public static bool IsFrostDisabled { get; set; }

	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06000BFC RID: 3068 RVA: 0x00035F50 File Offset: 0x00034150
	// (set) Token: 0x06000BFD RID: 3069 RVA: 0x00035F57 File Offset: 0x00034157
	public static bool CanChangeEquipsAnywhere { get; set; }

	// Token: 0x1700012C RID: 300
	// (get) Token: 0x06000BFE RID: 3070 RVA: 0x00035F5F File Offset: 0x0003415F
	public static bool AllowSaving
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700012D RID: 301
	// (get) Token: 0x06000BFF RID: 3071 RVA: 0x00035F62 File Offset: 0x00034162
	// (set) Token: 0x06000C00 RID: 3072 RVA: 0x00035F69 File Offset: 0x00034169
	public static bool UseFieldAccessOptimisers { get; set; } = true;

	// Token: 0x1700012E RID: 302
	// (get) Token: 0x06000C01 RID: 3073 RVA: 0x00035F71 File Offset: 0x00034171
	// (set) Token: 0x06000C02 RID: 3074 RVA: 0x00035F78 File Offset: 0x00034178
	public static bool ColliderCastForGroundCheck { get; set; } = false;

	// Token: 0x1700012F RID: 303
	// (get) Token: 0x06000C03 RID: 3075 RVA: 0x00035F80 File Offset: 0x00034180
	// (set) Token: 0x06000C04 RID: 3076 RVA: 0x00035F87 File Offset: 0x00034187
	public static bool DisableAsyncSceneLoad { get; set; }

	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000C05 RID: 3077 RVA: 0x00035F8F File Offset: 0x0003418F
	// (set) Token: 0x06000C06 RID: 3078 RVA: 0x00035F96 File Offset: 0x00034196
	public static bool UseAsyncSaveFileLoad { get; set; }

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000C07 RID: 3079 RVA: 0x00035F9E File Offset: 0x0003419E
	// (set) Token: 0x06000C08 RID: 3080 RVA: 0x00035FA5 File Offset: 0x000341A5
	public static bool UseTasksForJsonConversion { get; set; } = true;

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x06000C09 RID: 3081 RVA: 0x00035FAD File Offset: 0x000341AD
	// (set) Token: 0x06000C0A RID: 3082 RVA: 0x00035FB4 File Offset: 0x000341B4
	public static bool ForceCurrencyCountersAppear { get; set; }

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x06000C0B RID: 3083 RVA: 0x00035FBC File Offset: 0x000341BC
	// (set) Token: 0x06000C0C RID: 3084 RVA: 0x00035FC3 File Offset: 0x000341C3
	public static bool DisableMusicSync { get; private set; }

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000C0D RID: 3085 RVA: 0x00035FCB File Offset: 0x000341CB
	// (set) Token: 0x06000C0E RID: 3086 RVA: 0x00035FD2 File Offset: 0x000341D2
	public static bool SuperLuckyDice { get; private set; }

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000C0F RID: 3087 RVA: 0x00035FDA File Offset: 0x000341DA
	// (set) Token: 0x06000C10 RID: 3088 RVA: 0x00035FE1 File Offset: 0x000341E1
	public static bool ForceLanguageComponentUpdates { get; set; }

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000C11 RID: 3089 RVA: 0x00035FE9 File Offset: 0x000341E9
	// (set) Token: 0x06000C12 RID: 3090 RVA: 0x00035FF0 File Offset: 0x000341F0
	public static bool OverrideFrameSkip { get; private set; } = false;

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000C13 RID: 3091 RVA: 0x00035FF8 File Offset: 0x000341F8
	// (set) Token: 0x06000C14 RID: 3092 RVA: 0x00035FFF File Offset: 0x000341FF
	public static bool OverrideReadyWaitFrames { get; set; }

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000C15 RID: 3093 RVA: 0x00036007 File Offset: 0x00034207
	// (set) Token: 0x06000C16 RID: 3094 RVA: 0x0003600E File Offset: 0x0003420E
	public static int ReadyWaitFrames { get; set; } = 1;

	// Token: 0x17000139 RID: 313
	// (get) Token: 0x06000C17 RID: 3095 RVA: 0x00036016 File Offset: 0x00034216
	// (set) Token: 0x06000C18 RID: 3096 RVA: 0x0003601D File Offset: 0x0003421D
	public static bool OverrideFastTravelBackgroundLoadPriority { get; private set; }

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x06000C19 RID: 3097 RVA: 0x00036025 File Offset: 0x00034225
	// (set) Token: 0x06000C1A RID: 3098 RVA: 0x0003602C File Offset: 0x0003422C
	public static ThreadPriority BackgroundLoadPriority { get; private set; }

	// Token: 0x1700013B RID: 315
	// (get) Token: 0x06000C1B RID: 3099 RVA: 0x00036034 File Offset: 0x00034234
	// (set) Token: 0x06000C1C RID: 3100 RVA: 0x0003603B File Offset: 0x0003423B
	public static bool OverrideAsyncLoadPriority { get; private set; }

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x06000C1D RID: 3101 RVA: 0x00036043 File Offset: 0x00034243
	// (set) Token: 0x06000C1E RID: 3102 RVA: 0x0003604A File Offset: 0x0003424A
	public static int AsyncLoadPriority { get; private set; } = 50;

	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000C1F RID: 3103 RVA: 0x00036052 File Offset: 0x00034252
	// (set) Token: 0x06000C20 RID: 3104 RVA: 0x00036059 File Offset: 0x00034259
	public static bool OverrideSkipFrameOnDrop { get; private set; }

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000C21 RID: 3105 RVA: 0x00036061 File Offset: 0x00034261
	// (set) Token: 0x06000C22 RID: 3106 RVA: 0x00036068 File Offset: 0x00034268
	public static bool SkipVideoFrameOnDrop { get; private set; } = true;

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000C23 RID: 3107 RVA: 0x00036070 File Offset: 0x00034270
	// (set) Token: 0x06000C24 RID: 3108 RVA: 0x00036077 File Offset: 0x00034277
	public static bool FakeCorruptedState { get; private set; }

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000C25 RID: 3109 RVA: 0x0003607F File Offset: 0x0003427F
	// (set) Token: 0x06000C26 RID: 3110 RVA: 0x00036086 File Offset: 0x00034286
	public static bool FakeIncompatibleState { get; private set; }

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x06000C27 RID: 3111 RVA: 0x0003608E File Offset: 0x0003428E
	// (set) Token: 0x06000C28 RID: 3112 RVA: 0x00036095 File Offset: 0x00034295
	public static bool AlwaysAwardAchievement { get; set; }

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000C29 RID: 3113 RVA: 0x0003609D File Offset: 0x0003429D
	// (set) Token: 0x06000C2A RID: 3114 RVA: 0x000360A4 File Offset: 0x000342A4
	public static bool ShowAllQuestBoardQuest { get; set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000C2B RID: 3115 RVA: 0x000360AC File Offset: 0x000342AC
	// (set) Token: 0x06000C2C RID: 3116 RVA: 0x000360B3 File Offset: 0x000342B3
	public static float SceneEntryWait { get; set; } = -0.1f;

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000C2D RID: 3117 RVA: 0x000360BB File Offset: 0x000342BB
	// (set) Token: 0x06000C2E RID: 3118 RVA: 0x000360C2 File Offset: 0x000342C2
	public static bool BoostModeActive { get; set; }

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x06000C2F RID: 3119 RVA: 0x000360CA File Offset: 0x000342CA
	// (set) Token: 0x06000C30 RID: 3120 RVA: 0x000360D1 File Offset: 0x000342D1
	public static bool IsOpen { get; private set; }

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x06000C31 RID: 3121 RVA: 0x000360D9 File Offset: 0x000342D9
	// (set) Token: 0x06000C32 RID: 3122 RVA: 0x000360E0 File Offset: 0x000342E0
	public static bool IsStackTracesEnabled { get; private set; }

	// Token: 0x17000147 RID: 327
	// (get) Token: 0x06000C33 RID: 3123 RVA: 0x000360E8 File Offset: 0x000342E8
	// (set) Token: 0x06000C34 RID: 3124 RVA: 0x000360EF File Offset: 0x000342EF
	public static bool IsDialogueDebugEnabled { get; set; }

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x06000C35 RID: 3125 RVA: 0x000360F7 File Offset: 0x000342F7
	// (set) Token: 0x06000C36 RID: 3126 RVA: 0x000360FE File Offset: 0x000342FE
	public static bool IsWorldRumbleDisabled { get; set; }

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000C37 RID: 3127 RVA: 0x00036106 File Offset: 0x00034306
	// (set) Token: 0x06000C38 RID: 3128 RVA: 0x0003610D File Offset: 0x0003430D
	public static bool IsSilkDrainDisabled { get; set; }

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000C39 RID: 3129 RVA: 0x00036115 File Offset: 0x00034315
	// (set) Token: 0x06000C3A RID: 3130 RVA: 0x0003611C File Offset: 0x0003431C
	public static string LastErrorText { get; set; }

	// Token: 0x1700014B RID: 331
	// (get) Token: 0x06000C3B RID: 3131 RVA: 0x00036124 File Offset: 0x00034324
	// (set) Token: 0x06000C3C RID: 3132 RVA: 0x0003612B File Offset: 0x0003432B
	public static bool IsForcingUnloads { get; private set; }

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x06000C3D RID: 3133 RVA: 0x00036133 File Offset: 0x00034333
	// (set) Token: 0x06000C3E RID: 3134 RVA: 0x0003613A File Offset: 0x0003433A
	public static bool OverrideSceneLoadPriority { get; private set; }

	// Token: 0x1700014D RID: 333
	// (get) Token: 0x06000C3F RID: 3135 RVA: 0x00036142 File Offset: 0x00034342
	// (set) Token: 0x06000C40 RID: 3136 RVA: 0x00036149 File Offset: 0x00034349
	public static int SceneLoadPriority { get; private set; }

	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000C41 RID: 3137 RVA: 0x00036151 File Offset: 0x00034351
	// (set) Token: 0x06000C42 RID: 3138 RVA: 0x00036158 File Offset: 0x00034358
	public static bool ShowAllCompletionIcons { get; private set; } = false;

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000C43 RID: 3139 RVA: 0x00036160 File Offset: 0x00034360
	// (set) Token: 0x06000C44 RID: 3140 RVA: 0x00036167 File Offset: 0x00034367
	public static bool PS5BlockRefreshModeChange { get; set; }

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000C45 RID: 3141 RVA: 0x0003616F File Offset: 0x0003436F
	// (set) Token: 0x06000C46 RID: 3142 RVA: 0x00036176 File Offset: 0x00034376
	public static bool ForceChosenBlackThreadAttack { get; set; }

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000C47 RID: 3143 RVA: 0x0003617E File Offset: 0x0003437E
	// (set) Token: 0x06000C48 RID: 3144 RVA: 0x00036185 File Offset: 0x00034385
	public static BlackThreadAttack ChosenBlackThreadAttack { get; set; }

	// Token: 0x06000C49 RID: 3145 RVA: 0x0003618D File Offset: 0x0003438D
	protected IEnumerator Start()
	{
		CheatManager._instance = this;
		this.lineMaterial = new Material(Shader.Find("Sprites/Default"));
		this.CreateSubMenus();
		for (;;)
		{
			yield return new WaitForSeconds(1f);
			if (this.isRegenerating)
			{
				GameManager unsafeInstance = GameManager.UnsafeInstance;
				if (!(unsafeInstance == null))
				{
					HeroController hero_ctrl = unsafeInstance.hero_ctrl;
					if (!(hero_ctrl == null))
					{
						hero_ctrl.AddHealth(Mathf.Clamp(unsafeInstance.playerData.maxHealth - unsafeInstance.playerData.health, 0, 1));
						hero_ctrl.AddSilk(Mathf.Clamp(unsafeInstance.playerData.CurrentSilkMax - unsafeInstance.playerData.silk, 0, 3), false);
					}
				}
			}
		}
		yield break;
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x0003619C File Offset: 0x0003439C
	private void OnDestroy()
	{
		if (CheatManager._instance == this)
		{
			CheatManager._instance = null;
			CheatManager.IsOpen = false;
		}
		GUIDrawer.RemoveDrawer(this);
		if (this.lineMaterial != null)
		{
			Object.Destroy(this.lineMaterial);
			this.lineMaterial = null;
		}
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x000361E8 File Offset: 0x000343E8
	public static bool IsInventoryPaneHidden(InventoryPaneList.PaneTypes paneType)
	{
		return CheatManager._instance && CheatManager._instance.invPaneHiddenBitmask.IsBitSet((int)paneType);
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000C4C RID: 3148 RVA: 0x00036208 File Offset: 0x00034408
	public int GUIDepth
	{
		get
		{
			return 10;
		}
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0003620C File Offset: 0x0003440C
	public void DrawGUI()
	{
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0003620E File Offset: 0x0003440E
	private static string ColorTextRed(string text)
	{
		return "<color=red>" + text + "</color>";
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00036220 File Offset: 0x00034420
	private static string ColorTextGreen(string text)
	{
		return "<color=green>" + text + "</color>";
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000C50 RID: 3152 RVA: 0x00036232 File Offset: 0x00034432
	// (set) Token: 0x06000C51 RID: 3153 RVA: 0x00036239 File Offset: 0x00034439
	public static bool AddVibrationAmplitudes { get; private set; }

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000C52 RID: 3154 RVA: 0x00036241 File Offset: 0x00034441
	// (set) Token: 0x06000C53 RID: 3155 RVA: 0x00036248 File Offset: 0x00034448
	public static bool EnableLogMessages { get; private set; }

	// Token: 0x06000C54 RID: 3156 RVA: 0x00036250 File Offset: 0x00034450
	private void CreateSubMenus()
	{
	}

	// Token: 0x04000B65 RID: 2917
	private const float BUTTON_HEIGHT = 20f;

	// Token: 0x04000B66 RID: 2918
	private const float BUTTON_WIDTH = 350f;

	// Token: 0x04000B67 RID: 2919
	private const float SPACE_HEIGHT = 5f;

	// Token: 0x04000B68 RID: 2920
	private const float BASE_SCREEN_HEIGHT = 1080f;

	// Token: 0x04000B69 RID: 2921
	private const float X_INDENT = 15f;

	// Token: 0x04000B6A RID: 2922
	private const float Y_INDENT = 15f;

	// Token: 0x04000B6B RID: 2923
	private const int FONT_SIZE = 12;

	// Token: 0x04000B6C RID: 2924
	private const float MAX_SCALE_INCREASE = 5f;

	// Token: 0x04000B6D RID: 2925
	private const float MIN_SCALE = 0.5f;

	// Token: 0x04000B6E RID: 2926
	private static GUIStyle advanceButtonStyle;

	// Token: 0x04000B70 RID: 2928
	private static float lastMaxWidth;

	// Token: 0x04000B71 RID: 2929
	private static CheatManager _instance;

	// Token: 0x04000B72 RID: 2930
	private bool wasEverOpened;

	// Token: 0x04000B73 RID: 2931
	private int selectedButtonIndex;

	// Token: 0x04000B74 RID: 2932
	private int nextSelectedButtonIndex = -1;

	// Token: 0x04000B75 RID: 2933
	private int guiOffsetButtons;

	// Token: 0x04000B76 RID: 2934
	private int spaces;

	// Token: 0x04000B77 RID: 2935
	private int indentLevel;

	// Token: 0x04000B78 RID: 2936
	private bool isQuickHealEnabled;

	// Token: 0x04000B7A RID: 2938
	private bool isRegenerating;

	// Token: 0x04000B80 RID: 2944
	private static bool forceStun;

	// Token: 0x04000B9B RID: 2971
	private bool isFastTravelTeleportCheckActive;

	// Token: 0x04000B9C RID: 2972
	private int safetyCounter;

	// Token: 0x04000B9D RID: 2973
	private const int SAFETY_AMOUNT = 10;

	// Token: 0x04000B9E RID: 2974
	private const string TOGGLE_BUTTON_CHECKMARK = "■";

	// Token: 0x04000B9F RID: 2975
	private int selectDelta;

	// Token: 0x04000BA0 RID: 2976
	private CheatManager.MenuStates menuState;

	// Token: 0x04000BA1 RID: 2977
	private CheatManager.MenuStates queuedMenuState = CheatManager.MenuStates.None;

	// Token: 0x04000BA2 RID: 2978
	private bool disableAlphaInput;

	// Token: 0x04000BA3 RID: 2979
	private string textInputString;

	// Token: 0x04000BA4 RID: 2980
	private string playerDataSearchString;

	// Token: 0x04000BA5 RID: 2981
	private TouchScreenKeyboard touchKeyboard;

	// Token: 0x04000BA6 RID: 2982
	private CheatManager.TouchKeyboardStates touchKeyboardState;

	// Token: 0x04000BA7 RID: 2983
	private string pdSetFieldName;

	// Token: 0x04000BA8 RID: 2984
	private string pdSetString;

	// Token: 0x04000BA9 RID: 2985
	private string objectSearchString;

	// Token: 0x04000BAA RID: 2986
	private int invPaneHiddenBitmask;

	// Token: 0x04000BAB RID: 2987
	private readonly Dictionary<string, bool> tpMapGroupFoldout = new Dictionary<string, bool>();

	// Token: 0x04000BAC RID: 2988
	private readonly Dictionary<string, bool> tpMapSceneFoldout = new Dictionary<string, bool>();

	// Token: 0x04000BAD RID: 2989
	private readonly Dictionary<string, bool> crestFoldout = new Dictionary<string, bool>();

	// Token: 0x04000BAE RID: 2990
	private readonly Dictionary<string, bool> toolFoldout = new Dictionary<string, bool>();

	// Token: 0x04000BAF RID: 2991
	private readonly Dictionary<string, bool> questFoldout = new Dictionary<string, bool>();

	// Token: 0x04000BB0 RID: 2992
	private bool isDebugHeatEffect;

	// Token: 0x04000BB1 RID: 2993
	private static readonly int _heatEffectMultProp = Shader.PropertyToID("_HeatEffectMult");

	// Token: 0x04000BB2 RID: 2994
	private float slowOpenLeftStickTimer;

	// Token: 0x04000BB3 RID: 2995
	private float slowOpenRightStickTimer;

	// Token: 0x04000BB4 RID: 2996
	private float fastOpenTimer;

	// Token: 0x04000BB5 RID: 2997
	private float holdTime;

	// Token: 0x04000BB6 RID: 2998
	private float tickTime;

	// Token: 0x04000BB8 RID: 3000
	private bool isCrosshairEnabled;

	// Token: 0x04000BB9 RID: 3001
	private Material lineMaterial;

	// Token: 0x04000BBA RID: 3002
	private readonly Vector2[] currentLine = new Vector2[2];

	// Token: 0x04000BC8 RID: 3016
	private InputCapture inputCapture;

	// Token: 0x04000BC9 RID: 3017
	private int updateFrames = -1;

	// Token: 0x04000BCA RID: 3018
	private int mainMenuIndex = -1;

	// Token: 0x020014A2 RID: 5282
	private enum MenuStates
	{
		// Token: 0x040083FB RID: 33787
		None = -1,
		// Token: 0x040083FC RID: 33788
		Main,
		// Token: 0x040083FD RID: 33789
		Abilities,
		// Token: 0x040083FE RID: 33790
		System,
		// Token: 0x040083FF RID: 33791
		Teleport,
		// Token: 0x04008400 RID: 33792
		PlayerData,
		// Token: 0x04008401 RID: 33793
		Collectables,
		// Token: 0x04008402 RID: 33794
		Tools,
		// Token: 0x04008403 RID: 33795
		Quests,
		// Token: 0x04008404 RID: 33796
		SaveManagement,
		// Token: 0x04008405 RID: 33797
		Achievements,
		// Token: 0x04008406 RID: 33798
		GCPressure,
		// Token: 0x04008407 RID: 33799
		SubMenu
	}

	// Token: 0x020014A3 RID: 5283
	private enum TouchKeyboardStates
	{
		// Token: 0x04008409 RID: 33801
		Closed,
		// Token: 0x0400840A RID: 33802
		OpenPdSearch,
		// Token: 0x0400840B RID: 33803
		OpenPdSet,
		// Token: 0x0400840C RID: 33804
		OpenObjectSearch
	}

	// Token: 0x020014A4 RID: 5284
	private enum ButtonInputState
	{
		// Token: 0x0400840E RID: 33806
		None,
		// Token: 0x0400840F RID: 33807
		Confirm,
		// Token: 0x04008410 RID: 33808
		Left,
		// Token: 0x04008411 RID: 33809
		Right
	}

	// Token: 0x020014A5 RID: 5285
	public enum InvincibilityStates
	{
		// Token: 0x04008413 RID: 33811
		Off,
		// Token: 0x04008414 RID: 33812
		TestInvincible,
		// Token: 0x04008415 RID: 33813
		FullInvincible,
		// Token: 0x04008416 RID: 33814
		PreventDeath
	}

	// Token: 0x020014A6 RID: 5286
	public enum NailDamageStates
	{
		// Token: 0x04008418 RID: 33816
		Normal,
		// Token: 0x04008419 RID: 33817
		InstaKill,
		// Token: 0x0400841A RID: 33818
		NoDamage
	}

	// Token: 0x020014A7 RID: 5287
	public enum DamageSelfStates
	{
		// Token: 0x0400841C RID: 33820
		None,
		// Token: 0x0400841D RID: 33821
		SingleHit,
		// Token: 0x0400841E RID: 33822
		DoubleHit,
		// Token: 0x0400841F RID: 33823
		Death
	}
}
