using System;
using UnityEngine;

// Token: 0x02000718 RID: 1816
public class ScreenHeightAdjustPos : MonoBehaviour
{
	// Token: 0x060040A6 RID: 16550 RVA: 0x0011C34A File Offset: 0x0011A54A
	private void Awake()
	{
		this.initialLocalPos = base.transform.localPosition;
	}

	// Token: 0x060040A7 RID: 16551 RVA: 0x0011C35D File Offset: 0x0011A55D
	private void OnEnable()
	{
		ForceCameraAspect.MainCamHeightMultChanged += this.OnMainCamHeightMultChanged;
		this.OnMainCamHeightMultChanged(ForceCameraAspect.CurrentMainCamHeightMult);
	}

	// Token: 0x060040A8 RID: 16552 RVA: 0x0011C37B File Offset: 0x0011A57B
	private void OnDisable()
	{
		ForceCameraAspect.MainCamHeightMultChanged -= this.OnMainCamHeightMultChanged;
	}

	// Token: 0x060040A9 RID: 16553 RVA: 0x0011C390 File Offset: 0x0011A590
	private void OnMainCamHeightMultChanged(float heightMult)
	{
		float num = heightMult - 1f;
		Vector3 localPosition = this.initialLocalPos;
		localPosition.y += this.heightOffset * num;
		base.transform.localPosition = localPosition;
	}

	// Token: 0x0400422D RID: 16941
	[SerializeField]
	private float heightOffset;

	// Token: 0x0400422E RID: 16942
	private Vector3 initialLocalPos;
}
