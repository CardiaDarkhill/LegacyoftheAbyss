using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200135C RID: 4956
	public class SetNPCSpeakingAudioTableV2 : FsmStateAction
	{
		// Token: 0x06007FDD RID: 32733 RVA: 0x0025C9AA File Offset: 0x0025ABAA
		public override void Reset()
		{
			this.Target = null;
			this.SpeakerEvent = new FsmString
			{
				UseVariable = true
			};
			this.Table = null;
		}

		// Token: 0x06007FDE RID: 32734 RVA: 0x0025C9CC File Offset: 0x0025ABCC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				NPCSpeakingAudio component = safe.GetComponent<NPCSpeakingAudio>();
				if (component)
				{
					component.SetTableForSpeaker(this.SpeakerEvent.IsNone ? null : this.SpeakerEvent.Value, this.Table.Value as RandomAudioClipTable);
				}
			}
			base.Finish();
		}

		// Token: 0x04007F4E RID: 32590
		[RequiredField]
		[CheckForComponent(typeof(NPCSpeakingAudio))]
		public FsmOwnerDefault Target;

		// Token: 0x04007F4F RID: 32591
		public FsmString SpeakerEvent;

		// Token: 0x04007F50 RID: 32592
		[RequiredField]
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject Table;
	}
}
