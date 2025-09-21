using System;
using UnityEngine;

// Token: 0x02000167 RID: 359
[ExecuteInEditMode]
public class DarknessCameraEffect : MonoBehaviour
{
	// Token: 0x170000FE RID: 254
	// (get) Token: 0x06000B44 RID: 2884 RVA: 0x00033A28 File Offset: 0x00031C28
	// (set) Token: 0x06000B45 RID: 2885 RVA: 0x00033A30 File Offset: 0x00031C30
	public bool IsDebugView
	{
		get
		{
			return this.isDebugView;
		}
		set
		{
			this.isDebugView = value;
			this.mainCamera.enabled = !this.isDebugView;
		}
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x00033A4D File Offset: 0x00031C4D
	private void Awake()
	{
		if (!this.mainCamera)
		{
			Debug.LogError("Can't merge null camera!", this);
			return;
		}
		this.camera = base.GetComponent<Camera>();
		if (!Application.isPlaying)
		{
			this.OnPreRender();
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x00033A81 File Offset: 0x00031C81
	private void OnDestroy()
	{
		if (this.camTex != null)
		{
			this.camera.targetTexture = null;
			Object.DestroyImmediate(this.camTex);
			this.camTex = null;
		}
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00033AAF File Offset: 0x00031CAF
	private void OnPreRender()
	{
		this.EnsureSetup();
		Shader.SetGlobalMatrix(DarknessCameraEffect._darknessCameraVpProp, this.CalculateVp());
		Shader.SetGlobalFloat(DarknessCameraEffect._previewLerpProp, this.IsDebugView ? 0f : 1f);
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00033AE5 File Offset: 0x00031CE5
	private void OnPostRender()
	{
		Shader.SetGlobalFloat(DarknessCameraEffect._previewLerpProp, 0f);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x00033AF8 File Offset: 0x00031CF8
	private void EnsureSetup()
	{
		this.camera.transparencySortMode = this.mainCamera.transparencySortMode;
		this.camera.fieldOfView = this.mainCamera.fieldOfView;
		if (this.isDebugView)
		{
			if (this.camTex != null)
			{
				this.camera.targetTexture = null;
				Object.DestroyImmediate(this.camTex);
			}
			this.camera.rect = this.mainCamera.rect;
			return;
		}
		this.camera.rect = new Rect(0f, 0f, 1f, 1f);
		if (this.camTex == null)
		{
			this.CreateTargetTexture();
			return;
		}
		if (this.camTex.width != this.mainCamera.pixelWidth || this.camTex.height != this.mainCamera.pixelHeight)
		{
			this.camera.targetTexture = null;
			Object.DestroyImmediate(this.camTex);
			this.CreateTargetTexture();
		}
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00033BFC File Offset: 0x00031DFC
	private void CreateTargetTexture()
	{
		this.camTex = new RenderTexture(this.mainCamera.pixelWidth, this.mainCamera.pixelHeight, 0)
		{
			hideFlags = HideFlags.DontSave,
			name = "DarknessCameraEffect" + base.GetInstanceID().ToString()
		};
		this.camera.targetTexture = this.camTex;
		Shader.SetGlobalTexture(DarknessCameraEffect._darknessCutoutProp, this.camTex);
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00033C74 File Offset: 0x00031E74
	private Matrix4x4 CalculateVp()
	{
		Matrix4x4 worldToCameraMatrix = this.camera.worldToCameraMatrix;
		return GL.GetGPUProjectionMatrix(this.camera.projectionMatrix, true) * worldToCameraMatrix;
	}

	// Token: 0x04000AE9 RID: 2793
	[SerializeField]
	private Camera mainCamera;

	// Token: 0x04000AEA RID: 2794
	private RenderTexture camTex;

	// Token: 0x04000AEB RID: 2795
	private Camera camera;

	// Token: 0x04000AEC RID: 2796
	private bool hasChecked;

	// Token: 0x04000AED RID: 2797
	private static readonly int _darknessCutoutProp = Shader.PropertyToID("_DarknessCutout");

	// Token: 0x04000AEE RID: 2798
	private static readonly int _darknessCameraVpProp = Shader.PropertyToID("_DarknessCameraVP");

	// Token: 0x04000AEF RID: 2799
	private static readonly int _previewLerpProp = Shader.PropertyToID("_PreviewLerp");

	// Token: 0x04000AF0 RID: 2800
	private bool isDebugView;
}
