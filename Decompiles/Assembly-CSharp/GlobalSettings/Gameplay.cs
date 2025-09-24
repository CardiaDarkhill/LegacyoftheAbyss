using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

namespace GlobalSettings
{
	// Token: 0x020008CA RID: 2250
	[CreateAssetMenu(menuName = "Hornet/Global Settings/Global Gameplay Settings")]
	public class Gameplay : GlobalSettingsBase<Gameplay>
	{
		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x06004E13 RID: 19987 RVA: 0x0016D1F7 File Offset: 0x0016B3F7
		public static int ToolReplenishCost
		{
			get
			{
				return Gameplay.Get().toolReplenishCost;
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x06004E14 RID: 19988 RVA: 0x0016D203 File Offset: 0x0016B403
		public static float ToolPouchUpgradeIncrease
		{
			get
			{
				return Gameplay.Get().toolPouchUpgradeIncrease;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x06004E15 RID: 19989 RVA: 0x0016D20F File Offset: 0x0016B40F
		public static float ToolKitDamageIncrease
		{
			get
			{
				return Gameplay.Get().toolKitDamageIncrease;
			}
		}

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x06004E16 RID: 19990 RVA: 0x0016D21B File Offset: 0x0016B41B
		public static float ToolCameraDistanceBreak
		{
			get
			{
				return Gameplay.Get().toolCameraDistanceBreak;
			}
		}

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x06004E17 RID: 19991 RVA: 0x0016D227 File Offset: 0x0016B427
		public static int ConsumableItemCap
		{
			get
			{
				return Gameplay.Get().consumableItemCap;
			}
		}

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x06004E18 RID: 19992 RVA: 0x0016D233 File Offset: 0x0016B433
		public static CostReference SmallGeoValue
		{
			get
			{
				return Gameplay.Get().smallGeoValue;
			}
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06004E19 RID: 19993 RVA: 0x0016D23F File Offset: 0x0016B43F
		public static CostReference MediumGeoValue
		{
			get
			{
				return Gameplay.Get().mediumGeoValue;
			}
		}

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x06004E1A RID: 19994 RVA: 0x0016D24B File Offset: 0x0016B44B
		public static CostReference LargeGeoValue
		{
			get
			{
				return Gameplay.Get().largeGeoValue;
			}
		}

		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06004E1B RID: 19995 RVA: 0x0016D257 File Offset: 0x0016B457
		public static GameObject SmallGeoPrefab
		{
			get
			{
				return Gameplay.Get().smallGeoPrefab;
			}
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06004E1C RID: 19996 RVA: 0x0016D263 File Offset: 0x0016B463
		public static GameObject MediumGeoPrefab
		{
			get
			{
				return Gameplay.Get().mediumGeoPrefab;
			}
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x06004E1D RID: 19997 RVA: 0x0016D26F File Offset: 0x0016B46F
		public static GameObject LargeGeoPrefab
		{
			get
			{
				return Gameplay.Get().largeGeoPrefab;
			}
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x06004E1E RID: 19998 RVA: 0x0016D27B File Offset: 0x0016B47B
		public static GameObject LargeSmoothGeoPrefab
		{
			get
			{
				return Gameplay.Get().largeSmoothGeoPrefab;
			}
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x06004E1F RID: 19999 RVA: 0x0016D288 File Offset: 0x0016B488
		public static GameObject ShellShardPrefab
		{
			get
			{
				List<GameObject> list = Gameplay.Get().shellShardPrefabs;
				return list[Random.Range(0, list.Count)];
			}
		}

		// Token: 0x06004E20 RID: 20000 RVA: 0x0016D2B2 File Offset: 0x0016B4B2
		public static bool IsShellShardPrefab(GameObject prefab)
		{
			return Gameplay.Get().shellShardPrefabs.Contains(prefab);
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x06004E21 RID: 20001 RVA: 0x0016D2C4 File Offset: 0x0016B4C4
		public static GameObject ShellShardMidPrefab
		{
			get
			{
				return Gameplay.Get().shellShardMidPrefab;
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x06004E22 RID: 20002 RVA: 0x0016D2D0 File Offset: 0x0016B4D0
		public static CollectableItemPickup CollectableItemPickupPrefab
		{
			get
			{
				return Gameplay.Get().collectableItemPickupPrefab;
			}
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x06004E23 RID: 20003 RVA: 0x0016D2DC File Offset: 0x0016B4DC
		public static CollectableItemPickup CollectableItemPickupInstantPrefab
		{
			get
			{
				return Gameplay.Get().collectableItemPickupInstantPrefab;
			}
		}

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06004E24 RID: 20004 RVA: 0x0016D2E8 File Offset: 0x0016B4E8
		public static GenericPickup GenericPickupPrefab
		{
			get
			{
				return Gameplay.Get().genericPickupPrefab;
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x06004E25 RID: 20005 RVA: 0x0016D2F4 File Offset: 0x0016B4F4
		public static CollectableItemPickup CollectableItemPickupMeatPrefab
		{
			get
			{
				return Gameplay.Get().collectableItemPickupMeatPrefab;
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x06004E26 RID: 20006 RVA: 0x0016D300 File Offset: 0x0016B500
		public static GameObject ReaperBundlePrefab
		{
			get
			{
				return Gameplay.Get().reaperBundlePrefab;
			}
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x06004E27 RID: 20007 RVA: 0x0016D30C File Offset: 0x0016B50C
		public static FsmTemplate HornetMultiWounderFsmTemplate
		{
			get
			{
				return Gameplay.Get().hornetMultiWounderFsmTemplate;
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x06004E28 RID: 20008 RVA: 0x0016D318 File Offset: 0x0016B518
		public static CollectableItemMementoList MementoList
		{
			get
			{
				return Gameplay.Get().mementoList;
			}
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x06004E29 RID: 20009 RVA: 0x0016D324 File Offset: 0x0016B524
		public static ToolCrest HunterCrest
		{
			get
			{
				return Gameplay.Get().hunterCrest;
			}
		}

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06004E2A RID: 20010 RVA: 0x0016D330 File Offset: 0x0016B530
		public static ToolCrest HunterCrest2
		{
			get
			{
				return Gameplay.Get().hunterCrest2;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x06004E2B RID: 20011 RVA: 0x0016D33C File Offset: 0x0016B53C
		public static int HunterComboHits
		{
			get
			{
				return Gameplay.Get().hunterComboHits;
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x06004E2C RID: 20012 RVA: 0x0016D348 File Offset: 0x0016B548
		public static float HunterComboDamageMult
		{
			get
			{
				return Gameplay.Get().hunterComboDamageMult;
			}
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x06004E2D RID: 20013 RVA: 0x0016D354 File Offset: 0x0016B554
		public static ToolCrest HunterCrest3
		{
			get
			{
				return Gameplay.Get().hunterCrest3;
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x06004E2E RID: 20014 RVA: 0x0016D360 File Offset: 0x0016B560
		public static int HunterCombo2Hits
		{
			get
			{
				return Gameplay.Get().hunterCombo2Hits;
			}
		}

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x06004E2F RID: 20015 RVA: 0x0016D36C File Offset: 0x0016B56C
		public static float HunterCombo2DamageMult
		{
			get
			{
				return Gameplay.Get().hunterCombo2DamageMult;
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x06004E30 RID: 20016 RVA: 0x0016D378 File Offset: 0x0016B578
		public static GameObject HunterComboDamageEffect
		{
			get
			{
				return Gameplay.Get().hunterComboDamageEffect;
			}
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x06004E31 RID: 20017 RVA: 0x0016D384 File Offset: 0x0016B584
		public static int HunterCombo2ExtraHits
		{
			get
			{
				return Gameplay.Get().hunterCombo2ExtraHits;
			}
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06004E32 RID: 20018 RVA: 0x0016D390 File Offset: 0x0016B590
		public static float HunterCombo2ExtraDamageMult
		{
			get
			{
				return Gameplay.Get().hunterCombo2ExtraDamageMult;
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x06004E33 RID: 20019 RVA: 0x0016D39C File Offset: 0x0016B59C
		public static Vector2 HunterCombo2DamageExtraScale
		{
			get
			{
				return Gameplay.Get().hunterCombo2DamageExtraScale;
			}
		}

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x06004E34 RID: 20020 RVA: 0x0016D3A8 File Offset: 0x0016B5A8
		public static ToolCrest WarriorCrest
		{
			get
			{
				return Gameplay.Get().warriorCrest;
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x06004E35 RID: 20021 RVA: 0x0016D3B4 File Offset: 0x0016B5B4
		public static float WarriorRageDuration
		{
			get
			{
				return Gameplay.Get().warriorRageDuration;
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x06004E36 RID: 20022 RVA: 0x0016D3C0 File Offset: 0x0016B5C0
		public static IReadOnlyList<float> WarriorRageHitAddTimePerHit
		{
			get
			{
				return Gameplay.Get().warriorRageHitAddTimePerHit;
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x06004E37 RID: 20023 RVA: 0x0016D3CC File Offset: 0x0016B5CC
		public static float WarriorDamageMultiplier
		{
			get
			{
				return Gameplay.Get().warriorDamageMultiplier;
			}
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x06004E38 RID: 20024 RVA: 0x0016D3D8 File Offset: 0x0016B5D8
		public static float WarriorRageDamagedRemoveTime
		{
			get
			{
				return Gameplay.Get().warriorRageDamagedRemoveTime;
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x06004E39 RID: 20025 RVA: 0x0016D3E4 File Offset: 0x0016B5E4
		public static ToolCrest ReaperCrest
		{
			get
			{
				return Gameplay.Get().reaperCrest;
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x06004E3A RID: 20026 RVA: 0x0016D3F0 File Offset: 0x0016B5F0
		public static float ReaperModeDuration
		{
			get
			{
				return Gameplay.Get().reaperModeDuration;
			}
		}

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x06004E3B RID: 20027 RVA: 0x0016D3FC File Offset: 0x0016B5FC
		public static ToolCrest WandererCrest
		{
			get
			{
				return Gameplay.Get().wandererCrest;
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x06004E3C RID: 20028 RVA: 0x0016D408 File Offset: 0x0016B608
		public static float WandererCritChance
		{
			get
			{
				return Gameplay.Get().wandererCritChance;
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x06004E3D RID: 20029 RVA: 0x0016D414 File Offset: 0x0016B614
		public static float WandererCritMultiplier
		{
			get
			{
				return Gameplay.Get().wandererCritMultiplier;
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x06004E3E RID: 20030 RVA: 0x0016D420 File Offset: 0x0016B620
		public static float WandererCritMagnitudeMult
		{
			get
			{
				return Gameplay.Get().wandererCritMagnitudeMult;
			}
		}

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06004E3F RID: 20031 RVA: 0x0016D42C File Offset: 0x0016B62C
		public static GameObject WandererCritEffect
		{
			get
			{
				return Gameplay.Get().wandererCritEffect;
			}
		}

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06004E40 RID: 20032 RVA: 0x0016D438 File Offset: 0x0016B638
		public static ToolCrest CursedCrest
		{
			get
			{
				return Gameplay.Get().cursedCrest;
			}
		}

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x06004E41 RID: 20033 RVA: 0x0016D444 File Offset: 0x0016B644
		public static ToolCrest WitchCrest
		{
			get
			{
				return Gameplay.Get().witchCrest;
			}
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x06004E42 RID: 20034 RVA: 0x0016D450 File Offset: 0x0016B650
		public static int WitchCrestRecoilSteps
		{
			get
			{
				return Gameplay.Get().witchCrestRecoilSteps;
			}
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x06004E43 RID: 20035 RVA: 0x0016D45C File Offset: 0x0016B65C
		public static ToolCrest ToolmasterCrest
		{
			get
			{
				return Gameplay.Get().toolmasterCrest;
			}
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x06004E44 RID: 20036 RVA: 0x0016D468 File Offset: 0x0016B668
		public static int ToolmasterQuickCraftNoneUsage
		{
			get
			{
				return Gameplay.Get().toolmasterQuickCraftNoneUsage;
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x06004E45 RID: 20037 RVA: 0x0016D474 File Offset: 0x0016B674
		public static ToolCrest SpellCrest
		{
			get
			{
				return Gameplay.Get().spellCrest;
			}
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06004E46 RID: 20038 RVA: 0x0016D480 File Offset: 0x0016B680
		public static float SpellCrestRuneDamageMult
		{
			get
			{
				return Gameplay.Get().spellCrestRuneDamageMult;
			}
		}

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x06004E47 RID: 20039 RVA: 0x0016D48C File Offset: 0x0016B68C
		public static ToolCrest CloaklessCrest
		{
			get
			{
				return Gameplay.Get().cloaklessCrest;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x06004E48 RID: 20040 RVA: 0x0016D498 File Offset: 0x0016B698
		public static ToolItem DefaultSkillTool
		{
			get
			{
				return Gameplay.Get().defaultSkillTool;
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06004E49 RID: 20041 RVA: 0x0016D4A4 File Offset: 0x0016B6A4
		public static ToolItem LuckyDiceTool
		{
			get
			{
				return Gameplay.Get().luckyDiceTool;
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06004E4A RID: 20042 RVA: 0x0016D4B0 File Offset: 0x0016B6B0
		public static ToolItem BarbedWireTool
		{
			get
			{
				return Gameplay.Get().barbedWireTool;
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06004E4B RID: 20043 RVA: 0x0016D4BC File Offset: 0x0016B6BC
		public static float BarbedWireDamageDealtMultiplier
		{
			get
			{
				return Gameplay.Get().barbedWireDamageDealtMultiplier;
			}
		}

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06004E4C RID: 20044 RVA: 0x0016D4C8 File Offset: 0x0016B6C8
		public static float BarbedWireDamageTakenMultiplier
		{
			get
			{
				return Gameplay.Get().barbedWireDamageTakenMultiplier;
			}
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x06004E4D RID: 20045 RVA: 0x0016D4D4 File Offset: 0x0016B6D4
		public static ToolItem WeightedAnkletTool
		{
			get
			{
				return Gameplay.Get().weightedAnkletTool;
			}
		}

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x06004E4E RID: 20046 RVA: 0x0016D4E0 File Offset: 0x0016B6E0
		public static float WeightedAnkletDmgKnockbackMult
		{
			get
			{
				return Gameplay.Get().weightedAnkletDmgKnockbackMult;
			}
		}

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x06004E4F RID: 20047 RVA: 0x0016D4EC File Offset: 0x0016B6EC
		public static float WeightedAnkletDmgInvulnMult
		{
			get
			{
				return Gameplay.Get().weightedAnkletDmgInvulnMult;
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x06004E50 RID: 20048 RVA: 0x0016D4F8 File Offset: 0x0016B6F8
		public static int WeightedAnkletRecoilSteps
		{
			get
			{
				return Gameplay.Get().weightedAnkletRecoilSteps;
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x06004E51 RID: 20049 RVA: 0x0016D504 File Offset: 0x0016B704
		public static ToolItem BoneNecklaceTool
		{
			get
			{
				return Gameplay.Get().boneNecklaceTool;
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06004E52 RID: 20050 RVA: 0x0016D510 File Offset: 0x0016B710
		public static float BoneNecklaceShellshardIncrease
		{
			get
			{
				return Gameplay.Get().boneNecklaceShellshardIncrease;
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06004E53 RID: 20051 RVA: 0x0016D51C File Offset: 0x0016B71C
		public static ToolItem LongNeedleTool
		{
			get
			{
				return Gameplay.Get().longNeedleTool;
			}
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06004E54 RID: 20052 RVA: 0x0016D528 File Offset: 0x0016B728
		public static Vector2 LongNeedleMultiplier
		{
			get
			{
				return Gameplay.Get().longNeedleMultiplier;
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06004E55 RID: 20053 RVA: 0x0016D534 File Offset: 0x0016B734
		public static ToolItem DeadPurseTool
		{
			get
			{
				return Gameplay.Get().deadPurseTool;
			}
		}

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06004E56 RID: 20054 RVA: 0x0016D540 File Offset: 0x0016B740
		public static float DeadPurseHoldPercent
		{
			get
			{
				return Gameplay.Get().deadPurseHoldPercent;
			}
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06004E57 RID: 20055 RVA: 0x0016D54C File Offset: 0x0016B74C
		public static ToolItem ShellSatchelTool
		{
			get
			{
				return Gameplay.Get().shellSatchelTool;
			}
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06004E58 RID: 20056 RVA: 0x0016D558 File Offset: 0x0016B758
		public static float ShellSatchelToolIncrease
		{
			get
			{
				return Gameplay.Get().shellSatchelToolIncrease;
			}
		}

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06004E59 RID: 20057 RVA: 0x0016D564 File Offset: 0x0016B764
		public static ToolItem SpoolExtenderTool
		{
			get
			{
				return Gameplay.Get().spoolExtenderTool;
			}
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06004E5A RID: 20058 RVA: 0x0016D570 File Offset: 0x0016B770
		public static int SpoolExtenderSilk
		{
			get
			{
				return Gameplay.Get().spoolExtenderSilk;
			}
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x06004E5B RID: 20059 RVA: 0x0016D57C File Offset: 0x0016B77C
		public static ToolItem WhiteRingTool
		{
			get
			{
				return Gameplay.Get().whiteRingTool;
			}
		}

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x06004E5C RID: 20060 RVA: 0x0016D588 File Offset: 0x0016B788
		public static float WhiteRingSilkRegenTimeMultiplier
		{
			get
			{
				return Gameplay.Get().whiteRingSilkRegenTimeMultiplier;
			}
		}

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x06004E5D RID: 20061 RVA: 0x0016D594 File Offset: 0x0016B794
		public static int WhiteRingSilkRegenIncrease
		{
			get
			{
				return Gameplay.Get().whiteRingSilkRegenIncrease;
			}
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x06004E5E RID: 20062 RVA: 0x0016D5A0 File Offset: 0x0016B7A0
		public static ToolItem LavaBellTool
		{
			get
			{
				return Gameplay.Get().lavaBellTool;
			}
		}

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x06004E5F RID: 20063 RVA: 0x0016D5AC File Offset: 0x0016B7AC
		public static float LavaBellCooldownTime
		{
			get
			{
				return Gameplay.Get().lavaBellCooldownTime;
			}
		}

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x06004E60 RID: 20064 RVA: 0x0016D5B8 File Offset: 0x0016B7B8
		public static ToolItem ThiefCharmTool
		{
			get
			{
				return Gameplay.Get().thiefCharmTool;
			}
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06004E61 RID: 20065 RVA: 0x0016D5C4 File Offset: 0x0016B7C4
		public static float ThiefCharmGeoSmallIncrease
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoSmallIncrease;
			}
		}

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x06004E62 RID: 20066 RVA: 0x0016D5D0 File Offset: 0x0016B7D0
		public static float ThiefCharmGeoMedIncrease
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoMedIncrease;
			}
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x06004E63 RID: 20067 RVA: 0x0016D5DC File Offset: 0x0016B7DC
		public static float ThiefCharmGeoLargeIncrease
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoLargeIncrease;
			}
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06004E64 RID: 20068 RVA: 0x0016D5E8 File Offset: 0x0016B7E8
		public static MinMaxFloat ThiefCharmGeoLoss
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoLoss;
			}
		}

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x06004E65 RID: 20069 RVA: 0x0016D5F4 File Offset: 0x0016B7F4
		public static MinMaxInt ThiefCharmGeoLossCap
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoLossCap;
			}
		}

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x06004E66 RID: 20070 RVA: 0x0016D600 File Offset: 0x0016B800
		public static float ThiefCharmGeoLossLooseChance
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoLossLooseChance;
			}
		}

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x06004E67 RID: 20071 RVA: 0x0016D60C File Offset: 0x0016B80C
		public static MinMaxInt ThiefCharmGeoLossLooseAmount
		{
			get
			{
				return Gameplay.Get().thiefCharmGeoLossLooseAmount;
			}
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x06004E68 RID: 20072 RVA: 0x0016D618 File Offset: 0x0016B818
		public static GameObject ThiefCharmHeroHitPrefab
		{
			get
			{
				return Gameplay.Get().thiefCharmHeroHitPrefab;
			}
		}

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x06004E69 RID: 20073 RVA: 0x0016D624 File Offset: 0x0016B824
		public static AudioEvent ThiefCharmEnemyDeathAudio
		{
			get
			{
				return Gameplay.Get().thiefCharmEnemyDeathAudio;
			}
		}

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06004E6A RID: 20074 RVA: 0x0016D630 File Offset: 0x0016B830
		public static ToolItem ThiefPickTool
		{
			get
			{
				return Gameplay.Get().thiefPickTool;
			}
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06004E6B RID: 20075 RVA: 0x0016D63C File Offset: 0x0016B83C
		public static MinMaxFloat ThiefPickGeoSteal
		{
			get
			{
				return Gameplay.Get().thiefPickGeoSteal;
			}
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06004E6C RID: 20076 RVA: 0x0016D648 File Offset: 0x0016B848
		public static MinMaxInt ThiefPickGeoStealMin
		{
			get
			{
				return Gameplay.Get().thiefPickGeoStealMin;
			}
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06004E6D RID: 20077 RVA: 0x0016D654 File Offset: 0x0016B854
		public static MinMaxInt ThiefPickShardSteal
		{
			get
			{
				return Gameplay.Get().thiefPickShardSteal;
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06004E6E RID: 20078 RVA: 0x0016D660 File Offset: 0x0016B860
		public static ThiefSnatchEffect ThiefSnatchEffectPrefab
		{
			get
			{
				return Gameplay.Get().thiefSnatchEffectPrefab;
			}
		}

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06004E6F RID: 20079 RVA: 0x0016D66C File Offset: 0x0016B86C
		public static float ThiefPickLooseChance
		{
			get
			{
				return Gameplay.Get().thiefPickLooseChance;
			}
		}

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06004E70 RID: 20080 RVA: 0x0016D678 File Offset: 0x0016B878
		public static MinMaxInt ThiefPickGeoLoose
		{
			get
			{
				return Gameplay.Get().thiefPickGeoLoose;
			}
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06004E71 RID: 20081 RVA: 0x0016D684 File Offset: 0x0016B884
		public static MinMaxInt ThiefPickShardLoose
		{
			get
			{
				return Gameplay.Get().thiefPickShardLoose;
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x06004E72 RID: 20082 RVA: 0x0016D690 File Offset: 0x0016B890
		public static ToolItem MaggotCharm
		{
			get
			{
				return Gameplay.Get().maggotCharm;
			}
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x06004E73 RID: 20083 RVA: 0x0016D69C File Offset: 0x0016B89C
		public static float MaggotCharmHealthLossTime
		{
			get
			{
				return Gameplay.Get().maggotCharmHealthLossTime;
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x06004E74 RID: 20084 RVA: 0x0016D6A8 File Offset: 0x0016B8A8
		public static float MaggotCharmEnterWaterAddTime
		{
			get
			{
				return Gameplay.Get().maggotCharmEnterWaterAddTime;
			}
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06004E75 RID: 20085 RVA: 0x0016D6B4 File Offset: 0x0016B8B4
		public static GameObject MaggotCharmHitSinglePrefab
		{
			get
			{
				return Gameplay.Get().maggotCharmHitSinglePrefab;
			}
		}

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06004E76 RID: 20086 RVA: 0x0016D6C0 File Offset: 0x0016B8C0
		public static ToolItem PoisonPouchTool
		{
			get
			{
				return Gameplay.Get().poisonPouchTool;
			}
		}

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06004E77 RID: 20087 RVA: 0x0016D6CC File Offset: 0x0016B8CC
		public static DamageTag PoisonPouchDamageTag
		{
			get
			{
				return Gameplay.Get().poisonPouchDamageTag;
			}
		}

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06004E78 RID: 20088 RVA: 0x0016D6D8 File Offset: 0x0016B8D8
		public static Color PoisonPouchTintColour
		{
			get
			{
				return Gameplay.Get().poisonPouchTintColour;
			}
		}

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06004E79 RID: 20089 RVA: 0x0016D6E4 File Offset: 0x0016B8E4
		public static Color PoisonPouchHeroTintColour
		{
			get
			{
				return Gameplay.Get().poisonPouchHeroTintColour;
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06004E7A RID: 20090 RVA: 0x0016D6F0 File Offset: 0x0016B8F0
		public static ToolItem ZapImbuementTool
		{
			get
			{
				return Gameplay.Get().zapImbuementTool;
			}
		}

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x06004E7B RID: 20091 RVA: 0x0016D6FC File Offset: 0x0016B8FC
		public static DamageTag ZapDamageTag
		{
			get
			{
				return Gameplay.Get().zapDamageTag;
			}
		}

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x06004E7C RID: 20092 RVA: 0x0016D708 File Offset: 0x0016B908
		public static Color ZapDamageTintColour
		{
			get
			{
				return Gameplay.Get().zapDamageTintColour;
			}
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06004E7D RID: 20093 RVA: 0x0016D714 File Offset: 0x0016B914
		public static float ZapDamageMult
		{
			get
			{
				return Gameplay.Get().zapDamageMult;
			}
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06004E7E RID: 20094 RVA: 0x0016D720 File Offset: 0x0016B920
		public static ToolItem MusicianCharmTool
		{
			get
			{
				return Gameplay.Get().musicianCharmTool;
			}
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06004E7F RID: 20095 RVA: 0x0016D72C File Offset: 0x0016B92C
		public static float MusicianCharmSilkDrainTimeMult
		{
			get
			{
				return Gameplay.Get().musicianCharmSilkDrainTimeMult;
			}
		}

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x06004E80 RID: 20096 RVA: 0x0016D738 File Offset: 0x0016B938
		public static float MusicianCharmNeedolinRangeMult
		{
			get
			{
				return Gameplay.Get().musicianCharmNeedolinRangeMult;
			}
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x06004E81 RID: 20097 RVA: 0x0016D744 File Offset: 0x0016B944
		public static float MusicianCharmFovOffset
		{
			get
			{
				return Gameplay.Get().musicianCharmFovOffset;
			}
		}

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x06004E82 RID: 20098 RVA: 0x0016D750 File Offset: 0x0016B950
		public static float MusicianCharmFovStartDuration
		{
			get
			{
				return Gameplay.Get().musicianCharmFovStartDuration;
			}
		}

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x06004E83 RID: 20099 RVA: 0x0016D75C File Offset: 0x0016B95C
		public static AnimationCurve MusicianCharmFovStartCurve
		{
			get
			{
				return Gameplay.Get().musicianCharmFovStartCurve;
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x06004E84 RID: 20100 RVA: 0x0016D768 File Offset: 0x0016B968
		public static float MusicianCharmFovEndDuration
		{
			get
			{
				return Gameplay.Get().musicianCharmFovEndDuration;
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06004E85 RID: 20101 RVA: 0x0016D774 File Offset: 0x0016B974
		public static AnimationCurve MusicianCharmFovEndCurve
		{
			get
			{
				return Gameplay.Get().musicianCharmFovEndCurve;
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x06004E86 RID: 20102 RVA: 0x0016D780 File Offset: 0x0016B980
		public static ToolItem ScuttleCharmTool
		{
			get
			{
				return Gameplay.Get().scuttleCharmTool;
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x06004E87 RID: 20103 RVA: 0x0016D78C File Offset: 0x0016B98C
		public static GameObject ScuttleEvadeEffect
		{
			get
			{
				return Gameplay.Get().scuttleEvadeEffect;
			}
		}

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x06004E88 RID: 20104 RVA: 0x0016D798 File Offset: 0x0016B998
		public static ToolItem BrollySpikeTool
		{
			get
			{
				return Gameplay.Get().brollySpikeTool;
			}
		}

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x06004E89 RID: 20105 RVA: 0x0016D7A4 File Offset: 0x0016B9A4
		public static GameObject BrollySpikeObject_dj
		{
			get
			{
				return Gameplay.Get().brollySpikeObject_dj;
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x06004E8A RID: 20106 RVA: 0x0016D7B0 File Offset: 0x0016B9B0
		public static ToolItem CompassTool
		{
			get
			{
				return Gameplay.Get().compassTool;
			}
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x06004E8B RID: 20107 RVA: 0x0016D7BC File Offset: 0x0016B9BC
		public static ToolItem MossCreep1Tool
		{
			get
			{
				return Gameplay.Get().mossCreep1Tool;
			}
		}

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06004E8C RID: 20108 RVA: 0x0016D7C8 File Offset: 0x0016B9C8
		public static ToolItem MossCreep2Tool
		{
			get
			{
				return Gameplay.Get().mossCreep2Tool;
			}
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06004E8D RID: 20109 RVA: 0x0016D7D4 File Offset: 0x0016B9D4
		public static ToolItem FracturedMaskTool
		{
			get
			{
				return Gameplay.Get().fracturedMaskTool;
			}
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x06004E8E RID: 20110 RVA: 0x0016D7E0 File Offset: 0x0016B9E0
		public static ToolItem CurveclawTool
		{
			get
			{
				return Gameplay.Get().curveclawTool;
			}
		}

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x06004E8F RID: 20111 RVA: 0x0016D7EC File Offset: 0x0016B9EC
		public static ToolItem CurveclawUpgradedTool
		{
			get
			{
				return Gameplay.Get().curveclawUpgradedTool;
			}
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x06004E90 RID: 20112 RVA: 0x0016D7F8 File Offset: 0x0016B9F8
		public static ToolItem SprintmasterTool
		{
			get
			{
				return Gameplay.Get().sprintmasterTool;
			}
		}

		// Token: 0x170009F4 RID: 2548
		// (get) Token: 0x06004E91 RID: 20113 RVA: 0x0016D804 File Offset: 0x0016BA04
		public static ToolItem MultibindTool
		{
			get
			{
				return Gameplay.Get().multibindTool;
			}
		}

		// Token: 0x170009F5 RID: 2549
		// (get) Token: 0x06004E92 RID: 20114 RVA: 0x0016D810 File Offset: 0x0016BA10
		public static ToolItem QuickSlingTool
		{
			get
			{
				return Gameplay.Get().quickSlingTool;
			}
		}

		// Token: 0x170009F6 RID: 2550
		// (get) Token: 0x06004E93 RID: 20115 RVA: 0x0016D81C File Offset: 0x0016BA1C
		public static ToolItem WallClingTool
		{
			get
			{
				return Gameplay.Get().wallClingTool;
			}
		}

		// Token: 0x170009F7 RID: 2551
		// (get) Token: 0x06004E94 RID: 20116 RVA: 0x0016D828 File Offset: 0x0016BA28
		public static ToolItem WispLanternTool
		{
			get
			{
				return Gameplay.Get().wispLanternTool;
			}
		}

		// Token: 0x170009F8 RID: 2552
		// (get) Token: 0x06004E95 RID: 20117 RVA: 0x0016D834 File Offset: 0x0016BA34
		public static ToolItem FleaCharmTool
		{
			get
			{
				return Gameplay.Get().fleaCharmTool;
			}
		}

		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x06004E96 RID: 20118 RVA: 0x0016D840 File Offset: 0x0016BA40
		public static FullQuestBase HuntressQuest
		{
			get
			{
				return Gameplay.Get().huntressQuest;
			}
		}

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x06004E97 RID: 20119 RVA: 0x0016D84C File Offset: 0x0016BA4C
		public static ShopItemList MapperStock
		{
			get
			{
				return Gameplay.Get().mapperStock;
			}
		}

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x06004E98 RID: 20120 RVA: 0x0016D858 File Offset: 0x0016BA58
		public static QuestTargetPlayerDataBools FleasCollectedCounter
		{
			get
			{
				return Gameplay.Get().fleasCollectedCounter;
			}
		}

		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x06004E99 RID: 20121 RVA: 0x0016D864 File Offset: 0x0016BA64
		public static int FleasCollectedCount
		{
			get
			{
				QuestTargetPlayerDataBools questTargetPlayerDataBools = Gameplay.Get().fleasCollectedCounter;
				if (!questTargetPlayerDataBools)
				{
					return 0;
				}
				return questTargetPlayerDataBools.CountCompleted();
			}
		}

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x06004E9A RID: 20122 RVA: 0x0016D88C File Offset: 0x0016BA8C
		public static QuestBoardList EnclaveQuestBoard
		{
			get
			{
				return Gameplay.Get().enclaveQuestBoard;
			}
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x0016D898 File Offset: 0x0016BA98
		[RuntimeInitializeOnLoadMethod]
		public static void PreWarm()
		{
			GlobalSettingsBase<Gameplay>.StartPreloadAddressable("Global Gameplay Settings");
		}

		// Token: 0x06004E9C RID: 20124 RVA: 0x0016D8A4 File Offset: 0x0016BAA4
		public static void Unload()
		{
			GlobalSettingsBase<Gameplay>.StartUnload();
		}

		// Token: 0x06004E9D RID: 20125 RVA: 0x0016D8AB File Offset: 0x0016BAAB
		private static Gameplay Get()
		{
			return GlobalSettingsBase<Gameplay>.Get("Global Gameplay Settings");
		}

		// Token: 0x06004E9E RID: 20126 RVA: 0x0016D8B7 File Offset: 0x0016BAB7
		private void OnValidate()
		{
			ArrayForEnumAttribute.EnsureArraySize<int>(ref this.currencyCaps, typeof(CurrencyType));
			ArrayForEnumAttribute.EnsureArraySize<int>(ref this.maxCurrencyObjects, typeof(CurrencyType));
		}

		// Token: 0x06004E9F RID: 20127 RVA: 0x0016D8E3 File Offset: 0x0016BAE3
		private void Awake()
		{
			this.OnValidate();
		}

		// Token: 0x06004EA0 RID: 20128 RVA: 0x0016D8EC File Offset: 0x0016BAEC
		public static int GetCurrencyCap(CurrencyType type)
		{
			int num = Gameplay.Get().currencyCaps[(int)type];
			num += Mathf.FloorToInt((float)num * Gameplay.ToolPouchUpgradeIncrease * (float)PlayerData.instance.ToolPouchUpgrades);
			if (num <= 0)
			{
				return 9999999;
			}
			return num;
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x0016D92D File Offset: 0x0016BB2D
		public static int GetMaxCurrencyObjects(CurrencyType type)
		{
			return Gameplay.Get().maxCurrencyObjects[(int)type];
		}

		// Token: 0x04004ECC RID: 20172
		[Header("System")]
		[Space]
		[SerializeField]
		private int toolReplenishCost;

		// Token: 0x04004ECD RID: 20173
		[SerializeField]
		private float toolPouchUpgradeIncrease;

		// Token: 0x04004ECE RID: 20174
		[SerializeField]
		private float toolKitDamageIncrease;

		// Token: 0x04004ECF RID: 20175
		[Space]
		[SerializeField]
		private float toolCameraDistanceBreak;

		// Token: 0x04004ED0 RID: 20176
		[Space]
		[SerializeField]
		[ArrayForEnum(typeof(CurrencyType))]
		private int[] currencyCaps;

		// Token: 0x04004ED1 RID: 20177
		[SerializeField]
		private int consumableItemCap;

		// Token: 0x04004ED2 RID: 20178
		[Space]
		[SerializeField]
		private CostReference smallGeoValue;

		// Token: 0x04004ED3 RID: 20179
		[SerializeField]
		private CostReference mediumGeoValue;

		// Token: 0x04004ED4 RID: 20180
		[SerializeField]
		private CostReference largeGeoValue;

		// Token: 0x04004ED5 RID: 20181
		[Space]
		[SerializeField]
		private GameObject smallGeoPrefab;

		// Token: 0x04004ED6 RID: 20182
		[SerializeField]
		private GameObject mediumGeoPrefab;

		// Token: 0x04004ED7 RID: 20183
		[SerializeField]
		private GameObject largeGeoPrefab;

		// Token: 0x04004ED8 RID: 20184
		[SerializeField]
		private GameObject largeSmoothGeoPrefab;

		// Token: 0x04004ED9 RID: 20185
		[Space]
		[SerializeField]
		private List<GameObject> shellShardPrefabs = new List<GameObject>();

		// Token: 0x04004EDA RID: 20186
		[Space]
		[SerializeField]
		private GameObject shellShardMidPrefab;

		// Token: 0x04004EDB RID: 20187
		[Space]
		[SerializeField]
		[ArrayForEnum(typeof(CurrencyType))]
		private int[] maxCurrencyObjects;

		// Token: 0x04004EDC RID: 20188
		[Space]
		[SerializeField]
		private CollectableItemPickup collectableItemPickupPrefab;

		// Token: 0x04004EDD RID: 20189
		[SerializeField]
		private CollectableItemPickup collectableItemPickupInstantPrefab;

		// Token: 0x04004EDE RID: 20190
		[SerializeField]
		private GenericPickup genericPickupPrefab;

		// Token: 0x04004EDF RID: 20191
		[SerializeField]
		private CollectableItemPickup collectableItemPickupMeatPrefab;

		// Token: 0x04004EE0 RID: 20192
		[Space]
		[SerializeField]
		private GameObject reaperBundlePrefab;

		// Token: 0x04004EE1 RID: 20193
		[Space]
		[SerializeField]
		private FsmTemplate hornetMultiWounderFsmTemplate;

		// Token: 0x04004EE2 RID: 20194
		[Space]
		[SerializeField]
		private CollectableItemMementoList mementoList;

		// Token: 0x04004EE3 RID: 20195
		[Header("Crest Config (for reference in code)")]
		[Space]
		[SerializeField]
		private ToolCrest hunterCrest;

		// Token: 0x04004EE4 RID: 20196
		[SerializeField]
		private ToolCrest hunterCrest2;

		// Token: 0x04004EE5 RID: 20197
		[SerializeField]
		private int hunterComboHits;

		// Token: 0x04004EE6 RID: 20198
		[SerializeField]
		private float hunterComboDamageMult;

		// Token: 0x04004EE7 RID: 20199
		[SerializeField]
		private ToolCrest hunterCrest3;

		// Token: 0x04004EE8 RID: 20200
		[SerializeField]
		private int hunterCombo2Hits;

		// Token: 0x04004EE9 RID: 20201
		[SerializeField]
		private float hunterCombo2DamageMult;

		// Token: 0x04004EEA RID: 20202
		[SerializeField]
		private GameObject hunterComboDamageEffect;

		// Token: 0x04004EEB RID: 20203
		[SerializeField]
		private int hunterCombo2ExtraHits;

		// Token: 0x04004EEC RID: 20204
		[SerializeField]
		private float hunterCombo2ExtraDamageMult;

		// Token: 0x04004EED RID: 20205
		[SerializeField]
		private Vector2 hunterCombo2DamageExtraScale;

		// Token: 0x04004EEE RID: 20206
		[Space]
		[SerializeField]
		private ToolCrest warriorCrest;

		// Token: 0x04004EEF RID: 20207
		[SerializeField]
		private float warriorRageDuration;

		// Token: 0x04004EF0 RID: 20208
		[SerializeField]
		private float[] warriorRageHitAddTimePerHit;

		// Token: 0x04004EF1 RID: 20209
		[SerializeField]
		private float warriorDamageMultiplier;

		// Token: 0x04004EF2 RID: 20210
		[SerializeField]
		private float warriorRageDamagedRemoveTime;

		// Token: 0x04004EF3 RID: 20211
		[Space]
		[SerializeField]
		private ToolCrest reaperCrest;

		// Token: 0x04004EF4 RID: 20212
		[SerializeField]
		private float reaperModeDuration;

		// Token: 0x04004EF5 RID: 20213
		[Space]
		[SerializeField]
		private ToolCrest wandererCrest;

		// Token: 0x04004EF6 RID: 20214
		[SerializeField]
		[Range(0f, 1f)]
		private float wandererCritChance = 1f;

		// Token: 0x04004EF7 RID: 20215
		[SerializeField]
		private float wandererCritMultiplier = 2f;

		// Token: 0x04004EF8 RID: 20216
		[SerializeField]
		private float wandererCritMagnitudeMult = 2f;

		// Token: 0x04004EF9 RID: 20217
		[SerializeField]
		private GameObject wandererCritEffect;

		// Token: 0x04004EFA RID: 20218
		[Space]
		[SerializeField]
		private ToolCrest cursedCrest;

		// Token: 0x04004EFB RID: 20219
		[SerializeField]
		private ToolCrest witchCrest;

		// Token: 0x04004EFC RID: 20220
		[SerializeField]
		private int witchCrestRecoilSteps = 2;

		// Token: 0x04004EFD RID: 20221
		[Space]
		[SerializeField]
		private ToolCrest toolmasterCrest;

		// Token: 0x04004EFE RID: 20222
		[SerializeField]
		private int toolmasterQuickCraftNoneUsage;

		// Token: 0x04004EFF RID: 20223
		[Space]
		[SerializeField]
		private ToolCrest spellCrest;

		// Token: 0x04004F00 RID: 20224
		[SerializeField]
		private float spellCrestRuneDamageMult;

		// Token: 0x04004F01 RID: 20225
		[Space]
		[SerializeField]
		private ToolCrest cloaklessCrest;

		// Token: 0x04004F02 RID: 20226
		[Header("Tool Config (for reference in code)")]
		[Space]
		[SerializeField]
		private ToolItem defaultSkillTool;

		// Token: 0x04004F03 RID: 20227
		[Space]
		[SerializeField]
		private ToolItem luckyDiceTool;

		// Token: 0x04004F04 RID: 20228
		[Space]
		[SerializeField]
		private ToolItem barbedWireTool;

		// Token: 0x04004F05 RID: 20229
		[SerializeField]
		private float barbedWireDamageDealtMultiplier = 1.5f;

		// Token: 0x04004F06 RID: 20230
		[SerializeField]
		private float barbedWireDamageTakenMultiplier = 2f;

		// Token: 0x04004F07 RID: 20231
		[Space]
		[SerializeField]
		private ToolItem weightedAnkletTool;

		// Token: 0x04004F08 RID: 20232
		[SerializeField]
		private float weightedAnkletDmgKnockbackMult = 0.5f;

		// Token: 0x04004F09 RID: 20233
		[SerializeField]
		private float weightedAnkletDmgInvulnMult = 1.2f;

		// Token: 0x04004F0A RID: 20234
		[SerializeField]
		private int weightedAnkletRecoilSteps = 2;

		// Token: 0x04004F0B RID: 20235
		[Space]
		[SerializeField]
		private ToolItem boneNecklaceTool;

		// Token: 0x04004F0C RID: 20236
		[SerializeField]
		private float boneNecklaceShellshardIncrease = 0.2f;

		// Token: 0x04004F0D RID: 20237
		[Space]
		[SerializeField]
		private ToolItem longNeedleTool;

		// Token: 0x04004F0E RID: 20238
		[SerializeField]
		private Vector2 longNeedleMultiplier = new Vector2(1.25f, 1.25f);

		// Token: 0x04004F0F RID: 20239
		[Space]
		[SerializeField]
		private ToolItem deadPurseTool;

		// Token: 0x04004F10 RID: 20240
		[SerializeField]
		[Range(0f, 1f)]
		private float deadPurseHoldPercent;

		// Token: 0x04004F11 RID: 20241
		[Space]
		[SerializeField]
		private ToolItem shellSatchelTool;

		// Token: 0x04004F12 RID: 20242
		[SerializeField]
		private float shellSatchelToolIncrease;

		// Token: 0x04004F13 RID: 20243
		[Space]
		[SerializeField]
		private ToolItem spoolExtenderTool;

		// Token: 0x04004F14 RID: 20244
		[SerializeField]
		private int spoolExtenderSilk;

		// Token: 0x04004F15 RID: 20245
		[Space]
		[SerializeField]
		private ToolItem whiteRingTool;

		// Token: 0x04004F16 RID: 20246
		[SerializeField]
		private float whiteRingSilkRegenTimeMultiplier;

		// Token: 0x04004F17 RID: 20247
		[SerializeField]
		private int whiteRingSilkRegenIncrease;

		// Token: 0x04004F18 RID: 20248
		[Space]
		[SerializeField]
		private ToolItem lavaBellTool;

		// Token: 0x04004F19 RID: 20249
		[SerializeField]
		private float lavaBellCooldownTime;

		// Token: 0x04004F1A RID: 20250
		[Space]
		[SerializeField]
		private ToolItem thiefCharmTool;

		// Token: 0x04004F1B RID: 20251
		[SerializeField]
		private float thiefCharmGeoSmallIncrease;

		// Token: 0x04004F1C RID: 20252
		[SerializeField]
		private float thiefCharmGeoMedIncrease;

		// Token: 0x04004F1D RID: 20253
		[SerializeField]
		private float thiefCharmGeoLargeIncrease;

		// Token: 0x04004F1E RID: 20254
		[SerializeField]
		private MinMaxFloat thiefCharmGeoLoss;

		// Token: 0x04004F1F RID: 20255
		[SerializeField]
		private MinMaxInt thiefCharmGeoLossCap;

		// Token: 0x04004F20 RID: 20256
		[SerializeField]
		private float thiefCharmGeoLossLooseChance;

		// Token: 0x04004F21 RID: 20257
		[SerializeField]
		private MinMaxInt thiefCharmGeoLossLooseAmount;

		// Token: 0x04004F22 RID: 20258
		[SerializeField]
		private GameObject thiefCharmHeroHitPrefab;

		// Token: 0x04004F23 RID: 20259
		[SerializeField]
		private AudioEvent thiefCharmEnemyDeathAudio;

		// Token: 0x04004F24 RID: 20260
		[Space]
		[SerializeField]
		private ToolItem thiefPickTool;

		// Token: 0x04004F25 RID: 20261
		[SerializeField]
		private MinMaxFloat thiefPickGeoSteal;

		// Token: 0x04004F26 RID: 20262
		[SerializeField]
		private MinMaxInt thiefPickGeoStealMin;

		// Token: 0x04004F27 RID: 20263
		[SerializeField]
		private MinMaxInt thiefPickShardSteal;

		// Token: 0x04004F28 RID: 20264
		[SerializeField]
		private ThiefSnatchEffect thiefSnatchEffectPrefab;

		// Token: 0x04004F29 RID: 20265
		[SerializeField]
		private float thiefPickLooseChance;

		// Token: 0x04004F2A RID: 20266
		[SerializeField]
		private MinMaxInt thiefPickGeoLoose;

		// Token: 0x04004F2B RID: 20267
		[SerializeField]
		private MinMaxInt thiefPickShardLoose;

		// Token: 0x04004F2C RID: 20268
		[Space]
		[SerializeField]
		private ToolItem maggotCharm;

		// Token: 0x04004F2D RID: 20269
		[SerializeField]
		private float maggotCharmHealthLossTime;

		// Token: 0x04004F2E RID: 20270
		[SerializeField]
		private float maggotCharmEnterWaterAddTime;

		// Token: 0x04004F2F RID: 20271
		[SerializeField]
		private GameObject maggotCharmHitSinglePrefab;

		// Token: 0x04004F30 RID: 20272
		[Space]
		[SerializeField]
		private ToolItem poisonPouchTool;

		// Token: 0x04004F31 RID: 20273
		[SerializeField]
		private DamageTag poisonPouchDamageTag;

		// Token: 0x04004F32 RID: 20274
		[SerializeField]
		private Color poisonPouchTintColour;

		// Token: 0x04004F33 RID: 20275
		[SerializeField]
		private Color poisonPouchHeroTintColour;

		// Token: 0x04004F34 RID: 20276
		[Space]
		[SerializeField]
		private ToolItem zapImbuementTool;

		// Token: 0x04004F35 RID: 20277
		[SerializeField]
		private DamageTag zapDamageTag;

		// Token: 0x04004F36 RID: 20278
		[SerializeField]
		private Color zapDamageTintColour;

		// Token: 0x04004F37 RID: 20279
		[SerializeField]
		private float zapDamageMult;

		// Token: 0x04004F38 RID: 20280
		[Space]
		[SerializeField]
		private ToolItem musicianCharmTool;

		// Token: 0x04004F39 RID: 20281
		[SerializeField]
		private float musicianCharmSilkDrainTimeMult;

		// Token: 0x04004F3A RID: 20282
		[SerializeField]
		private float musicianCharmNeedolinRangeMult;

		// Token: 0x04004F3B RID: 20283
		[SerializeField]
		private float musicianCharmFovOffset;

		// Token: 0x04004F3C RID: 20284
		[SerializeField]
		private float musicianCharmFovStartDuration;

		// Token: 0x04004F3D RID: 20285
		[SerializeField]
		private AnimationCurve musicianCharmFovStartCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04004F3E RID: 20286
		[SerializeField]
		private float musicianCharmFovEndDuration;

		// Token: 0x04004F3F RID: 20287
		[SerializeField]
		private AnimationCurve musicianCharmFovEndCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04004F40 RID: 20288
		[Space]
		[SerializeField]
		private ToolItem scuttleCharmTool;

		// Token: 0x04004F41 RID: 20289
		[SerializeField]
		private GameObject scuttleEvadeEffect;

		// Token: 0x04004F42 RID: 20290
		[Space]
		[SerializeField]
		private ToolItem brollySpikeTool;

		// Token: 0x04004F43 RID: 20291
		[SerializeField]
		private GameObject brollySpikeObject_dj;

		// Token: 0x04004F44 RID: 20292
		[Space]
		[SerializeField]
		private ToolItem compassTool;

		// Token: 0x04004F45 RID: 20293
		[SerializeField]
		private ToolItem mossCreep1Tool;

		// Token: 0x04004F46 RID: 20294
		[SerializeField]
		private ToolItem mossCreep2Tool;

		// Token: 0x04004F47 RID: 20295
		[SerializeField]
		private ToolItem fracturedMaskTool;

		// Token: 0x04004F48 RID: 20296
		[SerializeField]
		private ToolItem curveclawTool;

		// Token: 0x04004F49 RID: 20297
		[SerializeField]
		private ToolItem curveclawUpgradedTool;

		// Token: 0x04004F4A RID: 20298
		[SerializeField]
		private ToolItem sprintmasterTool;

		// Token: 0x04004F4B RID: 20299
		[SerializeField]
		private ToolItem multibindTool;

		// Token: 0x04004F4C RID: 20300
		[SerializeField]
		private ToolItem quickSlingTool;

		// Token: 0x04004F4D RID: 20301
		[SerializeField]
		private ToolItem wallClingTool;

		// Token: 0x04004F4E RID: 20302
		[SerializeField]
		private ToolItem wispLanternTool;

		// Token: 0x04004F4F RID: 20303
		[SerializeField]
		private ToolItem fleaCharmTool;

		// Token: 0x04004F50 RID: 20304
		[Header("World")]
		[SerializeField]
		private FullQuestBase huntressQuest;

		// Token: 0x04004F51 RID: 20305
		[SerializeField]
		private ShopItemList mapperStock;

		// Token: 0x04004F52 RID: 20306
		[SerializeField]
		private QuestTargetPlayerDataBools fleasCollectedCounter;

		// Token: 0x04004F53 RID: 20307
		[SerializeField]
		private QuestBoardList enclaveQuestBoard;
	}
}
