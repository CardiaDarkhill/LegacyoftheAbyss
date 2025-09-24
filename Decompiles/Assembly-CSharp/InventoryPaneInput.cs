using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
public class InventoryPaneInput : MonoBehaviour
{
	// Token: 0x14000034 RID: 52
	// (add) Token: 0x060011C1 RID: 4545 RVA: 0x00052EB0 File Offset: 0x000510B0
	// (remove) Token: 0x060011C2 RID: 4546 RVA: 0x00052EE8 File Offset: 0x000510E8
	public event Action OnActivated;

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x060011C3 RID: 4547 RVA: 0x00052F20 File Offset: 0x00051120
	// (remove) Token: 0x060011C4 RID: 4548 RVA: 0x00052F58 File Offset: 0x00051158
	public event Action OnDeactivated;

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060011C5 RID: 4549 RVA: 0x00052F8D File Offset: 0x0005118D
	public float ListScrollSpeed
	{
		get
		{
			if (!this.isScrollingFast)
			{
				return 0.15f;
			}
			return 0.03f;
		}
	}

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x060011C6 RID: 4550 RVA: 0x00052FA2 File Offset: 0x000511A2
	// (set) Token: 0x060011C7 RID: 4551 RVA: 0x00052FBC File Offset: 0x000511BC
	public static bool IsInputBlocked
	{
		get
		{
			return StaticVariableList.Exists("IsUIListInputBlocked") && StaticVariableList.GetValue<bool>("IsUIListInputBlocked");
		}
		set
		{
			StaticVariableList.SetValue("IsUIListInputBlocked", value, 0);
		}
	}

	// Token: 0x060011C8 RID: 4552 RVA: 0x00052FCF File Offset: 0x000511CF
	private void Awake()
	{
		this.pane = base.GetComponent<InventoryPaneBase>();
		this.paneList = base.GetComponentInParent<InventoryPaneList>();
		this.isInInventory = (this.paneList != null);
	}

	// Token: 0x060011C9 RID: 4553 RVA: 0x00052FFC File Offset: 0x000511FC
	private void OnEnable()
	{
		this.wasSubmitPressed = false;
		this.wasExtraPressed = false;
		this.actionCooldown = 0f;
		this.directionRepeatTimer = 0f;
		this.isRepeatingDirection = false;
		this.isScrollingFast = false;
		UIManager instance = UIManager.instance;
		if (instance)
		{
			this.menuSubmitVibration = instance.menuSubmitVibration;
			this.menuCancelVibration = instance.menuCancelVibration;
		}
		Action onActivated = this.OnActivated;
		if (onActivated == null)
		{
			return;
		}
		onActivated();
	}

	// Token: 0x060011CA RID: 4554 RVA: 0x00053071 File Offset: 0x00051271
	private void OnDisable()
	{
		Action onDeactivated = this.OnDeactivated;
		if (onDeactivated == null)
		{
			return;
		}
		onDeactivated();
	}

	// Token: 0x060011CB RID: 4555 RVA: 0x00053083 File Offset: 0x00051283
	private void Start()
	{
		this.ih = ManagerSingleton<InputHandler>.Instance;
		this.platform = Platform.Current;
	}

	// Token: 0x060011CC RID: 4556 RVA: 0x0005309C File Offset: 0x0005129C
	private void Update()
	{
		if (this.actionCooldown > 0f)
		{
			this.actionCooldown -= Time.unscaledDeltaTime;
		}
		if (InventoryPaneInput.IsInputBlocked)
		{
			return;
		}
		if (CheatManager.IsOpen)
		{
			return;
		}
		HeroActions inputActions = this.ih.inputActions;
		switch (this.platform.GetMenuAction(inputActions, false, false))
		{
		case Platform.MenuActions.Submit:
			this.PressSubmit();
			return;
		case Platform.MenuActions.Cancel:
			this.PressCancel();
			return;
		case Platform.MenuActions.Extra:
			if (this.actionCooldown <= 0f)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI EXTRA", false);
				this.wasExtraPressed = true;
				this.isRepeatingDirection = false;
				this.actionCooldown = 0.25f;
				return;
			}
			return;
		case Platform.MenuActions.Super:
			if (this.actionCooldown <= 0f)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI SUPER", false);
				this.isRepeatingDirection = false;
				this.actionCooldown = 0.25f;
				return;
			}
			return;
		}
		Platform.MenuActions menuAction = this.platform.GetMenuAction(inputActions, false, true);
		InventoryPaneList.PaneTypes inventoryInputPressed = InventoryPaneInput.GetInventoryInputPressed(inputActions);
		if (this.wasSubmitPressed && menuAction != Platform.MenuActions.Submit)
		{
			this.ReleaseSubmit();
		}
		if (this.wasExtraPressed && menuAction != Platform.MenuActions.Extra)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "UI EXTRA RELEASED", false);
			this.wasExtraPressed = false;
			this.isRepeatingDirection = false;
			return;
		}
		if (inputActions.Right.WasPressed)
		{
			this.PressDirection(InventoryPaneBase.InputEventType.Right);
			return;
		}
		if (inputActions.Left.WasPressed)
		{
			this.PressDirection(InventoryPaneBase.InputEventType.Left);
			return;
		}
		if (inputActions.Up.WasPressed)
		{
			this.PressDirection(InventoryPaneBase.InputEventType.Up);
			return;
		}
		if (inputActions.Down.WasPressed)
		{
			this.PressDirection(InventoryPaneBase.InputEventType.Down);
			return;
		}
		if (inventoryInputPressed != InventoryPaneList.PaneTypes.None)
		{
			bool flag;
			switch (this.paneControl)
			{
			case InventoryPaneList.PaneTypes.None:
				flag = true;
				break;
			case InventoryPaneList.PaneTypes.Inv:
				flag = inputActions.OpenInventory.WasPressed;
				break;
			case InventoryPaneList.PaneTypes.Tools:
				flag = (inputActions.OpenInventoryTools.WasPressed || inputActions.SwipeInventoryTools.WasPressed);
				break;
			case InventoryPaneList.PaneTypes.Quests:
				flag = (inputActions.OpenInventoryQuests.WasPressed || inputActions.SwipeInventoryQuests);
				break;
			case InventoryPaneList.PaneTypes.Journal:
				flag = (inputActions.OpenInventoryJournal.WasPressed || inputActions.SwipeInventoryJournal.WasPressed);
				break;
			case InventoryPaneList.PaneTypes.Map:
				flag = (inputActions.OpenInventoryMap.WasPressed || inputActions.SwipeInventoryMap.WasPressed);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			bool flag2 = flag;
			if (!flag2)
			{
				InventoryPane inventoryPane = this.paneList.GetPane(inventoryInputPressed);
				if (inventoryPane == null || !inventoryPane.IsAvailable)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				this.PressCancel();
				return;
			}
			PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(this.paneList.gameObject, "Inventory Control");
			playMakerFSM.FsmVariables.FindFsmInt("Target Pane Index").Value = (int)inventoryInputPressed;
			playMakerFSM.SendEvent("MOVE PANE TO");
			return;
		}
		else if (inputActions.RsDown.WasPressed)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "UI RS DOWN", false);
			if (this.allowRightStickSpeed)
			{
				this.PressDirection(InventoryPaneBase.InputEventType.Down);
				this.isScrollingFast = true;
				this.directionRepeatTimer = this.ListScrollSpeed;
				return;
			}
		}
		else if (inputActions.RsUp.WasPressed)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "UI RS UP", false);
			if (this.allowRightStickSpeed)
			{
				this.PressDirection(InventoryPaneBase.InputEventType.Up);
				this.isScrollingFast = true;
				this.directionRepeatTimer = this.ListScrollSpeed;
				return;
			}
		}
		else if (inputActions.RsLeft.WasPressed)
		{
			if (this.isInInventory)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI RS LEFT", false);
			}
			if (this.allowRightStickSpeed)
			{
				this.PressDirection(InventoryPaneBase.InputEventType.Left);
				this.isScrollingFast = true;
				this.directionRepeatTimer = this.ListScrollSpeed;
				return;
			}
		}
		else if (inputActions.RsRight.WasPressed)
		{
			if (this.isInInventory)
			{
				FSMUtility.SendEventToGameObject(base.gameObject, "UI RS RIGHT", false);
			}
			if (this.allowRightStickSpeed)
			{
				this.PressDirection(InventoryPaneBase.InputEventType.Right);
				this.isScrollingFast = true;
				this.directionRepeatTimer = this.ListScrollSpeed;
				return;
			}
		}
		else if (this.isRepeatingDirection)
		{
			bool flag;
			switch (this.lastPressedDirection)
			{
			case InventoryPaneBase.InputEventType.Left:
				flag = inputActions.Left.IsPressed;
				break;
			case InventoryPaneBase.InputEventType.Right:
				flag = inputActions.Right.IsPressed;
				break;
			case InventoryPaneBase.InputEventType.Up:
				flag = (this.isScrollingFast ? inputActions.RsUp.IsPressed : inputActions.Up.IsPressed);
				break;
			case InventoryPaneBase.InputEventType.Down:
				flag = (this.isScrollingFast ? inputActions.RsDown.IsPressed : inputActions.Down.IsPressed);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!flag)
			{
				this.isRepeatingDirection = false;
				return;
			}
			this.directionRepeatTimer -= Time.unscaledDeltaTime;
			if (this.directionRepeatTimer <= 0f)
			{
				this.PressDirection(this.lastPressedDirection);
				this.directionRepeatTimer = this.ListScrollSpeed;
				return;
			}
		}
		else if (this.isRepeatingSubmit)
		{
			this.directionRepeatTimer -= Time.unscaledDeltaTime;
			if (this.directionRepeatTimer <= 0f)
			{
				this.ReleaseSubmit();
				this.PressSubmit();
				this.directionRepeatTimer = this.ListScrollSpeed;
				return;
			}
		}
		else
		{
			this.isScrollingFast = false;
		}
	}

	// Token: 0x060011CD RID: 4557 RVA: 0x000535B8 File Offset: 0x000517B8
	public static InventoryPaneList.PaneTypes GetInventoryButtonPressed(HeroActions ia)
	{
		InventoryPaneList.PaneTypes result = InventoryPaneList.PaneTypes.None;
		if (ia.OpenInventory.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Inv;
		}
		else if (ia.OpenInventoryMap.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Map;
		}
		else if (ia.OpenInventoryJournal.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Journal;
		}
		else if (ia.OpenInventoryTools.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Tools;
		}
		else if (ia.OpenInventoryQuests.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Quests;
		}
		return result;
	}

	// Token: 0x060011CE RID: 4558 RVA: 0x0005361C File Offset: 0x0005181C
	public static InventoryPaneList.PaneTypes GetInventoryInputPressed(HeroActions ia)
	{
		InventoryPaneList.PaneTypes result = InventoryPaneList.PaneTypes.None;
		if (ia.OpenInventory.WasPressed)
		{
			result = InventoryPaneList.PaneTypes.Inv;
		}
		else if (ia.OpenInventoryMap.WasPressed || ia.SwipeInventoryMap)
		{
			result = InventoryPaneList.PaneTypes.Map;
		}
		else if (ia.OpenInventoryJournal.WasPressed || ia.SwipeInventoryJournal)
		{
			result = InventoryPaneList.PaneTypes.Journal;
		}
		else if (ia.OpenInventoryTools.WasPressed || ia.SwipeInventoryTools)
		{
			result = InventoryPaneList.PaneTypes.Tools;
		}
		else if (ia.OpenInventoryQuests.WasPressed || ia.SwipeInventoryQuests)
		{
			result = InventoryPaneList.PaneTypes.Quests;
		}
		return result;
	}

	// Token: 0x060011CF RID: 4559 RVA: 0x000536B3 File Offset: 0x000518B3
	public static bool IsInventoryButtonPressed(HeroActions ia)
	{
		return InventoryPaneInput.GetInventoryButtonPressed(ia) != InventoryPaneList.PaneTypes.None;
	}

	// Token: 0x060011D0 RID: 4560 RVA: 0x000536C4 File Offset: 0x000518C4
	private void PressCancel()
	{
		FSMUtility.SendEventToGameObject(base.gameObject, "UI CANCEL", false);
		this.actionCooldown = 0.25f;
		this.isRepeatingDirection = false;
		VibrationManager.PlayVibrationClipOneShot(this.menuCancelVibration, null, false, "", false);
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x00053718 File Offset: 0x00051918
	private void PressDirection(InventoryPaneBase.InputEventType direction)
	{
		if (!this.allowHorizontalSelection && (direction == InventoryPaneBase.InputEventType.Left || direction == InventoryPaneBase.InputEventType.Right))
		{
			return;
		}
		if (!this.allowVerticalSelection && (direction == InventoryPaneBase.InputEventType.Up || direction == InventoryPaneBase.InputEventType.Down))
		{
			return;
		}
		if (this.allowRepeat)
		{
			this.isRepeatingDirection = true;
			this.isRepeatingSubmit = false;
			this.lastPressedDirection = direction;
			this.directionRepeatTimer = 0.25f;
		}
		GameObject gameObject = base.gameObject;
		string eventName;
		switch (direction)
		{
		case InventoryPaneBase.InputEventType.Left:
			eventName = "UI LEFT";
			break;
		case InventoryPaneBase.InputEventType.Right:
			eventName = "UI RIGHT";
			break;
		case InventoryPaneBase.InputEventType.Up:
			eventName = "UI UP";
			break;
		case InventoryPaneBase.InputEventType.Down:
			eventName = "UI DOWN";
			break;
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
		FSMUtility.SendEventToGameObject(gameObject, eventName, false);
		this.pane.SendInputEvent(direction);
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x000537D4 File Offset: 0x000519D4
	private void PressSubmit()
	{
		FSMUtility.SendEventToGameObject(base.gameObject, "UI CONFIRM", false);
		this.wasSubmitPressed = true;
		this.isRepeatingDirection = false;
		VibrationManager.PlayVibrationClipOneShot(this.menuSubmitVibration, null, false, "", false);
		if (this.allowRepeatSubmit)
		{
			this.isRepeatingSubmit = true;
			this.directionRepeatTimer = 0.25f;
			return;
		}
		this.actionCooldown = 0.25f;
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x00053847 File Offset: 0x00051A47
	private void ReleaseSubmit()
	{
		FSMUtility.SendEventToGameObject(base.gameObject, "UI CONFIRM RELEASED", false);
		this.wasSubmitPressed = false;
		this.isRepeatingDirection = false;
		this.isRepeatingSubmit = false;
	}

	// Token: 0x060011D4 RID: 4564 RVA: 0x0005386F File Offset: 0x00051A6F
	public void CancelRepeat()
	{
		this.isRepeatingDirection = false;
	}

	// Token: 0x040010A6 RID: 4262
	private const float INPUT_COOLDOWN = 0.25f;

	// Token: 0x040010A7 RID: 4263
	private const float DIRECTION_REPEAT_TIME = 0.15f;

	// Token: 0x040010A8 RID: 4264
	private const float LIST_SCROLL_SPEED_FAST = 0.03f;

	// Token: 0x040010AB RID: 4267
	[SerializeField]
	private bool allowHorizontalSelection;

	// Token: 0x040010AC RID: 4268
	[SerializeField]
	private bool allowVerticalSelection;

	// Token: 0x040010AD RID: 4269
	[SerializeField]
	private bool allowRepeat;

	// Token: 0x040010AE RID: 4270
	[SerializeField]
	private bool allowRightStickSpeed;

	// Token: 0x040010AF RID: 4271
	[SerializeField]
	private bool allowRepeatSubmit;

	// Token: 0x040010B0 RID: 4272
	[Space]
	[SerializeField]
	private InventoryPaneList.PaneTypes paneControl = InventoryPaneList.PaneTypes.None;

	// Token: 0x040010B1 RID: 4273
	private bool wasSubmitPressed;

	// Token: 0x040010B2 RID: 4274
	private bool wasExtraPressed;

	// Token: 0x040010B3 RID: 4275
	private float actionCooldown;

	// Token: 0x040010B4 RID: 4276
	private float directionRepeatTimer;

	// Token: 0x040010B5 RID: 4277
	private bool isRepeatingDirection;

	// Token: 0x040010B6 RID: 4278
	private InventoryPaneBase.InputEventType lastPressedDirection;

	// Token: 0x040010B7 RID: 4279
	private bool isScrollingFast;

	// Token: 0x040010B8 RID: 4280
	private bool isRepeatingSubmit;

	// Token: 0x040010B9 RID: 4281
	private InputHandler ih;

	// Token: 0x040010BA RID: 4282
	private Platform platform;

	// Token: 0x040010BB RID: 4283
	private InventoryPaneBase pane;

	// Token: 0x040010BC RID: 4284
	private InventoryPaneList paneList;

	// Token: 0x040010BD RID: 4285
	private VibrationDataAsset menuSubmitVibration;

	// Token: 0x040010BE RID: 4286
	private VibrationDataAsset menuCancelVibration;

	// Token: 0x040010BF RID: 4287
	private bool isInInventory;
}
