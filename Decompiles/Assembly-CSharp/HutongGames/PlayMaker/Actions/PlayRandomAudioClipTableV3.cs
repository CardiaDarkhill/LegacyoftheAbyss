using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200120B RID: 4619
	public class PlayRandomAudioClipTableV3 : FsmStateAction
	{
		// Token: 0x06007AC5 RID: 31429 RVA: 0x0024D6D0 File Offset: 0x0024B8D0
		public override void Reset()
		{
			this.Table = null;
			this.AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
			this.SpawnPoint = null;
			this.SpawnPosition = null;
			this.ForcePlay = null;
			this.StoreSpawned = null;
		}

		// Token: 0x06007AC6 RID: 31430 RVA: 0x0024D708 File Offset: 0x0024B908
		public override void OnEnter()
		{
			this.StoreSpawned.Value = null;
			Vector3 vector = this.SpawnPosition.Value;
			GameObject safe = this.SpawnPoint.GetSafe(this);
			if (safe)
			{
				vector += safe.transform.position;
			}
			RandomAudioClipTable randomAudioClipTable = this.Table.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				if (this.AudioPlayerPrefab.Value)
				{
					AudioSource audioSource = randomAudioClipTable.SpawnAndPlayOneShot(this.AudioPlayerPrefab.Value as AudioSource, vector, this.ForcePlay.Value, 1f, null);
					if (audioSource)
					{
						this.StoreSpawned.Value = audioSource.gameObject;
					}
				}
				else
				{
					AudioSource audioSource2 = randomAudioClipTable.SpawnAndPlayOneShot(vector, this.ForcePlay.Value);
					if (audioSource2)
					{
						this.StoreSpawned.Value = audioSource2.gameObject;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007B0A RID: 31498
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		// Token: 0x04007B0B RID: 31499
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		// Token: 0x04007B0C RID: 31500
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04007B0D RID: 31501
		public FsmVector3 SpawnPosition;

		// Token: 0x04007B0E RID: 31502
		public FsmBool ForcePlay;

		// Token: 0x04007B0F RID: 31503
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreSpawned;
	}
}
