using System;
using UnityEngine;

// Token: 0x0200024C RID: 588
[RequireComponent(typeof(GameCameras))]
public class LightBlurredBackground : MonoBehaviour
{
	// Token: 0x1700024B RID: 587
	// (get) Token: 0x0600155E RID: 5470 RVA: 0x00060B7B File Offset: 0x0005ED7B
	// (set) Token: 0x0600155F RID: 5471 RVA: 0x00060B83 File Offset: 0x0005ED83
	public int RenderTextureHeight
	{
		get
		{
			return this.renderTextureHeight;
		}
		set
		{
			this.renderTextureHeight = value;
		}
	}

	// Token: 0x06001560 RID: 5472 RVA: 0x00060B8C File Offset: 0x0005ED8C
	protected void Awake()
	{
		this.gameCameras = base.GetComponent<GameCameras>();
		this.sceneCamera = this.gameCameras.tk2dCam.GetComponent<Camera>();
		this.passGroupCount = 2;
		if (this.gameCameras != GameCameras.instance)
		{
			this.cancelEnable = true;
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x00060BDC File Offset: 0x0005EDDC
	protected void OnEnable()
	{
		if (this.cancelEnable)
		{
			return;
		}
		this.distantFarClipPlane = this.sceneCamera.farClipPlane;
		GameObject gameObject = new GameObject("BlurCamera");
		gameObject.transform.SetParent(this.sceneCamera.transform);
		this.backgroundCamera = gameObject.AddComponent<Camera>();
		this.backgroundCamera.CopyFrom(this.sceneCamera);
		this.backgroundCamera.farClipPlane = this.distantFarClipPlane;
		this.backgroundCamera.depth -= 5f;
		this.backgroundCamera.rect = new Rect(0f, 0f, 1f, 1f);
		this.backgroundCamera.cullingMask &= -33;
		this.lightBlur = gameObject.AddComponent<LightBlur>();
		this.lightBlur.PassGroupCount = this.passGroupCount;
		gameObject.AddComponent<CameraRenderHooks>();
		this.sceneCamera.GetComponent<CameraShakeManager>().CopyTo(gameObject);
		this.UpdateCameraClipPlanes();
		this.blitMaterialInstance = new Material(this.blitMaterial);
		this.blitMaterialInstance.EnableKeyword("BLUR_PLANE");
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
		this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		ForceCameraAspect.MainCamFovChanged += this.OnCameraFovChanged;
		this.OnCameraFovChanged(ForceCameraAspect.CurrentMainCamFov);
		this.OnBlurPlanesChanged();
		BlurPlane.BlurPlanesChanged += this.OnBlurPlanesChanged;
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x00060D4C File Offset: 0x0005EF4C
	private void OnCameraAspectChanged(float aspect)
	{
		if (aspect <= Mathf.Epsilon)
		{
			return;
		}
		int num = Mathf.RoundToInt((float)this.renderTextureHeight * aspect);
		if (num <= 0)
		{
			return;
		}
		if (this.renderTexture != null)
		{
			Object.Destroy(this.renderTexture);
		}
		this.renderTexture = new RenderTexture(num, this.renderTextureHeight, 32, RenderTextureFormat.Default);
		this.renderTexture.name = "LightBlurredBackground" + base.GetInstanceID().ToString();
		this.backgroundCamera.targetTexture = this.renderTexture;
		this.blitMaterialInstance.mainTexture = this.renderTexture;
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x00060DE9 File Offset: 0x0005EFE9
	private void OnCameraFovChanged(float newFov)
	{
		this.backgroundCamera.fieldOfView = newFov;
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x00060DF8 File Offset: 0x0005EFF8
	private void OnDestroy()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
		ForceCameraAspect.MainCamFovChanged -= this.OnCameraFovChanged;
		BlurPlane.BlurPlanesChanged -= this.OnBlurPlanesChanged;
		if (this.renderTexture != null)
		{
			this.renderTexture.Release();
			Object.Destroy(this.renderTexture);
			this.renderTexture = null;
		}
		if (this.blitMaterialInstance != null)
		{
			Object.Destroy(this.blitMaterialInstance);
			this.blitMaterialInstance = null;
		}
	}

	// Token: 0x06001565 RID: 5477 RVA: 0x00060E84 File Offset: 0x0005F084
	protected void OnDisable()
	{
		if (this.cancelEnable)
		{
			return;
		}
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
		ForceCameraAspect.MainCamFovChanged -= this.OnCameraFovChanged;
		BlurPlane.BlurPlanesChanged -= this.OnBlurPlanesChanged;
		for (int i = 0; i < BlurPlane.BlurPlaneCount; i++)
		{
			BlurPlane blurPlane = BlurPlane.GetBlurPlane(i);
			blurPlane.SetPlaneMaterial(null);
			blurPlane.SetPlaneVisibility(true);
		}
		Object.Destroy(this.blitMaterialInstance);
		this.blitMaterialInstance = null;
		this.lightBlur = null;
		this.backgroundCamera.targetTexture = null;
		Object.Destroy(this.renderTexture);
		this.renderTexture = null;
		this.sceneCamera.farClipPlane = this.distantFarClipPlane;
		Object.Destroy(this.backgroundCamera.gameObject);
		this.backgroundCamera = null;
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x06001566 RID: 5478 RVA: 0x00060F4F File Offset: 0x0005F14F
	// (set) Token: 0x06001567 RID: 5479 RVA: 0x00060F57 File Offset: 0x0005F157
	public int PassGroupCount
	{
		get
		{
			return this.passGroupCount;
		}
		set
		{
			this.passGroupCount = value;
			if (this.lightBlur != null)
			{
				this.lightBlur.PassGroupCount = this.passGroupCount;
			}
		}
	}

	// Token: 0x06001568 RID: 5480 RVA: 0x00060F80 File Offset: 0x0005F180
	private void OnBlurPlanesChanged()
	{
		for (int i = 0; i < BlurPlane.BlurPlaneCount; i++)
		{
			BlurPlane blurPlane = BlurPlane.GetBlurPlane(i);
			blurPlane.SetPlaneVisibility(true);
			blurPlane.SetPlaneMaterial(this.blitMaterialInstance);
		}
		this.UpdateCameraClipPlanes();
	}

	// Token: 0x06001569 RID: 5481 RVA: 0x00060FBB File Offset: 0x0005F1BB
	protected void LateUpdate()
	{
		if (this.cancelEnable)
		{
			return;
		}
		this.UpdateCameraClipPlanes();
	}

	// Token: 0x0600156A RID: 5482 RVA: 0x00060FCC File Offset: 0x0005F1CC
	private void UpdateCameraClipPlanes()
	{
		BlurPlane closestBlurPlane = BlurPlane.ClosestBlurPlane;
		if (closestBlurPlane != null)
		{
			this.sceneCamera.farClipPlane = closestBlurPlane.PlaneZ - this.sceneCamera.transform.GetPositionZ() + this.clipEpsilon;
			this.backgroundCamera.nearClipPlane = closestBlurPlane.PlaneZ - this.backgroundCamera.transform.GetPositionZ() + this.clipEpsilon;
			return;
		}
		this.sceneCamera.farClipPlane = this.distantFarClipPlane;
		this.backgroundCamera.nearClipPlane = this.distantFarClipPlane;
	}

	// Token: 0x04001402 RID: 5122
	[SerializeField]
	private float distantFarClipPlane;

	// Token: 0x04001403 RID: 5123
	[SerializeField]
	private int renderTextureHeight;

	// Token: 0x04001404 RID: 5124
	[SerializeField]
	private Material blitMaterial;

	// Token: 0x04001405 RID: 5125
	[SerializeField]
	private float clipEpsilon;

	// Token: 0x04001406 RID: 5126
	private GameCameras gameCameras;

	// Token: 0x04001407 RID: 5127
	private Camera sceneCamera;

	// Token: 0x04001408 RID: 5128
	private Camera backgroundCamera;

	// Token: 0x04001409 RID: 5129
	private RenderTexture renderTexture;

	// Token: 0x0400140A RID: 5130
	private Material blurMaterialInstance;

	// Token: 0x0400140B RID: 5131
	private Material blitMaterialInstance;

	// Token: 0x0400140C RID: 5132
	private LightBlur lightBlur;

	// Token: 0x0400140D RID: 5133
	private int passGroupCount;

	// Token: 0x0400140E RID: 5134
	private bool cancelEnable;
}
