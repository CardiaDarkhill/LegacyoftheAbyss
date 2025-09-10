using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200120A RID: 4618
	public class PlayRandomAudioClipTableV2 : FsmStateAction
	{
		// Token: 0x06007AC2 RID: 31426 RVA: 0x0024D5E8 File Offset: 0x0024B7E8
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
		}

		// Token: 0x06007AC3 RID: 31427 RVA: 0x0024D618 File Offset: 0x0024B818
		public override void OnEnter()
		{
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
					randomAudioClipTable.SpawnAndPlayOneShot(this.AudioPlayerPrefab.Value as AudioSource, vector, this.ForcePlay.Value, 1f, null);
				}
				else
				{
					randomAudioClipTable.SpawnAndPlayOneShot(vector, this.ForcePlay.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007B05 RID: 31493
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		// Token: 0x04007B06 RID: 31494
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		// Token: 0x04007B07 RID: 31495
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04007B08 RID: 31496
		public FsmVector3 SpawnPosition;

		// Token: 0x04007B09 RID: 31497
		public FsmBool ForcePlay;
	}
}
