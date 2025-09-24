using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200120E RID: 4622
	public sealed class PlayRandomAudioClipTableLooped : FsmStateAction
	{
		// Token: 0x06007AD0 RID: 31440 RVA: 0x0024D995 File Offset: 0x0024BB95
		public override void Reset()
		{
			this.AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
			this.AudioPlayerSource = new FsmObject
			{
				UseVariable = true
			};
			this.waitForPreviousClip = null;
			this.minRate = null;
			this.maxRate = null;
		}

		// Token: 0x06007AD1 RID: 31441 RVA: 0x0024D9D0 File Offset: 0x0024BBD0
		public override void OnEnter()
		{
			this.table = (this.Table.Value as RandomAudioClipTable);
			if (!(this.table != null))
			{
				base.Finish();
				return;
			}
			this.position = this.SpawnPosition.Value;
			GameObject safe = this.SpawnPoint.GetSafe(this);
			if (safe)
			{
				this.position += safe.transform.position;
			}
			if (!this.AudioPlayerSource.IsNone)
			{
				this.currentAudioSource = (this.AudioPlayerSource.Value as AudioSource);
				this.useCurrentSource = (this.currentAudioSource != null);
			}
			else
			{
				this.useCurrentSource = false;
			}
			if (!this.useCurrentSource)
			{
				this.audioSourcePrefab = (this.AudioPlayerPrefab.Value as AudioSource);
				this.hasPlayerPrefab = (this.audioSourcePrefab != null);
			}
			else
			{
				this.audioSourcePrefab = null;
			}
			if (this.useCurrentSource)
			{
				this.table.PlayOneShotUnsafe(this.currentAudioSource, 0f, true);
			}
			else if (this.hasPlayerPrefab)
			{
				this.currentAudioSource = this.table.SpawnAndPlayOneShot(this.audioSourcePrefab, this.position, true, 1f, null);
			}
			else
			{
				this.currentAudioSource = this.table.SpawnAndPlayOneShot(this.position, true);
			}
			if (this.currentAudioSource == null)
			{
				base.Finish();
				return;
			}
			this.timer = this.GetNextPlay();
		}

		// Token: 0x06007AD2 RID: 31442 RVA: 0x0024DB4C File Offset: 0x0024BD4C
		public override void OnExit()
		{
			this.currentAudioSource = null;
			this.audioSourcePrefab = null;
		}

		// Token: 0x06007AD3 RID: 31443 RVA: 0x0024DB5C File Offset: 0x0024BD5C
		private float GetNextPlay()
		{
			float value = this.minRate.Value;
			float num = this.maxRate.Value;
			if (num < value)
			{
				num = value;
			}
			return Random.Range(value, num);
		}

		// Token: 0x06007AD4 RID: 31444 RVA: 0x0024DB90 File Offset: 0x0024BD90
		public override void OnUpdate()
		{
			if (this.waitForPreviousClip.Value && this.currentAudioSource != null && this.currentAudioSource.isPlaying)
			{
				return;
			}
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.timer = this.GetNextPlay();
				if (this.useCurrentSource && this.currentAudioSource != null)
				{
					if (this.currentAudioSource != null)
					{
						this.table.PlayOneShotUnsafe(this.currentAudioSource, 0f, true);
						return;
					}
					this.useCurrentSource = false;
				}
				if (this.hasPlayerPrefab)
				{
					this.currentAudioSource = this.table.SpawnAndPlayOneShot(this.audioSourcePrefab, this.position, true, 1f, null);
					return;
				}
				this.currentAudioSource = this.table.SpawnAndPlayOneShot(this.position, true);
			}
		}

		// Token: 0x04007B18 RID: 31512
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		// Token: 0x04007B19 RID: 31513
		[Tooltip("If set, the audio clip will be played on a spawned instance of this prefab.")]
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		// Token: 0x04007B1A RID: 31514
		[Tooltip("If set, the audio clip will be played from this audio source.")]
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerSource;

		// Token: 0x04007B1B RID: 31515
		[Tooltip("Whether to wait for previous clip to finish playback before playing again.")]
		public FsmBool waitForPreviousClip;

		// Token: 0x04007B1C RID: 31516
		public FsmFloat minRate;

		// Token: 0x04007B1D RID: 31517
		public FsmFloat maxRate;

		// Token: 0x04007B1E RID: 31518
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04007B1F RID: 31519
		public FsmVector3 SpawnPosition;

		// Token: 0x04007B20 RID: 31520
		private AudioSource currentAudioSource;

		// Token: 0x04007B21 RID: 31521
		private float timer;

		// Token: 0x04007B22 RID: 31522
		private bool useCurrentSource;

		// Token: 0x04007B23 RID: 31523
		private bool hasPlayerPrefab;

		// Token: 0x04007B24 RID: 31524
		private AudioSource audioSourcePrefab;

		// Token: 0x04007B25 RID: 31525
		private RandomAudioClipTable table;

		// Token: 0x04007B26 RID: 31526
		private float min;

		// Token: 0x04007B27 RID: 31527
		private float max;

		// Token: 0x04007B28 RID: 31528
		private Vector3 position;
	}
}
