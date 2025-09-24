using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000237 RID: 567
[Serializable]
public class GradeMarker : MonoBehaviour
{
	// Token: 0x17000246 RID: 582
	// (get) Token: 0x060014BE RID: 5310 RVA: 0x0005D631 File Offset: 0x0005B831
	// (set) Token: 0x060014BF RID: 5311 RVA: 0x0005D639 File Offset: 0x0005B839
	public float Alpha
	{
		get
		{
			return this.alpha;
		}
		set
		{
			if (Math.Abs(this.alpha - value) <= Mathf.Epsilon)
			{
				return;
			}
			this.alpha = value;
			if (this.scm)
			{
				this.scm.UpdateScript(false);
			}
		}
	}

	// Token: 0x060014C0 RID: 5312 RVA: 0x0005D670 File Offset: 0x0005B870
	private bool IsUsingMapZone()
	{
		return this.useMapZone.IsEnabled && this.useMapZone.Value > MapZone.NONE;
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x0005D68F File Offset: 0x0005B88F
	protected void OnEnable()
	{
		GradeMarker._activeMarkers.Add(this);
		this.gm = GameManager.instance;
		if (this.gm != null)
		{
			this.gm.NextSceneWillActivate += this.OnUnloadingLevel;
		}
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x0005D6CC File Offset: 0x0005B8CC
	protected void OnDisable()
	{
		GradeMarker._activeMarkers.Remove(this);
		if (GradeMarker._previousClosest == this)
		{
			GradeMarker._previousClosest = null;
		}
		if (this.gm != null)
		{
			this.gm.NextSceneWillActivate -= this.OnUnloadingLevel;
		}
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x0005D720 File Offset: 0x0005B920
	private void Start()
	{
		this.gc = GameCameras.instance;
		this.scm = this.gc.sceneColorManager;
		this.hero = HeroController.instance;
		if (this.IsUsingMapZone())
		{
			PlayerData instance = PlayerData.instance;
			AsyncOperationHandle<References> handle = Addressables.LoadAssetAsync<References>("ReferencesData");
			References references = handle.WaitForCompletion();
			if (references && references.sceneDefaultSettings)
			{
				this.CopyLightingFromMapZone(references.sceneDefaultSettings.GetMapZoneSettingsRuntime(this.useMapZone.Value, instance.blackThreadWorld ? SceneManagerSettings.Conditions.BlackThread : SceneManagerSettings.Conditions.None));
			}
			else
			{
				Debug.LogError("Couldn't load SceneDefaultSettings", this);
			}
			Addressables.Release<References>(handle);
		}
		if (this.enableGrade)
		{
			this.Activate();
		}
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x0005D7D3 File Offset: 0x0005B9D3
	private void OnUnloadingLevel()
	{
		this.Deactivate();
		base.enabled = false;
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x0005D7E4 File Offset: 0x0005B9E4
	public void SetStartSizeForTrigger()
	{
		this.origCutoffRadius = this.cutoffRadius;
		this.origMaxIntensityRadius = this.maxIntensityRadius;
		this.cutoffRadius = this.origCutoffRadius * (this.shrunkPercentage / 100f);
		this.maxIntensityRadius = this.origMaxIntensityRadius * (this.shrunkPercentage / 100f);
		this.startCutoffRadius = this.cutoffRadius;
		this.startMaxIntensityRadius = this.maxIntensityRadius;
	}

	// Token: 0x060014C6 RID: 5318 RVA: 0x0005D854 File Offset: 0x0005BA54
	public void Activate()
	{
		this.heading = this.hero.transform.position - base.transform.position;
		this.sqrNear = this.maxIntensityRadius * this.maxIntensityRadius;
		this.sqrFar = this.cutoffRadius * this.cutoffRadius;
		this.sqrEffectRange = this.sqrFar - this.sqrNear;
		this.u = (this.heading.sqrMagnitude - this.sqrNear) / this.sqrEffectRange;
		this.t = Mathf.Clamp01(1f - this.u);
		if (GradeMarker._previousClosest == this)
		{
			this.SetScmParams();
		}
		this.enableGrade = true;
		this.scm.SetMarkerActive(true);
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x0005D924 File Offset: 0x0005BB24
	private void SetScmParams()
	{
		this.scm.SaturationB = CustomSceneManager.AdjustSaturationForPlatform(this.saturation, null);
		this.scm.RedB = this.redChannel;
		this.scm.GreenB = this.greenChannel;
		this.scm.BlueB = this.blueChannel;
		this.scm.AmbientColorB = this.ambientColor;
		this.scm.AmbientIntensityB = this.ambientIntensity;
		if (GameManager.instance.IsGameplayScene())
		{
			this.scm.HeroLightColorB = this.heroLightColor;
		}
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x0005D9C2 File Offset: 0x0005BBC2
	public void Deactivate()
	{
		this.enableGrade = false;
		if (this.scm)
		{
			this.scm.SetMarkerActive(false);
			this.scm.SetFactor(0f);
			this.scm.UpdateScript(false);
		}
	}

	// Token: 0x060014C9 RID: 5321 RVA: 0x0005DA00 File Offset: 0x0005BC00
	public void ActivateGradual()
	{
		this.startCutoffRadius = this.cutoffRadius;
		this.startMaxIntensityRadius = this.maxIntensityRadius;
		this.finalCutoffRadius = this.origCutoffRadius;
		this.finalMaxIntensityRadius = this.origMaxIntensityRadius;
		this.cutoffRadius = this.startCutoffRadius;
		this.maxIntensityRadius = this.startMaxIntensityRadius;
		this.Activate();
		this.activating = true;
		this.deactivating = false;
		this.easeTimer = 0f;
	}

	// Token: 0x060014CA RID: 5322 RVA: 0x0005DA74 File Offset: 0x0005BC74
	public void DeactivateGradual()
	{
		this.startCutoffRadius = this.cutoffRadius;
		this.startMaxIntensityRadius = this.maxIntensityRadius;
		this.finalCutoffRadius = this.cutoffRadius * (this.shrunkPercentage / 100f);
		this.finalMaxIntensityRadius = this.maxIntensityRadius * (this.shrunkPercentage / 100f);
		this.activating = false;
		this.deactivating = true;
		this.easeTimer = 0f;
	}

	// Token: 0x060014CB RID: 5323 RVA: 0x0005DAE4 File Offset: 0x0005BCE4
	private void Update()
	{
		if (!this.enableGrade)
		{
			return;
		}
		if (Time.frameCount % 2 == 0)
		{
			Vector2 a = this.hero.transform.position;
			float num = float.MaxValue;
			GradeMarker gradeMarker = null;
			foreach (GradeMarker gradeMarker2 in GradeMarker._activeMarkers)
			{
				Vector2 b = gradeMarker2.transform.position;
				float num2 = Vector2.Distance(a, b) - gradeMarker2.cutoffRadius;
				if (gradeMarker != null)
				{
					if (gradeMarker2.priority < gradeMarker.priority && (num <= 0f || num2 > 0f))
					{
						continue;
					}
					if (num2 <= 0f && gradeMarker2.priority > gradeMarker.priority)
					{
						num = float.MaxValue;
						gradeMarker = null;
					}
				}
				if (num2 <= num)
				{
					num = num2;
					gradeMarker = gradeMarker2;
				}
			}
			if (gradeMarker == this)
			{
				if (GradeMarker._previousClosest != this)
				{
					GradeMarker._previousClosest = this;
					this.SetScmParams();
					this.scm.UpdateScript(true);
				}
				this.UpdateScm();
			}
		}
		if (this.easeTimer < this.EaseDuration)
		{
			this.easeTimer += Time.deltaTime;
			float num3 = this.easeTimer / this.EaseDuration;
			this.maxIntensityRadius = Mathf.Lerp(this.startMaxIntensityRadius, this.finalMaxIntensityRadius, num3);
			this.cutoffRadius = Mathf.Lerp(this.startCutoffRadius, this.finalCutoffRadius, num3);
			if (this.activating)
			{
				if (this.easeTimer >= this.EaseDuration)
				{
					this.activating = false;
				}
			}
			else if (this.deactivating && this.easeTimer >= this.EaseDuration)
			{
				this.deactivating = false;
				this.enableGrade = false;
			}
			if (this.easeTimer > this.EaseDuration)
			{
				this.easeTimer = this.EaseDuration;
			}
		}
	}

	// Token: 0x060014CC RID: 5324 RVA: 0x0005DCD8 File Offset: 0x0005BED8
	public void UpdateScm()
	{
		this.heading = this.hero.transform.position - base.transform.position;
		this.sqrNear = this.maxIntensityRadius * this.maxIntensityRadius;
		this.sqrFar = this.cutoffRadius * this.cutoffRadius;
		this.sqrEffectRange = this.sqrFar - this.sqrNear;
		this.u = (this.heading.sqrMagnitude - this.sqrNear) / this.sqrEffectRange;
		this.t = Mathf.Clamp01(1f - this.u);
		if (this.scm.StartBufferActive)
		{
			this.scm.SetMarkerActive(true);
			this.SetFactor(this.t);
			return;
		}
		bool markerActive = this.scm.MarkerActive;
		if (this.u < 0f)
		{
			this.scm.SetMarkerActive(false);
			this.SetFactor(1f);
		}
		else if (this.u < 1.1f)
		{
			this.scm.SetMarkerActive(true);
			this.SetFactor(this.t);
		}
		else
		{
			this.scm.SetMarkerActive(false);
			this.SetFactor(0f);
		}
		if (markerActive != this.scm.MarkerActive)
		{
			this.scm.UpdateScript(false);
		}
	}

	// Token: 0x060014CD RID: 5325 RVA: 0x0005DE2D File Offset: 0x0005C02D
	private void SetFactor(float newT)
	{
		this.scm.SetFactor(newT * this.alpha);
	}

	// Token: 0x060014CE RID: 5326 RVA: 0x0005DE44 File Offset: 0x0005C044
	public void CopyLightingFromMapZone(SceneManagerSettings storedSettings)
	{
		this.saturation = storedSettings.saturation;
		this.redChannel = new AnimationCurve(storedSettings.redChannel.keys);
		this.greenChannel = new AnimationCurve(storedSettings.greenChannel.keys);
		this.blueChannel = new AnimationCurve(storedSettings.blueChannel.keys);
		this.ambientColor = storedSettings.defaultColor;
		this.ambientIntensity = storedSettings.defaultIntensity;
		this.heroLightColor = storedSettings.heroLightColor;
	}

	// Token: 0x04001321 RID: 4897
	public bool enableGrade = true;

	// Token: 0x04001322 RID: 4898
	private bool activating;

	// Token: 0x04001323 RID: 4899
	private bool deactivating;

	// Token: 0x04001324 RID: 4900
	[SerializeField]
	[Range(0f, 1f)]
	private float alpha = 1f;

	// Token: 0x04001325 RID: 4901
	[SerializeField]
	private OverrideMapZone useMapZone;

	// Token: 0x04001326 RID: 4902
	[Header("Range")]
	public float maxIntensityRadius;

	// Token: 0x04001327 RID: 4903
	public float cutoffRadius;

	// Token: 0x04001328 RID: 4904
	public int priority;

	// Token: 0x04001329 RID: 4905
	[Header("Target Color Grade")]
	[Range(0f, 5f)]
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public float saturation;

	// Token: 0x0400132A RID: 4906
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public AnimationCurve redChannel;

	// Token: 0x0400132B RID: 4907
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public AnimationCurve greenChannel;

	// Token: 0x0400132C RID: 4908
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public AnimationCurve blueChannel;

	// Token: 0x0400132D RID: 4909
	[Header("Target Scene Lighting")]
	[Range(0f, 1f)]
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public float ambientIntensity;

	// Token: 0x0400132E RID: 4910
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public Color ambientColor;

	// Token: 0x0400132F RID: 4911
	[Header("Target Hero Light")]
	[ModifiableProperty]
	[Conditional("IsUsingMapZone", false, true, false)]
	public Color heroLightColor;

	// Token: 0x04001330 RID: 4912
	private GameManager gm;

	// Token: 0x04001331 RID: 4913
	private GameCameras gc;

	// Token: 0x04001332 RID: 4914
	private HeroController hero;

	// Token: 0x04001333 RID: 4915
	private SceneColorManager scm;

	// Token: 0x04001334 RID: 4916
	private Vector2 heading;

	// Token: 0x04001335 RID: 4917
	private float sqrNear;

	// Token: 0x04001336 RID: 4918
	private float sqrFar;

	// Token: 0x04001337 RID: 4919
	private float sqrEffectRange;

	// Token: 0x04001338 RID: 4920
	private float t;

	// Token: 0x04001339 RID: 4921
	private float u;

	// Token: 0x0400133A RID: 4922
	private float origMaxIntensityRadius;

	// Token: 0x0400133B RID: 4923
	private float origCutoffRadius;

	// Token: 0x0400133C RID: 4924
	private float startMaxIntensityRadius;

	// Token: 0x0400133D RID: 4925
	private float startCutoffRadius;

	// Token: 0x0400133E RID: 4926
	private float finalMaxIntensityRadius;

	// Token: 0x0400133F RID: 4927
	private float finalCutoffRadius;

	// Token: 0x04001340 RID: 4928
	private float shrunkPercentage = 30f;

	// Token: 0x04001341 RID: 4929
	[NonSerialized]
	public float EaseDuration = 1.5f;

	// Token: 0x04001342 RID: 4930
	private float easeTimer = 2f;

	// Token: 0x04001343 RID: 4931
	private static readonly List<GradeMarker> _activeMarkers = new List<GradeMarker>();

	// Token: 0x04001344 RID: 4932
	private static GradeMarker _previousClosest;
}
