using System;
using UnityEngine;

// Token: 0x02000108 RID: 264
[RequireComponent(typeof(AudioSource))]
public class AudioLoopRandom : MonoBehaviour
{
	// Token: 0x06000837 RID: 2103 RVA: 0x00026E14 File Offset: 0x00025014
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00026E22 File Offset: 0x00025022
	private void Start()
	{
		this.PlayRandom(!this.dontAutoPlay);
	}

	// Token: 0x06000839 RID: 2105 RVA: 0x00026E34 File Offset: 0x00025034
	private void PlayRandom(bool doPlay)
	{
		this.source.loop = true;
		AudioLoopRandom.ProbabilityAudioClip[] array = this.setRandomClips;
		if (array != null && array.Length > 0)
		{
			AudioClip randomItemByProbability = Probability.GetRandomItemByProbability<AudioLoopRandom.ProbabilityAudioClip, AudioClip>(this.setRandomClips, null);
			this.source.clip = randomItemByProbability;
		}
		if (!this.source.clip)
		{
			return;
		}
		this.source.time = Random.Range(0f, this.source.clip.length);
		if (!this.source.isPlaying && doPlay)
		{
			this.source.Play();
		}
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00026ECB File Offset: 0x000250CB
	public void PlayRandom()
	{
		this.PlayRandom(true);
	}

	// Token: 0x040007CB RID: 1995
	[SerializeField]
	private bool dontAutoPlay;

	// Token: 0x040007CC RID: 1996
	[SerializeField]
	private AudioLoopRandom.ProbabilityAudioClip[] setRandomClips;

	// Token: 0x040007CD RID: 1997
	private AudioSource source;

	// Token: 0x0200145B RID: 5211
	[Serializable]
	private class ProbabilityAudioClip : Probability.ProbabilityBase<AudioClip>
	{
		// Token: 0x17000CC4 RID: 3268
		// (get) Token: 0x06008350 RID: 33616 RVA: 0x0026893A File Offset: 0x00266B3A
		public override AudioClip Item
		{
			get
			{
				return this.clip;
			}
		}

		// Token: 0x040082E8 RID: 33512
		[SerializeField]
		private AudioClip clip;
	}
}
