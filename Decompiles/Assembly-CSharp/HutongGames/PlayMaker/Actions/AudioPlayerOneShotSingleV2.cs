using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001300 RID: 4864
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
	public class AudioPlayerOneShotSingleV2 : FsmStateAction
	{
		// Token: 0x06007E7B RID: 32379 RVA: 0x00259113 File Offset: 0x00257313
		public override void Reset()
		{
			this.spawnPoint = null;
			this.pitchMin = 1f;
			this.pitchMax = 1f;
			this.volume = 1f;
			this.playVibration = null;
		}

		// Token: 0x06007E7C RID: 32380 RVA: 0x00259153 File Offset: 0x00257353
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.delay.Value == 0f)
			{
				this.DoPlayRandomClip();
				base.Finish();
			}
		}

		// Token: 0x06007E7D RID: 32381 RVA: 0x00259180 File Offset: 0x00257380
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

		// Token: 0x06007E7E RID: 32382 RVA: 0x002591D4 File Offset: 0x002573D4
		private void DoPlayRandomClip()
		{
			if (!this.audioClip.Value)
			{
				return;
			}
			if (this.audioPlayer.IsNone || this.audioPlayer.Value == null)
			{
				Debug.LogError("AudioPlayer object not set!");
				return;
			}
			if (this.spawnPoint.IsNone || this.spawnPoint.Value == null)
			{
				return;
			}
			Vector3 position = this.spawnPoint.Value.transform.position;
			Vector3 up = Vector3.up;
			GameObject gameObject = this.audioPlayer.Value.Spawn(position, Quaternion.Euler(up));
			this.audio = gameObject.GetComponent<AudioSource>();
			this.storePlayer.Value = gameObject;
			AudioClip audioClip = this.audioClip.Value as AudioClip;
			if (audioClip)
			{
				float pitch = Random.Range(this.pitchMin.Value, this.pitchMax.Value);
				this.audio.pitch = pitch;
				this.audio.volume = this.volume.Value;
				this.audio.PlayOneShot(audioClip);
				if (this.vibrationDataAsset.Value)
				{
					VibrationManager.PlayVibrationClipOneShot((VibrationDataAsset)this.vibrationDataAsset.Value, null, false, "", false);
				}
			}
		}

		// Token: 0x04007E35 RID: 32309
		[RequiredField]
		[Tooltip("The object to spawn. Select Audio Player prefab.")]
		public FsmGameObject audioPlayer;

		// Token: 0x04007E36 RID: 32310
		[RequiredField]
		[Tooltip("Object to use as the spawn point of Audio Player")]
		public FsmGameObject spawnPoint;

		// Token: 0x04007E37 RID: 32311
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x04007E38 RID: 32312
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x04007E39 RID: 32313
		public FsmBool playVibration;

		// Token: 0x04007E3A RID: 32314
		public FsmFloat pitchMin;

		// Token: 0x04007E3B RID: 32315
		public FsmFloat pitchMax;

		// Token: 0x04007E3C RID: 32316
		public FsmFloat volume = 1f;

		// Token: 0x04007E3D RID: 32317
		public FsmFloat delay;

		// Token: 0x04007E3E RID: 32318
		public FsmGameObject storePlayer;

		// Token: 0x04007E3F RID: 32319
		private AudioSource audio;

		// Token: 0x04007E40 RID: 32320
		private float timer;
	}
}
