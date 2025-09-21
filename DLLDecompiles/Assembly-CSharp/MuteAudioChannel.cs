using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000126 RID: 294
public class MuteAudioChannel : MonoBehaviour
{
	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000900 RID: 2304 RVA: 0x00029E94 File Offset: 0x00028094
	// (set) Token: 0x06000901 RID: 2305 RVA: 0x00029ECF File Offset: 0x000280CF
	public float Volume
	{
		get
		{
			float dB = 0f;
			if (this.mixer)
			{
				this.mixer.GetFloat(this.exposedProperty, out dB);
			}
			return this.DecibelToLinear(dB);
		}
		set
		{
			if (this.mixer)
			{
				this.mixer.SetFloat(this.exposedProperty, this.LinearToDecibel(value));
			}
		}
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00029EF7 File Offset: 0x000280F7
	private void OnEnable()
	{
		if (this.mixer)
		{
			this.initialVolume = this.Volume;
			this.Volume = 0f;
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00029F1D File Offset: 0x0002811D
	private void OnDisable()
	{
		if (this.mixer)
		{
			this.Volume = this.initialVolume;
		}
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00029F38 File Offset: 0x00028138
	private float LinearToDecibel(float linear)
	{
		float result;
		if (linear != 0f)
		{
			result = 20f * Mathf.Log10(linear);
		}
		else
		{
			result = -144f;
		}
		return result;
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00029F63 File Offset: 0x00028163
	private float DecibelToLinear(float dB)
	{
		return Mathf.Pow(10f, dB / 20f);
	}

	// Token: 0x040008B8 RID: 2232
	public AudioMixer mixer;

	// Token: 0x040008B9 RID: 2233
	public string exposedProperty = "volActors";

	// Token: 0x040008BA RID: 2234
	private float initialVolume;
}
