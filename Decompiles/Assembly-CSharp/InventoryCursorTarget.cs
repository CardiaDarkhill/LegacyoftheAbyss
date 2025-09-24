using System;
using UnityEngine;

// Token: 0x0200067F RID: 1663
public class InventoryCursorTarget : MonoBehaviour, InventoryCursor.ICursorTarget
{
	// Token: 0x170006B7 RID: 1719
	// (get) Token: 0x06003B8B RID: 15243 RVA: 0x00105F14 File Offset: 0x00104114
	public bool ShowCursor
	{
		get
		{
			return this.showCursor;
		}
	}

	// Token: 0x170006B8 RID: 1720
	// (get) Token: 0x06003B8C RID: 15244 RVA: 0x00105F1C File Offset: 0x0010411C
	public Color? CursorColor
	{
		get
		{
			if (!this.overrideCursorColor)
			{
				return null;
			}
			return new Color?(this.cursorColor);
		}
	}

	// Token: 0x04003DC3 RID: 15811
	[SerializeField]
	private bool showCursor = true;

	// Token: 0x04003DC4 RID: 15812
	[SerializeField]
	private bool overrideCursorColor;

	// Token: 0x04003DC5 RID: 15813
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideCursorColor", true, false, false)]
	private Color cursorColor;
}
