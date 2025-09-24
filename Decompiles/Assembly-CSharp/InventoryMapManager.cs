using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using JetBrains.Annotations;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x020006AB RID: 1707
public class InventoryMapManager : InventoryItemManager, IInventoryPaneAvailabilityProvider
{
	// Token: 0x17000707 RID: 1799
	// (get) Token: 0x06003D4C RID: 15692 RVA: 0x0010DB5C File Offset: 0x0010BD5C
	protected override IEnumerable<InventoryItemSelectable> DefaultSelectables
	{
		get
		{
			if (!this.wideMap)
			{
				return base.DefaultSelectables;
			}
			InventoryItemWideMapZone[] defaultSelectables = this.wideMap.DefaultSelectables;
			if (defaultSelectables.Length == 0)
			{
				return base.DefaultSelectables;
			}
			GameManager instance = GameManager.instance;
			MapZone currentMapZone = instance.gameMap.GetCurrentMapZone();
			return from mapPiece in defaultSelectables
			where mapPiece.IsUnlocked && mapPiece.gameObject.activeSelf
			select mapPiece into item
			orderby item.EnumerateMapZones().Contains(currentMapZone) descending
			select item;
		}
	}

	// Token: 0x17000708 RID: 1800
	// (get) Token: 0x06003D4D RID: 15693 RVA: 0x0010DBE7 File Offset: 0x0010BDE7
	private bool HasNoMap
	{
		get
		{
			return (CollectableItemManager.IsInHiddenMode() && !this.gameMap.HasAnyMapForZone(MapZone.THE_SLAB)) || this.gameMap.IsLostInAbyssPreMap();
		}
	}

	// Token: 0x06003D4E RID: 15694 RVA: 0x0010DC14 File Offset: 0x0010BE14
	protected override void Awake()
	{
		base.Awake();
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		this.pane = base.GetComponent<InventoryPane>();
		if (this.pane)
		{
			this.pane.OnPaneStart += this.OnPaneStart;
			this.OnPaneStart();
		}
		this.noMapSymbol.SetActive(false);
		this.hasMapParent.SetActive(true);
		this.EnsureWideMapSpawned();
		this.EnsureGameMapSpawned();
	}

	// Token: 0x06003D4F RID: 15695 RVA: 0x0010DC8D File Offset: 0x0010BE8D
	private void OnDisable()
	{
		if (this.sceneMapFade)
		{
			this.sceneMapFade.FadeToZero(0f);
		}
		this.isZoomed = false;
	}

	// Token: 0x06003D50 RID: 15696 RVA: 0x0010DCB3 File Offset: 0x0010BEB3
	protected override InventoryItemSelectable GetStartSelectable()
	{
		return this.DefaultSelectables.FirstOrDefault<InventoryItemSelectable>() ?? base.GetStartSelectable();
	}

	// Token: 0x06003D51 RID: 15697 RVA: 0x0010DCCC File Offset: 0x0010BECC
	private InventoryItemSelectable GetClosestSelectable()
	{
		IEnumerable<InventoryItemWideMapZone> wideMapPieces = this.DefaultSelectables.OfType<InventoryItemWideMapZone>();
		return this.gameMap.GetClosestWideMapZone(wideMapPieces);
	}

	// Token: 0x06003D52 RID: 15698 RVA: 0x0010DCF4 File Offset: 0x0010BEF4
	protected override IEnumerable<InventoryItemSelectable> GetRightMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = from mapPiece in this.wideMap.DefaultSelectables
		orderby mapPiece.transform.localPosition.x descending
		select mapPiece;
		return base.GetRightMostSelectables().Union(second);
	}

	// Token: 0x06003D53 RID: 15699 RVA: 0x0010DD40 File Offset: 0x0010BF40
	protected override IEnumerable<InventoryItemSelectable> GetLeftMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = from mapPiece in this.wideMap.DefaultSelectables
		orderby mapPiece.transform.localPosition.x
		select mapPiece;
		return base.GetRightMostSelectables().Union(second);
	}

	// Token: 0x06003D54 RID: 15700 RVA: 0x0010DD8C File Offset: 0x0010BF8C
	protected override IEnumerable<InventoryItemSelectable> GetTopMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = from mapPiece in this.wideMap.DefaultSelectables
		orderby mapPiece.transform.localPosition.y descending
		select mapPiece;
		return base.GetTopMostSelectables().Union(second);
	}

	// Token: 0x06003D55 RID: 15701 RVA: 0x0010DDD8 File Offset: 0x0010BFD8
	protected override IEnumerable<InventoryItemSelectable> GetBottomMostSelectables()
	{
		IOrderedEnumerable<InventoryItemWideMapZone> second = from mapPiece in this.wideMap.DefaultSelectables
		orderby mapPiece.transform.localPosition.y
		select mapPiece;
		return base.GetBottomMostSelectables().Union(second);
	}

	// Token: 0x06003D56 RID: 15702 RVA: 0x0010DE21 File Offset: 0x0010C021
	public override bool MoveSelection(InventoryItemManager.SelectionDirection direction)
	{
		return !this.HasNoMap && base.MoveSelection(direction);
	}

	// Token: 0x06003D57 RID: 15703 RVA: 0x0010DE34 File Offset: 0x0010C034
	public override bool MoveSelectionPage(InventoryItemManager.SelectionDirection direction)
	{
		if (this.isZoomed || this.zoomRoutine != null)
		{
			return true;
		}
		InventoryItemSelectable inventoryItemSelectable;
		if ((direction != InventoryItemManager.SelectionDirection.Up && direction != InventoryItemManager.SelectionDirection.Down) || !base.TryGetFurthestSelectableInDirection(direction, out inventoryItemSelectable))
		{
			return false;
		}
		if (inventoryItemSelectable == base.CurrentSelected)
		{
			return false;
		}
		base.SetSelected(inventoryItemSelectable, null, false);
		return true;
	}

	// Token: 0x06003D58 RID: 15704 RVA: 0x0010DE8A File Offset: 0x0010C08A
	public void EnsureMapsSpawned()
	{
		this.EnsureWideMapSpawned();
		this.EnsureGameMapSpawned();
	}

	// Token: 0x06003D59 RID: 15705 RVA: 0x0010DE98 File Offset: 0x0010C098
	private void EnsureWideMapSpawned()
	{
		if (this.wideMap)
		{
			return;
		}
		this.wideMap = Object.Instantiate<InventoryWideMap>(this.wideMapPrefab, this.wideMapParent);
		Transform transform = this.wideMapPrefab.transform;
		Transform transform2 = this.wideMap.transform;
		transform2.localPosition = Vector3.zero;
		transform2.localScale = transform.localScale;
		this.zoneMapInitialScale = transform2.localScale;
	}

	// Token: 0x06003D5A RID: 15706 RVA: 0x0010DF08 File Offset: 0x0010C108
	private void EnsureGameMapSpawned()
	{
		if (this.gameMap)
		{
			return;
		}
		this.gameMap = Object.Instantiate<GameMap>(this.gameMapPrefab, this.gameMapParent);
		this.gameMap.transform.localPosition = new Vector3(0f, 0f, 43f);
		PlayMakerGlobals.Instance.Variables.FindFsmGameObject("Game Map").Value = this.gameMap.gameObject;
		GameManager.instance.SetGameMap(this.gameMap.gameObject);
		this.gameMap.SetMapManager(this);
		this.gameMap.SetupMap(false);
		this.gameMap.SetPanArrows(this.panArrowUp, this.panArrowDown, this.panArrowLeft, this.panArrowRight);
	}

	// Token: 0x17000709 RID: 1801
	// (get) Token: 0x06003D5B RID: 15707 RVA: 0x0010DFD2 File Offset: 0x0010C1D2
	public Bounds MarkerScrollArea
	{
		get
		{
			if (!this.hasCreatedScrollArea)
			{
				this.GetMapCameraBounds();
			}
			return this.markerScrollArea;
		}
	}

	// Token: 0x06003D5C RID: 15708 RVA: 0x0010DFE8 File Offset: 0x0010C1E8
	private void GetMapCameraBounds()
	{
		if (!this.mapCamera)
		{
			return;
		}
		if (!this.mapMarkerMenu)
		{
			return;
		}
		Vector3 vector;
		Vector3 vector2;
		this.mapMarkerMenu.GetViewMinMax(out vector, out vector2);
		float z = this.gameMap.transform.position.z;
		Vector3 center = this.mapCamera.ViewportToWorldPoint(new Vector3(vector.x, vector.y, z));
		Vector3 point = this.mapCamera.ViewportToWorldPoint(new Vector3(vector2.x, vector2.y, z));
		this.markerScrollArea = new Bounds(center, Vector3.zero);
		this.markerScrollArea.Encapsulate(point);
		this.hasCreatedScrollArea = (this.markerScrollArea.size.x > 0f);
	}

	// Token: 0x06003D5D RID: 15709 RVA: 0x0010E0AE File Offset: 0x0010C2AE
	[ContextMenu("Update Map Camera Bounds")]
	private void UpdateMapCameraBounds()
	{
		this.GetMapCameraBounds();
		this.gameMap.CalculatePinAreaBounds();
	}

	// Token: 0x06003D5E RID: 15710 RVA: 0x0010E0C1 File Offset: 0x0010C2C1
	[UsedImplicitly]
	private bool? ValidateFsmEvent(string eventName)
	{
		return this.controlFSM.IsEventValid(eventName, true);
	}

	// Token: 0x06003D5F RID: 15711 RVA: 0x0010E0D0 File Offset: 0x0010C2D0
	public void AutoZoomIn()
	{
		this.ZoomIn(MapZone.NONE, false);
	}

	// Token: 0x06003D60 RID: 15712 RVA: 0x0010E0DC File Offset: 0x0010C2DC
	public void LockedZoomUndo()
	{
		this.sceneMapFade.AlphaSelf = 0f;
		Transform transform = this.wideMap.transform;
		NestedFadeGroupBase fadeGroup = this.wideMap.FadeGroup;
		Vector2 positionOffset = this.wideMap.PositionOffset;
		transform.localScale = this.zoneMapInitialScale;
		transform.SetLocalPosition2D(positionOffset);
		fadeGroup.AlphaSelf = 1f;
		base.IsActionsBlocked = false;
	}

	// Token: 0x06003D61 RID: 15713 RVA: 0x0010E140 File Offset: 0x0010C340
	public MapZone GetCurrentMapZone()
	{
		return this.gameMap.GetCurrentMapZone();
	}

	// Token: 0x06003D62 RID: 15714 RVA: 0x0010E150 File Offset: 0x0010C350
	public void ZoomIn(MapZone mapZone, bool animate)
	{
		if (this.HasNoMap)
		{
			this.controlFSM.SendEventSafe("WIDE MAP");
			return;
		}
		base.IsActionsBlocked = true;
		this.paneList.CanSwitchPanes = false;
		if (!this.controlFSM)
		{
			return;
		}
		if (this.zoomRoutine != null)
		{
			base.StopCoroutine(this.zoomRoutine);
			this.zoomRoutine = null;
		}
		this.gameMap.WorldMap();
		if (mapZone == MapZone.NONE)
		{
			mapZone = this.gameMap.GetCurrentMapZone();
		}
		Vector2 vector = this.gameMap.GetZoomPosition(mapZone);
		this.sceneMap = this.gameMap.transform;
		this.sceneMap.localScale = InventoryMapManager.SceneMapEndScale;
		this.gameMap.UpdateMapPosition(vector);
		this.sceneMapFade.transform.SetLocalPosition2D(Vector2.zero);
		if (animate)
		{
			this.zoomRoutine = base.StartCoroutine(this.ZoomInRoutine(vector));
			return;
		}
		this.wideMap.FadeGroup.AlphaSelf = 0f;
		this.sceneMap.localScale = InventoryMapManager.SceneMapEndScale;
		if (this.gameMap.CanStartPan())
		{
			float min;
			float max;
			float min2;
			float max2;
			this.gameMap.GetMapScrollBounds(out min, out max, out min2, out max2);
			vector.x = Mathf.Clamp(vector.x, min, max);
			vector.y = Mathf.Clamp(vector.y, min2, max2);
		}
		else if (!this.gameMap.HasAnyMapForZone(mapZone))
		{
			vector = this.gameMap.GetClosestUnlockedPoint(vector);
		}
		vector = this.gameMap.GetClampedPosition(vector, InventoryMapManager.SceneMapEndScale);
		this.sceneMap.SetLocalPosition2D(vector);
		this.gameMap.KeepWithinBounds(InventoryMapManager.SceneMapEndScale);
		this.sceneMapFade.FadeToOne(0f);
		this.isZoomed = true;
		this.gameMap.SetIsZoomed(true);
		this.UpdateKeyPromptState(false);
	}

	// Token: 0x06003D63 RID: 15715 RVA: 0x0010E32F File Offset: 0x0010C52F
	private IEnumerator ZoomInRoutine(Vector2 zoomToPos)
	{
		this.controlFSM.SendEvent(this.zoomInEvent);
		Transform zoneMap = this.wideMap.transform;
		NestedFadeGroupBase zoneMapFade = this.wideMap.FadeGroup;
		Vector2 zoneMapOffset = this.wideMap.PositionOffset;
		zoneMap.SetLocalPosition2D(zoneMapOffset);
		this.sceneMap.SetLocalPosition2D(zoneMapOffset);
		this.sceneMap.localScale = InventoryMapManager.SceneMapStartScale;
		Vector3 other = InventoryMapManager.SceneMapEndScale.DivideElements(InventoryMapManager.SceneMapStartScale);
		Vector3 zoneMapEndScale = this.zoneMapInitialScale.MultiplyElements(other);
		this.gameMap.SetIsZoomed(true);
		if (!this.gameMap.CanStartPan())
		{
			zoomToPos = zoomToPos.MultiplyElements(InventoryMapManager.SceneMapEndScale);
		}
		zoomToPos = this.gameMap.GetClampedPosition(zoomToPos, InventoryMapManager.SceneMapEndScale);
		for (float elapsed = 0f; elapsed < this.zoomDuration; elapsed += Time.unscaledDeltaTime)
		{
			float time = elapsed / this.zoomDuration;
			float num = this.zoomCurve.Evaluate(time);
			this.sceneMap.localScale = Vector3.Lerp(InventoryMapManager.SceneMapStartScale, InventoryMapManager.SceneMapEndScale, num);
			this.sceneMap.SetLocalPosition2D(Vector2.Lerp(zoneMapOffset, zoomToPos, num));
			this.sceneMapFade.AlphaSelf = num;
			zoneMap.localScale = Vector3.Lerp(this.zoneMapInitialScale, zoneMapEndScale, num);
			zoneMap.SetLocalPosition2D(Vector2.Lerp(zoneMapOffset, zoomToPos, num));
			zoneMapFade.AlphaSelf = this.zoneMapInCurve.Evaluate(time);
			yield return null;
		}
		zoneMapFade.AlphaSelf = 0f;
		this.sceneMap.localScale = InventoryMapManager.SceneMapEndScale;
		this.sceneMap.SetLocalPosition2D(zoomToPos);
		this.sceneMapFade.AlphaSelf = 1f;
		this.controlFSM.SendEvent(this.zoomedInEvent);
		this.gameMap.KeepWithinBounds(this.sceneMap.localScale);
		this.isZoomed = true;
		this.gameMap.SetIsZoomed(true);
		this.UpdateKeyPromptState(false);
		this.zoomRoutine = null;
		yield break;
	}

	// Token: 0x06003D64 RID: 15716 RVA: 0x0010E345 File Offset: 0x0010C545
	public void SetMarkerZoom(bool isPlacementActive)
	{
		if (this.zoomRoutine != null)
		{
			base.StopCoroutine(this.zoomRoutine);
			this.zoomRoutine = null;
		}
		this.zoomRoutine = base.StartCoroutine(this.ZoomInMarkerRoutine(isPlacementActive));
	}

	// Token: 0x06003D65 RID: 15717 RVA: 0x0010E375 File Offset: 0x0010C575
	private IEnumerator ZoomInMarkerRoutine(bool isPlacementActive)
	{
		Vector3 initialScale = this.sceneMap.localScale;
		Vector2 initialPos = this.sceneMap.localPosition;
		this.gameMap.SetIsMarkerZoom(isPlacementActive);
		Vector3 toScale = isPlacementActive ? InventoryMapManager.SceneMapMarkerZoomScale : InventoryMapManager.SceneMapEndScale;
		bool canStartPan = this.gameMap.CanStartPan();
		Vector3 v = (!isPlacementActive || canStartPan) ? toScale.DivideElements(initialScale) : toScale;
		Vector2 zoomToPos = initialPos.MultiplyElements(v);
		if (!isPlacementActive && !canStartPan)
		{
			zoomToPos = this.previousMarkerZoomPosition;
		}
		zoomToPos = this.gameMap.GetClampedPosition(zoomToPos, toScale);
		this.previousMarkerZoomPosition = initialPos;
		for (float elapsed = 0f; elapsed < this.zoomDuration; elapsed += Time.unscaledDeltaTime)
		{
			float time = elapsed / this.zoomDuration;
			float t = this.zoomCurve.Evaluate(time);
			this.sceneMap.localScale = Vector3.Lerp(initialScale, toScale, t);
			this.sceneMap.SetLocalPosition2D(Vector2.Lerp(initialPos, zoomToPos, t));
			yield return null;
		}
		this.sceneMap.localScale = toScale;
		this.sceneMap.SetLocalPosition2D(zoomToPos);
		if (canStartPan)
		{
			this.gameMap.KeepWithinBounds(this.sceneMap.localScale);
		}
		this.zoomRoutine = null;
		yield break;
	}

	// Token: 0x06003D66 RID: 15718 RVA: 0x0010E38B File Offset: 0x0010C58B
	public void PaneMovePrevented()
	{
		this.controlFSM.SendEvent("UI CANCEL");
	}

	// Token: 0x06003D67 RID: 15719 RVA: 0x0010E39D File Offset: 0x0010C59D
	public void ZoomOut()
	{
		if (this.zoomRoutine != null)
		{
			base.StopCoroutine(this.zoomRoutine);
		}
		this.zoomRoutine = base.StartCoroutine(this.ZoomOutRoutine());
	}

	// Token: 0x06003D68 RID: 15720 RVA: 0x0010E3C5 File Offset: 0x0010C5C5
	private IEnumerator ZoomOutRoutine()
	{
		Vector2 zoomFromPos = this.sceneMap.localPosition;
		this.sceneMap.localScale = InventoryMapManager.SceneMapStartScale;
		Vector3 other = InventoryMapManager.SceneMapEndScale.DivideElements(InventoryMapManager.SceneMapStartScale);
		Vector3 zoneMapEndScale = this.zoneMapInitialScale.MultiplyElements(other);
		base.SetSelected(this.GetClosestSelectable(), null, false);
		this.isZoomed = false;
		this.gameMap.SetIsZoomed(false);
		this.UpdateKeyPromptState(false);
		Transform zoneMap = this.wideMap.transform;
		NestedFadeGroupBase zoneMapFade = this.wideMap.FadeGroup;
		Vector2 zoneMapOffset = this.wideMap.PositionOffset;
		for (float elapsed = 0f; elapsed < this.zoomDuration; elapsed += Time.unscaledDeltaTime)
		{
			float num = this.zoomCurve.Evaluate(elapsed / this.zoomDuration);
			this.sceneMap.localScale = Vector3.Lerp(InventoryMapManager.SceneMapEndScale, InventoryMapManager.SceneMapStartScale, num);
			this.sceneMap.SetLocalPosition2D(Vector2.Lerp(zoomFromPos, zoneMapOffset, num));
			this.sceneMapFade.AlphaSelf = 1f - num;
			zoneMap.localScale = Vector3.Lerp(zoneMapEndScale, this.zoneMapInitialScale, num);
			zoneMap.SetLocalPosition2D(Vector2.Lerp(zoomFromPos, zoneMapOffset, num));
			zoneMapFade.AlphaSelf = num;
			yield return null;
		}
		this.sceneMapFade.AlphaSelf = 0f;
		zoneMap.localScale = this.zoneMapInitialScale;
		zoneMap.SetLocalPosition2D(zoneMapOffset);
		zoneMapFade.AlphaSelf = 1f;
		this.controlFSM.SendEventSafe(this.zoomedOutEvent);
		this.gameMap.KeepWithinBounds(this.sceneMap.localScale);
		base.IsActionsBlocked = false;
		this.paneList.CanSwitchPanes = true;
		this.zoomRoutine = null;
		yield break;
	}

	// Token: 0x06003D69 RID: 15721 RVA: 0x0010E3D4 File Offset: 0x0010C5D4
	private void OnPaneStart()
	{
		base.IsActionsBlocked = false;
		this.EnsureWideMapSpawned();
		this.wideMap.UpdatePositions();
		this.UpdateKeyPromptState(false);
		bool hasNoMap = this.HasNoMap;
		this.noMapSymbol.SetActive(hasNoMap);
		this.hasMapParent.SetActive(!hasNoMap);
	}

	// Token: 0x06003D6A RID: 15722 RVA: 0x0010E424 File Offset: 0x0010C624
	private void UpdateKeyPromptState(bool wasManual)
	{
		if (PlayerData.instance.HasAnyPin && !this.HasNoMap)
		{
			MapPin.PinVisibilityStates currentState = MapPin.CurrentState;
			MapPin.PinVisibilityStates nextState = MapPin.GetNextState(currentState);
			TMP_Text tmp_Text = this.keyText;
			LocalisedString s;
			switch (nextState)
			{
			case MapPin.PinVisibilityStates.PinsAndKey:
				s = this.keyShowText;
				break;
			case MapPin.PinVisibilityStates.Pins:
				s = this.keyHideText;
				break;
			case MapPin.PinVisibilityStates.None:
				s = this.pinHideText;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			tmp_Text.text = s;
			this.keyParent.SetActive(this.isZoomed && currentState == MapPin.PinVisibilityStates.PinsAndKey);
			this.keyPrompt.SetActive(this.isZoomed);
			if (wasManual)
			{
				this.keyToggleAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
				return;
			}
		}
		else
		{
			this.keyParent.SetActive(false);
			this.keyPrompt.SetActive(false);
		}
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x0010E504 File Offset: 0x0010C704
	public override bool SuperButtonSelected()
	{
		if (!this.isZoomed)
		{
			return base.SuperButtonSelected();
		}
		MapPin.CycleState();
		this.UpdateKeyPromptState(true);
		return true;
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x0010E522 File Offset: 0x0010C722
	public bool IsAvailable()
	{
		return PlayerData.instance.HasAnyMap;
	}

	// Token: 0x04003F02 RID: 16130
	public static readonly Vector3 SceneMapStartScale = new Vector3(0.39f, 0.39f, 1f);

	// Token: 0x04003F03 RID: 16131
	public static readonly Vector3 SceneMapEndScale = new Vector3(1.15f, 1.15f, 1f);

	// Token: 0x04003F04 RID: 16132
	public static readonly Vector3 SceneMapMarkerZoomScale = new Vector3(1.4f, 1.4f, 1f);

	// Token: 0x04003F05 RID: 16133
	public static readonly Vector2 SceneMapMarkerZoomScaleV2 = new Vector2(1.4f, 1.4f);

	// Token: 0x04003F06 RID: 16134
	[SerializeField]
	private PlayMakerFSM controlFSM;

	// Token: 0x04003F07 RID: 16135
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string zoomInEvent;

	// Token: 0x04003F08 RID: 16136
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string zoomedInEvent;

	// Token: 0x04003F09 RID: 16137
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	private string zoomedOutEvent;

	// Token: 0x04003F0A RID: 16138
	[SerializeField]
	private AnimationCurve zoneMapInCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04003F0B RID: 16139
	[SerializeField]
	private NestedFadeGroupBase sceneMapFade;

	// Token: 0x04003F0C RID: 16140
	[SerializeField]
	private AnimationCurve zoomCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003F0D RID: 16141
	[SerializeField]
	private float zoomDuration;

	// Token: 0x04003F0E RID: 16142
	[Space]
	[SerializeField]
	private InventoryWideMap wideMapPrefab;

	// Token: 0x04003F0F RID: 16143
	[SerializeField]
	private Transform wideMapParent;

	// Token: 0x04003F10 RID: 16144
	[Space]
	[SerializeField]
	private GameMap gameMapPrefab;

	// Token: 0x04003F11 RID: 16145
	[SerializeField]
	private Transform gameMapParent;

	// Token: 0x04003F12 RID: 16146
	[SerializeField]
	private GameObject panArrowUp;

	// Token: 0x04003F13 RID: 16147
	[SerializeField]
	private GameObject panArrowDown;

	// Token: 0x04003F14 RID: 16148
	[SerializeField]
	private GameObject panArrowLeft;

	// Token: 0x04003F15 RID: 16149
	[SerializeField]
	private GameObject panArrowRight;

	// Token: 0x04003F16 RID: 16150
	[Space]
	[SerializeField]
	private MapMarkerMenu mapMarkerMenu;

	// Token: 0x04003F17 RID: 16151
	[SerializeField]
	private UnityEngine.Camera mapCamera;

	// Token: 0x04003F18 RID: 16152
	[Space]
	[SerializeField]
	private GameObject keyPrompt;

	// Token: 0x04003F19 RID: 16153
	[SerializeField]
	private TMP_Text keyText;

	// Token: 0x04003F1A RID: 16154
	[SerializeField]
	private LocalisedString keyHideText;

	// Token: 0x04003F1B RID: 16155
	[SerializeField]
	private LocalisedString pinHideText;

	// Token: 0x04003F1C RID: 16156
	[SerializeField]
	private LocalisedString keyShowText;

	// Token: 0x04003F1D RID: 16157
	[SerializeField]
	private GameObject keyParent;

	// Token: 0x04003F1E RID: 16158
	[SerializeField]
	private AudioEvent keyToggleAudio;

	// Token: 0x04003F1F RID: 16159
	[Space]
	[SerializeField]
	private GameObject hasMapParent;

	// Token: 0x04003F20 RID: 16160
	[SerializeField]
	private GameObject noMapSymbol;

	// Token: 0x04003F21 RID: 16161
	private InventoryWideMap wideMap;

	// Token: 0x04003F22 RID: 16162
	private GameMap gameMap;

	// Token: 0x04003F23 RID: 16163
	private Vector3 zoneMapInitialScale;

	// Token: 0x04003F24 RID: 16164
	private Transform sceneMap;

	// Token: 0x04003F25 RID: 16165
	private Coroutine zoomRoutine;

	// Token: 0x04003F26 RID: 16166
	private bool isZoomed;

	// Token: 0x04003F27 RID: 16167
	private InventoryPane pane;

	// Token: 0x04003F28 RID: 16168
	private InventoryPaneList paneList;

	// Token: 0x04003F29 RID: 16169
	[NonSerialized]
	private bool hasCreatedScrollArea;

	// Token: 0x04003F2A RID: 16170
	private Bounds markerScrollArea;

	// Token: 0x04003F2B RID: 16171
	private Vector3 previousMarkerZoomPosition;
}
