using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200016F RID: 367
public sealed class ScaleWithAspect : MonoBehaviour
{
	// Token: 0x06000BB6 RID: 2998 RVA: 0x00035610 File Offset: 0x00033810
	private void OnEnable()
	{
		Camera mainCamera = GameCameras.instance.mainCamera;
		if (mainCamera)
		{
			this.OnCameraAspectChanged(mainCamera.aspect);
		}
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
	}

	// Token: 0x06000BB7 RID: 2999 RVA: 0x0003564D File Offset: 0x0003384D
	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
	}

	// Token: 0x06000BB8 RID: 3000 RVA: 0x00035660 File Offset: 0x00033860
	[ContextMenu("Record Scale")]
	private void RecordScale()
	{
		this.baseScale = base.transform.localScale;
	}

	// Token: 0x06000BB9 RID: 3001 RVA: 0x00035674 File Offset: 0x00033874
	private void OnCameraAspectChanged(float currentAspect)
	{
		float num = this.baseAspect / currentAspect;
		Vector3 vector = new Vector3(this.baseScale.x * num, this.baseScale.y * num, this.baseScale.z * num);
		if (this.minScale.IsEnabled)
		{
			vector = Vector3.Max(vector, this.minScale.Value);
		}
		if (this.maxScale.IsEnabled)
		{
			vector = Vector3.Min(vector, this.maxScale.Value);
		}
		base.transform.localScale = vector;
	}

	// Token: 0x04000B4B RID: 2891
	[SerializeField]
	private float baseAspect = 1.7777778f;

	// Token: 0x04000B4C RID: 2892
	[SerializeField]
	private Vector3 baseScale = new Vector3(1f, 1f, 1f);

	// Token: 0x04000B4D RID: 2893
	[SerializeField]
	private ScaleWithAspect.OverrideVector3 minScale;

	// Token: 0x04000B4E RID: 2894
	[SerializeField]
	private ScaleWithAspect.OverrideVector3 maxScale;

	// Token: 0x0200149C RID: 5276
	[Serializable]
	private class OverrideVector3 : OverrideValue<Vector3>
	{
	}
}
