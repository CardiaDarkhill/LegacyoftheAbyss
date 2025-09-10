using System;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class DriftflyCloud : MonoBehaviour
{
	// Token: 0x0600139F RID: 5023 RVA: 0x000595E9 File Offset: 0x000577E9
	private void Start()
	{
		this.gameCameraTransform = GameCameras.instance.mainCamera.gameObject.transform;
	}

	// Token: 0x060013A0 RID: 5024 RVA: 0x00059608 File Offset: 0x00057808
	private void Update()
	{
		float num = Vector3.Distance(base.transform.position, this.gameCameraTransform.position);
		if (num > 40f && this.isPlaying)
		{
			this.ptIdle.Stop();
			this.isPlaying = false;
			this.dispersed = false;
			return;
		}
		if (!this.isPlaying && !this.dispersed && num < 40f)
		{
			this.ptIdle.Play();
			this.isPlaying = true;
			return;
		}
		if (this.disperseRange.IsInside && !this.dispersed)
		{
			this.dispersed = true;
			this.ptIdle.Stop();
			this.ptDisperse.Play();
			this.timer = 5f;
			return;
		}
		if (this.disperseRange.IsInside)
		{
			this.timer = 5f;
			return;
		}
		if (this.timer <= 0f && this.dispersed)
		{
			this.ptIdle.Play();
			this.dispersed = false;
			return;
		}
		this.timer -= Time.deltaTime;
	}

	// Token: 0x04001201 RID: 4609
	public ParticleSystem ptIdle;

	// Token: 0x04001202 RID: 4610
	public ParticleSystem ptDisperse;

	// Token: 0x04001203 RID: 4611
	public TrackTriggerObjects disperseRange;

	// Token: 0x04001204 RID: 4612
	private bool isPlaying;

	// Token: 0x04001205 RID: 4613
	private bool dispersed;

	// Token: 0x04001206 RID: 4614
	private float timer;

	// Token: 0x04001207 RID: 4615
	private Transform gameCameraTransform;

	// Token: 0x04001208 RID: 4616
	private const float CAM_DISTANCE_MAX = 40f;

	// Token: 0x04001209 RID: 4617
	private const float DISPERSE_TIME = 5f;
}
