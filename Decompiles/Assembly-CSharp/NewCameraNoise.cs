using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200016D RID: 365
public sealed class NewCameraNoise : MonoBehaviour, IPostprocessModule
{
	// Token: 0x17000109 RID: 265
	// (get) Token: 0x06000B93 RID: 2963 RVA: 0x000350F1 File Offset: 0x000332F1
	// (set) Token: 0x06000B94 RID: 2964 RVA: 0x000350F9 File Offset: 0x000332F9
	public float Density
	{
		get
		{
			return this.density;
		}
		set
		{
			this.density = value;
			if (this.AutoUpdateTexture)
			{
				this.UpdateDensity();
			}
		}
	}

	// Token: 0x1700010A RID: 266
	// (get) Token: 0x06000B95 RID: 2965 RVA: 0x00035110 File Offset: 0x00033310
	// (set) Token: 0x06000B96 RID: 2966 RVA: 0x00035118 File Offset: 0x00033318
	public Vector2 DensityRatio
	{
		get
		{
			return this.densityRatio;
		}
		set
		{
			this.densityRatio = value;
			if (this.AutoUpdateTexture)
			{
				this.UpdateDensity();
			}
		}
	}

	// Token: 0x1700010B RID: 267
	// (get) Token: 0x06000B97 RID: 2967 RVA: 0x0003512F File Offset: 0x0003332F
	// (set) Token: 0x06000B98 RID: 2968 RVA: 0x00035137 File Offset: 0x00033337
	public float NoiseStrength
	{
		get
		{
			return this.noiseStrength;
		}
		set
		{
			this.noiseStrength = value;
		}
	}

	// Token: 0x1700010C RID: 268
	// (get) Token: 0x06000B99 RID: 2969 RVA: 0x00035140 File Offset: 0x00033340
	// (set) Token: 0x06000B9A RID: 2970 RVA: 0x00035148 File Offset: 0x00033348
	public float TimeSnap
	{
		get
		{
			return this.timeSnap;
		}
		set
		{
			this.timeSnap = value;
			if (this.timeSnap <= 0f)
			{
				this.timeSnap = 0.0033333334f;
			}
		}
	}

	// Token: 0x1700010D RID: 269
	// (get) Token: 0x06000B9B RID: 2971 RVA: 0x00035169 File Offset: 0x00033369
	// (set) Token: 0x06000B9C RID: 2972 RVA: 0x00035176 File Offset: 0x00033376
	public float R
	{
		get
		{
			return this.colorScale.x;
		}
		set
		{
			this.colorScale.x = Mathf.Max(0f, value);
		}
	}

	// Token: 0x1700010E RID: 270
	// (get) Token: 0x06000B9D RID: 2973 RVA: 0x0003518E File Offset: 0x0003338E
	// (set) Token: 0x06000B9E RID: 2974 RVA: 0x0003519B File Offset: 0x0003339B
	public float G
	{
		get
		{
			return this.colorScale.y;
		}
		set
		{
			this.colorScale.y = Mathf.Max(0f, value);
		}
	}

	// Token: 0x1700010F RID: 271
	// (get) Token: 0x06000B9F RID: 2975 RVA: 0x000351B3 File Offset: 0x000333B3
	// (set) Token: 0x06000BA0 RID: 2976 RVA: 0x000351C0 File Offset: 0x000333C0
	public float B
	{
		get
		{
			return this.colorScale.z;
		}
		set
		{
			this.colorScale.z = Mathf.Max(0f, value);
		}
	}

	// Token: 0x17000110 RID: 272
	// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x000351D8 File Offset: 0x000333D8
	// (set) Token: 0x06000BA2 RID: 2978 RVA: 0x000351E0 File Offset: 0x000333E0
	public bool AutoUpdateTexture { get; set; } = true;

	// Token: 0x17000111 RID: 273
	// (get) Token: 0x06000BA3 RID: 2979 RVA: 0x000351E9 File Offset: 0x000333E9
	// (set) Token: 0x06000BA4 RID: 2980 RVA: 0x00035210 File Offset: 0x00033410
	public static NewCameraNoise Instance
	{
		get
		{
			if (!NewCameraNoise.HasInstance)
			{
				NewCameraNoise.instance = Object.FindObjectOfType<NewCameraNoise>();
				NewCameraNoise.HasInstance = NewCameraNoise.instance;
			}
			return NewCameraNoise.instance;
		}
		private set
		{
			NewCameraNoise.instance = value;
			NewCameraNoise.HasInstance = NewCameraNoise.instance;
		}
	}

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x06000BA5 RID: 2981 RVA: 0x00035227 File Offset: 0x00033427
	// (set) Token: 0x06000BA6 RID: 2982 RVA: 0x0003522E File Offset: 0x0003342E
	public static bool HasInstance { get; private set; }

	// Token: 0x06000BA7 RID: 2983 RVA: 0x00035236 File Offset: 0x00033436
	private void Awake()
	{
		NewCameraNoise.Instance = this;
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x00035240 File Offset: 0x00033440
	private void Start()
	{
		this.isStarting = true;
		this.Density = this.density;
		this.DensityRatio = this.densityRatio;
		this.NoiseStrength = this.noiseStrength;
		this.TimeSnap = this.timeSnap;
		this.isStarting = false;
		this.UpdateDensity();
		if (!NewCameraNoise.IsEnabledOnPlatform() || (FastNoise.Instance && FastNoise.Instance.enabled))
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x000352B7 File Offset: 0x000334B7
	private void OnDestroy()
	{
		if (NewCameraNoise.Instance == this)
		{
			NewCameraNoise.Instance = null;
		}
		if (this.noiseTexture != null)
		{
			Object.Destroy(this.noiseTexture);
		}
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x000352E5 File Offset: 0x000334E5
	private static bool IsEnabledOnPlatform()
	{
		return true;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x000352E8 File Offset: 0x000334E8
	private Color GenerateColorPreview(Vector4 scale)
	{
		float num = Mathf.Max(new float[]
		{
			scale.x,
			scale.y,
			scale.z
		});
		float value = scale.x / num;
		float num2 = scale.y / num;
		float num3 = scale.z / num;
		float num4 = scale.w;
		float r = Mathf.Clamp(value, 0f, 1f);
		num2 = Mathf.Clamp(num2, 0f, 1f);
		num3 = Mathf.Clamp(num3, 0f, 1f);
		num4 = Mathf.Clamp(num4, 0f, 1f);
		return new Color(r, num2, num3, num4);
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x00035387 File Offset: 0x00033587
	public Color GetNoiseColor()
	{
		return this.GenerateColorPreview(this.colorScale);
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x00035398 File Offset: 0x00033598
	private Texture2D GenerateNoiseTexture(int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.name = string.Format("{0} NoiseTexture", this);
		Vector2 vector = this.densityRatio * this.density;
		if (vector.x > 0f)
		{
			vector.x = 1920f / vector.x;
		}
		else
		{
			vector.x = 0f;
		}
		if (vector.y > 0f)
		{
			vector.y = 1080f / vector.y;
		}
		else
		{
			vector.y = 0f;
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				float x = (float)i / (float)width * vector.x;
				float y = (float)j / (float)height * vector.y;
				float num = Mathf.PerlinNoise(x, y);
				texture2D.SetPixel(i, j, new Color(num, num, num, 1f));
			}
		}
		texture2D.wrapMode = TextureWrapMode.Repeat;
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x00035494 File Offset: 0x00033694
	private void UpdateDensity()
	{
		if (this.isStarting)
		{
			return;
		}
		this.UpdateNoiseTexture();
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x000354A5 File Offset: 0x000336A5
	public void UpdateNoiseTexture()
	{
		if (this.noiseTexture != null)
		{
			Object.Destroy(this.noiseTexture);
		}
		this.noiseTexture = this.GenerateNoiseTexture(this.noiseWidth, this.noiseHeight);
	}

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06000BB0 RID: 2992 RVA: 0x000354D8 File Offset: 0x000336D8
	public string EffectKeyword
	{
		get
		{
			return "NOISE_ENABLED";
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x000354E0 File Offset: 0x000336E0
	public void UpdateProperties(Material material)
	{
		material.SetFloat(NewCameraNoise.NOISE_STRENGTH, this.noiseStrength);
		material.SetFloat(NewCameraNoise.TIME_SNAP, this.timeSnap);
		material.SetTexture(NewCameraNoise.NOISE_TEXTURE, this.noiseTexture);
		material.SetVector(NewCameraNoise.NOISE_COLOR, this.colorScale);
	}

	// Token: 0x04000B37 RID: 2871
	[SerializeField]
	private Material material;

	// Token: 0x04000B38 RID: 2872
	[Space]
	[SerializeField]
	private float density = 6.4f;

	// Token: 0x04000B39 RID: 2873
	[SerializeField]
	private Vector2 densityRatio = new Vector2(1f, 1f);

	// Token: 0x04000B3A RID: 2874
	[SerializeField]
	private float noiseStrength = 0.25f;

	// Token: 0x04000B3B RID: 2875
	[SerializeField]
	private float timeSnap = 0.016666668f;

	// Token: 0x04000B3C RID: 2876
	[SerializeField]
	private Vector4 colorScale = new Vector4(1f, 1f, 1f, 1f);

	// Token: 0x04000B3D RID: 2877
	[SerializeField]
	private Color colorPreview;

	// Token: 0x04000B3E RID: 2878
	[Header("Noise Texture")]
	[SerializeField]
	private int noiseWidth = 1024;

	// Token: 0x04000B3F RID: 2879
	[SerializeField]
	private int noiseHeight = 1024;

	// Token: 0x04000B40 RID: 2880
	[Header("Debug")]
	[SerializeField]
	private Texture noiseTexture;

	// Token: 0x04000B42 RID: 2882
	private static NewCameraNoise instance;

	// Token: 0x04000B43 RID: 2883
	private static readonly int NOISE_STRENGTH = Shader.PropertyToID("_NoiseStrength");

	// Token: 0x04000B44 RID: 2884
	private static readonly int TIME_SNAP = Shader.PropertyToID("_TimeSnap");

	// Token: 0x04000B45 RID: 2885
	private static readonly int NOISE_TEXTURE = Shader.PropertyToID("_NoiseTex");

	// Token: 0x04000B46 RID: 2886
	private static readonly int NOISE_COLOR = Shader.PropertyToID("_NoiseColor");

	// Token: 0x04000B48 RID: 2888
	private bool isStarting;
}
