using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000665 RID: 1637
public class GameMap : MonoBehaviour, IInitialisable
{
	// Token: 0x140000C4 RID: 196
	// (add) Token: 0x06003A4F RID: 14927 RVA: 0x000FF870 File Offset: 0x000FDA70
	// (remove) Token: 0x06003A50 RID: 14928 RVA: 0x000FF8A8 File Offset: 0x000FDAA8
	public event Action<bool, MapZone> UpdateQuickMapDisplay;

	// Token: 0x140000C5 RID: 197
	// (add) Token: 0x06003A51 RID: 14929 RVA: 0x000FF8E0 File Offset: 0x000FDAE0
	// (remove) Token: 0x06003A52 RID: 14930 RVA: 0x000FF918 File Offset: 0x000FDB18
	public event Action<Vector2> ViewPosUpdated;

	// Token: 0x17000697 RID: 1687
	// (get) Token: 0x06003A53 RID: 14931 RVA: 0x000FF94D File Offset: 0x000FDB4D
	public Collider2D ViewportEdge
	{
		get
		{
			if (!this.viewportEdge)
			{
				this.viewportEdge = base.transform.parent.GetComponent<Collider2D>();
			}
			return this.viewportEdge;
		}
	}

	// Token: 0x06003A54 RID: 14932 RVA: 0x000FF978 File Offset: 0x000FDB78
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<GameMap.ZoneInfo>(ref this.mapZoneInfo, typeof(MapZone));
		ArrayForEnumAttribute.EnsureArraySize<GameObject>(ref this.mapMarkerTemplates, typeof(MapMarkerMenu.MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize<Transform>(ref this.fleaPinParents, typeof(CaravanTroupeHunter.PinGroups));
		foreach (GameMap.ZoneInfo zoneInfo in this.mapZoneInfo)
		{
			if (zoneInfo.WideMapZoomPositionsOrdered == null || zoneInfo.WideMapZoomPositionsOrdered.Length == 0)
			{
				zoneInfo.WideMapZoomPositionsOrdered = new GameMap.ConditionalPosition[]
				{
					new GameMap.ConditionalPosition
					{
						Position = zoneInfo.WideMapZoomPosition
					}
				};
			}
		}
		foreach (GameMap.ZoneInfo zoneInfo2 in this.mapZoneInfo)
		{
			if (zoneInfo2.QuickMapPositionsOrdered == null || zoneInfo2.QuickMapPositionsOrdered.Length == 0)
			{
				zoneInfo2.QuickMapPositionsOrdered = new GameMap.ConditionalPosition[]
				{
					new GameMap.ConditionalPosition
					{
						Position = zoneInfo2.QuickMapPosition
					}
				};
			}
		}
	}

	// Token: 0x06003A55 RID: 14933 RVA: 0x000FFA58 File Offset: 0x000FDC58
	private void OnEnable()
	{
		this.gm = GameManager.instance;
		this.isMarkerZoom = false;
	}

	// Token: 0x06003A56 RID: 14934 RVA: 0x000FFA6C File Offset: 0x000FDC6C
	private void OnDisable()
	{
		this.isMarkerZoom = false;
	}

	// Token: 0x06003A57 RID: 14935 RVA: 0x000FFA78 File Offset: 0x000FDC78
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.compassIcon.SetActive(true);
		this.compassIcon.SetActive(false);
		this.OnValidate();
		int nameID = Shader.PropertyToID("_TimeOffset");
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.spawnedMapMarkers = new GameObject[this.mapMarkerTemplates.Length, 9];
		for (int i = 0; i < this.spawnedMapMarkers.GetLength(0); i++)
		{
			GameObject gameObject = this.mapMarkerTemplates[i];
			Transform parent = gameObject.transform.parent;
			if (parent != null)
			{
				this.markerParent = parent;
			}
			for (int j = 0; j < this.spawnedMapMarkers.GetLength(1); j++)
			{
				GameObject gameObject2 = (j == 0) ? gameObject : Object.Instantiate<GameObject>(gameObject, parent);
				gameObject2.transform.SetLocalPositionZ(Random.Range(0f, 0.001f));
				this.spawnedMapMarkers[i, j] = gameObject2;
				InvMarker componentInChildren = gameObject2.GetComponentInChildren<InvMarker>();
				componentInChildren.Colour = (MapMarkerMenu.MarkerTypes)i;
				componentInChildren.Index = j;
				materialPropertyBlock.SetFloat(nameID, Random.Range(0f, 10f));
				gameObject2.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
			}
		}
		SceneManager.sceneLoaded += this.OnSceneLoaded;
		MapPin[] componentsInChildren = base.GetComponentsInChildren<MapPin>(true);
		for (int k = 0; k < componentsInChildren.Length; k++)
		{
			componentsInChildren[k].AddPin();
		}
		return true;
	}

	// Token: 0x06003A58 RID: 14936 RVA: 0x000FFBEC File Offset: 0x000FDDEC
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		this.gm = GameManager.instance;
		this.inputHandler = this.gm.GetComponent<InputHandler>();
		this.DisableAllAreas();
		this.InitZoneMaps();
		this.LevelReady();
		this.CalculatePinAreaBounds();
		return true;
	}

	// Token: 0x06003A59 RID: 14937 RVA: 0x000FFC46 File Offset: 0x000FDE46
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06003A5A RID: 14938 RVA: 0x000FFC4F File Offset: 0x000FDE4F
	private void Start()
	{
		this.OnStart();
	}

	// Token: 0x06003A5B RID: 14939 RVA: 0x000FFC58 File Offset: 0x000FDE58
	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
		MapPin.ClearActivePins();
	}

	// Token: 0x06003A5C RID: 14940 RVA: 0x000FFC70 File Offset: 0x000FDE70
	private void InitZoneMaps()
	{
		if (this.initZoneMaps)
		{
			return;
		}
		this.initZoneMaps = true;
		GameObject gameObject = base.gameObject;
		foreach (GameMap.ZoneInfo zoneInfo in this.mapZoneInfo)
		{
			zoneInfo.SetRoot(gameObject);
			zoneInfo.GetComponents();
		}
		this.UpdateMapCache();
	}

	// Token: 0x06003A5D RID: 14941 RVA: 0x000FFCC0 File Offset: 0x000FDEC0
	public void LevelReady()
	{
		if (!this.gm.IsGameplayScene())
		{
			return;
		}
		this.overriddenSceneName = null;
		string sceneNameString = this.gm.GetSceneNameString();
		this.currentSceneMapZone = this.GetMapZoneFromSceneName(sceneNameString);
		this.currentRegionMapZone = MapZone.NONE;
		this.corpseSceneMapZone = this.GetMapZoneFromSceneName(this.gm.playerData.HeroCorpseScene);
		tk2dTileMap tilemap = this.gm.tilemap;
		if (!tilemap)
		{
			Debug.LogError("gm.tilemap is null! Refreshing tilemap info manually", this);
			this.gm.RefreshTilemapInfo(sceneNameString);
		}
		if (tilemap)
		{
			this.currentSceneSize = new Vector2((float)tilemap.width, (float)tilemap.height);
			return;
		}
		Debug.LogError("gm.tilemap is null!", this);
		this.currentSceneSize = new Vector2(float.MaxValue, float.MaxValue);
	}

	// Token: 0x06003A5E RID: 14942 RVA: 0x000FFD8B File Offset: 0x000FDF8B
	public void OverrideMapZoneFromScene(string sceneName)
	{
		this.currentRegionMapZone = this.GetMapZoneFromSceneName(sceneName);
	}

	// Token: 0x06003A5F RID: 14943 RVA: 0x000FFD9A File Offset: 0x000FDF9A
	public void OverrideSceneName(string sceneName)
	{
		this.overriddenSceneName = sceneName;
		this.overriddenSceneRegion = this.GetMapZoneFromSceneName(sceneName);
		this.UpdateCurrentScene();
	}

	// Token: 0x06003A60 RID: 14944 RVA: 0x000FFDB6 File Offset: 0x000FDFB6
	public void ClearOverriddenSceneName(string sceneName)
	{
		if (this.overriddenSceneName != sceneName)
		{
			return;
		}
		this.overriddenSceneName = null;
		this.UpdateCurrentScene();
	}

	// Token: 0x06003A61 RID: 14945 RVA: 0x000FFDD4 File Offset: 0x000FDFD4
	private MapZone GetMapZoneFromSceneName(string sceneName)
	{
		for (int i = 0; i < this.mapZoneInfo.Length; i++)
		{
			GameMap.ParentInfo[] parents = this.mapZoneInfo[i].Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				GameObject parent = parents[j].Parent;
				if (parent)
				{
					using (IEnumerator enumerator = parent.transform.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (!(((Transform)enumerator.Current).name != sceneName))
							{
								return (MapZone)i;
							}
						}
					}
				}
			}
		}
		return MapZone.NONE;
	}

	// Token: 0x06003A62 RID: 14946 RVA: 0x000FFE88 File Offset: 0x000FE088
	public MapZone GetMapZoneForScene(Transform scene)
	{
		for (int i = 0; i < this.mapZoneInfo.Length; i++)
		{
			GameMap.ParentInfo[] parents = this.mapZoneInfo[i].Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				GameObject parent = parents[j].Parent;
				if (parent)
				{
					using (IEnumerator enumerator = parent.transform.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if ((Transform)enumerator.Current == scene)
							{
								return (MapZone)i;
							}
						}
					}
				}
			}
		}
		return MapZone.NONE;
	}

	// Token: 0x06003A63 RID: 14947 RVA: 0x000FFF34 File Offset: 0x000FE134
	private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
	{
		this.LevelReady();
	}

	// Token: 0x06003A64 RID: 14948 RVA: 0x000FFF3C File Offset: 0x000FE13C
	public bool IsLostInAbyssPreMap()
	{
		return this.IsLostInAbyssBase() && !this.gm.playerData.HasAbyssMap && !this.IsLostInAbyssEnded();
	}

	// Token: 0x06003A65 RID: 14949 RVA: 0x000FFF63 File Offset: 0x000FE163
	public bool IsLostInAbyssPostMap()
	{
		return (this.IsLostInAbyssBase() || this.gm.GetSceneNameString() == "Dock_06_Church") && this.gm.playerData.HasAbyssMap && !this.IsLostInAbyssEnded();
	}

	// Token: 0x06003A66 RID: 14950 RVA: 0x000FFFA1 File Offset: 0x000FE1A1
	private bool IsLostInAbyssBase()
	{
		return !this.gm.playerData.HasWhiteFlower && this.gm.GetCurrentMapZoneEnum() == MapZone.ABYSS;
	}

	// Token: 0x06003A67 RID: 14951 RVA: 0x000FFFC6 File Offset: 0x000FE1C6
	private bool IsLostInAbyssEnded()
	{
		return this.gm.playerData.SatAtBenchAfterAbyssEscape || this.gm.playerData.QuestCompletionData.GetData("Black Thread Pt3 Escape").IsCompleted;
	}

	// Token: 0x06003A68 RID: 14952 RVA: 0x000FFFFC File Offset: 0x000FE1FC
	public void SetupMap(bool pinsOnly = false)
	{
		PlayerData instance = PlayerData.instance;
		int num = instance.scenesMapped.Count + instance.scenesVisited.Count;
		bool flag = false;
		if (this.lastMappedCount != num)
		{
			this.lastMappedCount = num;
			flag = true;
		}
		bool flag2 = CollectableItemManager.IsInHiddenMode();
		this.unlockedMapZones.Clear();
		foreach (GameMap.ZoneInfo zoneInfo in this.mapZoneInfo)
		{
			bool flag3 = false;
			foreach (GameMap.ParentInfo parentInfo in zoneInfo.Parents)
			{
				parentInfo.Validate();
				if (parentInfo.HasParent)
				{
					parentInfo.CheckActivation();
					if (parentInfo.HasPositionConditions)
					{
						parentInfo.PositionConditions.Evaluate();
					}
					if (!flag3 && parentInfo.IsUnlocked)
					{
						flag3 = true;
					}
					foreach (GameMap.ZoneInfo.MapCache mapCache in parentInfo.Maps)
					{
						if (mapCache.hasGameMap)
						{
							GameMapScene gameMapScene = mapCache.gameMapScene;
							GameObject gameObject = mapCache.gameObject;
							string sceneName = mapCache.sceneName;
							bool flag4 = gameMapScene.IsMapped;
							if (flag && !flag4)
							{
								flag4 = (instance.mapAllRooms || instance.scenesMapped.Contains(sceneName) || gameMapScene.IsOtherMapped(instance.scenesMapped));
								if (flag4)
								{
									zoneInfo.SetBoundsDirty();
								}
							}
							bool flag5 = flag4 || instance.scenesVisited.Contains(sceneName);
							if (flag5)
							{
								gameMapScene.SetVisited();
							}
							if (gameMapScene.InitialState == GameMapScene.States.Full || (flag4 && !flag2))
							{
								if (instance.hasQuill && !pinsOnly)
								{
									gameMapScene.SetMapped();
								}
								for (int k = 0; k < gameObject.transform.childCount; k++)
								{
									GameObject gameObject2 = gameObject.transform.GetChild(k).gameObject;
									if (gameObject2.name == "pin_blue_health" && !gameObject2.activeSelf)
									{
										if (instance.scenesEncounteredCocoon.Contains(sceneName) && instance.hasPinCocoon)
										{
											gameObject2.SetActive(true);
										}
									}
									else
									{
										gameObject2.SetActive(true);
									}
									GameMapPinLayout component = gameObject2.GetComponent<GameMapPinLayout>();
									if (component)
									{
										component.Evaluate();
									}
								}
							}
							else
							{
								gameMapScene.SetNotMapped();
								for (int l = 0; l < gameObject.transform.childCount; l++)
								{
									GameObject gameObject3 = gameObject.transform.GetChild(l).gameObject;
									bool active = false;
									if (flag5)
									{
										if (gameMapScene.InitialState == GameMapScene.States.Rough && gameObject3.GetComponent<MapPin>())
										{
											active = true;
										}
										else if (gameObject3.GetComponent<TextMeshPro>())
										{
											active = true;
										}
										if (gameMapScene.InitialState == GameMapScene.States.Rough)
										{
											GameMapPinLayout component2 = gameObject3.GetComponent<GameMapPinLayout>();
											if (component2)
											{
												component2.Evaluate();
												active = true;
											}
										}
									}
									gameObject3.SetActive(active);
								}
							}
							if (mapCache.gameObject && !mapCache.gameObject.activeSelf)
							{
								mapCache.gameObject.SetActive(true);
								mapCache.gameObject.SetActive(false);
							}
						}
					}
				}
			}
			if (flag3)
			{
				this.unlockedMapZones.Add(zoneInfo);
			}
		}
		this.StartCalculatingVisibleLocalBoundsAsync();
	}

	// Token: 0x06003A69 RID: 14953 RVA: 0x0010036C File Offset: 0x000FE56C
	public void InitPinUpdate()
	{
		if (!this.doneInitialLoad)
		{
			this.doneInitialLoad = true;
			if (MapPin.DidActivateNewPin)
			{
				MapPin.DidActivateNewPin = false;
			}
		}
	}

	// Token: 0x06003A6A RID: 14954 RVA: 0x0010038C File Offset: 0x000FE58C
	private void UpdateMapCache()
	{
		this.mapCaches.Clear();
		foreach (GameMap.ZoneInfo zoneInfo in this.mapZoneInfo)
		{
			if (zoneInfo.Parents != null)
			{
				GameMap.ParentInfo[] parents = zoneInfo.Parents;
				for (int j = 0; j < parents.Length; j++)
				{
					foreach (GameMap.ZoneInfo.MapCache mapCache in parents[j].Maps)
					{
						List<GameMap.ZoneInfo.MapCache> list;
						if (!this.mapCaches.TryGetValue(mapCache.sceneName, out list))
						{
							list = new List<GameMap.ZoneInfo.MapCache>();
							this.mapCaches.Add(mapCache.sceneName, list);
						}
						list.Add(mapCache);
					}
				}
			}
		}
	}

	// Token: 0x06003A6B RID: 14955 RVA: 0x00100468 File Offset: 0x000FE668
	public bool HasRemainingPinFor(CaravanTroupeHunter.PinGroups pinGroup)
	{
		PlayerData instance = PlayerData.instance;
		Transform transform = this.fleaPinParents[(int)pinGroup];
		if (!transform)
		{
			return false;
		}
		foreach (object obj in transform)
		{
			GameObject gameObject = ((Transform)obj).gameObject;
			if (!instance.GetVariable(gameObject.name))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003A6C RID: 14956 RVA: 0x001004F0 File Offset: 0x000FE6F0
	private void SetDisplayNextArea(bool isQuickMap, MapZone mapZone)
	{
		MapPin.ToggleQuickMapView(isQuickMap);
		Action<bool, MapZone> updateQuickMapDisplay = this.UpdateQuickMapDisplay;
		if (updateQuickMapDisplay == null)
		{
			return;
		}
		updateQuickMapDisplay(isQuickMap, mapZone);
	}

	// Token: 0x06003A6D RID: 14957 RVA: 0x0010050C File Offset: 0x000FE70C
	public void WorldMap()
	{
		this.shadeMarker.SetActive(true);
		this.SetupMapMarkers();
		this.EnableUnlockedAreas(null);
		this.PositionCompassAndCorpse();
		this.SetDisplayNextArea(false, MapZone.NONE);
		this.CalculateMapScrollBounds();
	}

	// Token: 0x06003A6E RID: 14958 RVA: 0x0010054E File Offset: 0x000FE74E
	public MapZone GetCurrentMapZone()
	{
		if (this.currentRegionMapZone <= MapZone.NONE)
		{
			return this.currentSceneMapZone;
		}
		return this.currentRegionMapZone;
	}

	// Token: 0x06003A6F RID: 14959 RVA: 0x00100566 File Offset: 0x000FE766
	public MapZone GetCorpseMapZone()
	{
		return this.corpseSceneMapZone;
	}

	// Token: 0x06003A70 RID: 14960 RVA: 0x00100570 File Offset: 0x000FE770
	public Vector3 GetClosestUnlockedPoint(Vector2 position)
	{
		if (this.gm == null)
		{
			return position;
		}
		Vector3 result = position;
		float num = float.MaxValue;
		foreach (GameMap.ZoneInfo zoneInfo in this.unlockedMapZones)
		{
			Vector2 wideMapZoomPosition = zoneInfo.GetWideMapZoomPosition(this.gm);
			float num2 = Vector2.SqrMagnitude(wideMapZoomPosition - position);
			if (num2 < num)
			{
				num = num2;
				result = wideMapZoomPosition;
				if (num < 0.5f)
				{
					break;
				}
			}
		}
		return result;
	}

	// Token: 0x06003A71 RID: 14961 RVA: 0x00100614 File Offset: 0x000FE814
	public InventoryItemWideMapZone GetClosestWideMapZone(IEnumerable<InventoryItemWideMapZone> wideMapPieces)
	{
		if (this.mapZoneDistances == null)
		{
			this.mapZoneDistances = new List<ValueTuple<MapZone, float>>(this.mapZoneInfo.Length);
		}
		InventoryItemWideMapZone result;
		try
		{
			Vector3 v = -base.transform.localPosition;
			for (int i = 0; i < this.mapZoneInfo.Length; i++)
			{
				GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[i];
				MapZone item = (MapZone)i;
				Transform x = null;
				float num = float.MaxValue;
				foreach (GameMap.ParentInfo parentInfo in zoneInfo.Parents)
				{
					if (parentInfo.Parent)
					{
						for (int k = 0; k < parentInfo.Parent.transform.childCount; k++)
						{
							Transform child = parentInfo.Parent.transform.GetChild(k);
							Vector2 localScenePos = GameMap.GetLocalScenePos(child);
							float num2 = Vector2.Distance(v, localScenePos);
							if (num2 <= num)
							{
								x = child;
								num = num2;
							}
						}
					}
				}
				if (!(x == null))
				{
					this.mapZoneDistances.Add(new ValueTuple<MapZone, float>(item, num));
				}
			}
			this.mapZoneDistances.Sort(([TupleElementNames(new string[]
			{
				"mapZone",
				"distance"
			})] ValueTuple<MapZone, float> a, [TupleElementNames(new string[]
			{
				"mapZone",
				"distance"
			})] ValueTuple<MapZone, float> b) => a.Item2.CompareTo(b.Item2));
			foreach (ValueTuple<MapZone, float> valueTuple in this.mapZoneDistances)
			{
				MapZone item2 = valueTuple.Item1;
				foreach (InventoryItemWideMapZone inventoryItemWideMapZone in wideMapPieces)
				{
					if (inventoryItemWideMapZone.ZoomToZone == item2)
					{
						return inventoryItemWideMapZone;
					}
				}
			}
			result = wideMapPieces.FirstOrDefault<InventoryItemWideMapZone>();
		}
		finally
		{
			this.mapZoneDistances.Clear();
		}
		return result;
	}

	// Token: 0x06003A72 RID: 14962 RVA: 0x00100820 File Offset: 0x000FEA20
	private static Vector2 GetLocalScenePos(Transform scene)
	{
		Component parent = scene.transform.parent;
		Vector3 localPosition = scene.transform.localPosition;
		Vector3 localPosition2 = parent.transform.localPosition;
		return new Vector3(localPosition.x + localPosition2.x, localPosition.y + localPosition2.y, 0f);
	}

	// Token: 0x06003A73 RID: 14963 RVA: 0x00100878 File Offset: 0x000FEA78
	public bool TryOpenQuickMap(out string displayName)
	{
		displayName = string.Empty;
		MapZone currentMapZone = this.GetCurrentMapZone();
		if (currentMapZone != MapZone.ABYSS && (this.IsLostInAbyssPostMap() || this.IsLostInAbyssPreMap()))
		{
			return false;
		}
		GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[(int)currentMapZone];
		bool flag = false;
		GameMap.ParentInfo[] parents = zoneInfo.Parents;
		for (int i = 0; i < parents.Length; i++)
		{
			if (parents[i].IsUnlocked)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		this.DisableAllAreas();
		this.EnableUnlockedAreas(new MapZone?(currentMapZone));
		displayName = (zoneInfo.NameOverride.IsEmpty ? Language.Get(currentMapZone.ToString(), "Map Zones").Replace("<br>", string.Empty) : zoneInfo.NameOverride);
		base.transform.localScale = new Vector3(1.4725f, 1.4725f, 1f);
		Vector2 quickMapPosition = zoneInfo.GetQuickMapPosition(this.gm);
		base.transform.SetLocalPosition2D(quickMapPosition);
		if (currentMapZone == this.corpseSceneMapZone)
		{
			this.shadeMarker.SetActive(true);
		}
		this.PositionCompassAndCorpse();
		this.SetDisplayNextArea(true, currentMapZone);
		this.SetupMapMarkers();
		return true;
	}

	// Token: 0x06003A74 RID: 14964 RVA: 0x0010099C File Offset: 0x000FEB9C
	public void CloseQuickMap()
	{
		this.shadeMarker.SetActive(false);
		this.DisableMarkers();
		this.DisableAllAreas();
		for (int i = 0; i < this.fleaPinParents.Length; i++)
		{
			Transform transform = this.fleaPinParents[i];
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
		this.compassIcon.SetActive(false);
		this.SetDisplayNextArea(false, MapZone.NONE);
		this.displayingCompass = false;
	}

	// Token: 0x06003A75 RID: 14965 RVA: 0x00100A0C File Offset: 0x000FEC0C
	private void DisableAllAreas()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			string name = transform.name;
			if (!(name == "Compass Icon") && !(name == "Shade Pos") && !(name == "Map Markers") && !(name == "Flea Tracker Markers") && !(name == "Pan Audio Loop"))
			{
				transform.gameObject.SetActive(false);
			}
		}
		CameraRenderToMesh.SetActive(CameraRenderToMesh.ActiveSources.GameMap, false);
	}

	// Token: 0x06003A76 RID: 14966 RVA: 0x00100ABC File Offset: 0x000FECBC
	private void EnableUnlockedAreas(MapZone? setCurrent)
	{
		PlayerData instance = PlayerData.instance;
		bool flag = CollectableItemManager.IsInHiddenMode();
		bool flag2 = this.IsLostInAbyssPostMap();
		for (int i = 0; i < this.mapZoneInfo.Length; i++)
		{
			MapZone mapZone = (MapZone)i;
			GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[i];
			bool flag3 = setCurrent == null || setCurrent.Value == (MapZone)i;
			if (flag && mapZone != MapZone.THE_SLAB)
			{
				flag3 = false;
			}
			else if (flag2 && mapZone != MapZone.ABYSS)
			{
				flag3 = false;
			}
			foreach (GameMap.ParentInfo parentInfo in zoneInfo.Parents)
			{
				if (parentInfo.Parent)
				{
					parentInfo.Parent.SetActive(parentInfo.IsUnlocked && flag3);
				}
			}
		}
		if (this.mainQuestPins)
		{
			this.mainQuestPins.SetActive(true);
		}
		for (int k = 0; k < this.fleaPinParents.Length; k++)
		{
			Transform transform = this.fleaPinParents[k];
			if (transform)
			{
				if (instance.GetVariable(CaravanTroupeHunter.PdBools[(CaravanTroupeHunter.PinGroups)k]))
				{
					foreach (object obj in transform)
					{
						GameObject gameObject = ((Transform)obj).gameObject;
						MapPin component = gameObject.GetComponent<MapPin>();
						if (component)
						{
							component.IsActive = !instance.GetVariable(gameObject.name);
						}
					}
					transform.gameObject.SetActive(true);
				}
				else
				{
					transform.gameObject.SetActive(false);
				}
			}
		}
		CameraRenderToMesh.SetActive(CameraRenderToMesh.ActiveSources.GameMap, true);
	}

	// Token: 0x06003A77 RID: 14967 RVA: 0x00100C74 File Offset: 0x000FEE74
	public void UpdateCurrentScene()
	{
		this.OnStart();
		string sceneName;
		MapZone mapZone;
		if (!string.IsNullOrEmpty(this.overriddenSceneName))
		{
			sceneName = this.overriddenSceneName;
			mapZone = this.overriddenSceneRegion;
		}
		else if (MazeController.NewestInstance && !MazeController.NewestInstance.IsCapScene)
		{
			sceneName = "DustMazeCompassMarker";
			mapZone = this.currentSceneMapZone;
		}
		else
		{
			sceneName = this.gm.sceneName;
			mapZone = this.currentSceneMapZone;
		}
		this.GetSceneInfo(sceneName, mapZone, out this.currentScene, out this.currentSceneObj, out this.currentScenePos);
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x00100CFC File Offset: 0x000FEEFC
	private void GetSceneInfo(string sceneName, MapZone mapZone, out GameMapScene foundScene, out GameObject foundSceneObj, out Vector2 foundScenePos)
	{
		foundScene = null;
		foundSceneObj = null;
		foundScenePos = Vector2.zero;
		foreach (GameMap.ParentInfo parentInfo in this.mapZoneInfo[(int)mapZone].Parents)
		{
			if (parentInfo.Parent)
			{
				for (int j = 0; j < parentInfo.Parent.transform.childCount; j++)
				{
					GameObject gameObject = parentInfo.Parent.transform.GetChild(j).gameObject;
					if (!(gameObject.name != sceneName))
					{
						foundSceneObj = gameObject;
						foundScene = foundSceneObj.GetComponent<GameMapScene>();
						break;
					}
				}
				if (foundSceneObj)
				{
					break;
				}
			}
		}
		if (foundSceneObj == null)
		{
			return;
		}
		Component parent = foundSceneObj.transform.parent;
		Vector3 localPosition = foundSceneObj.transform.localPosition;
		Vector3 localPosition2 = parent.transform.localPosition;
		foundScenePos = new Vector3(localPosition.x + localPosition2.x, localPosition.y + localPosition2.y, 0f);
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x00100E14 File Offset: 0x000FF014
	public void PositionCompassAndCorpse()
	{
		this.UpdateCurrentScene();
		if (this.currentSceneObj != null)
		{
			ToolItem compassTool = Gameplay.CompassTool;
			if (compassTool && compassTool.IsEquipped && !this.IsLostInAbyssPreMap())
			{
				this.compassIcon.SetActive(true);
				this.displayingCompass = true;
			}
			else
			{
				this.compassIcon.SetActive(false);
				this.displayingCompass = false;
			}
		}
		this.shadeMarker.SetPosition(this.GetCorpsePosition());
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x00100E8C File Offset: 0x000FF08C
	private Vector2 GetPositionLocalBounds(Vector2 pos, MapZone zoneForBounds)
	{
		Rect localBounds = this.mapZoneInfo[(int)zoneForBounds].LocalBounds;
		Vector2 min = localBounds.min;
		Vector2 max = localBounds.max;
		MinMaxFloat minMaxFloat = new MinMaxFloat(min.x, max.x);
		MinMaxFloat minMaxFloat2 = new MinMaxFloat(min.y, max.y);
		float tbetween = minMaxFloat.GetTBetween(pos.x);
		float tbetween2 = minMaxFloat2.GetTBetween(pos.y);
		return new Vector2(tbetween, tbetween2);
	}

	// Token: 0x06003A7B RID: 14971 RVA: 0x00100F00 File Offset: 0x000FF100
	public Vector2 GetCompassPositionLocalBounds(out MapZone zoneForBounds)
	{
		zoneForBounds = this.currentSceneMapZone;
		Vector2 mapPosition = this.GetMapPosition(HeroController.instance.transform.position, this.currentScene, this.currentSceneObj, this.currentScenePos, this.currentSceneSize);
		return this.GetPositionLocalBounds(mapPosition, zoneForBounds);
	}

	// Token: 0x06003A7C RID: 14972 RVA: 0x00100F54 File Offset: 0x000FF154
	public Vector2 GetCorpsePositionLocalBounds(out MapZone zoneForBounds)
	{
		zoneForBounds = this.corpseSceneMapZone;
		PlayerData instance = PlayerData.instance;
		GameMapScene scene;
		GameObject sceneObj;
		Vector2 scenePos;
		this.GetSceneInfo(instance.HeroCorpseScene, this.corpseSceneMapZone, out scene, out sceneObj, out scenePos);
		Vector2 mapPosition = this.GetMapPosition(instance.HeroDeathScenePos, scene, sceneObj, scenePos, instance.HeroDeathSceneSize);
		return this.GetPositionLocalBounds(mapPosition, zoneForBounds);
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x00100FA8 File Offset: 0x000FF1A8
	public Vector2 GetCorpsePosition()
	{
		PlayerData instance = PlayerData.instance;
		GameMapScene scene;
		GameObject sceneObj;
		Vector2 scenePos;
		this.GetSceneInfo(instance.HeroCorpseScene, this.corpseSceneMapZone, out scene, out sceneObj, out scenePos);
		return this.GetMapPosition(instance.HeroDeathScenePos, scene, sceneObj, scenePos, instance.HeroDeathSceneSize);
	}

	// Token: 0x06003A7E RID: 14974 RVA: 0x00100FE8 File Offset: 0x000FF1E8
	private Vector2 GetMapPosition(Vector2 positionInScene, GameMapScene scene, GameObject sceneObj, Vector2 scenePos, Vector2 sceneSize)
	{
		if (sceneObj == null)
		{
			return new Vector2(-1000f, -1000f);
		}
		if (scene && scene.BoundsSprite)
		{
			Vector2 vector = scene.BoundsSprite.bounds.size * scene.transform.localScale;
			Vector3 localScale = base.transform.localScale;
			float x = scenePos.x - vector.x / 2f + positionInScene.x / sceneSize.x * (vector.x * localScale.x) / localScale.x;
			float y = scenePos.y - vector.y / 2f + positionInScene.y / sceneSize.y * (vector.y * localScale.y) / localScale.y;
			return new Vector2(x, y);
		}
		return scenePos;
	}

	// Token: 0x06003A7F RID: 14975 RVA: 0x001010DC File Offset: 0x000FF2DC
	private void Update()
	{
		if (this.displayingCompass)
		{
			Vector2 mapPosition = this.GetMapPosition(HeroController.instance.transform.position, this.currentScene, this.currentSceneObj, this.currentScenePos, this.currentSceneSize);
			this.compassIcon.transform.SetLocalPosition2D(new Vector3(mapPosition.x, mapPosition.y, -1f));
			if (!this.compassIcon.activeSelf)
			{
				this.compassIcon.SetActive(true);
			}
		}
		if (!this.canPan)
		{
			this.UpdatePanLoop(false);
			return;
		}
		bool flag;
		Vector2 sticksInput = this.inputHandler.GetSticksInput(out flag);
		if (sticksInput.magnitude <= Mathf.Epsilon)
		{
			this.UpdatePanLoop(false);
			return;
		}
		float num = flag ? (this.panSpeed * 2f) : this.panSpeed;
		Vector2 vector = base.transform.localPosition;
		vector -= sticksInput * (num * Time.unscaledDeltaTime);
		this.UpdateMapPosition(vector);
		Vector3 localPosition = base.transform.localPosition;
		bool isPlaying = Mathf.Abs(localPosition.x - vector.x) <= Mathf.Epsilon || Mathf.Abs(localPosition.y - vector.y) <= Mathf.Epsilon;
		this.UpdatePanLoop(isPlaying);
	}

	// Token: 0x06003A80 RID: 14976 RVA: 0x00101234 File Offset: 0x000FF434
	private void UpdatePanLoop(bool isPlaying)
	{
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x00101236 File Offset: 0x000FF436
	public void UpdateMapPosition(Vector2 pos)
	{
		base.transform.SetLocalPosition2D(pos);
		if (this.CanStartPan())
		{
			this.UpdatePanArrows();
		}
		else
		{
			this.DisableArrows();
		}
		this.KeepWithinBounds(base.transform.localScale);
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x00101270 File Offset: 0x000FF470
	private void UpdatePanArrows()
	{
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localScale = base.transform.localScale;
		Bounds zoomedBounds = this.ZoomedBounds;
		Vector3 size = zoomedBounds.size;
		zoomedBounds.center = zoomedBounds.center.MultiplyElements(localScale);
		size.Scale(localScale);
		zoomedBounds.size = size;
		Vector3 min = zoomedBounds.min;
		Vector3 max = zoomedBounds.max;
		Vector3 vector = max - min;
		GameMap.<UpdatePanArrows>g__ToggleArrow|109_0(this.panArrowR, vector.x > this.minPanAmount.x && localPosition.x > min.x);
		GameMap.<UpdatePanArrows>g__ToggleArrow|109_0(this.panArrowL, vector.x > this.minPanAmount.x && localPosition.x < max.x);
		GameMap.<UpdatePanArrows>g__ToggleArrow|109_0(this.panArrowU, vector.y > this.minPanAmount.y && localPosition.y > min.y);
		GameMap.<UpdatePanArrows>g__ToggleArrow|109_0(this.panArrowD, vector.y > this.minPanAmount.y && localPosition.y < max.y);
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x001013AC File Offset: 0x000FF5AC
	private void DisableArrows()
	{
		if (this.panArrowR.activeSelf)
		{
			this.panArrowR.SetActive(false);
		}
		if (this.panArrowL.activeSelf)
		{
			this.panArrowL.SetActive(false);
		}
		if (this.panArrowU.activeSelf)
		{
			this.panArrowU.SetActive(false);
		}
		if (this.panArrowD.activeSelf)
		{
			this.panArrowD.SetActive(false);
		}
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x00101420 File Offset: 0x000FF620
	private void DisableMarkers()
	{
		for (int i = 0; i < this.spawnedMapMarkers.GetLength(0); i++)
		{
			for (int j = 0; j < this.spawnedMapMarkers.GetLength(1); j++)
			{
				this.spawnedMapMarkers[i, j].SetActive(false);
			}
		}
	}

	// Token: 0x06003A85 RID: 14981 RVA: 0x0010146E File Offset: 0x000FF66E
	public void SetPanArrows(GameObject arrowU, GameObject arrowD, GameObject arrowL, GameObject arrowR)
	{
		this.panArrowU = arrowU;
		this.panArrowD = arrowD;
		this.panArrowL = arrowL;
		this.panArrowR = arrowR;
	}

	// Token: 0x06003A86 RID: 14982 RVA: 0x00101490 File Offset: 0x000FF690
	public void KeepWithinBounds(Vector2 zoomScale)
	{
		Transform transform = base.transform;
		Vector3 clampedPosition = this.GetClampedPosition(transform.localPosition, zoomScale);
		transform.localPosition = clampedPosition;
		Action<Vector2> viewPosUpdated = this.ViewPosUpdated;
		if (viewPosUpdated == null)
		{
			return;
		}
		viewPosUpdated(-clampedPosition.DivideElements(transform.localScale));
	}

	// Token: 0x06003A87 RID: 14983 RVA: 0x001014E0 File Offset: 0x000FF6E0
	public Vector3 GetClampedPosition(Vector3 pos, Vector2 scale)
	{
		Bounds bounds = this.GetCurrentBounds();
		bounds.center = bounds.center.MultiplyElements(scale);
		Vector3 size = bounds.size;
		size.Scale(scale);
		bounds.size = size;
		if (this.isMarkerZoom)
		{
			bounds = this.ApplyScrollAreaOffset(bounds);
		}
		Vector3 result = bounds.ClosestPoint(pos);
		result.z = pos.z;
		return result;
	}

	// Token: 0x06003A88 RID: 14984 RVA: 0x00101551 File Offset: 0x000FF751
	public void SetIsMarkerZoom(bool isMarkerZoom)
	{
		this.isMarkerZoom = isMarkerZoom;
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x0010155A File Offset: 0x000FF75A
	public void SetIsZoomed(bool isZoomed)
	{
		this.isZoomed = isZoomed;
	}

	// Token: 0x06003A8A RID: 14986 RVA: 0x00101563 File Offset: 0x000FF763
	public Vector3 GetCenter()
	{
		return this.ZoomedBounds.center;
	}

	// Token: 0x06003A8B RID: 14987 RVA: 0x00101570 File Offset: 0x000FF770
	private Bounds GetCurrentBounds()
	{
		if (this.isMarkerZoom)
		{
			return this.MapMarkerBounds;
		}
		return this.ZoomedBounds;
	}

	// Token: 0x06003A8C RID: 14988 RVA: 0x00101587 File Offset: 0x000FF787
	public void StopPan()
	{
		this.canPan = false;
		this.panArrowU.SetActive(false);
		this.panArrowL.SetActive(false);
		this.panArrowR.SetActive(false);
		this.panArrowD.SetActive(false);
	}

	// Token: 0x06003A8D RID: 14989 RVA: 0x001015C0 File Offset: 0x000FF7C0
	public bool CanStartPan()
	{
		Vector3 size = this.GetCurrentBounds().size;
		size.Scale(base.transform.localScale);
		return size.x > this.minPanAmount.x || size.y > this.minPanAmount.y;
	}

	// Token: 0x06003A8E RID: 14990 RVA: 0x00101618 File Offset: 0x000FF818
	public bool CanMarkerPan()
	{
		Vector3 size = this.MapMarkerBounds.size;
		size.Scale(base.transform.localScale);
		return size.x > this.mapMarkerScrollArea.size.x || size.y > this.mapMarkerScrollArea.size.y;
	}

	// Token: 0x06003A8F RID: 14991 RVA: 0x00101675 File Offset: 0x000FF875
	public void StartPan()
	{
		if (!this.CanStartPan())
		{
			this.DisableArrows();
			return;
		}
		this.canPan = true;
		this.UpdateMapPosition(base.transform.localPosition);
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x001016A4 File Offset: 0x000FF8A4
	public void SetupMapMarkers()
	{
		this.DisableMarkers();
		if (CollectableItemManager.IsInHiddenMode())
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		ArrayForEnumAttribute.EnsureArraySize<WrappedVector2List>(ref instance.placedMarkers, typeof(MapMarkerMenu.MarkerTypes));
		for (int i = 0; i < this.spawnedMapMarkers.GetLength(0); i++)
		{
			WrappedVector2List wrappedVector2List = instance.placedMarkers[i];
			if (wrappedVector2List == null)
			{
				wrappedVector2List = (instance.placedMarkers[i] = new WrappedVector2List());
			}
			int num = Mathf.Min(this.spawnedMapMarkers.GetLength(1), wrappedVector2List.List.Count);
			for (int j = 0; j < num; j++)
			{
				GameObject gameObject = this.spawnedMapMarkers[i, j];
				gameObject.SetActive(true);
				gameObject.transform.SetLocalPosition2D(wrappedVector2List.List[j]);
			}
		}
	}

	// Token: 0x06003A91 RID: 14993 RVA: 0x00101768 File Offset: 0x000FF968
	public void ResetMapped(string sceneName)
	{
		List<GameMap.ZoneInfo.MapCache> list;
		if (this.mapCaches.TryGetValue(sceneName, out list))
		{
			foreach (GameMap.ZoneInfo.MapCache mapCache in list)
			{
				if (mapCache.hasGameMap)
				{
					mapCache.gameMapScene.ResetMapped();
				}
			}
		}
	}

	// Token: 0x06003A92 RID: 14994 RVA: 0x001017D4 File Offset: 0x000FF9D4
	public bool UpdateGameMap()
	{
		PlayerData instance = PlayerData.instance;
		if (!instance.CanUpdateMap)
		{
			return false;
		}
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		bool result = false;
		foreach (string text in instance.scenesVisited)
		{
			bool flag;
			if (!instance.scenesMapped.Contains(text) && this.HasMapForScene(text, out flag) && this.IsSceneMappable(text))
			{
				instance.scenesMapped.Add(text);
				if (flag)
				{
					result = true;
				}
			}
		}
		return result;
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x00101874 File Offset: 0x000FFA74
	private bool IsSceneMappable(string sceneName)
	{
		if (GameMap._conditionalMappingLookup == null)
		{
			GameMap._conditionalMappingLookup = new Dictionary<string, GameMap.MapConditional>();
			foreach (GameMap.MapConditional mapConditional in GameMap._conditionalMapping)
			{
				foreach (string key in mapConditional.Scenes)
				{
					GameMap._conditionalMappingLookup.TryAdd(key, mapConditional);
				}
			}
		}
		GameMap.MapConditional mapConditional2;
		return !GameMap._conditionalMappingLookup.TryGetValue(sceneName, out mapConditional2) || mapConditional2.Condition.IsFulfilled;
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x0010192C File Offset: 0x000FFB2C
	public bool HasMapForScene(string sceneName, out bool sceneHasSprite)
	{
		sceneHasSprite = false;
		if (string.IsNullOrEmpty(sceneName))
		{
			sceneHasSprite = false;
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		GameMapScene gameMapScene = null;
		List<GameMap.ZoneInfo.MapCache> list;
		if (this.mapCaches.TryGetValue(sceneName, out list))
		{
			foreach (GameMap.ZoneInfo.MapCache mapCache in list)
			{
				if (mapCache.mapParent != null)
				{
					flag = mapCache.mapParent.IsUnlocked;
				}
				gameMapScene = mapCache.gameMapScene;
				flag2 = mapCache.hasGameMap;
				if (flag)
				{
					break;
				}
			}
			if (flag2 && gameMapScene.BoundsSprite)
			{
				sceneHasSprite = true;
			}
			return flag;
		}
		return false;
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x001019DC File Offset: 0x000FFBDC
	public Vector2 GetZoomPosition(MapZone mapZone)
	{
		return this.mapZoneInfo[(int)mapZone].GetWideMapZoomPosition(this.gm);
	}

	// Token: 0x06003A96 RID: 14998 RVA: 0x001019F1 File Offset: 0x000FFBF1
	public Vector2 GetZoomPositionNew(MapZone mapZone)
	{
		return this.mapZoneInfo[(int)mapZone].GetWideMapZoomPositionNew();
	}

	// Token: 0x06003A97 RID: 14999 RVA: 0x00101A00 File Offset: 0x000FFC00
	public bool HasAnyMapForZone(MapZone mapZone)
	{
		GameMap.ParentInfo[] parents = this.mapZoneInfo[(int)mapZone].Parents;
		for (int i = 0; i < parents.Length; i++)
		{
			if (parents[i].IsUnlocked)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003A98 RID: 15000 RVA: 0x00101A36 File Offset: 0x000FFC36
	public void SetMapManager(InventoryMapManager mapManager)
	{
		this.mapManager = mapManager;
	}

	// Token: 0x06003A99 RID: 15001 RVA: 0x00101A40 File Offset: 0x000FFC40
	public void CalculatePinAreaBounds()
	{
		if (!this.mapManager)
		{
			return;
		}
		this.mapMarkerScrollAreaWorld = this.mapManager.MarkerScrollArea;
		this.updatePinAreaBounds = (this.mapMarkerScrollAreaWorld.size.x <= 0f);
		if (this.updatePinAreaBounds)
		{
			return;
		}
		this.mapMarkerScrollArea = GameMap.TransformBoundsToLocalSpace(this.mapMarkerScrollAreaWorld, base.transform);
		this.mapMarkerScrollArea.center = this.mapMarkerScrollArea.center;
	}

	// Token: 0x06003A9A RID: 15002 RVA: 0x00101ACC File Offset: 0x000FFCCC
	private static Bounds TransformBoundsToLocalSpace(Bounds worldBounds, Transform target)
	{
		Vector3[] array = new Vector3[8];
		Vector3 min = worldBounds.min;
		Vector3 max = worldBounds.max;
		int num = 0;
		for (int i = 0; i <= 1; i++)
		{
			for (int j = 0; j <= 1; j++)
			{
				for (int k = 0; k <= 1; k++)
				{
					array[num++] = target.InverseTransformPoint(new Vector3((i == 0) ? min.x : max.x, (j == 0) ? min.y : max.y, (k == 0) ? min.z : max.z));
				}
			}
		}
		Bounds result = new Bounds(array[0], Vector3.zero);
		for (int l = 1; l < 8; l++)
		{
			result.Encapsulate(array[l]);
		}
		return result;
	}

	// Token: 0x06003A9B RID: 15003 RVA: 0x00101BA4 File Offset: 0x000FFDA4
	private void CalculateMapScrollBounds()
	{
		this.CompleteVisibleLocalBoundsNow();
		if (this.updatePinAreaBounds)
		{
			this.CalculatePinAreaBounds();
		}
		this.panMinX = float.MaxValue;
		this.panMaxX = float.MinValue;
		this.panMinY = float.MaxValue;
		this.panMaxY = float.MinValue;
		PlayerData instance = PlayerData.instance;
		MapZone mapZone = this.displayingCompass ? this.GetCurrentMapZone() : MapZone.NONE;
		bool flag = CollectableItemManager.IsInHiddenMode();
		bool flag2 = this.IsLostInAbyssPostMap();
		bool flag3 = flag || flag2;
		Bounds bounds = default(Bounds);
		int num = 0;
		for (int i = 0; i < this.mapZoneInfo.Length; i++)
		{
			GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[i];
			MapZone mapZone2 = (MapZone)i;
			bool flag4 = false;
			foreach (GameMap.ParentInfo parentInfo in zoneInfo.Parents)
			{
				if (parentInfo.Parent)
				{
					if (mapZone2 != mapZone && !parentInfo.Parent.gameObject.activeSelf)
					{
						if (parentInfo.BoundsAddedByPinGroup == CaravanTroupeHunter.PinGroups.None || flag || flag2)
						{
							goto IL_18C;
						}
						string fieldName = CaravanTroupeHunter.PdBools[parentInfo.BoundsAddedByPinGroup];
						if (!instance.GetVariable(fieldName) || !this.HasRemainingPinFor(parentInfo.BoundsAddedByPinGroup))
						{
							goto IL_18C;
						}
					}
					foreach (object obj in parentInfo.Parent.transform)
					{
						Transform transform = (Transform)obj;
						if (transform.gameObject.activeSelf)
						{
							SpriteRenderer component = transform.GetComponent<SpriteRenderer>();
							if (component && component.sprite)
							{
								flag4 = true;
								break;
							}
						}
					}
					if (flag4)
					{
						break;
					}
				}
				IL_18C:;
			}
			if (flag4)
			{
				Vector2 wideMapZoomPositionNew = zoneInfo.GetWideMapZoomPositionNew();
				if (wideMapZoomPositionNew.x < this.panMinX)
				{
					this.panMinX = wideMapZoomPositionNew.x;
				}
				if (wideMapZoomPositionNew.x > this.panMaxX)
				{
					this.panMaxX = wideMapZoomPositionNew.x;
				}
				if (wideMapZoomPositionNew.y < this.panMinY)
				{
					this.panMinY = wideMapZoomPositionNew.y;
				}
				if (wideMapZoomPositionNew.y > this.panMaxY)
				{
					this.panMaxY = wideMapZoomPositionNew.y;
				}
				Vector3 size = zoneInfo.VisibleLocalBounds.size;
				Bounds bounds2 = new Bounds(zoneInfo.VisibleLocalBounds.center, size);
				if (num == 0)
				{
					bounds = bounds2;
				}
				else
				{
					bounds.Encapsulate(bounds2);
				}
				num++;
			}
		}
		this.mapBounds = bounds;
		Vector3 center = this.mapMarkerScrollArea.center;
		ArrayForEnumAttribute.EnsureArraySize<WrappedVector2List>(ref instance.placedMarkers, typeof(MapMarkerMenu.MarkerTypes));
		if (!flag3)
		{
			Bounds bounds3 = default(Bounds);
			bool flag5 = false;
			if (this.markerParent != null && this.markerParent != base.transform)
			{
				this.markerToGameMapLocal = base.transform.worldToLocalMatrix * this.markerParent.localToWorldMatrix;
			}
			for (int k = 0; k < this.spawnedMapMarkers.GetLength(0); k++)
			{
				WrappedVector2List wrappedVector2List = instance.placedMarkers[k];
				if (wrappedVector2List == null)
				{
					wrappedVector2List = (instance.placedMarkers[k] = new WrappedVector2List());
				}
				for (int l = 0; l < wrappedVector2List.List.Count; l++)
				{
					Vector2 v = wrappedVector2List.List[l];
					Vector3 vector = this.markerToGameMapLocal.MultiplyPoint3x4(v);
					if (flag5)
					{
						bounds3.Encapsulate(vector);
					}
					else
					{
						bounds3 = new Bounds(vector, Vector3.zero);
						flag5 = true;
					}
				}
			}
			if (flag5)
			{
				bounds.Encapsulate(bounds3);
			}
		}
		this.MapMarkerBounds = bounds;
		this.NoPanBounds = bounds;
		bounds = this.ApplyScrollAreaOffset(bounds);
		Vector3 b = center;
		bounds.center = -bounds.center + b;
		this.MapMarkerBounds.center = -this.MapMarkerBounds.center + b;
		this.NoPanBounds.center = -this.NoPanBounds.center + b;
		if (num > 1)
		{
			this.panMinX = bounds.min.x;
			this.panMinY = bounds.min.y;
			this.panMaxX = bounds.max.x;
			this.panMaxY = bounds.max.y;
		}
		else
		{
			this.panMinX += center.x;
			this.panMaxX += center.x;
			this.panMinY += center.y;
			this.panMaxY += center.y;
		}
		this.ZoomedBounds = bounds;
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x00102080 File Offset: 0x00100280
	private Bounds ApplyScrollAreaOffset(Bounds bounds)
	{
		Vector3 extents = this.mapMarkerScrollArea.extents;
		Vector3 vector = bounds.extents - extents;
		if (vector.x <= 0f)
		{
			vector.x = 0f;
		}
		if (vector.y <= 0f)
		{
			vector.y = 0f;
		}
		if (vector.z < 0f)
		{
			vector.z = 0f;
		}
		bounds.extents = vector;
		return bounds;
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x001020FB File Offset: 0x001002FB
	public void GetMapScrollBounds(out float minX, out float maxX, out float minY, out float maxY)
	{
		minX = this.panMinX;
		maxX = this.panMaxX;
		minY = this.panMinY;
		maxY = this.panMaxY;
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x00102120 File Offset: 0x00100320
	public Vector2 GetDirectionBetweenScenes(string fromSceneName, string toSceneName)
	{
		Transform transform = null;
		Transform transform2 = null;
		GameMap.<>c__DisplayClass141_0 CS$<>8__locals1;
		CS$<>8__locals1.hasToScene = false;
		CS$<>8__locals1.hasFromScene = false;
		GameMap.ZoneInfo[] array = this.mapZoneInfo;
		for (int i = 0; i < array.Length; i++)
		{
			GameMap.ParentInfo[] parents = array[i].Parents;
			for (int j = 0; j < parents.Length; j++)
			{
				GameObject parent = parents[j].Parent;
				if (parent)
				{
					foreach (object obj in parent.transform)
					{
						Transform transform3 = (Transform)obj;
						if (!CS$<>8__locals1.hasFromScene && transform3.name == fromSceneName)
						{
							transform = transform3.transform;
							CS$<>8__locals1.hasFromScene = (transform != null);
						}
						else if (!CS$<>8__locals1.hasToScene && transform3.name == toSceneName)
						{
							transform2 = transform3.transform;
							CS$<>8__locals1.hasToScene = (transform2 != null);
						}
						if (GameMap.<GetDirectionBetweenScenes>g__IsDone|141_0(ref CS$<>8__locals1))
						{
							break;
						}
					}
					if (GameMap.<GetDirectionBetweenScenes>g__IsDone|141_0(ref CS$<>8__locals1))
					{
						break;
					}
				}
			}
		}
		if (!GameMap.<GetDirectionBetweenScenes>g__IsDone|141_0(ref CS$<>8__locals1))
		{
			return Vector2.zero;
		}
		return (transform2.position - transform.position).normalized;
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x00102290 File Offset: 0x00100490
	public float GetAngleBetweenScenes(string fromSceneName, string toSceneName)
	{
		Vector2 directionBetweenScenes = this.GetDirectionBetweenScenes(fromSceneName, toSceneName);
		float num;
		for (num = Vector2.SignedAngle(Vector2.right, directionBetweenScenes); num < 0f; num += 360f)
		{
		}
		return num;
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x001022C8 File Offset: 0x001004C8
	[ContextMenu("Calculate Visible Local Bounds")]
	private void CalculateVisibleLocalBounds()
	{
		GameObject gameObject = base.gameObject;
		this.isCoroutineRunning = false;
		this.boundsCalculationCoroutine = null;
		for (int i = 0; i < this.mapZoneInfo.Length; i++)
		{
			GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[i];
			if (zoneInfo != null)
			{
				zoneInfo.SetBoundsDirty();
				zoneInfo.CalculateWideBounds(gameObject);
			}
		}
	}

	// Token: 0x06003AA1 RID: 15009 RVA: 0x00102318 File Offset: 0x00100518
	private void StartCalculatingVisibleLocalBoundsAsync()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			this.CalculateVisibleLocalBounds();
			return;
		}
		if (this.isCoroutineRunning && this.boundsCalculationCoroutine != null)
		{
			base.StopCoroutine(this.boundsCalculationCoroutine);
		}
		this.isCoroutineRunning = true;
		this.currentZoneIndex = 0;
		this.boundsCalculationCoroutine = base.StartCoroutine(this.CalculateVisibleLocalBoundsCoroutine());
	}

	// Token: 0x06003AA2 RID: 15010 RVA: 0x00102378 File Offset: 0x00100578
	private void CompleteVisibleLocalBoundsNow()
	{
		if (this.isCoroutineRunning)
		{
			if (this.boundsCalculationCoroutine != null)
			{
				base.StopCoroutine(this.boundsCalculationCoroutine);
				this.boundsCalculationCoroutine = null;
			}
			this.isCoroutineRunning = false;
			GameObject gameObject = base.gameObject;
			while (this.currentZoneIndex < this.mapZoneInfo.Length)
			{
				GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[this.currentZoneIndex];
				if (zoneInfo != null)
				{
					zoneInfo.CalculateWideBounds(gameObject);
				}
				this.currentZoneIndex++;
			}
		}
	}

	// Token: 0x06003AA3 RID: 15011 RVA: 0x001023EE File Offset: 0x001005EE
	private IEnumerator CalculateVisibleLocalBoundsCoroutine()
	{
		this.isCoroutineRunning = true;
		GameObject root = base.gameObject;
		while (this.currentZoneIndex < this.mapZoneInfo.Length)
		{
			GameMap.ZoneInfo zoneInfo = this.mapZoneInfo[this.currentZoneIndex];
			if (zoneInfo != null && zoneInfo.BoundsDirty)
			{
				zoneInfo.CalculateWideBounds(root);
				yield return null;
			}
			this.currentZoneIndex++;
		}
		this.boundsCalculationCoroutine = null;
		this.isCoroutineRunning = false;
		yield break;
	}

	// Token: 0x06003AA6 RID: 15014 RVA: 0x0010249B File Offset: 0x0010069B
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06003AA7 RID: 15015 RVA: 0x001024A3 File Offset: 0x001006A3
	[CompilerGenerated]
	internal static void <UpdatePanArrows>g__ToggleArrow|109_0(GameObject arrow, bool shouldBeActive)
	{
		if (arrow.activeSelf != shouldBeActive)
		{
			arrow.SetActive(shouldBeActive);
		}
	}

	// Token: 0x06003AA8 RID: 15016 RVA: 0x001024B5 File Offset: 0x001006B5
	[CompilerGenerated]
	internal static bool <GetDirectionBetweenScenes>g__IsDone|141_0(ref GameMap.<>c__DisplayClass141_0 A_0)
	{
		return A_0.hasToScene & A_0.hasFromScene;
	}

	// Token: 0x04003CD4 RID: 15572
	private GameManager gm;

	// Token: 0x04003CD5 RID: 15573
	private InputHandler inputHandler;

	// Token: 0x04003CD6 RID: 15574
	[SerializeField]
	private GameObject compassIcon;

	// Token: 0x04003CD7 RID: 15575
	private MapZone currentSceneMapZone;

	// Token: 0x04003CD8 RID: 15576
	private MapZone currentRegionMapZone;

	// Token: 0x04003CD9 RID: 15577
	private string overriddenSceneName;

	// Token: 0x04003CDA RID: 15578
	private MapZone overriddenSceneRegion;

	// Token: 0x04003CDB RID: 15579
	private MapZone corpseSceneMapZone;

	// Token: 0x04003CDC RID: 15580
	[SerializeField]
	private ShadeMarkerArrow shadeMarker;

	// Token: 0x04003CDD RID: 15581
	private bool displayingCompass;

	// Token: 0x04003CDE RID: 15582
	private Vector2 currentScenePos;

	// Token: 0x04003CDF RID: 15583
	private Vector2 currentSceneSize;

	// Token: 0x04003CE0 RID: 15584
	private GameObject currentSceneObj;

	// Token: 0x04003CE1 RID: 15585
	private GameMapScene currentScene;

	// Token: 0x04003CE2 RID: 15586
	private bool canPan;

	// Token: 0x04003CE3 RID: 15587
	[SerializeField]
	private float panSpeed;

	// Token: 0x04003CE4 RID: 15588
	[SerializeField]
	private Vector2 minPanAmount = Vector2.one;

	// Token: 0x04003CE5 RID: 15589
	[SerializeField]
	private AudioSource panLoop;

	// Token: 0x04003CE6 RID: 15590
	[SerializeField]
	private float panLoopFadeDuration;

	// Token: 0x04003CE7 RID: 15591
	private float panMinX;

	// Token: 0x04003CE8 RID: 15592
	private float panMaxX;

	// Token: 0x04003CE9 RID: 15593
	private float panMinY;

	// Token: 0x04003CEA RID: 15594
	private float panMaxY;

	// Token: 0x04003CEB RID: 15595
	private GameObject panArrowU;

	// Token: 0x04003CEC RID: 15596
	private GameObject panArrowD;

	// Token: 0x04003CED RID: 15597
	private GameObject panArrowL;

	// Token: 0x04003CEE RID: 15598
	private GameObject panArrowR;

	// Token: 0x04003CEF RID: 15599
	[SerializeField]
	[ArrayForEnum(typeof(MapMarkerMenu.MarkerTypes))]
	private GameObject[] mapMarkerTemplates;

	// Token: 0x04003CF0 RID: 15600
	[SerializeField]
	[ArrayForEnum(typeof(MapZone))]
	private GameMap.ZoneInfo[] mapZoneInfo;

	// Token: 0x04003CF1 RID: 15601
	[SerializeField]
	[ArrayForEnum(typeof(CaravanTroupeHunter.PinGroups))]
	private Transform[] fleaPinParents;

	// Token: 0x04003CF2 RID: 15602
	[SerializeField]
	private GameObject mainQuestPins;

	// Token: 0x04003CF3 RID: 15603
	[Header("Map Bounds")]
	[SerializeField]
	private Bounds mapMarkerScrollArea;

	// Token: 0x04003CF4 RID: 15604
	private Bounds mapMarkerScrollAreaWorld;

	// Token: 0x04003CF5 RID: 15605
	[SerializeField]
	private Bounds ZoomedBounds;

	// Token: 0x04003CF6 RID: 15606
	private Bounds NoPanBounds;

	// Token: 0x04003CF7 RID: 15607
	private Bounds MapMarkerBounds;

	// Token: 0x04003CF8 RID: 15608
	private Bounds mapBounds;

	// Token: 0x04003CF9 RID: 15609
	private GameObject[,] spawnedMapMarkers;

	// Token: 0x04003CFC RID: 15612
	private static Dictionary<string, GameMap.MapConditional> _conditionalMappingLookup;

	// Token: 0x04003CFD RID: 15613
	private static readonly IReadOnlyList<GameMap.MapConditional> _conditionalMapping = new List<GameMap.MapConditional>
	{
		new GameMap.MapConditional
		{
			Condition = new PlayerDataTest("defeatedPhantom", true),
			Scenes = new string[]
			{
				"Dust_09",
				"Dust_09_top_2",
				"Dust_09_into_citadel",
				"Dust_Maze_08_completed"
			}
		}
	};

	// Token: 0x04003CFE RID: 15614
	private Collider2D viewportEdge;

	// Token: 0x04003CFF RID: 15615
	private readonly Dictionary<string, List<GameMap.ZoneInfo.MapCache>> mapCaches = new Dictionary<string, List<GameMap.ZoneInfo.MapCache>>();

	// Token: 0x04003D00 RID: 15616
	private bool initZoneMaps;

	// Token: 0x04003D01 RID: 15617
	[TupleElementNames(new string[]
	{
		"mapZone",
		"distance"
	})]
	private List<ValueTuple<MapZone, float>> mapZoneDistances;

	// Token: 0x04003D02 RID: 15618
	private bool hasAwaken;

	// Token: 0x04003D03 RID: 15619
	private bool hasStarted;

	// Token: 0x04003D04 RID: 15620
	private Matrix4x4 markerToGameMapLocal = Matrix4x4.identity;

	// Token: 0x04003D05 RID: 15621
	private Transform markerParent;

	// Token: 0x04003D06 RID: 15622
	private int lastMappedCount = -1;

	// Token: 0x04003D07 RID: 15623
	private bool doneInitialLoad;

	// Token: 0x04003D08 RID: 15624
	private List<GameMap.ZoneInfo> unlockedMapZones = new List<GameMap.ZoneInfo>();

	// Token: 0x04003D09 RID: 15625
	private bool isMarkerZoom;

	// Token: 0x04003D0A RID: 15626
	private bool isZoomed;

	// Token: 0x04003D0B RID: 15627
	private InventoryMapManager mapManager;

	// Token: 0x04003D0C RID: 15628
	private bool updatePinAreaBounds;

	// Token: 0x04003D0D RID: 15629
	private Coroutine boundsCalculationCoroutine;

	// Token: 0x04003D0E RID: 15630
	private bool isCoroutineRunning;

	// Token: 0x04003D0F RID: 15631
	private int currentZoneIndex;

	// Token: 0x0200196F RID: 6511
	[Serializable]
	private class ParentInfo
	{
		// Token: 0x1700109E RID: 4254
		// (get) Token: 0x0600943C RID: 37948 RVA: 0x002A1C80 File Offset: 0x0029FE80
		public bool IsUnlocked
		{
			get
			{
				if (string.IsNullOrEmpty(this.PlayerDataBool))
				{
					return false;
				}
				PlayerData instance = PlayerData.instance;
				return instance.mapAllRooms || instance.GetVariable(this.PlayerDataBool);
			}
		}

		// Token: 0x0600943D RID: 37949 RVA: 0x002A1CB8 File Offset: 0x0029FEB8
		public void Validate()
		{
			if (this.validated)
			{
				return;
			}
			this.HasParent = this.Parent;
			this.validated = true;
			if (!this.HasParent)
			{
				return;
			}
			this.CacheMaps();
			this.PositionConditions = this.Parent.GetComponent<PositionConditions>();
			this.HasPositionConditions = this.PositionConditions;
		}

		// Token: 0x0600943E RID: 37950 RVA: 0x002A1D18 File Offset: 0x0029FF18
		public void CheckActivation()
		{
			if (this.hasActivatedOnce)
			{
				return;
			}
			if (!this.HasParent)
			{
				return;
			}
			if (!this.Parent.activeSelf)
			{
				this.Parent.gameObject.SetActive(true);
				this.hasActivatedOnce = this.Parent.activeInHierarchy;
				this.Parent.gameObject.SetActive(false);
				return;
			}
			this.hasActivatedOnce = this.Parent.activeInHierarchy;
		}

		// Token: 0x0600943F RID: 37951 RVA: 0x002A1D8C File Offset: 0x0029FF8C
		public void CacheMaps()
		{
			this.Validate();
			if (!this.HasParent)
			{
				return;
			}
			this.Maps.Clear();
			foreach (object obj in this.Parent.transform)
			{
				Transform transform = (Transform)obj;
				this.Maps.Add(new GameMap.ZoneInfo.MapCache(this, transform));
			}
		}

		// Token: 0x040095CF RID: 38351
		public GameObject Parent;

		// Token: 0x040095D0 RID: 38352
		[NonSerialized]
		public bool HasParent;

		// Token: 0x040095D1 RID: 38353
		[PlayerDataField(typeof(bool), false)]
		public string PlayerDataBool;

		// Token: 0x040095D2 RID: 38354
		public CaravanTroupeHunter.PinGroups BoundsAddedByPinGroup = CaravanTroupeHunter.PinGroups.None;

		// Token: 0x040095D3 RID: 38355
		private bool validated;

		// Token: 0x040095D4 RID: 38356
		[NonSerialized]
		public bool HasPositionConditions;

		// Token: 0x040095D5 RID: 38357
		public PositionConditions PositionConditions;

		// Token: 0x040095D6 RID: 38358
		public List<GameMap.ZoneInfo.MapCache> Maps = new List<GameMap.ZoneInfo.MapCache>();

		// Token: 0x040095D7 RID: 38359
		[NonSerialized]
		private bool hasActivatedOnce;
	}

	// Token: 0x02001970 RID: 6512
	[Serializable]
	private class ConditionalPosition
	{
		// Token: 0x040095D8 RID: 38360
		public Vector2 Position;

		// Token: 0x040095D9 RID: 38361
		public GameMapScene[] IfMapped;
	}

	// Token: 0x02001971 RID: 6513
	[Serializable]
	private class ZoneInfo
	{
		// Token: 0x06009442 RID: 37954 RVA: 0x002A1E34 File Offset: 0x002A0034
		public void GetComponents()
		{
			this.sprites = new List<SpriteRenderer>();
			this.texts = new List<TMP_Text>();
			foreach (GameMap.ParentInfo parentInfo in this.Parents)
			{
				if (!(parentInfo.Parent == null))
				{
					this.sprites.AddRange(parentInfo.Parent.GetComponentsInChildren<SpriteRenderer>(true));
					this.texts.AddRange(parentInfo.Parent.GetComponentsInChildren<TMP_Text>(true));
				}
			}
			this.initialSpriteColors = new Color[this.sprites.Count];
			for (int j = 0; j < this.initialSpriteColors.Length; j++)
			{
				this.initialSpriteColors[j] = this.sprites[j].color;
			}
			this.initialTextColors = new Color[this.texts.Count];
			for (int k = 0; k < this.initialTextColors.Length; k++)
			{
				this.initialTextColors[k] = this.texts[k].color;
			}
			this.CacheMaps(false);
		}

		// Token: 0x06009443 RID: 37955 RVA: 0x002A1F48 File Offset: 0x002A0148
		public void CacheMaps(bool forced = false)
		{
			if (!this.init || forced)
			{
				this.init = true;
				if (this.Parents == null)
				{
					return;
				}
				foreach (GameMap.ParentInfo parentInfo in this.Parents)
				{
					if (parentInfo != null)
					{
						parentInfo.Validate();
						parentInfo.CacheMaps();
					}
				}
			}
		}

		// Token: 0x06009444 RID: 37956 RVA: 0x002A1F9C File Offset: 0x002A019C
		private void SetAlpha(float value)
		{
			Color other = new Color(value, value, value, 1f);
			for (int i = 0; i < this.sprites.Count; i++)
			{
				this.sprites[i].color = this.initialSpriteColors[i].MultiplyElements(other);
			}
			for (int j = 0; j < this.texts.Count; j++)
			{
				this.texts[j].color = this.initialTextColors[j].MultiplyElements(other);
			}
		}

		// Token: 0x06009445 RID: 37957 RVA: 0x002A202A File Offset: 0x002A022A
		public Vector2 GetWideMapZoomPosition(GameManager gm)
		{
			return this.GetOrderedPosition(gm, this.WideMapZoomPositionsOrdered);
		}

		// Token: 0x06009446 RID: 37958 RVA: 0x002A2039 File Offset: 0x002A0239
		public Vector2 GetQuickMapPosition(GameManager gm)
		{
			return this.GetOrderedPosition(gm, this.QuickMapPositionsOrdered);
		}

		// Token: 0x06009447 RID: 37959 RVA: 0x002A2048 File Offset: 0x002A0248
		private Vector2 GetOrderedPosition(GameManager gm, IEnumerable<GameMap.ConditionalPosition> positions)
		{
			HashSet<string> scenesVisited = gm.playerData.scenesVisited;
			foreach (GameMap.ConditionalPosition conditionalPosition in positions)
			{
				if (conditionalPosition.IfMapped != null && conditionalPosition.IfMapped.Length != 0)
				{
					if (!conditionalPosition.IfMapped.All((GameMapScene mapScene) => !mapScene))
					{
						foreach (GameMapScene gameMapScene in conditionalPosition.IfMapped)
						{
							if (gameMapScene && gameMapScene.IsMapped)
							{
								return conditionalPosition.Position;
							}
						}
						continue;
					}
				}
				return conditionalPosition.Position;
			}
			return Vector2.zero;
		}

		// Token: 0x06009448 RID: 37960 RVA: 0x002A2124 File Offset: 0x002A0324
		public Vector2 GetWideMapZoomPositionNew()
		{
			this.UpdateIfDirty();
			return -this.VisibleLocalBounds.center;
		}

		// Token: 0x06009449 RID: 37961 RVA: 0x002A2141 File Offset: 0x002A0341
		public void SetRoot(GameObject root)
		{
			this.root = root;
			this.hasRoot = (this.root != null);
			this.SetBoundsDirty();
		}

		// Token: 0x0600944A RID: 37962 RVA: 0x002A2162 File Offset: 0x002A0362
		private void UpdateIfDirty()
		{
			if (!this.BoundsDirty)
			{
				return;
			}
			if (!this.hasRoot)
			{
				return;
			}
			this.CalculateWideBounds(this.root);
		}

		// Token: 0x1700109F RID: 4255
		// (get) Token: 0x0600944B RID: 37963 RVA: 0x002A2182 File Offset: 0x002A0382
		// (set) Token: 0x0600944C RID: 37964 RVA: 0x002A218A File Offset: 0x002A038A
		public bool BoundsDirty { get; private set; } = true;

		// Token: 0x0600944D RID: 37965 RVA: 0x002A2193 File Offset: 0x002A0393
		public void SetBoundsDirty()
		{
			this.BoundsDirty = true;
		}

		// Token: 0x0600944E RID: 37966 RVA: 0x002A219C File Offset: 0x002A039C
		public void CalculateWideBounds(GameObject root)
		{
			if (!this.BoundsDirty)
			{
				return;
			}
			this.BoundsDirty = false;
			if (this.Parents == null || this.Parents.Length == 0)
			{
				return;
			}
			Transform transform = root.transform;
			if (!this.cachedMapSprites)
			{
				this.cachedMapSprites = true;
				foreach (GameMap.ParentInfo parentInfo in this.Parents)
				{
					if (parentInfo != null && !(parentInfo.Parent == null))
					{
						foreach (object obj in parentInfo.Parent.transform)
						{
							GameMapScene component = ((Transform)obj).GetComponent<GameMapScene>();
							if (!(component == null))
							{
								this.mapScenes.Add(component);
							}
						}
					}
				}
			}
			Bounds visibleLocalBounds = default(Bounds);
			bool flag = false;
			foreach (GameMapScene gameMapScene in this.mapScenes)
			{
				Bounds bounds;
				if (gameMapScene.gameObject.activeSelf && gameMapScene.TryGetSpriteBounds(transform, out bounds))
				{
					if (flag)
					{
						visibleLocalBounds.Encapsulate(bounds);
					}
					else
					{
						flag = true;
						visibleLocalBounds = new Bounds(bounds.center, bounds.size);
					}
				}
			}
			this.VisibleLocalBounds = visibleLocalBounds;
		}

		// Token: 0x0600944F RID: 37967 RVA: 0x002A2314 File Offset: 0x002A0514
		private Bounds GetCroppedBounds(Sprite sprite)
		{
			Vector2[] vertices = sprite.vertices;
			Vector2 vector = vertices[0];
			Vector2 vector2 = vertices[0];
			for (int i = 1; i < vertices.Length; i++)
			{
				Vector2 vector3 = vertices[i];
				if (vector3.x < vector.x)
				{
					vector.x = vector3.x;
				}
				if (vector3.y < vector.y)
				{
					vector.y = vector3.y;
				}
				if (vector3.x > vector2.x)
				{
					vector2.x = vector3.x;
				}
				if (vector3.y > vector2.y)
				{
					vector2.y = vector3.y;
				}
			}
			Vector2 v = vector2 - vector;
			Vector2 v2 = (vector + vector2) / 2f;
			return new Bounds(v2, v);
		}

		// Token: 0x040095DA RID: 38362
		public GameMap.ParentInfo[] Parents;

		// Token: 0x040095DB RID: 38363
		[HideInInspector]
		[Obsolete]
		public Vector2 WideMapZoomPosition;

		// Token: 0x040095DC RID: 38364
		public GameMap.ConditionalPosition[] WideMapZoomPositionsOrdered;

		// Token: 0x040095DD RID: 38365
		[HideInInspector]
		[Obsolete]
		public Vector2 QuickMapPosition;

		// Token: 0x040095DE RID: 38366
		public GameMap.ConditionalPosition[] QuickMapPositionsOrdered;

		// Token: 0x040095DF RID: 38367
		public Rect LocalBounds;

		// Token: 0x040095E0 RID: 38368
		public Bounds VisibleLocalBounds;

		// Token: 0x040095E1 RID: 38369
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString NameOverride;

		// Token: 0x040095E2 RID: 38370
		private List<SpriteRenderer> sprites;

		// Token: 0x040095E3 RID: 38371
		private Color[] initialSpriteColors;

		// Token: 0x040095E4 RID: 38372
		private List<TMP_Text> texts;

		// Token: 0x040095E5 RID: 38373
		private Color[] initialTextColors;

		// Token: 0x040095E6 RID: 38374
		private bool init;

		// Token: 0x040095E7 RID: 38375
		private bool cachedMapSprites;

		// Token: 0x040095E8 RID: 38376
		private List<GameMapScene> mapScenes = new List<GameMapScene>();

		// Token: 0x040095E9 RID: 38377
		private bool hasRoot;

		// Token: 0x040095EA RID: 38378
		private GameObject root;

		// Token: 0x02001C24 RID: 7204
		public class MapCache
		{
			// Token: 0x06009AE8 RID: 39656 RVA: 0x002B498C File Offset: 0x002B2B8C
			public MapCache(GameMap.ParentInfo mapParent, Transform transform)
			{
				this.sceneName = transform.name;
				this.mapParent = mapParent;
				this.gameObject = transform.gameObject;
				this.gameMapScene = transform.GetComponent<GameMapScene>();
				this.hasGameMap = this.gameMapScene;
			}

			// Token: 0x0400A01F RID: 40991
			public string sceneName;

			// Token: 0x0400A020 RID: 40992
			public GameMap.ParentInfo mapParent;

			// Token: 0x0400A021 RID: 40993
			public bool hasGameMap;

			// Token: 0x0400A022 RID: 40994
			public GameObject gameObject;

			// Token: 0x0400A023 RID: 40995
			public GameMapScene gameMapScene;
		}
	}

	// Token: 0x02001972 RID: 6514
	private class MapConditional
	{
		// Token: 0x040095EC RID: 38380
		public PlayerDataTest Condition;

		// Token: 0x040095ED RID: 38381
		public IReadOnlyList<string> Scenes;
	}
}
