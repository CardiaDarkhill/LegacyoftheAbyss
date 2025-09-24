using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000250 RID: 592
[ExecuteInEditMode]
public class MaterialPropertyRegion : MonoBehaviour
{
	// Token: 0x06001578 RID: 5496 RVA: 0x00061278 File Offset: 0x0005F478
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.minPoint, 0.2f);
		Gizmos.DrawWireSphere(this.maxPoint, 0.2f);
		Gizmos.DrawLine(this.minPoint, this.maxPoint);
	}

	// Token: 0x06001579 RID: 5497 RVA: 0x000612DA File Offset: 0x0005F4DA
	private void Awake()
	{
		this.Refresh();
	}

	// Token: 0x0600157A RID: 5498 RVA: 0x000612E4 File Offset: 0x0005F4E4
	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (this.modifiers == null || this.modifiers.Length == 0)
		{
			return;
		}
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		this.renderers = base.GetComponentsInChildren<Renderer>();
		this.UpdateSharedData();
		foreach (Renderer renderer in this.renderers)
		{
			this.ApplyModifiers(renderer);
		}
	}

	// Token: 0x0600157B RID: 5499 RVA: 0x00061348 File Offset: 0x0005F548
	private void UpdateSharedData()
	{
		this.minWorldPoint = base.transform.TransformPoint(this.minPoint);
		this.maxWorldPoint = base.transform.TransformPoint(this.maxPoint);
		this.distanceMax = this.maxWorldPoint - this.minWorldPoint;
		float num = Mathf.Min(this.distanceMax.x, this.distanceMax.y) / Mathf.Max(this.distanceMax.x, this.distanceMax.y);
		this.multiplierX = 0f;
		if (Mathf.Abs(this.distanceMax.x) > 0f)
		{
			this.multiplierX = ((this.distanceMax.x > this.distanceMax.y) ? (1f - num) : num);
		}
		this.multiplierY = 0f;
		if (Mathf.Abs(this.distanceMax.y) > 0f)
		{
			this.multiplierY = ((this.distanceMax.y > this.distanceMax.x) ? (1f - num) : num);
		}
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x0006147C File Offset: 0x0005F67C
	private void ApplyModifiers(Renderer renderer)
	{
		Vector2 vector = renderer.transform.position;
		vector.x = Mathf.Clamp(vector.x, this.minWorldPoint.x, this.maxWorldPoint.x);
		vector.y = Mathf.Clamp(vector.y, this.minWorldPoint.y, this.maxWorldPoint.y);
		Vector2 vector2 = this.maxWorldPoint - vector;
		float num = (this.multiplierX > 0f) ? (1f - vector2.x / this.distanceMax.x) : 0f;
		float num2 = (this.multiplierY > 0f) ? (1f - vector2.y / this.distanceMax.y) : 0f;
		float t = num * this.multiplierX + num2 * this.multiplierY;
		renderer.GetPropertyBlock(this.block);
		foreach (MaterialPropertyRegion.Modifier modifier in this.modifiers)
		{
			float value = Mathf.Lerp(modifier.Range.Start, modifier.Range.End, t);
			this.block.SetFloat(modifier.PropertyName, value);
		}
		renderer.SetPropertyBlock(this.block);
	}

	// Token: 0x04001415 RID: 5141
	[SerializeField]
	private MaterialPropertyRegion.Modifier[] modifiers;

	// Token: 0x04001416 RID: 5142
	[SerializeField]
	private Vector2 minPoint;

	// Token: 0x04001417 RID: 5143
	[SerializeField]
	private Vector2 maxPoint;

	// Token: 0x04001418 RID: 5144
	private Vector2 minWorldPoint;

	// Token: 0x04001419 RID: 5145
	private Vector2 maxWorldPoint;

	// Token: 0x0400141A RID: 5146
	private Vector2 distanceMax;

	// Token: 0x0400141B RID: 5147
	private float multiplierX;

	// Token: 0x0400141C RID: 5148
	private float multiplierY;

	// Token: 0x0400141D RID: 5149
	private Renderer[] renderers;

	// Token: 0x0400141E RID: 5150
	private MaterialPropertyBlock block;

	// Token: 0x0200154C RID: 5452
	[Serializable]
	private struct Modifier
	{
		// Token: 0x040086A7 RID: 34471
		public string PropertyName;

		// Token: 0x040086A8 RID: 34472
		public MinMaxFloat Range;
	}
}
