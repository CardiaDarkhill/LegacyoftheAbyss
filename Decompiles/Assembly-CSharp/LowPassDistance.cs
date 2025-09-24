using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class LowPassDistance : MonoBehaviour
{
	// Token: 0x060008DC RID: 2268 RVA: 0x00029907 File Offset: 0x00027B07
	private void Reset()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00029915 File Offset: 0x00027B15
	private void OnEnable()
	{
		if (this.audioSource == null)
		{
			base.enabled = false;
		}
		if (this.lowPassFilter == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x00029941 File Offset: 0x00027B41
	private void Start()
	{
		this.camera = GameCameras.instance.mainCamera.transform;
		this.audioTrans = this.audioSource.transform;
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0002996C File Offset: 0x00027B6C
	private void LateUpdate()
	{
		if (!this.camera)
		{
			if (!GameCameras.instance)
			{
				return;
			}
			this.camera = GameCameras.instance.mainCamera.transform;
			if (!this.camera)
			{
				return;
			}
		}
		Vector3 position = this.camera.position;
		Vector3 position2 = this.audioTrans.position;
		float distance = Vector3.Distance(position, position2);
		float tonCurve = this.GetTOnCurve(AudioSourceCurveType.CustomRolloff, distance);
		float lerpedValue = this.mapToRange.GetLerpedValue(tonCurve);
		this.lowPassFilter.cutoffFrequency = lerpedValue;
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x000299F8 File Offset: 0x00027BF8
	private float GetTOnCurve(AudioSourceCurveType curveType, float distance)
	{
		AnimationCurve customCurve;
		if (!this.curveCache.TryGetValue(curveType, out customCurve))
		{
			customCurve = this.audioSource.GetCustomCurve(curveType);
			this.curveCache[curveType] = customCurve;
		}
		return customCurve.Evaluate(distance / this.audioSource.maxDistance);
	}

	// Token: 0x04000892 RID: 2194
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000893 RID: 2195
	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	// Token: 0x04000894 RID: 2196
	[SerializeField]
	private MinMaxFloat mapToRange = new MinMaxFloat(0f, 1f);

	// Token: 0x04000895 RID: 2197
	private Transform camera;

	// Token: 0x04000896 RID: 2198
	private Transform audioTrans;

	// Token: 0x04000897 RID: 2199
	private readonly Dictionary<AudioSourceCurveType, AnimationCurve> curveCache = new Dictionary<AudioSourceCurveType, AnimationCurve>();
}
