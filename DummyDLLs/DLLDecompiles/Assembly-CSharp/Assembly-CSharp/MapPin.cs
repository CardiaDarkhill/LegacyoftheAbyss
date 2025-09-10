using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006D5 RID: 1749
public class MapPin : MonoBehaviour
{
	// Token: 0x17000742 RID: 1858
	// (get) Token: 0x06003F3B RID: 16187 RVA: 0x00117523 File Offset: 0x00115723
	public static MapPin.PinVisibilityStates CurrentState
	{
		get
		{
			return (MapPin.PinVisibilityStates)Platform.Current.RoamingSharedData.GetInt("MapPinVisibilityState", 0);
		}
	}

	// Token: 0x17000743 RID: 1859
	// (get) Token: 0x06003F3C RID: 16188 RVA: 0x0011753A File Offset: 0x0011573A
	// (set) Token: 0x06003F3D RID: 16189 RVA: 0x00117542 File Offset: 0x00115742
	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
		set
		{
			this.isActive = value;
			this.ApplyState(MapPin.CurrentState);
		}
	}

	// Token: 0x06003F3E RID: 16190 RVA: 0x00117556 File Offset: 0x00115756
	private void Awake()
	{
		this.AddPin();
		this.CheckDidActivate();
	}

	// Token: 0x06003F3F RID: 16191 RVA: 0x00117564 File Offset: 0x00115764
	private void Start()
	{
		this.ApplyState(MapPin.CurrentState);
	}

	// Token: 0x06003F40 RID: 16192 RVA: 0x00117571 File Offset: 0x00115771
	private void OnDestroy()
	{
		if (this.added)
		{
			MapPin._activePins.Remove(this);
			this.added = false;
		}
	}

	// Token: 0x06003F41 RID: 16193 RVA: 0x00117590 File Offset: 0x00115790
	private void OnEnable()
	{
		if (this.pinLayoutState == 0)
		{
			this.pinLayout = base.GetComponentInParent<GameMapPinLayout>();
			this.pinLayoutState = ((this.pinLayout != null) ? 1 : -1);
		}
		if (this.pinLayoutState >= 1)
		{
			this.pinLayout.SetLayoutDirty();
		}
	}

	// Token: 0x06003F42 RID: 16194 RVA: 0x001175DD File Offset: 0x001157DD
	private void OnDisable()
	{
		if (this.pinLayoutState >= 1)
		{
			this.pinLayout.SetLayoutDirty();
		}
	}

	// Token: 0x06003F43 RID: 16195 RVA: 0x001175F3 File Offset: 0x001157F3
	public void AddPin()
	{
		if (!this.added)
		{
			MapPin._activePins.Add(this);
			this.added = true;
		}
	}

	// Token: 0x06003F44 RID: 16196 RVA: 0x0011760F File Offset: 0x0011580F
	public static void ClearActivePins()
	{
		MapPin._activePins.Clear();
	}

	// Token: 0x17000744 RID: 1860
	// (get) Token: 0x06003F45 RID: 16197 RVA: 0x0011761C File Offset: 0x0011581C
	public static int ActivePinCount
	{
		get
		{
			int num = 0;
			using (List<MapPin>.Enumerator enumerator = MapPin._activePins.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.CanBeActive())
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	// Token: 0x17000745 RID: 1861
	// (get) Token: 0x06003F46 RID: 16198 RVA: 0x00117674 File Offset: 0x00115874
	// (set) Token: 0x06003F47 RID: 16199 RVA: 0x001176C8 File Offset: 0x001158C8
	public static bool DidActivateNewPin
	{
		get
		{
			foreach (MapPin mapPin in MapPin._activePins)
			{
				mapPin.CheckDidActivate();
			}
			return MapPin.didActivateNewPin;
		}
		set
		{
			MapPin.didActivateNewPin = value;
		}
	}

	// Token: 0x06003F48 RID: 16200 RVA: 0x001176D0 File Offset: 0x001158D0
	private void CheckDidActivate()
	{
		if (this.didActivate)
		{
			return;
		}
		if (this.CanBeActive())
		{
			this.didActivate = true;
			MapPin.DidActivateNewPin = true;
		}
	}

	// Token: 0x06003F49 RID: 16201 RVA: 0x001176F0 File Offset: 0x001158F0
	private bool CanBeActive()
	{
		if (this.hideIfOtherActive && this.hideIfOtherActive.CanBeActive())
		{
			return false;
		}
		if (!this.parentScene)
		{
			this.parentScene = base.GetComponentInParent<GameMapScene>(true);
		}
		MapPin.ActiveConditions activeConditions = this.activeCondition;
		if (activeConditions != MapPin.ActiveConditions.None)
		{
			if (activeConditions != MapPin.ActiveConditions.CurrentMapZone)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (!this.gameMap)
			{
				this.gameMap = base.GetComponentInParent<GameMap>(true);
			}
			if (this.gameMap.GetMapZoneForScene(this.parentScene.transform) != this.gameMap.GetCurrentMapZone())
			{
				return false;
			}
		}
		return this.IsActive && (!this.parentScene || this.parentScene.IsMapped || (this.parentScene.InitialState != GameMapScene.States.Hidden && this.parentScene.IsVisited));
	}

	// Token: 0x06003F4A RID: 16202 RVA: 0x001177C6 File Offset: 0x001159C6
	private void ApplyState(MapPin.PinVisibilityStates state)
	{
		if (state <= MapPin.PinVisibilityStates.Pins)
		{
			base.gameObject.SetActive(this.CanBeActive());
			return;
		}
		if (state != MapPin.PinVisibilityStates.None)
		{
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003F4B RID: 16203 RVA: 0x00117802 File Offset: 0x00115A02
	public static MapPin.PinVisibilityStates GetNextState(MapPin.PinVisibilityStates currentState)
	{
		switch (currentState)
		{
		case MapPin.PinVisibilityStates.PinsAndKey:
			return MapPin.PinVisibilityStates.Pins;
		case MapPin.PinVisibilityStates.Pins:
			return MapPin.PinVisibilityStates.None;
		case MapPin.PinVisibilityStates.None:
			return MapPin.PinVisibilityStates.PinsAndKey;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06003F4C RID: 16204 RVA: 0x00117824 File Offset: 0x00115A24
	public static void CycleState()
	{
		MapPin.PinVisibilityStates pinVisibilityStates = MapPin.CurrentState;
		pinVisibilityStates = MapPin.GetNextState(pinVisibilityStates);
		for (int i = MapPin._activePins.Count - 1; i >= 0; i--)
		{
			MapPin._activePins[i].ApplyState(pinVisibilityStates);
		}
		Platform.Current.RoamingSharedData.SetInt("MapPinVisibilityState", (int)pinVisibilityStates);
	}

	// Token: 0x06003F4D RID: 16205 RVA: 0x0011787C File Offset: 0x00115A7C
	public static void ToggleQuickMapView(bool isQuickMap)
	{
		MapPin.PinVisibilityStates state = MapPin.CurrentState;
		if (isQuickMap)
		{
			state = MapPin.PinVisibilityStates.PinsAndKey;
		}
		for (int i = MapPin._activePins.Count - 1; i >= 0; i--)
		{
			MapPin._activePins[i].ApplyState(state);
		}
	}

	// Token: 0x0400410B RID: 16651
	[SerializeField]
	private MapPin.ActiveConditions activeCondition;

	// Token: 0x0400410C RID: 16652
	[SerializeField]
	private MapPin hideIfOtherActive;

	// Token: 0x0400410D RID: 16653
	private const string SAVE_KEY = "MapPinVisibilityState";

	// Token: 0x0400410E RID: 16654
	private GameMapScene parentScene;

	// Token: 0x0400410F RID: 16655
	private GameMap gameMap;

	// Token: 0x04004110 RID: 16656
	private static readonly List<MapPin> _activePins = new List<MapPin>();

	// Token: 0x04004111 RID: 16657
	private bool isActive = true;

	// Token: 0x04004112 RID: 16658
	private bool didQuickMapChange;

	// Token: 0x04004113 RID: 16659
	private bool didActivate;

	// Token: 0x04004114 RID: 16660
	private int pinLayoutState;

	// Token: 0x04004115 RID: 16661
	private GameMapPinLayout pinLayout;

	// Token: 0x04004116 RID: 16662
	private bool added;

	// Token: 0x04004117 RID: 16663
	private static bool didActivateNewPin;

	// Token: 0x020019D2 RID: 6610
	public enum PinVisibilityStates
	{
		// Token: 0x04009746 RID: 38726
		PinsAndKey,
		// Token: 0x04009747 RID: 38727
		Pins,
		// Token: 0x04009748 RID: 38728
		None
	}

	// Token: 0x020019D3 RID: 6611
	private enum ActiveConditions
	{
		// Token: 0x0400974A RID: 38730
		None,
		// Token: 0x0400974B RID: 38731
		CurrentMapZone
	}
}
