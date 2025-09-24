using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public sealed class AudioEffectTag : MonoBehaviour, IInitialisable
{
	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060007DB RID: 2011 RVA: 0x000258B7 File Offset: 0x00023AB7
	public AudioSource AudioSource
	{
		get
		{
			this.OnAwake();
			return this.audioSource;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060007DC RID: 2012 RVA: 0x000258C6 File Offset: 0x00023AC6
	public AudioEffectTag.AudioEffectType EffectType
	{
		get
		{
			return this.audioEffectType;
		}
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x000258CE File Offset: 0x00023ACE
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		return true;
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x000258FC File Offset: 0x00023AFC
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x00025917 File Offset: 0x00023B17
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x00025920 File Offset: 0x00023B20
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x00025944 File Offset: 0x00023B44
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0400079B RID: 1947
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400079C RID: 1948
	[SerializeField]
	private AudioEffectTag.AudioEffectType audioEffectType;

	// Token: 0x0400079D RID: 1949
	private bool hasAwaken;

	// Token: 0x0400079E RID: 1950
	private bool hasStarted;

	// Token: 0x02001453 RID: 5203
	public enum AudioEffectType
	{
		// Token: 0x040082D8 RID: 33496
		BlackThreadVoice
	}
}
