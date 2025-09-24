using System;
using UnityEngine;

// Token: 0x02000277 RID: 631
[ExecuteInEditMode]
public class SetShaderPropertyRotation : MonoBehaviour
{
	// Token: 0x06001676 RID: 5750 RVA: 0x00065193 File Offset: 0x00063393
	private void OnEnable()
	{
		this.UpdateValue();
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x0006519B File Offset: 0x0006339B
	private void Update()
	{
		if (!Application.isPlaying || this.updateAtRuntime)
		{
			this.UpdateValue();
		}
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x000651B4 File Offset: 0x000633B4
	private void UpdateValue()
	{
		if (!this.renderer)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.variableName))
		{
			return;
		}
		float value = (base.transform.eulerAngles.z + this.offset) * 0.017453292f;
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.block.Clear();
		this.renderer.GetPropertyBlock(this.block);
		this.block.SetFloat(this.variableName, value);
		this.renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x040014E7 RID: 5351
	[SerializeField]
	private Renderer renderer;

	// Token: 0x040014E8 RID: 5352
	[SerializeField]
	private string variableName;

	// Token: 0x040014E9 RID: 5353
	[SerializeField]
	private float offset;

	// Token: 0x040014EA RID: 5354
	[SerializeField]
	private bool updateAtRuntime;

	// Token: 0x040014EB RID: 5355
	private MaterialPropertyBlock block;
}
