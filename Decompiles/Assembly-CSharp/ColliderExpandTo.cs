using System;
using UnityEngine;

// Token: 0x02000226 RID: 550
public class ColliderExpandTo : MonoBehaviour
{
	// Token: 0x0600145F RID: 5215 RVA: 0x0005BA4F File Offset: 0x00059C4F
	private void Reset()
	{
		this.expandCollider = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x06001460 RID: 5216 RVA: 0x0005BA60 File Offset: 0x00059C60
	private void Awake()
	{
		Bounds bounds = this.expandCollider.bounds;
		Bounds bounds2 = this.toCollider.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector3 min2 = bounds2.min;
		Vector3 max2 = bounds2.max;
		if (min2.x < min.x)
		{
			min.x = min2.x;
		}
		if (min2.y < min.y)
		{
			min.y = min2.y;
		}
		if (max2.x > max.x)
		{
			max.x = max2.x;
		}
		if (max2.y > max.y)
		{
			max.y = max2.y;
		}
		bounds.SetMinMax(min, max);
		Transform transform = this.expandCollider.transform;
		this.expandCollider.offset = transform.InverseTransformPoint(bounds.center);
		this.expandCollider.size = transform.InverseTransformVector(bounds.size).Abs();
	}

	// Token: 0x0400129F RID: 4767
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private BoxCollider2D expandCollider;

	// Token: 0x040012A0 RID: 4768
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private BoxCollider2D toCollider;
}
