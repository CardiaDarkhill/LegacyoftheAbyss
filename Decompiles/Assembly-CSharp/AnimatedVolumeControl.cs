using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public sealed class AnimatedVolumeControl : MonoBehaviour
{
	// Token: 0x060007C6 RID: 1990 RVA: 0x00025550 File Offset: 0x00023750
	private void Awake()
	{
		VolumeBlendController component = base.gameObject.GetComponent<VolumeBlendController>();
		if (component)
		{
			this.volumeModifier = component.GetModifier("AnimatedVolumeControl");
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x00025584 File Offset: 0x00023784
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (!Application.isPlaying && this.audioSource)
		{
			this.audioSource.volume = this.volume;
		}
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x000255D0 File Offset: 0x000237D0
	private void OnEnable()
	{
		if (this.audioSource)
		{
			this.currentValue = this.audioSource.volume;
		}
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x000255F0 File Offset: 0x000237F0
	private void LateUpdate()
	{
		if (!Mathf.Approximately(this.currentValue, this.volume))
		{
			this.currentValue = Mathf.MoveTowards(this.currentValue, this.volume, this.maxChangeRate * Time.deltaTime);
			this.SetVolume(this.currentValue);
			return;
		}
		base.enabled = false;
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00025647 File Offset: 0x00023847
	private void OnDidApplyAnimationProperties()
	{
		if (!Mathf.Approximately(this.currentValue, this.volume))
		{
			base.enabled = true;
			return;
		}
		this.SetVolume(this.volume);
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x00025670 File Offset: 0x00023870
	public void SetTargetAlpha(float targetAlpha)
	{
		this.volume = Mathf.Clamp01(targetAlpha);
		if (!Mathf.Approximately(this.currentValue, this.volume))
		{
			base.enabled = true;
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00025698 File Offset: 0x00023898
	private void SetVolume(float alpha)
	{
		if (this.volumeModifier != null)
		{
			VolumeModifier volumeModifier = this.volumeModifier;
			this.currentValue = alpha;
			volumeModifier.Volume = alpha;
			return;
		}
		if (this.audioSource)
		{
			AudioSource audioSource = this.audioSource;
			this.currentValue = alpha;
			audioSource.volume = alpha;
			return;
		}
		base.enabled = false;
	}

	// Token: 0x0400078B RID: 1931
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400078C RID: 1932
	[SerializeField]
	[Range(0f, 1f)]
	private float volume = 1f;

	// Token: 0x0400078D RID: 1933
	[SerializeField]
	private float maxChangeRate = 5f;

	// Token: 0x0400078E RID: 1934
	private float currentValue;

	// Token: 0x0400078F RID: 1935
	private VolumeModifier volumeModifier;
}
