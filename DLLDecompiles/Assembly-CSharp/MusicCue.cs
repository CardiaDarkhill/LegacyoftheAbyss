using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000122 RID: 290
[CreateAssetMenu(fileName = "MusicCue", menuName = "Hollow Knight/Music Cue", order = 1000)]
public class MusicCue : ScriptableObject
{
	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x060008EE RID: 2286 RVA: 0x00029BF0 File Offset: 0x00027DF0
	public string OriginalMusicEventName
	{
		get
		{
			return this.originalMusicEventName;
		}
	}

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x060008EF RID: 2287 RVA: 0x00029BF8 File Offset: 0x00027DF8
	public int OriginalMusicTrackNumber
	{
		get
		{
			return this.originalMusicTrackNumber;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x060008F0 RID: 2288 RVA: 0x00029C00 File Offset: 0x00027E00
	public AudioMixerSnapshot Snapshot
	{
		get
		{
			return this.snapshot;
		}
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00029C08 File Offset: 0x00027E08
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<MusicCue.MusicChannelInfo>(ref this.channelInfos, typeof(MusicChannels));
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00029C20 File Offset: 0x00027E20
	public MusicCue.MusicChannelInfo GetChannelInfo(MusicChannels channel)
	{
		if (channel < MusicChannels.Main || channel >= (MusicChannels)this.channelInfos.Length)
		{
			return null;
		}
		return this.channelInfos[(int)channel];
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00029C48 File Offset: 0x00027E48
	public MusicCue ResolveAlternatives()
	{
		if (this.alternatives != null)
		{
			MusicCue.Alternative[] array = this.alternatives;
			int i = 0;
			while (i < array.Length)
			{
				MusicCue.Alternative alternative = array[i];
				if (alternative.Condition.IsFulfilled)
				{
					MusicCue cue = alternative.Cue;
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

	// Token: 0x060008F4 RID: 2292 RVA: 0x00029C9D File Offset: 0x00027E9D
	public void Preload(GameObject gameObject)
	{
		this.InternalPreload(gameObject, true);
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00029CA8 File Offset: 0x00027EA8
	private void InternalPreload(GameObject gameObject, bool preloadAlts)
	{
		if (this.channelInfos != null)
		{
			foreach (MusicCue.MusicChannelInfo musicChannelInfo in this.channelInfos)
			{
				if (musicChannelInfo != null && !(musicChannelInfo.Clip == null))
				{
					this.PreloadClip(gameObject, musicChannelInfo.Clip);
				}
			}
		}
		if (this.alternatives != null)
		{
			MusicCue.Alternative[] array2 = this.alternatives;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Preload(gameObject);
			}
		}
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00029D1A File Offset: 0x00027F1A
	private void PreloadClip(GameObject gameObject, AudioClip clip)
	{
		AudioPreloader.PreloadClip(clip);
	}

	// Token: 0x040008A6 RID: 2214
	[SerializeField]
	private string originalMusicEventName;

	// Token: 0x040008A7 RID: 2215
	[SerializeField]
	private int originalMusicTrackNumber;

	// Token: 0x040008A8 RID: 2216
	[SerializeField]
	private AudioMixerSnapshot snapshot;

	// Token: 0x040008A9 RID: 2217
	[SerializeField]
	[ArrayForEnum(typeof(MusicChannels))]
	private MusicCue.MusicChannelInfo[] channelInfos;

	// Token: 0x040008AA RID: 2218
	[SerializeField]
	private MusicCue.Alternative[] alternatives;

	// Token: 0x02001465 RID: 5221
	[Serializable]
	public class MusicChannelInfo
	{
		// Token: 0x17000CCD RID: 3277
		// (get) Token: 0x0600836B RID: 33643 RVA: 0x00268D42 File Offset: 0x00266F42
		public AudioClip Clip
		{
			get
			{
				return this.clip;
			}
		}

		// Token: 0x17000CCE RID: 3278
		// (get) Token: 0x0600836C RID: 33644 RVA: 0x00268D4A File Offset: 0x00266F4A
		public bool IsEnabled
		{
			get
			{
				return this.clip != null;
			}
		}

		// Token: 0x17000CCF RID: 3279
		// (get) Token: 0x0600836D RID: 33645 RVA: 0x00268D58 File Offset: 0x00266F58
		public bool IsSyncRequired
		{
			get
			{
				if (this.sync == MusicChannelSync.Implicit)
				{
					return this.clip != null;
				}
				return this.sync == MusicChannelSync.ExplicitOn;
			}
		}

		// Token: 0x04008310 RID: 33552
		[SerializeField]
		private AudioClip clip;

		// Token: 0x04008311 RID: 33553
		[SerializeField]
		private MusicChannelSync sync;
	}

	// Token: 0x02001466 RID: 5222
	[Serializable]
	private class Alternative
	{
		// Token: 0x0600836F RID: 33647 RVA: 0x00268D83 File Offset: 0x00266F83
		public void Preload(GameObject gameObject)
		{
			if (this.Cue == null)
			{
				return;
			}
			this.Cue.InternalPreload(gameObject, false);
		}

		// Token: 0x04008312 RID: 33554
		public MusicCue Cue;

		// Token: 0x04008313 RID: 33555
		public PlayerDataTest Condition;
	}
}
