using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class CameraShakeAnimator : MonoBehaviour
{
	// Token: 0x06000A32 RID: 2610 RVA: 0x0002E356 File Offset: 0x0002C556
	private void Awake()
	{
		if (this.requiredVisible)
		{
			this.requiredVisibleRenderers = this.requiredVisible.GetComponentsInChildren<Renderer>();
		}
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0002E378 File Offset: 0x0002C578
	private bool CanShake()
	{
		if (this.range && !this.range.IsInside)
		{
			return false;
		}
		if (this.requiredVisibleRenderers == null)
		{
			return true;
		}
		Renderer[] array = this.requiredVisibleRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isVisible)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0002E3CD File Offset: 0x0002C5CD
	public void DoCameraShake(int index)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (!this.CanShake())
		{
			return;
		}
		if (index < 0 || index >= this.cameraShakeTargets.Length)
		{
			return;
		}
		this.cameraShakeTargets[index].DoShake(this, true);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0002E3FF File Offset: 0x0002C5FF
	public void CancelCameraShake(int index)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (index < 0 || index >= this.cameraShakeTargets.Length)
		{
			return;
		}
		this.cameraShakeTargets[index].CancelShake();
	}

	// Token: 0x040009BA RID: 2490
	[SerializeField]
	private GameObject requiredVisible;

	// Token: 0x040009BB RID: 2491
	[SerializeField]
	private TrackTriggerObjects range;

	// Token: 0x040009BC RID: 2492
	[SerializeField]
	private CameraShakeTarget[] cameraShakeTargets;

	// Token: 0x040009BD RID: 2493
	private Renderer[] requiredVisibleRenderers;
}
