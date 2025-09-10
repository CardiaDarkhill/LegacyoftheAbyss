using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class AmbientFloat : MonoBehaviour
{
	// Token: 0x06000249 RID: 585 RVA: 0x0000E047 File Offset: 0x0000C247
	private bool IsInEditMode()
	{
		return !Application.isPlaying;
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600024A RID: 586 RVA: 0x0000E051 File Offset: 0x0000C251
	// (set) Token: 0x0600024B RID: 587 RVA: 0x0000E059 File Offset: 0x0000C259
	public float SpeedMultiplier
	{
		get
		{
			return this.speedMultiplier;
		}
		set
		{
			this.speedMultiplier = value;
		}
	}

	// Token: 0x0600024C RID: 588 RVA: 0x0000E062 File Offset: 0x0000C262
	private void Awake()
	{
		if (this.speedController)
		{
			this.speedController.SpeedChanged += delegate(float speed)
			{
				this.speedMultiplier = speed;
			};
		}
	}

	// Token: 0x0600024D RID: 589 RVA: 0x0000E088 File Offset: 0x0000C288
	private void Start()
	{
		this.initialPosition = base.transform.localPosition;
		this.targetPosition = this.initialPosition;
		this.timeOffset = Random.Range(-10f, 10f);
		if (this.useCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull += this.OnCameraPreCull;
			CameraRenderHooks.CameraPostRender += this.OnCameraPostRender;
		}
	}

	// Token: 0x0600024E RID: 590 RVA: 0x0000E0F1 File Offset: 0x0000C2F1
	private void OnDestroy()
	{
		if (this.useCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull -= this.OnCameraPreCull;
			CameraRenderHooks.CameraPostRender -= this.OnCameraPostRender;
		}
	}

	// Token: 0x0600024F RID: 591 RVA: 0x0000E11D File Offset: 0x0000C31D
	private void OnEnable()
	{
		if (!this.profile)
		{
			base.enabled = false;
		}
		this.updateOffset = Random.Range(0f, 1f / this.fpsLimit);
	}

	// Token: 0x06000250 RID: 592 RVA: 0x0000E150 File Offset: 0x0000C350
	private void Update()
	{
		if (this.speedMultiplier <= 0f)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		this.time += deltaTime * this.speedMultiplier;
		double num = Time.timeAsDouble + (double)this.updateOffset;
		if (this.fpsLimit > 0f)
		{
			if (num < this.nextUpdateTime)
			{
				return;
			}
			this.nextUpdateTime = num + (double)(1f / this.fpsLimit);
		}
		this.targetPosition = this.initialPosition + this.profile.GetOffset(this.time, this.timeOffset);
		if (!this.useCameraRenderHooks)
		{
			base.transform.localPosition = this.targetPosition;
		}
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0000E201 File Offset: 0x0000C401
	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != CameraRenderHooks.CameraSource.MainCamera || !base.isActiveAndEnabled)
		{
			return;
		}
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		this.preCullPosition = base.transform.localPosition;
		base.transform.localPosition = this.targetPosition;
	}

	// Token: 0x06000252 RID: 594 RVA: 0x0000E23F File Offset: 0x0000C43F
	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType != CameraRenderHooks.CameraSource.MainCamera || !base.isActiveAndEnabled)
		{
			return;
		}
		if (Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		base.transform.localPosition = this.preCullPosition;
	}

	// Token: 0x040001F3 RID: 499
	[SerializeField]
	private AmbientFloatProfile profile;

	// Token: 0x040001F4 RID: 500
	[SerializeField]
	private SpeedChanger speedController;

	// Token: 0x040001F5 RID: 501
	[SerializeField]
	private float speedMultiplier = 1f;

	// Token: 0x040001F6 RID: 502
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsInEditMode", true, true, false)]
	private bool useCameraRenderHooks;

	// Token: 0x040001F7 RID: 503
	[SerializeField]
	private float fpsLimit;

	// Token: 0x040001F8 RID: 504
	private float time;

	// Token: 0x040001F9 RID: 505
	private float updateOffset;

	// Token: 0x040001FA RID: 506
	private float timeOffset;

	// Token: 0x040001FB RID: 507
	private double nextUpdateTime;

	// Token: 0x040001FC RID: 508
	private Vector3 initialPosition;

	// Token: 0x040001FD RID: 509
	private Vector3 targetPosition;

	// Token: 0x040001FE RID: 510
	private Vector3 preCullPosition;
}
