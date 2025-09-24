using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200015A RID: 346
public class CameraAspectScaler : MonoBehaviour
{
	// Token: 0x06000A83 RID: 2691 RVA: 0x0002F1E3 File Offset: 0x0002D3E3
	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
		this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0002F201 File Offset: 0x0002D401
	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0002F214 File Offset: 0x0002D414
	private void OnCameraAspectChanged(float aspect)
	{
		MinMaxFloat minMaxFloat = new MinMaxFloat(1.7777778f, 2.3916667f);
		float tbetween = minMaxFloat.GetTBetween(aspect);
		Vector3 localScale = Vector3.Lerp(this.minAspectScale, this.maxAspectScale, tbetween);
		base.transform.localScale = localScale;
	}

	// Token: 0x040009FD RID: 2557
	[SerializeField]
	private Vector3 minAspectScale = Vector3.one;

	// Token: 0x040009FE RID: 2558
	[SerializeField]
	private Vector3 maxAspectScale = Vector3.one;
}
