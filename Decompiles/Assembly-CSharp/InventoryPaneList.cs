using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using HutongGames.PlayMaker;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;

// Token: 0x020006B1 RID: 1713
public class InventoryPaneList : MonoBehaviour
{
	// Token: 0x140000D6 RID: 214
	// (add) Token: 0x06003DA3 RID: 15779 RVA: 0x0010EF18 File Offset: 0x0010D118
	// (remove) Token: 0x06003DA4 RID: 15780 RVA: 0x0010EF50 File Offset: 0x0010D150
	public event Action OpeningInventory;

	// Token: 0x140000D7 RID: 215
	// (add) Token: 0x06003DA5 RID: 15781 RVA: 0x0010EF88 File Offset: 0x0010D188
	// (remove) Token: 0x06003DA6 RID: 15782 RVA: 0x0010EFC0 File Offset: 0x0010D1C0
	public event Action ClosingInventory;

	// Token: 0x140000D8 RID: 216
	// (add) Token: 0x06003DA7 RID: 15783 RVA: 0x0010EFF8 File Offset: 0x0010D1F8
	// (remove) Token: 0x06003DA8 RID: 15784 RVA: 0x0010F030 File Offset: 0x0010D230
	public event Action<int> MovedPaneIndex;

	// Token: 0x17000710 RID: 1808
	// (get) Token: 0x06003DA9 RID: 15785 RVA: 0x0010F065 File Offset: 0x0010D265
	public static string NextPaneOpen
	{
		get
		{
			if (InventoryPaneList._instance.nextPaneOpenTimeLeft <= 0f)
			{
				return string.Empty;
			}
			return InventoryPaneList._instance.nextPaneOpen ?? string.Empty;
		}
	}

	// Token: 0x17000711 RID: 1809
	// (get) Token: 0x06003DAA RID: 15786 RVA: 0x0010F091 File Offset: 0x0010D291
	// (set) Token: 0x06003DAB RID: 15787 RVA: 0x0010F0A8 File Offset: 0x0010D2A8
	public bool CanSwitchPanes
	{
		get
		{
			return this.allowSwapping == null || this.allowSwapping.Value;
		}
		set
		{
			if (this.allowSwapping == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if (playMakerFSM)
				{
					this.allowSwapping = playMakerFSM.FsmVariables.FindFsmBool("Allow Pane Swapping");
				}
			}
			if (this.allowSwapping != null && this.UnlockedPaneCount > 1 && this.allowSwapping.Value != value)
			{
				this.allowSwapping.Value = value;
				this.paneArrows.FadeTo(value ? 1f : 0f, this.arrowFadeTime, null, true, null);
			}
		}
	}

	// Token: 0x17000712 RID: 1810
	// (get) Token: 0x06003DAC RID: 15788 RVA: 0x0010F13B File Offset: 0x0010D33B
	// (set) Token: 0x06003DAD RID: 15789 RVA: 0x0010F154 File Offset: 0x0010D354
	public bool CloseBlocked
	{
		get
		{
			return this.doNotClose == null || this.doNotClose.Value;
		}
		set
		{
			if (this.doNotClose == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if (playMakerFSM)
				{
					this.doNotClose = playMakerFSM.FsmVariables.FindFsmBool("Do Not Close");
				}
			}
			if (this.doNotClose != null && this.doNotClose.Value != value)
			{
				this.doNotClose.Value = value;
			}
		}
	}

	// Token: 0x17000713 RID: 1811
	// (get) Token: 0x06003DAE RID: 15790 RVA: 0x0010F1BA File Offset: 0x0010D3BA
	// (set) Token: 0x06003DAF RID: 15791 RVA: 0x0010F1D4 File Offset: 0x0010D3D4
	public bool InSubMenu
	{
		get
		{
			return this.inSubMenu == null || this.inSubMenu.Value;
		}
		set
		{
			if (this.inSubMenu == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if (playMakerFSM)
				{
					this.inSubMenu = playMakerFSM.FsmVariables.FindFsmBool("In Sub Menu");
				}
			}
			if (this.inSubMenu != null && this.inSubMenu.Value != value)
			{
				this.inSubMenu.Value = value;
			}
		}
	}

	// Token: 0x17000714 RID: 1812
	// (get) Token: 0x06003DB0 RID: 15792 RVA: 0x0010F23A File Offset: 0x0010D43A
	// (set) Token: 0x06003DB1 RID: 15793 RVA: 0x0010F254 File Offset: 0x0010D454
	public bool IsPaneMoveCustom
	{
		get
		{
			return this.isPaneMoveCustom == null || this.isPaneMoveCustom.Value;
		}
		set
		{
			if (this.isPaneMoveCustom == null)
			{
				PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Control");
				if (playMakerFSM)
				{
					this.isPaneMoveCustom = playMakerFSM.FsmVariables.FindFsmBool("Pane Move Custom");
				}
			}
			if (this.isPaneMoveCustom != null && this.isPaneMoveCustom.Value != value)
			{
				this.isPaneMoveCustom.Value = value;
			}
		}
	}

	// Token: 0x17000715 RID: 1813
	// (get) Token: 0x06003DB2 RID: 15794 RVA: 0x0010F2BA File Offset: 0x0010D4BA
	public int UnlockedPaneCount
	{
		get
		{
			return this.panes.Where((InventoryPane t, int i) => this.IsPaneAvailable(t, (InventoryPaneList.PaneTypes)i)).Count<InventoryPane>();
		}
	}

	// Token: 0x17000716 RID: 1814
	// (get) Token: 0x06003DB3 RID: 15795 RVA: 0x0010F2D8 File Offset: 0x0010D4D8
	public int TotalPaneCount
	{
		get
		{
			return this.panes.Length;
		}
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x0010F2E2 File Offset: 0x0010D4E2
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<InventoryPane>(ref this.panes, typeof(InventoryPaneList.PaneTypes));
	}

	// Token: 0x06003DB5 RID: 15797 RVA: 0x0010F2F9 File Offset: 0x0010D4F9
	private void Awake()
	{
		this.OnValidate();
		InventoryPaneList._instance = this;
	}

	// Token: 0x06003DB6 RID: 15798 RVA: 0x0010F307 File Offset: 0x0010D507
	private void OnDestroy()
	{
		if (InventoryPaneList._instance == this)
		{
			InventoryPaneList._instance = null;
		}
	}

	// Token: 0x06003DB7 RID: 15799 RVA: 0x0010F31C File Offset: 0x0010D51C
	private void Start()
	{
		if (this.paneListDisplay)
		{
			this.paneListDisplay.PreInstantiate(this.panes.Length);
		}
		foreach (InventoryPane inventoryPane in this.panes)
		{
			if (inventoryPane)
			{
				GameObject gameObject = inventoryPane.gameObject;
				gameObject.SetActive(true);
				NestedFadeGroupBase component = gameObject.GetComponent<NestedFadeGroupBase>();
				if (component)
				{
					component.AlphaSelf = 0f;
				}
				else
				{
					Debug.LogErrorFormat(gameObject, "Inventory pane {0} did not have fade group", new object[]
					{
						gameObject.name
					});
				}
			}
		}
		this.panesDeactivated = false;
	}

	// Token: 0x06003DB8 RID: 15800 RVA: 0x0010F3B8 File Offset: 0x0010D5B8
	private void Update()
	{
		if (!this.panesDeactivated)
		{
			InventoryPane[] array = this.panes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(false);
			}
			this.panesDeactivated = true;
		}
		if (this.nextPaneOpenTimeLeft > 0f && GameManager.instance.GameState == GameState.PLAYING && InteractManager.BlockingInteractable == null && ManagerSingleton<InputHandler>.Instance.acceptingInput && !PlayerData.instance.disablePause && HeroController.instance.acceptingInput)
		{
			this.nextPaneOpenTimeLeft -= Time.deltaTime;
		}
	}

	// Token: 0x06003DB9 RID: 15801 RVA: 0x0010F454 File Offset: 0x0010D654
	private bool IsPaneAvailable(InventoryPane pane, InventoryPaneList.PaneTypes paneType)
	{
		return pane.IsAvailable && !CheatManager.IsInventoryPaneHidden(paneType);
	}

	// Token: 0x06003DBA RID: 15802 RVA: 0x0010F469 File Offset: 0x0010D669
	public void OnOpeningInventory()
	{
		this.paneArrows.AlphaSelf = ((this.UnlockedPaneCount > 1) ? 1f : 0f);
		Action openingInventory = this.OpeningInventory;
		if (openingInventory == null)
		{
			return;
		}
		openingInventory();
	}

	// Token: 0x06003DBB RID: 15803 RVA: 0x0010F49B File Offset: 0x0010D69B
	public void OnClosingInventory()
	{
		Action closingInventory = this.ClosingInventory;
		if (closingInventory == null)
		{
			return;
		}
		closingInventory();
	}

	// Token: 0x06003DBC RID: 15804 RVA: 0x0010F4B0 File Offset: 0x0010D6B0
	public InventoryPane SetCurrentPane(int index, InventoryPane currentPane)
	{
		if (currentPane)
		{
			currentPane.PaneEnd();
		}
		this.nextPaneOpenTimeLeft = 0f;
		InventoryPane inventoryPane = this.panes[index];
		inventoryPane = (this.IsPaneAvailable(inventoryPane, (InventoryPaneList.PaneTypes)index) ? inventoryPane : this.GetNextPane(this.GetPaneIndex(inventoryPane), 1, this.panes.Length));
		return this.BeginPane(inventoryPane, 0);
	}

	// Token: 0x06003DBD RID: 15805 RVA: 0x0010F50C File Offset: 0x0010D70C
	public int GetPaneIndex(string paneName)
	{
		if (!string.IsNullOrEmpty(paneName))
		{
			for (int i = 0; i < this.panes.Length; i++)
			{
				if (this.panes[i].name == paneName)
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06003DBE RID: 15806 RVA: 0x0010F54C File Offset: 0x0010D74C
	public InventoryPane SetNextPane(int direction, InventoryPane currentPane)
	{
		currentPane.PaneEnd();
		InventoryPane inventoryPane = this.GetNextPane(this.GetPaneIndex(currentPane), direction, this.panes.Length);
		if (inventoryPane == null)
		{
			inventoryPane = currentPane;
		}
		return this.BeginPane(inventoryPane, (int)Mathf.Sign((float)direction));
	}

	// Token: 0x06003DBF RID: 15807 RVA: 0x0010F590 File Offset: 0x0010D790
	public InventoryPane BeginPane(InventoryPane pane, int cycleDirection)
	{
		InventoryPane inventoryPane = pane.Get();
		InventoryPane inventoryPane2 = inventoryPane.RootPane;
		if (!inventoryPane2)
		{
			inventoryPane2 = pane;
		}
		this.UpdateDisplay(inventoryPane, inventoryPane2, cycleDirection);
		inventoryPane.PaneStart();
		if (cycleDirection != 0)
		{
			this.paneCycleSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		Action<int> movedPaneIndex = this.MovedPaneIndex;
		if (movedPaneIndex != null)
		{
			movedPaneIndex(this.GetPaneIndex(pane));
		}
		return inventoryPane;
	}

	// Token: 0x06003DC0 RID: 15808 RVA: 0x0010F600 File Offset: 0x0010D800
	private InventoryPane GetNextPane(int index, int direction, int recursionsLeft)
	{
		if (recursionsLeft == 0)
		{
			return null;
		}
		index += direction;
		if (index >= this.panes.Length)
		{
			index = 0;
		}
		else if (index < 0)
		{
			index = this.panes.Length - 1;
		}
		InventoryPane inventoryPane = this.panes[index];
		if (!this.IsPaneAvailable(inventoryPane, (InventoryPaneList.PaneTypes)index))
		{
			return this.GetNextPane(index, direction, recursionsLeft - 1);
		}
		return inventoryPane;
	}

	// Token: 0x06003DC1 RID: 15809 RVA: 0x0010F657 File Offset: 0x0010D857
	public int GetPaneIndex(InventoryPane pane)
	{
		if (pane.RootPane)
		{
			pane = pane.RootPane;
		}
		return Array.IndexOf<InventoryPane>(this.panes, pane);
	}

	// Token: 0x06003DC2 RID: 15810 RVA: 0x0010F67A File Offset: 0x0010D87A
	public InventoryPane GetPane(InventoryPaneList.PaneTypes paneTypes)
	{
		return this.GetPane((int)paneTypes);
	}

	// Token: 0x06003DC3 RID: 15811 RVA: 0x0010F683 File Offset: 0x0010D883
	public InventoryPane GetPane(int index)
	{
		if (index < 0 || index >= this.panes.Length)
		{
			return null;
		}
		return this.panes[index];
	}

	// Token: 0x06003DC4 RID: 15812 RVA: 0x0010F6A0 File Offset: 0x0010D8A0
	private void UpdateDisplay(InventoryPane displayPane, InventoryPane rootPane, int cycleDirection)
	{
		if (this.currentPaneText)
		{
			this.currentPaneText.text = displayPane.DisplayName;
		}
		if (this.paneListDisplay)
		{
			List<InventoryPane> list = this.panes.Where((InventoryPane t, int i) => this.IsPaneAvailable(t, (InventoryPaneList.PaneTypes)i)).ToList<InventoryPane>();
			this.paneListDisplay.UpdateDisplay(list.IndexOf(rootPane), list, cycleDirection);
		}
	}

	// Token: 0x06003DC5 RID: 15813 RVA: 0x0010F709 File Offset: 0x0010D909
	public static void SetNextOpen(string paneName)
	{
		if (!InventoryPaneList._instance)
		{
			return;
		}
		InventoryPaneList._instance.nextPaneOpen = paneName;
		InventoryPaneList._instance.nextPaneOpenTimeLeft = 5f;
	}

	// Token: 0x04003F53 RID: 16211
	[SerializeField]
	[ArrayForEnum(typeof(InventoryPaneList.PaneTypes))]
	private InventoryPane[] panes;

	// Token: 0x04003F54 RID: 16212
	[SerializeField]
	private TextMeshPro currentPaneText;

	// Token: 0x04003F55 RID: 16213
	[SerializeField]
	private InventoryPaneListDisplay paneListDisplay;

	// Token: 0x04003F56 RID: 16214
	[SerializeField]
	private NestedFadeGroupBase paneArrows;

	// Token: 0x04003F57 RID: 16215
	[Space]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x04003F58 RID: 16216
	[SerializeField]
	private AudioEvent paneCycleSound;

	// Token: 0x04003F59 RID: 16217
	private float arrowFadeTime = 0.2f;

	// Token: 0x04003F5A RID: 16218
	private FsmBool allowSwapping;

	// Token: 0x04003F5B RID: 16219
	private FsmBool doNotClose;

	// Token: 0x04003F5C RID: 16220
	private FsmBool inSubMenu;

	// Token: 0x04003F5D RID: 16221
	private FsmBool isPaneMoveCustom;

	// Token: 0x04003F5E RID: 16222
	private float nextPaneOpenTimeLeft;

	// Token: 0x04003F5F RID: 16223
	private string nextPaneOpen;

	// Token: 0x04003F60 RID: 16224
	private bool panesDeactivated;

	// Token: 0x04003F61 RID: 16225
	private static InventoryPaneList _instance;

	// Token: 0x020019B6 RID: 6582
	public enum PaneTypes
	{
		// Token: 0x040096DD RID: 38621
		None = -1,
		// Token: 0x040096DE RID: 38622
		Inv,
		// Token: 0x040096DF RID: 38623
		Tools,
		// Token: 0x040096E0 RID: 38624
		Quests,
		// Token: 0x040096E1 RID: 38625
		Journal,
		// Token: 0x040096E2 RID: 38626
		Map
	}
}
