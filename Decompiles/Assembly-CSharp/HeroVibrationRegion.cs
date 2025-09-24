using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020007A3 RID: 1955
public sealed class HeroVibrationRegion : MonoBehaviour
{
	// Token: 0x140000EB RID: 235
	// (add) Token: 0x06004509 RID: 17673 RVA: 0x0012DB88 File Offset: 0x0012BD88
	// (remove) Token: 0x0600450A RID: 17674 RVA: 0x0012DBC0 File Offset: 0x0012BDC0
	public event HeroVibrationRegion.EmissionEvent MainEmissionStarted;

	// Token: 0x0600450B RID: 17675 RVA: 0x0012DBF8 File Offset: 0x0012BDF8
	private void Awake()
	{
		if (this.heroDetector)
		{
			this.heroDetector.OnTriggerEntered += this.OnTriggerEntered;
			this.heroDetector.OnTriggerExited += this.OnTriggerExited;
			if (this.vibrationCentre == null)
			{
				this.vibrationCentre = this.heroDetector.transform;
			}
		}
		if (this.vibrationCentre == null)
		{
			this.vibrationCentre = base.transform;
		}
		if (this.distanceScaleMode == HeroVibrationRegion.ScaleType.None)
		{
			this.strength = this.strengthRange.GetLerpedValue(1f);
			return;
		}
		this.strength = this.strengthRange.GetLerpedValue(0f);
	}

	// Token: 0x0600450C RID: 17676 RVA: 0x0012DCB0 File Offset: 0x0012BEB0
	private void OnDisable()
	{
		foreach (HeroVibrationRegion.EmissionTracker emissionTracker in this.emissions)
		{
			emissionTracker.emission.Stop();
		}
		this.emissions.Clear();
	}

	// Token: 0x0600450D RID: 17677 RVA: 0x0012DD10 File Offset: 0x0012BF10
	private void Update()
	{
		bool flag = this.StrengthUpdate();
		if (this.EmissionsUpdate())
		{
			flag = true;
		}
		if (!flag)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600450E RID: 17678 RVA: 0x0012DD38 File Offset: 0x0012BF38
	private bool ShowRadial()
	{
		return this.distanceScaleMode == HeroVibrationRegion.ScaleType.Radial;
	}

	// Token: 0x0600450F RID: 17679 RVA: 0x0012DD43 File Offset: 0x0012BF43
	private bool ShowRectangle()
	{
		return this.distanceScaleMode == HeroVibrationRegion.ScaleType.Rectangle;
	}

	// Token: 0x06004510 RID: 17680 RVA: 0x0012DD50 File Offset: 0x0012BF50
	private void OnTriggerEntered(Collider2D collider, GameObject sender)
	{
		if (!this.IsCorrectTarget(collider))
		{
			return;
		}
		if (!this.isInside)
		{
			this.isInside = true;
			if (this.distanceScaleMode == HeroVibrationRegion.ScaleType.None)
			{
				this.strength = this.strengthRange.GetLerpedValue(1f);
				this.SetEmissionsStrength(this.strength);
			}
			if (this.loopRequested || this.playStopMode.HasFlag(HeroVibrationRegion.PlayStop.PlayOnEnter))
			{
				this.StartVibration();
			}
			if (this.distanceScaleMode != HeroVibrationRegion.ScaleType.None)
			{
				this.StartStrengthUpdate();
			}
		}
	}

	// Token: 0x06004511 RID: 17681 RVA: 0x0012DDD4 File Offset: 0x0012BFD4
	private void OnTriggerExited(Collider2D collider, GameObject sender)
	{
		if (!this.IsCorrectTarget(collider))
		{
			return;
		}
		this.isInside = false;
		if (this.playStopMode.HasFlag(HeroVibrationRegion.PlayStop.StopOnExit))
		{
			this.StopVibration();
		}
		else if (this.requireInside)
		{
			this.StopMainVibration();
		}
		this.StopEmissions(HeroVibrationRegion.VibrationSettings.StopOnExit);
		if (this.distanceScaleMode == HeroVibrationRegion.ScaleType.None)
		{
			this.strength = this.strengthRange.GetLerpedValue(0f);
			this.SetEmissionsStrength(this.strength);
		}
		this.StopStrengthUpdate();
	}

	// Token: 0x06004512 RID: 17682 RVA: 0x0012DE57 File Offset: 0x0012C057
	private bool IsCorrectTarget(Collider2D collider2D)
	{
		return collider2D.CompareTag("Player");
	}

	// Token: 0x06004513 RID: 17683 RVA: 0x0012DE64 File Offset: 0x0012C064
	public void StartVibration()
	{
		if (this.loop)
		{
			this.loopRequested = true;
		}
		if (this.requireInside && !this.isInside)
		{
			return;
		}
		VibrationEmission emission;
		if (this.loop)
		{
			if (this.mainEmissionLoop != null && this.mainEmissionLoop.IsPlaying && this.mainEmissionLoop.IsLooping)
			{
				return;
			}
			VibrationData vibrationData = this.vibrationDataAsset;
			bool isLooping = this.loop;
			bool isRealtime = this.isRealTime;
			this.mainEmissionLoop = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, "", isRealtime);
			VibrationEmission vibrationEmission = this.mainEmissionLoop;
			if (vibrationEmission != null)
			{
				vibrationEmission.SetStrength(this.strength * this.vibrationDataStrength);
			}
			emission = this.mainEmissionLoop;
		}
		else
		{
			HeroVibrationRegion.VibrationSettings vibrationSettings = HeroVibrationRegion.VibrationSettings.ControlledByRegion;
			if (this.isRealTime)
			{
				vibrationSettings |= HeroVibrationRegion.VibrationSettings.RealTime;
			}
			if (this.playStopMode.HasFlag(HeroVibrationRegion.PlayStop.StopOnExit))
			{
				vibrationSettings |= HeroVibrationRegion.VibrationSettings.StopOnExit;
			}
			emission = this.AddEmission(this.vibrationDataAsset, this.vibrationDataStrength, vibrationSettings, null);
		}
		HeroVibrationRegion.EmissionEvent mainEmissionStarted = this.MainEmissionStarted;
		if (mainEmissionStarted == null)
		{
			return;
		}
		mainEmissionStarted(emission);
	}

	// Token: 0x06004514 RID: 17684 RVA: 0x0012DF74 File Offset: 0x0012C174
	public void StopMainVibration()
	{
		if (this.mainEmissionLoop != null)
		{
			this.mainEmissionLoop.Stop();
			this.mainEmissionLoop = null;
		}
	}

	// Token: 0x06004515 RID: 17685 RVA: 0x0012DF90 File Offset: 0x0012C190
	public void StopVibration()
	{
		this.loopRequested = false;
		this.StopMainVibration();
		this.StopEmissions(HeroVibrationRegion.VibrationSettings.ControlledByRegion);
	}

	// Token: 0x06004516 RID: 17686 RVA: 0x0012DFA8 File Offset: 0x0012C1A8
	private void StartStrengthUpdate()
	{
		this.scale = this.GetScaleSetting();
		HeroController instance = HeroController.instance;
		if (instance)
		{
			this.heroTransform = instance.transform;
			this.doStrengthUpdate = true;
			base.enabled = true;
		}
	}

	// Token: 0x06004517 RID: 17687 RVA: 0x0012DFE9 File Offset: 0x0012C1E9
	private void StopStrengthUpdate()
	{
		if (this.doStrengthUpdate)
		{
			this.doStrengthUpdate = false;
			this.strength = this.strengthRange.GetLerpedValue(0f);
		}
	}

	// Token: 0x06004518 RID: 17688 RVA: 0x0012E010 File Offset: 0x0012C210
	private bool StrengthUpdate()
	{
		if (this.doStrengthUpdate)
		{
			float intensity = this.scale.GetIntensity(this.heroTransform.position, this.vibrationCentre.position);
			this.strength = this.strengthRange.GetLerpedValue(this.strengthCurve.Evaluate(intensity));
			this.SetEmissionsStrength(this.strength);
			return true;
		}
		return false;
	}

	// Token: 0x06004519 RID: 17689 RVA: 0x0012E073 File Offset: 0x0012C273
	private void StartEmissionsUpdate()
	{
		if (this.emissions.Count > 0)
		{
			base.enabled = true;
		}
	}

	// Token: 0x0600451A RID: 17690 RVA: 0x0012E08C File Offset: 0x0012C28C
	private bool EmissionsUpdate()
	{
		if (this.emissions.Count > 0)
		{
			for (int i = this.emissions.Count - 1; i >= 0; i--)
			{
				if (!this.emissions[i].emission.IsPlaying)
				{
					this.emissions.RemoveAt(i);
				}
			}
		}
		bool flag = this.mainEmissionLoop != null;
		if (flag && !this.mainEmissionLoop.IsPlaying)
		{
			this.mainEmissionLoop = null;
			flag = false;
		}
		return flag || this.emissions.Count > 0;
	}

	// Token: 0x0600451B RID: 17691 RVA: 0x0012E11C File Offset: 0x0012C31C
	private void SetEmissionsStrength(float value)
	{
		if (this.onlyFeltInsideRegion && !this.isInside)
		{
			value = 0f;
		}
		VibrationEmission vibrationEmission = this.mainEmissionLoop;
		if (vibrationEmission != null)
		{
			vibrationEmission.SetStrength(this.vibrationDataStrength * value);
		}
		foreach (HeroVibrationRegion.EmissionTracker emissionTracker in this.emissions)
		{
			emissionTracker.SetStrength(value);
		}
	}

	// Token: 0x0600451C RID: 17692 RVA: 0x0012E1A0 File Offset: 0x0012C3A0
	private HeroVibrationRegion.ScaleSettings GetScaleSetting()
	{
		HeroVibrationRegion.ScaleType scaleType = this.distanceScaleMode;
		HeroVibrationRegion.ScaleSettings result;
		if (scaleType != HeroVibrationRegion.ScaleType.Radial)
		{
			if (scaleType != HeroVibrationRegion.ScaleType.Rectangle)
			{
				result = this.radialScaleSettings;
			}
			else
			{
				result = this.rectangleScaleSettings;
			}
		}
		else
		{
			result = this.radialScaleSettings;
		}
		return result;
	}

	// Token: 0x0600451D RID: 17693 RVA: 0x0012E1D8 File Offset: 0x0012C3D8
	public VibrationEmission PlayVibrationOneShot(VibrationData vibrationData, bool requireInside, HeroVibrationRegion.VibrationSettings vibrationSettings = HeroVibrationRegion.VibrationSettings.None, string tag = null)
	{
		if (requireInside && !this.isInside)
		{
			return null;
		}
		return this.AddEmission(vibrationData, vibrationSettings, tag);
	}

	// Token: 0x0600451E RID: 17694 RVA: 0x0012E1F1 File Offset: 0x0012C3F1
	private VibrationEmission AddEmission(VibrationData vibrationData, HeroVibrationRegion.VibrationSettings vibrationSettings = HeroVibrationRegion.VibrationSettings.None, string tag = null)
	{
		return this.AddEmission(vibrationData, 1f, vibrationSettings, tag);
	}

	// Token: 0x0600451F RID: 17695 RVA: 0x0012E204 File Offset: 0x0012C404
	private VibrationEmission AddEmission(VibrationData vibrationData, float baseStrength, HeroVibrationRegion.VibrationSettings vibrationSettings = HeroVibrationRegion.VibrationSettings.None, string tag = null)
	{
		bool flag = vibrationSettings.HasFlag(HeroVibrationRegion.VibrationSettings.Loop);
		bool flag2 = vibrationSettings.HasFlag(HeroVibrationRegion.VibrationSettings.RealTime);
		bool isLooping = flag;
		bool isRealtime = flag2;
		VibrationEmission vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, tag, isRealtime);
		if (vibrationEmission != null)
		{
			float num = this.strength * baseStrength;
			if (this.onlyFeltInsideRegion && !this.isInside)
			{
				num = 0f;
			}
			vibrationEmission.SetStrength(num);
			this.emissions.Add(new HeroVibrationRegion.EmissionTracker(vibrationEmission, vibrationSettings, baseStrength));
			this.StartEmissionsUpdate();
		}
		return vibrationEmission;
	}

	// Token: 0x06004520 RID: 17696 RVA: 0x0012E29C File Offset: 0x0012C49C
	private void StopEmissions(HeroVibrationRegion.VibrationSettings settings)
	{
		if (settings == HeroVibrationRegion.VibrationSettings.None)
		{
			for (int i = this.emissions.Count - 1; i >= 0; i--)
			{
				this.emissions[i].emission.Stop();
			}
			this.emissions.Clear();
			return;
		}
		for (int j = this.emissions.Count - 1; j >= 0; j--)
		{
			HeroVibrationRegion.EmissionTracker emissionTracker = this.emissions[j];
			if ((emissionTracker.settings & settings) != HeroVibrationRegion.VibrationSettings.None)
			{
				emissionTracker.emission.Stop();
				this.emissions.RemoveAt(j);
			}
		}
	}

	// Token: 0x06004521 RID: 17697 RVA: 0x0012E32C File Offset: 0x0012C52C
	private void DrawGizmos()
	{
		Transform transform = this.vibrationCentre;
		if (!transform)
		{
			transform = (this.heroDetector ? this.heroDetector.transform : base.transform);
		}
		HeroVibrationRegion.ScaleType scaleType = this.distanceScaleMode;
		if (scaleType == HeroVibrationRegion.ScaleType.Radial)
		{
			Vector3 position = transform.position;
			Gizmos.color = Color.yellow.SetAlpha(0.5f);
			Gizmos.DrawWireSphere(position, this.radialScaleSettings.minIntensityRadius);
			Gizmos.color = Color.green.SetAlpha(0.5f);
			Gizmos.DrawWireSphere(position, this.radialScaleSettings.maxIntensityRadius);
			return;
		}
		if (scaleType != HeroVibrationRegion.ScaleType.Rectangle)
		{
			return;
		}
		Vector3 position2 = transform.position;
		Gizmos.color = Color.yellow.SetAlpha(0.5f);
		Gizmos.DrawWireCube(position2, new Vector3(this.rectangleScaleSettings.minIntensityWidth, this.rectangleScaleSettings.minIntensityHeight));
		Gizmos.color = Color.green.SetAlpha(0.5f);
		Gizmos.DrawWireCube(position2, new Vector3(this.rectangleScaleSettings.maxIntensityWidth, this.rectangleScaleSettings.maxIntensityHeight));
	}

	// Token: 0x06004522 RID: 17698 RVA: 0x0012E438 File Offset: 0x0012C638
	private void OnDrawGizmos()
	{
		if (this.alwaysDrawGizmos || GizmoUtility.IsChildSelected(base.transform))
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x0012E455 File Offset: 0x0012C655
	private void OnDrawGizmosSelected()
	{
		if (!this.alwaysDrawGizmos)
		{
			this.DrawGizmos();
		}
	}

	// Token: 0x040045E7 RID: 17895
	[Header("Detection Settings")]
	[SerializeField]
	private TriggerEnterEvent heroDetector;

	// Token: 0x040045E8 RID: 17896
	[Header("Vibration Configuration")]
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x040045E9 RID: 17897
	[SerializeField]
	private float vibrationDataStrength = 1f;

	// Token: 0x040045EA RID: 17898
	[SerializeField]
	private HeroVibrationRegion.PlayStop playStopMode;

	// Token: 0x040045EB RID: 17899
	[SerializeField]
	private bool loop;

	// Token: 0x040045EC RID: 17900
	[SerializeField]
	private bool isRealTime;

	// Token: 0x040045ED RID: 17901
	[Header("Scaling and Strength Settings")]
	[SerializeField]
	private HeroVibrationRegion.ScaleType distanceScaleMode;

	// Token: 0x040045EE RID: 17902
	[SerializeField]
	private Transform vibrationCentre;

	// Token: 0x040045EF RID: 17903
	[SerializeField]
	private MinMaxFloat strengthRange = new MinMaxFloat(0f, 1f);

	// Token: 0x040045F0 RID: 17904
	[SerializeField]
	private AnimationCurve strengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040045F1 RID: 17905
	[ModifiableProperty]
	[Conditional("ShowRadial", true, true, true)]
	[SerializeField]
	private HeroVibrationRegion.RadialScaleSettings radialScaleSettings;

	// Token: 0x040045F2 RID: 17906
	[ModifiableProperty]
	[Conditional("ShowRectangle", true, true, true)]
	[SerializeField]
	private HeroVibrationRegion.RectangleScaleSettings rectangleScaleSettings;

	// Token: 0x040045F3 RID: 17907
	[Header("Additional Settings")]
	[Tooltip("Requires hero inside region to start vibration")]
	[SerializeField]
	private bool requireInside;

	// Token: 0x040045F4 RID: 17908
	[SerializeField]
	private bool onlyFeltInsideRegion;

	// Token: 0x040045F5 RID: 17909
	[Header("Debug")]
	[SerializeField]
	private bool alwaysDrawGizmos;

	// Token: 0x040045F7 RID: 17911
	private bool isInside;

	// Token: 0x040045F8 RID: 17912
	private VibrationEmission mainEmissionLoop;

	// Token: 0x040045F9 RID: 17913
	private float strength = 1f;

	// Token: 0x040045FA RID: 17914
	private bool loopRequested;

	// Token: 0x040045FB RID: 17915
	private List<HeroVibrationRegion.EmissionTracker> emissions = new List<HeroVibrationRegion.EmissionTracker>();

	// Token: 0x040045FC RID: 17916
	private bool doStrengthUpdate;

	// Token: 0x040045FD RID: 17917
	private Transform heroTransform;

	// Token: 0x040045FE RID: 17918
	private HeroVibrationRegion.ScaleSettings scale;

	// Token: 0x02001A7A RID: 6778
	// (Invoke) Token: 0x0600970E RID: 38670
	public delegate void EmissionEvent(VibrationEmission emission);

	// Token: 0x02001A7B RID: 6779
	[Serializable]
	private enum ScaleType
	{
		// Token: 0x0400999F RID: 39327
		None,
		// Token: 0x040099A0 RID: 39328
		Radial,
		// Token: 0x040099A1 RID: 39329
		Rectangle
	}

	// Token: 0x02001A7C RID: 6780
	[Flags]
	[Serializable]
	private enum PlayStop
	{
		// Token: 0x040099A3 RID: 39331
		Manual = 0,
		// Token: 0x040099A4 RID: 39332
		PlayOnEnter = 1,
		// Token: 0x040099A5 RID: 39333
		StopOnExit = 2
	}

	// Token: 0x02001A7D RID: 6781
	[Flags]
	[Serializable]
	public enum VibrationSettings
	{
		// Token: 0x040099A7 RID: 39335
		None = 0,
		// Token: 0x040099A8 RID: 39336
		Loop = 1,
		// Token: 0x040099A9 RID: 39337
		RealTime = 2,
		// Token: 0x040099AA RID: 39338
		StopOnExit = 4,
		// Token: 0x040099AB RID: 39339
		ControlledByRegion = 8
	}

	// Token: 0x02001A7E RID: 6782
	private struct EmissionTracker
	{
		// Token: 0x17001128 RID: 4392
		// (get) Token: 0x06009711 RID: 38673 RVA: 0x002A9673 File Offset: 0x002A7873
		public bool StopOnExit
		{
			get
			{
				return this.settings.HasFlag(HeroVibrationRegion.VibrationSettings.StopOnExit);
			}
		}

		// Token: 0x17001129 RID: 4393
		// (get) Token: 0x06009712 RID: 38674 RVA: 0x002A968B File Offset: 0x002A788B
		public bool ControlledByRegion
		{
			get
			{
				return this.settings.HasFlag(HeroVibrationRegion.VibrationSettings.ControlledByRegion);
			}
		}

		// Token: 0x06009713 RID: 38675 RVA: 0x002A96A3 File Offset: 0x002A78A3
		public EmissionTracker(VibrationEmission emission, HeroVibrationRegion.VibrationSettings settings, float strength)
		{
			this.emission = emission;
			this.settings = settings;
			this.strength = strength;
		}

		// Token: 0x06009714 RID: 38676 RVA: 0x002A96BA File Offset: 0x002A78BA
		public void SetStrength(float strength)
		{
			VibrationEmission vibrationEmission = this.emission;
			if (vibrationEmission == null)
			{
				return;
			}
			vibrationEmission.SetStrength(this.strength * strength);
		}

		// Token: 0x040099AC RID: 39340
		public VibrationEmission emission;

		// Token: 0x040099AD RID: 39341
		public HeroVibrationRegion.VibrationSettings settings;

		// Token: 0x040099AE RID: 39342
		public readonly float strength;
	}

	// Token: 0x02001A7F RID: 6783
	private abstract class ScaleSettings
	{
		// Token: 0x06009715 RID: 38677
		public abstract float GetIntensity(Vector3 position, Vector3 centerPoint);
	}

	// Token: 0x02001A80 RID: 6784
	[Serializable]
	private class RadialScaleSettings : HeroVibrationRegion.ScaleSettings
	{
		// Token: 0x06009717 RID: 38679 RVA: 0x002A96DC File Offset: 0x002A78DC
		public RadialScaleSettings(float minIntensityRadius, float maxIntensityRadius)
		{
			this.minIntensityRadius = minIntensityRadius;
			this.maxIntensityRadius = maxIntensityRadius;
		}

		// Token: 0x06009718 RID: 38680 RVA: 0x002A96F4 File Offset: 0x002A78F4
		public override float GetIntensity(Vector3 position, Vector3 centerPoint)
		{
			float num = Vector3.Distance(position, centerPoint);
			if (num <= this.maxIntensityRadius)
			{
				return 1f;
			}
			if (num >= this.minIntensityRadius)
			{
				return 0f;
			}
			return Mathf.Lerp(1f, 0f, (num - this.maxIntensityRadius) / (this.minIntensityRadius - this.maxIntensityRadius));
		}

		// Token: 0x040099AF RID: 39343
		public float minIntensityRadius;

		// Token: 0x040099B0 RID: 39344
		public float maxIntensityRadius;
	}

	// Token: 0x02001A81 RID: 6785
	[Serializable]
	private class RectangleScaleSettings : HeroVibrationRegion.ScaleSettings
	{
		// Token: 0x06009719 RID: 38681 RVA: 0x002A974C File Offset: 0x002A794C
		public RectangleScaleSettings(float min, float max)
		{
			this.minIntensityWidth = min;
			this.maxIntensityWidth = max;
			this.minIntensityHeight = min;
			this.maxIntensityHeight = max;
		}

		// Token: 0x0600971A RID: 38682 RVA: 0x002A9770 File Offset: 0x002A7970
		public RectangleScaleSettings(float minIntensityWidth, float maxIntensityWidth, float minIntensityHeight, float maxIntensityHeight)
		{
			this.minIntensityWidth = minIntensityWidth;
			this.maxIntensityWidth = maxIntensityWidth;
			this.minIntensityHeight = minIntensityHeight;
			this.maxIntensityHeight = maxIntensityHeight;
		}

		// Token: 0x0600971B RID: 38683 RVA: 0x002A9798 File Offset: 0x002A7998
		public override float GetIntensity(Vector3 position, Vector3 centerPoint)
		{
			float num = Mathf.Abs(position.x - centerPoint.x);
			float num2 = Mathf.Abs(position.y - centerPoint.y);
			float a = (num <= this.maxIntensityWidth / 2f) ? 1f : Mathf.InverseLerp(this.minIntensityWidth / 2f, this.maxIntensityWidth / 2f, num);
			float b = (num2 <= this.maxIntensityHeight / 2f) ? 1f : Mathf.InverseLerp(this.minIntensityHeight / 2f, this.maxIntensityHeight / 2f, num2);
			return Mathf.Min(a, b);
		}

		// Token: 0x040099B1 RID: 39345
		public float minIntensityWidth;

		// Token: 0x040099B2 RID: 39346
		public float maxIntensityWidth;

		// Token: 0x040099B3 RID: 39347
		public float minIntensityHeight;

		// Token: 0x040099B4 RID: 39348
		public float maxIntensityHeight;
	}
}
