using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000165 RID: 357
public class CameraRenderToUI : MonoBehaviour
{
	// Token: 0x06000B1E RID: 2846 RVA: 0x0003230C File Offset: 0x0003050C
	private void Start()
	{
		Rect rect = this.image.rectTransform.rect;
		this.renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 0, RenderTextureFormat.Default);
		this.renderTexture.name = "CameraRenderToUI" + base.GetInstanceID().ToString();
		this.camera.targetTexture = this.renderTexture;
		this.image.texture = this.renderTexture;
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0003238C File Offset: 0x0003058C
	private void OnDestroy()
	{
		if (this.renderTexture != null)
		{
			this.renderTexture.Release();
			Object.Destroy(this.renderTexture);
			this.renderTexture = null;
		}
	}

	// Token: 0x04000A98 RID: 2712
	[SerializeField]
	private Camera camera;

	// Token: 0x04000A99 RID: 2713
	[SerializeField]
	private RawImage image;

	// Token: 0x04000A9A RID: 2714
	private RenderTexture renderTexture;
}
