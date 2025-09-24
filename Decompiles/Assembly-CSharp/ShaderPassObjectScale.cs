using System;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200027C RID: 636
[ExecuteAlways]
public class ShaderPassObjectScale : MonoBehaviour
{
	// Token: 0x0600168C RID: 5772 RVA: 0x0006559E File Offset: 0x0006379E
	private void Start()
	{
		this.DoUpdate();
	}

	// Token: 0x0600168D RID: 5773 RVA: 0x000655A8 File Offset: 0x000637A8
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
		Vector3 lossyScale = base.transform.lossyScale;
		Vector4 value = new Vector4(1f, 1f, 1f, 1f);
		if ((this.passAxis & ShaderPassObjectScale.Axis.X) == ShaderPassObjectScale.Axis.X)
		{
			value.x = Mathf.Abs(lossyScale.x);
		}
		if ((this.passAxis & ShaderPassObjectScale.Axis.Y) == ShaderPassObjectScale.Axis.Y)
		{
			value.y = Mathf.Abs(lossyScale.y);
		}
		if ((this.passAxis & ShaderPassObjectScale.Axis.Z) == ShaderPassObjectScale.Axis.Z)
		{
			value.z = Mathf.Abs(lossyScale.z);
		}
		this.renderer.GetPropertyBlock(this.block);
		this.block.SetVector(ShaderPassObjectScale._objectScaleId, value);
		this.renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x040014FE RID: 5374
	[SerializeField]
	[EnumPickerBitmask]
	private ShaderPassObjectScale.Axis passAxis = ShaderPassObjectScale.Axis.All;

	// Token: 0x040014FF RID: 5375
	private MaterialPropertyBlock block;

	// Token: 0x04001500 RID: 5376
	private MeshRenderer renderer;

	// Token: 0x04001501 RID: 5377
	private static readonly int _objectScaleId = Shader.PropertyToID("_ObjectScale");

	// Token: 0x02001559 RID: 5465
	[Flags]
	private enum Axis
	{
		// Token: 0x040086D1 RID: 34513
		[UsedImplicitly]
		None = 0,
		// Token: 0x040086D2 RID: 34514
		X = 1,
		// Token: 0x040086D3 RID: 34515
		Y = 2,
		// Token: 0x040086D4 RID: 34516
		Z = 4,
		// Token: 0x040086D5 RID: 34517
		All = -1
	}
}
