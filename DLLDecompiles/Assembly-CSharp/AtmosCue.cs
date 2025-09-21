using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020000F8 RID: 248
[CreateAssetMenu(fileName = "NewAtmosCue", menuName = "Hollow Knight/Atmos Cue", order = 1000)]
public class AtmosCue : ScriptableObject
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060007CE RID: 1998 RVA: 0x0002570B File Offset: 0x0002390B
	public AudioMixerSnapshot Snapshot
	{
		get
		{
			return this.snapshot;
		}
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00025713 File Offset: 0x00023913
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<AtmosCue.AtmosChannelInfo>(ref this.channelInfos, typeof(AtmosChannels));
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x0002572C File Offset: 0x0002392C
	public AtmosCue.AtmosChannelInfo GetChannelInfo(AtmosChannels channel)
	{
		if (channel < AtmosChannels.Layer1 || channel >= (AtmosChannels)this.channelInfos.Length)
		{
			return null;
		}
		return this.channelInfos[(int)channel];
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x00025754 File Offset: 0x00023954
	public AtmosCue ResolveAlternatives()
	{
		if (this.alternatives != null)
		{
			AtmosCue.Alternative[] array = this.alternatives;
			int i = 0;
			while (i < array.Length)
			{
				AtmosCue.Alternative alternative = array[i];
				if (alternative.Condition.IsFulfilled)
				{
					AtmosCue cue = alternative.Cue;
					if (!(cue != null))
					{
						return null;
					}
					return cue.ResolveAlternatives();
				}
				else
				{
					i++;
				}
			}
		}
		return this;
	}

	// Token: 0x04000790 RID: 1936
	[SerializeField]
	private AudioMixerSnapshot snapshot;

	// Token: 0x04000791 RID: 1937
	[SerializeField]
	[ArrayForEnum(typeof(AtmosChannels))]
	private AtmosCue.AtmosChannelInfo[] channelInfos;

	// Token: 0x04000792 RID: 1938
	[SerializeField]
	private AtmosCue.Alternative[] alternatives;

	// Token: 0x02001450 RID: 5200
	[Serializable]
	public class AtmosChannelInfo
	{
		// Token: 0x17000CC0 RID: 3264
		// (get) Token: 0x06008337 RID: 33591 RVA: 0x0026873E File Offset: 0x0026693E
		public AudioClip Clip
		{
			get
			{
				return this.clip;
			}
		}

		// Token: 0x17000CC1 RID: 3265
		// (get) Token: 0x06008338 RID: 33592 RVA: 0x00268746 File Offset: 0x00266946
		public bool IsEnabled
		{
			get
			{
				return this.clip != null;
			}
		}

		// Token: 0x040082D1 RID: 33489
		[SerializeField]
		private AudioClip clip;
	}

	// Token: 0x02001451 RID: 5201
	[Serializable]
	private class Alternative
	{
		// Token: 0x040082D2 RID: 33490
		public AtmosCue Cue;

		// Token: 0x040082D3 RID: 33491
		public PlayerDataTest Condition;
	}
}
