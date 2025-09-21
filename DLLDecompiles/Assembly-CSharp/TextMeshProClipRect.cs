using System;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;

// Token: 0x02000734 RID: 1844
[ExecuteInEditMode]
public class TextMeshProClipRect : MonoBehaviour
{
	// Token: 0x060041E7 RID: 16871 RVA: 0x0012205C File Offset: 0x0012025C
	private void OnEnable()
	{
		this.renderer = base.GetComponent<Renderer>();
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
	}

	// Token: 0x060041E8 RID: 16872 RVA: 0x00122080 File Offset: 0x00120280
	private void LateUpdate()
	{
		Transform transform = base.transform;
		Vector3 a = this.relativeTo ? this.relativeTo.position : transform.position;
		Vector3 position = a + this.min.ToVector3(0f);
		Vector3 position2 = a + this.max.ToVector3(0f);
		Vector3 vector = transform.InverseTransformPoint(position);
		Vector3 vector2 = transform.InverseTransformPoint(position2);
		Vector4 bounds = new Vector4(vector.x, vector.y, vector2.x, vector2.y);
		this.ApplyBounds(this.renderer, bounds);
		foreach (TMP_SubMesh tmp_SubMesh in this.subMeshes)
		{
			this.ApplyBounds(tmp_SubMesh.renderer, bounds);
		}
	}

	// Token: 0x060041E9 RID: 16873 RVA: 0x00122174 File Offset: 0x00120374
	private void ApplyBounds(Renderer renderer, Vector4 bounds)
	{
		this.block.Clear();
		renderer.GetPropertyBlock(this.block);
		this.block.SetVector(TextMeshProClipRect._clipRectProp, bounds);
		renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x060041EA RID: 16874 RVA: 0x001221AA File Offset: 0x001203AA
	public void AddSubMesh(TMP_SubMesh subMesh)
	{
		this.subMeshes.Add(subMesh);
	}

	// Token: 0x060041EB RID: 16875 RVA: 0x001221B8 File Offset: 0x001203B8
	public void RemoveSubMesh(TMP_SubMesh subMesh)
	{
		this.subMeshes.Remove(subMesh);
	}

	// Token: 0x04004370 RID: 17264
	[SerializeField]
	private Transform relativeTo;

	// Token: 0x04004371 RID: 17265
	[SerializeField]
	private Vector2 min;

	// Token: 0x04004372 RID: 17266
	[SerializeField]
	private Vector2 max;

	// Token: 0x04004373 RID: 17267
	private Renderer renderer;

	// Token: 0x04004374 RID: 17268
	private MaterialPropertyBlock block;

	// Token: 0x04004375 RID: 17269
	private static readonly int _clipRectProp = Shader.PropertyToID("_ClipRect");

	// Token: 0x04004376 RID: 17270
	private List<TMP_SubMesh> subMeshes = new List<TMP_SubMesh>();
}
