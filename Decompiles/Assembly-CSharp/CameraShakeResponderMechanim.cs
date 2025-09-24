using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200006D RID: 109
public sealed class CameraShakeResponderMechanim : MonoBehaviour
{
	// Token: 0x060002D2 RID: 722 RVA: 0x0000F740 File Offset: 0x0000D940
	private void Awake()
	{
		this.shakeEvents.RemoveAll((CameraShakeResponderMechanim.ShakeEvent o) => o == null);
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
			if (this.animator == null)
			{
				Debug.LogError(string.Format("{0} is missing an Animator component.", this), this);
				base.enabled = false;
			}
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0000F7B8 File Offset: 0x0000D9B8
	private void OnValidate()
	{
		foreach (CameraShakeResponderMechanim.ShakeEvent shakeEvent in this.shakeEvents)
		{
			if (shakeEvent != null)
			{
				shakeEvent.OnValidate();
			}
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponent<Animator>();
		}
		this.calculatedRange = false;
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x0000F830 File Offset: 0x0000DA30
	private void LateUpdate()
	{
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				if (this.animator != null)
				{
					CameraShakeResponderMechanim.ShakeEvent shakeEvent = this.currentShakeEvent;
					if (shakeEvent != null)
					{
						shakeEvent.StopAnimation(this.animator);
					}
				}
				this.currentShakeEvent = null;
			}
		}
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0000F895 File Offset: 0x0000DA95
	private void OnEnable()
	{
		this.RegisterEvents();
	}

	// Token: 0x060002D6 RID: 726 RVA: 0x0000F89D File Offset: 0x0000DA9D
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x060002D7 RID: 727 RVA: 0x0000F8A8 File Offset: 0x0000DAA8
	private void RegisterEvents()
	{
		if (!this.registeredEvent)
		{
			this.registeredEvent = true;
			CameraShakeResponderMechanim.ShakeEventType shakeEventType = this.shakeEventType;
			if (shakeEventType == CameraShakeResponderMechanim.ShakeEventType.Shaked)
			{
				GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce += this.OnCameraShaked;
				return;
			}
			if (shakeEventType != CameraShakeResponderMechanim.ShakeEventType.Shaking)
			{
				return;
			}
			GlobalSettings.Camera.MainCameraShakeManager.CameraShakingWorldForce += this.OnCameraShaking;
		}
	}

	// Token: 0x060002D8 RID: 728 RVA: 0x0000F900 File Offset: 0x0000DB00
	private void UnregisterEvents()
	{
		if (this.registeredEvent)
		{
			this.registeredEvent = false;
			CameraShakeResponderMechanim.ShakeEventType shakeEventType = this.shakeEventType;
			if (shakeEventType == CameraShakeResponderMechanim.ShakeEventType.Shaked)
			{
				GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce -= this.OnCameraShaked;
				return;
			}
			if (shakeEventType != CameraShakeResponderMechanim.ShakeEventType.Shaking)
			{
				return;
			}
			GlobalSettings.Camera.MainCameraShakeManager.CameraShakingWorldForce -= this.OnCameraShaking;
		}
	}

	// Token: 0x060002D9 RID: 729 RVA: 0x0000F958 File Offset: 0x0000DB58
	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (intensity < this.minIntensity || intensity > this.maxIntensity)
		{
			return;
		}
		this.TryDoShake(cameraPosition);
	}

	// Token: 0x060002DA RID: 730 RVA: 0x0000F974 File Offset: 0x0000DB74
	private void TryDoShake(Vector2 cameraPosition)
	{
		if (this.shakeEvents.Count == 0)
		{
			return;
		}
		if (this.radius > 0f && Vector2.SqrMagnitude(base.transform.position - cameraPosition) > this.radius * this.radius)
		{
			return;
		}
		if (this.currentShakeEvent != null)
		{
			float randomValue = this.duration.GetRandomValue();
			if (randomValue > this.timer)
			{
				this.timer = randomValue;
			}
			return;
		}
		if (!(this.animator != null))
		{
			base.enabled = false;
			return;
		}
		this.currentShakeEvent = this.shakeEvents[Random.Range(0, this.shakeEvents.Count)];
		if (this.currentShakeEvent != null)
		{
			this.currentShakeEvent.PlayAnimation(this.animator);
			this.timer = this.minDuration.GetRandomValue();
			return;
		}
		Debug.LogError(string.Format("{0} has a null shake", this), this);
	}

	// Token: 0x060002DB RID: 731 RVA: 0x0000FA60 File Offset: 0x0000DC60
	private void CalculateRange()
	{
		if (!this.calculatedRange)
		{
			this.validRange = CameraShakeWorldForceFlag.None;
			for (CameraShakeWorldForceIntensities cameraShakeWorldForceIntensities = this.minIntensity; cameraShakeWorldForceIntensities <= this.maxIntensity; cameraShakeWorldForceIntensities++)
			{
				this.validRange |= cameraShakeWorldForceIntensities.ToFlagMax();
			}
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0000FAA5 File Offset: 0x0000DCA5
	private void OnCameraShaking(Vector2 cameraPosition, CameraShakeWorldForceFlag intensity)
	{
		this.CalculateRange();
		if ((intensity & this.validRange) == CameraShakeWorldForceFlag.None)
		{
			return;
		}
		this.TryDoShake(cameraPosition);
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0000FABF File Offset: 0x0000DCBF
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x04000265 RID: 613
	[SerializeField]
	private Animator animator;

	// Token: 0x04000266 RID: 614
	[SerializeField]
	private List<CameraShakeResponderMechanim.ShakeEvent> shakeEvents = new List<CameraShakeResponderMechanim.ShakeEvent>();

	// Token: 0x04000267 RID: 615
	[SerializeField]
	private CameraShakeResponderMechanim.ShakeEventType shakeEventType;

	// Token: 0x04000268 RID: 616
	[Header("Shake Settings")]
	[SerializeField]
	private MinMaxFloat minDuration = new MinMaxFloat(0.25f, 0.3f);

	// Token: 0x04000269 RID: 617
	[SerializeField]
	private MinMaxFloat duration = new MinMaxFloat(0.025f, 0.075f);

	// Token: 0x0400026A RID: 618
	[SerializeField]
	private float radius = 20f;

	// Token: 0x0400026B RID: 619
	[Space]
	[SerializeField]
	private CameraShakeWorldForceIntensities minIntensity = CameraShakeWorldForceIntensities.Medium;

	// Token: 0x0400026C RID: 620
	[SerializeField]
	private CameraShakeWorldForceIntensities maxIntensity = CameraShakeWorldForceIntensities.Intense;

	// Token: 0x0400026D RID: 621
	private bool registeredEvent;

	// Token: 0x0400026E RID: 622
	private float timer;

	// Token: 0x0400026F RID: 623
	private float minTimer;

	// Token: 0x04000270 RID: 624
	private CameraShakeResponderMechanim.ShakeEvent currentShakeEvent;

	// Token: 0x04000271 RID: 625
	private bool calculatedRange;

	// Token: 0x04000272 RID: 626
	private CameraShakeWorldForceFlag validRange;

	// Token: 0x020013E2 RID: 5090
	[Serializable]
	private class ShakeEvent
	{
		// Token: 0x060081C7 RID: 33223 RVA: 0x002624EC File Offset: 0x002606EC
		public void PlayAnimation(Animator animator)
		{
			if (this.randomiseShakeOffset)
			{
				animator.Play(this.shakeAnim.Hash, 0, Random.Range(0f, 1f));
				return;
			}
			animator.Play(this.shakeAnim.Hash);
		}

		// Token: 0x060081C8 RID: 33224 RVA: 0x00262529 File Offset: 0x00260729
		public void StopAnimation(Animator animator)
		{
			if (this.randomiseIdleOffset)
			{
				animator.Play(this.idleAnim.Hash, 0, Random.Range(0f, 1f));
				return;
			}
			animator.Play(this.idleAnim.Hash);
		}

		// Token: 0x060081C9 RID: 33225 RVA: 0x00262566 File Offset: 0x00260766
		public void OnValidate()
		{
			this.shakeAnim.Dirty();
			this.idleAnim.Dirty();
		}

		// Token: 0x04008107 RID: 33031
		public AnimatorHashCache shakeAnim = new AnimatorHashCache("shake");

		// Token: 0x04008108 RID: 33032
		public bool randomiseShakeOffset;

		// Token: 0x04008109 RID: 33033
		public AnimatorHashCache idleAnim = new AnimatorHashCache("idle");

		// Token: 0x0400810A RID: 33034
		public bool randomiseIdleOffset;
	}

	// Token: 0x020013E3 RID: 5091
	[Serializable]
	private enum ShakeEventType
	{
		// Token: 0x0400810C RID: 33036
		Shaked,
		// Token: 0x0400810D RID: 33037
		Shaking
	}
}
