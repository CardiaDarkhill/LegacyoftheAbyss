using System;
using UnityEngine;

// Token: 0x02000374 RID: 884
public sealed class VisibilityObject : VisibilityEvent
{
	// Token: 0x06001E48 RID: 7752 RVA: 0x0008BA0D File Offset: 0x00089C0D
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x06001E49 RID: 7753 RVA: 0x0008BA15 File Offset: 0x00089C15
	private void OnValidate()
	{
		if (!this.renderer)
		{
			this.renderer = base.GetComponent<Renderer>();
		}
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x0008BA30 File Offset: 0x00089C30
	private void Init()
	{
		if (this.init)
		{
			return;
		}
		this.init = true;
		base.FindParent();
		if (!this.hasRenderer)
		{
			if (!this.renderer)
			{
				this.renderer = base.GetComponent<Renderer>();
				if (!this.renderer)
				{
					return;
				}
			}
			base.IsVisible = this.renderer.isVisible;
		}
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x0008BA93 File Offset: 0x00089C93
	public void SetRenderer(Renderer renderer)
	{
		if (renderer == null)
		{
			return;
		}
		if (renderer.gameObject != base.gameObject)
		{
			return;
		}
		this.renderer = renderer;
		this.hasRenderer = true;
		base.IsVisible = renderer.isVisible;
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x0008BACD File Offset: 0x00089CCD
	private void OnBecameVisible()
	{
		base.IsVisible = true;
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x0008BAD6 File Offset: 0x00089CD6
	private void OnBecameInvisible()
	{
		base.IsVisible = false;
	}

	// Token: 0x04001D52 RID: 7506
	[SerializeField]
	private Renderer renderer;

	// Token: 0x04001D53 RID: 7507
	private bool init;

	// Token: 0x04001D54 RID: 7508
	private bool hasRenderer;

	// Token: 0x04001D55 RID: 7509
	private VisibilityGroup group;
}
