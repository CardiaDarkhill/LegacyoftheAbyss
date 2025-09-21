using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200120C RID: 4620
	public class GetRandomAudioClipFromTable : FsmStateAction
	{
		// Token: 0x06007AC8 RID: 31432 RVA: 0x0024D804 File Offset: 0x0024BA04
		public override void Reset()
		{
			this.Table = null;
			this.ForcePlay = null;
			this.StoreClip = null;
		}

		// Token: 0x06007AC9 RID: 31433 RVA: 0x0024D81C File Offset: 0x0024BA1C
		public override void OnEnter()
		{
			RandomAudioClipTable randomAudioClipTable = this.Table.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				this.StoreClip.Value = randomAudioClipTable.SelectClip(this.ForcePlay.Value);
			}
			base.Finish();
		}

		// Token: 0x04007B10 RID: 31504
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;

		// Token: 0x04007B11 RID: 31505
		public FsmBool ForcePlay;

		// Token: 0x04007B12 RID: 31506
		[ObjectType(typeof(AudioClip))]
		public FsmObject StoreClip;
	}
}
