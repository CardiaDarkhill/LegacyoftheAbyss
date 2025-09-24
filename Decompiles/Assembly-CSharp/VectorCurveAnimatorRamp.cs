using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000CC RID: 204
public class VectorCurveAnimatorRamp : RampBase
{
	// Token: 0x0600068B RID: 1675 RVA: 0x0002132C File Offset: 0x0001F52C
	private void Awake()
	{
		Transform transform = this.parentOverride ? this.parentOverride : base.transform;
		if (this.getChildren)
		{
			this.curveAnimators = transform.GetComponentsInChildren<VectorCurveAnimator>();
		}
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0002136C File Offset: 0x0001F56C
	protected override void UpdateValues(float multiplier)
	{
		VectorCurveAnimator[] array = this.curveAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpeedMultiplier = multiplier;
		}
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00021398 File Offset: 0x0001F598
	protected override void ResetValues()
	{
		VectorCurveAnimator[] array = this.curveAnimators;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpeedMultiplier = 1f;
		}
	}

	// Token: 0x0400065E RID: 1630
	[Space]
	[SerializeField]
	private VectorCurveAnimator[] curveAnimators;

	// Token: 0x0400065F RID: 1631
	[SerializeField]
	private bool getChildren;

	// Token: 0x04000660 RID: 1632
	[SerializeField]
	[ModifiableProperty]
	[Conditional("getChildren", true, false, false)]
	private Transform parentOverride;
}
