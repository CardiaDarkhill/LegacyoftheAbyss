using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020007C0 RID: 1984
public class MazeController : MonoBehaviour
{
	// Token: 0x140000ED RID: 237
	// (add) Token: 0x060045DB RID: 17883 RVA: 0x0012FBC4 File Offset: 0x0012DDC4
	// (remove) Token: 0x060045DC RID: 17884 RVA: 0x0012FBFC File Offset: 0x0012DDFC
	public event Action DoorsLinked;

	// Token: 0x170007DC RID: 2012
	// (get) Token: 0x060045DD RID: 17885 RVA: 0x0012FC31 File Offset: 0x0012DE31
	public bool IsCapScene
	{
		get
		{
			return this.isCapScene;
		}
	}

	// Token: 0x170007DD RID: 2013
	// (get) Token: 0x060045DE RID: 17886 RVA: 0x0012FC39 File Offset: 0x0012DE39
	public IReadOnlyCollection<string> SceneNames
	{
		get
		{
			return this.sceneNames;
		}
	}

	// Token: 0x170007DE RID: 2014
	// (get) Token: 0x060045DF RID: 17887 RVA: 0x0012FC41 File Offset: 0x0012DE41
	// (set) Token: 0x060045E0 RID: 17888 RVA: 0x0012FC49 File Offset: 0x0012DE49
	public bool IsDoorLinkComplete { get; private set; }

	// Token: 0x170007DF RID: 2015
	// (get) Token: 0x060045E1 RID: 17889 RVA: 0x0012FC54 File Offset: 0x0012DE54
	public int CorrectDoorsLeft
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			return this.neededCorrectDoors - instance.CorrectMazeDoorsEntered;
		}
	}

	// Token: 0x170007E0 RID: 2016
	// (get) Token: 0x060045E2 RID: 17890 RVA: 0x0012FC74 File Offset: 0x0012DE74
	public int IncorrectDoorsLeft
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			return this.allowedIncorrectDoors - instance.IncorrectMazeDoorsEntered;
		}
	}

	// Token: 0x170007E1 RID: 2017
	// (get) Token: 0x060045E3 RID: 17891 RVA: 0x0012FC94 File Offset: 0x0012DE94
	// (set) Token: 0x060045E4 RID: 17892 RVA: 0x0012FC9B File Offset: 0x0012DE9B
	public static MazeController NewestInstance { get; private set; }

	// Token: 0x060045E5 RID: 17893 RVA: 0x0012FCA4 File Offset: 0x0012DEA4
	private void OnValidate()
	{
		if (this.neededCorrectDoors < 1)
		{
			this.neededCorrectDoors = 1;
		}
		if (this.allowedIncorrectDoors < 1)
		{
			this.allowedIncorrectDoors = 1;
		}
		if (this.restScenePoint > this.neededCorrectDoors)
		{
			this.restScenePoint = this.neededCorrectDoors;
		}
		if (!Application.isPlaying)
		{
			Shader.SetGlobalFloat(MazeController._fogRotationProp, 0f);
		}
	}

	// Token: 0x060045E6 RID: 17894 RVA: 0x0012FD01 File Offset: 0x0012DF01
	private void Awake()
	{
		this.OnValidate();
		MazeController.NewestInstance = this;
	}

	// Token: 0x060045E7 RID: 17895 RVA: 0x0012FD0F File Offset: 0x0012DF0F
	private void OnDestroy()
	{
		if (MazeController.NewestInstance == this)
		{
			MazeController.NewestInstance = null;
		}
	}

	// Token: 0x060045E8 RID: 17896 RVA: 0x0012FD24 File Offset: 0x0012DF24
	private void Start()
	{
		if (!this.startInactive)
		{
			this.Activate();
		}
	}

	// Token: 0x060045E9 RID: 17897 RVA: 0x0012FD34 File Offset: 0x0012DF34
	private MazeController.EntryMatch GetExitMatch()
	{
		PlayerData instance = PlayerData.instance;
		foreach (MazeController.EntryMatch entryMatch in this.entryMatchExit)
		{
			if (entryMatch.EntryScene == instance.MazeEntranceScene && instance.MazeEntranceDoor.StartsWith(entryMatch.EntryDoorDir))
			{
				return entryMatch;
			}
		}
		throw new UnityException("Ne exit matches found for maze entry scene");
	}

	// Token: 0x060045EA RID: 17898 RVA: 0x0012FD94 File Offset: 0x0012DF94
	public void Activate()
	{
		if (this.isActive)
		{
			return;
		}
		this.isActive = true;
		List<TransitionPoint> list = (this.isCapScene || this.entryDoors.Count > 0) ? this.entryDoors : (from door in TransitionPoint.TransitionPoints
		where door.gameObject.scene == base.gameObject.scene
		select door).ToList<TransitionPoint>();
		PlayerData instance = PlayerData.instance;
		if (base.gameObject.scene.name == "Dust_Maze_Last_Hall" && instance.PreviousMazeScene != this.exitSceneName)
		{
			instance.CorrectMazeDoorsEntered = this.neededCorrectDoors + 1;
			instance.IncorrectMazeDoorsEntered = 0;
			this.forceExit = true;
		}
		foreach (TransitionPoint door2 in list)
		{
			this.SubscribeDoorEntered(door2);
		}
		this.LinkDoors(list);
		if (this.isCapScene)
		{
			MazeController.ResetSaveData();
		}
		else
		{
			float t = Mathf.Clamp01((float)instance.CorrectMazeDoorsEntered / (float)this.neededCorrectDoors);
			float lerpedValue = this.GetExitMatch().FogRotationRange.GetLerpedValue(t);
			foreach (SetMaterialPropertyBlocks setMaterialPropertyBlocks in this.fogPropertyControllers)
			{
				if (setMaterialPropertyBlocks)
				{
					setMaterialPropertyBlocks.SetFloatModifier("_FogRotation", lerpedValue);
				}
			}
		}
		this.IsDoorLinkComplete = true;
		Action doorsLinked = this.DoorsLinked;
		if (doorsLinked == null)
		{
			return;
		}
		doorsLinked();
	}

	// Token: 0x060045EB RID: 17899 RVA: 0x0012FF14 File Offset: 0x0012E114
	private void LinkDoors(IReadOnlyList<TransitionPoint> totalDoors)
	{
		this.correctDoors.Clear();
		var list = (from door in totalDoors.Select(delegate(TransitionPoint door)
		{
			if (door == null)
			{
				return null;
			}
			string name = door.name;
			string doorDirMatch = this.GetDoorDirMatch(name);
			if (string.IsNullOrEmpty(doorDirMatch))
			{
				return null;
			}
			return new
			{
				Door = door,
				DoorName = name,
				TargetDoorMatch = doorDirMatch
			};
		})
		where door != null
		select door).ToList();
		Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
		List<KeyValuePair<string, SceneTeleportMap.SceneInfo>> list2 = (from kvp in teleportMap
		where kvp.Key != base.gameObject.scene.name && this.sceneNames.Contains(kvp.Key)
		select kvp).ToList<KeyValuePair<string, SceneTeleportMap.SceneInfo>>();
		PlayerData instance = PlayerData.instance;
		int num = list.Count;
		int num2 = instance.hasNeedolin ? Random.Range(0, num) : -1;
		list.Shuffle();
		int num3 = this.neededCorrectDoors - 1;
		if (!this.isCapScene)
		{
			string sceneName;
			IReadOnlyList<string> readOnlyList;
			bool flag;
			if (instance.CorrectMazeDoorsEntered >= num3)
			{
				sceneName = this.exitSceneName;
				if (this.forceExit)
				{
					readOnlyList = teleportMap[this.exitSceneName].TransitionGates;
				}
				else
				{
					MazeController.EntryMatch exitMatch = this.GetExitMatch();
					if (exitMatch != null)
					{
						readOnlyList = (from gate in teleportMap[this.exitSceneName].TransitionGates
						where gate.StartsWith(exitMatch.ExitDoorDir)
						select gate).ToList<string>();
					}
					else
					{
						readOnlyList = teleportMap[this.exitSceneName].TransitionGates;
					}
				}
				flag = true;
			}
			else
			{
				if (instance.CorrectMazeDoorsEntered < this.restScenePoint - 1 || instance.EnteredMazeRestScene)
				{
					goto IL_229;
				}
				sceneName = this.restSceneName;
				readOnlyList = teleportMap[this.restSceneName].TransitionGates;
				flag = false;
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				var <>f__AnonymousType = list[i];
				TransitionPoint door3 = <>f__AnonymousType.Door;
				string doorName = <>f__AnonymousType.DoorName;
				if (string.IsNullOrEmpty(instance.PreviousMazeTargetDoor) || !(instance.PreviousMazeTargetDoor == doorName))
				{
					bool flag2 = false;
					foreach (string doorName2 in readOnlyList)
					{
						if (this.TryMatchDoor(sceneName, doorName2, <>f__AnonymousType.TargetDoorMatch, door3, true))
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						num2 = -1;
						list.RemoveAt(i);
						num--;
						if (flag)
						{
							break;
						}
					}
				}
			}
		}
		IL_229:
		for (int j = 0; j < num; j++)
		{
			var <>f__AnonymousType2 = list[j];
			TransitionPoint door2 = <>f__AnonymousType2.Door;
			string doorName3 = <>f__AnonymousType2.DoorName;
			if (!string.IsNullOrEmpty(instance.PreviousMazeTargetDoor) && doorName3 == instance.PreviousMazeTargetDoor)
			{
				door2.SetTargetScene(instance.PreviousMazeScene);
				door2.entryPoint = instance.PreviousMazeDoor;
				if (instance.PreviousMazeScene == this.exitSceneName)
				{
					num2 = j;
				}
				else if (num > 1)
				{
					while (num2 == j)
					{
						num2 = Random.Range(0, num);
					}
				}
				if (num2 == j)
				{
					this.correctDoors.Add(door2);
				}
			}
			else
			{
				list2.Shuffle<KeyValuePair<string, SceneTeleportMap.SceneInfo>>();
				if (!string.IsNullOrEmpty(instance.PreviousMazeScene))
				{
					for (int k = list2.Count - 1; k >= 0; k--)
					{
						KeyValuePair<string, SceneTeleportMap.SceneInfo> item = list2[k];
						if (item.Key == instance.PreviousMazeScene)
						{
							list2.RemoveAt(k);
							list2.Add(item);
						}
					}
				}
				foreach (KeyValuePair<string, SceneTeleportMap.SceneInfo> keyValuePair in list2)
				{
					if (this.TryFindMatchingDoor(keyValuePair.Key, keyValuePair.Value.TransitionGates, <>f__AnonymousType2.TargetDoorMatch, door2, num2 == j))
					{
						break;
					}
				}
			}
		}
		if (this.isCapScene || this.correctDoors.Count > 0 || num2 < 0)
		{
			return;
		}
		this.LinkDoors(totalDoors);
	}

	// Token: 0x060045EC RID: 17900 RVA: 0x001302F0 File Offset: 0x0012E4F0
	private string GetDoorDirMatch(string doorName)
	{
		if (doorName.StartsWith("left"))
		{
			return "right";
		}
		if (doorName.StartsWith("right"))
		{
			return "left";
		}
		if (doorName.StartsWith("top"))
		{
			return "bot";
		}
		if (doorName.StartsWith("bot"))
		{
			return "top";
		}
		return null;
	}

	// Token: 0x060045ED RID: 17901 RVA: 0x0013034C File Offset: 0x0012E54C
	private bool TryFindMatchingDoor(string sceneName, List<string> transitionGates, string targetDoorMatch, TransitionPoint door, bool isCorrectDoor)
	{
		foreach (string doorName in transitionGates)
		{
			if (this.TryMatchDoor(sceneName, doorName, targetDoorMatch, door, isCorrectDoor))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060045EE RID: 17902 RVA: 0x001303AC File Offset: 0x0012E5AC
	private bool TryMatchDoor(string sceneName, string doorName, string targetDoorMatch, TransitionPoint door, bool isCorrectDoor)
	{
		if (!doorName.StartsWith(targetDoorMatch))
		{
			return false;
		}
		door.SetTargetScene(sceneName);
		door.entryPoint = doorName;
		if (isCorrectDoor)
		{
			this.correctDoors.Add(door);
		}
		return true;
	}

	// Token: 0x060045EF RID: 17903 RVA: 0x001303DC File Offset: 0x0012E5DC
	private void SubscribeDoorEntered(TransitionPoint door)
	{
		door.OnBeforeTransition += delegate()
		{
			PlayerData instance = PlayerData.instance;
			string name = door.name;
			if (!this.isCapScene)
			{
				if (door.targetScene == this.restSceneName)
				{
					instance.EnteredMazeRestScene = true;
					instance.CorrectMazeDoorsEntered = this.neededCorrectDoors - this.restScenePoint;
					instance.IncorrectMazeDoorsEntered = 0;
				}
				else if (!(instance.PreviousMazeTargetDoor == name))
				{
					if (this.correctDoors.Contains(door))
					{
						instance.CorrectMazeDoorsEntered++;
						instance.IncorrectMazeDoorsEntered = 0;
					}
					else
					{
						instance.CorrectMazeDoorsEntered = 0;
						instance.IncorrectMazeDoorsEntered++;
						instance.EnteredMazeRestScene = false;
						if (instance.IncorrectMazeDoorsEntered >= this.allowedIncorrectDoors && name.StartsWith("right"))
						{
							door.SetTargetScene("Dust_Maze_09_entrance");
							door.entryPoint = "left1";
						}
					}
				}
			}
			instance.PreviousMazeTargetDoor = door.entryPoint;
			instance.PreviousMazeScene = door.gameObject.scene.name;
			instance.PreviousMazeDoor = name;
		};
	}

	// Token: 0x060045F0 RID: 17904 RVA: 0x00130414 File Offset: 0x0012E614
	public static void ResetSaveData()
	{
		PlayerData instance = PlayerData.instance;
		instance.PreviousMazeTargetDoor = string.Empty;
		instance.PreviousMazeScene = string.Empty;
		instance.PreviousMazeDoor = string.Empty;
		instance.CorrectMazeDoorsEntered = 0;
		instance.IncorrectMazeDoorsEntered = 0;
		instance.EnteredMazeRestScene = false;
	}

	// Token: 0x060045F1 RID: 17905 RVA: 0x00130450 File Offset: 0x0012E650
	public IEnumerable<TransitionPoint> EnumerateCorrectDoors()
	{
		foreach (TransitionPoint transitionPoint in this.correctDoors)
		{
			yield return transitionPoint;
		}
		List<TransitionPoint>.Enumerator enumerator = default(List<TransitionPoint>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x04004684 RID: 18052
	[SerializeField]
	[Tooltip("Is this scene an entry or exit scene?")]
	private bool isCapScene;

	// Token: 0x04004685 RID: 18053
	[SerializeField]
	private List<TransitionPoint> entryDoors;

	// Token: 0x04004686 RID: 18054
	[Space]
	[SerializeField]
	private string[] sceneNames;

	// Token: 0x04004687 RID: 18055
	[SerializeField]
	private int neededCorrectDoors;

	// Token: 0x04004688 RID: 18056
	[SerializeField]
	private int allowedIncorrectDoors;

	// Token: 0x04004689 RID: 18057
	[Space]
	[SerializeField]
	private int restScenePoint;

	// Token: 0x0400468A RID: 18058
	[SerializeField]
	private string restSceneName;

	// Token: 0x0400468B RID: 18059
	[Space]
	[SerializeField]
	private string exitSceneName;

	// Token: 0x0400468C RID: 18060
	[SerializeField]
	private MazeController.EntryMatch[] entryMatchExit;

	// Token: 0x0400468D RID: 18061
	[Space]
	[SerializeField]
	private SetMaterialPropertyBlocks[] fogPropertyControllers;

	// Token: 0x0400468E RID: 18062
	[Space]
	[SerializeField]
	private bool startInactive;

	// Token: 0x0400468F RID: 18063
	private bool isActive;

	// Token: 0x04004690 RID: 18064
	private bool forceExit;

	// Token: 0x04004691 RID: 18065
	private readonly List<TransitionPoint> correctDoors = new List<TransitionPoint>();

	// Token: 0x04004692 RID: 18066
	private static readonly int _fogRotationProp = Shader.PropertyToID("_FogRotation");

	// Token: 0x02001A8C RID: 6796
	[Serializable]
	private class EntryMatch
	{
		// Token: 0x040099E3 RID: 39395
		public string EntryScene;

		// Token: 0x040099E4 RID: 39396
		public string EntryDoorDir;

		// Token: 0x040099E5 RID: 39397
		public string ExitDoorDir;

		// Token: 0x040099E6 RID: 39398
		public MinMaxFloat FogRotationRange;
	}
}
