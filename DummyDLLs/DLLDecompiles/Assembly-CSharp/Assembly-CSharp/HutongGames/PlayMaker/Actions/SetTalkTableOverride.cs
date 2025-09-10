using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001364 RID: 4964
	public class SetTalkTableOverride : FsmStateAction
	{
		// Token: 0x0600800B RID: 32779 RVA: 0x0025D38C File Offset: 0x0025B58C
		public override void Reset()
		{
			this.Target = null;
			this.PlayerVoiceTableOverride = null;
		}

		// Token: 0x0600800C RID: 32780 RVA: 0x0025D39C File Offset: 0x0025B59C
		public override void OnEnter()
		{
			PlayMakerNPC safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				RandomAudioClipTable randomAudioClipTable = this.PlayerVoiceTableOverride.Value as RandomAudioClipTable;
				if (randomAudioClipTable != null)
				{
					safe.SetTalkTableOverride(randomAudioClipTable);
				}
				else
				{
					safe.RemoveTalkTableOverride();
				}
			}
			base.Finish();
		}

		// Token: 0x04007F78 RID: 32632
		[RequiredField]
		[CheckForComponent(typeof(PlayMakerNPC))]
		public FsmOwnerDefault Target;

		// Token: 0x04007F79 RID: 32633
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject PlayerVoiceTableOverride;
	}
}
