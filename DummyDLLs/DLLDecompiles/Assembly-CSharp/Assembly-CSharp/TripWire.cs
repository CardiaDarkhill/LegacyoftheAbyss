using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000581 RID: 1409
public class TripWire : MonoBehaviour
{
	// Token: 0x06003276 RID: 12918 RVA: 0x000E097C File Offset: 0x000DEB7C
	private void Awake()
	{
		if (this.wireFlash)
		{
			this.wireFlash.SetActive(false);
		}
		this.activateEffects.SetAllActive(false);
		if (this.trigger)
		{
			this.trigger.OnTriggerEntered += this.OnTriggerEntered;
		}
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x000E09D4 File Offset: 0x000DEBD4
	private void OnTriggerEntered(Collider2D collider, GameObject sender)
	{
		if (this.isTriggered)
		{
			return;
		}
		if (collider.gameObject.layer != 9 && collider.gameObject.layer != 17 && collider.gameObject.layer != 20)
		{
			return;
		}
		this.isTriggered = true;
		this.tripShake.DoShake(this, true);
		if (this.wireFlash)
		{
			if (this.wire)
			{
				this.wireFlash.transform.SetPosition2D(this.wire.transform.position);
				this.wireFlash.transform.SetRotation2D(this.wire.transform.eulerAngles.z);
			}
			this.wireFlash.SetActive(true);
		}
		if (this.wire)
		{
			this.wire.SetActive(false);
		}
		if (this.wireCapRotator)
		{
			this.wireCapRotator.StartAnimation();
		}
		this.activateEffects.SetAllActive(true);
		this.OnTriggered.Invoke();
	}

	// Token: 0x04003634 RID: 13876
	[SerializeField]
	private TriggerEnterEvent trigger;

	// Token: 0x04003635 RID: 13877
	[SerializeField]
	private CameraShakeTarget tripShake;

	// Token: 0x04003636 RID: 13878
	[SerializeField]
	private GameObject wire;

	// Token: 0x04003637 RID: 13879
	[SerializeField]
	private GameObject wireFlash;

	// Token: 0x04003638 RID: 13880
	[SerializeField]
	private GameObject[] activateEffects;

	// Token: 0x04003639 RID: 13881
	[SerializeField]
	private CurveRotationAnimation wireCapRotator;

	// Token: 0x0400363A RID: 13882
	[Space]
	public UnityEvent OnTriggered;

	// Token: 0x0400363B RID: 13883
	private bool isTriggered;
}
