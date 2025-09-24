using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB9 RID: 3001
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Instantiate an Audio Player object and play a oneshot sound via its Audio Source.")]
	public class AudioPlayerOneShotSingle : FsmStateAction
	{
		// Token: 0x06005C6E RID: 23662 RVA: 0x001D1AC4 File Offset: 0x001CFCC4
		public override void Reset()
		{
			this.spawnPoint = null;
			this.pitchMin = 1f;
			this.pitchMax = 1f;
			this.volume = 1f;
		}

		// Token: 0x06005C6F RID: 23663 RVA: 0x001D1AFD File Offset: 0x001CFCFD
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.delay.Value == 0f)
			{
				this.DoPlayRandomClip();
				base.Finish();
			}
		}

		// Token: 0x06005C70 RID: 23664 RVA: 0x001D1B28 File Offset: 0x001CFD28
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

		// Token: 0x06005C71 RID: 23665 RVA: 0x001D1B7C File Offset: 0x001CFD7C
		private void DoPlayRandomClip()
		{
			AudioClip audioClip = this.audioClip.Value as AudioClip;
			if (audioClip == null)
			{
				return;
			}
			if (!this.audioPlayer.IsNone && !this.spawnPoint.IsNone && this.spawnPoint.Value != null)
			{
				GameObject value = this.audioPlayer.Value;
				Vector3 position = this.spawnPoint.Value.transform.position;
				Vector3 up = Vector3.up;
				if (this.audioPlayer.Value != null)
				{
					GameObject newObject = this.audioPlayer.Value.Spawn(position, Quaternion.Euler(up));
					this.audio = newObject.GetComponent<AudioSource>();
					if (this.audio != null)
					{
						if (!this.storePlayer.IsNone)
						{
							this.storePlayer.Value = newObject;
							RecycleResetHandler.Add(newObject, delegate()
							{
								if (!this.storePlayer.IsNone && this.storePlayer.Value == newObject)
								{
									this.storePlayer.Value = null;
								}
							});
						}
						float pitch = Random.Range(this.pitchMin.Value, this.pitchMax.Value);
						this.audio.pitch = pitch;
						this.audio.volume = this.volume.Value;
						this.audio.PlayOneShot(audioClip);
						return;
					}
				}
				else
				{
					Debug.LogError("AudioPlayer object not set!");
				}
			}
		}

		// Token: 0x040057E6 RID: 22502
		[RequiredField]
		[Tooltip("The object to spawn. Select Audio Player prefab.")]
		public FsmGameObject audioPlayer;

		// Token: 0x040057E7 RID: 22503
		[RequiredField]
		[Tooltip("Object to use as the spawn point of Audio Player")]
		public FsmGameObject spawnPoint;

		// Token: 0x040057E8 RID: 22504
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x040057E9 RID: 22505
		public FsmFloat pitchMin;

		// Token: 0x040057EA RID: 22506
		public FsmFloat pitchMax;

		// Token: 0x040057EB RID: 22507
		public FsmFloat volume = 1f;

		// Token: 0x040057EC RID: 22508
		public FsmFloat delay;

		// Token: 0x040057ED RID: 22509
		public FsmGameObject storePlayer;

		// Token: 0x040057EE RID: 22510
		private AudioSource audio;

		// Token: 0x040057EF RID: 22511
		private float timer;
	}
}
