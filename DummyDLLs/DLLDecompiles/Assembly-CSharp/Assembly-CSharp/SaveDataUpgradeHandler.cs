using System;
using System.Collections.Generic;

// Token: 0x02000366 RID: 870
public static class SaveDataUpgradeHandler
{
	// Token: 0x06001DFD RID: 7677 RVA: 0x0008A994 File Offset: 0x00088B94
	private static void ClearDreamGate(SaveDataUpgradeHandler.SceneSplit sceneSplit, ref string dreamGateScene)
	{
		if (sceneSplit.SceneName == dreamGateScene)
		{
			dreamGateScene = "";
		}
	}

	// Token: 0x06001DFE RID: 7678 RVA: 0x0008A9AC File Offset: 0x00088BAC
	private static void UpdateMap(SaveDataUpgradeHandler.SceneSplit sceneSplit, ref HashSet<string> scenesMapped)
	{
		if (!scenesMapped.Contains(sceneSplit.SceneName))
		{
			return;
		}
		foreach (string item in sceneSplit.NewSceneNames)
		{
			scenesMapped.Add(item);
		}
	}

	// Token: 0x06001DFF RID: 7679 RVA: 0x0008A9EC File Offset: 0x00088BEC
	public static void UpgradeSaveData(ref PlayerData playerData)
	{
		foreach (SaveDataUpgradeHandler.SceneSplit sceneSplit in SaveDataUpgradeHandler._splitScenes)
		{
			if (sceneSplit.ShouldHandleSplit(playerData.version))
			{
				SaveDataUpgradeHandler.UpdateMap(sceneSplit, ref playerData.scenesMapped);
			}
		}
	}

	// Token: 0x06001E00 RID: 7680 RVA: 0x0008AA30 File Offset: 0x00088C30
	public static void UpgradeSystemData<T>(T system)
	{
		Type typeFromHandle = typeof(T);
		if (!SaveDataUpgradeHandler.systemDataUpgrades.ContainsKey(typeFromHandle))
		{
			return;
		}
		string key = string.Format("lastSystemVersion_{0}", typeFromHandle);
		Version v = new Version(Platform.Current.LocalSharedData.GetString(key, "0.0.0.0"));
		SaveDataUpgradeHandler.SystemDataUpgrade systemDataUpgrade = SaveDataUpgradeHandler.systemDataUpgrades[typeFromHandle];
		if (v >= systemDataUpgrade.TargetVersion)
		{
			return;
		}
		systemDataUpgrade.UpgradeAction(system);
		Platform.Current.LocalSharedData.SetString(key, "1.0.28324");
	}

	// Token: 0x04001D1E RID: 7454
	private static readonly SaveDataUpgradeHandler.SceneSplit[] _splitScenes = new SaveDataUpgradeHandler.SceneSplit[0];

	// Token: 0x04001D1F RID: 7455
	private static readonly Dictionary<Type, SaveDataUpgradeHandler.SystemDataUpgrade> systemDataUpgrades = new Dictionary<Type, SaveDataUpgradeHandler.SystemDataUpgrade>();

	// Token: 0x02001619 RID: 5657
	private class SceneSplit
	{
		// Token: 0x17000E15 RID: 3605
		// (get) Token: 0x060088E8 RID: 35048 RVA: 0x0027B9E2 File Offset: 0x00279BE2
		// (set) Token: 0x060088E9 RID: 35049 RVA: 0x0027B9EA File Offset: 0x00279BEA
		public string SceneName { get; private set; }

		// Token: 0x17000E16 RID: 3606
		// (get) Token: 0x060088EA RID: 35050 RVA: 0x0027B9F3 File Offset: 0x00279BF3
		// (set) Token: 0x060088EB RID: 35051 RVA: 0x0027B9FB File Offset: 0x00279BFB
		public string Version { get; private set; }

		// Token: 0x17000E17 RID: 3607
		// (get) Token: 0x060088EC RID: 35052 RVA: 0x0027BA04 File Offset: 0x00279C04
		// (set) Token: 0x060088ED RID: 35053 RVA: 0x0027BA0C File Offset: 0x00279C0C
		public string[] NewSceneNames { get; private set; }

		// Token: 0x060088EE RID: 35054 RVA: 0x0027BA15 File Offset: 0x00279C15
		public SceneSplit(string sceneName, string version, params string[] newSceneNames)
		{
			this.SceneName = sceneName;
			this.Version = version;
			this.NewSceneNames = newSceneNames;
		}

		// Token: 0x060088EF RID: 35055 RVA: 0x0027BA34 File Offset: 0x00279C34
		public bool ShouldHandleSplit(string otherVersion)
		{
			otherVersion = SaveDataUtility.CleanupVersionText(otherVersion);
			Version version = new Version(this.Version);
			Version value = new Version(otherVersion);
			return version.CompareTo(value) > 0;
		}
	}

	// Token: 0x0200161A RID: 5658
	private struct SystemDataUpgrade
	{
		// Token: 0x040089B3 RID: 35251
		public Version TargetVersion;

		// Token: 0x040089B4 RID: 35252
		public Action<object> UpgradeAction;
	}
}
