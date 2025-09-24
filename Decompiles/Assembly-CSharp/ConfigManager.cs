using System;
using System.IO;
using UnityEngine;

// Token: 0x02000414 RID: 1044
public static class ConfigManager
{
	// Token: 0x1700039F RID: 927
	// (get) Token: 0x06002356 RID: 9046 RVA: 0x000A189E File Offset: 0x0009FA9E
	// (set) Token: 0x06002357 RID: 9047 RVA: 0x000A18A5 File Offset: 0x0009FAA5
	public static bool IsNativeInputEnabled { get; private set; }

	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06002358 RID: 9048 RVA: 0x000A18AD File Offset: 0x0009FAAD
	// (set) Token: 0x06002359 RID: 9049 RVA: 0x000A18B4 File Offset: 0x0009FAB4
	public static float ReducedCameraShake { get; private set; }

	// Token: 0x170003A1 RID: 929
	// (get) Token: 0x0600235A RID: 9050 RVA: 0x000A18BC File Offset: 0x0009FABC
	// (set) Token: 0x0600235B RID: 9051 RVA: 0x000A18C3 File Offset: 0x0009FAC3
	public static float ReducedControllerRumble { get; private set; }

	// Token: 0x170003A2 RID: 930
	// (get) Token: 0x0600235C RID: 9052 RVA: 0x000A18CB File Offset: 0x0009FACB
	// (set) Token: 0x0600235D RID: 9053 RVA: 0x000A18D2 File Offset: 0x0009FAD2
	public static bool IsNoiseEffectEnabled { get; private set; }

	// Token: 0x170003A3 RID: 931
	// (get) Token: 0x0600235E RID: 9054 RVA: 0x000A18DA File Offset: 0x0009FADA
	private static bool IsConfigFileSupported
	{
		get
		{
			return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer;
		}
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000A18F8 File Offset: 0x0009FAF8
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		ConfigManager._isInit = true;
		ConfigManager.IsNativeInputEnabled = true;
		ConfigManager.ReducedCameraShake = 0.2f;
		ConfigManager.ReducedControllerRumble = 0.4f;
		ConfigManager.IsNoiseEffectEnabled = false;
		if (!ConfigManager.IsConfigFileSupported)
		{
			return;
		}
		if (Platform.Current)
		{
			ConfigManager.LoadConfig();
			return;
		}
		Platform.PlatformBecameCurrent += ConfigManager.LoadConfig;
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000A1958 File Offset: 0x0009FB58
	private static void LoadConfig()
	{
		if (!ConfigManager.IsConfigFileSupported)
		{
			return;
		}
		if (!ConfigManager._isInit)
		{
			return;
		}
		if (File.Exists(ConfigManager._path))
		{
			INIParser iniparser = new INIParser();
			iniparser.Open(ConfigManager._path);
			ConfigManager.IsNoiseEffectEnabled = iniparser.ReadValue("VideoSettings", "NoiseEffect", ConfigManager.IsNoiseEffectEnabled);
			ConfigManager.IsNativeInputEnabled = iniparser.ReadValue("Input", "NativeInput", ConfigManager.IsNativeInputEnabled);
			ConfigManager.ReducedCameraShake = iniparser.ReadValue("Accessibility", "ReducedCameraShake", ConfigManager.ReducedCameraShake);
			ConfigManager.ReducedControllerRumble = iniparser.ReadValue("Accessibility", "ReducedControllerRumble", ConfigManager.ReducedControllerRumble);
			iniparser.Close();
		}
		ConfigManager.SaveConfig();
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000A1A04 File Offset: 0x0009FC04
	public static void SaveConfig()
	{
		if (!ConfigManager.IsConfigFileSupported)
		{
			return;
		}
		if (!ConfigManager._isInit)
		{
			return;
		}
		INIParser iniparser = new INIParser();
		iniparser.Open(ConfigManager._path);
		iniparser.WriteValue("VideoSettings", "NoiseEffect", ConfigManager.IsNoiseEffectEnabled);
		iniparser.WriteValue("Input", "NativeInput", ConfigManager.IsNativeInputEnabled);
		iniparser.WriteValue("Accessibility", "ReducedCameraShake", ConfigManager.ReducedCameraShake);
		iniparser.WriteValue("Accessibility", "ReducedControllerRumble", ConfigManager.ReducedControllerRumble);
		iniparser.Close();
	}

	// Token: 0x040021F2 RID: 8690
	private static bool _isInit;

	// Token: 0x040021F3 RID: 8691
	private static readonly string _path = ConfigManager.IsConfigFileSupported ? Path.Combine(Application.persistentDataPath, "AppConfig.ini") : null;
}
