using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000164 RID: 356
public class CameraRenderToMesh : MonoBehaviour
{
	// Token: 0x06000B13 RID: 2835 RVA: 0x00031FF2 File Offset: 0x000301F2
	private void Reset()
	{
		this.targetCamera = base.GetComponent<Camera>();
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00032000 File Offset: 0x00030200
	private void Awake()
	{
		this.targetCamera.orthographic = this.sourceCamera.orthographic;
		this.targetCamera.orthographicSize = this.sourceCamera.orthographicSize;
		this.UpdateTexture();
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x00032034 File Offset: 0x00030234
	private void OnEnable()
	{
		if (this.aspectMatchMainCam)
		{
			ForceCameraAspect.ViewportAspectChanged += this.OnAspectChanged;
			this.OnAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		}
		if (this.activeSource == CameraRenderToMesh.ActiveSources.None)
		{
			return;
		}
		this.targetCamera.enabled = false;
		this.meshRenderer.gameObject.SetActive(false);
		CameraRenderToMesh._hasActiveSource.Add(this);
	}

	// Token: 0x06000B16 RID: 2838 RVA: 0x00032096 File Offset: 0x00030296
	private void OnDisable()
	{
		CameraRenderToMesh._hasActiveSource.Remove(this);
		if (this.aspectMatchMainCam)
		{
			ForceCameraAspect.ViewportAspectChanged -= this.OnAspectChanged;
		}
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x000320BD File Offset: 0x000302BD
	private void OnDestroy()
	{
		if (this.texture != null)
		{
			this.texture.Release();
			Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	// Token: 0x06000B18 RID: 2840 RVA: 0x000320EA File Offset: 0x000302EA
	private void LateUpdate()
	{
		this.UpdateTexture();
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x000320F2 File Offset: 0x000302F2
	private void OnAspectChanged(float newAspect)
	{
		this.aspect = newAspect;
		if (this.aspect < 1.7777778f)
		{
			this.aspect = 1.7777778f;
		}
		this.heightMult = ForceCameraAspect.CurrentMainCamHeightMult;
		this.UpdateTexture();
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00032124 File Offset: 0x00030324
	private void UpdateTexture()
	{
		float num = (this.aspect > Mathf.Epsilon) ? this.aspect : this.targetCamera.aspect;
		int num2 = Mathf.RoundToInt((float)this.sourceCamera.pixelHeight * this.heightMult);
		int num3 = Mathf.RoundToInt((float)num2 * this.aspect);
		if (num3 <= 0 || num2 <= 0)
		{
			return;
		}
		if (num3 == this.previousPixelWidth && num2 == this.previousPixelHeight)
		{
			return;
		}
		this.previousPixelWidth = num3;
		this.previousPixelHeight = num2;
		if (this.texture != null)
		{
			this.texture.Release();
			Object.Destroy(this.texture);
		}
		this.texture = new RenderTexture(num3, num2, 32, this.textureFormat);
		this.texture.name = "CameraRenderToMesh" + base.GetInstanceID().ToString();
		this.targetCamera.targetTexture = this.texture;
		Transform transform = this.meshRenderer.transform;
		float num4 = this.targetCamera.orthographicSize * 2f;
		transform.localScale = new Vector3(num4 * num, num4, 1f);
		this.meshRenderer.material.SetTexture(CameraRenderToMesh._mainTexProp, this.texture);
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x00032260 File Offset: 0x00030460
	public static void SetActive(CameraRenderToMesh.ActiveSources activeSource, bool value)
	{
		foreach (CameraRenderToMesh cameraRenderToMesh in CameraRenderToMesh._hasActiveSource)
		{
			if (cameraRenderToMesh.activeSource == activeSource)
			{
				cameraRenderToMesh.targetCamera.enabled = value;
				cameraRenderToMesh.meshRenderer.gameObject.SetActive(value);
			}
		}
	}

	// Token: 0x04000A8B RID: 2699
	[SerializeField]
	private Camera sourceCamera;

	// Token: 0x04000A8C RID: 2700
	[Space]
	[SerializeField]
	private Camera targetCamera;

	// Token: 0x04000A8D RID: 2701
	[SerializeField]
	private RenderTextureFormat textureFormat = RenderTextureFormat.Default;

	// Token: 0x04000A8E RID: 2702
	[SerializeField]
	[ModifiableProperty]
	[Conditional("aspectMatchMainCam", false, false, false)]
	private float aspect;

	// Token: 0x04000A8F RID: 2703
	[SerializeField]
	private bool aspectMatchMainCam;

	// Token: 0x04000A90 RID: 2704
	[Space]
	[SerializeField]
	private MeshRenderer meshRenderer;

	// Token: 0x04000A91 RID: 2705
	[Space]
	[SerializeField]
	private CameraRenderToMesh.ActiveSources activeSource;

	// Token: 0x04000A92 RID: 2706
	private int previousPixelWidth;

	// Token: 0x04000A93 RID: 2707
	private int previousPixelHeight;

	// Token: 0x04000A94 RID: 2708
	private RenderTexture texture;

	// Token: 0x04000A95 RID: 2709
	private float heightMult = 1f;

	// Token: 0x04000A96 RID: 2710
	private static readonly List<CameraRenderToMesh> _hasActiveSource = new List<CameraRenderToMesh>();

	// Token: 0x04000A97 RID: 2711
	private static readonly int _mainTexProp = Shader.PropertyToID("_MainTex");

	// Token: 0x02001497 RID: 5271
	public enum ActiveSources
	{
		// Token: 0x040083CD RID: 33741
		None,
		// Token: 0x040083CE RID: 33742
		GameMap
	}
}
