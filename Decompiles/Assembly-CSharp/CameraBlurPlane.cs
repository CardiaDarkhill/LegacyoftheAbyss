using System;
using UnityEngine;

// Token: 0x0200021F RID: 543
public class CameraBlurPlane : MonoBehaviour
{
	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600142F RID: 5167 RVA: 0x0005B057 File Offset: 0x00059257
	// (set) Token: 0x06001430 RID: 5168 RVA: 0x0005B075 File Offset: 0x00059275
	public static float Spacing
	{
		get
		{
			if (!CameraBlurPlane._instance)
			{
				return 0f;
			}
			return CameraBlurPlane._instance.lastSetSpacing;
		}
		set
		{
			if (CameraBlurPlane._instance)
			{
				CameraBlurPlane._instance.SetSpacingInternal(value);
			}
		}
	}

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x06001431 RID: 5169 RVA: 0x0005B08E File Offset: 0x0005928E
	// (set) Token: 0x06001432 RID: 5170 RVA: 0x0005B0AC File Offset: 0x000592AC
	public static float Vibrancy
	{
		get
		{
			if (!CameraBlurPlane._instance)
			{
				return 0f;
			}
			return CameraBlurPlane._instance.lastSetVibrancy;
		}
		set
		{
			if (CameraBlurPlane._instance)
			{
				CameraBlurPlane._instance.SetVibrancyInternal(value);
			}
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x06001433 RID: 5171 RVA: 0x0005B0C5 File Offset: 0x000592C5
	// (set) Token: 0x06001434 RID: 5172 RVA: 0x0005B0E3 File Offset: 0x000592E3
	public static float MaskLerp
	{
		get
		{
			if (!CameraBlurPlane._instance)
			{
				return 0f;
			}
			return CameraBlurPlane._instance.lastSetMaskLerp;
		}
		set
		{
			if (CameraBlurPlane._instance)
			{
				CameraBlurPlane._instance.SetMaskLerpInternal(value);
			}
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x06001435 RID: 5173 RVA: 0x0005B0FC File Offset: 0x000592FC
	// (set) Token: 0x06001436 RID: 5174 RVA: 0x0005B11A File Offset: 0x0005931A
	public static float MaskScale
	{
		get
		{
			if (!CameraBlurPlane._instance)
			{
				return 0f;
			}
			return CameraBlurPlane._instance.lastSetMaskScale;
		}
		set
		{
			if (CameraBlurPlane._instance)
			{
				CameraBlurPlane._instance.SetMaskScale(value, ForceCameraAspect.CurrentViewportAspect);
			}
		}
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x0005B138 File Offset: 0x00059338
	private void Awake()
	{
		if (!CameraBlurPlane._instance)
		{
			CameraBlurPlane._instance = this;
		}
		this.camera = base.GetComponent<Camera>();
		this.blurMaterial = new Material(this.material);
		this.SetMaskScale(1f, ForceCameraAspect.CurrentViewportAspect);
		base.enabled = false;
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0005B18B File Offset: 0x0005938B
	private void OnDestroy()
	{
		if (CameraBlurPlane._instance == this)
		{
			CameraBlurPlane._instance = null;
		}
		if (this.blurMaterial != null)
		{
			Object.Destroy(this.blurMaterial);
			this.blurMaterial = null;
		}
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0005B1C0 File Offset: 0x000593C0
	private void OnEnable()
	{
		this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0005B1DE File Offset: 0x000593DE
	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0005B1F1 File Offset: 0x000593F1
	private void Update()
	{
		if (this.lastSetMaskLerp >= 0.0001f)
		{
			this.SetMaskOffset();
		}
	}

	// Token: 0x0600143C RID: 5180 RVA: 0x0005B206 File Offset: 0x00059406
	private void SetSpacingInternal(float value)
	{
		value *= 0.1f;
		this.SetMaterialFloatValue(CameraBlurPlane._sizeProp, value);
		this.lastSetSpacing = value;
		this.UpdateActiveState();
	}

	// Token: 0x0600143D RID: 5181 RVA: 0x0005B22A File Offset: 0x0005942A
	private void SetVibrancyInternal(float value)
	{
		this.SetMaterialFloatValue(CameraBlurPlane._vibrancyProp, value);
		this.lastSetVibrancy = value;
		this.UpdateActiveState();
	}

	// Token: 0x0600143E RID: 5182 RVA: 0x0005B248 File Offset: 0x00059448
	private void SetMaskLerpInternal(float value)
	{
		this.SetMaterialFloatValue(CameraBlurPlane._maskLerpProp, value);
		if (this.lastSetMaskLerp > 0.0001f)
		{
			if (value <= 0.0001f)
			{
				this.blurMaterial.DisableKeyword("USE_MASK");
			}
		}
		else if (value > 0.0001f)
		{
			this.blurMaterial.EnableKeyword("USE_MASK");
		}
		this.lastSetMaskLerp = value;
		this.UpdateActiveState();
	}

	// Token: 0x0600143F RID: 5183 RVA: 0x0005B2AD File Offset: 0x000594AD
	private void SetMaterialFloatValue(int propId, float value)
	{
		if (!this.blurMaterial)
		{
			return;
		}
		this.blurMaterial.SetFloat(propId, value);
	}

	// Token: 0x06001440 RID: 5184 RVA: 0x0005B2CA File Offset: 0x000594CA
	private void UpdateActiveState()
	{
		base.enabled = (this.lastSetVibrancy > 0.0001f || this.lastSetSpacing > 0.0001f);
	}

	// Token: 0x06001441 RID: 5185 RVA: 0x0005B2F0 File Offset: 0x000594F0
	private void SetMaskScale(float scale, float cameraAspect)
	{
		this.maskTiling = new Vector2(cameraAspect, 1f) / scale;
		this.baseMaskOffset = new Vector2((1f - this.maskTiling.x) / 2f, (1f - this.maskTiling.y) / 2f);
		this.SetMaskOffset();
		this.lastSetMaskScale = scale;
	}

	// Token: 0x06001442 RID: 5186 RVA: 0x0005B35C File Offset: 0x0005955C
	private void SetMaskOffset()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		Vector3 position = silentInstance.transform.position;
		Vector2 a = this.camera.WorldToViewportPoint(position) - new Vector2(0.5f, 0.5f);
		Vector2 vector = this.baseMaskOffset - a / 2f;
		this.blurMaterial.SetVector(CameraBlurPlane._maskStProp, new Vector4(this.maskTiling.x, this.maskTiling.y, vector.x, vector.y));
	}

	// Token: 0x06001443 RID: 5187 RVA: 0x0005B3F8 File Offset: 0x000595F8
	private void OnCameraAspectChanged(float aspect)
	{
		this.SetMaskScale(this.lastSetMaskScale, aspect);
	}

	// Token: 0x06001444 RID: 5188 RVA: 0x0005B408 File Offset: 0x00059608
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, temporary, this.blurMaterial, 0);
		bool flag = false;
		for (int i = 1; i < 4; i++)
		{
			if (flag)
			{
				Graphics.Blit(destination, temporary, this.blurMaterial, i);
			}
			else
			{
				Graphics.Blit(temporary, destination, this.blurMaterial, i);
			}
			flag = !flag;
		}
		Graphics.Blit(flag ? destination : temporary, flag ? temporary : destination, this.blurMaterial, 4);
		if (flag)
		{
			Graphics.Blit(temporary, destination);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}

	// Token: 0x04001270 RID: 4720
	private const int PASSES = 5;

	// Token: 0x04001271 RID: 4721
	[SerializeField]
	private Material material;

	// Token: 0x04001272 RID: 4722
	private static CameraBlurPlane _instance;

	// Token: 0x04001273 RID: 4723
	private Material blurMaterial;

	// Token: 0x04001274 RID: 4724
	private Camera camera;

	// Token: 0x04001275 RID: 4725
	private float lastSetSpacing;

	// Token: 0x04001276 RID: 4726
	private float lastSetVibrancy;

	// Token: 0x04001277 RID: 4727
	private float lastSetMaskLerp;

	// Token: 0x04001278 RID: 4728
	private float lastSetMaskScale;

	// Token: 0x04001279 RID: 4729
	private Vector2 maskTiling;

	// Token: 0x0400127A RID: 4730
	private Vector2 baseMaskOffset;

	// Token: 0x0400127B RID: 4731
	private const float MIN_SNAP = 0.0001f;

	// Token: 0x0400127C RID: 4732
	private static readonly int _maskStProp = Shader.PropertyToID("_Mask_ST");

	// Token: 0x0400127D RID: 4733
	private static readonly int _sizeProp = Shader.PropertyToID("_Size");

	// Token: 0x0400127E RID: 4734
	private static readonly int _vibrancyProp = Shader.PropertyToID("_Vibrancy");

	// Token: 0x0400127F RID: 4735
	private static readonly int _maskLerpProp = Shader.PropertyToID("_MaskLerp");
}
