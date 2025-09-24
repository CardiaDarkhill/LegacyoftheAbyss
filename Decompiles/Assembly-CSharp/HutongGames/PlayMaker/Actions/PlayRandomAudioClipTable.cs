using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001209 RID: 4617
	public class PlayRandomAudioClipTable : FsmStateAction
	{
		// Token: 0x06007ABF RID: 31423 RVA: 0x0024D52F File Offset: 0x0024B72F
		public override void Reset()
		{
			this.AudioPlayerPrefab = new FsmObject
			{
				UseVariable = true
			};
		}

		// Token: 0x06007AC0 RID: 31424 RVA: 0x0024D544 File Offset: 0x0024B744
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
					randomAudioClipTable.SpawnAndPlayOneShot(this.AudioPlayerPrefab.Value as AudioSource, vector, false, 1f, null);
				}
				else
				{
					randomAudioClipTable.SpawnAndPlayOneShot(vector, false);
				}
			}
			base.Finish();
		}

		// Token: 0x04007B01 RID: 31489
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		// Token: 0x04007B02 RID: 31490
		[ObjectType(typeof(AudioSource))]
		public FsmObject AudioPlayerPrefab;

		// Token: 0x04007B03 RID: 31491
		public FsmOwnerDefault SpawnPoint;

		// Token: 0x04007B04 RID: 31492
		public FsmVector3 SpawnPosition;
	}
}
