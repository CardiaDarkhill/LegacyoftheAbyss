using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200021A RID: 538
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Bloom and Glow/Bloom (Optimized)")]
public class BloomOptimized : PostEffectsBase
{
	// Token: 0x17000230 RID: 560
	// (get) Token: 0x060013EA RID: 5098 RVA: 0x0005A571 File Offset: 0x00058771
	// (set) Token: 0x060013EB RID: 5099 RVA: 0x0005A579 File Offset: 0x00058779
	public float Threshold
	{
		get
		{
			return this.threshold;
		}
		set
		{
			this.threshold = value;
			this.previousThreshold = value;
			this.currentThreshold = value;
		}
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x060013EC RID: 5100 RVA: 0x0005A590 File Offset: 0x00058790
	// (set) Token: 0x060013ED RID: 5101 RVA: 0x0005A598 File Offset: 0x00058798
	public float Intensity
	{
		get
		{
			return this.intensity;
		}
		set
		{
			this.intensity = value;
			this.previousIntensity = value;
			this.currentIntensity = value;
		}
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x060013EE RID: 5102 RVA: 0x0005A5AF File Offset: 0x000587AF
	// (set) Token: 0x060013EF RID: 5103 RVA: 0x0005A5B7 File Offset: 0x000587B7
	public float BlurSize
	{
		get
		{
			return this.blurSize;
		}
		set
		{
			this.blurSize = value;
			this.previousBlurSize = value;
			this.currentBlurSize = value;
		}
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x060013F0 RID: 5104 RVA: 0x0005A5CE File Offset: 0x000587CE
	// (set) Token: 0x060013F1 RID: 5105 RVA: 0x0005A5D6 File Offset: 0x000587D6
	public float BlurShape
	{
		get
		{
			return this.blurShape;
		}
		set
		{
			this.blurShape = value;
		}
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x060013F2 RID: 5106 RVA: 0x0005A5DF File Offset: 0x000587DF
	// (set) Token: 0x060013F3 RID: 5107 RVA: 0x0005A5E7 File Offset: 0x000587E7
	public int BlurIterations
	{
		get
		{
			return this.blurIterations;
		}
		set
		{
			this.blurIterations = value;
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060013F4 RID: 5108 RVA: 0x0005A5F0 File Offset: 0x000587F0
	// (set) Token: 0x060013F5 RID: 5109 RVA: 0x0005A5F8 File Offset: 0x000587F8
	public BloomOptimized.BlurTypes BlurType
	{
		get
		{
			return this.blurType;
		}
		set
		{
			this.blurType = value;
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0005A601 File Offset: 0x00058801
	// (set) Token: 0x060013F7 RID: 5111 RVA: 0x0005A609 File Offset: 0x00058809
	public float InitialIntensity { get; private set; }

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060013F8 RID: 5112 RVA: 0x0005A612 File Offset: 0x00058812
	// (set) Token: 0x060013F9 RID: 5113 RVA: 0x0005A61A File Offset: 0x0005881A
	public float InitialThreshold { get; private set; }

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060013FA RID: 5114 RVA: 0x0005A623 File Offset: 0x00058823
	// (set) Token: 0x060013FB RID: 5115 RVA: 0x0005A62B File Offset: 0x0005882B
	public float InitialBlurSize { get; private set; }

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060013FC RID: 5116 RVA: 0x0005A634 File Offset: 0x00058834
	// (set) Token: 0x060013FD RID: 5117 RVA: 0x0005A63C File Offset: 0x0005883C
	public float InitialBlurShape { get; private set; }

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x060013FE RID: 5118 RVA: 0x0005A645 File Offset: 0x00058845
	// (set) Token: 0x060013FF RID: 5119 RVA: 0x0005A64D File Offset: 0x0005884D
	public int InitialIterations { get; private set; }

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001400 RID: 5120 RVA: 0x0005A656 File Offset: 0x00058856
	// (set) Token: 0x06001401 RID: 5121 RVA: 0x0005A65E File Offset: 0x0005885E
	public BloomOptimized.BlurTypes InitialBlurType { get; private set; }

	// Token: 0x06001402 RID: 5122 RVA: 0x0005A668 File Offset: 0x00058868
	private void OnValidate()
	{
		if (Math.Abs(this.previousThreshold - this.threshold) > 0.0001f)
		{
			this.previousThreshold = this.threshold;
			this.currentThreshold = this.threshold;
		}
		if (Math.Abs(this.previousIntensity - this.intensity) > 0.0001f)
		{
			this.previousIntensity = this.intensity;
			this.currentIntensity = this.intensity;
		}
		if (Math.Abs(this.previousBlurSize - this.blurSize) > 0.0001f)
		{
			this.previousBlurSize = this.blurSize;
			this.currentBlurSize = this.blurSize;
		}
	}

	// Token: 0x06001403 RID: 5123 RVA: 0x0005A708 File Offset: 0x00058908
	public override bool CheckResources()
	{
		base.CheckSupport(false);
		this.fastBloomMaterial = base.CheckShaderAndCreateMaterial(this.fastBloomShader, this.fastBloomMaterial);
		if (!this.isSupported)
		{
			base.ReportAutoDisable();
		}
		return this.isSupported;
	}

	// Token: 0x06001404 RID: 5124 RVA: 0x0005A740 File Offset: 0x00058940
	private void Awake()
	{
		if (this.blurShape < 0.2f)
		{
			this.blurShape = 0.5f;
		}
		this.InitialThreshold = this.threshold;
		this.InitialIntensity = this.intensity;
		this.InitialBlurSize = this.blurSize;
		this.InitialBlurShape = this.blurShape;
		this.InitialIterations = this.blurIterations;
		this.InitialBlurType = this.blurType;
	}

	// Token: 0x06001405 RID: 5125 RVA: 0x0005A7B0 File Offset: 0x000589B0
	protected override void OnEnable()
	{
		this.currentIntensity = this.intensity;
		this.currentThreshold = this.threshold;
		this.currentBlurSize = this.blurSize;
		base.OnEnable();
		this.effectIsSupported = this.CheckResources();
		this.resources = BloomOptimized.BloomResources.Create(32, 32, RenderTextureFormat.ARGB32, FilterMode.Bilinear);
	}

	// Token: 0x06001406 RID: 5126 RVA: 0x0005A804 File Offset: 0x00058A04
	private void OnDisable()
	{
		if (this.fastBloomMaterial)
		{
			Object.DestroyImmediate(this.fastBloomMaterial);
		}
		this.resources.Release();
		this.resources = null;
	}

	// Token: 0x06001407 RID: 5127 RVA: 0x0005A830 File Offset: 0x00058A30
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.effectIsSupported)
		{
			Graphics.Blit(source, destination);
			return;
		}
		int num = (this.resolution == BloomOptimized.Resolution.Low) ? 4 : 2;
		float num2 = (this.resolution == BloomOptimized.Resolution.Low) ? 0.5f : 1f;
		this.fastBloomMaterial.SetVector(BloomOptimized._parameterPropId, new Vector4(this.currentBlurSize * num2, 0f, this.currentThreshold, this.currentIntensity));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width / num;
		int height = source.height / num;
		this.resources.EnsureFormat(width, height, source.format, FilterMode.Bilinear);
		this.fastBloomMaterial.SetVector(BloomOptimized._targetTextelSizePropId, new Vector4(1f / (float)this.resources.BufferA.width, 1f / (float)this.resources.BufferA.height, (float)this.resources.BufferA.width, (float)this.resources.BufferA.height));
		Graphics.Blit(source, this.resources.BufferA, this.fastBloomMaterial, 1);
		int num3 = (this.blurType == BloomOptimized.BlurTypes.Standard) ? 0 : 2;
		for (int i = 0; i < this.blurIterations; i++)
		{
			float num4 = this.currentBlurSize * num2 + (float)i * 1f;
			this.fastBloomMaterial.SetVector(BloomOptimized._parameterPropId, new Vector4(num4 * this.blurShape, num4, this.currentThreshold, this.currentIntensity));
			Graphics.Blit(this.resources.BufferA, this.resources.BufferB, this.fastBloomMaterial, 2 + num3);
			Graphics.Blit(this.resources.BufferB, this.resources.BufferA, this.fastBloomMaterial, 3 + num3);
		}
		this.fastBloomMaterial.SetTexture(BloomOptimized._bloomPropId, this.resources.BufferA);
		Graphics.Blit(source, destination, this.fastBloomMaterial, 0);
		this.fastBloomMaterial.SetTexture(BloomOptimized._bloomPropId, null);
	}

	// Token: 0x06001408 RID: 5128 RVA: 0x0005AA34 File Offset: 0x00058C34
	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.insideRegions.AddIfNotPresent(region);
		this.UpdateValues(forceImmediate);
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0005AA4A File Offset: 0x00058C4A
	public void RemoveInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.insideRegions.Remove(region);
		this.UpdateValues(forceImmediate);
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0005AA60 File Offset: 0x00058C60
	private void UpdateValues(bool forceImmediate)
	{
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		float bloomIntensity;
		float bloomThreshold;
		float bloomBlurSize;
		if (this.insideRegions.Count == 0)
		{
			bloomIntensity = this.intensity;
			bloomThreshold = this.threshold;
			bloomBlurSize = this.blurSize;
		}
		else
		{
			SceneAppearanceRegion sceneAppearanceRegion = this.insideRegions.Last<SceneAppearanceRegion>();
			bloomIntensity = sceneAppearanceRegion.BloomIntensity;
			bloomThreshold = sceneAppearanceRegion.BloomThreshold;
			bloomBlurSize = sceneAppearanceRegion.BloomBlurSize;
			this.fadeDuration = sceneAppearanceRegion.FadeDuration;
		}
		if (this.fadeDuration > 0f && !forceImmediate)
		{
			this.fadeRoutine = base.StartCoroutine(this.FadeValues(bloomIntensity, bloomThreshold, bloomBlurSize));
			return;
		}
		this.currentIntensity = bloomIntensity;
		this.currentThreshold = bloomThreshold;
		this.currentBlurSize = bloomBlurSize;
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0005AB0D File Offset: 0x00058D0D
	private IEnumerator FadeValues(float targetIntensity, float targetThreshold, float targetBlurSize)
	{
		float startIntensity = this.currentIntensity;
		float startThreshold = this.currentThreshold;
		float startBlurSize = this.currentBlurSize;
		for (float elapsed = 0f; elapsed < this.fadeDuration; elapsed += Time.deltaTime)
		{
			float t = this.fadeCurve.Evaluate(elapsed / this.fadeDuration);
			this.currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
			this.currentThreshold = Mathf.Lerp(startThreshold, targetThreshold, t);
			this.currentBlurSize = Mathf.Lerp(startBlurSize, targetBlurSize, t);
			yield return null;
		}
		this.currentIntensity = targetIntensity;
		this.currentThreshold = targetThreshold;
		this.currentBlurSize = targetBlurSize;
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x0400123D RID: 4669
	[SerializeField]
	[Range(0f, 1.5f)]
	private float threshold = 0.25f;

	// Token: 0x0400123E RID: 4670
	private float previousThreshold;

	// Token: 0x0400123F RID: 4671
	[SerializeField]
	[Range(0f, 2.5f)]
	private float intensity = 0.75f;

	// Token: 0x04001240 RID: 4672
	private float previousIntensity;

	// Token: 0x04001241 RID: 4673
	[SerializeField]
	[Range(0.25f, 5.5f)]
	private float blurSize = 1f;

	// Token: 0x04001242 RID: 4674
	private float previousBlurSize;

	// Token: 0x04001243 RID: 4675
	[SerializeField]
	[Range(0.25f, 4f)]
	private float blurShape = 0.5f;

	// Token: 0x04001244 RID: 4676
	[SerializeField]
	[Range(1f, 4f)]
	private int blurIterations = 1;

	// Token: 0x04001245 RID: 4677
	[SerializeField]
	private BloomOptimized.BlurTypes blurType;

	// Token: 0x04001246 RID: 4678
	[SerializeField]
	private Shader fastBloomShader;

	// Token: 0x04001247 RID: 4679
	[Space]
	[SerializeField]
	private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04001248 RID: 4680
	private float currentIntensity;

	// Token: 0x04001249 RID: 4681
	private float currentThreshold;

	// Token: 0x0400124A RID: 4682
	private float currentBlurSize;

	// Token: 0x0400124B RID: 4683
	private float fadeDuration;

	// Token: 0x0400124C RID: 4684
	private Material fastBloomMaterial;

	// Token: 0x0400124D RID: 4685
	private BloomOptimized.Resolution resolution;

	// Token: 0x0400124E RID: 4686
	private readonly List<SceneAppearanceRegion> insideRegions = new List<SceneAppearanceRegion>();

	// Token: 0x0400124F RID: 4687
	private Coroutine fadeRoutine;

	// Token: 0x04001250 RID: 4688
	private static readonly int _parameterPropId = Shader.PropertyToID("_Parameter");

	// Token: 0x04001251 RID: 4689
	private static readonly int _bloomPropId = Shader.PropertyToID("_Bloom");

	// Token: 0x04001252 RID: 4690
	private static readonly int _targetTextelSizePropId = Shader.PropertyToID("_TargetTexelSize");

	// Token: 0x04001253 RID: 4691
	private bool effectIsSupported;

	// Token: 0x04001254 RID: 4692
	private BloomOptimized.BloomResources resources;

	// Token: 0x02001539 RID: 5433
	private enum Resolution
	{
		// Token: 0x0400865E RID: 34398
		Low,
		// Token: 0x0400865F RID: 34399
		High
	}

	// Token: 0x0200153A RID: 5434
	public enum BlurTypes
	{
		// Token: 0x04008661 RID: 34401
		Standard,
		// Token: 0x04008662 RID: 34402
		Sgx
	}

	// Token: 0x0200153B RID: 5435
	private class BloomResources
	{
		// Token: 0x17000D66 RID: 3430
		// (get) Token: 0x06008618 RID: 34328 RVA: 0x00271EF2 File Offset: 0x002700F2
		// (set) Token: 0x06008619 RID: 34329 RVA: 0x00271EFA File Offset: 0x002700FA
		public RenderTexture BufferA { get; private set; }

		// Token: 0x17000D67 RID: 3431
		// (get) Token: 0x0600861A RID: 34330 RVA: 0x00271F03 File Offset: 0x00270103
		// (set) Token: 0x0600861B RID: 34331 RVA: 0x00271F0B File Offset: 0x0027010B
		public RenderTexture BufferB { get; private set; }

		// Token: 0x17000D68 RID: 3432
		// (get) Token: 0x0600861C RID: 34332 RVA: 0x00271F14 File Offset: 0x00270114
		public bool IsCreated
		{
			get
			{
				return this.BufferA != null && this.BufferB != null;
			}
		}

		// Token: 0x0600861D RID: 34333 RVA: 0x00271F32 File Offset: 0x00270132
		private BloomResources()
		{
		}

		// Token: 0x0600861E RID: 34334 RVA: 0x00271F3A File Offset: 0x0027013A
		public static BloomOptimized.BloomResources Create(int width, int height, RenderTextureFormat format, FilterMode filterMode)
		{
			BloomOptimized.BloomResources bloomResources = new BloomOptimized.BloomResources();
			bloomResources.CreateBuffers(width, height, format, filterMode);
			return bloomResources;
		}

		// Token: 0x0600861F RID: 34335 RVA: 0x00271F4C File Offset: 0x0027014C
		public void Release()
		{
			if (this.BufferA != null)
			{
				this.BufferA.Release();
				this.BufferA = null;
			}
			if (this.BufferB != null)
			{
				this.BufferB.Release();
				this.BufferB = null;
			}
		}

		// Token: 0x06008620 RID: 34336 RVA: 0x00271F9C File Offset: 0x0027019C
		public void EnsureFormat(int width, int height, RenderTextureFormat format, FilterMode filterMode)
		{
			if (this.BufferA == null)
			{
				throw new InvalidOperationException("Can't use textures in non-created bloom resources");
			}
			if (this.BufferA.width != width || this.BufferA.height != height || this.BufferA.format != format || this.BufferA.filterMode != filterMode)
			{
				this.Release();
				this.CreateBuffers(width, height, format, filterMode);
			}
		}

		// Token: 0x06008621 RID: 34337 RVA: 0x0027200C File Offset: 0x0027020C
		private void CreateBuffers(int width, int height, RenderTextureFormat format, FilterMode filterMode)
		{
			this.BufferA = new RenderTexture(width, height, 0, format);
			this.BufferA.filterMode = filterMode;
			this.BufferB = new RenderTexture(width, height, 0, format);
			this.BufferB.filterMode = filterMode;
		}
	}
}
