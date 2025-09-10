using System;
using UnityEngine;

// Token: 0x02000788 RID: 1928
public class SpriteRendererVisibilityCentre : MonoBehaviour
{
	// Token: 0x06004469 RID: 17513 RVA: 0x0012BA1E File Offset: 0x00129C1E
	private void Awake()
	{
		this.spriteRenderers = base.GetComponentsInChildren<SpriteRenderer>(true);
		this.totalCentre = this.GetCentre(false);
	}

	// Token: 0x0600446A RID: 17514 RVA: 0x0012BA3C File Offset: 0x00129C3C
	public void Evaluate()
	{
		Transform t = this.repositionOverride ? this.repositionOverride : base.transform;
		t.SetLocalPosition2D(Vector2.zero);
		Vector2 centre = this.GetCentre(true);
		Vector2 position = this.totalCentre - centre;
		t.SetLocalPosition2D(position);
	}

	// Token: 0x0600446B RID: 17515 RVA: 0x0012BA8C File Offset: 0x00129C8C
	private Vector2 GetCentre(bool checkVisibility)
	{
		Vector2 a;
		Vector2 b;
		this.GetCurrentBounds(checkVisibility, out a, out b);
		return (a + b) * 0.5f;
	}

	// Token: 0x0600446C RID: 17516 RVA: 0x0012BAB8 File Offset: 0x00129CB8
	private void GetCurrentBounds(bool checkVisibility, out Vector2 boundsMin, out Vector2 boundsMax)
	{
		boundsMin = Vector2.one * float.MaxValue;
		boundsMax = Vector2.zero * float.MinValue;
		foreach (SpriteRenderer spriteRenderer in this.spriteRenderers)
		{
			if (!checkVisibility || spriteRenderer.gameObject.activeSelf)
			{
				Bounds bounds = spriteRenderer.bounds;
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (min.x < boundsMin.x)
				{
					boundsMin.x = min.x;
				}
				if (min.y < boundsMin.y)
				{
					boundsMin.y = min.y;
				}
				if (max.x > boundsMax.x)
				{
					boundsMax.x = max.x;
				}
				if (max.y > boundsMax.y)
				{
					boundsMax.y = max.y;
				}
			}
		}
	}

	// Token: 0x0400457E RID: 17790
	[SerializeField]
	private Transform repositionOverride;

	// Token: 0x0400457F RID: 17791
	private SpriteRenderer[] spriteRenderers;

	// Token: 0x04004580 RID: 17792
	private Vector2 totalCentre;
}
