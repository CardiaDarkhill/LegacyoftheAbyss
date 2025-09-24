using System;
using UnityEngine;

// Token: 0x02000220 RID: 544
public class CameraBlurPlaneAnimator : MonoBehaviour
{
	// Token: 0x06001447 RID: 5191 RVA: 0x0005B4D9 File Offset: 0x000596D9
	private void OnEnable()
	{
		CameraBlurPlane.Spacing = this.Spacing;
		CameraBlurPlane.Vibrancy = this.Vibrancy;
	}

	// Token: 0x06001448 RID: 5192 RVA: 0x0005B4F4 File Offset: 0x000596F4
	private void LateUpdate()
	{
		if (!Mathf.Approximately(this.Spacing, this.oldSpacing))
		{
			this.oldSpacing = this.Spacing;
			CameraBlurPlane.Spacing = this.Spacing;
		}
		if (!Mathf.Approximately(this.Vibrancy, this.oldVibrancy))
		{
			this.oldVibrancy = this.Vibrancy;
			CameraBlurPlane.Vibrancy = this.Vibrancy;
		}
	}

	// Token: 0x04001280 RID: 4736
	public float Spacing;

	// Token: 0x04001281 RID: 4737
	public float Vibrancy;

	// Token: 0x04001282 RID: 4738
	private float oldSpacing;

	// Token: 0x04001283 RID: 4739
	private float oldVibrancy;
}
