using System;
using UnityEngine;

// Token: 0x0200026F RID: 623
[ExecuteInEditMode]
public class ScaleToMatchCollider : MonoBehaviour
{
	// Token: 0x06001638 RID: 5688 RVA: 0x00063CA0 File Offset: 0x00061EA0
	private void OnDrawGizmosSelected()
	{
		Bounds contractedBounds = this.GetContractedBounds();
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(contractedBounds.center, contractedBounds.size);
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00063CD1 File Offset: 0x00061ED1
	private void Start()
	{
		this.UpdateScale();
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00063CDC File Offset: 0x00061EDC
	private Bounds GetContractedBounds()
	{
		Bounds bounds = this.collider.bounds;
		bounds.SetMinMax(bounds.min + this.contractMin.ToVector3(0f), bounds.max - this.contractMax.ToVector3(0f));
		return bounds;
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x00063D38 File Offset: 0x00061F38
	private void UpdateScale()
	{
		if (!this.collider)
		{
			return;
		}
		Bounds contractedBounds = this.GetContractedBounds();
		Vector3 size = contractedBounds.size;
		Transform transform = base.transform;
		Transform parent = transform.parent;
		Vector3 self = parent ? parent.InverseTransformVector(size) : size;
		transform.localScale = self.MultiplyElements(this.multiplyScale);
		transform.position = contractedBounds.center;
	}

	// Token: 0x040014A5 RID: 5285
	[SerializeField]
	private Collider2D collider;

	// Token: 0x040014A6 RID: 5286
	[SerializeField]
	private Vector2 multiplyScale = Vector2.one;

	// Token: 0x040014A7 RID: 5287
	[SerializeField]
	private Vector2 contractMax;

	// Token: 0x040014A8 RID: 5288
	[SerializeField]
	private Vector2 contractMin;
}
