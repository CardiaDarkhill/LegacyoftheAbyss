using System;
using UnityEngine;

// Token: 0x020006AE RID: 1710
public abstract class InventoryPaneBase : MonoBehaviour
{
	// Token: 0x140000CD RID: 205
	// (add) Token: 0x06003D7D RID: 15741 RVA: 0x0010E738 File Offset: 0x0010C938
	// (remove) Token: 0x06003D7E RID: 15742 RVA: 0x0010E770 File Offset: 0x0010C970
	public event Action OnPrePaneStart;

	// Token: 0x140000CE RID: 206
	// (add) Token: 0x06003D7F RID: 15743 RVA: 0x0010E7A8 File Offset: 0x0010C9A8
	// (remove) Token: 0x06003D80 RID: 15744 RVA: 0x0010E7E0 File Offset: 0x0010C9E0
	public event Action OnPaneStart;

	// Token: 0x140000CF RID: 207
	// (add) Token: 0x06003D81 RID: 15745 RVA: 0x0010E818 File Offset: 0x0010CA18
	// (remove) Token: 0x06003D82 RID: 15746 RVA: 0x0010E850 File Offset: 0x0010CA50
	public event Action OnPostPaneStart;

	// Token: 0x140000D0 RID: 208
	// (add) Token: 0x06003D83 RID: 15747 RVA: 0x0010E888 File Offset: 0x0010CA88
	// (remove) Token: 0x06003D84 RID: 15748 RVA: 0x0010E8C0 File Offset: 0x0010CAC0
	public event Action OnPaneEnd;

	// Token: 0x140000D1 RID: 209
	// (add) Token: 0x06003D85 RID: 15749 RVA: 0x0010E8F8 File Offset: 0x0010CAF8
	// (remove) Token: 0x06003D86 RID: 15750 RVA: 0x0010E930 File Offset: 0x0010CB30
	public event Action OnPrePaneEnd;

	// Token: 0x140000D2 RID: 210
	// (add) Token: 0x06003D87 RID: 15751 RVA: 0x0010E968 File Offset: 0x0010CB68
	// (remove) Token: 0x06003D88 RID: 15752 RVA: 0x0010E9A0 File Offset: 0x0010CBA0
	public event Action OnInputLeft;

	// Token: 0x140000D3 RID: 211
	// (add) Token: 0x06003D89 RID: 15753 RVA: 0x0010E9D8 File Offset: 0x0010CBD8
	// (remove) Token: 0x06003D8A RID: 15754 RVA: 0x0010EA10 File Offset: 0x0010CC10
	public event Action OnInputRight;

	// Token: 0x140000D4 RID: 212
	// (add) Token: 0x06003D8B RID: 15755 RVA: 0x0010EA48 File Offset: 0x0010CC48
	// (remove) Token: 0x06003D8C RID: 15756 RVA: 0x0010EA80 File Offset: 0x0010CC80
	public event Action OnInputUp;

	// Token: 0x140000D5 RID: 213
	// (add) Token: 0x06003D8D RID: 15757 RVA: 0x0010EAB8 File Offset: 0x0010CCB8
	// (remove) Token: 0x06003D8E RID: 15758 RVA: 0x0010EAF0 File Offset: 0x0010CCF0
	public event Action OnInputDown;

	// Token: 0x1700070F RID: 1807
	// (get) Token: 0x06003D8F RID: 15759 RVA: 0x0010EB25 File Offset: 0x0010CD25
	// (set) Token: 0x06003D90 RID: 15760 RVA: 0x0010EB2D File Offset: 0x0010CD2D
	public bool IsPaneActive { get; private set; }

	// Token: 0x06003D91 RID: 15761 RVA: 0x0010EB38 File Offset: 0x0010CD38
	public virtual void PaneStart()
	{
		this.IsPaneActive = true;
		base.gameObject.SetActive(true);
		if (this.OnPrePaneStart != null)
		{
			this.OnPrePaneStart();
		}
		if (this.OnPaneStart != null)
		{
			this.OnPaneStart();
		}
		if (this.OnPostPaneStart != null)
		{
			this.OnPostPaneStart();
		}
	}

	// Token: 0x06003D92 RID: 15762 RVA: 0x0010EB91 File Offset: 0x0010CD91
	public virtual void PaneEnd()
	{
		this.IsPaneActive = false;
		if (this.OnPrePaneEnd != null)
		{
			this.OnPrePaneEnd();
		}
		if (this.OnPaneEnd != null)
		{
			this.OnPaneEnd();
		}
	}

	// Token: 0x06003D93 RID: 15763 RVA: 0x0010EBC0 File Offset: 0x0010CDC0
	public void SendInputEvent(InventoryPaneBase.InputEventType eventType)
	{
		Action action = null;
		switch (eventType)
		{
		case InventoryPaneBase.InputEventType.Left:
			action = this.OnInputLeft;
			break;
		case InventoryPaneBase.InputEventType.Right:
			action = this.OnInputRight;
			break;
		case InventoryPaneBase.InputEventType.Up:
			action = this.OnInputUp;
			break;
		case InventoryPaneBase.InputEventType.Down:
			action = this.OnInputDown;
			break;
		}
		if (action != null)
		{
			action();
		}
	}

	// Token: 0x020019B3 RID: 6579
	public enum InputEventType
	{
		// Token: 0x040096C4 RID: 38596
		Left,
		// Token: 0x040096C5 RID: 38597
		Right,
		// Token: 0x040096C6 RID: 38598
		Up,
		// Token: 0x040096C7 RID: 38599
		Down
	}
}
