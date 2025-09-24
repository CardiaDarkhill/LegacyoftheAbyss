using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200027A RID: 634
[ExecuteInEditMode]
public class SetZPerRotation : MonoBehaviour
{
	// Token: 0x06001683 RID: 5763 RVA: 0x000653B4 File Offset: 0x000635B4
	private void Reset()
	{
		this.rotationSource = base.transform;
		this.zTarget = base.transform;
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x000653D0 File Offset: 0x000635D0
	private void OnValidate()
	{
		if (this.altZRange.Start < 0f)
		{
			this.altZRange.Start = 0f;
		}
		if (this.altZRange.End > 360f)
		{
			this.altZRange.End = 360f;
		}
		if (this.altZRange.Start > this.altZRange.End)
		{
			this.altZRange.Start = this.altZRange.End;
		}
	}

	// Token: 0x06001685 RID: 5765 RVA: 0x0006544F File Offset: 0x0006364F
	private void OnEnable()
	{
		if (!this.rotationSource || !this.zTarget)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06001686 RID: 5766 RVA: 0x00065474 File Offset: 0x00063674
	private void LateUpdate()
	{
		float num;
		for (num = this.rotationSource.localEulerAngles.y; num < 0f; num += 360f)
		{
		}
		float num2 = this.altZRange.IsInRange(num) ? this.altZ : this.defaultZ;
		Vector3 localPosition = this.zTarget.localPosition;
		if (localPosition.z != num2)
		{
			localPosition.z = num2;
			this.zTarget.localPosition = localPosition;
		}
	}

	// Token: 0x040014F6 RID: 5366
	[SerializeField]
	private Transform rotationSource;

	// Token: 0x040014F7 RID: 5367
	[SerializeField]
	private Transform zTarget;

	// Token: 0x040014F8 RID: 5368
	[SerializeField]
	private float defaultZ;

	// Token: 0x040014F9 RID: 5369
	[SerializeField]
	private float altZ;

	// Token: 0x040014FA RID: 5370
	[SerializeField]
	private MinMaxFloat altZRange;
}
