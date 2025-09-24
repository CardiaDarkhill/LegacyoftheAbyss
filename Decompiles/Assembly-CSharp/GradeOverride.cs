using System;
using UnityEngine;

// Token: 0x02000238 RID: 568
public class GradeOverride : MonoBehaviour
{
	// Token: 0x060014D1 RID: 5329 RVA: 0x0005DF0A File Offset: 0x0005C10A
	private void Start()
	{
		this.gc = GameCameras.instance;
		this.hero = HeroController.instance;
	}

	// Token: 0x060014D2 RID: 5330 RVA: 0x0005DF22 File Offset: 0x0005C122
	private void OnEnable()
	{
		base.Invoke("Activate", 0.1f);
	}

	// Token: 0x060014D3 RID: 5331 RVA: 0x0005DF34 File Offset: 0x0005C134
	private void OnDisable()
	{
		this.Deactivate();
	}

	// Token: 0x060014D4 RID: 5332 RVA: 0x0005DF3C File Offset: 0x0005C13C
	public void Activate()
	{
		this.gc = GameCameras.instance;
		this.scm = this.gc.sceneColorManager;
		this.hero = HeroController.instance;
		this.o_saturation = this.scm.SaturationA;
		this.o_redChannel = this.scm.RedA;
		this.o_greenChannel = this.scm.GreenA;
		this.o_blueChannel = this.scm.BlueA;
		this.o_ambientIntensity = this.scm.AmbientIntensityA;
		this.o_ambientColor = this.scm.AmbientColorA;
		this.scm.SaturationA = CustomSceneManager.AdjustSaturationForPlatform(this.saturation, null);
		this.scm.RedA = this.redChannel;
		this.scm.GreenA = this.greenChannel;
		this.scm.BlueA = this.blueChannel;
		this.scm.AmbientColorA = this.ambientColor;
		this.scm.AmbientIntensityA = this.ambientIntensity;
		CustomSceneManager.SetLighting(this.ambientColor, this.ambientIntensity);
		if (GameManager.instance.IsGameplayScene())
		{
			this.o_heroLightColor = this.scm.HeroLightColorA;
			this.scm.HeroLightColorA = this.heroLightColor;
		}
		this.scm.SetMarkerActive(true);
		base.StartCoroutine(this.scm.ForceRefresh());
	}

	// Token: 0x060014D5 RID: 5333 RVA: 0x0005E0A8 File Offset: 0x0005C2A8
	public void Deactivate()
	{
		this.scm.SaturationA = this.o_saturation;
		this.scm.RedA = this.o_redChannel;
		this.scm.GreenA = this.o_greenChannel;
		this.scm.BlueA = this.o_blueChannel;
		this.scm.AmbientColorA = this.o_ambientColor;
		this.scm.AmbientIntensityA = this.o_ambientIntensity;
		CustomSceneManager.SetLighting(this.o_ambientColor, this.o_ambientIntensity);
		if (GameManager.instance != null && GameManager.instance.IsGameplayScene())
		{
			this.scm.HeroLightColorA = this.o_heroLightColor;
		}
	}

	// Token: 0x04001345 RID: 4933
	[Header("Overriding Color Grade")]
	[Range(0f, 5f)]
	public float saturation;

	// Token: 0x04001346 RID: 4934
	public AnimationCurve redChannel;

	// Token: 0x04001347 RID: 4935
	public AnimationCurve greenChannel;

	// Token: 0x04001348 RID: 4936
	public AnimationCurve blueChannel;

	// Token: 0x04001349 RID: 4937
	[Header("Overriding Scene Lighting")]
	[Range(0f, 1f)]
	public float ambientIntensity;

	// Token: 0x0400134A RID: 4938
	public Color ambientColor;

	// Token: 0x0400134B RID: 4939
	[Header("Overriding Hero Light")]
	public Color heroLightColor;

	// Token: 0x0400134C RID: 4940
	private float o_saturation;

	// Token: 0x0400134D RID: 4941
	private AnimationCurve o_redChannel;

	// Token: 0x0400134E RID: 4942
	private AnimationCurve o_greenChannel;

	// Token: 0x0400134F RID: 4943
	private AnimationCurve o_blueChannel;

	// Token: 0x04001350 RID: 4944
	private float o_ambientIntensity;

	// Token: 0x04001351 RID: 4945
	private Color o_ambientColor;

	// Token: 0x04001352 RID: 4946
	private Color o_heroLightColor;

	// Token: 0x04001353 RID: 4947
	private GameCameras gc;

	// Token: 0x04001354 RID: 4948
	private HeroController hero;

	// Token: 0x04001355 RID: 4949
	private SceneColorManager scm;
}
