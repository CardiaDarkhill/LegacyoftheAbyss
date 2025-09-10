using System;
using UnityEngine;

// Token: 0x0200021B RID: 539
public sealed class RenderTextureCache
{
	// Token: 0x1700023C RID: 572
	// (get) Token: 0x0600140E RID: 5134 RVA: 0x0005ABD3 File Offset: 0x00058DD3
	// (set) Token: 0x0600140F RID: 5135 RVA: 0x0005ABDA File Offset: 0x00058DDA
	public static bool ReuseRenderTexture { get; set; } = true;

	// Token: 0x06001410 RID: 5136 RVA: 0x0005ABE4 File Offset: 0x00058DE4
	~RenderTextureCache()
	{
		this.CleanUpRenderTexture();
	}

	// Token: 0x06001411 RID: 5137 RVA: 0x0005AC10 File Offset: 0x00058E10
	public RenderTexture GetRenderTexture(int width, int height, int depth, RenderTextureFormat format)
	{
		if (!RenderTextureCache.ReuseRenderTexture)
		{
			return RenderTexture.GetTemporary(width, height, depth, format);
		}
		if (this.createdRenderTexture && width == this.cachedW && height == this.cachedH && format == this.cachedFormat)
		{
			return this.renderTexture;
		}
		this.cachedW = width;
		this.cachedH = height;
		this.cachedFormat = format;
		if (this.createdRenderTexture)
		{
			this.renderTexture.Release();
			Object.Destroy(this.renderTexture);
			this.renderTexture = null;
		}
		this.createdRenderTexture = true;
		this.renderTexture = new RenderTexture(width, height, depth, format)
		{
			name = string.Format("{0}_{1}x{2}_frame{3}", new object[]
			{
				"BloomOptimized",
				width,
				height,
				Time.frameCount
			})
		};
		return this.renderTexture;
	}

	// Token: 0x06001412 RID: 5138 RVA: 0x0005ACF1 File Offset: 0x00058EF1
	public void CleanUpRenderTexture()
	{
		if (!this.createdRenderTexture)
		{
			return;
		}
		this.createdRenderTexture = false;
		if (!this.renderTexture)
		{
			return;
		}
		this.renderTexture.Release();
		Object.Destroy(this.renderTexture);
		this.renderTexture = null;
	}

	// Token: 0x0400125C RID: 4700
	private RenderTexture renderTexture;

	// Token: 0x0400125D RID: 4701
	private bool createdRenderTexture;

	// Token: 0x0400125E RID: 4702
	private int cachedW;

	// Token: 0x0400125F RID: 4703
	private int cachedH;

	// Token: 0x04001260 RID: 4704
	private int cachedDepth;

	// Token: 0x04001261 RID: 4705
	private RenderTextureFormat cachedFormat;
}
