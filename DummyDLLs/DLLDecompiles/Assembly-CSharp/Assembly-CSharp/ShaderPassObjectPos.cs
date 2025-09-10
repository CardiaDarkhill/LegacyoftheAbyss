using System;
using UnityEngine;

// Token: 0x0200027B RID: 635
[ExecuteAlways]
public class ShaderPassObjectPos : MonoBehaviour
{
	// Token: 0x06001688 RID: 5768 RVA: 0x000654F2 File Offset: 0x000636F2
	private void Start()
	{
		this.DoUpdate();
	}

	// Token: 0x06001689 RID: 5769 RVA: 0x000654FC File Offset: 0x000636FC
	public void DoUpdate()
	{
		if (!this.renderer)
		{
			this.renderer = base.GetComponent<MeshRenderer>();
		}
		if (!this.renderer)
		{
			return;
		}
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.renderer.GetPropertyBlock(this.block);
		this.block.SetVector(ShaderPassObjectPos._objectScaleId, base.transform.position);
		this.renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x040014FB RID: 5371
	private MaterialPropertyBlock block;

	// Token: 0x040014FC RID: 5372
	private MeshRenderer renderer;

	// Token: 0x040014FD RID: 5373
	private static readonly int _objectScaleId = Shader.PropertyToID("_ObjectPos");
}
