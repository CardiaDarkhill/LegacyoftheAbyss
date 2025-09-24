using System;
using UnityEngine;

// Token: 0x020006A0 RID: 1696
public abstract class InventoryItemSelectable : MonoBehaviour, InventoryCursor.ICursorTarget
{
	// Token: 0x140000C8 RID: 200
	// (add) Token: 0x06003C74 RID: 15476 RVA: 0x00109DD8 File Offset: 0x00107FD8
	// (remove) Token: 0x06003C75 RID: 15477 RVA: 0x00109E10 File Offset: 0x00108010
	public event Action<InventoryItemSelectable> OnSelected;

	// Token: 0x140000C9 RID: 201
	// (add) Token: 0x06003C76 RID: 15478 RVA: 0x00109E48 File Offset: 0x00108048
	// (remove) Token: 0x06003C77 RID: 15479 RVA: 0x00109E80 File Offset: 0x00108080
	public event Action<InventoryItemSelectable, InventoryItemManager.SelectionDirection?> OnSelectedDirection;

	// Token: 0x140000CA RID: 202
	// (add) Token: 0x06003C78 RID: 15480 RVA: 0x00109EB8 File Offset: 0x001080B8
	// (remove) Token: 0x06003C79 RID: 15481 RVA: 0x00109EF0 File Offset: 0x001080F0
	public event Action<InventoryItemSelectable> OnDeselected;

	// Token: 0x140000CB RID: 203
	// (add) Token: 0x06003C7A RID: 15482 RVA: 0x00109F28 File Offset: 0x00108128
	// (remove) Token: 0x06003C7B RID: 15483 RVA: 0x00109F60 File Offset: 0x00108160
	public event Action<InventoryItemSelectable> OnUpdateDisplay;

	// Token: 0x170006DC RID: 1756
	// (get) Token: 0x06003C7C RID: 15484 RVA: 0x00109F95 File Offset: 0x00108195
	public virtual string DisplayName
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170006DD RID: 1757
	// (get) Token: 0x06003C7D RID: 15485 RVA: 0x00109F9C File Offset: 0x0010819C
	public virtual string Description
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x170006DE RID: 1758
	// (get) Token: 0x06003C7E RID: 15486 RVA: 0x00109FA4 File Offset: 0x001081A4
	public virtual Color? CursorColor
	{
		get
		{
			return null;
		}
	}

	// Token: 0x170006DF RID: 1759
	// (get) Token: 0x06003C7F RID: 15487 RVA: 0x00109FBA File Offset: 0x001081BA
	// (set) Token: 0x06003C80 RID: 15488 RVA: 0x00109FC2 File Offset: 0x001081C2
	public Vector2 CursorGlowScale
	{
		get
		{
			return this.cursorGlowScale;
		}
		set
		{
			this.cursorGlowScale = value;
		}
	}

	// Token: 0x170006E0 RID: 1760
	// (get) Token: 0x06003C81 RID: 15489 RVA: 0x00109FCB File Offset: 0x001081CB
	public Vector2 NavigationOffset
	{
		get
		{
			return this.navigationOffset;
		}
	}

	// Token: 0x170006E1 RID: 1761
	// (get) Token: 0x06003C82 RID: 15490 RVA: 0x00109FD3 File Offset: 0x001081D3
	public virtual bool ShowCursor
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003C83 RID: 15491 RVA: 0x00109FD6 File Offset: 0x001081D6
	public virtual InventoryItemSelectable Get(InventoryItemManager.SelectionDirection? direction)
	{
		return this;
	}

	// Token: 0x06003C84 RID: 15492
	public abstract InventoryItemSelectable GetNextSelectable(InventoryItemManager.SelectionDirection direction);

	// Token: 0x06003C85 RID: 15493 RVA: 0x00109FDC File Offset: 0x001081DC
	public virtual InventoryItemSelectable GetNextSelectablePage(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction)
	{
		if (!base.transform.parent)
		{
			return null;
		}
		InventoryItemSelectable componentInParent = base.transform.parent.GetComponentInParent<InventoryItemSelectable>();
		if (!componentInParent)
		{
			return null;
		}
		return componentInParent.GetNextSelectablePage(currentSelected, direction);
	}

	// Token: 0x06003C86 RID: 15494 RVA: 0x0010A020 File Offset: 0x00108220
	public virtual void Select(InventoryItemManager.SelectionDirection? direction)
	{
		this.UpdateDisplay();
		if (this.OnSelected != null)
		{
			this.OnSelected(this);
		}
		if (this.OnSelectedDirection != null)
		{
			this.OnSelectedDirection(this, direction);
		}
	}

	// Token: 0x06003C87 RID: 15495 RVA: 0x0010A051 File Offset: 0x00108251
	public virtual void Deselect()
	{
		if (this.OnDeselected != null)
		{
			this.OnDeselected(this);
		}
	}

	// Token: 0x06003C88 RID: 15496 RVA: 0x0010A067 File Offset: 0x00108267
	public virtual bool Submit()
	{
		return false;
	}

	// Token: 0x06003C89 RID: 15497 RVA: 0x0010A06A File Offset: 0x0010826A
	public virtual bool SubmitReleased()
	{
		return false;
	}

	// Token: 0x06003C8A RID: 15498 RVA: 0x0010A06D File Offset: 0x0010826D
	public virtual bool Cancel()
	{
		return false;
	}

	// Token: 0x06003C8B RID: 15499 RVA: 0x0010A070 File Offset: 0x00108270
	public virtual bool Extra()
	{
		return false;
	}

	// Token: 0x06003C8C RID: 15500 RVA: 0x0010A073 File Offset: 0x00108273
	public virtual bool ExtraReleased()
	{
		return false;
	}

	// Token: 0x06003C8D RID: 15501 RVA: 0x0010A076 File Offset: 0x00108276
	public virtual bool Super()
	{
		return false;
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x0010A079 File Offset: 0x00108279
	protected virtual void UpdateDisplay()
	{
		if (this.OnUpdateDisplay != null)
		{
			this.OnUpdateDisplay(this);
		}
	}

	// Token: 0x04003E5C RID: 15964
	[SerializeField]
	private Vector2 cursorGlowScale = Vector2.one;

	// Token: 0x04003E5D RID: 15965
	[SerializeField]
	private Vector2 navigationOffset;
}
