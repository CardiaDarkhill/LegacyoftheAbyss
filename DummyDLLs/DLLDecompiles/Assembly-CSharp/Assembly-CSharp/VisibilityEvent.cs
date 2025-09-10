using System;
using UnityEngine;

// Token: 0x02000373 RID: 883
public abstract class VisibilityEvent : MonoBehaviour
{
	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06001E40 RID: 7744 RVA: 0x0008B8D8 File Offset: 0x00089AD8
	// (remove) Token: 0x06001E41 RID: 7745 RVA: 0x0008B910 File Offset: 0x00089B10
	public event VisibilityEvent.VisibilityChanged OnVisibilityChanged;

	// Token: 0x170002FE RID: 766
	// (get) Token: 0x06001E42 RID: 7746 RVA: 0x0008B945 File Offset: 0x00089B45
	// (set) Token: 0x06001E43 RID: 7747 RVA: 0x0008B94D File Offset: 0x00089B4D
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
		protected set
		{
			if (this.isVisible != value)
			{
				this.isVisible = value;
				VisibilityEvent.VisibilityChanged onVisibilityChanged = this.OnVisibilityChanged;
				if (onVisibilityChanged == null)
				{
					return;
				}
				onVisibilityChanged(value);
			}
		}
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x0008B970 File Offset: 0x00089B70
	protected virtual void OnDestroy()
	{
		if (this.parent)
		{
			this.parent.UnregisterObject(this);
			this.parent = null;
		}
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x0008B992 File Offset: 0x00089B92
	protected void FindParent()
	{
		if (this.parentMode == VisibilityEvent.ParentMode.Auto)
		{
			this.parent = base.GetComponentInParent<VisibilityGroup>();
		}
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x0008B9AC File Offset: 0x00089BAC
	public void SetParent(VisibilityGroup visibilityGroup)
	{
		if (this.parent == visibilityGroup)
		{
			return;
		}
		if (this.parent != null)
		{
			this.parent.UnregisterObject(this);
		}
		this.parent = visibilityGroup;
		if (visibilityGroup != null)
		{
			this.parentMode = VisibilityEvent.ParentMode.Manual;
			this.parent.UnsafeRegisterObject(this);
		}
	}

	// Token: 0x04001D4E RID: 7502
	[SerializeField]
	private VisibilityEvent.ParentMode parentMode;

	// Token: 0x04001D50 RID: 7504
	private bool isVisible;

	// Token: 0x04001D51 RID: 7505
	private VisibilityGroup parent;

	// Token: 0x02001621 RID: 5665
	// (Invoke) Token: 0x06008909 RID: 35081
	public delegate void VisibilityChanged(bool visible);

	// Token: 0x02001622 RID: 5666
	[Serializable]
	private enum ParentMode
	{
		// Token: 0x040089CA RID: 35274
		Manual,
		// Token: 0x040089CB RID: 35275
		Auto
	}
}
