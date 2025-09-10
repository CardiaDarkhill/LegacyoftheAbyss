using System;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public class VibrationPlayer : MonoBehaviour
{
	// Token: 0x170007D1 RID: 2001
	// (get) Token: 0x0600458F RID: 17807 RVA: 0x0012F09D File Offset: 0x0012D29D
	// (set) Token: 0x06004590 RID: 17808 RVA: 0x0012F0A8 File Offset: 0x0012D2A8
	public VibrationData VibrationData
	{
		get
		{
			return this.data;
		}
		set
		{
			if (this.data.LowFidelityVibration != value.LowFidelityVibration || this.data.HighFidelityVibration != value.HighFidelityVibration || this.data.GamepadVibration != value.GamepadVibration)
			{
				this.data = value;
				this.Stop();
			}
		}
	}

	// Token: 0x170007D2 RID: 2002
	// (get) Token: 0x06004591 RID: 17809 RVA: 0x0012F108 File Offset: 0x0012D308
	// (set) Token: 0x06004592 RID: 17810 RVA: 0x0012F110 File Offset: 0x0012D310
	public VibrationTarget Target
	{
		get
		{
			return this.target;
		}
		set
		{
			if (this.target.Motors != value.Motors)
			{
				this.target = value;
				if (this.emission != null)
				{
					this.emission.Target = this.target;
				}
			}
		}
	}

	// Token: 0x170007D3 RID: 2003
	// (get) Token: 0x06004593 RID: 17811 RVA: 0x0012F146 File Offset: 0x0012D346
	// (set) Token: 0x06004594 RID: 17812 RVA: 0x0012F14E File Offset: 0x0012D34E
	public bool PlayAutomatically
	{
		get
		{
			return this.playAutomatically;
		}
		set
		{
			this.playAutomatically = value;
		}
	}

	// Token: 0x170007D4 RID: 2004
	// (get) Token: 0x06004595 RID: 17813 RVA: 0x0012F157 File Offset: 0x0012D357
	// (set) Token: 0x06004596 RID: 17814 RVA: 0x0012F15F File Offset: 0x0012D35F
	public bool IsLooping
	{
		get
		{
			return this.isLooping;
		}
		set
		{
			this.isLooping = value;
			if (this.emission != null)
			{
				this.emission.IsLooping = this.isLooping;
			}
		}
	}

	// Token: 0x170007D5 RID: 2005
	// (get) Token: 0x06004597 RID: 17815 RVA: 0x0012F181 File Offset: 0x0012D381
	// (set) Token: 0x06004598 RID: 17816 RVA: 0x0012F189 File Offset: 0x0012D389
	public string VibrationTag
	{
		get
		{
			return this.vibrationTag;
		}
		set
		{
			this.vibrationTag = value;
			if (this.emission != null)
			{
				this.emission.Tag = this.vibrationTag;
			}
		}
	}

	// Token: 0x06004599 RID: 17817 RVA: 0x0012F1AC File Offset: 0x0012D3AC
	protected void OnEnable()
	{
		if (this.audioSource)
		{
			this.tryPlayFromAudioSource = true;
		}
		if (this.vibrationDataAsset)
		{
			this.data = this.vibrationDataAsset;
		}
		if (this.playAutomatically && !ObjectPool.IsCreatingPool)
		{
			this.Play();
		}
	}

	// Token: 0x0600459A RID: 17818 RVA: 0x0012F200 File Offset: 0x0012D400
	protected void OnDisable()
	{
		this.Stop();
	}

	// Token: 0x0600459B RID: 17819 RVA: 0x0012F208 File Offset: 0x0012D408
	public void Play()
	{
		if (this.emission != null)
		{
			this.emission.Stop();
		}
		this.emission = VibrationManager.PlayVibrationClipOneShot(this.data, new VibrationTarget?(this.target), this.isLooping, this.vibrationTag, this.isRealtime);
	}

	// Token: 0x0600459C RID: 17820 RVA: 0x0012F256 File Offset: 0x0012D456
	public void Stop()
	{
		if (this.emission != null)
		{
			this.emission.Stop();
			this.emission = null;
		}
	}

	// Token: 0x04004636 RID: 17974
	[SerializeField]
	private VibrationData data;

	// Token: 0x04004637 RID: 17975
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x04004638 RID: 17976
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04004639 RID: 17977
	[SerializeField]
	private VibrationTarget target;

	// Token: 0x0400463A RID: 17978
	[SerializeField]
	private bool playAutomatically;

	// Token: 0x0400463B RID: 17979
	[SerializeField]
	private bool isLooping;

	// Token: 0x0400463C RID: 17980
	[SerializeField]
	private string vibrationTag;

	// Token: 0x0400463D RID: 17981
	[SerializeField]
	private bool isRealtime;

	// Token: 0x0400463E RID: 17982
	private VibrationEmission emission;

	// Token: 0x0400463F RID: 17983
	private bool tryPlayFromAudioSource;
}
