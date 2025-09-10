using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBB RID: 3003
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandom : FsmStateAction
	{
		// Token: 0x06005C77 RID: 23671 RVA: 0x001D1DEC File Offset: 0x001CFFEC
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClips = new AudioClip[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.pitchMin = 1f;
			this.pitchMax = 1f;
		}

		// Token: 0x06005C78 RID: 23672 RVA: 0x001D1E5F File Offset: 0x001D005F
		public override void OnEnter()
		{
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06005C79 RID: 23673 RVA: 0x001D1E70 File Offset: 0x001D0070
		private void DoPlayRandomClip()
		{
			if (this.audioClips.Length == 0)
			{
				return;
			}
			this.audio = this.gameObject.GetSafe<AudioSource>();
			if (this.audio != null)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					AudioClip audioClip = this.audioClips[randomWeightedIndex];
					if (audioClip != null)
					{
						float pitch = Random.Range(this.pitchMin.Value, this.pitchMax.Value);
						this.audio.pitch = pitch;
						this.audio.PlayOneShot(audioClip);
					}
				}
			}
		}

		// Token: 0x040057F5 RID: 22517
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmGameObject gameObject;

		// Token: 0x040057F6 RID: 22518
		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		// Token: 0x040057F7 RID: 22519
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x040057F8 RID: 22520
		public FsmFloat pitchMin = 1f;

		// Token: 0x040057F9 RID: 22521
		public FsmFloat pitchMax = 2f;

		// Token: 0x040057FA RID: 22522
		private AudioSource audio;
	}
}
