using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBD RID: 3005
	[ActionCategory(ActionCategory.Audio)]
	public class AudioPlayRandomVoice : FsmStateAction
	{
		// Token: 0x06005C7F RID: 23679 RVA: 0x001D1FF4 File Offset: 0x001D01F4
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

		// Token: 0x06005C80 RID: 23680 RVA: 0x001D2067 File Offset: 0x001D0267
		public override void OnEnter()
		{
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x06005C81 RID: 23681 RVA: 0x001D208C File Offset: 0x001D028C
		private void DoPlayRandomClip()
		{
			if (this.audioClips == null || this.audioClips.Length == 0)
			{
				return;
			}
			if (this.self == null)
			{
				return;
			}
			this.audio = this.self.GetComponent<AudioSource>();
			if (this.stopPreviousSound)
			{
				this.audio.Stop();
			}
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

		// Token: 0x04005800 RID: 22528
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an AudioSource component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005801 RID: 22529
		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		// Token: 0x04005802 RID: 22530
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04005803 RID: 22531
		public FsmFloat pitchMin = 1f;

		// Token: 0x04005804 RID: 22532
		public FsmFloat pitchMax = 2f;

		// Token: 0x04005805 RID: 22533
		public bool stopPreviousSound;

		// Token: 0x04005806 RID: 22534
		private GameObject self;

		// Token: 0x04005807 RID: 22535
		private AudioSource audio;
	}
}
