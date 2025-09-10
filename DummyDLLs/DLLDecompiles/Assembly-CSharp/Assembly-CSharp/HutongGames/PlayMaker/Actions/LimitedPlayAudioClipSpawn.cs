using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001205 RID: 4613
	public class LimitedPlayAudioClipSpawn : LimitedPlayAudioBase
	{
		// Token: 0x06007AB3 RID: 31411 RVA: 0x0024D205 File Offset: 0x0024B405
		public override void Reset()
		{
			base.Reset();
			this.audioClip = null;
			this.AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
			this.SpawnPoint = null;
			this.SpawnPosition = null;
			this.StoreSpawned = null;
		}

		// Token: 0x06007AB4 RID: 31412 RVA: 0x0024D23C File Offset: 0x0024B43C
		protected override bool PlayAudio(out AudioSource audioSource)
		{
			this.StoreSpawned.Value = null;
			Vector3 vector = this.SpawnPosition.Value;
			GameObject safe = this.SpawnPoint.GetSafe(this);
			if (safe)
			{
				vector += safe.transform.position;
			}
			bool result = false;
			audioSource = new AudioEvent
			{
				Clip = (this.audioClip.Value as AudioClip),
				PitchMax = 1f,
				PitchMin = 1f,
				Volume = 1f
			}.SpawnAndPlayOneShot(this.AudioPlayerPrefab.Value as AudioSource, vector, null);
			if (audioSource != null)
			{
				result = true;
				if (!this.StoreSpawned.IsNone)
				{
					this.StoreSpawned.Value = audioSource.gameObject;
				}
			}
			else
			{
				audioSource = null;
			}
			return result;
		}

		// Token: 0x04007AF1 RID: 31473
		[Space]
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		// Token: 0x04007AF2 RID: 31474
		[RequiredField]
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x04007AF3 RID: 31475
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04007AF4 RID: 31476
		public FsmVector3 SpawnPosition;

		// Token: 0x04007AF5 RID: 31477
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;
	}
}
