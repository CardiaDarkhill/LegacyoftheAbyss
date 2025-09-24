using System;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class KeepWorldPosScreenDepth : MonoBehaviour
{
	// Token: 0x06001546 RID: 5446 RVA: 0x000606C5 File Offset: 0x0005E8C5
	private void Awake()
	{
		this.mainCamera = GameCameras.instance.mainCamera;
	}

	// Token: 0x06001547 RID: 5447 RVA: 0x000606D7 File Offset: 0x0005E8D7
	private void OnEnable()
	{
		CameraRenderHooks.CameraPreCull += this.OnCameraPreCull;
		CameraRenderHooks.CameraPostRender += this.OnCameraPostRender;
	}

	// Token: 0x06001548 RID: 5448 RVA: 0x000606FB File Offset: 0x0005E8FB
	private void OnDisable()
	{
		CameraRenderHooks.CameraPreCull -= this.OnCameraPreCull;
		CameraRenderHooks.CameraPostRender -= this.OnCameraPostRender;
	}

	// Token: 0x06001549 RID: 5449 RVA: 0x00060720 File Offset: 0x0005E920
	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		this.lastSource = (CameraRenderHooks.CameraSource)(-1);
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		Transform transform = base.transform;
		this.preCullPos = transform.position;
		this.preCullScale = transform.localScale;
		this.lastSource = cameraType;
		if (cameraType != this.targetCamera)
		{
			transform.position = new Vector3(-2000f, -2000f);
			transform.localScale = new Vector3(0.001f, 0.001f, 1f);
			return;
		}
		Vector3 position = this.mainCamera.transform.position;
		float num = this.preCullPos.z - position.z;
		float num2 = (this.renderZ - position.z) / num;
		Vector3 vector = this.mainCamera.WorldToViewportPoint(this.preCullPos);
		Vector3 position2 = new Vector3(vector.x, vector.y, vector.z * num2);
		Vector3 position3 = this.mainCamera.ViewportToWorldPoint(position2);
		transform.position = position3;
		transform.localScale = this.preCullScale * num2;
	}

	// Token: 0x0600154A RID: 5450 RVA: 0x0006082A File Offset: 0x0005EA2A
	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != this.lastSource)
		{
			return;
		}
		Transform transform = base.transform;
		transform.position = this.preCullPos;
		transform.localScale = this.preCullScale;
	}

	// Token: 0x040013EC RID: 5100
	[SerializeField]
	private CameraRenderHooks.CameraSource targetCamera = CameraRenderHooks.CameraSource.MainCamera;

	// Token: 0x040013ED RID: 5101
	[SerializeField]
	private float renderZ;

	// Token: 0x040013EE RID: 5102
	private CameraRenderHooks.CameraSource lastSource;

	// Token: 0x040013EF RID: 5103
	private Camera mainCamera;

	// Token: 0x040013F0 RID: 5104
	private Vector3 preCullPos;

	// Token: 0x040013F1 RID: 5105
	private Vector3 preCullScale;
}
