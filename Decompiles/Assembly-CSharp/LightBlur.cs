using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200024B RID: 587
public class LightBlur : PostEffectsBase
{
	// Token: 0x17000249 RID: 585
	// (get) Token: 0x06001554 RID: 5460 RVA: 0x00060941 File Offset: 0x0005EB41
	// (set) Token: 0x06001555 RID: 5461 RVA: 0x00060949 File Offset: 0x0005EB49
	public int PassGroupCount
	{
		get
		{
			return this.passGroupCount;
		}
		set
		{
			this.passGroupCount = value;
		}
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x06001556 RID: 5462 RVA: 0x00060952 File Offset: 0x0005EB52
	public int BlurPassCount
	{
		get
		{
			return this.passGroupCount * 2;
		}
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x0006095C File Offset: 0x0005EB5C
	protected void Awake()
	{
		this.passGroupCount = 2;
	}

	// Token: 0x06001558 RID: 5464 RVA: 0x00060965 File Offset: 0x0005EB65
	protected override void OnEnable()
	{
		base.OnEnable();
		this.effectIsSupported = this.CheckResources();
	}

	// Token: 0x06001559 RID: 5465 RVA: 0x00060979 File Offset: 0x0005EB79
	private void OnDisable()
	{
		this.rt1Cache.CleanUpRenderTexture();
		this.rt2Cache.CleanUpRenderTexture();
	}

	// Token: 0x0600155A RID: 5466 RVA: 0x00060991 File Offset: 0x0005EB91
	protected void OnDestroy()
	{
		if (this.blurMaterial != null)
		{
			Object.Destroy(this.blurMaterial);
		}
		this.blurMaterial = null;
	}

	// Token: 0x0600155B RID: 5467 RVA: 0x000609B4 File Offset: 0x0005EBB4
	public override bool CheckResources()
	{
		bool flag = true;
		if (this.blurInfoId == 0)
		{
			this.blurInfoId = Shader.PropertyToID("_BlurInfo");
		}
		if (this.blurShader == null)
		{
			this.blurShader = Shader.Find("Hollow Knight/Light Blur");
			if (this.blurShader == null)
			{
				Debug.LogErrorFormat(this, "Failed to find shader {0}", new object[]
				{
					"Hollow Knight/Light Blur"
				});
				flag = false;
			}
		}
		if (this.blurMaterial == null)
		{
			this.blurMaterial = base.CheckShaderAndCreateMaterial(this.blurShader, this.blurMaterial);
		}
		return base.CheckSupport() && flag;
	}

	// Token: 0x0600155C RID: 5468 RVA: 0x00060A50 File Offset: 0x0005EC50
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.effectIsSupported)
		{
			base.enabled = false;
			Graphics.Blit(source, destination);
			return;
		}
		RenderTexture renderTexture = source;
		Vector4 value = new Vector4(1f / (float)source.width, 1f / (float)source.height, 0f, 0f);
		RenderTexture renderTexture2 = this.rt1Cache.GetRenderTexture(source.width, source.height, 32, source.format);
		RenderTexture renderTexture3 = this.rt2Cache.GetRenderTexture(source.width, source.height, 32, source.format);
		bool flag = true;
		for (int i = 0; i < this.BlurPassCount; i++)
		{
			RenderTexture renderTexture4;
			if (i == this.BlurPassCount - 1)
			{
				renderTexture4 = destination;
			}
			else if (flag)
			{
				renderTexture4 = renderTexture2;
				flag = false;
			}
			else
			{
				renderTexture4 = renderTexture3;
				flag = true;
			}
			this.blurMaterial.SetVector(this.blurInfoId, value);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, renderTexture4, this.blurMaterial, i % 2);
			renderTexture = renderTexture4;
		}
		if (!RenderTextureCache.ReuseRenderTexture)
		{
			RenderTexture.ReleaseTemporary(renderTexture2);
			RenderTexture.ReleaseTemporary(renderTexture3);
		}
	}

	// Token: 0x040013F7 RID: 5111
	private const string BlurShaderName = "Hollow Knight/Light Blur";

	// Token: 0x040013F8 RID: 5112
	private const int BlurMaterialPassCount = 2;

	// Token: 0x040013F9 RID: 5113
	private int passGroupCount;

	// Token: 0x040013FA RID: 5114
	private const int BlurPassCountMax = 4;

	// Token: 0x040013FB RID: 5115
	private const string BlurInfoPropertyName = "_BlurInfo";

	// Token: 0x040013FC RID: 5116
	private int blurInfoId;

	// Token: 0x040013FD RID: 5117
	private Shader blurShader;

	// Token: 0x040013FE RID: 5118
	private Material blurMaterial;

	// Token: 0x040013FF RID: 5119
	private bool effectIsSupported;

	// Token: 0x04001400 RID: 5120
	private RenderTextureCache rt1Cache = new RenderTextureCache();

	// Token: 0x04001401 RID: 5121
	private RenderTextureCache rt2Cache = new RenderTextureCache();
}
