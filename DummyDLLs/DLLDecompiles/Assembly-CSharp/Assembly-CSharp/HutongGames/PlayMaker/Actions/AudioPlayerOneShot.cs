using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB8 RID: 3000
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
	public class AudioPlayerOneShot : FsmStateAction
	{
		// Token: 0x06005C69 RID: 23657 RVA: 0x001D18AC File Offset: 0x001CFAAC
		public override void Reset()
		{
			this.spawnPoint = null;
			this.audioClips = new AudioClip[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.pitchMin = 1f;
			this.pitchMax = 1f;
			this.volume = 1f;
			this.timer = 0f;
		}

		// Token: 0x06005C6A RID: 23658 RVA: 0x001D193A File Offset: 0x001CFB3A
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.delay.Value == 0f)
			{
				this.DoPlayRandomClip();
				base.Finish();
			}
		}

		// Token: 0x06005C6B RID: 23659 RVA: 0x001D1968 File Offset: 0x001CFB68
		public override void OnUpdate()
		{
			if (this.delay.Value > 0f)
			{
				if (this.timer < this.delay.Value)
				{
					this.timer += Time.deltaTime;
					return;
				}
				this.DoPlayRandomClip();
				base.Finish();
			}
		}

		// Token: 0x06005C6C RID: 23660 RVA: 0x001D19BC File Offset: 0x001CFBBC
		private void DoPlayRandomClip()
		{
			if (this.audioClips.Length == 0)
			{
				return;
			}
			GameObject value = this.audioPlayer.Value;
			Vector3 position = this.spawnPoint.Value.transform.position;
			Vector3 up = Vector3.up;
			GameObject gameObject = this.audioPlayer.Value.Spawn(position, Quaternion.Euler(up));
			this.audio = gameObject.GetComponent<AudioSource>();
			this.storePlayer.Value = gameObject;
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
			this.audio.volume = this.volume.Value;
		}

		// Token: 0x040057DB RID: 22491
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The object to spawn. Select Audio Player prefab.")]
		public FsmGameObject audioPlayer;

		// Token: 0x040057DC RID: 22492
		[RequiredField]
		[Tooltip("Object to use as the spawn point of Audio Player")]
		public FsmGameObject spawnPoint;

		// Token: 0x040057DD RID: 22493
		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		// Token: 0x040057DE RID: 22494
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x040057DF RID: 22495
		public FsmFloat pitchMin = 1f;

		// Token: 0x040057E0 RID: 22496
		public FsmFloat pitchMax = 2f;

		// Token: 0x040057E1 RID: 22497
		public FsmFloat volume;

		// Token: 0x040057E2 RID: 22498
		public FsmFloat delay;

		// Token: 0x040057E3 RID: 22499
		public FsmGameObject storePlayer;

		// Token: 0x040057E4 RID: 22500
		private AudioSource audio;

		// Token: 0x040057E5 RID: 22501
		private float timer;
	}
}
