using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x020007D0 RID: 2000
public class SceneTeleportMap : ScriptableObject
{
	// Token: 0x170007F6 RID: 2038
	// (get) Token: 0x06004677 RID: 18039 RVA: 0x00131994 File Offset: 0x0012FB94
	private static SceneTeleportMap Instance
	{
		get
		{
			if (SceneTeleportMap._instance)
			{
				return SceneTeleportMap._instance;
			}
			SceneTeleportMap.Load();
			return SceneTeleportMap._instance;
		}
	}

	// Token: 0x170007F7 RID: 2039
	// (get) Token: 0x06004678 RID: 18040 RVA: 0x001319B2 File Offset: 0x0012FBB2
	// (set) Token: 0x06004679 RID: 18041 RVA: 0x001319CC File Offset: 0x0012FBCC
	public static int LintVer
	{
		get
		{
			if (!SceneTeleportMap.Instance)
			{
				return -1;
			}
			return SceneTeleportMap._instance.lintVer;
		}
		set
		{
			if (!SceneTeleportMap.Instance)
			{
				return;
			}
			SceneTeleportMap._instance.lintVer = value;
			SceneTeleportMap._instance.SetDirty();
		}
	}

	// Token: 0x0600467A RID: 18042 RVA: 0x001319F0 File Offset: 0x0012FBF0
	[RuntimeInitializeOnLoadMethod]
	private static void Load()
	{
		SceneTeleportMap._instance = Resources.Load<SceneTeleportMap>("SceneTeleportMap");
	}

	// Token: 0x0600467B RID: 18043 RVA: 0x00131A04 File Offset: 0x0012FC04
	public static void AddTransitionGate(string sceneName, string gateName)
	{
		if (!SceneTeleportMap.Instance)
		{
			return;
		}
		SceneTeleportMap.SceneInfo sceneInfo = SceneTeleportMap._instance.sceneList.GetSceneInfo(sceneName);
		if (sceneInfo.TransitionGates.Contains(gateName))
		{
			return;
		}
		sceneInfo.TransitionGates.Add(gateName);
		SceneTeleportMap._instance.SetDirty();
	}

	// Token: 0x0600467C RID: 18044 RVA: 0x00131A54 File Offset: 0x0012FC54
	public static void AddRespawnPoint(string sceneName, string pointName)
	{
		if (!SceneTeleportMap.Instance)
		{
			return;
		}
		SceneTeleportMap.SceneInfo sceneInfo = SceneTeleportMap._instance.sceneList.GetSceneInfo(sceneName);
		if (sceneInfo.RespawnPoints.Contains(pointName))
		{
			return;
		}
		sceneInfo.RespawnPoints.Add(pointName);
		SceneTeleportMap._instance.SetDirty();
	}

	// Token: 0x0600467D RID: 18045 RVA: 0x00131AA4 File Offset: 0x0012FCA4
	public static void AddMapZone(string sceneName, MapZone mapZone)
	{
		if (!SceneTeleportMap.Instance)
		{
			return;
		}
		SceneTeleportMap._instance.sceneList.GetSceneInfo(sceneName).MapZone = mapZone;
		SceneTeleportMap._instance.SetDirty();
	}

	// Token: 0x0600467E RID: 18046 RVA: 0x00131AD3 File Offset: 0x0012FCD3
	public static void RecordHash(string sceneName, string hash)
	{
		if (!SceneTeleportMap.Instance)
		{
			return;
		}
		SceneTeleportMap._instance.sceneList.GetSceneInfo(sceneName).SceneFileHash = hash;
		SceneTeleportMap._instance.SetDirty();
	}

	// Token: 0x0600467F RID: 18047 RVA: 0x00131B02 File Offset: 0x0012FD02
	public static void ClearInSceneLists(string sceneName)
	{
		if (!SceneTeleportMap.Instance)
		{
			return;
		}
		SceneTeleportMap.SceneInfo sceneInfo = SceneTeleportMap._instance.sceneList.GetSceneInfo(sceneName);
		sceneInfo.TransitionGates.Clear();
		sceneInfo.RespawnPoints.Clear();
		SceneTeleportMap._instance.SetDirty();
	}

	// Token: 0x06004680 RID: 18048 RVA: 0x00131B40 File Offset: 0x0012FD40
	public static Dictionary<string, SceneTeleportMap.SceneInfo> GetTeleportMap()
	{
		if (!SceneTeleportMap.Instance)
		{
			return null;
		}
		return SceneTeleportMap._instance.sceneList.GetAllSceneInfo();
	}

	// Token: 0x06004681 RID: 18049 RVA: 0x00131B5F File Offset: 0x0012FD5F
	public new void SetDirty()
	{
	}

	// Token: 0x040046E5 RID: 18149
	private const string OBJECT_NAME = "SceneTeleportMap";

	// Token: 0x040046E6 RID: 18150
	[SerializeField]
	private int lintVer;

	// Token: 0x040046E7 RID: 18151
	[SerializeField]
	private SceneTeleportMap.SerializableSceneList sceneList;

	// Token: 0x040046E8 RID: 18152
	private static SceneTeleportMap _instance;

	// Token: 0x02001AA5 RID: 6821
	[Serializable]
	public class SceneInfo
	{
		// Token: 0x04009A14 RID: 39444
		public string SceneFileHash;

		// Token: 0x04009A15 RID: 39445
		public MapZone MapZone;

		// Token: 0x04009A16 RID: 39446
		public List<string> TransitionGates = new List<string>();

		// Token: 0x04009A17 RID: 39447
		public List<string> RespawnPoints = new List<string>();
	}

	// Token: 0x02001AA6 RID: 6822
	[Serializable]
	private class SerializableSceneInfo : SerializableNamedData<SceneTeleportMap.SceneInfo>
	{
	}

	// Token: 0x02001AA7 RID: 6823
	[Serializable]
	private class SerializableSceneList : SerializableNamedList<SceneTeleportMap.SceneInfo, SceneTeleportMap.SerializableSceneInfo>
	{
		// Token: 0x0600979D RID: 38813 RVA: 0x002AADB8 File Offset: 0x002A8FB8
		public SceneTeleportMap.SceneInfo GetSceneInfo(string sceneName)
		{
			SceneTeleportMap.SceneInfo result;
			if (this.RuntimeData.TryGetValue(sceneName, out result))
			{
				return result;
			}
			SceneTeleportMap.SceneInfo sceneInfo = new SceneTeleportMap.SceneInfo();
			this.RuntimeData[sceneName] = sceneInfo;
			return sceneInfo;
		}

		// Token: 0x0600979E RID: 38814 RVA: 0x002AADEB File Offset: 0x002A8FEB
		public Dictionary<string, SceneTeleportMap.SceneInfo> GetAllSceneInfo()
		{
			return this.RuntimeData;
		}
	}
}
