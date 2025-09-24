using System;
using UnityEngine;

// Token: 0x020004B7 RID: 1207
public class CogPlatArm : MonoBehaviour
{
	// Token: 0x06002B93 RID: 11155 RVA: 0x000BF200 File Offset: 0x000BD400
	public void StartRotation()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.initialRotation = base.transform.GetLocalRotation2D();
		this.targetRotation = this.initialRotation + this.rotationOffset;
		CogPlat[] array = this.platforms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartRotation();
		}
	}

	// Token: 0x06002B94 RID: 11156 RVA: 0x000BF258 File Offset: 0x000BD458
	public void UpdateRotation(float time)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		float rotation = Mathf.LerpUnclamped(this.initialRotation, this.targetRotation, this.rotationCurve.Evaluate(time));
		base.transform.SetLocalRotation2D(rotation);
		CogPlat[] array = this.platforms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateRotation(time);
		}
	}

	// Token: 0x06002B95 RID: 11157 RVA: 0x000BF2B8 File Offset: 0x000BD4B8
	public void EndRotation()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		base.transform.SetLocalRotation2D(this.targetRotation);
		CogPlat[] array = this.platforms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].EndRotation();
		}
	}

	// Token: 0x04002CE4 RID: 11492
	[Header("Structure")]
	[SerializeField]
	private CogPlat[] platforms;

	// Token: 0x04002CE5 RID: 11493
	[Header("Parameters")]
	[SerializeField]
	private float rotationOffset = 90f;

	// Token: 0x04002CE6 RID: 11494
	[SerializeField]
	private AnimationCurve rotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002CE7 RID: 11495
	private float initialRotation;

	// Token: 0x04002CE8 RID: 11496
	private float targetRotation;
}
