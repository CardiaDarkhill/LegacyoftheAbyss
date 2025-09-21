using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class CameraRenderScaledApply : MonoBehaviour
{
	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000B0B RID: 2827 RVA: 0x00031F4A File Offset: 0x0003014A
	// (set) Token: 0x06000B0C RID: 2828 RVA: 0x00031F52 File Offset: 0x00030152
	public RenderTexture Texture { get; set; }

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000B0D RID: 2829 RVA: 0x00031F5B File Offset: 0x0003015B
	// (set) Token: 0x06000B0E RID: 2830 RVA: 0x00031F63 File Offset: 0x00030163
	public Camera SourceCamera { get; set; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000B0F RID: 2831 RVA: 0x00031F6C File Offset: 0x0003016C
	public bool IsActive
	{
		get
		{
			return this.camera && this.camera.isActiveAndEnabled;
		}
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x00031F88 File Offset: 0x00030188
	private void Awake()
	{
		this.camera = base.GetComponent<Camera>();
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00031F98 File Offset: 0x00030198
	private void OnPreRender()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.SourceCamera && this.SourceCamera.isActiveAndEnabled && this.Texture)
		{
			Graphics.Blit(this.Texture, this.camera.activeTexture);
		}
	}

	// Token: 0x04000A88 RID: 2696
	private Camera camera;
}
