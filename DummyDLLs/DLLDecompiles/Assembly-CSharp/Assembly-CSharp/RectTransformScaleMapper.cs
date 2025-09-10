using System;
using UnityEngine;

// Token: 0x020006FE RID: 1790
public class RectTransformScaleMapper : MonoBehaviour
{
	// Token: 0x06004009 RID: 16393 RVA: 0x0011A4BC File Offset: 0x001186BC
	private void Start()
	{
		if (!this.overrideInitialSourceRectSize)
		{
			this.initialSourceRectSize = this.source.rect.size;
		}
		if (!this.overrideInitialTargetScale)
		{
			this.initialTargetScale = this.target.localScale;
		}
	}

	// Token: 0x0600400A RID: 16394 RVA: 0x0011A508 File Offset: 0x00118708
	private void LateUpdate()
	{
		Vector2 size = this.source.rect.size;
		if (Math.Abs(size.x - this.previousSize.x) < 0.001f && Math.Abs(size.y - this.previousSize.y) < 0.001f)
		{
			return;
		}
		this.previousSize = size;
		Vector2 vector = size.DivideElements(this.initialSourceRectSize);
		Vector3 localScale = this.target.localScale;
		localScale.x = this.initialTargetScale.x * vector.x;
		localScale.y = this.initialTargetScale.y * vector.y;
		this.target.localScale = localScale;
	}

	// Token: 0x040041B7 RID: 16823
	[SerializeField]
	private RectTransform source;

	// Token: 0x040041B8 RID: 16824
	[SerializeField]
	private bool overrideInitialSourceRectSize;

	// Token: 0x040041B9 RID: 16825
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideInitialSourceRectSize", true, false, false)]
	private Vector2 initialSourceRectSize;

	// Token: 0x040041BA RID: 16826
	[Space]
	[SerializeField]
	private Transform target;

	// Token: 0x040041BB RID: 16827
	[SerializeField]
	private bool overrideInitialTargetScale;

	// Token: 0x040041BC RID: 16828
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideInitialTargetScale", true, false, false)]
	private Vector2 initialTargetScale;

	// Token: 0x040041BD RID: 16829
	private Vector2 previousSize;
}
