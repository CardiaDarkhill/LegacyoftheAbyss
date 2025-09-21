using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000219 RID: 537
public sealed class BlackThreadEffectRendererGroup : BlackThreadedEffect
{
	// Token: 0x060013E3 RID: 5091 RVA: 0x0005A39C File Offset: 0x0005859C
	[ContextMenu("Find Renderers In Children")]
	private void FindRenderersInChildren()
	{
		this.renderers.RemoveAll((Renderer o) => o == null);
		this.renderers = this.renderers.Union(base.GetComponentsInChildren<Renderer>(true)).ToList<Renderer>();
	}

	// Token: 0x060013E4 RID: 5092 RVA: 0x0005A3F1 File Offset: 0x000585F1
	protected override void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.renderers.RemoveAll((Renderer o) => o == null);
		}
	}

	// Token: 0x060013E5 RID: 5093 RVA: 0x0005A428 File Offset: 0x00058628
	protected override void Initialize()
	{
		this.renderers.RemoveAll((Renderer o) => o == null);
		if (this.autoFindRenderers)
		{
			this.renderers = this.renderers.Union(base.GetComponentsInChildren<Renderer>(true)).ToList<Renderer>();
		}
	}

	// Token: 0x060013E6 RID: 5094 RVA: 0x0005A488 File Offset: 0x00058688
	protected override void DoSetBlackThreadAmount(float amount)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			Renderer renderer = this.renderers[i];
			renderer.GetPropertyBlock(this.block);
			this.block.SetFloat(BlackThreadedEffect.BLACK_THREAD_AMOUNT, amount);
			renderer.SetPropertyBlock(this.block);
		}
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0005A4E0 File Offset: 0x000586E0
	protected override void EnableKeyword()
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			this.renderers[i].material.EnableKeyword("BLACKTHREAD");
		}
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x0005A520 File Offset: 0x00058720
	protected override void DisableKeyword()
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			this.renderers[i].material.DisableKeyword("BLACKTHREAD");
		}
	}

	// Token: 0x0400123B RID: 4667
	[SerializeField]
	private List<Renderer> renderers = new List<Renderer>();

	// Token: 0x0400123C RID: 4668
	[SerializeField]
	private bool autoFindRenderers;
}
