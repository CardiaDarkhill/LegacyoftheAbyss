using System;
using UnityEngine;

// Token: 0x020006C7 RID: 1735
public class LinkRendererState : MonoBehaviour
{
	// Token: 0x06003EBF RID: 16063 RVA: 0x0011427D File Offset: 0x0011247D
	private void Start()
	{
		this.UpdateLink();
	}

	// Token: 0x06003EC0 RID: 16064 RVA: 0x00114285 File Offset: 0x00112485
	private void LateUpdate()
	{
		this.UpdateLink();
	}

	// Token: 0x06003EC1 RID: 16065 RVA: 0x00114290 File Offset: 0x00112490
	public void UpdateLink()
	{
		if (this.parent && this.child && this.child.enabled != this.parent.enabled)
		{
			this.child.enabled = this.parent.enabled;
		}
	}

	// Token: 0x0400405B RID: 16475
	public Renderer parent;

	// Token: 0x0400405C RID: 16476
	public Renderer child;
}
