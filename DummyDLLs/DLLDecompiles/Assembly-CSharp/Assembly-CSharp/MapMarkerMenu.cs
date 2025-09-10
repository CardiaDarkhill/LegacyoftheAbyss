using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006D1 RID: 1745
public class MapMarkerMenu : MonoBehaviour
{
	// Token: 0x06003EF2 RID: 16114 RVA: 0x00115118 File Offset: 0x00113318
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<Animator>(ref this.markers, typeof(MapMarkerMenu.MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize<TextMeshPro>(ref this.amounts, typeof(MapMarkerMenu.MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize<Sprite>(ref this.sprites, typeof(MapMarkerMenu.MarkerTypes));
		ArrayForEnumAttribute.EnsureArraySize<string>(ref this.markerPdBools, typeof(MapMarkerMenu.MarkerTypes));
		this.vpValid = false;
	}

	// Token: 0x06003EF3 RID: 16115 RVA: 0x00115180 File Offset: 0x00113380
	private void Awake()
	{
		this.OnValidate();
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		this.mapManager = base.GetComponentInParent<InventoryMapManager>();
	}

	// Token: 0x06003EF4 RID: 16116 RVA: 0x001151A0 File Offset: 0x001133A0
	private void Start()
	{
		if (this.changeButton)
		{
			this.changeButton.SetActive(false);
		}
		if (this.cancelButton)
		{
			this.cancelButton.SetActive(false);
		}
		if (this.fadeGroup)
		{
			this.fadeGroup.AlphaSelf = 0f;
		}
		this.placementCursor.SetActive(false);
		this.placementBox.transform.parent = this.placementCursor.transform;
		this.IsNotColliding();
	}

	// Token: 0x06003EF5 RID: 16117 RVA: 0x0011522C File Offset: 0x0011342C
	private void Update()
	{
		if (this.inPlacementMode)
		{
			if (!this.PanMap() && this.collidingMarkers.Count > 0)
			{
				List<GameObject> list = this.collidingMarkers;
				Vector3 v = list[list.Count - 1].transform.position - this.gameMap.transform.parent.position;
				Vector2 position = Vector2.Lerp(this.placementCursor.transform.position, v, this.markerPullSpeed * Time.unscaledDeltaTime);
				this.placementCursor.transform.SetPosition2D(position);
			}
			HeroActions inputActions = ManagerSingleton<InputHandler>.Instance.inputActions;
			Platform.MenuActions menuAction = Platform.Current.GetMenuAction(inputActions, false, false);
			if (this.confirmTimer <= 0f)
			{
				if (menuAction != Platform.MenuActions.Submit)
				{
					if (menuAction != Platform.MenuActions.Extra)
					{
						if (this.inputHandler.inputActions.PaneRight.WasPressed && this.confirmTimer <= 0f)
						{
							this.MarkerSelectRight();
						}
						else if (this.inputHandler.inputActions.PaneLeft.WasPressed && this.confirmTimer <= 0f)
						{
							this.MarkerSelectLeft();
						}
					}
					else
					{
						this.MarkerSelectRight();
					}
				}
				else if (this.collidingWithMarker)
				{
					this.RemoveMarker();
				}
				else
				{
					this.PlaceMarker();
				}
			}
		}
		if (this.timer > 0f)
		{
			this.timer -= Time.unscaledDeltaTime;
		}
		if (this.confirmTimer > 0f)
		{
			this.confirmTimer -= Time.unscaledDeltaTime;
		}
		if (this.placementTimer > 0f)
		{
			this.placementTimer -= Time.unscaledDeltaTime;
		}
	}

	// Token: 0x06003EF6 RID: 16118 RVA: 0x001153DC File Offset: 0x001135DC
	public void Open()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		if (this.pd == null)
		{
			this.pd = PlayerData.instance;
		}
		if (this.inputHandler == null)
		{
			this.inputHandler = this.gm.GetComponent<InputHandler>();
		}
		if (this.gameMapObject == null)
		{
			this.gameMap = this.gm.gameMap;
			this.gameMapObject = (this.gameMap ? this.gameMap.gameObject : null);
		}
		this.placementCursor.SetActive(false);
		this.selectedIndex = -1;
		float num = this.xPos_start;
		for (int i = 0; i < this.markerPdBools.Length; i++)
		{
			GameObject gameObject = this.markers[i].gameObject;
			if (!this.pd.GetVariable(this.markerPdBools[i]))
			{
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
				gameObject.transform.localPosition = new Vector3(num, this.markerY, this.markerZ);
				num += this.xPos_interval;
				if (this.selectedIndex < 0)
				{
					List<Vector2> markerList = this.GetMarkerList(i);
					if (9 - markerList.Count > 0)
					{
						this.selectedIndex = i;
					}
				}
			}
		}
		this.markerLayout.ForceUpdateLayoutNoCanvas();
		if (this.selectedIndex < 0)
		{
			for (int j = 0; j < this.markerPdBools.Length; j++)
			{
				if (this.pd.GetVariable(this.markerPdBools[j]))
				{
					this.selectedIndex = j;
					break;
				}
			}
		}
		this.UpdateAmounts();
		this.cursor.SetActive(true);
		this.cursor.transform.localPosition = new Vector3(this.xPos_start, this.markerY, -3f);
		if (this.fadeGroup)
		{
			this.fadeGroup.FadeTo(1f, this.fadeTime, null, true, null);
		}
		this.changeButton.SetActive(true);
		this.cancelButton.SetActive(true);
		this.collidingMarkers.Clear();
		this.timer = 0f;
		this.confirmTimer = this.uiPause;
		this.StartMarkerPlacement();
		this.MarkerSelect(this.selectedIndex, true);
		this.IsNotColliding();
	}

	// Token: 0x06003EF7 RID: 16119 RVA: 0x00115624 File Offset: 0x00113824
	public void Close()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.FadeTo(0f, this.fadeTime, null, true, null);
		}
		this.changeButton.SetActive(false);
		this.cancelButton.SetActive(false);
		this.inPlacementMode = false;
		this.paneList.IsPaneMoveCustom = false;
		this.placementCursor.SetActive(false);
		this.mapManager.SetMarkerZoom(false);
		this.audioSource.PlayOneShot(this.cursorClip);
	}

	// Token: 0x06003EF8 RID: 16120 RVA: 0x001156AC File Offset: 0x001138AC
	private void StartMarkerPlacement()
	{
		this.placementCursor.SetActive(true);
		this.placementCursor.transform.localPosition = this.placementCursorOrigin;
		this.placementBox.transform.parent = this.placementCursor.transform;
		this.placementBox.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.placementBox.transform.position += this.gameMap.transform.parent.position;
		this.confirmTimer = this.uiPause;
		this.inPlacementMode = true;
		this.paneList.IsPaneMoveCustom = true;
		this.mapManager.SetMarkerZoom(true);
		this.audioSource.PlayOneShot(this.cursorClip);
	}

	// Token: 0x06003EF9 RID: 16121 RVA: 0x00115788 File Offset: 0x00113988
	private bool PanMap()
	{
		bool flag;
		Vector2 sticksInput = this.inputHandler.GetSticksInput(out flag);
		if (sticksInput.magnitude <= Mathf.Epsilon)
		{
			return false;
		}
		float num = flag ? (this.panSpeed * 2f) : this.panSpeed;
		Vector2 vector = sticksInput * (num * Time.unscaledDeltaTime);
		Vector2 vector2 = this.placementCursor.transform.localPosition;
		vector2 += vector;
		Vector2 position = this.gameMapObject.transform.localPosition;
		if (vector2.x < this.placementCursorMinX)
		{
			vector2.x = this.placementCursorMinX;
			if (this.placementTimer <= 0f)
			{
				position.x -= vector.x;
			}
		}
		else if (vector2.x > this.placementCursorMaxX)
		{
			vector2.x = this.placementCursorMaxX;
			if (this.placementTimer <= 0f)
			{
				position.x -= vector.x;
			}
		}
		if (vector2.y < this.placementCursorMinY)
		{
			vector2.y = this.placementCursorMinY;
			if (this.placementTimer <= 0f)
			{
				position.y -= vector.y;
			}
		}
		else if (vector2.y > this.placementCursorMaxY)
		{
			vector2.y = this.placementCursorMaxY;
			if (this.placementTimer <= 0f)
			{
				position.y -= vector.y;
			}
		}
		this.placementCursor.transform.SetLocalPosition2D(vector2);
		if (this.gameMap.CanMarkerPan())
		{
			this.gameMapObject.transform.SetLocalPosition2D(position);
			this.gameMap.KeepWithinBounds(InventoryMapManager.SceneMapMarkerZoomScale);
		}
		return true;
	}

	// Token: 0x06003EFA RID: 16122 RVA: 0x00115944 File Offset: 0x00113B44
	private void MarkerSelect(int selection, bool isInstant)
	{
		for (int i = 0; i < this.markers.Length; i++)
		{
			Transform transform = this.markers[i].transform;
			if (i == selection)
			{
				transform.localScale = new Vector3(1.1f, 1.1f, 1f);
				Vector3 position = transform.position;
				Vector3 vector = base.transform.InverseTransformPoint(position);
				Vector3 vector2 = new Vector3(vector.x, this.markerY, -3f);
				if (isInstant)
				{
					this.cursor.transform.localPosition = vector2;
				}
				else
				{
					this.cursorTweenFSM.FsmVariables.GetFsmVector3("Tween Pos").Value = vector2;
					this.cursorTweenFSM.SendEvent("TWEEN");
				}
			}
			else
			{
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (!isInstant)
		{
			this.audioSource.PlayOneShot(this.cursorClip);
		}
	}

	// Token: 0x06003EFB RID: 16123 RVA: 0x00115A38 File Offset: 0x00113C38
	private List<Vector2> GetMarkerList(int markerTypeIndex)
	{
		ArrayForEnumAttribute.EnsureArraySize<WrappedVector2List>(ref this.pd.placedMarkers, typeof(MapMarkerMenu.MarkerTypes));
		return this.pd.placedMarkers[markerTypeIndex].List;
	}

	// Token: 0x06003EFC RID: 16124 RVA: 0x00115A68 File Offset: 0x00113C68
	private void PlaceMarker()
	{
		List<Vector2> markerList = this.GetMarkerList(this.selectedIndex);
		if (9 - markerList.Count > 0)
		{
			this.placementBox.transform.parent = this.gameMapObject.transform;
			Vector3 localPosition = this.placementBox.transform.localPosition;
			Vector2 item = new Vector2(localPosition.x, localPosition.y);
			this.placementBox.transform.parent = this.placementCursor.transform;
			GameObject gameObject = this.placeEffectPrefab.Spawn(this.placementCursor.transform.position, Quaternion.Euler(0f, 0f, 0f));
			Transform transform = gameObject.transform;
			Vector3 position = transform.position;
			transform.position = new Vector3(position.x, position.y, -30f);
			markerList.Add(item);
			gameObject.GetComponent<SpriteRenderer>().sprite = this.sprites[this.selectedIndex];
			this.UpdateAmounts();
			this.gameMap.SetupMapMarkers();
			this.audioSource.PlayOneShot(this.placeClip);
			VibrationManager.PlayVibrationClipOneShot(this.placementVibration, null, false, "", false);
			this.placementTimer = 0.3f;
			return;
		}
		this.failureSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
		this.markers[this.selectedIndex].SetTrigger(MapMarkerMenu._failedPropId);
	}

	// Token: 0x06003EFD RID: 16125 RVA: 0x00115BE8 File Offset: 0x00113DE8
	private void RemoveMarker()
	{
		List<GameObject> list = this.collidingMarkers;
		GameObject gameObject = list[list.Count - 1];
		MapMarkerMenu.MarkerTypes colour = gameObject.GetComponent<InvMarker>().Colour;
		int index = gameObject.GetComponent<InvMarker>().Index;
		GameObject gameObject2 = this.removeEffectPrefab.Spawn(this.placementCursor.transform.position, Quaternion.Euler(0f, 0f, 0f));
		Transform transform = gameObject2.transform;
		Vector3 position = transform.position;
		transform.position = new Vector3(position.x, position.y, -30f);
		int num = (int)colour;
		this.pd.placedMarkers[num].List.RemoveAt(index);
		gameObject2.GetComponent<SpriteRenderer>().sprite = this.sprites[num];
		this.collidingMarkers.Remove(gameObject);
		if (this.collidingMarkers.Count <= 0)
		{
			this.IsNotColliding();
		}
		this.audioSource.PlayOneShot(this.removeClip);
		VibrationManager.PlayVibrationClipOneShot(this.placementVibration, null, false, "", false);
		this.UpdateAmounts();
		this.gameMap.SetupMapMarkers();
	}

	// Token: 0x06003EFE RID: 16126 RVA: 0x00115D07 File Offset: 0x00113F07
	private void MarkerSelectLeft()
	{
		if (!this.MarkerSelectMoveValidated(-1))
		{
			return;
		}
		this.timer = this.uiPause;
		this.MarkerSelect(this.selectedIndex, false);
	}

	// Token: 0x06003EFF RID: 16127 RVA: 0x00115D2C File Offset: 0x00113F2C
	private void MarkerSelectRight()
	{
		if (!this.MarkerSelectMoveValidated(1))
		{
			return;
		}
		this.timer = this.uiPause;
		this.MarkerSelect(this.selectedIndex, false);
	}

	// Token: 0x06003F00 RID: 16128 RVA: 0x00115D54 File Offset: 0x00113F54
	private bool MarkerSelectMoveValidated(int direction)
	{
		int num = this.selectedIndex;
		foreach (string text in this.markerPdBools)
		{
			this.selectedIndex += direction;
			if (this.selectedIndex < 0)
			{
				this.selectedIndex = this.markerPdBools.Length - 1;
			}
			else if (this.selectedIndex >= this.markerPdBools.Length)
			{
				this.selectedIndex = 0;
			}
			if (this.pd.GetVariable(this.markerPdBools[this.selectedIndex]))
			{
				break;
			}
		}
		return this.selectedIndex != num;
	}

	// Token: 0x06003F01 RID: 16129 RVA: 0x00115DE8 File Offset: 0x00113FE8
	private void UpdateAmounts()
	{
		for (int i = 0; i < this.amounts.Length; i++)
		{
			List<Vector2> markerList = this.GetMarkerList(i);
			int num = 9 - markerList.Count;
			TextMeshPro textMeshPro = this.amounts[i];
			textMeshPro.text = num.ToString();
			SpriteRenderer componentInChildren = this.markers[i].GetComponentInChildren<SpriteRenderer>();
			if (num > 0)
			{
				componentInChildren.color = this.enabledColour;
				textMeshPro.color = this.enabledColour;
			}
			else
			{
				componentInChildren.color = this.disabledColour;
				textMeshPro.color = this.disabledColour;
			}
		}
	}

	// Token: 0x06003F02 RID: 16130 RVA: 0x00115E77 File Offset: 0x00114077
	public void AddToCollidingList(GameObject go)
	{
		this.collidingMarkers.AddIfNotPresent(go);
		this.IsColliding();
	}

	// Token: 0x06003F03 RID: 16131 RVA: 0x00115E8C File Offset: 0x0011408C
	public void RemoveFromCollidingList(GameObject go)
	{
		this.collidingMarkers.Remove(go);
		if (this.collidingMarkers.Count <= 0)
		{
			this.IsNotColliding();
		}
	}

	// Token: 0x06003F04 RID: 16132 RVA: 0x00115EAF File Offset: 0x001140AF
	private void IsColliding()
	{
		this.collidingWithMarker = true;
		this.actionText.text = this.removeString;
	}

	// Token: 0x06003F05 RID: 16133 RVA: 0x00115ECE File Offset: 0x001140CE
	private void IsNotColliding()
	{
		this.collidingWithMarker = false;
		this.actionText.text = this.placeString;
	}

	// Token: 0x06003F06 RID: 16134 RVA: 0x00115EF0 File Offset: 0x001140F0
	public void UpdateVP()
	{
		if (this.vpValid)
		{
			return;
		}
		GameCameras instance = GameCameras.instance;
		Vector3 position = new Vector3(this.placementCursorMinX, this.placementCursorMinY);
		Vector3 position2 = new Vector3(this.placementCursorMaxX, this.placementCursorMaxY);
		Vector3 position3 = base.transform.TransformPoint(position);
		Vector3 position4 = base.transform.TransformPoint(position2);
		if (instance)
		{
			UnityEngine.Camera hudCamera = instance.hudCamera;
			if (hudCamera)
			{
				this.viewMin = hudCamera.WorldToViewportPoint(position3);
				this.viewMax = hudCamera.WorldToViewportPoint(position4);
				this.vpValid = true;
			}
		}
		if (!this.vpValid)
		{
			UnityEngine.Camera main = UnityEngine.Camera.main;
			if (main)
			{
				this.viewMin = main.WorldToViewportPoint(position3);
				this.viewMax = main.WorldToViewportPoint(position4);
			}
		}
	}

	// Token: 0x06003F07 RID: 16135 RVA: 0x00115FBF File Offset: 0x001141BF
	public void GetViewMinMax(out Vector3 viewMin, out Vector3 viewMax)
	{
		this.UpdateVP();
		viewMin = this.viewMin;
		viewMax = this.viewMax;
	}

	// Token: 0x06003F08 RID: 16136 RVA: 0x00115FE0 File Offset: 0x001141E0
	private void DrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawLine(new Vector3(this.placementCursorMinX, this.placementCursorMinY, 0f), new Vector3(this.placementCursorMaxX, this.placementCursorMinY, 100f));
		Gizmos.DrawLine(new Vector3(this.placementCursorMaxX, this.placementCursorMinY, 0f), new Vector3(this.placementCursorMaxX, this.placementCursorMaxY, 100f));
		Gizmos.DrawLine(new Vector3(this.placementCursorMaxX, this.placementCursorMaxY, 0f), new Vector3(this.placementCursorMinX, this.placementCursorMaxY, 100f));
		Gizmos.DrawLine(new Vector3(this.placementCursorMinX, this.placementCursorMaxY, 0f), new Vector3(this.placementCursorMinX, this.placementCursorMinY, 100f));
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06003F09 RID: 16137 RVA: 0x001160D5 File Offset: 0x001142D5
	private void OnDrawGizmos()
	{
		this.DrawGizmos();
	}

	// Token: 0x04004098 RID: 16536
	public float xPos_start = 1.9f;

	// Token: 0x04004099 RID: 16537
	public float xPos_interval = 1.4333f;

	// Token: 0x0400409A RID: 16538
	public float markerY = -12.82f;

	// Token: 0x0400409B RID: 16539
	public float markerZ = -1f;

	// Token: 0x0400409C RID: 16540
	public float uiPause = 0.2f;

	// Token: 0x0400409D RID: 16541
	[Space]
	[SerializeField]
	private LayoutGroup markerLayout;

	// Token: 0x0400409E RID: 16542
	[Space]
	public NestedFadeGroupBase fadeGroup;

	// Token: 0x0400409F RID: 16543
	public float fadeTime = 0.2f;

	// Token: 0x040040A0 RID: 16544
	[Space]
	public AudioSource audioSource;

	// Token: 0x040040A1 RID: 16545
	public AudioClip placeClip;

	// Token: 0x040040A2 RID: 16546
	public AudioClip removeClip;

	// Token: 0x040040A3 RID: 16547
	public AudioClip cursorClip;

	// Token: 0x040040A4 RID: 16548
	public RandomAudioClipTable failureSound;

	// Token: 0x040040A5 RID: 16549
	public VibrationData placementVibration;

	// Token: 0x040040A6 RID: 16550
	[Space]
	public GameObject cursor;

	// Token: 0x040040A7 RID: 16551
	public PlayMakerFSM cursorTweenFSM;

	// Token: 0x040040A8 RID: 16552
	public GameObject placementCursor;

	// Token: 0x040040A9 RID: 16553
	public GameObject placementBox;

	// Token: 0x040040AA RID: 16554
	public GameObject changeButton;

	// Token: 0x040040AB RID: 16555
	public GameObject cancelButton;

	// Token: 0x040040AC RID: 16556
	public TextMeshPro actionText;

	// Token: 0x040040AD RID: 16557
	[Space]
	[ArrayForEnum(typeof(MapMarkerMenu.MarkerTypes))]
	public Animator[] markers;

	// Token: 0x040040AE RID: 16558
	[ArrayForEnum(typeof(MapMarkerMenu.MarkerTypes))]
	public TextMeshPro[] amounts;

	// Token: 0x040040AF RID: 16559
	[Space]
	public Vector3 placementCursorOrigin;

	// Token: 0x040040B0 RID: 16560
	public float panSpeed;

	// Token: 0x040040B1 RID: 16561
	public float markerPullSpeed;

	// Token: 0x040040B2 RID: 16562
	public float placementCursorMinX;

	// Token: 0x040040B3 RID: 16563
	public float placementCursorMaxX;

	// Token: 0x040040B4 RID: 16564
	public float placementCursorMinY;

	// Token: 0x040040B5 RID: 16565
	public float placementCursorMaxY;

	// Token: 0x040040B6 RID: 16566
	[Space]
	public List<GameObject> collidingMarkers;

	// Token: 0x040040B7 RID: 16567
	[Space]
	public GameObject placeEffectPrefab;

	// Token: 0x040040B8 RID: 16568
	public GameObject removeEffectPrefab;

	// Token: 0x040040B9 RID: 16569
	[ArrayForEnum(typeof(MapMarkerMenu.MarkerTypes))]
	public Sprite[] sprites;

	// Token: 0x040040BA RID: 16570
	[ArrayForEnum(typeof(MapMarkerMenu.MarkerTypes))]
	[PlayerDataField(typeof(bool), true)]
	public string[] markerPdBools;

	// Token: 0x040040BB RID: 16571
	private GameManager gm;

	// Token: 0x040040BC RID: 16572
	private PlayerData pd;

	// Token: 0x040040BD RID: 16573
	private InputHandler inputHandler;

	// Token: 0x040040BE RID: 16574
	private GameObject gameMapObject;

	// Token: 0x040040BF RID: 16575
	private GameMap gameMap;

	// Token: 0x040040C0 RID: 16576
	private InventoryPaneList paneList;

	// Token: 0x040040C1 RID: 16577
	private InventoryMapManager mapManager;

	// Token: 0x040040C2 RID: 16578
	private bool[] hasMarkers;

	// Token: 0x040040C3 RID: 16579
	private bool inPlacementMode;

	// Token: 0x040040C4 RID: 16580
	private int selectedIndex;

	// Token: 0x040040C5 RID: 16581
	private float timer;

	// Token: 0x040040C6 RID: 16582
	private float confirmTimer;

	// Token: 0x040040C7 RID: 16583
	private float placementTimer;

	// Token: 0x040040C8 RID: 16584
	private readonly Color enabledColour = new Color(1f, 1f, 1f, 1f);

	// Token: 0x040040C9 RID: 16585
	private readonly Color disabledColour = new Color(0.5f, 0.5f, 0.5f, 1f);

	// Token: 0x040040CA RID: 16586
	private bool collidingWithMarker;

	// Token: 0x040040CB RID: 16587
	private readonly LocalisedString placeString = new LocalisedString("UI", "CTRL_MARKER_PLACE");

	// Token: 0x040040CC RID: 16588
	private readonly LocalisedString removeString = new LocalisedString("UI", "CTRL_MARKER_REMOVE");

	// Token: 0x040040CD RID: 16589
	private static readonly int _failedPropId = Animator.StringToHash("Failed");

	// Token: 0x040040CE RID: 16590
	[SerializeField]
	private UnityEngine.Camera mapCamera;

	// Token: 0x040040CF RID: 16591
	private Vector3 viewMin;

	// Token: 0x040040D0 RID: 16592
	private Vector3 viewMax;

	// Token: 0x040040D1 RID: 16593
	private bool vpValid;

	// Token: 0x020019D0 RID: 6608
	public enum MarkerTypes
	{
		// Token: 0x0400973E RID: 38718
		A,
		// Token: 0x0400973F RID: 38719
		B,
		// Token: 0x04009740 RID: 38720
		C,
		// Token: 0x04009741 RID: 38721
		D,
		// Token: 0x04009742 RID: 38722
		E
	}
}
