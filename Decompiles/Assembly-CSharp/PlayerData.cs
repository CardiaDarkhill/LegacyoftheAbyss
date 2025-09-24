using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using GlobalEnums;
using GlobalSettings;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000477 RID: 1143
[JsonObject(MemberSerialization.Fields)]
[Serializable]
public class PlayerData : PlayerDataBase
{
	// Token: 0x170004C1 RID: 1217
	// (get) Token: 0x06002911 RID: 10513 RVA: 0x000B3720 File Offset: 0x000B1920
	public bool IsDemoMode
	{
		get
		{
			return DemoHelper.IsDemoMode;
		}
	}

	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x06002912 RID: 10514 RVA: 0x000B3727 File Offset: 0x000B1927
	public bool IsExhibitionMode
	{
		get
		{
			return DemoHelper.IsExhibitionMode;
		}
	}

	// Token: 0x170004C3 RID: 1219
	// (get) Token: 0x06002913 RID: 10515 RVA: 0x000B372E File Offset: 0x000B192E
	public bool IsHornetStrengthRegained
	{
		get
		{
			return this.maxHealthBase > 5 || this.silkMax > 9 || this.silkRegenMax > 0;
		}
	}

	// Token: 0x170004C4 RID: 1220
	// (get) Token: 0x06002914 RID: 10516 RVA: 0x000B3750 File Offset: 0x000B1950
	public int nailDamage
	{
		get
		{
			int result;
			switch (this.nailUpgrades)
			{
			case 0:
				result = 5;
				break;
			case 1:
				result = 9;
				break;
			case 2:
				result = 13;
				break;
			case 3:
				result = 17;
				break;
			default:
				result = 21;
				break;
			}
			return result;
		}
	}

	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06002915 RID: 10517 RVA: 0x000B3793 File Offset: 0x000B1993
	private PlayerData.MapBoolList MapBools
	{
		get
		{
			if (this.mapBoolList == null)
			{
				this.mapBoolList = new PlayerData.MapBoolList(this);
			}
			return this.mapBoolList;
		}
	}

	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06002916 RID: 10518 RVA: 0x000B37AF File Offset: 0x000B19AF
	public bool HasAnyMap
	{
		get
		{
			return this.mapAllRooms || this.MapBools.HasAnyMap;
		}
	}

	// Token: 0x170004C7 RID: 1223
	// (get) Token: 0x06002917 RID: 10519 RVA: 0x000B37C6 File Offset: 0x000B19C6
	public bool HasAllMaps
	{
		get
		{
			return this.mapAllRooms || this.MapBools.HasAllMaps;
		}
	}

	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x06002918 RID: 10520 RVA: 0x000B37DD File Offset: 0x000B19DD
	public int MapCount
	{
		get
		{
			if (this.mapAllRooms)
			{
				return 28;
			}
			return this.mapBoolList.HasCount;
		}
	}

	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x06002919 RID: 10521 RVA: 0x000B37F5 File Offset: 0x000B19F5
	public bool CanUpdateMap
	{
		get
		{
			return this.hasQuill && this.HasAnyMap;
		}
	}

	// Token: 0x170004CA RID: 1226
	// (get) Token: 0x0600291A RID: 10522 RVA: 0x000B3807 File Offset: 0x000B1A07
	public bool IsWildsWideMapFull
	{
		get
		{
			return this.scenesVisited.Contains("Bone_East_24");
		}
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x0600291B RID: 10523 RVA: 0x000B3819 File Offset: 0x000B1A19
	public bool HasAnyFleaPin
	{
		get
		{
			return this.hasPinFleaMarrowlands || this.hasPinFleaMidlands || this.hasPinFleaBlastedlands || this.hasPinFleaCitadel || this.hasPinFleaPeaklands || this.hasPinFleaMucklands;
		}
	}

	// Token: 0x170004CC RID: 1228
	// (get) Token: 0x0600291C RID: 10524 RVA: 0x000B384C File Offset: 0x000B1A4C
	public bool IsFleaPinMapKeyVisible
	{
		get
		{
			if (!this.HasAnyFleaPin)
			{
				return false;
			}
			if (!this.CaravanLechSaved || !this.tamedGiantFlea)
			{
				return true;
			}
			this.CacheSavedFleas();
			FieldInfo[] savedFleaFields = PlayerData._savedFleaFields;
			for (int i = 0; i < savedFleaFields.Length; i++)
			{
				if (!(bool)savedFleaFields[i].GetValue(this))
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x170004CD RID: 1229
	// (get) Token: 0x0600291D RID: 10525 RVA: 0x000B38A2 File Offset: 0x000B1AA2
	public int SavedFleasCount
	{
		get
		{
			this.CacheSavedFleas();
			return PlayerData._savedFleaFields.Count((FieldInfo fieldInfo) => (bool)fieldInfo.GetValue(this));
		}
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x000B38C0 File Offset: 0x000B1AC0
	private void CacheSavedFleas()
	{
		if (PlayerData._savedFleaFields == null)
		{
			PlayerData._savedFleaFields = (from fieldInfo in base.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
			where fieldInfo.FieldType == typeof(bool) && fieldInfo.Name.StartsWith("SavedFlea_")
			select fieldInfo).ToArray<FieldInfo>();
		}
	}

	// Token: 0x170004CE RID: 1230
	// (get) Token: 0x0600291F RID: 10527 RVA: 0x000B390F File Offset: 0x000B1B0F
	public bool HasAnyPin
	{
		get
		{
			return this.hasPinBench || this.hasPinStag || this.hasPinShop || this.hasPinTube || this.IsFleaPinMapKeyVisible;
		}
	}

	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x06002920 RID: 10528 RVA: 0x000B3939 File Offset: 0x000B1B39
	public bool VampireGnatBossInAltLoc
	{
		get
		{
			return this.allowVampireGnatInAltLoc && (this.CaravanTroupeLocation == CaravanTroupeLocations.Greymoor || this.visitedBellhart || this.defeatedLastJudge) && this.visitedCitadel;
		}
	}

	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x06002921 RID: 10529 RVA: 0x000B3964 File Offset: 0x000B1B64
	public bool CaravanInGreymoor
	{
		get
		{
			return this.CaravanTroupeLocation == CaravanTroupeLocations.Greymoor;
		}
	}

	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x06002922 RID: 10530 RVA: 0x000B396F File Offset: 0x000B1B6F
	public bool HasLifebloodSyringeGland
	{
		get
		{
			return this.Collectables.GetData("Plasmium Gland").Amount > 0;
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x06002923 RID: 10531 RVA: 0x000B3989 File Offset: 0x000B1B89
	public bool GourmandQuestAccepted
	{
		get
		{
			return QuestManager.GetQuest("Great Gourmand").IsAccepted;
		}
	}

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x06002924 RID: 10532 RVA: 0x000B399A File Offset: 0x000B1B9A
	public bool BelltownHouseVisited
	{
		get
		{
			return this.scenesVisited.Contains("Belltown_Room_Spare");
		}
	}

	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x06002925 RID: 10533 RVA: 0x000B39AC File Offset: 0x000B1BAC
	public bool CrawbellHasSomething
	{
		get
		{
			if (this.CrawbellCurrency == null)
			{
				return false;
			}
			int[] crawbellCurrency = this.CrawbellCurrency;
			for (int i = 0; i < crawbellCurrency.Length; i++)
			{
				if (crawbellCurrency[i] > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x06002926 RID: 10534 RVA: 0x000B39E4 File Offset: 0x000B1BE4
	public bool HasAnyMemento
	{
		get
		{
			foreach (CollectableItemMemento collectableItemMemento in Gameplay.MementoList)
			{
				if (collectableItemMemento && collectableItemMemento.CollectedAmount > 0)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x06002927 RID: 10535 RVA: 0x000B3A44 File Offset: 0x000B1C44
	public bool CloakFlySmell
	{
		get
		{
			return this.cloakOdour_slabFly > 0;
		}
	}

	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x06002928 RID: 10536 RVA: 0x000B3A4F File Offset: 0x000B1C4F
	public bool IsAnyRelicsDeposited
	{
		get
		{
			return this.Relics.IsAnyMatching((CollectableRelicsData.Data relic) => relic.IsDeposited);
		}
	}

	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x06002929 RID: 10537 RVA: 0x000B3A7B File Offset: 0x000B1C7B
	public bool WillLoadWardBoss
	{
		get
		{
			return !this.wardBossDefeated && (this.collectedWardBossKey || this.wardBossHatchOpened);
		}
	}

	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x0600292A RID: 10538 RVA: 0x000B3A98 File Offset: 0x000B1C98
	public int CollectedCloverMemoryOrbs
	{
		get
		{
			return Convert.ToInt32(this.memoryOrbs_Clover_02c_A) + Convert.ToInt32(this.memoryOrbs_Clover_03_B) + Convert.ToInt32(this.memoryOrbs_Clover_06_A) + Convert.ToInt32(this.memoryOrbs_Clover_11) + Convert.ToInt32(this.memoryOrbs_Clover_16_B) + Convert.ToInt32(this.memoryOrbs_Clover_16_C) + Convert.ToInt32(this.memoryOrbs_Clover_21) + this.memoryOrbs_Clover_18_A.CountSetBits() + this.memoryOrbs_Clover_18_B.CountSetBits() + this.memoryOrbs_Clover_18_C.CountSetBits() + this.memoryOrbs_Clover_18_D.CountSetBits() + this.memoryOrbs_Clover_18_E.CountSetBits() + this.memoryOrbs_Clover_19.CountSetBits();
		}
	}

	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x0600292B RID: 10539 RVA: 0x000B3B40 File Offset: 0x000B1D40
	public bool CloverMemoryOrbsCollectedAll
	{
		get
		{
			return this.CollectedCloverMemoryOrbs >= 17;
		}
	}

	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x0600292C RID: 10540 RVA: 0x000B3B4F File Offset: 0x000B1D4F
	public bool CloverMemoryOrbsCollectedTarget
	{
		get
		{
			return this.CollectedCloverMemoryOrbs >= 12;
		}
	}

	// Token: 0x170004DC RID: 1244
	// (get) Token: 0x0600292D RID: 10541 RVA: 0x000B3B5E File Offset: 0x000B1D5E
	public bool IsAct3IntroQueued
	{
		get
		{
			return this.blackThreadWorld && !this.act3_wokeUp;
		}
	}

	// Token: 0x170004DD RID: 1245
	// (get) Token: 0x0600292E RID: 10542 RVA: 0x000B3B73 File Offset: 0x000B1D73
	public bool HasWhiteFlower
	{
		get
		{
			return this.Collectables.GetData("White Flower").Amount > 0;
		}
	}

	// Token: 0x170004DE RID: 1246
	// (get) Token: 0x0600292F RID: 10543 RVA: 0x000B3B8D File Offset: 0x000B1D8D
	public bool FleaGamesIsJugglingChampion
	{
		get
		{
			return this.fleaGames_juggling_highscore > 30;
		}
	}

	// Token: 0x170004DF RID: 1247
	// (get) Token: 0x06002930 RID: 10544 RVA: 0x000B3B99 File Offset: 0x000B1D99
	public bool FleaGamesIsJugglingSethChampion
	{
		get
		{
			return this.fleaGames_juggling_highscore > 55;
		}
	}

	// Token: 0x170004E0 RID: 1248
	// (get) Token: 0x06002931 RID: 10545 RVA: 0x000B3BA5 File Offset: 0x000B1DA5
	public bool FleaGamesIsBouncingChampion
	{
		get
		{
			return this.fleaGames_bouncing_highscore > 42;
		}
	}

	// Token: 0x170004E1 RID: 1249
	// (get) Token: 0x06002932 RID: 10546 RVA: 0x000B3BB1 File Offset: 0x000B1DB1
	public bool FleaGamesIsBouncingSethChampion
	{
		get
		{
			return this.fleaGames_bouncing_highscore > 68;
		}
	}

	// Token: 0x170004E2 RID: 1250
	// (get) Token: 0x06002933 RID: 10547 RVA: 0x000B3BBD File Offset: 0x000B1DBD
	public bool FleaGamesIsDodgingChampion
	{
		get
		{
			return this.fleaGames_dodging_highscore > 65;
		}
	}

	// Token: 0x170004E3 RID: 1251
	// (get) Token: 0x06002934 RID: 10548 RVA: 0x000B3BC9 File Offset: 0x000B1DC9
	public bool FleaGamesIsDodgingSethChampion
	{
		get
		{
			return this.fleaGames_dodging_highscore > 95;
		}
	}

	// Token: 0x170004E4 RID: 1252
	// (get) Token: 0x06002935 RID: 10549 RVA: 0x000B3BD5 File Offset: 0x000B1DD5
	public bool FleaGamesOutroReady
	{
		get
		{
			return this.FleaGamesIsJugglingChampion && this.FleaGamesIsBouncingChampion && this.FleaGamesIsDodgingChampion;
		}
	}

	// Token: 0x170004E5 RID: 1253
	// (get) Token: 0x06002936 RID: 10550 RVA: 0x000B3BEF File Offset: 0x000B1DEF
	public bool FleaGamesBestedSeth
	{
		get
		{
			return this.FleaGamesIsJugglingSethChampion && this.FleaGamesIsBouncingSethChampion && this.FleaGamesIsDodgingSethChampion;
		}
	}

	// Token: 0x170004E6 RID: 1254
	// (get) Token: 0x06002937 RID: 10551 RVA: 0x000B3C09 File Offset: 0x000B1E09
	public bool BellCentipedeWaiting
	{
		get
		{
			return this.blackThreadWorld && !this.UnlockedFastTravelTeleport;
		}
	}

	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x06002938 RID: 10552 RVA: 0x000B3C1E File Offset: 0x000B1E1E
	public bool BellCentipedeLocked
	{
		get
		{
			return this.bellCentipedeAppeared && !this.UnlockedFastTravelTeleport;
		}
	}

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x06002939 RID: 10553 RVA: 0x000B3C33 File Offset: 0x000B1E33
	public bool UnlockedAnyTube
	{
		get
		{
			return this.UnlockedArboriumTube || this.UnlockedEnclaveTube || this.UnlockedHangTube || this.UnlockedSongTube || this.UnlockedUnderTube || this.UnlockedCityBellwayTube;
		}
	}

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x0600293A RID: 10554 RVA: 0x000B3C65 File Offset: 0x000B1E65
	public int CurrentMaxHealth
	{
		get
		{
			if (BossSequenceController.BoundShell)
			{
				return Mathf.Min(this.maxHealth, BossSequenceController.BoundMaxHealth);
			}
			return this.maxHealth;
		}
	}

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x0600293B RID: 10555 RVA: 0x000B3C88 File Offset: 0x000B1E88
	public int CurrentSilkMax
	{
		get
		{
			int num = this.CurrentSilkMaxBasic;
			ToolItem spoolExtenderTool = Gameplay.SpoolExtenderTool;
			if (spoolExtenderTool && spoolExtenderTool.IsEquipped)
			{
				num += Gameplay.SpoolExtenderSilk;
			}
			return num;
		}
	}

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x0600293C RID: 10556 RVA: 0x000B3CBB File Offset: 0x000B1EBB
	public int CurrentSilkMaxBasic
	{
		get
		{
			if (this.UnlockSilkFinalCutscene)
			{
				return this.silk;
			}
			if (this.IsAnyCursed)
			{
				return 3;
			}
			if (this.IsSilkSpoolBroken)
			{
				return 9;
			}
			return this.silkMax;
		}
	}

	// Token: 0x170004EC RID: 1260
	// (get) Token: 0x0600293D RID: 10557 RVA: 0x000B3CE7 File Offset: 0x000B1EE7
	public int SilkSkillCost
	{
		get
		{
			if (!Gameplay.FleaCharmTool.IsEquippedHud || this.health < this.CurrentMaxHealth)
			{
				return 4;
			}
			return 3;
		}
	}

	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x0600293E RID: 10558 RVA: 0x000B3D06 File Offset: 0x000B1F06
	public bool IsAnyCursed
	{
		get
		{
			return this.CurrentCrestID == Gameplay.CursedCrest.name;
		}
	}

	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x0600293F RID: 10559 RVA: 0x000B3D1D File Offset: 0x000B1F1D
	public int CurrentSilkRegenMax
	{
		get
		{
			if (!Gameplay.WhiteRingTool.IsEquipped)
			{
				return this.silkRegenMax;
			}
			return this.silkRegenMax + Gameplay.WhiteRingSilkRegenIncrease;
		}
	}

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x06002940 RID: 10560 RVA: 0x000B3D3E File Offset: 0x000B1F3E
	public bool JournalIsCompleted
	{
		get
		{
			return EnemyJournalManager.IsAllRequiredComplete();
		}
	}

	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x06002941 RID: 10561 RVA: 0x000B3D45 File Offset: 0x000B1F45
	public int JournalCompletedCount
	{
		get
		{
			return EnemyJournalManager.GetCompletedEnemiesCount();
		}
	}

	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x06002942 RID: 10562 RVA: 0x000B3D4C File Offset: 0x000B1F4C
	public bool MushroomQuestCompleted
	{
		get
		{
			return this.QuestCompletionData.GetData("Mr Mushroom").IsCompleted;
		}
	}

	// Token: 0x170004F2 RID: 1266
	// (get) Token: 0x06002943 RID: 10563 RVA: 0x000B3D63 File Offset: 0x000B1F63
	// (set) Token: 0x06002944 RID: 10564 RVA: 0x000B3D78 File Offset: 0x000B1F78
	public static PlayerData instance
	{
		get
		{
			if (PlayerData._instance != null)
			{
				return PlayerData._instance;
			}
			return PlayerData.CreateNewSingleton(true);
		}
		set
		{
			PlayerData._instance = value;
		}
	}

	// Token: 0x170004F3 RID: 1267
	// (get) Token: 0x06002945 RID: 10565 RVA: 0x000B3D80 File Offset: 0x000B1F80
	public static bool HasInstance
	{
		get
		{
			return PlayerData._instance != null;
		}
	}

	// Token: 0x06002946 RID: 10566 RVA: 0x000B3D8A File Offset: 0x000B1F8A
	public PlayerData()
	{
		this.SetupNewPlayerData(false);
	}

	// Token: 0x06002947 RID: 10567 RVA: 0x000B3D99 File Offset: 0x000B1F99
	public static PlayerData CreateNewSingleton(bool addEditorOverrides)
	{
		PlayerData._instance = SaveDataUtility.DeserializeSaveData<PlayerData>("{}");
		PlayerData._instance.SetupNewPlayerData(addEditorOverrides);
		PlayerData._instance.SetupExistingPlayerData();
		return PlayerData._instance;
	}

	// Token: 0x06002948 RID: 10568 RVA: 0x000B3DC4 File Offset: 0x000B1FC4
	[OnDeserializing]
	private void OnDeserialized(StreamingContext context)
	{
		this.SetupNewPlayerData(false);
	}

	// Token: 0x06002949 RID: 10569 RVA: 0x000B3DCD File Offset: 0x000B1FCD
	public static void ClearOptimisers()
	{
		PlayerData.boolFieldAccessOptimizer = new BoolFieldAccessOptimizer<PlayerData>();
		PlayerData.intFieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, int>();
		PlayerData.floatFieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, float>();
		PlayerData.stringFieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, string>();
		PlayerData.vector3FieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, Vector3>();
	}

	// Token: 0x0600294A RID: 10570 RVA: 0x000B3E04 File Offset: 0x000B2004
	public void SetBool(string boolName, bool value)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			PlayerData.boolFieldAccessOptimizer.SetField(this, boolName, value);
			return;
		}
		FieldInfo field = base.GetType().GetField(boolName);
		if (field != null)
		{
			field.SetValue(PlayerData.instance, value);
		}
	}

	// Token: 0x0600294B RID: 10571 RVA: 0x000B3E50 File Offset: 0x000B2050
	public void SetInt(string intName, int value)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			PlayerData.intFieldAccessOptimiser.SetField(this, intName, value);
			return;
		}
		FieldInfo field = base.GetType().GetField(intName);
		if (field != null)
		{
			field.SetValue(PlayerData.instance, value);
		}
	}

	// Token: 0x0600294C RID: 10572 RVA: 0x000B3E9C File Offset: 0x000B209C
	public void IncrementInt(string intName)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			this.IntAdd(intName, 1);
			return;
		}
		FieldInfo field = base.GetType().GetField(intName);
		if (field != null)
		{
			int num = (int)field.GetValue(PlayerData.instance);
			field.SetValue(PlayerData.instance, num + 1);
		}
	}

	// Token: 0x0600294D RID: 10573 RVA: 0x000B3EF4 File Offset: 0x000B20F4
	public void IntAdd(string intName, int amount)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			int field = PlayerData.intFieldAccessOptimiser.GetField(this, intName);
			PlayerData.intFieldAccessOptimiser.SetField(this, intName, field + amount);
			return;
		}
		FieldInfo field2 = base.GetType().GetField(intName);
		if (field2 != null)
		{
			int num = (int)field2.GetValue(PlayerData.instance);
			field2.SetValue(PlayerData.instance, num + amount);
		}
	}

	// Token: 0x0600294E RID: 10574 RVA: 0x000B3F60 File Offset: 0x000B2160
	public void SetFloat(string floatName, float value)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			PlayerData.floatFieldAccessOptimiser.SetField(this, floatName, value);
			return;
		}
		FieldInfo field = base.GetType().GetField(floatName);
		if (field != null)
		{
			field.SetValue(PlayerData.instance, value);
		}
	}

	// Token: 0x0600294F RID: 10575 RVA: 0x000B3FAC File Offset: 0x000B21AC
	public void DecrementInt(string intName)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			this.IntAdd(intName, -1);
			return;
		}
		FieldInfo field = base.GetType().GetField(intName);
		if (field == null)
		{
			return;
		}
		int num = (int)field.GetValue(PlayerData.instance);
		field.SetValue(PlayerData.instance, num - 1);
	}

	// Token: 0x06002950 RID: 10576 RVA: 0x000B4004 File Offset: 0x000B2204
	public bool GetBool(string boolName)
	{
		if (string.IsNullOrEmpty(boolName))
		{
			return false;
		}
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return PlayerData.boolFieldAccessOptimizer.GetField(this, boolName);
		}
		FieldInfo field = base.GetType().GetField(boolName);
		return field != null && (bool)field.GetValue(PlayerData.instance);
	}

	// Token: 0x06002951 RID: 10577 RVA: 0x000B4058 File Offset: 0x000B2258
	public int GetInt(string intName)
	{
		if (string.IsNullOrEmpty(intName))
		{
			return -9999;
		}
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return PlayerData.intFieldAccessOptimiser.GetField(this, intName);
		}
		FieldInfo field = base.GetType().GetField(intName);
		if (field != null)
		{
			return (int)field.GetValue(PlayerData.instance);
		}
		return -9999;
	}

	// Token: 0x06002952 RID: 10578 RVA: 0x000B40B4 File Offset: 0x000B22B4
	public float GetFloat(string floatName)
	{
		if (string.IsNullOrEmpty(floatName))
		{
			return -9999f;
		}
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return PlayerData.floatFieldAccessOptimiser.GetField(this, floatName);
		}
		FieldInfo field = base.GetType().GetField(floatName);
		if (field != null)
		{
			return (float)field.GetValue(PlayerData.instance);
		}
		return -9999f;
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x000B4110 File Offset: 0x000B2310
	public string GetString(string stringName)
	{
		if (string.IsNullOrEmpty(stringName))
		{
			return " ";
		}
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return PlayerData.stringFieldAccessOptimiser.GetField(this, stringName);
		}
		FieldInfo field = base.GetType().GetField(stringName);
		if (field != null)
		{
			return (string)field.GetValue(PlayerData.instance);
		}
		return " ";
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000B416C File Offset: 0x000B236C
	public void SetString(string stringName, string value)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			PlayerData.stringFieldAccessOptimiser.SetField(this, stringName, value);
			return;
		}
		FieldInfo field = base.GetType().GetField(stringName);
		if (field != null)
		{
			field.SetValue(PlayerData.instance, value);
		}
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x000B41B0 File Offset: 0x000B23B0
	public void SetVector3(string vectorName, Vector3 value)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			PlayerData.vector3FieldAccessOptimiser.SetField(this, vectorName, value);
			return;
		}
		FieldInfo field = base.GetType().GetField(vectorName);
		if (field != null)
		{
			field.SetValue(PlayerData.instance, value);
		}
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000B41FC File Offset: 0x000B23FC
	public Vector3 GetVector3(string vectorName)
	{
		if (CheatManager.UseFieldAccessOptimisers)
		{
			return PlayerData.vector3FieldAccessOptimiser.GetField(this, vectorName);
		}
		FieldInfo field = base.GetType().GetField(vectorName);
		if (field != null)
		{
			return (Vector3)field.GetValue(PlayerData.instance);
		}
		return Vector3.zero;
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000B4249 File Offset: 0x000B2449
	public int GetNextMossberryValue()
	{
		return this.mossBerryValueList[this.druidMossBerriesSold];
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000B4258 File Offset: 0x000B2458
	public int GetNextSilkGrubValue()
	{
		return this.GrubFarmerMimicValueList[this.GrubFarmerSilkGrubsSold % this.GrubFarmerMimicValueList.Length];
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000B4270 File Offset: 0x000B2470
	public void CaptureToolAmountsOverride()
	{
		this.toolAmountsOverride = new Dictionary<string, int>();
		foreach (KeyValuePair<string, ToolItemsData.Data> keyValuePair in this.Tools.Enumerate())
		{
			this.toolAmountsOverride[keyValuePair.Key] = keyValuePair.Value.AmountLeft;
		}
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x000B42E4 File Offset: 0x000B24E4
	public void ClearToolAmountsOverride()
	{
		if (this.toolAmountsOverride != null)
		{
			this.toolAmountsOverride = null;
			ToolItemManager.SendEquippedChangedEvent(true);
		}
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000B42FC File Offset: 0x000B24FC
	public ToolItemsData.Data GetToolData(string toolName)
	{
		ToolItemsData.Data data = this.Tools.GetData(toolName);
		int amountLeft;
		if (this.toolAmountsOverride != null && this.toolAmountsOverride.TryGetValue(toolName, out amountLeft))
		{
			data.AmountLeft = amountLeft;
		}
		return data;
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000B4338 File Offset: 0x000B2538
	public void SetToolData(string toolName, ToolItemsData.Data data)
	{
		if (this.toolAmountsOverride != null)
		{
			ToolItemsData.Data data2 = this.Tools.GetData(toolName);
			this.toolAmountsOverride[toolName] = data.AmountLeft;
			data.AmountLeft = data2.AmountLeft;
		}
		this.Tools.SetData(toolName, data);
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000B4388 File Offset: 0x000B2588
	public void AddHealth(int amount)
	{
		if (this.health + amount >= this.maxHealth)
		{
			this.health = this.maxHealth;
		}
		else
		{
			this.health += amount;
		}
		if (this.health >= this.CurrentMaxHealth)
		{
			this.health = this.maxHealth;
		}
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000B43DC File Offset: 0x000B25DC
	public void TakeHealth(int amount, bool hasBlueHealth, bool allowFracturedMaskBreak)
	{
		if (amount > 0 && this.health == this.maxHealth && this.health != this.CurrentMaxHealth)
		{
			this.health = this.CurrentMaxHealth;
		}
		this.damagedBlue = hasBlueHealth;
		if (!this.damagedBlue)
		{
			this.damagedPurple = false;
		}
		if (this.healthBlue > 0)
		{
			int num = amount - this.healthBlue;
			this.damagedBlue = true;
			this.damagedPurple = false;
			if (this.damagedBlue)
			{
				EventRegister.SendEvent("PURPLE HEALTH CHECK", null);
			}
			this.healthBlue -= amount;
			if (this.healthBlue < 0)
			{
				this.healthBlue = 0;
			}
			if (num > 0)
			{
				this.TakeHealth(num, true, allowFracturedMaskBreak);
				return;
			}
		}
		else
		{
			int num2 = this.health - amount;
			ToolItem fracturedMaskTool = Gameplay.FracturedMaskTool;
			if (num2 <= 0 && fracturedMaskTool && fracturedMaskTool.IsEquipped && fracturedMaskTool.SavedData.AmountLeft > 0)
			{
				if (allowFracturedMaskBreak)
				{
					ToolItemsData.Data savedData = fracturedMaskTool.SavedData;
					savedData.AmountLeft = 0;
					fracturedMaskTool.SavedData = savedData;
				}
				amount = this.health - 1;
			}
			if (this.health - amount <= 0)
			{
				this.health = ((CheatManager.Invincibility == CheatManager.InvincibilityStates.PreventDeath) ? 1 : 0);
				return;
			}
			this.health -= amount;
		}
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000B450B File Offset: 0x000B270B
	public void MaxHealth()
	{
		this.prevHealth = this.health;
		this.health = this.CurrentMaxHealth;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000B4525 File Offset: 0x000B2725
	public void ActivateTestingCheats()
	{
		this.AddGeo(50000);
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x000B4532 File Offset: 0x000B2732
	public void GetAllPowerups()
	{
		this.hasDash = true;
		this.hasBrolly = true;
		this.hasWalljump = true;
		this.hasDoubleJump = true;
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x000B4550 File Offset: 0x000B2750
	public void AddToMaxHealth(int amount)
	{
		this.maxHealthBase += amount;
		this.maxHealth += amount;
		this.prevHealth = this.health;
		this.health = this.maxHealth;
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x000B4588 File Offset: 0x000B2788
	public void AddGeo(int amount)
	{
		this.geo += amount;
		int currencyCap = Gameplay.GetCurrencyCap(CurrencyType.Money);
		if (this.geo > currencyCap)
		{
			this.geo = currencyCap;
		}
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x000B45BA File Offset: 0x000B27BA
	public void TakeGeo(int amount)
	{
		this.geo -= amount;
		if (this.geo < 0)
		{
			this.geo = 0;
		}
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000B45DC File Offset: 0x000B27DC
	public void AddShards(int amount)
	{
		this.ShellShards += amount;
		int currencyCap = Gameplay.GetCurrencyCap(CurrencyType.Shard);
		if (this.ShellShards > currencyCap)
		{
			this.ShellShards = currencyCap;
		}
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000B460E File Offset: 0x000B280E
	public void TakeShards(int amount)
	{
		this.ShellShards -= amount;
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000B461E File Offset: 0x000B281E
	public bool WouldDie(int damage)
	{
		return this.health - damage <= 0;
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000B4630 File Offset: 0x000B2830
	public bool AddSilk(int amount)
	{
		int num = this.silk;
		this.silk += amount;
		int currentSilkMax = this.CurrentSilkMax;
		if (this.silk >= currentSilkMax)
		{
			this.silkParts = 0;
			this.silk = currentSilkMax;
		}
		return this.silk > num;
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x000B4679 File Offset: 0x000B2879
	public void TakeSilk(int amount)
	{
		this.silk = Math.Max(this.silk - amount, 0);
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x000B468F File Offset: 0x000B288F
	public void ReduceOdours(int amount)
	{
		if (this.cloakOdour_slabFly > 0)
		{
			this.cloakOdour_slabFly -= amount;
			if (this.cloakOdour_slabFly < 0)
			{
				this.cloakOdour_slabFly = 0;
			}
		}
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x000B46B8 File Offset: 0x000B28B8
	public void EquipCharm(int charmNum)
	{
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x000B46BA File Offset: 0x000B28BA
	public void UnequipCharm(int charmNum)
	{
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x000B46BC File Offset: 0x000B28BC
	public void CalculateNotchesUsed()
	{
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x000B46C0 File Offset: 0x000B28C0
	public void SetBenchRespawn(RespawnMarker spawnMarker, string sceneName, int spawnType)
	{
		this.respawnMarkerName = spawnMarker.name;
		this.respawnScene = sceneName;
		this.respawnType = spawnType;
		if (spawnMarker.overrideMapZone.IsEnabled && spawnMarker.overrideMapZone.Value != MapZone.NONE)
		{
			GameManager.instance.SetOverrideMapZoneAsRespawn(spawnMarker.overrideMapZone.Value);
			return;
		}
		GameManager.instance.SetCurrentMapZoneAsRespawn();
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x000B4721 File Offset: 0x000B2921
	public void SetBenchRespawn(string spawnMarker, string sceneName, bool facingRight)
	{
		this.respawnMarkerName = spawnMarker;
		this.respawnScene = sceneName;
		GameManager.instance.SetCurrentMapZoneAsRespawn();
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x000B473B File Offset: 0x000B293B
	public void SetBenchRespawn(string spawnMarker, string sceneName, int spawnType, bool facingRight)
	{
		this.respawnMarkerName = spawnMarker;
		this.respawnScene = sceneName;
		this.respawnType = spawnType;
		GameManager.instance.SetCurrentMapZoneAsRespawn();
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x000B475C File Offset: 0x000B295C
	public void SetHazardRespawn(HazardRespawnMarker location)
	{
		this.hazardRespawnLocation = location.transform.position;
		this.hazardRespawnFacing = location.RespawnFacingDirection;
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x000B477B File Offset: 0x000B297B
	public void SetHazardRespawn(Vector3 position, bool facingRight)
	{
		this.hazardRespawnLocation = position;
		this.hazardRespawnFacing = (facingRight ? HazardRespawnMarker.FacingDirection.Right : HazardRespawnMarker.FacingDirection.Left);
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x000B4794 File Offset: 0x000B2994
	public void MapperLeaveAll()
	{
		this.MapperLeftBellhart = true;
		this.MapperLeftBoneForest = true;
		this.MapperLeftBonetown = true;
		this.MapperLeftCoralCaverns = true;
		this.MapperLeftCrawl = true;
		this.MapperLeftDocks = true;
		this.MapperLeftDustpens = true;
		this.MapperLeftGreymoor = true;
		this.MapperLeftHuntersNest = true;
		this.MapperLeftJudgeSteps = true;
		this.MapperLeftPeak = true;
		this.MapperLeftShadow = true;
		this.MapperLeftShellwood = true;
		this.MapperLeftWilds = true;
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x000B4804 File Offset: 0x000B2A04
	public void CountGameCompletion()
	{
		this.completionPercentage = 0f;
		this.completionPercentage += (float)ToolItemManager.GetCount(ToolItemManager.GetUnlockedTools(), null);
		this.completionPercentage += (float)(ToolItemManager.GetUnlockedCrestsCount() - 1);
		this.completionPercentage += (float)this.nailUpgrades;
		this.completionPercentage += (float)this.ToolKitUpgrades;
		this.completionPercentage += (float)this.ToolPouchUpgrades;
		this.completionPercentage += (float)this.silkRegenMax;
		if (this.hasNeedolin)
		{
			this.completionPercentage += 1f;
		}
		if (this.hasDash)
		{
			this.completionPercentage += 1f;
		}
		if (this.hasWalljump)
		{
			this.completionPercentage += 1f;
		}
		if (this.hasHarpoonDash)
		{
			this.completionPercentage += 1f;
		}
		if (this.hasSuperJump)
		{
			this.completionPercentage += 1f;
		}
		this.completionPercentage += (float)(this.maxHealthBase - 5);
		this.completionPercentage += (float)(this.silkMax - 9);
		if (this.hasChargeSlash)
		{
			this.completionPercentage += 1f;
		}
		if (this.HasBoundCrestUpgrader)
		{
			this.completionPercentage += 1f;
		}
		if (this.HasWhiteFlower)
		{
			this.completionPercentage += 1f;
		}
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x000B4998 File Offset: 0x000B2B98
	private void SetupNewPlayerData(bool addEditorOverrides)
	{
		this.ResetNonSerializableFields();
		this.scenesVisited = new HashSet<string>();
		this.scenesMapped = new HashSet<string>();
		this.scenesEncounteredBench = new HashSet<string>();
		this.scenesEncounteredCocoon = new HashSet<string>();
		this.ToolEquips = new ToolCrestsData();
		this.ToolEquips.SetData("Hunter", new ToolCrestsData.Data
		{
			IsUnlocked = true
		});
		this.ExtraToolEquips = new FloatingCrestSlotsData();
		this.Tools = new ToolItemsData();
		this.ToolLiquids = new ToolItemLiquidsData();
		this.EnemyJournalKillData = new EnemyJournalKillData();
		this.QuestCompletionData = new QuestCompletionData();
		this.QuestRumourData = new QuestRumourData();
		this.Collectables = new CollectableItemsData();
		this.Relics = new CollectableRelicsData();
		this.MementosDeposited = new CollectableMementosData();
		this.MateriumCollected = new MateriumItemsData();
		this.mossBerryValueList = Array.Empty<int>();
		this.GrubFarmerMimicValueList = Array.Empty<int>();
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x000B4A88 File Offset: 0x000B2C88
	public void SetupExistingPlayerData()
	{
		if (this.mossBerryValueList == null || this.mossBerryValueList.Length == 0)
		{
			this.mossBerryValueList = new int[]
			{
				1,
				2,
				3
			};
			this.mossBerryValueList.Shuffle<int>();
		}
		if (this.GrubFarmerMimicValueList == null || this.GrubFarmerMimicValueList.Length == 0)
		{
			this.GrubFarmerMimicValueList = new int[]
			{
				1,
				2,
				3
			};
			this.GrubFarmerMimicValueList.Shuffle<int>();
		}
		SteelSoulQuestSpot.Spot[] steelQuestSpots = this.SteelQuestSpots;
		if (steelQuestSpots == null || steelQuestSpots.Length != 3)
		{
			this.SteelQuestSpots = new SteelSoulQuestSpot.Spot[3];
			List<string> list = new List<string>
			{
				"Shellwood_26",
				"Bone_East_14",
				"Aspid_01"
			};
			List<string> list2 = new List<string>
			{
				"Hang_08",
				"Coral_28",
				"Aqueduct_05"
			};
			this.SteelQuestSpots[0] = new SteelSoulQuestSpot.Spot
			{
				SceneName = list.GetAndRemoveRandomElement<string>()
			};
			this.SteelQuestSpots[1] = new SteelSoulQuestSpot.Spot
			{
				SceneName = list2.GetAndRemoveRandomElement<string>()
			};
			List<string> list3 = new List<string>(list.Count + list2.Count);
			list3.AddRange(list);
			list3.AddRange(list2);
			this.SteelQuestSpots[2] = new SteelSoulQuestSpot.Spot
			{
				SceneName = list3.GetRandomElement<string>()
			};
		}
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x000B4BD4 File Offset: 0x000B2DD4
	public void ResetNonSerializableFields()
	{
		this.tempRespawnScene = null;
		this.tempRespawnMarker = null;
		this.tempRespawnType = -1;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000B4BEB File Offset: 0x000B2DEB
	public void ResetTempRespawn()
	{
		this.tempRespawnType = -1;
		this.tempRespawnMarker = null;
		this.tempRespawnScene = null;
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000B4C02 File Offset: 0x000B2E02
	public void ResetCutsceneBools()
	{
		this.disablePause = false;
		this.disableInventory = false;
		this.disableSaveQuit = false;
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000B4C19 File Offset: 0x000B2E19
	public void AddGGPlayerDataOverrides()
	{
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000B4C1B File Offset: 0x000B2E1B
	public override void OnUpdatedVariable(string variableName)
	{
		this.LastSetFieldName = variableName;
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000B4C24 File Offset: 0x000B2E24
	public static string GetDateString()
	{
		return DateTime.Now.ToString("yyyy/MM/dd");
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x000B4C44 File Offset: 0x000B2E44
	public void OnBeforeSave()
	{
		GameManager instance = GameManager.instance;
		bool flag = false;
		if (!this.slab_cloak_battle_completed && instance.GetSceneNameString() == "Slab_16" && this.CurrentCrestID != "Cloakless" && this.PreviousCrestID == "Cloakless")
		{
			string currentCrestID = this.CurrentCrestID;
			this.CurrentCrestID = this.PreviousCrestID;
			this.PreviousCrestID = currentCrestID;
			flag = true;
		}
		else if (this.IsCurrentCrestTemp)
		{
			string currentCrestID2 = this.CurrentCrestID;
			this.CurrentCrestID = this.PreviousCrestID;
			this.PreviousCrestID = string.Empty;
			this.IsCurrentCrestTemp = false;
			flag = true;
		}
		if (flag)
		{
			ToolItemManager.SendEquippedChangedEvent(true);
		}
		Platform.Current.UpdatePlayTime(this.playTime);
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000B4CFD File Offset: 0x000B2EFD
	public void UpdateDate()
	{
		this.date = PlayerData.GetDateString();
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000B4D0A File Offset: 0x000B2F0A
	private void AddEditorOverrides()
	{
	}

	// Token: 0x040024CC RID: 9420
	public string LastSetFieldName;

	// Token: 0x040024CD RID: 9421
	[DefaultValue("1.0.28324")]
	public string version;

	// Token: 0x040024CE RID: 9422
	[DefaultValue(28104)]
	public int RevisionBreak;

	// Token: 0x040024CF RID: 9423
	public string date;

	// Token: 0x040024D0 RID: 9424
	public int profileID;

	// Token: 0x040024D1 RID: 9425
	public float playTime;

	// Token: 0x040024D2 RID: 9426
	public bool openingCreditsPlayed;

	// Token: 0x040024D3 RID: 9427
	public PermadeathModes permadeathMode;

	// Token: 0x040024D4 RID: 9428
	public bool CollectedDockDemoKey;

	// Token: 0x040024D5 RID: 9429
	public HeroItemsState PreMemoryState;

	// Token: 0x040024D6 RID: 9430
	public bool HasStoredMemoryState;

	// Token: 0x040024D7 RID: 9431
	[DefaultValue(5)]
	public int health;

	// Token: 0x040024D8 RID: 9432
	[DefaultValue(5)]
	public int maxHealth;

	// Token: 0x040024D9 RID: 9433
	[DefaultValue(5)]
	public int maxHealthBase;

	// Token: 0x040024DA RID: 9434
	public int healthBlue;

	// Token: 0x040024DB RID: 9435
	[NonSerialized]
	public bool damagedBlue;

	// Token: 0x040024DC RID: 9436
	[NonSerialized]
	public bool damagedPurple;

	// Token: 0x040024DD RID: 9437
	[DefaultValue(5)]
	public int prevHealth;

	// Token: 0x040024DE RID: 9438
	public int heartPieces;

	// Token: 0x040024DF RID: 9439
	public bool SeenBindPrompt;

	// Token: 0x040024E0 RID: 9440
	public int geo;

	// Token: 0x040024E1 RID: 9441
	public int silk;

	// Token: 0x040024E2 RID: 9442
	[DefaultValue(9)]
	public int silkMax;

	// Token: 0x040024E3 RID: 9443
	public int silkRegenMax;

	// Token: 0x040024E4 RID: 9444
	[NonSerialized]
	public int silkParts;

	// Token: 0x040024E5 RID: 9445
	[DefaultValue(true)]
	public bool IsSilkSpoolBroken;

	// Token: 0x040024E6 RID: 9446
	[NonSerialized]
	public bool UnlockSilkFinalCutscene;

	// Token: 0x040024E7 RID: 9447
	public int silkSpoolParts;

	// Token: 0x040024E8 RID: 9448
	public bool atBench;

	// Token: 0x040024E9 RID: 9449
	[DefaultValue("Tut_01")]
	public string respawnScene;

	// Token: 0x040024EA RID: 9450
	[DefaultValue(MapZone.MOSS_CAVE)]
	public MapZone mapZone;

	// Token: 0x040024EB RID: 9451
	public ExtraRestZones extraRestZone;

	// Token: 0x040024EC RID: 9452
	[DefaultValue("Death Respawn Marker Init")]
	public string respawnMarkerName;

	// Token: 0x040024ED RID: 9453
	public int respawnType;

	// Token: 0x040024EE RID: 9454
	[NonSerialized]
	public string tempRespawnScene;

	// Token: 0x040024EF RID: 9455
	[NonSerialized]
	public string tempRespawnMarker;

	// Token: 0x040024F0 RID: 9456
	[NonSerialized]
	public int tempRespawnType;

	// Token: 0x040024F1 RID: 9457
	[NonSerialized]
	public string nonLethalRespawnScene;

	// Token: 0x040024F2 RID: 9458
	[NonSerialized]
	public string nonLethalRespawnMarker;

	// Token: 0x040024F3 RID: 9459
	[NonSerialized]
	public int nonLethalRespawnType;

	// Token: 0x040024F4 RID: 9460
	[NonSerialized]
	public Vector3 hazardRespawnLocation;

	// Token: 0x040024F5 RID: 9461
	public HazardRespawnMarker.FacingDirection hazardRespawnFacing;

	// Token: 0x040024F6 RID: 9462
	public string HeroCorpseScene;

	// Token: 0x040024F7 RID: 9463
	public Vector2 HeroDeathScenePos;

	// Token: 0x040024F8 RID: 9464
	public Vector2 HeroDeathSceneSize;

	// Token: 0x040024F9 RID: 9465
	public byte[] HeroCorpseMarkerGuid;

	// Token: 0x040024FA RID: 9466
	public HeroDeathCocoonTypes HeroCorpseType;

	// Token: 0x040024FB RID: 9467
	public int HeroCorpseMoneyPool;

	// Token: 0x040024FC RID: 9468
	public int nailRange;

	// Token: 0x040024FD RID: 9469
	public int beamDamage;

	// Token: 0x040024FE RID: 9470
	public int nailUpgrades;

	// Token: 0x040024FF RID: 9471
	public bool InvNailHasNew;

	// Token: 0x04002500 RID: 9472
	public bool hasSilkSpecial;

	// Token: 0x04002501 RID: 9473
	public int silkSpecialLevel;

	// Token: 0x04002502 RID: 9474
	public bool hasNeedleThrow;

	// Token: 0x04002503 RID: 9475
	public bool hasThreadSphere;

	// Token: 0x04002504 RID: 9476
	public bool hasParry;

	// Token: 0x04002505 RID: 9477
	public bool hasHarpoonDash;

	// Token: 0x04002506 RID: 9478
	public bool hasSilkCharge;

	// Token: 0x04002507 RID: 9479
	public bool hasSilkBomb;

	// Token: 0x04002508 RID: 9480
	public bool hasSilkBossNeedle;

	// Token: 0x04002509 RID: 9481
	public bool hasNeedolin;

	// Token: 0x0400250A RID: 9482
	public int attunement;

	// Token: 0x0400250B RID: 9483
	[DefaultValue(1)]
	public int attunementLevel;

	// Token: 0x0400250C RID: 9484
	public bool hasNeedolinMemoryPowerup;

	// Token: 0x0400250D RID: 9485
	public bool hasDash;

	// Token: 0x0400250E RID: 9486
	public bool hasBrolly;

	// Token: 0x0400250F RID: 9487
	public bool hasWalljump;

	// Token: 0x04002510 RID: 9488
	public bool hasDoubleJump;

	// Token: 0x04002511 RID: 9489
	public bool hasQuill;

	// Token: 0x04002512 RID: 9490
	public bool hasChargeSlash;

	// Token: 0x04002513 RID: 9491
	public bool hasSuperJump;

	// Token: 0x04002514 RID: 9492
	public int QuillState;

	// Token: 0x04002515 RID: 9493
	public bool HasSeenDash;

	// Token: 0x04002516 RID: 9494
	public bool HasSeenWalljump;

	// Token: 0x04002517 RID: 9495
	public bool HasSeenSuperJump;

	// Token: 0x04002518 RID: 9496
	public bool HasSeenNeedolin;

	// Token: 0x04002519 RID: 9497
	public bool HasSeenNeedolinUp;

	// Token: 0x0400251A RID: 9498
	public bool HasSeenNeedolinDown;

	// Token: 0x0400251B RID: 9499
	public bool HasSeenHarpoon;

	// Token: 0x0400251C RID: 9500
	public bool HasSeenEvaHeal;

	// Token: 0x0400251D RID: 9501
	public int cloakOdour_slabFly;

	// Token: 0x0400251E RID: 9502
	public bool HasSeenSilkHearts;

	// Token: 0x0400251F RID: 9503
	public bool hasKilled;

	// Token: 0x04002520 RID: 9504
	public SaveSlotCompletionIcons.CompletionState CompletedEndings;

	// Token: 0x04002521 RID: 9505
	public SaveSlotCompletionIcons.CompletionState LastCompletedEnding;

	// Token: 0x04002522 RID: 9506
	public bool fixerQuestBoardConvo;

	// Token: 0x04002523 RID: 9507
	public bool fixerAcceptedQuestConvo;

	// Token: 0x04002524 RID: 9508
	public bool fixerBridgeConstructed;

	// Token: 0x04002525 RID: 9509
	public bool fixerBridgeBreaking;

	// Token: 0x04002526 RID: 9510
	public bool fixerBridgeBroken;

	// Token: 0x04002527 RID: 9511
	public bool fixerStatueConstructed;

	// Token: 0x04002528 RID: 9512
	public bool fixerStatueConvo;

	// Token: 0x04002529 RID: 9513
	public bool metSherma;

	// Token: 0x0400252A RID: 9514
	public bool seenBellBeast;

	// Token: 0x0400252B RID: 9515
	public int shermaPos;

	// Token: 0x0400252C RID: 9516
	public bool shermaConvoBellBeast;

	// Token: 0x0400252D RID: 9517
	public bool metShermaPilgrimsRest;

	// Token: 0x0400252E RID: 9518
	public bool shermaInBellhart;

	// Token: 0x0400252F RID: 9519
	public bool shermaSeenInBellhart;

	// Token: 0x04002530 RID: 9520
	public bool shermaSeenInSteps;

	// Token: 0x04002531 RID: 9521
	public bool shermaAtSteps;

	// Token: 0x04002532 RID: 9522
	public bool shermaConvoCoralBench;

	// Token: 0x04002533 RID: 9523
	public bool shermaConvoCoralJudges;

	// Token: 0x04002534 RID: 9524
	public bool hasActivatedBellBench;

	// Token: 0x04002535 RID: 9525
	public bool shermaWokeInSteps;

	// Token: 0x04002536 RID: 9526
	public bool enteredCoral_10;

	// Token: 0x04002537 RID: 9527
	public bool shermaCitadelEntrance_Visiting;

	// Token: 0x04002538 RID: 9528
	public bool shermaCitadelEntrance_Seen;

	// Token: 0x04002539 RID: 9529
	public bool shermaCitadelEntrance_Left;

	// Token: 0x0400253A RID: 9530
	public bool openedCitadelSpaLeft;

	// Token: 0x0400253B RID: 9531
	public bool openedCitadelSpaRight;

	// Token: 0x0400253C RID: 9532
	[DefaultValue(true)]
	public bool shermaCitadelSpa_Visiting;

	// Token: 0x0400253D RID: 9533
	public bool shermaCitadelSpa_Seen;

	// Token: 0x0400253E RID: 9534
	public bool shermaCitadelSpa_Left;

	// Token: 0x0400253F RID: 9535
	public bool shermaCitadelSpa_ExtraConvo;

	// Token: 0x04002540 RID: 9536
	public bool shermaInEnclave;

	// Token: 0x04002541 RID: 9537
	public bool shermaCitadelEnclave_Seen;

	// Token: 0x04002542 RID: 9538
	public bool metShermaEnclave;

	// Token: 0x04002543 RID: 9539
	public bool shermaEnclaveHealingConvo;

	// Token: 0x04002544 RID: 9540
	public bool shermaQuestActive;

	// Token: 0x04002545 RID: 9541
	public bool shermaHealerActive;

	// Token: 0x04002546 RID: 9542
	[DefaultValue(1)]
	public int shermaWoundedPilgrim;

	// Token: 0x04002547 RID: 9543
	public bool shermaCaretakerConvo1;

	// Token: 0x04002548 RID: 9544
	public bool shermaCaretakerConvoFinal;

	// Token: 0x04002549 RID: 9545
	public bool metMapper;

	// Token: 0x0400254A RID: 9546
	public bool mapperRosaryConvo;

	// Token: 0x0400254B RID: 9547
	public bool mapperMentorConvo;

	// Token: 0x0400254C RID: 9548
	public bool mapperQuillConvo;

	// Token: 0x0400254D RID: 9549
	public bool mapperMappingConvo;

	// Token: 0x0400254E RID: 9550
	public bool mapperCalledConvo;

	// Token: 0x0400254F RID: 9551
	public bool mapperHauntedBellhartConvo;

	// Token: 0x04002550 RID: 9552
	public bool mapperBellhartConvo;

	// Token: 0x04002551 RID: 9553
	public bool mapperBellhartConvoTimePassed;

	// Token: 0x04002552 RID: 9554
	public bool mapperBellhartConvo2;

	// Token: 0x04002553 RID: 9555
	public bool mapperAway;

	// Token: 0x04002554 RID: 9556
	public bool mapperMetInAnt04;

	// Token: 0x04002555 RID: 9557
	public bool mapperTubeConvo;

	// Token: 0x04002556 RID: 9558
	public bool mapperBrokenBenchConvo;

	// Token: 0x04002557 RID: 9559
	public bool mapperCursedConvo;

	// Token: 0x04002558 RID: 9560
	public bool mapperMaggottedConvo;

	// Token: 0x04002559 RID: 9561
	public bool mapperSellingTubePins;

	// Token: 0x0400255A RID: 9562
	public bool mapperMasterAfterConvo;

	// Token: 0x0400255B RID: 9563
	public bool mapperReactedToBrokenBellBench;

	// Token: 0x0400255C RID: 9564
	public bool SeenMapperBonetown;

	// Token: 0x0400255D RID: 9565
	public bool MapperLeftBonetown;

	// Token: 0x0400255E RID: 9566
	public bool MapperAppearInBellhart;

	// Token: 0x0400255F RID: 9567
	public bool SeenMapperBoneForest;

	// Token: 0x04002560 RID: 9568
	public bool MapperLeftBoneForest;

	// Token: 0x04002561 RID: 9569
	public bool SeenMapperDocks;

	// Token: 0x04002562 RID: 9570
	public bool MapperLeftDocks;

	// Token: 0x04002563 RID: 9571
	public bool SeenMapperWilds;

	// Token: 0x04002564 RID: 9572
	public bool MapperLeftWilds;

	// Token: 0x04002565 RID: 9573
	public bool SeenMapperCrawl;

	// Token: 0x04002566 RID: 9574
	public bool MapperLeftCrawl;

	// Token: 0x04002567 RID: 9575
	public bool SeenMapperGreymoor;

	// Token: 0x04002568 RID: 9576
	public bool MapperLeftGreymoor;

	// Token: 0x04002569 RID: 9577
	public bool SeenMapperBellhart;

	// Token: 0x0400256A RID: 9578
	public bool MapperLeftBellhart;

	// Token: 0x0400256B RID: 9579
	public bool SeenMapperShellwood;

	// Token: 0x0400256C RID: 9580
	public bool MapperLeftShellwood;

	// Token: 0x0400256D RID: 9581
	public bool SeenMapperHuntersNest;

	// Token: 0x0400256E RID: 9582
	public bool MapperLeftHuntersNest;

	// Token: 0x0400256F RID: 9583
	public bool SeenMapperJudgeSteps;

	// Token: 0x04002570 RID: 9584
	public bool MapperLeftJudgeSteps;

	// Token: 0x04002571 RID: 9585
	public bool SeenMapperDustpens;

	// Token: 0x04002572 RID: 9586
	public bool MapperLeftDustpens;

	// Token: 0x04002573 RID: 9587
	public bool SeenMapperPeak;

	// Token: 0x04002574 RID: 9588
	public bool MapperLeftPeak;

	// Token: 0x04002575 RID: 9589
	public bool SeenMapperShadow;

	// Token: 0x04002576 RID: 9590
	public bool MapperLeftShadow;

	// Token: 0x04002577 RID: 9591
	public bool SeenMapperCoralCaverns;

	// Token: 0x04002578 RID: 9592
	public bool MapperLeftCoralCaverns;

	// Token: 0x04002579 RID: 9593
	public bool mapperSparIntro;

	// Token: 0x0400257A RID: 9594
	public int mapperLocationAct3;

	// Token: 0x0400257B RID: 9595
	public bool seenMapperAct3;

	// Token: 0x0400257C RID: 9596
	[DefaultValue(true)]
	public bool mapperIsFightingAct3;

	// Token: 0x0400257D RID: 9597
	[DefaultValue(1)]
	public int mapperFightGroup;

	// Token: 0x0400257E RID: 9598
	public bool mapperConvo_Act3Intro;

	// Token: 0x0400257F RID: 9599
	public bool mapperConvo_Act3IntroTimePassed;

	// Token: 0x04002580 RID: 9600
	public bool mapperConvo_Act3NoStock;

	// Token: 0x04002581 RID: 9601
	public bool mapperConvo_WhiteFlower;

	// Token: 0x04002582 RID: 9602
	public bool metDruid;

	// Token: 0x04002583 RID: 9603
	public bool druidTradeIntro;

	// Token: 0x04002584 RID: 9604
	public int druidMossBerriesSold;

	// Token: 0x04002585 RID: 9605
	public int[] mossBerryValueList;

	// Token: 0x04002586 RID: 9606
	public bool druidAct3Intro;

	// Token: 0x04002587 RID: 9607
	public bool metLearnedPilgrim;

	// Token: 0x04002588 RID: 9608
	public bool metLearnedPilgrimAct3;

	// Token: 0x04002589 RID: 9609
	public bool metDicePilgrim;

	// Token: 0x0400258A RID: 9610
	public bool dicePilgrimDefeated;

	// Token: 0x0400258B RID: 9611
	public int dicePilgrimState;

	// Token: 0x0400258C RID: 9612
	public bool dicePilgrimGameExplained;

	// Token: 0x0400258D RID: 9613
	[DefaultValue(16)]
	public int dicePilgrimBank;

	// Token: 0x0400258E RID: 9614
	public bool metGarmond;

	// Token: 0x0400258F RID: 9615
	public bool garmondMoorwingConvo;

	// Token: 0x04002590 RID: 9616
	public bool garmondMoorwingConvoReady;

	// Token: 0x04002591 RID: 9617
	public bool garmondPurposeConvo;

	// Token: 0x04002592 RID: 9618
	public bool garmondSeenInGreymoor10;

	// Token: 0x04002593 RID: 9619
	public bool garmondInDust05;

	// Token: 0x04002594 RID: 9620
	public bool garmondSeenInDust05;

	// Token: 0x04002595 RID: 9621
	public bool garmondEncounterCooldown;

	// Token: 0x04002596 RID: 9622
	public bool enteredSong_19;

	// Token: 0x04002597 RID: 9623
	public bool enteredSong_01;

	// Token: 0x04002598 RID: 9624
	public bool enteredSong_02;

	// Token: 0x04002599 RID: 9625
	public bool garmondInSong01;

	// Token: 0x0400259A RID: 9626
	public bool garmondSeenInSong01;

	// Token: 0x0400259B RID: 9627
	public bool garmondInSong02;

	// Token: 0x0400259C RID: 9628
	public bool garmondSeenInSong02;

	// Token: 0x0400259D RID: 9629
	public bool enteredSong_13;

	// Token: 0x0400259E RID: 9630
	public bool garmondInSong13;

	// Token: 0x0400259F RID: 9631
	public bool garmondSeenInSong13;

	// Token: 0x040025A0 RID: 9632
	public bool enteredSong_17;

	// Token: 0x040025A1 RID: 9633
	public bool garmondInSong17;

	// Token: 0x040025A2 RID: 9634
	public bool garmondSeenInSong17;

	// Token: 0x040025A3 RID: 9635
	public bool garmondInLibrary;

	// Token: 0x040025A4 RID: 9636
	public bool garmondLibrarySeen;

	// Token: 0x040025A5 RID: 9637
	public bool garmondLibraryMet;

	// Token: 0x040025A6 RID: 9638
	public bool garmondLibraryOffered;

	// Token: 0x040025A7 RID: 9639
	public bool garmondLibraryDefeatedHornet;

	// Token: 0x040025A8 RID: 9640
	public bool garmondWillAidInForumBattle;

	// Token: 0x040025A9 RID: 9641
	public bool garmondInEnclave;

	// Token: 0x040025AA RID: 9642
	public bool garmondMetEnclave;

	// Token: 0x040025AB RID: 9643
	public int garmondEncounters_act3;

	// Token: 0x040025AC RID: 9644
	public bool metGarmondAct3;

	// Token: 0x040025AD RID: 9645
	public bool garmondFinalQuestReady;

	// Token: 0x040025AE RID: 9646
	public bool garmondBlackThreadDefeated;

	// Token: 0x040025AF RID: 9647
	public bool pilgrimRestMerchant_SingConvo;

	// Token: 0x040025B0 RID: 9648
	public bool pilgrimRestMerchant_RhinoRuckusConvo;

	// Token: 0x040025B1 RID: 9649
	public int pilgrimRestCrowd;

	// Token: 0x040025B2 RID: 9650
	[DefaultValue(true)]
	public bool nuuIsHome;

	// Token: 0x040025B3 RID: 9651
	public bool MetHalfwayHunterFan;

	// Token: 0x040025B4 RID: 9652
	public bool MetHunterFanOutside;

	// Token: 0x040025B5 RID: 9653
	public bool nuuVisiting_splinterQueen;

	// Token: 0x040025B6 RID: 9654
	public bool nuuEncountered_splinterQueen;

	// Token: 0x040025B7 RID: 9655
	public bool nuuVisiting_coralDrillers;

	// Token: 0x040025B8 RID: 9656
	public bool nuuEncountered_coralDrillers;

	// Token: 0x040025B9 RID: 9657
	public bool nuuVisiting_skullKing;

	// Token: 0x040025BA RID: 9658
	public bool nuuEncountered_skullKing;

	// Token: 0x040025BB RID: 9659
	public bool nuuVisiting_zapNest;

	// Token: 0x040025BC RID: 9660
	public bool nuuEncountered_zapNest;

	// Token: 0x040025BD RID: 9661
	public bool nuuSlappedOutside;

	// Token: 0x040025BE RID: 9662
	public bool nuuIntroAct3;

	// Token: 0x040025BF RID: 9663
	public bool nuuMementoAwarded;

	// Token: 0x040025C0 RID: 9664
	public bool gillyMet;

	// Token: 0x040025C1 RID: 9665
	public bool gillyIntroduced;

	// Token: 0x040025C2 RID: 9666
	public bool gillyStatueConvo;

	// Token: 0x040025C3 RID: 9667
	public bool gillyTrapConvo;

	// Token: 0x040025C4 RID: 9668
	public bool gillyHunterCampConvo;

	// Token: 0x040025C5 RID: 9669
	public bool gillyAct3Convo;

	// Token: 0x040025C6 RID: 9670
	public int gillyLocation;

	// Token: 0x040025C7 RID: 9671
	public int gillyLocationAct3;

	// Token: 0x040025C8 RID: 9672
	public bool gillyQueueMovingOn;

	// Token: 0x040025C9 RID: 9673
	public string dreamReturnScene;

	// Token: 0x040025CA RID: 9674
	public bool hasJournal;

	// Token: 0x040025CB RID: 9675
	public bool seenJournalMsg;

	// Token: 0x040025CC RID: 9676
	public bool seenMateriumMsg;

	// Token: 0x040025CD RID: 9677
	public bool seenJournalQuestUpdateMsg;

	// Token: 0x040025CE RID: 9678
	public EnemyJournalKillData EnemyJournalKillData;

	// Token: 0x040025CF RID: 9679
	public int currentInvPane;

	// Token: 0x040025D0 RID: 9680
	public bool showGeoUI;

	// Token: 0x040025D1 RID: 9681
	public bool showHealthUI;

	// Token: 0x040025D2 RID: 9682
	public bool promptFocus;

	// Token: 0x040025D3 RID: 9683
	public bool seenFocusTablet;

	// Token: 0x040025D4 RID: 9684
	public bool seenDreamNailPrompt;

	// Token: 0x040025D5 RID: 9685
	[DefaultValue(true)]
	public bool isFirstGame;

	// Token: 0x040025D6 RID: 9686
	public bool enteredTutorialFirstTime;

	// Token: 0x040025D7 RID: 9687
	public bool isInvincible;

	// Token: 0x040025D8 RID: 9688
	public bool infiniteAirJump;

	// Token: 0x040025D9 RID: 9689
	public string currentArea;

	// Token: 0x040025DA RID: 9690
	public bool visitedMossCave;

	// Token: 0x040025DB RID: 9691
	public bool visitedBoneBottom;

	// Token: 0x040025DC RID: 9692
	public bool visitedBoneForest;

	// Token: 0x040025DD RID: 9693
	public bool visitedMosstown;

	// Token: 0x040025DE RID: 9694
	public bool visitedHuntersTrail;

	// Token: 0x040025DF RID: 9695
	public bool visitedDeepDocks;

	// Token: 0x040025E0 RID: 9696
	public bool visitedWilds;

	// Token: 0x040025E1 RID: 9697
	public bool visitedGrove;

	// Token: 0x040025E2 RID: 9698
	public bool visitedGreymoor;

	// Token: 0x040025E3 RID: 9699
	public bool visitedWisp;

	// Token: 0x040025E4 RID: 9700
	public bool visitedBellhartHaunted;

	// Token: 0x040025E5 RID: 9701
	public bool visitedBellhart;

	// Token: 0x040025E6 RID: 9702
	public bool visitedBellhartSaved;

	// Token: 0x040025E7 RID: 9703
	public bool visitedShellwood;

	// Token: 0x040025E8 RID: 9704
	public bool visitedCrawl;

	// Token: 0x040025E9 RID: 9705
	public bool visitedDustpens;

	// Token: 0x040025EA RID: 9706
	public bool visitedShadow;

	// Token: 0x040025EB RID: 9707
	public bool visitedAqueducts;

	// Token: 0x040025EC RID: 9708
	public bool visitedMistmaze;

	// Token: 0x040025ED RID: 9709
	public bool visitedCoral;

	// Token: 0x040025EE RID: 9710
	public bool visitedCoralRiver;

	// Token: 0x040025EF RID: 9711
	public bool visitedCoralRiverInner;

	// Token: 0x040025F0 RID: 9712
	public bool visitedCoralTower;

	// Token: 0x040025F1 RID: 9713
	public bool visitedSlab;

	// Token: 0x040025F2 RID: 9714
	public bool visitedGrandGate;

	// Token: 0x040025F3 RID: 9715
	public bool visitedCitadel;

	// Token: 0x040025F4 RID: 9716
	public bool visitedUnderstore;

	// Token: 0x040025F5 RID: 9717
	public bool visitedWard;

	// Token: 0x040025F6 RID: 9718
	public bool visitedHalls;

	// Token: 0x040025F7 RID: 9719
	public bool visitedLibrary;

	// Token: 0x040025F8 RID: 9720
	public bool visitedStage;

	// Token: 0x040025F9 RID: 9721
	public bool visitedGloom;

	// Token: 0x040025FA RID: 9722
	public bool visitedWeave;

	// Token: 0x040025FB RID: 9723
	public bool visitedMountain;

	// Token: 0x040025FC RID: 9724
	public bool visitedIceCore;

	// Token: 0x040025FD RID: 9725
	public bool visitedHang;

	// Token: 0x040025FE RID: 9726
	public bool visitedHangAtrium;

	// Token: 0x040025FF RID: 9727
	public bool visitedEnclave;

	// Token: 0x04002600 RID: 9728
	public bool visitedArborium;

	// Token: 0x04002601 RID: 9729
	public bool visitedCogwork;

	// Token: 0x04002602 RID: 9730
	public bool visitedCradle;

	// Token: 0x04002603 RID: 9731
	public bool visitedRuinedCradle;

	// Token: 0x04002604 RID: 9732
	public bool visitedFleatopia;

	// Token: 0x04002605 RID: 9733
	public bool visitedFleaFestival;

	// Token: 0x04002606 RID: 9734
	public bool visitedAbyss;

	// Token: 0x04002607 RID: 9735
	public bool citadelHalfwayComplete;

	// Token: 0x04002608 RID: 9736
	public HashSet<string> scenesVisited;

	// Token: 0x04002609 RID: 9737
	public HashSet<string> scenesMapped;

	// Token: 0x0400260A RID: 9738
	public HashSet<string> scenesEncounteredBench;

	// Token: 0x0400260B RID: 9739
	public HashSet<string> scenesEncounteredCocoon;

	// Token: 0x0400260C RID: 9740
	public bool mapUpdateQueued;

	// Token: 0x0400260D RID: 9741
	public bool mapAllRooms;

	// Token: 0x0400260E RID: 9742
	public bool HasSeenMapUpdated;

	// Token: 0x0400260F RID: 9743
	public bool HasSeenMapMarkerUpdated;

	// Token: 0x04002610 RID: 9744
	public bool HasMossGrottoMap;

	// Token: 0x04002611 RID: 9745
	public bool HasWildsMap;

	// Token: 0x04002612 RID: 9746
	public bool HasBoneforestMap;

	// Token: 0x04002613 RID: 9747
	public bool HasDocksMap;

	// Token: 0x04002614 RID: 9748
	public bool HasGreymoorMap;

	// Token: 0x04002615 RID: 9749
	public bool HasBellhartMap;

	// Token: 0x04002616 RID: 9750
	public bool HasShellwoodMap;

	// Token: 0x04002617 RID: 9751
	public bool HasCrawlMap;

	// Token: 0x04002618 RID: 9752
	public bool HasHuntersNestMap;

	// Token: 0x04002619 RID: 9753
	public bool HasJudgeStepsMap;

	// Token: 0x0400261A RID: 9754
	public bool HasDustpensMap;

	// Token: 0x0400261B RID: 9755
	public bool HasSlabMap;

	// Token: 0x0400261C RID: 9756
	public bool HasPeakMap;

	// Token: 0x0400261D RID: 9757
	public bool HasCitadelUnderstoreMap;

	// Token: 0x0400261E RID: 9758
	public bool HasCoralMap;

	// Token: 0x0400261F RID: 9759
	public bool HasSwampMap;

	// Token: 0x04002620 RID: 9760
	public bool HasCloverMap;

	// Token: 0x04002621 RID: 9761
	public bool HasAbyssMap;

	// Token: 0x04002622 RID: 9762
	public bool HasHangMap;

	// Token: 0x04002623 RID: 9763
	public bool HasSongGateMap;

	// Token: 0x04002624 RID: 9764
	public bool HasHallsMap;

	// Token: 0x04002625 RID: 9765
	public bool HasWardMap;

	// Token: 0x04002626 RID: 9766
	public bool HasCogMap;

	// Token: 0x04002627 RID: 9767
	public bool HasLibraryMap;

	// Token: 0x04002628 RID: 9768
	public bool HasCradleMap;

	// Token: 0x04002629 RID: 9769
	public bool HasArboriumMap;

	// Token: 0x0400262A RID: 9770
	public bool HasAqueductMap;

	// Token: 0x0400262B RID: 9771
	public bool HasWeavehomeMap;

	// Token: 0x0400262C RID: 9772
	public bool act3MapUpdated;

	// Token: 0x0400262D RID: 9773
	[JsonIgnore]
	[NonSerialized]
	private PlayerData.MapBoolList mapBoolList;

	// Token: 0x0400262E RID: 9774
	public bool ShakraFinalQuestAppear;

	// Token: 0x0400262F RID: 9775
	public bool hasPinBench;

	// Token: 0x04002630 RID: 9776
	public bool hasPinCocoon;

	// Token: 0x04002631 RID: 9777
	public bool hasPinShop;

	// Token: 0x04002632 RID: 9778
	public bool hasPinSpa;

	// Token: 0x04002633 RID: 9779
	public bool hasPinStag;

	// Token: 0x04002634 RID: 9780
	public bool hasPinTube;

	// Token: 0x04002635 RID: 9781
	public bool hasPinFleaMarrowlands;

	// Token: 0x04002636 RID: 9782
	public bool hasPinFleaMidlands;

	// Token: 0x04002637 RID: 9783
	public bool hasPinFleaBlastedlands;

	// Token: 0x04002638 RID: 9784
	public bool hasPinFleaCitadel;

	// Token: 0x04002639 RID: 9785
	public bool hasPinFleaPeaklands;

	// Token: 0x0400263A RID: 9786
	public bool hasPinFleaMucklands;

	// Token: 0x0400263B RID: 9787
	private static FieldInfo[] _savedFleaFields;

	// Token: 0x0400263C RID: 9788
	public bool hasMarker;

	// Token: 0x0400263D RID: 9789
	public bool hasMarker_a;

	// Token: 0x0400263E RID: 9790
	public bool hasMarker_b;

	// Token: 0x0400263F RID: 9791
	public bool hasMarker_c;

	// Token: 0x04002640 RID: 9792
	public bool hasMarker_d;

	// Token: 0x04002641 RID: 9793
	public bool hasMarker_e;

	// Token: 0x04002642 RID: 9794
	public WrappedVector2List[] placedMarkers;

	// Token: 0x04002643 RID: 9795
	public EnvironmentTypes environmentType;

	// Token: 0x04002644 RID: 9796
	public int previousDarkness;

	// Token: 0x04002645 RID: 9797
	public bool HasMelodyArchitect;

	// Token: 0x04002646 RID: 9798
	public bool HasMelodyLibrarian;

	// Token: 0x04002647 RID: 9799
	public bool SeenMelodyLibrarianReturn;

	// Token: 0x04002648 RID: 9800
	public bool HasMelodyConductor;

	// Token: 0x04002649 RID: 9801
	public bool UnlockedMelodyLift;

	// Token: 0x0400264A RID: 9802
	public bool MelodyLiftCanReturn;

	// Token: 0x0400264B RID: 9803
	public bool HeardMelodyConductorNoQuest;

	// Token: 0x0400264C RID: 9804
	public bool ConductorWeaverDlgQueued;

	// Token: 0x0400264D RID: 9805
	public bool ConductorWeaverDlgHeard;

	// Token: 0x0400264E RID: 9806
	public bool muchTimePassed;

	// Token: 0x0400264F RID: 9807
	[DefaultValue(1)]
	public int pilgrimGroupBonegrave;

	// Token: 0x04002650 RID: 9808
	[DefaultValue(1)]
	public int pilgrimGroupShellgrave;

	// Token: 0x04002651 RID: 9809
	[DefaultValue(1)]
	public int pilgrimGroupGreymoorField;

	// Token: 0x04002652 RID: 9810
	public bool shellGravePopulated;

	// Token: 0x04002653 RID: 9811
	public bool bellShrineBoneForest;

	// Token: 0x04002654 RID: 9812
	public bool bellShrineWilds;

	// Token: 0x04002655 RID: 9813
	public bool bellShrineGreymoor;

	// Token: 0x04002656 RID: 9814
	public bool bellShrineShellwood;

	// Token: 0x04002657 RID: 9815
	public bool bellShrineBellhart;

	// Token: 0x04002658 RID: 9816
	public bool bellShrineEnclave;

	// Token: 0x04002659 RID: 9817
	public bool completedMemory_reaper;

	// Token: 0x0400265A RID: 9818
	public bool completedMemory_wanderer;

	// Token: 0x0400265B RID: 9819
	public bool completedMemory_beast;

	// Token: 0x0400265C RID: 9820
	public bool completedMemory_witch;

	// Token: 0x0400265D RID: 9821
	public bool completedMemory_toolmaster;

	// Token: 0x0400265E RID: 9822
	public bool completedMemory_shaman;

	// Token: 0x0400265F RID: 9823
	public bool chapelClosed_reaper;

	// Token: 0x04002660 RID: 9824
	public bool chapelClosed_wanderer;

	// Token: 0x04002661 RID: 9825
	public bool chapelClosed_beast;

	// Token: 0x04002662 RID: 9826
	public bool chapelClosed_witch;

	// Token: 0x04002663 RID: 9827
	public bool chapelClosed_toolmaster;

	// Token: 0x04002664 RID: 9828
	public bool chapelClosed_shaman;

	// Token: 0x04002665 RID: 9829
	public bool bindCutscenePlayed;

	// Token: 0x04002666 RID: 9830
	public bool encounteredMossMother;

	// Token: 0x04002667 RID: 9831
	public bool defeatedMossMother;

	// Token: 0x04002668 RID: 9832
	public bool entered_Tut01b;

	// Token: 0x04002669 RID: 9833
	public bool completedTutorial;

	// Token: 0x0400266A RID: 9834
	public bool BonePlazaOpened;

	// Token: 0x0400266B RID: 9835
	public bool sawPlinneyLeft;

	// Token: 0x0400266C RID: 9836
	public bool savedPlinney;

	// Token: 0x0400266D RID: 9837
	public bool savedPlinneyConvo;

	// Token: 0x0400266E RID: 9838
	public bool defeatedMossEvolver;

	// Token: 0x0400266F RID: 9839
	public bool wokeMossEvolver;

	// Token: 0x04002670 RID: 9840
	public bool MetCrestUpgrader;

	// Token: 0x04002671 RID: 9841
	public bool MetCrestUpgraderAct3;

	// Token: 0x04002672 RID: 9842
	public bool CrestPreUpgradeTalked;

	// Token: 0x04002673 RID: 9843
	public bool CrestPreUpgradeAdditional;

	// Token: 0x04002674 RID: 9844
	public bool CrestPurposeQueued;

	// Token: 0x04002675 RID: 9845
	public bool CrestTalkedPurpose;

	// Token: 0x04002676 RID: 9846
	public bool CrestUpgraderTalkedSnare;

	// Token: 0x04002677 RID: 9847
	public bool CrestUpgraderOfferedFinal;

	// Token: 0x04002678 RID: 9848
	public bool HasBoundCrestUpgrader;

	// Token: 0x04002679 RID: 9849
	public bool churchKeeperIntro;

	// Token: 0x0400267A RID: 9850
	public bool churchKeeperCursedConvo;

	// Token: 0x0400267B RID: 9851
	public bool churchKeeperBonegraveConvo;

	// Token: 0x0400267C RID: 9852
	public bool bonebottomQuestBoardFixed;

	// Token: 0x0400267D RID: 9853
	public bool EncounteredBonetownBoss;

	// Token: 0x0400267E RID: 9854
	public bool DefeatedBonetownBoss;

	// Token: 0x0400267F RID: 9855
	public bool boneBottomAddition_RagLine;

	// Token: 0x04002680 RID: 9856
	public bool seenPilbyLeft;

	// Token: 0x04002681 RID: 9857
	public bool seenPebbLeft;

	// Token: 0x04002682 RID: 9858
	public bool seenBonetownDestroyed;

	// Token: 0x04002683 RID: 9859
	public bool bonetownPilgrimRoundActive;

	// Token: 0x04002684 RID: 9860
	public bool bonetownPilgrimRoundSeen;

	// Token: 0x04002685 RID: 9861
	public bool bonetownPilgrimHornedActive;

	// Token: 0x04002686 RID: 9862
	public bool bonetownPilgrimHornedSeen;

	// Token: 0x04002687 RID: 9863
	[DefaultValue(20)]
	public int bonetownPilgrimRoundCount;

	// Token: 0x04002688 RID: 9864
	[DefaultValue(10)]
	public int bonetownPilgrimHornedCount;

	// Token: 0x04002689 RID: 9865
	public bool ChurchKeeperLeftBasement;

	// Token: 0x0400268A RID: 9866
	public bool BoneBottomShellFrag1;

	// Token: 0x0400268B RID: 9867
	public bool SeenBoneBottomShopKeep;

	// Token: 0x0400268C RID: 9868
	public bool MetBoneBottomShopKeep;

	// Token: 0x0400268D RID: 9869
	public bool HeardBoneBottomShopKeepPostBoss;

	// Token: 0x0400268E RID: 9870
	public bool PurchasedBonebottomFaithToken;

	// Token: 0x0400268F RID: 9871
	public bool PurchasedBonebottomHeartPiece;

	// Token: 0x04002690 RID: 9872
	public bool PurchasedBonebottomToolMetal;

	// Token: 0x04002691 RID: 9873
	public bool BoneBottomShopKeepWillLeave;

	// Token: 0x04002692 RID: 9874
	public bool BoneBottomShopKeepLeft;

	// Token: 0x04002693 RID: 9875
	[DefaultValue(1)]
	public int bonetownCrowd;

	// Token: 0x04002694 RID: 9876
	public bool grindleReleasedFromBonejail;

	// Token: 0x04002695 RID: 9877
	public bool explodeWallMosstown3;

	// Token: 0x04002696 RID: 9878
	public bool bonegraveOpen;

	// Token: 0x04002697 RID: 9879
	public bool mosstownAspidBerryCollected;

	// Token: 0x04002698 RID: 9880
	public bool bonegraveAspidBerryCollected;

	// Token: 0x04002699 RID: 9881
	public bool bonegraveRosaryPilgrimDefeated;

	// Token: 0x0400269A RID: 9882
	public bool bonegravePilgrimCrowdsCanReturn;

	// Token: 0x0400269B RID: 9883
	public bool ShopkeeperQuestMentioned;

	// Token: 0x0400269C RID: 9884
	public bool belltownBasementBreakWall;

	// Token: 0x0400269D RID: 9885
	public bool basementAntWall;

	// Token: 0x0400269E RID: 9886
	public bool hunterInfestationBoneForest;

	// Token: 0x0400269F RID: 9887
	public bool skullKingShortcut;

	// Token: 0x040026A0 RID: 9888
	public bool skullKingAwake;

	// Token: 0x040026A1 RID: 9889
	public bool skullKingDefeated;

	// Token: 0x040026A2 RID: 9890
	public bool skullKingDefeatedBlackThreaded;

	// Token: 0x040026A3 RID: 9891
	public bool skullKingWillInvade;

	// Token: 0x040026A4 RID: 9892
	public bool skullKingInvaded;

	// Token: 0x040026A5 RID: 9893
	public bool skullKingKilled;

	// Token: 0x040026A6 RID: 9894
	public bool skullKingBenchMended;

	// Token: 0x040026A7 RID: 9895
	public bool skullKingPlatMended;

	// Token: 0x040026A8 RID: 9896
	public bool learnedPilbyName;

	// Token: 0x040026A9 RID: 9897
	public int pilbyFriendship;

	// Token: 0x040026AA RID: 9898
	public bool pilbyMeetConvo;

	// Token: 0x040026AB RID: 9899
	public bool pilbyCampConvo;

	// Token: 0x040026AC RID: 9900
	public bool pilbyFirstRepeatConvo;

	// Token: 0x040026AD RID: 9901
	public bool pilbyMosstownConvo;

	// Token: 0x040026AE RID: 9902
	public bool pilbyGotSprintConvo;

	// Token: 0x040026AF RID: 9903
	public bool pilbyBellhartConvo;

	// Token: 0x040026B0 RID: 9904
	public bool pilbyKilled;

	// Token: 0x040026B1 RID: 9905
	public bool pilbyAtPilgrimsRest;

	// Token: 0x040026B2 RID: 9906
	public bool pilbyInsidePilgrimsRest;

	// Token: 0x040026B3 RID: 9907
	public bool pilbySeenAtPilgrimsRest;

	// Token: 0x040026B4 RID: 9908
	public bool pilbyLeftPilgrimsRest;

	// Token: 0x040026B5 RID: 9909
	public bool pilbyPilgrimsRestMeetConvo;

	// Token: 0x040026B6 RID: 9910
	public bool boneBottomFuneral;

	// Token: 0x040026B7 RID: 9911
	public bool boneBottomFuneralComplete;

	// Token: 0x040026B8 RID: 9912
	public int BonebottomBellwayPilgrimState;

	// Token: 0x040026B9 RID: 9913
	public bool BonebottomBellwayPilgrimScared;

	// Token: 0x040026BA RID: 9914
	public bool BonebottomBellwayPilgrimLeft;

	// Token: 0x040026BB RID: 9915
	public bool greatBoneGateOpened;

	// Token: 0x040026BC RID: 9916
	public bool bone01shortcutPlat;

	// Token: 0x040026BD RID: 9917
	public bool didPilgrimIntroScene;

	// Token: 0x040026BE RID: 9918
	public bool mosstown01_shortcut;

	// Token: 0x040026BF RID: 9919
	public bool encounteredBellBeast;

	// Token: 0x040026C0 RID: 9920
	public bool defeatedBellBeast;

	// Token: 0x040026C1 RID: 9921
	public bool bonetownAspidBerryCollected;

	// Token: 0x040026C2 RID: 9922
	public int pinGalleriesCompleted;

	// Token: 0x040026C3 RID: 9923
	public bool PilgrimStomperNPCOffered;

	// Token: 0x040026C4 RID: 9924
	public bool pilgrimQuestSpoolCollected;

	// Token: 0x040026C5 RID: 9925
	public bool Bone_East_04b_ExplodeWall;

	// Token: 0x040026C6 RID: 9926
	public bool bone03_openedTrapdoor;

	// Token: 0x040026C7 RID: 9927
	public bool bone03_openedTrapdoorForRockRoller;

	// Token: 0x040026C8 RID: 9928
	public bool rockRollerDefeated_bone01;

	// Token: 0x040026C9 RID: 9929
	public bool rockRollerDefeated_bone06;

	// Token: 0x040026CA RID: 9930
	public bool rockRollerDefeated_bone07;

	// Token: 0x040026CB RID: 9931
	public bool collectorEggsHatched;

	// Token: 0x040026CC RID: 9932
	public bool creaturesReturnedToBone10;

	// Token: 0x040026CD RID: 9933
	public bool ant02GuardDefeated;

	// Token: 0x040026CE RID: 9934
	public bool antBenchTrapDefused;

	// Token: 0x040026CF RID: 9935
	public bool ant04_battleCompleted;

	// Token: 0x040026D0 RID: 9936
	public bool ant04_enemiesReturn;

	// Token: 0x040026D1 RID: 9937
	public int enemyGroupAnt04;

	// Token: 0x040026D2 RID: 9938
	public bool antMerchantKilled;

	// Token: 0x040026D3 RID: 9939
	public bool ant21_InitBattleCompleted;

	// Token: 0x040026D4 RID: 9940
	public bool ant21_ExtraBattleAdded;

	// Token: 0x040026D5 RID: 9941
	public bool metAntQueenNPC;

	// Token: 0x040026D6 RID: 9942
	public bool antQueenNPC_deepMelodyConvo;

	// Token: 0x040026D7 RID: 9943
	public bool defeatedAntQueen;

	// Token: 0x040026D8 RID: 9944
	public bool tookRestroomRosaries;

	// Token: 0x040026D9 RID: 9945
	public bool encounteredLace1;

	// Token: 0x040026DA RID: 9946
	public bool encounteredLace1Grotto;

	// Token: 0x040026DB RID: 9947
	public bool encounteredLaceBlastedBridge;

	// Token: 0x040026DC RID: 9948
	public bool defeatedLace1;

	// Token: 0x040026DD RID: 9949
	public bool laceLeftDocks;

	// Token: 0x040026DE RID: 9950
	public bool encounteredSongGolem;

	// Token: 0x040026DF RID: 9951
	public bool defeatedSongGolem;

	// Token: 0x040026E0 RID: 9952
	public bool destroyedSongGolemRock;

	// Token: 0x040026E1 RID: 9953
	public bool boneEast07_openedMidRoof;

	// Token: 0x040026E2 RID: 9954
	public bool openedTallGeyser;

	// Token: 0x040026E3 RID: 9955
	public bool openedGeyserShaft;

	// Token: 0x040026E4 RID: 9956
	public bool openedSongGateDocks;

	// Token: 0x040026E5 RID: 9957
	public bool openedDocksBackEntrance;

	// Token: 0x040026E6 RID: 9958
	public bool docksBomberAmbush;

	// Token: 0x040026E7 RID: 9959
	public bool docks_02_shortcut_right;

	// Token: 0x040026E8 RID: 9960
	public bool docks_02_shortcut_left;

	// Token: 0x040026E9 RID: 9961
	public bool gotPastDockSpearThrower;

	// Token: 0x040026EA RID: 9962
	public bool encounteredDockForemen;

	// Token: 0x040026EB RID: 9963
	public bool defeatedDockForemen;

	// Token: 0x040026EC RID: 9964
	public bool boneEastJailerKilled;

	// Token: 0x040026ED RID: 9965
	public bool boneEastJailerClearedOut;

	// Token: 0x040026EE RID: 9966
	public bool MetPilgrimsRestShop;

	// Token: 0x040026EF RID: 9967
	public bool SeenMortLeft;

	// Token: 0x040026F0 RID: 9968
	public bool SeenMortDead;

	// Token: 0x040026F1 RID: 9969
	public int PilgrimsRestShopIdleTalkState;

	// Token: 0x040026F2 RID: 9970
	public bool PurchasedPilgrimsRestToolPouch;

	// Token: 0x040026F3 RID: 9971
	public bool PurchasedPilgrimsRestMemoryLocket;

	// Token: 0x040026F4 RID: 9972
	public bool PilgrimsRestDoorBroken;

	// Token: 0x040026F5 RID: 9973
	public bool pilgrimsRestRosaryThiefCowardLeft;

	// Token: 0x040026F6 RID: 9974
	public bool mortKeptWeightedAnklet;

	// Token: 0x040026F7 RID: 9975
	public bool rhinoChurchUnlocked;

	// Token: 0x040026F8 RID: 9976
	public bool churchRhinoKilled;

	// Token: 0x040026F9 RID: 9977
	public bool rhinoRampageCompleted;

	// Token: 0x040026FA RID: 9978
	public bool rhinoRuckus;

	// Token: 0x040026FB RID: 9979
	public bool didRhinoRuckus;

	// Token: 0x040026FC RID: 9980
	public bool churchRhinoBlackThreadCorpse;

	// Token: 0x040026FD RID: 9981
	public bool MetAntMerchant;

	// Token: 0x040026FE RID: 9982
	public bool SeenAntMerchantDead;

	// Token: 0x040026FF RID: 9983
	public bool antMerchantShortcut;

	// Token: 0x04002700 RID: 9984
	public bool defeatedBoneFlyerGiant;

	// Token: 0x04002701 RID: 9985
	public bool defeatedBoneFlyerGiantGolemScene;

	// Token: 0x04002702 RID: 9986
	public bool openedBeastmasterDen;

	// Token: 0x04002703 RID: 9987
	public bool openedCauldronShortcut;

	// Token: 0x04002704 RID: 9988
	public bool visitedBoneEast14b;

	// Token: 0x04002705 RID: 9989
	public bool cauldronShortcutUpdraft;

	// Token: 0x04002706 RID: 9990
	public bool lavaChallengeEntranceCavedIn;

	// Token: 0x04002707 RID: 9991
	public bool completedLavaChallenge;

	// Token: 0x04002708 RID: 9992
	public bool lavaSpittersEmerge;

	// Token: 0x04002709 RID: 9993
	public bool IsPinGallerySetup;

	// Token: 0x0400270A RID: 9994
	public bool MetPinChallengeBug;

	// Token: 0x0400270B RID: 9995
	public bool WasInPinChallenge;

	// Token: 0x0400270C RID: 9996
	public bool PinGalleryLastChallengeOpen;

	// Token: 0x0400270D RID: 9997
	public bool PinGalleryHasPlayedFinalChallenge;

	// Token: 0x0400270E RID: 9998
	[DefaultValue(380)]
	public int PinGalleryWallet;

	// Token: 0x0400270F RID: 9999
	public bool HuntressQuestOffered;

	// Token: 0x04002710 RID: 10000
	public bool HuntressRuntQuestOffered;

	// Token: 0x04002711 RID: 10001
	public bool HuntressRuntAppeared;

	// Token: 0x04002712 RID: 10002
	public bool MottledChildGivenTool;

	// Token: 0x04002713 RID: 10003
	public bool MottledChildNewTool;

	// Token: 0x04002714 RID: 10004
	public bool encounteredAntTrapper;

	// Token: 0x04002715 RID: 10005
	public bool defeatedAntTrapper;

	// Token: 0x04002716 RID: 10006
	public bool explodeWallBoneEast18c;

	// Token: 0x04002717 RID: 10007
	public bool defeatedGuardBoneEast25;

	// Token: 0x04002718 RID: 10008
	public bool CompletedWeaveSprintChallenge;

	// Token: 0x04002719 RID: 10009
	public bool CompletedWeaveSprintChallengeMax;

	// Token: 0x0400271A RID: 10010
	public bool crashingIntoGreymoor;

	// Token: 0x0400271B RID: 10011
	public bool crashedIntoGreymoor;

	// Token: 0x0400271C RID: 10012
	public bool greymoor_04_battleCompleted;

	// Token: 0x0400271D RID: 10013
	public bool greymoor_10_entered;

	// Token: 0x0400271E RID: 10014
	public bool greymoor_05_centipedeArrives;

	// Token: 0x0400271F RID: 10015
	public bool killedRoostingCrowman;

	// Token: 0x04002720 RID: 10016
	public bool hitCrowCourtSwitch;

	// Token: 0x04002721 RID: 10017
	public bool tookGreymoor17Spool;

	// Token: 0x04002722 RID: 10018
	public bool completedGreymoor17Battle;

	// Token: 0x04002723 RID: 10019
	public bool CrowCourtInSession;

	// Token: 0x04002724 RID: 10020
	public string CrowSummonsAppearedScene;

	// Token: 0x04002725 RID: 10021
	public bool OpenedCrowSummonsDoor;

	// Token: 0x04002726 RID: 10022
	public bool PickedUpCrowMemento;

	// Token: 0x04002727 RID: 10023
	public bool MetHalfwayBartender;

	// Token: 0x04002728 RID: 10024
	public bool HalfwayPatronsCanVisit;

	// Token: 0x04002729 RID: 10025
	public bool SeenHalfwayPatronLeft;

	// Token: 0x0400272A RID: 10026
	public bool HalfwayPatronLeftGone;

	// Token: 0x0400272B RID: 10027
	public bool SeenHalfwayPatronRight;

	// Token: 0x0400272C RID: 10028
	public bool HalfwayPatronRightGone;

	// Token: 0x0400272D RID: 10029
	public int HalfwayDrinksPurchased;

	// Token: 0x0400272E RID: 10030
	public bool DeclinedBartenderDrink;

	// Token: 0x0400272F RID: 10031
	public bool HalfwayBartenderOfferedQuest;

	// Token: 0x04002730 RID: 10032
	public bool HalfwayBartenderCursedConvo;

	// Token: 0x04002731 RID: 10033
	public bool HalfwayBartenderHauntedBellhartConvo;

	// Token: 0x04002732 RID: 10034
	public bool visitedHalfway;

	// Token: 0x04002733 RID: 10035
	public int halfwayCrowd;

	// Token: 0x04002734 RID: 10036
	public bool HalfwayScarecrawAppeared;

	// Token: 0x04002735 RID: 10037
	public bool HalfwayNectarOffered;

	// Token: 0x04002736 RID: 10038
	public bool HalfwayNectarPaid;

	// Token: 0x04002737 RID: 10039
	public bool MetHalfwayBartenderAct3;

	// Token: 0x04002738 RID: 10040
	[DefaultValue(2)]
	public int halfwayCrowEnemyGroup;

	// Token: 0x04002739 RID: 10041
	public bool brokeUnderstoreFloor;

	// Token: 0x0400273A RID: 10042
	public bool enteredGreymoor05;

	// Token: 0x0400273B RID: 10043
	public bool previouslyVisitedGreymoor_05;

	// Token: 0x0400273C RID: 10044
	public bool greymoor05_clearedOut;

	// Token: 0x0400273D RID: 10045
	public bool greymoor05_killedJailer;

	// Token: 0x0400273E RID: 10046
	public bool greymoor05_farmerPlatBroken;

	// Token: 0x0400273F RID: 10047
	public bool greymoor08_plat_destroyed;

	// Token: 0x04002740 RID: 10048
	public bool encounteredVampireGnat_05;

	// Token: 0x04002741 RID: 10049
	public bool allowVampireGnatInAltLoc;

	// Token: 0x04002742 RID: 10050
	public bool encounteredVampireGnat_07;

	// Token: 0x04002743 RID: 10051
	public bool encounteredVampireGnatBoss;

	// Token: 0x04002744 RID: 10052
	public bool defeatedVampireGnatBoss;

	// Token: 0x04002745 RID: 10053
	public int vampireGnatDeaths;

	// Token: 0x04002746 RID: 10054
	public bool vampireGnatRequestedAid;

	// Token: 0x04002747 RID: 10055
	public bool VampireGnatDefeatedBeforeCaravanArrived;

	// Token: 0x04002748 RID: 10056
	public bool VampireGnatCorpseOnCaravan;

	// Token: 0x04002749 RID: 10057
	public bool VampireGnatCorpseInWater;

	// Token: 0x0400274A RID: 10058
	public bool encounteredCrowCourt;

	// Token: 0x0400274B RID: 10059
	public bool defeatedCrowCourt;

	// Token: 0x0400274C RID: 10060
	public bool defeatedWispPyreEffigy;

	// Token: 0x0400274D RID: 10061
	public bool wisp02_enemiesReturned;

	// Token: 0x0400274E RID: 10062
	public bool crawl03_oneWayWall;

	// Token: 0x0400274F RID: 10063
	public bool roofCrabEncountered;

	// Token: 0x04002750 RID: 10064
	public bool roofCrabDefeated;

	// Token: 0x04002751 RID: 10065
	public bool littleCrabsAppeared;

	// Token: 0x04002752 RID: 10066
	public bool aspid06_battleComplete;

	// Token: 0x04002753 RID: 10067
	public bool aspid06_cloverStagsReturned;

	// Token: 0x04002754 RID: 10068
	public bool aspid07_cloverStagsReturned;

	// Token: 0x04002755 RID: 10069
	public int whiteCloverPos;

	// Token: 0x04002756 RID: 10070
	public bool aspid_04_gate;

	// Token: 0x04002757 RID: 10071
	public bool aspid_16_oneway;

	// Token: 0x04002758 RID: 10072
	public bool aspid_16_relic;

	// Token: 0x04002759 RID: 10073
	public bool aspid_04b_battleCompleted;

	// Token: 0x0400275A RID: 10074
	public bool aspid_04b_wildlifeReturned;

	// Token: 0x0400275B RID: 10075
	public bool pilgrimFisherPossessed;

	// Token: 0x0400275C RID: 10076
	public int spinnerEncounter;

	// Token: 0x0400275D RID: 10077
	public bool encounteredSpinner;

	// Token: 0x0400275E RID: 10078
	public bool spinnerDefeated;

	// Token: 0x0400275F RID: 10079
	public bool SpinnerDefeatedTimePassed;

	// Token: 0x04002760 RID: 10080
	public bool shellwood14_ambushed;

	// Token: 0x04002761 RID: 10081
	public bool shellwoodTwigShortcut;

	// Token: 0x04002762 RID: 10082
	public bool encounteredSplinterQueen;

	// Token: 0x04002763 RID: 10083
	public bool defeatedSplinterQueen;

	// Token: 0x04002764 RID: 10084
	public int splinterQueenSproutTimer;

	// Token: 0x04002765 RID: 10085
	public bool splinterQueenSproutGrewLarge;

	// Token: 0x04002766 RID: 10086
	public bool splinterQueenSproutCut;

	// Token: 0x04002767 RID: 10087
	public bool shellwood13_BellWall;

	// Token: 0x04002768 RID: 10088
	public bool defeatedShellwoodRosaryPilgrim;

	// Token: 0x04002769 RID: 10089
	public bool shellwoodBellshrineTwigWall;

	// Token: 0x0400276A RID: 10090
	public bool seenEmptyShellwood16;

	// Token: 0x0400276B RID: 10091
	public bool slabFlyInShellwood16;

	// Token: 0x0400276C RID: 10092
	public bool shellwoodSlabflyDefeated;

	// Token: 0x0400276D RID: 10093
	public bool visitedShellwood_16;

	// Token: 0x0400276E RID: 10094
	public bool sethShortcut;

	// Token: 0x0400276F RID: 10095
	public bool encounteredSeth;

	// Token: 0x04002770 RID: 10096
	public int sethConvo;

	// Token: 0x04002771 RID: 10097
	public bool defeatedSeth;

	// Token: 0x04002772 RID: 10098
	public bool sethRevived;

	// Token: 0x04002773 RID: 10099
	public bool sethLeftShellwood;

	// Token: 0x04002774 RID: 10100
	public SethNpcLocations SethNpcLocation;

	// Token: 0x04002775 RID: 10101
	public bool MetSethNPC;

	// Token: 0x04002776 RID: 10102
	public bool SethJoinedFleatopia;

	// Token: 0x04002777 RID: 10103
	public bool encounteredFlowerQueen;

	// Token: 0x04002778 RID: 10104
	public bool defeatedFlowerQueen;

	// Token: 0x04002779 RID: 10105
	public bool flowerQueenHeartAppeared;

	// Token: 0x0400277A RID: 10106
	public bool MetWoodWitch;

	// Token: 0x0400277B RID: 10107
	public bool WoodWitchOfferedItemQuest;

	// Token: 0x0400277C RID: 10108
	public bool WoodWitchOfferedFlowerQuest;

	// Token: 0x0400277D RID: 10109
	public bool WoodWitchTalkedPostQuest;

	// Token: 0x0400277E RID: 10110
	public bool WoodWitchOfferedCurse;

	// Token: 0x0400277F RID: 10111
	public bool WoodWitchGaveMandrake;

	// Token: 0x04002780 RID: 10112
	public bool gainedCurse;

	// Token: 0x04002781 RID: 10113
	public bool BlueScientistMet;

	// Token: 0x04002782 RID: 10114
	public bool BlueScientistQuestOffered;

	// Token: 0x04002783 RID: 10115
	public bool BlueAssistantCorpseFound;

	// Token: 0x04002784 RID: 10116
	public bool BlueAssistantEnemyEncountered;

	// Token: 0x04002785 RID: 10117
	[DefaultValue(4)]
	public int BlueAssistantBloodCount;

	// Token: 0x04002786 RID: 10118
	public bool BlueScientistTalkedCorpse;

	// Token: 0x04002787 RID: 10119
	public bool BlueScientistPreQuest2Convo;

	// Token: 0x04002788 RID: 10120
	public bool BlueScientistQuest2Offered;

	// Token: 0x04002789 RID: 10121
	public bool BlueScientistQuest3Offered;

	// Token: 0x0400278A RID: 10122
	public bool BlueScientistInfectedSeen;

	// Token: 0x0400278B RID: 10123
	public bool BlueScientistInfectedMet;

	// Token: 0x0400278C RID: 10124
	public bool BlueScientistDead;

	// Token: 0x0400278D RID: 10125
	public bool BlueScientistSceneryPustulesGrown;

	// Token: 0x0400278E RID: 10126
	public bool dust01_battleCompleted;

	// Token: 0x0400278F RID: 10127
	public bool dust01_returnReady;

	// Token: 0x04002790 RID: 10128
	public bool dust03_battleCompleted;

	// Token: 0x04002791 RID: 10129
	public bool dust03_returnReady;

	// Token: 0x04002792 RID: 10130
	public bool openedDust05Gate;

	// Token: 0x04002793 RID: 10131
	public bool dust05EnemyClearedOut;

	// Token: 0x04002794 RID: 10132
	public bool CollectedDustCageKey;

	// Token: 0x04002795 RID: 10133
	public bool UnlockedDustCage;

	// Token: 0x04002796 RID: 10134
	public GreenPrinceLocations GreenPrinceLocation;

	// Token: 0x04002797 RID: 10135
	public bool GreenPrinceSeenSong04;

	// Token: 0x04002798 RID: 10136
	public bool FixedDustBellBench;

	// Token: 0x04002799 RID: 10137
	public bool silkFarmBattle1_complete;

	// Token: 0x0400279A RID: 10138
	public bool grubFarmerEmerged;

	// Token: 0x0400279B RID: 10139
	public bool metGrubFarmer;

	// Token: 0x0400279C RID: 10140
	public int grubFarmLevel;

	// Token: 0x0400279D RID: 10141
	public bool farmer_grewFirstGrub;

	// Token: 0x0400279E RID: 10142
	public bool farmer_grubGrowing_1;

	// Token: 0x0400279F RID: 10143
	public bool farmer_grubGrown_1;

	// Token: 0x040027A0 RID: 10144
	public bool farmer_grubGrowing_2;

	// Token: 0x040027A1 RID: 10145
	public bool farmer_grubGrown_2;

	// Token: 0x040027A2 RID: 10146
	public bool farmer_grubGrowing_3;

	// Token: 0x040027A3 RID: 10147
	public bool farmer_grubGrown_3;

	// Token: 0x040027A4 RID: 10148
	public bool grubFarmer_firstGrubConvo;

	// Token: 0x040027A5 RID: 10149
	public bool grubFarmer_needolinConvo1;

	// Token: 0x040027A6 RID: 10150
	public float grubFarmerTimer;

	// Token: 0x040027A7 RID: 10151
	public bool silkFarmAbyssCoresCleared;

	// Token: 0x040027A8 RID: 10152
	public bool metGrubFarmerAct3;

	// Token: 0x040027A9 RID: 10153
	public bool DustTradersOfferedQuest;

	// Token: 0x040027AA RID: 10154
	public bool DustTradersOfferedPins;

	// Token: 0x040027AB RID: 10155
	public bool defeatedRoachkeeperChef;

	// Token: 0x040027AC RID: 10156
	public bool gotPickledRoachEgg;

	// Token: 0x040027AD RID: 10157
	public bool roachkeeperChefCorpsePrepared;

	// Token: 0x040027AE RID: 10158
	public bool MetGrubFarmerMimic;

	// Token: 0x040027AF RID: 10159
	public int[] GrubFarmerMimicValueList;

	// Token: 0x040027B0 RID: 10160
	public int GrubFarmerSilkGrubsSold;

	// Token: 0x040027B1 RID: 10161
	public bool encounteredPhantom;

	// Token: 0x040027B2 RID: 10162
	public bool defeatedPhantom;

	// Token: 0x040027B3 RID: 10163
	public bool metSwampMuckmen;

	// Token: 0x040027B4 RID: 10164
	public bool visitedShadow03;

	// Token: 0x040027B5 RID: 10165
	public bool swampMuckmanTallInvades;

	// Token: 0x040027B6 RID: 10166
	public bool DefeatedSwampShaman;

	// Token: 0x040027B7 RID: 10167
	public bool thievesReturnedToShadow28;

	// Token: 0x040027B8 RID: 10168
	public bool SeenBelltownCutscene;

	// Token: 0x040027B9 RID: 10169
	public bool belltownCrowdsReady;

	// Token: 0x040027BA RID: 10170
	public int belltownCrowd;

	// Token: 0x040027BB RID: 10171
	public bool MetBelltownShopkeep;

	// Token: 0x040027BC RID: 10172
	public bool BelltownShopkeepCourierConvo1Accepted;

	// Token: 0x040027BD RID: 10173
	public bool BelltownShopkeepCourierConvo1Completed;

	// Token: 0x040027BE RID: 10174
	public bool BelltownShopkeepCursedConvo;

	// Token: 0x040027BF RID: 10175
	public bool BelltownShopkeepHouseConvo;

	// Token: 0x040027C0 RID: 10176
	public bool BelltownShopkeepAct3Convo;

	// Token: 0x040027C1 RID: 10177
	public bool PurchasedBelltownShellFragment;

	// Token: 0x040027C2 RID: 10178
	public bool PurchasedBelltownToolPouch;

	// Token: 0x040027C3 RID: 10179
	public bool PurchasedBelltownSpoolSegment;

	// Token: 0x040027C4 RID: 10180
	public bool PurchasedBelltownMemoryLocket;

	// Token: 0x040027C5 RID: 10181
	public int BelltownGreeterConvo;

	// Token: 0x040027C6 RID: 10182
	public bool BelltownGreetCursedConvo;

	// Token: 0x040027C7 RID: 10183
	public bool BelltownGreeterHouseHalfDlg;

	// Token: 0x040027C8 RID: 10184
	public bool BelltownGreeterHouseFullDlg;

	// Token: 0x040027C9 RID: 10185
	public bool BelltownGreeterFurnishingDlg;

	// Token: 0x040027CA RID: 10186
	public bool BelltownGreeterMetTimePassed;

	// Token: 0x040027CB RID: 10187
	public bool BelltownGreeterTwistedBudDlg;

	// Token: 0x040027CC RID: 10188
	public bool BelltownCouriersMet;

	// Token: 0x040027CD RID: 10189
	public bool BelltownCouriersMetAct3;

	// Token: 0x040027CE RID: 10190
	public bool BelltownCouriersGourmandHint;

	// Token: 0x040027CF RID: 10191
	public bool BelltownCouriersTalkedCursed;

	// Token: 0x040027D0 RID: 10192
	public bool BelltownCouriersTalkedGourmand;

	// Token: 0x040027D1 RID: 10193
	public bool BelltownCouriersBrokenDlgQueued;

	// Token: 0x040027D2 RID: 10194
	public bool BelltownCouriersBrokenDlg;

	// Token: 0x040027D3 RID: 10195
	public bool BelltownCouriersNotPurchasedDlg;

	// Token: 0x040027D4 RID: 10196
	public int BelltownCouriersPurchasedDlgBitmask;

	// Token: 0x040027D5 RID: 10197
	public List<string> BelltownCouriersGenericQuests;

	// Token: 0x040027D6 RID: 10198
	public bool BelltownCouriersFirstBeginDlg;

	// Token: 0x040027D7 RID: 10199
	public bool PinsmithMetBelltown;

	// Token: 0x040027D8 RID: 10200
	public bool PinsmithQuestOffered;

	// Token: 0x040027D9 RID: 10201
	public bool PinsmithUpg2Offered;

	// Token: 0x040027DA RID: 10202
	public bool PinsmithUpg3Offered;

	// Token: 0x040027DB RID: 10203
	public bool PinsmithUpg4Offered;

	// Token: 0x040027DC RID: 10204
	public bool BelltownHermitMet;

	// Token: 0x040027DD RID: 10205
	public int BelltownHermitEnslavedConvo;

	// Token: 0x040027DE RID: 10206
	public int BelltownHermitSavedConvo;

	// Token: 0x040027DF RID: 10207
	public bool BelltownHermitCursedConvo;

	// Token: 0x040027E0 RID: 10208
	public bool BelltownHermitConvoCooldown;

	// Token: 0x040027E1 RID: 10209
	public bool MetBelltownBagpipers;

	// Token: 0x040027E2 RID: 10210
	public bool BelltownBagpipersOfferedQuest;

	// Token: 0x040027E3 RID: 10211
	public bool MetBelltownDoctorDoor;

	// Token: 0x040027E4 RID: 10212
	public bool MetBelltownDoctorDoorAct3;

	// Token: 0x040027E5 RID: 10213
	public bool MetBelltownDoctor;

	// Token: 0x040027E6 RID: 10214
	public bool BelltownDoctorQuestOffered;

	// Token: 0x040027E7 RID: 10215
	public bool BelltownDoctorFixOffered;

	// Token: 0x040027E8 RID: 10216
	public bool BelltownDoctorMaggotSpoke;

	// Token: 0x040027E9 RID: 10217
	public bool BelltownDoctorLifebloodSpoke;

	// Token: 0x040027EA RID: 10218
	public bool BelltownDoctorCuredCurse;

	// Token: 0x040027EB RID: 10219
	public int BelltownDoctorConvo;

	// Token: 0x040027EC RID: 10220
	public bool MetFisherHomeBasic;

	// Token: 0x040027ED RID: 10221
	public bool MetFisherHomeFull;

	// Token: 0x040027EE RID: 10222
	public float FisherWalkerTimer;

	// Token: 0x040027EF RID: 10223
	public bool FisherWalkerDirection;

	// Token: 0x040027F0 RID: 10224
	public float FisherWalkerIdleTimeLeft;

	// Token: 0x040027F1 RID: 10225
	public bool MetBelltownRelicDealer;

	// Token: 0x040027F2 RID: 10226
	public bool BelltownRelicDealerGaveRelic;

	// Token: 0x040027F3 RID: 10227
	public bool BelltownRelicDealerCylinderConvo;

	// Token: 0x040027F4 RID: 10228
	public bool BelltownRelicDealerOutroConvo;

	// Token: 0x040027F5 RID: 10229
	public bool BelltownRelicDealerOutroConvoAllComplete;

	// Token: 0x040027F6 RID: 10230
	public bool MetBelltownRelicDealerAct3;

	// Token: 0x040027F7 RID: 10231
	public BelltownHouseStates BelltownHouseState;

	// Token: 0x040027F8 RID: 10232
	public bool BelltownHouseUnlocked;

	// Token: 0x040027F9 RID: 10233
	public BellhomePaintColours BelltownHouseColour;

	// Token: 0x040027FA RID: 10234
	public bool BelltownHousePaintComplete;

	// Token: 0x040027FB RID: 10235
	public bool BelltownFurnishingDesk;

	// Token: 0x040027FC RID: 10236
	public bool BelltownFurnishingSpaAvailable;

	// Token: 0x040027FD RID: 10237
	public bool BelltownFurnishingSpa;

	// Token: 0x040027FE RID: 10238
	public bool BelltownFurnishingFairyLights;

	// Token: 0x040027FF RID: 10239
	public bool BelltownFurnishingGramaphone;

	// Token: 0x04002800 RID: 10240
	public CollectionGramaphone.PlayingInfo BelltownHousePlayingInfo;

	// Token: 0x04002801 RID: 10241
	public bool CrawbellInstalled;

	// Token: 0x04002802 RID: 10242
	public float CrawbellTimer;

	// Token: 0x04002803 RID: 10243
	public int[] CrawbellCurrency;

	// Token: 0x04002804 RID: 10244
	public int[] CrawbellCurrencyCaps;

	// Token: 0x04002805 RID: 10245
	public bool CrawbellCrawsInside;

	// Token: 0x04002806 RID: 10246
	public bool DeskPlacedRelicList;

	// Token: 0x04002807 RID: 10247
	public bool DeskPlacedLibrarianList;

	// Token: 0x04002808 RID: 10248
	public bool ConstructedMaterium;

	// Token: 0x04002809 RID: 10249
	public bool ConstructedFarsight;

	// Token: 0x0400280A RID: 10250
	public bool CollectedToolMetal;

	// Token: 0x0400280B RID: 10251
	public bool CollectedCommonSpine;

	// Token: 0x0400280C RID: 10252
	public CollectableMementosData MementosDeposited;

	// Token: 0x0400280D RID: 10253
	public MateriumItemsData MateriumCollected;

	// Token: 0x0400280E RID: 10254
	public bool CollectedMementoGrey;

	// Token: 0x0400280F RID: 10255
	public bool CollectedMementoSprintmaster;

	// Token: 0x04002810 RID: 10256
	public bool MetForgeDaughter;

	// Token: 0x04002811 RID: 10257
	public int ForgeDaughterTalkState;

	// Token: 0x04002812 RID: 10258
	public bool ForgeDaughterPurchaseDlg;

	// Token: 0x04002813 RID: 10259
	public bool ForgeDaughterSpentToolMetal;

	// Token: 0x04002814 RID: 10260
	public bool ForgeDaughterMentionedWebShot;

	// Token: 0x04002815 RID: 10261
	public bool MetForgeDaughterAct3;

	// Token: 0x04002816 RID: 10262
	public bool PurchasedForgeToolKit;

	// Token: 0x04002817 RID: 10263
	public bool BallowInSauna;

	// Token: 0x04002818 RID: 10264
	public bool BallowSeenInSauna;

	// Token: 0x04002819 RID: 10265
	public bool BallowLeftSauna;

	// Token: 0x0400281A RID: 10266
	public bool ForgeDaughterMentionedDivingBell;

	// Token: 0x0400281B RID: 10267
	public bool BallowMovedToDivingBell;

	// Token: 0x0400281C RID: 10268
	public bool BallowGivenKey;

	// Token: 0x0400281D RID: 10269
	public bool BallowTalkedPostRepair;

	// Token: 0x0400281E RID: 10270
	public bool BallowTalkedPostRepairGramaphone;

	// Token: 0x0400281F RID: 10271
	public bool ForgeDaughterPostAbyssDlg;

	// Token: 0x04002820 RID: 10272
	public bool ForgeDaughterWhiteFlowerDlg;

	// Token: 0x04002821 RID: 10273
	public bool SeenDivingBellGoneAbyss;

	// Token: 0x04002822 RID: 10274
	public bool openedGateCoral_14;

	// Token: 0x04002823 RID: 10275
	public bool defeatedZapGuard1;

	// Token: 0x04002824 RID: 10276
	public bool encounteredCoralDrillers;

	// Token: 0x04002825 RID: 10277
	public bool defeatedCoralDrillers;

	// Token: 0x04002826 RID: 10278
	public bool coralDrillerSoloReady;

	// Token: 0x04002827 RID: 10279
	public bool activatedStepsUpperBellbench;

	// Token: 0x04002828 RID: 10280
	public bool defeatedCoralBridgeGuard1;

	// Token: 0x04002829 RID: 10281
	public bool coralBridgeGuard2Stationed;

	// Token: 0x0400282A RID: 10282
	public bool defeatedCoralBridgeGuard2;

	// Token: 0x0400282B RID: 10283
	public bool encounteredCoralKing;

	// Token: 0x0400282C RID: 10284
	public bool defeatedCoralKing;

	// Token: 0x0400282D RID: 10285
	public bool coralKingHeartAppeared;

	// Token: 0x0400282E RID: 10286
	public bool metGatePilgrim;

	// Token: 0x0400282F RID: 10287
	public bool gatePilgrimNoNeedolinConvo;

	// Token: 0x04002830 RID: 10288
	public bool encounteredLastJudge;

	// Token: 0x04002831 RID: 10289
	public bool defeatedLastJudge;

	// Token: 0x04002832 RID: 10290
	public bool SeenLastJudgeGateOpen;

	// Token: 0x04002833 RID: 10291
	public bool pinstressStoppedResting;

	// Token: 0x04002834 RID: 10292
	public bool pinstressInsideSitting;

	// Token: 0x04002835 RID: 10293
	public bool pinstressQuestReady;

	// Token: 0x04002836 RID: 10294
	public bool PinstressPeakQuestOffered;

	// Token: 0x04002837 RID: 10295
	public bool PinstressPeakBattleOffered;

	// Token: 0x04002838 RID: 10296
	public bool PinstressPeakBattleAccepted;

	// Token: 0x04002839 RID: 10297
	public bool SteelSentinelMet;

	// Token: 0x0400283A RID: 10298
	public bool SteelSentinelOffered;

	// Token: 0x0400283B RID: 10299
	public bool EncounteredSummonedSaviour;

	// Token: 0x0400283C RID: 10300
	public SteelSoulQuestSpot.Spot[] SteelQuestSpots;

	// Token: 0x0400283D RID: 10301
	public int GrowstoneState;

	// Token: 0x0400283E RID: 10302
	public float GrowstoneTimer;

	// Token: 0x0400283F RID: 10303
	public bool SeenGrindleShop;

	// Token: 0x04002840 RID: 10304
	public bool grindleShopEnemyIntro;

	// Token: 0x04002841 RID: 10305
	public bool purchasedGrindleSimpleKey;

	// Token: 0x04002842 RID: 10306
	public bool purchasedGrindleMemoryLocket;

	// Token: 0x04002843 RID: 10307
	public bool purchasedGrindleSpoolPiece;

	// Token: 0x04002844 RID: 10308
	public bool purchasedGrindleToolKit;

	// Token: 0x04002845 RID: 10309
	public bool metGrindleAct3;

	// Token: 0x04002846 RID: 10310
	public bool encounteredCoralDrillerSolo;

	// Token: 0x04002847 RID: 10311
	public bool defeatedCoralDrillerSolo;

	// Token: 0x04002848 RID: 10312
	public bool coralDrillerSoloEnemiesReturned;

	// Token: 0x04002849 RID: 10313
	public bool defeatedZapCoreEnemy;

	// Token: 0x0400284A RID: 10314
	public bool wokeGreyWarrior;

	// Token: 0x0400284B RID: 10315
	public bool defeatedGreyWarrior;

	// Token: 0x0400284C RID: 10316
	public float greyWarriorDeathX;

	// Token: 0x0400284D RID: 10317
	public bool visitedCoralBellshrine;

	// Token: 0x0400284E RID: 10318
	public bool coral19_clearedOut;

	// Token: 0x0400284F RID: 10319
	public bool encounteredPharloomEdge;

	// Token: 0x04002850 RID: 10320
	public bool encounteredPharloomEdgeAct3;

	// Token: 0x04002851 RID: 10321
	public bool weave01_oneWay;

	// Token: 0x04002852 RID: 10322
	public bool weave05_oneWay;

	// Token: 0x04002853 RID: 10323
	public bool wokeLiftWeaver;

	// Token: 0x04002854 RID: 10324
	public bool visitedUpperSlab;

	// Token: 0x04002855 RID: 10325
	public bool slab_03_rubbishCleared;

	// Token: 0x04002856 RID: 10326
	public bool slab_cloak_battle_encountered;

	// Token: 0x04002857 RID: 10327
	public bool slab_cloak_battle_completed;

	// Token: 0x04002858 RID: 10328
	public bool slab_cloak_gate_reopened;

	// Token: 0x04002859 RID: 10329
	public bool slab_05_gateOpen;

	// Token: 0x0400285A RID: 10330
	public bool slab_07_gateOpen;

	// Token: 0x0400285B RID: 10331
	public bool slab_17_openedGateRight;

	// Token: 0x0400285C RID: 10332
	public bool slab_cell_quiet_oneWayWall;

	// Token: 0x0400285D RID: 10333
	public bool slab_17_openedGateLeft;

	// Token: 0x0400285E RID: 10334
	public bool slabCaptor_heardChallenge;

	// Token: 0x0400285F RID: 10335
	public bool slabCaptor_heardChallengeRings;

	// Token: 0x04002860 RID: 10336
	public bool encounteredFirstWeaver;

	// Token: 0x04002861 RID: 10337
	public bool defeatedFirstWeaver;

	// Token: 0x04002862 RID: 10338
	public int grindleSlabSequence;

	// Token: 0x04002863 RID: 10339
	public bool slabPrisonerSingConvo;

	// Token: 0x04002864 RID: 10340
	public bool slabPrisonerFlyConvo;

	// Token: 0x04002865 RID: 10341
	public bool slabPrisonerRemeetConvo;

	// Token: 0x04002866 RID: 10342
	public bool HasSlabKeyA;

	// Token: 0x04002867 RID: 10343
	public bool HasSlabKeyB;

	// Token: 0x04002868 RID: 10344
	public bool HasSlabKeyC;

	// Token: 0x04002869 RID: 10345
	public bool defeatedBroodMother;

	// Token: 0x0400286A RID: 10346
	public bool broodMotherEyeCollected;

	// Token: 0x0400286B RID: 10347
	public bool tinyBroodMotherAppeared;

	// Token: 0x0400286C RID: 10348
	public bool peak13_oneWay;

	// Token: 0x0400286D RID: 10349
	public bool peak05b_oneWay;

	// Token: 0x0400286E RID: 10350
	public bool peak05c_oneWay;

	// Token: 0x0400286F RID: 10351
	public bool peak06_oneWay;

	// Token: 0x04002870 RID: 10352
	public bool MetMaskMaker;

	// Token: 0x04002871 RID: 10353
	public bool MetMaskMakerAct3;

	// Token: 0x04002872 RID: 10354
	public bool MaskMakerTalkedRelationship;

	// Token: 0x04002873 RID: 10355
	public bool MaskMakerTalkedPeak;

	// Token: 0x04002874 RID: 10356
	public bool MaskMakerTalkedUnmaskedAct3;

	// Token: 0x04002875 RID: 10357
	public bool MaskMakerTalkedUnmasked;

	// Token: 0x04002876 RID: 10358
	public bool MaskMakerTalkedUnmasked1;

	// Token: 0x04002877 RID: 10359
	public bool MaskMakerQueuedUnmasked2;

	// Token: 0x04002878 RID: 10360
	public bool MaskMakerTalkedUnmasked2;

	// Token: 0x04002879 RID: 10361
	public bool understoreLiftBroke;

	// Token: 0x0400287A RID: 10362
	public bool brokeConfessional;

	// Token: 0x0400287B RID: 10363
	public bool droppedFloorBreakerPlat;

	// Token: 0x0400287C RID: 10364
	public bool rosaryThievesInUnder07;

	// Token: 0x0400287D RID: 10365
	public bool under07_battleCompleted;

	// Token: 0x0400287E RID: 10366
	public bool under07_heavyWorkerReturned;

	// Token: 0x0400287F RID: 10367
	public bool openedShellwoodShortcut;

	// Token: 0x04002880 RID: 10368
	public bool openedUnder_05;

	// Token: 0x04002881 RID: 10369
	public bool openedUnder_19;

	// Token: 0x04002882 RID: 10370
	public bool openedUnder_01b;

	// Token: 0x04002883 RID: 10371
	public bool MetArchitect;

	// Token: 0x04002884 RID: 10372
	public bool MetArchitectAct3;

	// Token: 0x04002885 RID: 10373
	public bool PurchasedArchitectToolKit;

	// Token: 0x04002886 RID: 10374
	public bool PurchasedArchitectKey;

	// Token: 0x04002887 RID: 10375
	public bool ArchitectTalkedCrest;

	// Token: 0x04002888 RID: 10376
	public bool ArchitectMentionedWebShot;

	// Token: 0x04002889 RID: 10377
	public bool ArchitectMentionedCogHeart;

	// Token: 0x0400288A RID: 10378
	public bool ArchitectMentionedMelody;

	// Token: 0x0400288B RID: 10379
	public bool ArchitectMelodyReturnQueued;

	// Token: 0x0400288C RID: 10380
	public bool ArchitectMelodyReturnSeen;

	// Token: 0x0400288D RID: 10381
	public bool ArchitectMelodyGainSeen;

	// Token: 0x0400288E RID: 10382
	public bool ArchitectWillLeave;

	// Token: 0x0400288F RID: 10383
	public bool ArchitectLeft;

	// Token: 0x04002890 RID: 10384
	public bool SeenArchitectLeft;

	// Token: 0x04002891 RID: 10385
	public bool citadelWoken;

	// Token: 0x04002892 RID: 10386
	public bool song05MarchGroupReady;

	// Token: 0x04002893 RID: 10387
	public bool laceMeetCitadel;

	// Token: 0x04002894 RID: 10388
	public bool song18Shortcut;

	// Token: 0x04002895 RID: 10389
	public bool encounteredLibraryEntryBattle;

	// Token: 0x04002896 RID: 10390
	public bool completedLibraryEntryBattle;

	// Token: 0x04002897 RID: 10391
	public bool scholarAmbushReady;

	// Token: 0x04002898 RID: 10392
	public bool libraryRoofShortcut;

	// Token: 0x04002899 RID: 10393
	public bool scholarAcolytesReleased;

	// Token: 0x0400289A RID: 10394
	public bool seenScholarAcolytes;

	// Token: 0x0400289B RID: 10395
	public bool scholarAcolytesInLibrary_02;

	// Token: 0x0400289C RID: 10396
	public bool completedLibraryAcolyteBattle;

	// Token: 0x0400289D RID: 10397
	public bool library_14_ambush;

	// Token: 0x0400289E RID: 10398
	public bool completedGrandStageBattle;

	// Token: 0x0400289F RID: 10399
	public bool encounteredTrobbio;

	// Token: 0x040028A0 RID: 10400
	public bool defeatedTrobbio;

	// Token: 0x040028A1 RID: 10401
	public bool trobbioCleanedUp;

	// Token: 0x040028A2 RID: 10402
	public bool encounteredTormentedTrobbio;

	// Token: 0x040028A3 RID: 10403
	public bool defeatedTormentedTrobbio;

	// Token: 0x040028A4 RID: 10404
	public bool tormentedTrobbioLurking;

	// Token: 0x040028A5 RID: 10405
	public bool libraryStatueWoken;

	// Token: 0x040028A6 RID: 10406
	public bool marionettesMet;

	// Token: 0x040028A7 RID: 10407
	public bool song_17_clearedOut;

	// Token: 0x040028A8 RID: 10408
	public bool song_27_opened;

	// Token: 0x040028A9 RID: 10409
	public bool marionettesBurned;

	// Token: 0x040028AA RID: 10410
	public bool song_11_oneway;

	// Token: 0x040028AB RID: 10411
	public bool citadel_encounteredFencers;

	// Token: 0x040028AC RID: 10412
	public bool enteredHang_08;

	// Token: 0x040028AD RID: 10413
	public bool LibrarianAskedForRelic;

	// Token: 0x040028AE RID: 10414
	public bool GivenLibrarianRelic;

	// Token: 0x040028AF RID: 10415
	public bool LibrarianMetAct3;

	// Token: 0x040028B0 RID: 10416
	public bool LibrarianMentionedMelody;

	// Token: 0x040028B1 RID: 10417
	public bool LibrarianAskedForMelody;

	// Token: 0x040028B2 RID: 10418
	public CollectionGramaphone.PlayingInfo LibrarianPlayingInfo;

	// Token: 0x040028B3 RID: 10419
	public bool LibrarianCollectionComplete;

	// Token: 0x040028B4 RID: 10420
	public bool encounteredCogworkDancers;

	// Token: 0x040028B5 RID: 10421
	public bool defeatedCogworkDancers;

	// Token: 0x040028B6 RID: 10422
	public bool cityMerchantSaved;

	// Token: 0x040028B7 RID: 10423
	public bool cityMerchantIntroduced;

	// Token: 0x040028B8 RID: 10424
	public bool cityMerchantEnclaveConvo;

	// Token: 0x040028B9 RID: 10425
	public bool cityMerchantRecentlySeenInEnclave;

	// Token: 0x040028BA RID: 10426
	public bool cityMerchantInGrandForum;

	// Token: 0x040028BB RID: 10427
	public bool cityMerchantInGrandForumSeen;

	// Token: 0x040028BC RID: 10428
	public bool cityMerchantInGrandForumLeft;

	// Token: 0x040028BD RID: 10429
	public bool cityMerchantInLibrary03;

	// Token: 0x040028BE RID: 10430
	public bool cityMerchantInLibrary03Seen;

	// Token: 0x040028BF RID: 10431
	public bool cityMerchantInLibrary03Left;

	// Token: 0x040028C0 RID: 10432
	public bool cityMerchantCanLeaveForBridge;

	// Token: 0x040028C1 RID: 10433
	public bool MetCityMerchantScavenge;

	// Token: 0x040028C2 RID: 10434
	public bool MetCityMerchantEnclave;

	// Token: 0x040028C3 RID: 10435
	public bool MetCityMerchantEnclaveAct3;

	// Token: 0x040028C4 RID: 10436
	public bool cityMerchantConvo1;

	// Token: 0x040028C5 RID: 10437
	public bool cityMerchantBridgeSaveRemeet;

	// Token: 0x040028C6 RID: 10438
	public bool MerchantEnclaveShellFragment;

	// Token: 0x040028C7 RID: 10439
	public bool MerchantEnclaveSpoolPiece;

	// Token: 0x040028C8 RID: 10440
	public bool MerchantEnclaveSocket;

	// Token: 0x040028C9 RID: 10441
	public bool MerchantEnclaveWardKey;

	// Token: 0x040028CA RID: 10442
	public bool MerchantEnclaveSimpleKey;

	// Token: 0x040028CB RID: 10443
	public bool MerchantEnclaveToolMetal;

	// Token: 0x040028CC RID: 10444
	public bool encounteredLaceTower;

	// Token: 0x040028CD RID: 10445
	public bool defeatedLaceTower;

	// Token: 0x040028CE RID: 10446
	public float laceCorpseScaleX;

	// Token: 0x040028CF RID: 10447
	public float laceCorpsePosX;

	// Token: 0x040028D0 RID: 10448
	public bool laceTowerDoorOpened;

	// Token: 0x040028D1 RID: 10449
	public bool laceCorpseAddedEffects;

	// Token: 0x040028D2 RID: 10450
	public bool MetGourmandServant;

	// Token: 0x040028D3 RID: 10451
	public bool GourmandServantOfferedQuest;

	// Token: 0x040028D4 RID: 10452
	public bool GourmandGivenStew;

	// Token: 0x040028D5 RID: 10453
	public bool GourmandGivenNectar;

	// Token: 0x040028D6 RID: 10454
	public bool GourmandGivenEgg;

	// Token: 0x040028D7 RID: 10455
	public bool GourmandGivenMeat;

	// Token: 0x040028D8 RID: 10456
	public bool GourmandGivenCoral;

	// Token: 0x040028D9 RID: 10457
	public bool GotGourmandReward;

	// Token: 0x040028DA RID: 10458
	public bool MetGourmandServantAct3;

	// Token: 0x040028DB RID: 10459
	public bool metCaretaker;

	// Token: 0x040028DC RID: 10460
	public bool caretakerWardConvo;

	// Token: 0x040028DD RID: 10461
	public bool caretakerMerchantConvo;

	// Token: 0x040028DE RID: 10462
	public bool caretakerBeastConvo;

	// Token: 0x040028DF RID: 10463
	public bool caretakerLaceConvo;

	// Token: 0x040028E0 RID: 10464
	public bool caretakerConvoLv1;

	// Token: 0x040028E1 RID: 10465
	public bool caretakerConvoLv2;

	// Token: 0x040028E2 RID: 10466
	public bool caretakerConvoLv3;

	// Token: 0x040028E3 RID: 10467
	public bool CaretakerSwampSoulConvo;

	// Token: 0x040028E4 RID: 10468
	public bool CaretakerSnareProgressConvo;

	// Token: 0x040028E5 RID: 10469
	public bool CaretakerOfferedSnareQuest;

	// Token: 0x040028E6 RID: 10470
	public bool enclaveMerchantSaved;

	// Token: 0x040028E7 RID: 10471
	public bool enclaveMerchantSeenInEnclave;

	// Token: 0x040028E8 RID: 10472
	public NPCEncounterState EnclaveStatePilgrimSmall;

	// Token: 0x040028E9 RID: 10473
	public NPCEncounterState EnclaveStateNPCShortHorned;

	// Token: 0x040028EA RID: 10474
	public NPCEncounterState EnclaveStateNPCTall;

	// Token: 0x040028EB RID: 10475
	public bool MetEnclaveScaredPilgrim;

	// Token: 0x040028EC RID: 10476
	public NPCEncounterState EnclaveStateNPCStandard;

	// Token: 0x040028ED RID: 10477
	public NPCEncounterState EnclaveState_songKnightFan;

	// Token: 0x040028EE RID: 10478
	[DefaultValue(0)]
	public int enclaveLevel;

	// Token: 0x040028EF RID: 10479
	public bool enclaveDonation2_Available;

	// Token: 0x040028F0 RID: 10480
	public bool enclaveAddition_PinRack;

	// Token: 0x040028F1 RID: 10481
	public bool enclaveAddition_CloakLine;

	// Token: 0x040028F2 RID: 10482
	public bool enclaveNPC_songKnightFan;

	// Token: 0x040028F3 RID: 10483
	public bool savedGrindleInCitadel;

	// Token: 0x040028F4 RID: 10484
	public bool grindleEnclaveConvo;

	// Token: 0x040028F5 RID: 10485
	public int grindleChestLocation;

	// Token: 0x040028F6 RID: 10486
	public bool grindleChestEncountered;

	// Token: 0x040028F7 RID: 10487
	public bool grindleInSong_08;

	// Token: 0x040028F8 RID: 10488
	public bool seenGrindleInSong_08;

	// Token: 0x040028F9 RID: 10489
	public bool collectedWardKey;

	// Token: 0x040028FA RID: 10490
	public bool wardBossEncountered;

	// Token: 0x040028FB RID: 10491
	public bool wardBossDefeated;

	// Token: 0x040028FC RID: 10492
	public bool wardBossHatchOpened;

	// Token: 0x040028FD RID: 10493
	public bool collectedWardBossKey;

	// Token: 0x040028FE RID: 10494
	public bool wardWoken;

	// Token: 0x040028FF RID: 10495
	public bool garmondAidForumBattle;

	// Token: 0x04002900 RID: 10496
	public bool shakraAidForumBattle;

	// Token: 0x04002901 RID: 10497
	public bool hang_10_oneWay;

	// Token: 0x04002902 RID: 10498
	public bool bankOpened;

	// Token: 0x04002903 RID: 10499
	public bool rosaryThievesInBank;

	// Token: 0x04002904 RID: 10500
	public bool rosaryThievesLeftBank;

	// Token: 0x04002905 RID: 10501
	public bool destroyedRosaryCannonMachine;

	// Token: 0x04002906 RID: 10502
	public bool hang04Battle;

	// Token: 0x04002907 RID: 10503
	public bool leftTheGrandForum;

	// Token: 0x04002908 RID: 10504
	public bool grindleMetGrandForum;

	// Token: 0x04002909 RID: 10505
	public bool opened_cog_06_door;

	// Token: 0x0400290A RID: 10506
	public bool cog7_automaton_defeated;

	// Token: 0x0400290B RID: 10507
	public bool cog7_gateOpened;

	// Token: 0x0400290C RID: 10508
	public bool cog7_automatonRepairing;

	// Token: 0x0400290D RID: 10509
	public bool cog7_automatonRepairingComplete;

	// Token: 0x0400290E RID: 10510
	public bool cog7_automatonDestroyed;

	// Token: 0x0400290F RID: 10511
	public bool wokeSongChevalier;

	// Token: 0x04002910 RID: 10512
	public bool songChevalierActiveInSong_25;

	// Token: 0x04002911 RID: 10513
	public bool songChevalierSeenInSong_25;

	// Token: 0x04002912 RID: 10514
	public bool songChevalierActiveInSong_27;

	// Token: 0x04002913 RID: 10515
	public bool songChevalierSeenInSong_27;

	// Token: 0x04002914 RID: 10516
	public bool song_04_battleCompleted;

	// Token: 0x04002915 RID: 10517
	public bool songChevalierActiveInSong_04;

	// Token: 0x04002916 RID: 10518
	public bool songChevalierSeenInSong_04;

	// Token: 0x04002917 RID: 10519
	public bool songChevalierActiveInSong_02;

	// Token: 0x04002918 RID: 10520
	public bool songChevalierSeenInSong_02;

	// Token: 0x04002919 RID: 10521
	public bool songChevalierActiveInSong_07;

	// Token: 0x0400291A RID: 10522
	public bool songChevalierSeenInSong_07;

	// Token: 0x0400291B RID: 10523
	public bool songChevalierActiveInSong_24;

	// Token: 0x0400291C RID: 10524
	public bool songChevalierSeenInSong_24;

	// Token: 0x0400291D RID: 10525
	public bool songChevalierActiveInHang_02;

	// Token: 0x0400291E RID: 10526
	public bool songChevalierSeenInHang_02;

	// Token: 0x0400291F RID: 10527
	public bool songChevalierEncounterCooldown;

	// Token: 0x04002920 RID: 10528
	public int songChevalierEncounters;

	// Token: 0x04002921 RID: 10529
	public bool songChevalierRestingMet;

	// Token: 0x04002922 RID: 10530
	public bool songChevalierRestingMetAct3;

	// Token: 0x04002923 RID: 10531
	public bool songChevalierQuestReady;

	// Token: 0x04002924 RID: 10532
	public bool encounteredSongChevalierBoss;

	// Token: 0x04002925 RID: 10533
	public bool defeatedSongChevalierBoss;

	// Token: 0x04002926 RID: 10534
	public bool arborium_09_oneWay;

	// Token: 0x04002927 RID: 10535
	public bool arborium_08_oneWay;

	// Token: 0x04002928 RID: 10536
	public bool uncagedGiantFlea;

	// Token: 0x04002929 RID: 10537
	public bool tamedGiantFlea;

	// Token: 0x0400292A RID: 10538
	public bool encounteredSilk;

	// Token: 0x0400292B RID: 10539
	public bool soulSnareReady;

	// Token: 0x0400292C RID: 10540
	public bool caretakerSoulSnareConvo;

	// Token: 0x0400292D RID: 10541
	public bool encounteredSurfaceEdge;

	// Token: 0x0400292E RID: 10542
	public bool fullyEnteredVerdania;

	// Token: 0x0400292F RID: 10543
	public bool summonedLakeOrbs;

	// Token: 0x04002930 RID: 10544
	public bool encounteredWhiteCloverstagMid;

	// Token: 0x04002931 RID: 10545
	public bool encounteredWhiteCloverstag;

	// Token: 0x04002932 RID: 10546
	public bool defeatedWhiteCloverstag;

	// Token: 0x04002933 RID: 10547
	public bool encounteredCloverDancers;

	// Token: 0x04002934 RID: 10548
	public bool defeatedCloverDancers;

	// Token: 0x04002935 RID: 10549
	public bool memoryOrbs_Clover_02c_A;

	// Token: 0x04002936 RID: 10550
	public bool memoryOrbs_Clover_03_B;

	// Token: 0x04002937 RID: 10551
	public bool memoryOrbs_Clover_06_A;

	// Token: 0x04002938 RID: 10552
	public bool memoryOrbs_Clover_11;

	// Token: 0x04002939 RID: 10553
	public bool memoryOrbs_Clover_16_B;

	// Token: 0x0400293A RID: 10554
	public bool memoryOrbs_Clover_16_C;

	// Token: 0x0400293B RID: 10555
	public bool memoryOrbs_Clover_21;

	// Token: 0x0400293C RID: 10556
	public ulong memoryOrbs_Clover_18_A;

	// Token: 0x0400293D RID: 10557
	public ulong memoryOrbs_Clover_18_B;

	// Token: 0x0400293E RID: 10558
	public ulong memoryOrbs_Clover_18_C;

	// Token: 0x0400293F RID: 10559
	public ulong memoryOrbs_Clover_18_D;

	// Token: 0x04002940 RID: 10560
	public ulong memoryOrbs_Clover_18_E;

	// Token: 0x04002941 RID: 10561
	public ulong memoryOrbs_Clover_19;

	// Token: 0x04002942 RID: 10562
	public const int CLOVER_MEMORY_ORBS_TOTAL = 17;

	// Token: 0x04002943 RID: 10563
	public const int CLOVER_MEMORY_ORBS_TARGET = 12;

	// Token: 0x04002944 RID: 10564
	public bool completedSuperJumpSequence;

	// Token: 0x04002945 RID: 10565
	public bool completedAbyssAscent;

	// Token: 0x04002946 RID: 10566
	public bool blackThreadWorld;

	// Token: 0x04002947 RID: 10567
	public bool act3_wokeUp;

	// Token: 0x04002948 RID: 10568
	public bool completedCog10_abyssBattle;

	// Token: 0x04002949 RID: 10569
	public bool act3_enclaveWakeSceneCompleted;

	// Token: 0x0400294A RID: 10570
	public bool AbyssBellSeenDocks;

	// Token: 0x0400294B RID: 10571
	public bool AbyssBellSeenDocksRepaired;

	// Token: 0x0400294C RID: 10572
	public bool SatAtBenchAfterAbyssEscape;

	// Token: 0x0400294D RID: 10573
	public bool CollectedHeartFlower;

	// Token: 0x0400294E RID: 10574
	public bool CollectedHeartCoral;

	// Token: 0x0400294F RID: 10575
	public bool CollectedHeartHunter;

	// Token: 0x04002950 RID: 10576
	public bool CollectedHeartClover;

	// Token: 0x04002951 RID: 10577
	public bool ShamanRitualCursedConvo;

	// Token: 0x04002952 RID: 10578
	public bool CompletedRedMemory;

	// Token: 0x04002953 RID: 10579
	public bool LastDiveCursedConvo;

	// Token: 0x04002954 RID: 10580
	public bool SnailShamansCrestConvo;

	// Token: 0x04002955 RID: 10581
	public bool SnailShamansCloverHeartConvo;

	// Token: 0x04002956 RID: 10582
	public bool EncounteredLostLace;

	// Token: 0x04002957 RID: 10583
	public bool MetCaravanTroupeLeader;

	// Token: 0x04002958 RID: 10584
	public bool SeenFleaCaravan;

	// Token: 0x04002959 RID: 10585
	public bool FleaQuestOffered;

	// Token: 0x0400295A RID: 10586
	public bool CaravanPilgrimAttackComplete;

	// Token: 0x0400295B RID: 10587
	public CaravanTroupeLocations CaravanTroupeLocation;

	// Token: 0x0400295C RID: 10588
	public bool MetCaravanTroupeLeaderGreymoor;

	// Token: 0x0400295D RID: 10589
	public bool CaravanTroupeLeaderCanLeaveGreymoor;

	// Token: 0x0400295E RID: 10590
	public bool MetCaravanTroupeLeaderGreymoorScared;

	// Token: 0x0400295F RID: 10591
	public bool MetCaravanTroupeLeaderJudge;

	// Token: 0x04002960 RID: 10592
	public bool CaravanTroupeLeaderCanLeaveJudge;

	// Token: 0x04002961 RID: 10593
	public bool CaravanLechSaved;

	// Token: 0x04002962 RID: 10594
	public bool CaravanLechReturnedToCaravan;

	// Token: 0x04002963 RID: 10595
	public bool CaravanLechMet;

	// Token: 0x04002964 RID: 10596
	public bool CaravanLechSpaAcceptState;

	// Token: 0x04002965 RID: 10597
	public bool CaravanLechSpaAttacked;

	// Token: 0x04002966 RID: 10598
	public bool CaravanLechWoundedSpoken;

	// Token: 0x04002967 RID: 10599
	public bool CaravanLechAct3Convo;

	// Token: 0x04002968 RID: 10600
	public bool CaravanHauntedBellhartConvo_TroupeLeader;

	// Token: 0x04002969 RID: 10601
	public bool MetTroupeHunterWild;

	// Token: 0x0400296A RID: 10602
	public bool TroupeHunterWildAct3Convo;

	// Token: 0x0400296B RID: 10603
	public bool TroupeLeaderSpokenLech;

	// Token: 0x0400296C RID: 10604
	public bool TroupeLeaderSpokenHunter;

	// Token: 0x0400296D RID: 10605
	public bool SeenFleatopiaEmpty;

	// Token: 0x0400296E RID: 10606
	public bool TroupeLeaderSpokenFleatopiaSearch;

	// Token: 0x0400296F RID: 10607
	public bool FleaGamesCanStart;

	// Token: 0x04002970 RID: 10608
	public bool FleaGamesStarted;

	// Token: 0x04002971 RID: 10609
	public bool FleaGamesPinataHit;

	// Token: 0x04002972 RID: 10610
	public bool FleaGamesEnded;

	// Token: 0x04002973 RID: 10611
	public bool grishkinSethConvo;

	// Token: 0x04002974 RID: 10612
	public bool fleaGames_juggling_played;

	// Token: 0x04002975 RID: 10613
	public int fleaGames_juggling_highscore;

	// Token: 0x04002976 RID: 10614
	public bool fleaGames_bouncing_played;

	// Token: 0x04002977 RID: 10615
	public int fleaGames_bouncing_highscore;

	// Token: 0x04002978 RID: 10616
	public bool fleaGames_dodging_played;

	// Token: 0x04002979 RID: 10617
	public int fleaGames_dodging_highscore;

	// Token: 0x0400297A RID: 10618
	public bool FleaGamesEndedContinuedPlaying;

	// Token: 0x0400297B RID: 10619
	public bool FleaGamesSpiritScoreAdded;

	// Token: 0x0400297C RID: 10620
	public bool FleaGamesMementoGiven;

	// Token: 0x0400297D RID: 10621
	public List<int> FleasCollectedTargetOrder;

	// Token: 0x0400297E RID: 10622
	public bool SavedFlea_Bone_06;

	// Token: 0x0400297F RID: 10623
	public bool SavedFlea_Dock_16;

	// Token: 0x04002980 RID: 10624
	public bool SavedFlea_Bone_East_05;

	// Token: 0x04002981 RID: 10625
	public bool SavedFlea_Bone_East_17b;

	// Token: 0x04002982 RID: 10626
	public bool SavedFlea_Ant_03;

	// Token: 0x04002983 RID: 10627
	public bool SavedFlea_Greymoor_15b;

	// Token: 0x04002984 RID: 10628
	public bool SavedFlea_Greymoor_06;

	// Token: 0x04002985 RID: 10629
	public bool SavedFlea_Shellwood_03;

	// Token: 0x04002986 RID: 10630
	public bool SavedFlea_Bone_East_10_Church;

	// Token: 0x04002987 RID: 10631
	public bool SavedFlea_Coral_35;

	// Token: 0x04002988 RID: 10632
	public bool SavedFlea_Dust_12;

	// Token: 0x04002989 RID: 10633
	public bool SavedFlea_Dust_09;

	// Token: 0x0400298A RID: 10634
	public bool SavedFlea_Belltown_04;

	// Token: 0x0400298B RID: 10635
	public bool SavedFlea_Crawl_06;

	// Token: 0x0400298C RID: 10636
	public bool SavedFlea_Slab_Cell;

	// Token: 0x0400298D RID: 10637
	public bool SavedFlea_Shadow_28;

	// Token: 0x0400298E RID: 10638
	public bool SavedFlea_Dock_03d;

	// Token: 0x0400298F RID: 10639
	public bool SavedFlea_Under_23;

	// Token: 0x04002990 RID: 10640
	public bool SavedFlea_Shadow_10;

	// Token: 0x04002991 RID: 10641
	public bool SavedFlea_Song_14;

	// Token: 0x04002992 RID: 10642
	public bool SavedFlea_Coral_24;

	// Token: 0x04002993 RID: 10643
	public bool SavedFlea_Peak_05c;

	// Token: 0x04002994 RID: 10644
	public bool SavedFlea_Library_09;

	// Token: 0x04002995 RID: 10645
	public bool SavedFlea_Song_11;

	// Token: 0x04002996 RID: 10646
	public bool SavedFlea_Library_01;

	// Token: 0x04002997 RID: 10647
	public bool SavedFlea_Under_21;

	// Token: 0x04002998 RID: 10648
	public bool SavedFlea_Slab_06;

	// Token: 0x04002999 RID: 10649
	public bool MetSeamstress;

	// Token: 0x0400299A RID: 10650
	public bool SeamstressOfferedQuest;

	// Token: 0x0400299B RID: 10651
	public int SeamstressIdleTalkState;

	// Token: 0x0400299C RID: 10652
	public bool SeamstressCitadelConvo;

	// Token: 0x0400299D RID: 10653
	public bool SeamstressPinstressConvo;

	// Token: 0x0400299E RID: 10654
	public bool SeamstressAct3Convo;

	// Token: 0x0400299F RID: 10655
	public bool SeamstressBadgeConvo;

	// Token: 0x040029A0 RID: 10656
	public bool FreedCaravanSpider;

	// Token: 0x040029A1 RID: 10657
	public bool SeenCaravanSpider;

	// Token: 0x040029A2 RID: 10658
	public bool MetCaravanSpider;

	// Token: 0x040029A3 RID: 10659
	public string CaravanSpiderTargetScene;

	// Token: 0x040029A4 RID: 10660
	public float CaravanSpiderTravelDirection;

	// Token: 0x040029A5 RID: 10661
	public bool OpenedCoralCaravanSpider;

	// Token: 0x040029A6 RID: 10662
	public bool MetCaravanSpiderCoral;

	// Token: 0x040029A7 RID: 10663
	public bool CaravanSpiderPaidExtraBellhart;

	// Token: 0x040029A8 RID: 10664
	[DefaultValue("Dust_05")]
	public string MazeEntranceScene;

	// Token: 0x040029A9 RID: 10665
	[DefaultValue("left1")]
	public string MazeEntranceDoor;

	// Token: 0x040029AA RID: 10666
	[DefaultValue("Dust_05")]
	public string MazeEntranceInitialScene;

	// Token: 0x040029AB RID: 10667
	[DefaultValue("left1")]
	public string MazeEntranceInitialDoor;

	// Token: 0x040029AC RID: 10668
	public string PreviousMazeTargetDoor;

	// Token: 0x040029AD RID: 10669
	public string PreviousMazeScene;

	// Token: 0x040029AE RID: 10670
	public string PreviousMazeDoor;

	// Token: 0x040029AF RID: 10671
	public int CorrectMazeDoorsEntered;

	// Token: 0x040029B0 RID: 10672
	public int IncorrectMazeDoorsEntered;

	// Token: 0x040029B1 RID: 10673
	public bool EnteredMazeRestScene;

	// Token: 0x040029B2 RID: 10674
	public bool WasInSceneRace;

	// Token: 0x040029B3 RID: 10675
	public int SprintMasterCurrentRace;

	// Token: 0x040029B4 RID: 10676
	public bool SprintMasterExtraRaceAvailable;

	// Token: 0x040029B5 RID: 10677
	public bool SprintMasterExtraRaceDlg;

	// Token: 0x040029B6 RID: 10678
	public bool SprintMasterExtraRaceWon;

	// Token: 0x040029B7 RID: 10679
	public bool CurseKilledFlyBoneEast;

	// Token: 0x040029B8 RID: 10680
	public bool CurseKilledFlyGreymoor;

	// Token: 0x040029B9 RID: 10681
	public bool CurseKilledFlyShellwood;

	// Token: 0x040029BA RID: 10682
	public bool CurseKilledFlySwamp;

	// Token: 0x040029BB RID: 10683
	public bool act2Started;

	// Token: 0x040029BC RID: 10684
	public float completionPercentage;

	// Token: 0x040029BD RID: 10685
	public int mapKeyPref;

	// Token: 0x040029BE RID: 10686
	public bool promisedFirstWish;

	// Token: 0x040029BF RID: 10687
	[NonSerialized]
	public bool disablePause;

	// Token: 0x040029C0 RID: 10688
	[NonSerialized]
	public bool disableSaveQuit;

	// Token: 0x040029C1 RID: 10689
	[NonSerialized]
	public bool disableInventory;

	// Token: 0x040029C2 RID: 10690
	[NonSerialized]
	public bool isInventoryOpen;

	// Token: 0x040029C3 RID: 10691
	[NonSerialized]
	public bool disableSilkAbilities;

	// Token: 0x040029C4 RID: 10692
	public bool betaEnd;

	// Token: 0x040029C5 RID: 10693
	public bool newDatTraitorLord;

	// Token: 0x040029C6 RID: 10694
	public string bossReturnEntryGate;

	// Token: 0x040029C7 RID: 10695
	public int bossStatueTargetLevel;

	// Token: 0x040029C8 RID: 10696
	public string currentBossStatueCompletionKey;

	// Token: 0x040029C9 RID: 10697
	public BossSequenceController.BossSequenceData currentBossSequence;

	// Token: 0x040029CA RID: 10698
	public bool bossRushMode;

	// Token: 0x040029CB RID: 10699
	public List<string> unlockedBossScenes;

	// Token: 0x040029CC RID: 10700
	public bool unlockedNewBossStatue;

	// Token: 0x040029CD RID: 10701
	public bool hasGodfinder;

	// Token: 0x040029CE RID: 10702
	public bool queuedGodfinderIcon;

	// Token: 0x040029CF RID: 10703
	public bool InvPaneHasNew;

	// Token: 0x040029D0 RID: 10704
	public bool ToolPaneHasNew;

	// Token: 0x040029D1 RID: 10705
	public bool QuestPaneHasNew;

	// Token: 0x040029D2 RID: 10706
	public bool JournalPaneHasNew;

	// Token: 0x040029D3 RID: 10707
	[DefaultValue("Hunter")]
	public string CurrentCrestID;

	// Token: 0x040029D4 RID: 10708
	public string PreviousCrestID;

	// Token: 0x040029D5 RID: 10709
	public ToolCrestsData ToolEquips;

	// Token: 0x040029D6 RID: 10710
	[NonSerialized]
	public bool IsCurrentCrestTemp;

	// Token: 0x040029D7 RID: 10711
	public bool UnlockedExtraBlueSlot;

	// Token: 0x040029D8 RID: 10712
	public bool UnlockedExtraYellowSlot;

	// Token: 0x040029D9 RID: 10713
	public FloatingCrestSlotsData ExtraToolEquips;

	// Token: 0x040029DA RID: 10714
	public ToolItemsData Tools;

	// Token: 0x040029DB RID: 10715
	[NonSerialized]
	private Dictionary<string, int> toolAmountsOverride;

	// Token: 0x040029DC RID: 10716
	public ToolItemLiquidsData ToolLiquids;

	// Token: 0x040029DD RID: 10717
	public int ToolPouchUpgrades;

	// Token: 0x040029DE RID: 10718
	public int ToolKitUpgrades;

	// Token: 0x040029DF RID: 10719
	public bool LightningToolToggle;

	// Token: 0x040029E0 RID: 10720
	public bool SeenToolGetPrompt;

	// Token: 0x040029E1 RID: 10721
	public bool SeenToolWeaponGetPrompt;

	// Token: 0x040029E2 RID: 10722
	public bool SeenToolEquipPrompt;

	// Token: 0x040029E3 RID: 10723
	public bool SeenToolUsePrompt;

	// Token: 0x040029E4 RID: 10724
	public QuestCompletionData QuestCompletionData;

	// Token: 0x040029E5 RID: 10725
	public QuestRumourData QuestRumourData;

	// Token: 0x040029E6 RID: 10726
	public int ShellShards;

	// Token: 0x040029E7 RID: 10727
	public bool HasSeenGeo;

	// Token: 0x040029E8 RID: 10728
	public bool HasSeenGeoMid;

	// Token: 0x040029E9 RID: 10729
	public bool HasSeenGeoBig;

	// Token: 0x040029EA RID: 10730
	public bool HasSeenShellShards;

	// Token: 0x040029EB RID: 10731
	public bool HasSeenRation;

	// Token: 0x040029EC RID: 10732
	public int TempGeoStore;

	// Token: 0x040029ED RID: 10733
	public int TempShellShardStore;

	// Token: 0x040029EE RID: 10734
	public CollectableItemsData Collectables;

	// Token: 0x040029EF RID: 10735
	public CollectableRelicsData Relics;

	// Token: 0x040029F0 RID: 10736
	public bool UnlockedFastTravel;

	// Token: 0x040029F1 RID: 10737
	public FastTravelLocations FastTravelNPCLocation;

	// Token: 0x040029F2 RID: 10738
	public bool UnlockedFastTravelTeleport;

	// Token: 0x040029F3 RID: 10739
	[NonSerialized]
	public bool travelling;

	// Token: 0x040029F4 RID: 10740
	[NonSerialized]
	public string nextScene;

	// Token: 0x040029F5 RID: 10741
	[NonSerialized]
	public bool IsTeleporting;

	// Token: 0x040029F6 RID: 10742
	public bool UnlockedDocksStation;

	// Token: 0x040029F7 RID: 10743
	public bool UnlockedBoneforestEastStation;

	// Token: 0x040029F8 RID: 10744
	public bool UnlockedGreymoorStation;

	// Token: 0x040029F9 RID: 10745
	public bool UnlockedBelltownStation;

	// Token: 0x040029FA RID: 10746
	public bool UnlockedCoralTowerStation;

	// Token: 0x040029FB RID: 10747
	public bool UnlockedCityStation;

	// Token: 0x040029FC RID: 10748
	public bool UnlockedPeakStation;

	// Token: 0x040029FD RID: 10749
	public bool UnlockedShellwoodStation;

	// Token: 0x040029FE RID: 10750
	public bool UnlockedShadowStation;

	// Token: 0x040029FF RID: 10751
	public bool UnlockedAqueductStation;

	// Token: 0x04002A00 RID: 10752
	public bool bellCentipedeAppeared;

	// Token: 0x04002A01 RID: 10753
	public bool UnlockedSongTube;

	// Token: 0x04002A02 RID: 10754
	public bool UnlockedUnderTube;

	// Token: 0x04002A03 RID: 10755
	public bool UnlockedCityBellwayTube;

	// Token: 0x04002A04 RID: 10756
	public bool UnlockedHangTube;

	// Token: 0x04002A05 RID: 10757
	public bool UnlockedEnclaveTube;

	// Token: 0x04002A06 RID: 10758
	public bool UnlockedArboriumTube;

	// Token: 0x04002A07 RID: 10759
	[NonSerialized]
	public int MaggotCharmHits;

	// Token: 0x04002A08 RID: 10760
	public bool MushroomQuestFound1;

	// Token: 0x04002A09 RID: 10761
	public bool MushroomQuestFound2;

	// Token: 0x04002A0A RID: 10762
	public bool MushroomQuestFound3;

	// Token: 0x04002A0B RID: 10763
	public bool MushroomQuestFound4;

	// Token: 0x04002A0C RID: 10764
	public bool MushroomQuestFound5;

	// Token: 0x04002A0D RID: 10765
	public bool MushroomQuestFound6;

	// Token: 0x04002A0E RID: 10766
	public bool MushroomQuestFound7;

	// Token: 0x04002A0F RID: 10767
	public List<PlayerStory.EventInfo> StoryEvents;

	// Token: 0x04002A10 RID: 10768
	private static PlayerData _instance;

	// Token: 0x04002A11 RID: 10769
	private static BoolFieldAccessOptimizer<PlayerData> boolFieldAccessOptimizer = new BoolFieldAccessOptimizer<PlayerData>();

	// Token: 0x04002A12 RID: 10770
	private static FieldAccessOptimizer<PlayerData, int> intFieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, int>();

	// Token: 0x04002A13 RID: 10771
	private static FieldAccessOptimizer<PlayerData, float> floatFieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, float>();

	// Token: 0x04002A14 RID: 10772
	private static FieldAccessOptimizer<PlayerData, string> stringFieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, string>();

	// Token: 0x04002A15 RID: 10773
	private static FieldAccessOptimizer<PlayerData, Vector3> vector3FieldAccessOptimiser = new FieldAccessOptimizer<PlayerData, Vector3>();

	// Token: 0x02001783 RID: 6019
	private class MapBoolList
	{
		// Token: 0x17000F1A RID: 3866
		// (get) Token: 0x06008DD2 RID: 36306 RVA: 0x0028A670 File Offset: 0x00288870
		public bool HasAnyMap
		{
			get
			{
				foreach (Func<bool> func in this.mapGetters)
				{
					if (func != null && func())
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000F1B RID: 3867
		// (get) Token: 0x06008DD3 RID: 36307 RVA: 0x0028A6D0 File Offset: 0x002888D0
		public bool HasAllMaps
		{
			get
			{
				foreach (Func<bool> func in this.mapGetters)
				{
					if (func != null && !func())
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x17000F1C RID: 3868
		// (get) Token: 0x06008DD4 RID: 36308 RVA: 0x0028A730 File Offset: 0x00288930
		public int HasCount
		{
			get
			{
				int num = 0;
				foreach (Func<bool> func in this.mapGetters)
				{
					if (func != null && func())
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x06008DD5 RID: 36309 RVA: 0x0028A790 File Offset: 0x00288990
		public MapBoolList(PlayerData playerData)
		{
			if (playerData == null)
			{
				return;
			}
			this.BuildTargetList(playerData);
		}

		// Token: 0x06008DD6 RID: 36310 RVA: 0x0028A7B0 File Offset: 0x002889B0
		private void BuildTargetList(PlayerData dataSource)
		{
			if (dataSource == null)
			{
				return;
			}
			Type type = dataSource.GetType();
			Type typeFromHandle = typeof(bool);
			FieldInfo[] fields = type.GetFields();
			PropertyInfo[] properties = type.GetProperties();
			Func<string, bool> func = (string varName) => varName.StartsWith("Has") && varName.EndsWith("Map") && !varName.Equals("HasAnyMap");
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = array[i];
				if (fieldInfo.FieldType == typeFromHandle && func(fieldInfo.Name))
				{
					this.mapGetters.Add(() => (bool)fieldInfo.GetValue(dataSource));
				}
			}
			PropertyInfo[] array2 = properties;
			for (int i = 0; i < array2.Length; i++)
			{
				PropertyInfo propertyInfo = array2[i];
				if (propertyInfo.PropertyType == typeFromHandle && func(propertyInfo.Name))
				{
					this.mapGetters.Add(() => (bool)propertyInfo.GetValue(dataSource, null));
				}
			}
		}

		// Token: 0x04008E56 RID: 36438
		private List<Func<bool>> mapGetters = new List<Func<bool>>();
	}
}
