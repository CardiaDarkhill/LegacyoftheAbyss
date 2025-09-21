using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200135B RID: 4955
	public class SetNPCSpeakingAudioTable : FsmStateAction
	{
		// Token: 0x06007FDA RID: 32730 RVA: 0x0025C943 File Offset: 0x0025AB43
		public override void Reset()
		{
			this.Target = null;
			this.Table = null;
		}

		// Token: 0x06007FDB RID: 32731 RVA: 0x0025C954 File Offset: 0x0025AB54
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				NPCSpeakingAudio component = safe.GetComponent<NPCSpeakingAudio>();
				if (component)
				{
					component.SetTableForSpeaker(null, this.Table.Value as RandomAudioClipTable);
				}
			}
			base.Finish();
		}

		// Token: 0x04007F4C RID: 32588
		[RequiredField]
		[CheckForComponent(typeof(NPCSpeakingAudio))]
		public FsmOwnerDefault Target;

		// Token: 0x04007F4D RID: 32589
		[RequiredField]
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;
	}
}
