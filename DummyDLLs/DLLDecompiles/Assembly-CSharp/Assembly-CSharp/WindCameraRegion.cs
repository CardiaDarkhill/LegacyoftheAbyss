using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000F3 RID: 243
public class WindCameraRegion : TrackTriggerObjects, ICameraShake
{
	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x060007AF RID: 1967 RVA: 0x00025231 File Offset: 0x00023431
	public bool CanFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x060007B0 RID: 1968 RVA: 0x00025234 File Offset: 0x00023434
	public int FreezeFrames
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x060007B1 RID: 1969 RVA: 0x00025237 File Offset: 0x00023437
	public CameraShakeWorldForceIntensities WorldForceOnStart
	{
		get
		{
			return CameraShakeWorldForceIntensities.None;
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002523A File Offset: 0x0002343A
	public Vector2 GetOffset(float elapsedTime)
	{
		if (!this.sourceSway)
		{
			return Vector2.zero;
		}
		return this.sourceSway.GetOffset(elapsedTime) * this.insideMagnitude;
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x060007B3 RID: 1971 RVA: 0x00025266 File Offset: 0x00023466
	public float Magnitude
	{
		get
		{
			if (!this.sourceSway)
			{
				return 0f;
			}
			return this.sourceSway.Magnitude;
		}
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00025286 File Offset: 0x00023486
	public bool IsDone(float elapsedTime)
	{
		return !this || (this.insideMagnitudeFadeRoutine == null && this.insideMagnitude <= 0.001f);
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x060007B5 RID: 1973 RVA: 0x000252AC File Offset: 0x000234AC
	public ICameraShakeVibration CameraShakeVibration
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x000252B0 File Offset: 0x000234B0
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (this.wasInside == isInside)
		{
			return;
		}
		this.wasInside = isInside;
		if (isInside)
		{
			WindCameraRegion._insideRegions++;
		}
		else
		{
			WindCameraRegion._insideRegions--;
		}
		if (WindCameraRegion._insideRegions == 1)
		{
			this.SetWindy(true);
			return;
		}
		if (WindCameraRegion._insideRegions == 0)
		{
			this.SetWindy(false);
		}
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0002530C File Offset: 0x0002350C
	private void SetWindy(bool value)
	{
		this.cameraManager.CancelShake(this);
		if (this.insideMagnitudeFadeRoutine != null)
		{
			base.StopCoroutine(this.insideMagnitudeFadeRoutine);
		}
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.insideMagnitudeFadeRoutine = base.StartCoroutine(this.FadeRoutine(value ? 1f : 0f));
		this.cameraManager.DoShake(this, this, true, true, true);
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x00025373 File Offset: 0x00023573
	private IEnumerator FadeRoutine(float newMagnitude)
	{
		float initialMagnitude = this.insideMagnitude;
		for (float elapsed = 0f; elapsed < this.fadeTime; elapsed += Time.deltaTime)
		{
			this.insideMagnitude = Mathf.Lerp(initialMagnitude, newMagnitude, elapsed / this.fadeTime);
			yield return null;
		}
		this.insideMagnitude = newMagnitude;
		this.insideMagnitudeFadeRoutine = null;
		yield break;
	}

	// Token: 0x0400077F RID: 1919
	[SerializeField]
	private CameraManagerReference cameraManager;

	// Token: 0x04000780 RID: 1920
	[SerializeField]
	private CameraShakeProfile sourceSway;

	// Token: 0x04000781 RID: 1921
	[Space]
	[SerializeField]
	private float fadeTime;

	// Token: 0x04000782 RID: 1922
	private static int _insideRegions;

	// Token: 0x04000783 RID: 1923
	private bool wasInside;

	// Token: 0x04000784 RID: 1924
	private float insideMagnitude;

	// Token: 0x04000785 RID: 1925
	private Coroutine insideMagnitudeFadeRoutine;
}
