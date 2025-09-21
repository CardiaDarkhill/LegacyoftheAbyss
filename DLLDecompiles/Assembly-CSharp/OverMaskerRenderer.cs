using System;
using UnityEngine;

// Token: 0x0200064F RID: 1615
[DisallowMultipleComponent]
public sealed class OverMaskerRenderer : OverMaskerBase
{
	// Token: 0x060039C8 RID: 14792 RVA: 0x000FD657 File Offset: 0x000FB857
	private void OnValidate()
	{
		if (this.renderer == null)
		{
			this.renderer = base.GetComponent<Renderer>();
		}
	}

	// Token: 0x060039C9 RID: 14793 RVA: 0x000FD673 File Offset: 0x000FB873
	protected override void ApplySettings(int sortingLayer, short order)
	{
		if (this.renderer != null)
		{
			this.renderer.sortingLayerID = sortingLayer;
			this.renderer.sortingOrder = (int)order;
		}
	}

	// Token: 0x04003C74 RID: 15476
	[SerializeField]
	private Renderer renderer;
}
