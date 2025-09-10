using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TMProOld;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000693 RID: 1683
public class InventoryItemManager : MonoBehaviour
{
	// Token: 0x170006CD RID: 1741
	// (get) Token: 0x06003C15 RID: 15381 RVA: 0x00108897 File Offset: 0x00106A97
	// (set) Token: 0x06003C16 RID: 15382 RVA: 0x0010889F File Offset: 0x00106A9F
	public InventoryItemSelectable CurrentSelected { get; private set; }

	// Token: 0x170006CE RID: 1742
	// (get) Token: 0x06003C17 RID: 15383 RVA: 0x001088A8 File Offset: 0x00106AA8
	// (set) Token: 0x06003C18 RID: 15384 RVA: 0x001088B0 File Offset: 0x00106AB0
	public InventoryItemSelectable NextSelected { get; private set; }

	// Token: 0x170006CF RID: 1743
	// (get) Token: 0x06003C19 RID: 15385 RVA: 0x001088B9 File Offset: 0x00106AB9
	protected virtual IEnumerable<InventoryItemSelectable> DefaultSelectables
	{
		get
		{
			return this.defaultSelectables;
		}
	}

	// Token: 0x170006D0 RID: 1744
	// (get) Token: 0x06003C1A RID: 15386 RVA: 0x001088C1 File Offset: 0x00106AC1
	// (set) Token: 0x06003C1B RID: 15387 RVA: 0x001088C9 File Offset: 0x00106AC9
	public bool IsActionsBlocked { get; set; }

	// Token: 0x06003C1C RID: 15388 RVA: 0x001088D4 File Offset: 0x00106AD4
	protected virtual void OnValidate()
	{
		if (this.old_defaultSelected)
		{
			this.defaultSelectables = new InventoryItemSelectable[]
			{
				this.old_defaultSelected
			};
			this.old_defaultSelected = null;
		}
		if (this.leftMostSelected)
		{
			this.leftMostSelectables = new InventoryItemSelectable[]
			{
				this.leftMostSelected
			};
			this.leftMostSelected = null;
		}
		if (this.rightMostSelected)
		{
			this.rightMostSelectables = new InventoryItemSelectable[]
			{
				this.rightMostSelected
			};
			this.rightMostSelected = null;
		}
	}

	// Token: 0x06003C1D RID: 15389 RVA: 0x0010895C File Offset: 0x00106B5C
	protected virtual void Awake()
	{
		this.OnValidate();
		if (this.cursorPrefab)
		{
			this.cursor = Object.Instantiate<InventoryCursor>(this.cursorPrefab, this.cursorParentOverride ? this.cursorParentOverride : base.transform);
			this.cursor.gameObject.name = "Cursor";
			this.cursor.transform.SetLocalPosition2D(new Vector2(-100f, -100f));
		}
		InventoryPaneBase component = base.GetComponent<InventoryPaneBase>();
		if (component)
		{
			component.OnPrePaneStart += delegate()
			{
				this.InstantScroll();
			};
			component.OnPaneEnd += delegate()
			{
				if (this.isSubmitHeld)
				{
					this.isSubmitHeld = false;
					if (this.CurrentSelected)
					{
						this.CurrentSelected.SubmitReleased();
					}
				}
				if (this.isExtraHeld)
				{
					this.isExtraHeld = false;
					if (this.CurrentSelected)
					{
						this.CurrentSelected.ExtraReleased();
					}
				}
			};
		}
	}

	// Token: 0x06003C1E RID: 15390 RVA: 0x00108A10 File Offset: 0x00106C10
	public bool SetSelected(InventoryItemManager.SelectedActionType selectedAction, bool justDisplay = false)
	{
		InventoryItemSelectable inventoryItemSelectable = this.GetStartSelectable();
		InventoryItemManager.SelectionDirection? direction = null;
		IEnumerable<InventoryItemSelectable> collection = this.DefaultSelectables;
		switch (selectedAction)
		{
		case InventoryItemManager.SelectedActionType.LeftMost:
			direction = new InventoryItemManager.SelectionDirection?(InventoryItemManager.SelectionDirection.Right);
			if (this.TrySelectOrdered(this.GetLeftMostSelectables(), direction, justDisplay))
			{
				return true;
			}
			break;
		case InventoryItemManager.SelectedActionType.RightMost:
			direction = new InventoryItemManager.SelectionDirection?(InventoryItemManager.SelectionDirection.Left);
			if (this.TrySelectOrdered(this.GetRightMostSelectables(), direction, justDisplay))
			{
				return true;
			}
			break;
		case InventoryItemManager.SelectedActionType.Previous:
			if (!inventoryItemSelectable)
			{
				if (!this.CurrentSelected || !this.CurrentSelected.isActiveAndEnabled)
				{
					return this.TrySelectOrdered(collection, null, justDisplay);
				}
				inventoryItemSelectable = this.CurrentSelected;
			}
			break;
		}
		if (!inventoryItemSelectable)
		{
			return this.TrySelectOrdered(collection, direction, justDisplay);
		}
		return this.SetSelected(inventoryItemSelectable, direction, justDisplay) || this.TrySelectOrdered(collection, direction, justDisplay);
	}

	// Token: 0x06003C1F RID: 15391 RVA: 0x00108AE4 File Offset: 0x00106CE4
	private bool TrySelectOrdered(IEnumerable<InventoryItemSelectable> collection, InventoryItemManager.SelectionDirection? direction, bool justDisplay)
	{
		if (collection == null)
		{
			return false;
		}
		foreach (InventoryItemSelectable selectable in from s in collection
		where s != null && s.gameObject.activeSelf
		select s)
		{
			if (this.SetSelected(selectable, direction, justDisplay))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003C20 RID: 15392 RVA: 0x00108B60 File Offset: 0x00106D60
	public bool TryGetFurthestSelectableInDirection(InventoryItemManager.SelectionDirection direction, out InventoryItemSelectable furthestSelectable)
	{
		switch (direction)
		{
		case InventoryItemManager.SelectionDirection.Up:
			using (IEnumerator<InventoryItemSelectable> enumerator = this.GetTopMostSelectables().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					InventoryItemSelectable inventoryItemSelectable = enumerator.Current;
					if (!(inventoryItemSelectable == null) && inventoryItemSelectable.isActiveAndEnabled && !(inventoryItemSelectable.Get(null) == null))
					{
						furthestSelectable = inventoryItemSelectable;
						return true;
					}
				}
				goto IL_19E;
			}
			break;
		case InventoryItemManager.SelectionDirection.Down:
			break;
		case InventoryItemManager.SelectionDirection.Left:
			goto IL_DC;
		case InventoryItemManager.SelectionDirection.Right:
			goto IL_13D;
		default:
			goto IL_19E;
		}
		using (IEnumerator<InventoryItemSelectable> enumerator = this.GetBottomMostSelectables().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				InventoryItemSelectable inventoryItemSelectable2 = enumerator.Current;
				if (!(inventoryItemSelectable2 == null) && inventoryItemSelectable2.isActiveAndEnabled && !(inventoryItemSelectable2.Get(null) == null))
				{
					furthestSelectable = inventoryItemSelectable2;
					return true;
				}
			}
			goto IL_19E;
		}
		IL_DC:
		using (IEnumerator<InventoryItemSelectable> enumerator = this.GetLeftMostSelectables().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				InventoryItemSelectable inventoryItemSelectable3 = enumerator.Current;
				if (!(inventoryItemSelectable3 == null) && inventoryItemSelectable3.isActiveAndEnabled)
				{
					InventoryItemSelectable inventoryItemSelectable4 = inventoryItemSelectable3.Get(null);
					if (!(inventoryItemSelectable4 == null))
					{
						furthestSelectable = inventoryItemSelectable4;
						return true;
					}
				}
			}
			goto IL_19E;
		}
		IL_13D:
		foreach (InventoryItemSelectable inventoryItemSelectable5 in this.GetRightMostSelectables())
		{
			if (!(inventoryItemSelectable5 == null) && inventoryItemSelectable5.isActiveAndEnabled)
			{
				InventoryItemSelectable inventoryItemSelectable6 = inventoryItemSelectable5.Get(null);
				if (!(inventoryItemSelectable6 == null))
				{
					furthestSelectable = inventoryItemSelectable6;
					return true;
				}
			}
		}
		IL_19E:
		furthestSelectable = null;
		return false;
	}

	// Token: 0x06003C21 RID: 15393 RVA: 0x00108D48 File Offset: 0x00106F48
	public void SetSelected(GameObject selectedGameObject, bool justDisplay = false)
	{
		if (this.CurrentSelected && selectedGameObject != this.CurrentSelected.gameObject)
		{
			this.CurrentSelected.Deselect();
			this.CurrentSelected.OnUpdateDisplay -= this.SetDisplay;
		}
		if (selectedGameObject == null)
		{
			return;
		}
		if (justDisplay)
		{
			return;
		}
		if (this.cursor)
		{
			ScrollView componentInParent = selectedGameObject.GetComponentInParent<ScrollView>();
			if (componentInParent)
			{
				Vector3 position = componentInParent.transform.parent.position;
				Bounds viewBounds = componentInParent.ViewBounds;
				Vector3 extents = viewBounds.extents;
				if (extents.x == 0f)
				{
					extents.x = float.MaxValue;
				}
				if (extents.y == 0f)
				{
					extents.y = float.MaxValue;
				}
				viewBounds.extents = extents;
				Vector3 v = position + viewBounds.min;
				Vector3 v2 = position + viewBounds.max;
				this.cursor.SetClampedPos(v, v2);
			}
			else
			{
				this.cursor.ResetClampedPos();
			}
			this.cursor.SetTarget(selectedGameObject ? selectedGameObject.transform : null);
			return;
		}
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Update Cursor");
		if (playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Item").Value = selectedGameObject;
			playMakerFSM.SendEvent("UPDATE CURSOR");
		}
	}

	// Token: 0x06003C22 RID: 15394 RVA: 0x00108EBD File Offset: 0x001070BD
	public virtual void InstantScroll()
	{
	}

	// Token: 0x06003C23 RID: 15395 RVA: 0x00108EC0 File Offset: 0x001070C0
	public bool SetSelected(InventoryItemSelectable selectable, InventoryItemManager.SelectionDirection? direction, bool justDisplay = false)
	{
		InventoryItemSelectable currentSelected = this.CurrentSelected;
		if (selectable)
		{
			InventoryItemSelectable inventoryItemSelectable = selectable.Get(direction);
			if (inventoryItemSelectable == null)
			{
				return false;
			}
			selectable = inventoryItemSelectable;
		}
		if (!selectable)
		{
			return false;
		}
		this.NextSelected = selectable;
		if (currentSelected)
		{
			currentSelected.Deselect();
			currentSelected.OnUpdateDisplay -= this.SetDisplay;
		}
		this.CurrentSelected = selectable;
		this.NextSelected = null;
		selectable.OnUpdateDisplay += this.SetDisplay;
		if (justDisplay)
		{
			this.SetDisplay(selectable);
		}
		else
		{
			selectable.Select(direction);
			this.SetSelected(selectable.gameObject, false);
		}
		if (this.descriptionLayout)
		{
			this.descriptionLayout.ForceUpdateLayoutNoCanvas();
		}
		return true;
	}

	// Token: 0x06003C24 RID: 15396 RVA: 0x00108F80 File Offset: 0x00107180
	public virtual bool MoveSelection(InventoryItemManager.SelectionDirection direction)
	{
		if (this.IsActionsBlocked)
		{
			return true;
		}
		if (this.CurrentSelected == null)
		{
			return false;
		}
		InventoryItemSelectable nextSelectable = this.CurrentSelected.GetNextSelectable(direction);
		if (nextSelectable == null)
		{
			return false;
		}
		if (nextSelectable == this.CurrentSelected)
		{
			return true;
		}
		this.PlayMoveSound();
		return this.SetSelected(nextSelectable, new InventoryItemManager.SelectionDirection?(direction), false);
	}

	// Token: 0x06003C25 RID: 15397 RVA: 0x00108FE4 File Offset: 0x001071E4
	public virtual bool MoveSelectionPage(InventoryItemManager.SelectionDirection direction)
	{
		if (this.IsActionsBlocked)
		{
			return true;
		}
		if (this.CurrentSelected == null)
		{
			return false;
		}
		InventoryItemSelectable nextSelectablePage = this.CurrentSelected.GetNextSelectablePage(this.CurrentSelected, direction);
		if (nextSelectablePage == null)
		{
			return false;
		}
		if (nextSelectablePage == this.CurrentSelected)
		{
			return true;
		}
		this.PlayMoveSound();
		return this.SetSelected(nextSelectablePage, new InventoryItemManager.SelectionDirection?(direction), false);
	}

	// Token: 0x06003C26 RID: 15398 RVA: 0x00109050 File Offset: 0x00107250
	public void PlayMoveSound()
	{
		Audio.InventorySelectionMoveSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
	}

	// Token: 0x06003C27 RID: 15399 RVA: 0x0010907C File Offset: 0x0010727C
	public virtual void SetDisplay(GameObject selectedGameObject)
	{
		if (this.nameText)
		{
			this.nameText.text = string.Empty;
		}
		if (this.descriptionText)
		{
			this.descriptionText.text = string.Empty;
		}
	}

	// Token: 0x06003C28 RID: 15400 RVA: 0x001090B8 File Offset: 0x001072B8
	protected virtual string FormatDisplayName(string displayName)
	{
		return displayName;
	}

	// Token: 0x06003C29 RID: 15401 RVA: 0x001090BB File Offset: 0x001072BB
	protected virtual string FormatDescription(string description)
	{
		return description;
	}

	// Token: 0x06003C2A RID: 15402 RVA: 0x001090C0 File Offset: 0x001072C0
	public virtual void SetDisplay(InventoryItemSelectable selectable)
	{
		this.SetDisplay(selectable.gameObject);
		if (this.nameText)
		{
			this.nameText.text = this.FormatDisplayName(selectable.DisplayName);
		}
		if (this.descriptionText)
		{
			this.descriptionText.text = this.FormatDescription(selectable.Description);
		}
	}

	// Token: 0x06003C2B RID: 15403 RVA: 0x00109121 File Offset: 0x00107321
	public virtual bool SubmitButtonSelected()
	{
		if (this.IsActionsBlocked)
		{
			return false;
		}
		if (this.CurrentSelected && this.CurrentSelected.Submit())
		{
			this.isSubmitHeld = true;
			return true;
		}
		return false;
	}

	// Token: 0x06003C2C RID: 15404 RVA: 0x00109151 File Offset: 0x00107351
	public virtual bool SubmitButtonReleaseSelected()
	{
		if (this.IsActionsBlocked)
		{
			return false;
		}
		if (this.isSubmitHeld)
		{
			this.isSubmitHeld = false;
			if (this.CurrentSelected && this.CurrentSelected.SubmitReleased())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003C2D RID: 15405 RVA: 0x00109189 File Offset: 0x00107389
	public virtual bool CancelButtonSelected()
	{
		return !this.IsActionsBlocked && this.CurrentSelected && this.CurrentSelected.Cancel();
	}

	// Token: 0x06003C2E RID: 15406 RVA: 0x001091AF File Offset: 0x001073AF
	public virtual bool ExtraButtonSelected()
	{
		if (this.IsActionsBlocked)
		{
			return false;
		}
		if (this.CurrentSelected && this.CurrentSelected.Extra())
		{
			this.isExtraHeld = true;
			return true;
		}
		return false;
	}

	// Token: 0x06003C2F RID: 15407 RVA: 0x001091DF File Offset: 0x001073DF
	public virtual bool ExtraButtonReleaseSelected()
	{
		if (this.IsActionsBlocked)
		{
			return false;
		}
		if (this.isExtraHeld)
		{
			this.isExtraHeld = false;
			if (this.CurrentSelected && this.CurrentSelected.ExtraReleased())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003C30 RID: 15408 RVA: 0x00109217 File Offset: 0x00107417
	public virtual bool SuperButtonSelected()
	{
		return !this.IsActionsBlocked && this.CurrentSelected && this.CurrentSelected.Super();
	}

	// Token: 0x06003C31 RID: 15409 RVA: 0x00109240 File Offset: 0x00107440
	public static void PropagateSelectables(InventoryItemSelectableDirectional source, InventoryItemSelectableDirectional target)
	{
		for (int i = 0; i < InventoryItemManager._selectionDirectionLength; i++)
		{
			if (target.Selectables[i] == null)
			{
				target.Selectables[i] = source.Selectables[i];
			}
		}
	}

	// Token: 0x06003C32 RID: 15410 RVA: 0x00109280 File Offset: 0x00107480
	public void SetProxyActive(bool value, InventoryItemManager.SelectedActionType select = InventoryItemManager.SelectedActionType.Default)
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Proxy");
		if (playMakerFSM)
		{
			playMakerFSM.SendEvent(value ? "ACTIVATE" : "PANE RESET");
			playMakerFSM.FsmVariables.FindFsmEnum("Start Selection").Value = select;
		}
	}

	// Token: 0x06003C33 RID: 15411 RVA: 0x001092D6 File Offset: 0x001074D6
	protected virtual InventoryItemSelectable GetStartSelectable()
	{
		return null;
	}

	// Token: 0x06003C34 RID: 15412 RVA: 0x001092D9 File Offset: 0x001074D9
	protected virtual IEnumerable<InventoryItemSelectable> GetRightMostSelectables()
	{
		return this.rightMostSelectables;
	}

	// Token: 0x06003C35 RID: 15413 RVA: 0x001092E1 File Offset: 0x001074E1
	protected virtual IEnumerable<InventoryItemSelectable> GetLeftMostSelectables()
	{
		return this.leftMostSelectables;
	}

	// Token: 0x06003C36 RID: 15414 RVA: 0x001092E9 File Offset: 0x001074E9
	protected virtual IEnumerable<InventoryItemSelectable> GetTopMostSelectables()
	{
		return Array.Empty<InventoryItemSelectable>();
	}

	// Token: 0x06003C37 RID: 15415 RVA: 0x001092F0 File Offset: 0x001074F0
	protected virtual IEnumerable<InventoryItemSelectable> GetBottomMostSelectables()
	{
		return Array.Empty<InventoryItemSelectable>();
	}

	// Token: 0x04003E2C RID: 15916
	[SerializeField]
	[FormerlySerializedAs("defaultSelected")]
	[HideInInspector]
	[Obsolete]
	private InventoryItemSelectable old_defaultSelected;

	// Token: 0x04003E2D RID: 15917
	[SerializeField]
	private InventoryItemSelectable[] defaultSelectables;

	// Token: 0x04003E2E RID: 15918
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private InventoryItemSelectable leftMostSelected;

	// Token: 0x04003E2F RID: 15919
	[SerializeField]
	private InventoryItemSelectable[] leftMostSelectables;

	// Token: 0x04003E30 RID: 15920
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private InventoryItemSelectable rightMostSelected;

	// Token: 0x04003E31 RID: 15921
	[SerializeField]
	private InventoryItemSelectable[] rightMostSelectables;

	// Token: 0x04003E32 RID: 15922
	[SerializeField]
	protected TextMeshPro nameText;

	// Token: 0x04003E33 RID: 15923
	[SerializeField]
	protected TextMeshPro descriptionText;

	// Token: 0x04003E34 RID: 15924
	[SerializeField]
	private LayoutGroup descriptionLayout;

	// Token: 0x04003E35 RID: 15925
	[SerializeField]
	private InventoryCursor cursorPrefab;

	// Token: 0x04003E36 RID: 15926
	protected InventoryCursor cursor;

	// Token: 0x04003E37 RID: 15927
	[SerializeField]
	private Transform cursorParentOverride;

	// Token: 0x04003E38 RID: 15928
	private bool isSubmitHeld;

	// Token: 0x04003E39 RID: 15929
	private bool isExtraHeld;

	// Token: 0x04003E3A RID: 15930
	private static readonly int _selectionDirectionLength = Enum.GetNames(typeof(InventoryItemManager.SelectionDirection)).Length;

	// Token: 0x02001999 RID: 6553
	public enum SelectedActionType
	{
		// Token: 0x0400965A RID: 38490
		Default,
		// Token: 0x0400965B RID: 38491
		LeftMost,
		// Token: 0x0400965C RID: 38492
		RightMost,
		// Token: 0x0400965D RID: 38493
		Previous
	}

	// Token: 0x0200199A RID: 6554
	public enum SelectionDirection
	{
		// Token: 0x0400965F RID: 38495
		Up,
		// Token: 0x04009660 RID: 38496
		Down,
		// Token: 0x04009661 RID: 38497
		Left,
		// Token: 0x04009662 RID: 38498
		Right
	}
}
