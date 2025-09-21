using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013D RID: 317
public sealed class VolumeBlendController : MonoBehaviour
{
	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060009C9 RID: 2505 RVA: 0x0002C7A6 File Offset: 0x0002A9A6
	public float InitialVolume
	{
		get
		{
			this.Init();
			return this.initialVolume;
		}
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0002C7B4 File Offset: 0x0002A9B4
	public void Init()
	{
		if (this.hasInitialized)
		{
			return;
		}
		this.hasInitialized = true;
		this.hasAudioSource = this.audioSource;
		if (!this.hasAudioSource)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				return;
			}
			this.initialVolume = this.audioSource.volume;
			this.hasAudioSource = true;
		}
		this.UpdateFinalVolume();
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0002C823 File Offset: 0x0002AA23
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0002C82B File Offset: 0x0002AA2B
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0002C848 File Offset: 0x0002AA48
	public void AddOrUpdateModifier(string key, float value)
	{
		VolumeModifier volumeModifier;
		if (this.modifiers.TryGetValue(key, out volumeModifier))
		{
			volumeModifier.Volume = value;
			return;
		}
		volumeModifier = new VolumeModifier(value);
		volumeModifier.OnValueChanged += this.OnModifierChanged;
		this.modifiers.Add(key, volumeModifier);
		this.RecalculateCachedProduct();
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0002C89C File Offset: 0x0002AA9C
	public void RemoveModifier(string key)
	{
		VolumeModifier volumeModifier;
		if (this.modifiers.TryGetValue(key, out volumeModifier))
		{
			volumeModifier.OnValueChanged -= this.OnModifierChanged;
			this.modifiers.Remove(key);
			volumeModifier.SetInvalid();
			this.RecalculateCachedProduct();
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0002C8E4 File Offset: 0x0002AAE4
	private void OnModifierChanged()
	{
		this.RecalculateCachedProduct();
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0002C8EC File Offset: 0x0002AAEC
	private void RecalculateCachedProduct()
	{
		this.cachedProduct = 1f;
		foreach (VolumeModifier volumeModifier in this.modifiers.Values)
		{
			this.cachedProduct *= volumeModifier.Volume;
		}
		this.UpdateFinalVolume();
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0002C964 File Offset: 0x0002AB64
	private void UpdateFinalVolume()
	{
		if (!this.hasAudioSource)
		{
			return;
		}
		this.audioSource.volume = this.baseVolume * this.cachedProduct;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x0002C988 File Offset: 0x0002AB88
	public VolumeModifier GetModifier(string key)
	{
		VolumeModifier volumeModifier;
		if (!this.modifiers.TryGetValue(key, out volumeModifier))
		{
			volumeModifier = (this.modifiers[key] = new VolumeModifier(1f));
			volumeModifier.OnValueChanged += this.OnModifierChanged;
		}
		return volumeModifier;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0002C9D0 File Offset: 0x0002ABD0
	public VolumeModifier GetSharedFSMModifier()
	{
		return this.GetModifier("FSM_SHARED_KEY");
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x0002C9DD File Offset: 0x0002ABDD
	public void SetSharedFSMModifier(float volume)
	{
		this.AddOrUpdateModifier("FSM_SHARED_KEY", volume);
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060009D5 RID: 2517 RVA: 0x0002C9EB File Offset: 0x0002ABEB
	// (set) Token: 0x060009D6 RID: 2518 RVA: 0x0002C9F8 File Offset: 0x0002ABF8
	public float FSMSharedVolume
	{
		get
		{
			return this.GetSharedFSMModifier().Volume;
		}
		set
		{
			this.GetSharedFSMModifier().Volume = value;
		}
	}

	// Token: 0x04000960 RID: 2400
	[Range(0f, 1f)]
	[SerializeField]
	private float baseVolume = 1f;

	// Token: 0x04000961 RID: 2401
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000962 RID: 2402
	private const string FSM_SHARED_KEY = "FSM_SHARED_KEY";

	// Token: 0x04000963 RID: 2403
	private Dictionary<string, VolumeModifier> modifiers = new Dictionary<string, VolumeModifier>(4);

	// Token: 0x04000964 RID: 2404
	private float cachedProduct = 1f;

	// Token: 0x04000965 RID: 2405
	private bool hasAudioSource;

	// Token: 0x04000966 RID: 2406
	private float initialVolume = 1f;

	// Token: 0x04000967 RID: 2407
	private bool hasInitialized;
}
