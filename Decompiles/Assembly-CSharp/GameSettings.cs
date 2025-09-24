using System;
using GlobalEnums;
using InControl;
using UnityEngine;

// Token: 0x02000351 RID: 849
[Serializable]
public class GameSettings
{
	// Token: 0x06001D5B RID: 7515 RVA: 0x0008785A File Offset: 0x00085A5A
	public GameSettings()
	{
		this.ResetGameOptionsSettings();
		this.ResetAudioSettings();
		this.ResetVideoSettings();
	}

	// Token: 0x06001D5C RID: 7516 RVA: 0x00087874 File Offset: 0x00085A74
	public void LoadGameOptionsSettings()
	{
		GameSettings.LoadEnum<SupportedLanguages>("GameLang", ref this.gameLanguage, SupportedLanguages.EN);
		GameSettings.LoadInt("GameBackers", ref this.backerCredits, 0);
		GameSettings.LoadInt("GameNativePopups", ref this.showNativeAchievementPopups, 0);
		GameSettings.LoadInt("RumbleSetting", ref this.vibrationSetting, 0);
		GameSettings.LoadInt("CameraShakeSetting", ref this.cameraShakeSetting, 0);
		GameSettings.LoadInt("HudScaleSetting", ref this.hudScaleSetting, Platform.Current.DefaultHudSetting);
	}

	// Token: 0x06001D5D RID: 7517 RVA: 0x000878F8 File Offset: 0x00085AF8
	public void SaveGameOptionsSettings()
	{
		Platform.Current.LocalSharedData.SetInt("GameLang", (int)this.gameLanguage);
		Platform.Current.LocalSharedData.SetInt("GameBackers", this.backerCredits);
		Platform.Current.LocalSharedData.SetInt("GameNativePopups", this.showNativeAchievementPopups);
		Platform.Current.LocalSharedData.SetInt("RumbleSetting", this.vibrationSetting);
		Platform.Current.LocalSharedData.SetInt("CameraShakeSetting", this.cameraShakeSetting);
		Platform.Current.LocalSharedData.SetInt("HudScaleSetting", this.hudScaleSetting);
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06001D5E RID: 7518 RVA: 0x000879B0 File Offset: 0x00085BB0
	public void ResetGameOptionsSettings()
	{
		this.gameLanguage = SupportedLanguages.EN;
		this.backerCredits = 0;
		this.showNativeAchievementPopups = 0;
		this.vibrationSetting = 0;
		this.cameraShakeSetting = 0;
		if (Platform.Current)
		{
			this.hudScaleSetting = Platform.Current.DefaultHudSetting;
		}
	}

	// Token: 0x06001D5F RID: 7519 RVA: 0x00087A00 File Offset: 0x00085C00
	public void LoadVideoSettings()
	{
		if (this.CommandArgumentUsed("-resetres"))
		{
			Screen.SetResolution(1920, 1080, true);
		}
		GameSettings.LoadInt("VidFullscreen", ref this.fullScreen, 2);
		GameSettings.LoadInt("VidVSync", ref this.vSync, 1);
		GameSettings.LoadInt("VidDisplay", ref this.useDisplay, 0);
		GameSettings.LoadInt("VidTFR", ref this.targetFrameRate, 60);
		GameSettings.LoadInt("VidParticles", ref this.particleEffectsLevel, 1);
		GameSettings.LoadEnum<ShaderQualities>("ShaderQuality", ref this.shaderQuality, ShaderQualities.High);
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x00087A98 File Offset: 0x00085C98
	public void SaveVideoSettings()
	{
		Platform.Current.LocalSharedData.SetInt("VidFullscreen", this.fullScreen);
		Platform.Current.LocalSharedData.SetInt("VidVSync", this.vSync);
		Platform.Current.LocalSharedData.SetInt("VidDisplay", this.useDisplay);
		Platform.Current.LocalSharedData.SetInt("VidTFR", this.targetFrameRate);
		Platform.Current.LocalSharedData.SetInt("VidParticles", this.particleEffectsLevel);
		Platform.Current.LocalSharedData.SetInt("ShaderQuality", (int)this.shaderQuality);
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x00087B50 File Offset: 0x00085D50
	public void ResetVideoSettings()
	{
		this.fullScreen = 2;
		this.vSync = 1;
		this.useDisplay = 0;
		this.targetFrameRate = 60;
		this.particleEffectsLevel = 1;
		this.overscanAdjusted = 0;
		this.overScanAdjustment = 0f;
		this.brightnessAdjusted = 0;
		this.brightnessAdjustment = 20f;
		this.shaderQuality = ShaderQualities.High;
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x00087BAC File Offset: 0x00085DAC
	public void LoadOverscanSettings()
	{
		GameSettings.LoadFloat("VidOSValue", ref this.overScanAdjustment, 0f);
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x00087BC4 File Offset: 0x00085DC4
	public void SaveOverscanSettings()
	{
		Platform.Current.LocalSharedData.SetFloat("VidOSValue", this.overScanAdjustment);
		this.overscanAdjusted = 1;
		Platform.Current.LocalSharedData.SetInt("VidOSSet", this.overscanAdjusted);
		if (GameSettings._verboseMode)
		{
			GameSettings.LogSavedKey("VidOSValue", this.overScanAdjustment);
		}
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x00087C32 File Offset: 0x00085E32
	public void ResetOverscanSettings()
	{
		this.overScanAdjustment = 0f;
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x00087C3F File Offset: 0x00085E3F
	public void LoadOverscanConfigured()
	{
		GameSettings.LoadInt("VidOSSet", ref this.overscanAdjusted, 0);
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x00087C53 File Offset: 0x00085E53
	public void LoadBrightnessSettings()
	{
		GameSettings.LoadFloat("VidBrightValue", ref this.brightnessAdjustment, 20f);
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x00087C6C File Offset: 0x00085E6C
	public void SaveBrightnessSettings()
	{
		Platform.Current.LocalSharedData.SetFloat("VidBrightValue", this.brightnessAdjustment);
		this.brightnessAdjusted = 1;
		Platform.Current.LocalSharedData.SetInt("VidBrightSet", this.brightnessAdjusted);
		if (GameSettings._verboseMode)
		{
			GameSettings.LogSavedKey("VidBrightValue", this.brightnessAdjustment);
		}
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x00087CDA File Offset: 0x00085EDA
	public void ResetBrightnessSettings()
	{
		this.brightnessAdjustment = 20f;
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x00087CE7 File Offset: 0x00085EE7
	public void LoadBrightnessConfigured()
	{
		this.brightnessAdjusted = Platform.Current.LocalSharedData.GetInt("VidBrightSet", 0);
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x00087D04 File Offset: 0x00085F04
	public void LoadAudioSettings()
	{
		GameSettings.LoadFloat("MasterVolume", ref this.masterVolume, 10f);
		GameSettings.LoadFloat("MusicVolume", ref this.musicVolume, 10f);
		GameSettings.LoadFloat("SoundVolume", ref this.soundVolume, 10f);
		GameSettings.LoadBool("PlayerVoiceEnabled", ref this.playerVoiceEnabled, true);
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x00087D68 File Offset: 0x00085F68
	public void SaveAudioSettings()
	{
		Platform.Current.LocalSharedData.SetFloat("MasterVolume", this.masterVolume);
		Platform.Current.LocalSharedData.SetFloat("MusicVolume", this.musicVolume);
		Platform.Current.LocalSharedData.SetFloat("SoundVolume", this.soundVolume);
		Platform.Current.LocalSharedData.SetBool("PlayerVoiceEnabled", this.playerVoiceEnabled);
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x00087DEC File Offset: 0x00085FEC
	public void ResetAudioSettings()
	{
		this.masterVolume = 10f;
		this.musicVolume = 10f;
		this.soundVolume = 10f;
		this.playerVoiceEnabled = true;
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x00087E18 File Offset: 0x00086018
	public void LoadKeyboardSettings()
	{
		this.LoadAndUpgradeKeyboardKey("KeyJump", ref this.jumpKey, Key.Z);
		this.LoadAndUpgradeKeyboardKey("KeyAttack", ref this.attackKey, Key.X);
		this.LoadAndUpgradeKeyboardKey("KeyDash", ref this.dashKey, Key.C);
		this.LoadAndUpgradeKeyboardKey("KeyCast", ref this.castKey, Key.A);
		this.LoadAndUpgradeKeyboardKey("KeySupDash", ref this.superDashKey, Key.S);
		this.LoadAndUpgradeKeyboardKey("KeyDreamnail", ref this.dreamNailKey, Key.D);
		this.LoadAndUpgradeKeyboardKey("KeyQuickMap", ref this.quickMapKey, Key.Tab);
		this.LoadAndUpgradeKeyboardKey("KeyQuickCast", ref this.quickCastKey, Key.F);
		this.LoadAndUpgradeKeyboardKey("KeyTaunt", ref this.tauntKey, Key.V);
		this.LoadAndUpgradeKeyboardKey("KeyInventory", ref this.inventoryKey, Key.I);
		this.LoadAndUpgradeKeyboardKey("KeyInventoryMap", ref this.inventoryMapKey, Key.M);
		this.LoadAndUpgradeKeyboardKey("KeyInventoryJournal", ref this.inventoryJournalKey, Key.J);
		this.LoadAndUpgradeKeyboardKey("KeyInventoryTools", ref this.inventoryToolsKey, Key.Q);
		this.LoadAndUpgradeKeyboardKey("KeyInventoryQuests", ref this.inventoryQuestsKey, Key.T);
		this.LoadAndUpgradeKeyboardKey("KeyUp", ref this.upKey, Key.UpArrow);
		this.LoadAndUpgradeKeyboardKey("KeyDown", ref this.downKey, Key.DownArrow);
		this.LoadAndUpgradeKeyboardKey("KeyLeft", ref this.leftKey, Key.LeftArrow);
		this.LoadAndUpgradeKeyboardKey("KeyRight", ref this.rightKey, Key.RightArrow);
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x00087F7C File Offset: 0x0008617C
	private void LoadAndUpgradeKeyboardKey(string prefsKey, ref string setString, Key defaultKey)
	{
		string text = defaultKey.ToString();
		if (GameSettings.LoadString(prefsKey, ref setString, text))
		{
			return;
		}
		setString = text;
	}

	// Token: 0x06001D6F RID: 7535 RVA: 0x00087FA8 File Offset: 0x000861A8
	public void SaveKeyboardSettings()
	{
		Platform.Current.LocalSharedData.SetString("KeyJump", this.jumpKey);
		Platform.Current.LocalSharedData.SetString("KeyAttack", this.attackKey);
		Platform.Current.LocalSharedData.SetString("KeyDash", this.dashKey);
		Platform.Current.LocalSharedData.SetString("KeyCast", this.castKey);
		Platform.Current.LocalSharedData.SetString("KeySupDash", this.superDashKey);
		Platform.Current.LocalSharedData.SetString("KeyDreamnail", this.dreamNailKey);
		Platform.Current.LocalSharedData.SetString("KeyQuickMap", this.quickMapKey);
		Platform.Current.LocalSharedData.SetString("KeyQuickCast", this.quickCastKey);
		Platform.Current.LocalSharedData.SetString("KeyTaunt", this.tauntKey);
		Platform.Current.LocalSharedData.SetString("KeyInventory", this.inventoryKey);
		Platform.Current.LocalSharedData.SetString("KeyInventoryMap", this.inventoryMapKey);
		Platform.Current.LocalSharedData.SetString("KeyInventoryTools", this.inventoryToolsKey);
		Platform.Current.LocalSharedData.SetString("KeyInventoryJournal", this.inventoryJournalKey);
		Platform.Current.LocalSharedData.SetString("KeyInventoryQuests", this.inventoryQuestsKey);
		Platform.Current.LocalSharedData.SetString("KeyUp", this.upKey);
		Platform.Current.LocalSharedData.SetString("KeyDown", this.downKey);
		Platform.Current.LocalSharedData.SetString("KeyLeft", this.leftKey);
		Platform.Current.LocalSharedData.SetString("KeyRight", this.rightKey);
	}

	// Token: 0x06001D70 RID: 7536 RVA: 0x0008818C File Offset: 0x0008638C
	public bool LoadGamepadSettings(GamepadType gamepadType)
	{
		gamepadType = this.RemapGamepadTypeForSettings(gamepadType);
		if (gamepadType != GamepadType.NONE)
		{
			string key = "Controller" + gamepadType.ToString();
			string json = "";
			if (GameSettings.LoadString(key, ref json, ""))
			{
				this.controllerMapping = JsonUtility.FromJson<ControllerMapping>(json);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001D71 RID: 7537 RVA: 0x000881E0 File Offset: 0x000863E0
	public void SaveGamepadSettings(GamepadType gamepadType)
	{
		gamepadType = this.RemapGamepadTypeForSettings(gamepadType);
		string key = "Controller" + gamepadType.ToString();
		string text = JsonUtility.ToJson(this.controllerMapping);
		Platform.Current.LocalSharedData.SetString(key, text);
		GameSettings.LogSavedKey(key, text);
		Platform.Current.LocalSharedData.Save();
	}

	// Token: 0x06001D72 RID: 7538 RVA: 0x00088244 File Offset: 0x00086444
	public void ResetGamepadSettings(GamepadType gamepadType)
	{
		gamepadType = this.RemapGamepadTypeForSettings(gamepadType);
		this.controllerMapping = new ControllerMapping();
		this.controllerMapping.gamepadType = gamepadType;
		if (GameSettings._verboseMode)
		{
			Debug.LogFormat("ResetSettings - {0}", new object[]
			{
				gamepadType
			});
		}
	}

	// Token: 0x06001D73 RID: 7539 RVA: 0x00088294 File Offset: 0x00086494
	private GamepadType RemapGamepadTypeForSettings(GamepadType sourceType)
	{
		GamepadType gamepadType;
		if (sourceType == GamepadType.SWITCH_PRO_CONTROLLER || sourceType - GamepadType.SWITCH2_JOYCON_DUAL <= 1)
		{
			gamepadType = GamepadType.SWITCH_JOYCON_DUAL;
		}
		else
		{
			gamepadType = sourceType;
		}
		if (gamepadType != sourceType)
		{
			Debug.LogFormat("Remapped GamepadType from {0} to {1}", new object[]
			{
				sourceType.ToString(),
				gamepadType.ToString()
			});
		}
		return gamepadType;
	}

	// Token: 0x06001D74 RID: 7540 RVA: 0x000882EC File Offset: 0x000864EC
	public static bool LoadInt(string key, ref int val, int def)
	{
		if (Platform.Current.LocalSharedData.HasKey(key))
		{
			val = Platform.Current.LocalSharedData.GetInt(key, def);
			if (GameSettings._verboseMode)
			{
				GameSettings.LogLoadedKey(key, val);
			}
			return true;
		}
		val = def;
		if (GameSettings._verboseMode)
		{
			GameSettings.LogMissingKey(key);
		}
		return false;
	}

	// Token: 0x06001D75 RID: 7541 RVA: 0x00088340 File Offset: 0x00086540
	private static bool HasSetting(string key)
	{
		return Platform.Current.LocalSharedData.HasKey(key);
	}

	// Token: 0x06001D76 RID: 7542 RVA: 0x00088354 File Offset: 0x00086554
	private static bool LoadEnum<EnumTy>(string key, ref EnumTy val, EnumTy def)
	{
		int num = (int)((object)val);
		bool result = GameSettings.LoadInt(key, ref num, (int)((object)def));
		val = (EnumTy)((object)num);
		return result;
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x00088398 File Offset: 0x00086598
	private static bool LoadBool(string key, ref bool val, bool def)
	{
		int num = val ? 1 : 0;
		bool result = GameSettings.LoadInt(key, ref num, def ? 1 : 0);
		val = (num > 0);
		return result;
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x000883C4 File Offset: 0x000865C4
	private static bool LoadFloat(string key, ref float val, float def)
	{
		if (Platform.Current.LocalSharedData.HasKey(key))
		{
			val = Platform.Current.LocalSharedData.GetFloat(key, def);
			if (GameSettings._verboseMode)
			{
				GameSettings.LogLoadedKey(key, val);
			}
			return true;
		}
		val = def;
		if (GameSettings._verboseMode)
		{
			GameSettings.LogMissingKey(key);
		}
		return false;
	}

	// Token: 0x06001D79 RID: 7545 RVA: 0x00088418 File Offset: 0x00086618
	private static bool LoadString(string key, ref string val, string def)
	{
		if (Platform.Current.LocalSharedData.HasKey(key))
		{
			val = Platform.Current.LocalSharedData.GetString(key, def);
			if (GameSettings._verboseMode)
			{
				GameSettings.LogLoadedKey(key, val);
			}
			return true;
		}
		val = def;
		if (GameSettings._verboseMode)
		{
			GameSettings.LogMissingKey(key);
		}
		return false;
	}

	// Token: 0x06001D7A RID: 7546 RVA: 0x0008846C File Offset: 0x0008666C
	private static void LogMissingKey(string key)
	{
		Debug.LogFormat("LoadSettings - {0} setting not found. Loading defaults.", new object[]
		{
			key
		});
	}

	// Token: 0x06001D7B RID: 7547 RVA: 0x00088482 File Offset: 0x00086682
	private static void LogLoadedKey(string key, int value)
	{
		Debug.LogFormat("LoadSettings - {0} Loaded ({1})", new object[]
		{
			key,
			value
		});
	}

	// Token: 0x06001D7C RID: 7548 RVA: 0x000884A1 File Offset: 0x000866A1
	private static void LogLoadedKey(string key, float value)
	{
		Debug.LogFormat("LoadSettings - {0} Loaded ({1})", new object[]
		{
			key,
			value
		});
	}

	// Token: 0x06001D7D RID: 7549 RVA: 0x000884C0 File Offset: 0x000866C0
	private static void LogLoadedKey(string key, string value)
	{
		Debug.LogFormat("LoadSettings - {0} Loaded ({1})", new object[]
		{
			key,
			value
		});
	}

	// Token: 0x06001D7E RID: 7550 RVA: 0x000884DA File Offset: 0x000866DA
	private static void LogSavedKey(string key, int value)
	{
		Debug.LogFormat("SaveSettings - {0} Saved ({1})", new object[]
		{
			key,
			value
		});
	}

	// Token: 0x06001D7F RID: 7551 RVA: 0x000884F9 File Offset: 0x000866F9
	private static void LogSavedKey(string key, float value)
	{
		Debug.LogFormat("SaveSettings - {0} Saved ({1})", new object[]
		{
			key,
			value
		});
	}

	// Token: 0x06001D80 RID: 7552 RVA: 0x00088518 File Offset: 0x00086718
	private static void LogSavedKey(string key, string value)
	{
		Debug.LogFormat("SaveSettings - {0} Saved ({1})", new object[]
		{
			key,
			value
		});
	}

	// Token: 0x06001D81 RID: 7553 RVA: 0x00088534 File Offset: 0x00086734
	public bool CommandArgumentUsed(string arg)
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].Equals(arg))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04001C91 RID: 7313
	private static readonly bool _verboseMode;

	// Token: 0x04001C92 RID: 7314
	[Header("Game Settings")]
	public SupportedLanguages gameLanguage;

	// Token: 0x04001C93 RID: 7315
	public int backerCredits;

	// Token: 0x04001C94 RID: 7316
	public int showNativeAchievementPopups;

	// Token: 0x04001C95 RID: 7317
	public int vibrationSetting;

	// Token: 0x04001C96 RID: 7318
	public int cameraShakeSetting;

	// Token: 0x04001C97 RID: 7319
	public int hudScaleSetting;

	// Token: 0x04001C98 RID: 7320
	[Header("Audio Settings")]
	public float masterVolume;

	// Token: 0x04001C99 RID: 7321
	public float musicVolume;

	// Token: 0x04001C9A RID: 7322
	public float soundVolume;

	// Token: 0x04001C9B RID: 7323
	public bool playerVoiceEnabled;

	// Token: 0x04001C9C RID: 7324
	[Header("Video Settings")]
	public int fullScreen;

	// Token: 0x04001C9D RID: 7325
	public int vSync;

	// Token: 0x04001C9E RID: 7326
	public int useDisplay;

	// Token: 0x04001C9F RID: 7327
	public float overScanAdjustment;

	// Token: 0x04001CA0 RID: 7328
	public float brightnessAdjustment;

	// Token: 0x04001CA1 RID: 7329
	public int overscanAdjusted;

	// Token: 0x04001CA2 RID: 7330
	public int brightnessAdjusted;

	// Token: 0x04001CA3 RID: 7331
	public int targetFrameRate;

	// Token: 0x04001CA4 RID: 7332
	public int particleEffectsLevel;

	// Token: 0x04001CA5 RID: 7333
	public ShaderQualities shaderQuality;

	// Token: 0x04001CA6 RID: 7334
	[Header("Controller Settings")]
	public ControllerMapping controllerMapping;

	// Token: 0x04001CA7 RID: 7335
	[Header("Keyboard Settings")]
	public string jumpKey;

	// Token: 0x04001CA8 RID: 7336
	public string attackKey;

	// Token: 0x04001CA9 RID: 7337
	public string dashKey;

	// Token: 0x04001CAA RID: 7338
	public string castKey;

	// Token: 0x04001CAB RID: 7339
	public string superDashKey;

	// Token: 0x04001CAC RID: 7340
	public string dreamNailKey;

	// Token: 0x04001CAD RID: 7341
	public string tauntKey;

	// Token: 0x04001CAE RID: 7342
	public string quickMapKey;

	// Token: 0x04001CAF RID: 7343
	public string quickCastKey;

	// Token: 0x04001CB0 RID: 7344
	public string inventoryKey;

	// Token: 0x04001CB1 RID: 7345
	public string inventoryMapKey;

	// Token: 0x04001CB2 RID: 7346
	public string inventoryJournalKey;

	// Token: 0x04001CB3 RID: 7347
	public string inventoryToolsKey;

	// Token: 0x04001CB4 RID: 7348
	public string inventoryQuestsKey;

	// Token: 0x04001CB5 RID: 7349
	public string upKey;

	// Token: 0x04001CB6 RID: 7350
	public string downKey;

	// Token: 0x04001CB7 RID: 7351
	public string leftKey;

	// Token: 0x04001CB8 RID: 7352
	public string rightKey;
}
