using System;
using System.Runtime.CompilerServices;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000168 RID: 360
public class DisplayFrozenCamera : MonoBehaviour
{
	// Token: 0x170000FF RID: 255
	// (get) Token: 0x06000B4F RID: 2895 RVA: 0x00033CDB File Offset: 0x00031EDB
	// (set) Token: 0x06000B50 RID: 2896 RVA: 0x00033CE2 File Offset: 0x00031EE2
	public static bool IsRendering { get; private set; }

	// Token: 0x17000100 RID: 256
	// (get) Token: 0x06000B51 RID: 2897 RVA: 0x00033CEA File Offset: 0x00031EEA
	// (set) Token: 0x06000B52 RID: 2898 RVA: 0x00033CF2 File Offset: 0x00031EF2
	public float BlurT
	{
		get
		{
			return this.lastBlurT;
		}
		set
		{
			if (!Application.isPlaying)
			{
				return;
			}
			if (Math.Abs(value - this.lastBlurT) <= Mathf.Epsilon)
			{
				return;
			}
			this.lastBlurT = value;
			this.UpdateBlur();
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x00033D1E File Offset: 0x00031F1E
	private void Awake()
	{
		this.renderer = base.GetComponent<MeshRenderer>();
		this.activeBlurMat = new Material(this.blurMaterial);
		if (this.activeBlurMat)
		{
			this.UpdateBlur();
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00033D50 File Offset: 0x00031F50
	private void OnDestroy()
	{
		if (this.activeBlurMat)
		{
			Object.Destroy(this.activeBlurMat);
			this.activeBlurMat = null;
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x00033D74 File Offset: 0x00031F74
	private void Update()
	{
		if (this.unfreezeQueueFrames < 0)
		{
			return;
		}
		this.unfreezeQueueFrames--;
		if (this.unfreezeQueueFrames > 0)
		{
			return;
		}
		this.renderer.enabled = false;
		if (this.baseTexture != null)
		{
			Object.Destroy(this.baseTexture);
			this.baseTexture = null;
		}
		if (this.blurredTexture != null)
		{
			Object.Destroy(this.blurredTexture);
			this.blurredTexture = null;
		}
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x00033DF0 File Offset: 0x00031FF0
	public void Freeze()
	{
		DisplayFrozenCamera.<>c__DisplayClass25_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.renderer == null)
		{
			return;
		}
		Camera main = Camera.main;
		if (main == null)
		{
			return;
		}
		float num = (float)this.displayCamera.pixelWidth;
		float num2 = (float)this.displayCamera.pixelHeight;
		float num3 = this.displayCamera.orthographicSize * 2f;
		float x = num3 * (num / num2);
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		localScale.x = x;
		localScale.y = num3;
		transform.localScale = localScale;
		CS$<>8__locals1.width = main.pixelWidth;
		CS$<>8__locals1.height = main.pixelHeight;
		ScreenRes resolution = CameraRenderScaled.Resolution;
		if (resolution.Width != 0)
		{
			CS$<>8__locals1.width = resolution.Width;
		}
		if (resolution.Height != 0)
		{
			CS$<>8__locals1.height = resolution.Height;
		}
		this.<Freeze>g__FixTextureSize|25_0(ref this.baseTexture, ref CS$<>8__locals1);
		if (this.blurMaterial)
		{
			this.<Freeze>g__FixTextureSize|25_0(ref this.blurredTexture, ref CS$<>8__locals1);
		}
		RenderTexture targetTexture = main.targetTexture;
		main.enabled = true;
		main.targetTexture = this.baseTexture;
		DisplayFrozenCamera.IsRendering = true;
		main.Render();
		DisplayFrozenCamera.IsRendering = false;
		main.targetTexture = targetTexture;
		main.enabled = false;
		this.frozenCamera = main;
		this.renderer.enabled = true;
		this.renderer.material.SetTexture(DisplayFrozenCamera._mainTexId, this.baseTexture);
	}

	// Token: 0x06000B57 RID: 2903 RVA: 0x00033F64 File Offset: 0x00032164
	public void Unfreeze()
	{
		if (this.frozenCamera == null)
		{
			return;
		}
		if (this.fixNoise)
		{
			this.fixNoise.ForceRender = true;
		}
		this.frozenCamera.enabled = true;
		this.frozenCamera = null;
		this.unfreezeQueueFrames = 2;
	}

	// Token: 0x06000B58 RID: 2904 RVA: 0x00033FB4 File Offset: 0x000321B4
	private void UpdateBlur()
	{
		if (!this.renderer.enabled)
		{
			return;
		}
		float num = this.blurSpacingCurve.Evaluate(this.lastBlurT);
		if (num <= Mathf.Epsilon)
		{
			this.renderer.material.SetTexture(DisplayFrozenCamera._mainTexId, this.baseTexture);
			return;
		}
		this.activeBlurMat.SetFloat(DisplayFrozenCamera._sizeProp, this.blurSpacingMax * num);
		RenderTexture temporary = RenderTexture.GetTemporary(this.blurredTexture.width, this.blurredTexture.height);
		RenderTexture temporary2 = RenderTexture.GetTemporary(this.blurredTexture.width, this.blurredTexture.height);
		Graphics.Blit(this.baseTexture, temporary, this.activeBlurMat, 0);
		bool flag = false;
		for (int i = 1; i < 4; i++)
		{
			if (flag)
			{
				Graphics.Blit(temporary2, temporary, this.activeBlurMat, i);
			}
			else
			{
				Graphics.Blit(temporary, temporary2, this.activeBlurMat, i);
			}
			flag = !flag;
		}
		Graphics.Blit(flag ? temporary2 : temporary, this.blurredTexture, this.activeBlurMat, 4);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		this.renderer.material.SetTexture(DisplayFrozenCamera._mainTexId, this.blurredTexture);
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x0003412C File Offset: 0x0003232C
	[CompilerGenerated]
	private void <Freeze>g__FixTextureSize|25_0(ref RenderTexture texture, ref DisplayFrozenCamera.<>c__DisplayClass25_0 A_2)
	{
		bool flag = false;
		if (texture != null && (texture.width != A_2.width || texture.height != A_2.height))
		{
			Object.Destroy(texture);
			flag = true;
		}
		if (flag || texture == null)
		{
			texture = new RenderTexture(A_2.width, A_2.height, 24)
			{
				name = "DisplayFrozenCamera" + base.GetInstanceID().ToString()
			};
		}
	}

	// Token: 0x04000AF1 RID: 2801
	private const int BLUR_MAT_PASSES = 5;

	// Token: 0x04000AF2 RID: 2802
	[SerializeField]
	private Camera displayCamera;

	// Token: 0x04000AF3 RID: 2803
	[SerializeField]
	private FastNoise fixNoise;

	// Token: 0x04000AF4 RID: 2804
	[Space]
	[SerializeField]
	private Material blurMaterial;

	// Token: 0x04000AF5 RID: 2805
	[SerializeField]
	private float blurSpacingMax;

	// Token: 0x04000AF6 RID: 2806
	[SerializeField]
	private AnimationCurve blurSpacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04000AF7 RID: 2807
	private RenderTexture baseTexture;

	// Token: 0x04000AF8 RID: 2808
	private RenderTexture blurredTexture;

	// Token: 0x04000AF9 RID: 2809
	private Camera frozenCamera;

	// Token: 0x04000AFA RID: 2810
	private MeshRenderer renderer;

	// Token: 0x04000AFB RID: 2811
	private Material activeBlurMat;

	// Token: 0x04000AFC RID: 2812
	private float lastBlurT;

	// Token: 0x04000AFD RID: 2813
	private int unfreezeQueueFrames;

	// Token: 0x04000AFE RID: 2814
	private static readonly int _mainTexId = Shader.PropertyToID("_MainTex");

	// Token: 0x04000AFF RID: 2815
	private static readonly int _sizeProp = Shader.PropertyToID("_Size");
}
