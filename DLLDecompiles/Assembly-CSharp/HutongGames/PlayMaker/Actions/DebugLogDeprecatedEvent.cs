using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0B RID: 3083
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs a detailed deprecation message.")]
	public class DebugLogDeprecatedEvent : BaseLogAction
	{
		// Token: 0x06005E0F RID: 24079 RVA: 0x001DA507 File Offset: 0x001D8707
		public override void Reset()
		{
			this.noteText = "";
			base.Reset();
		}

		// Token: 0x06005E10 RID: 24080 RVA: 0x001DA520 File Offset: 0x001D8720
		public override void OnEnter()
		{
			string arg;
			if (Fsm.EventData.SentByFsm == null)
			{
				arg = "<native-code>";
			}
			else
			{
				arg = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					(Fsm.EventData.SentByFsm.GameObject == null) ? "<unknown-game-object>" : Fsm.EventData.SentByFsm.GameObject.name,
					Fsm.EventData.SentByFsm.Name,
					(Fsm.EventData.SentByState == null) ? "<unknown-state>" : Fsm.EventData.SentByState.Name,
					(Fsm.EventData.SentByAction == null) ? "<unknown-action>" : Fsm.EventData.SentByAction.Name
				});
			}
			PlayMakerFSM playMakerFSM = base.Fsm.Owner as PlayMakerFSM;
			string arg2;
			if (playMakerFSM == null)
			{
				arg2 = "<no-owner>";
			}
			else
			{
				arg2 = string.Format("{0}.{1}.{2}", playMakerFSM.gameObject.name, base.Fsm.Name, base.Fsm.ActiveStateName);
			}
			string value = this.noteText.Value;
			string text = string.Format("Entry to {0} (sent by {1}) is deprecated", arg2, arg);
			if (!string.IsNullOrEmpty(value))
			{
				text = text + ": " + value;
			}
			Debug.LogError(text, playMakerFSM);
			base.Finish();
		}

		// Token: 0x04005A71 RID: 23153
		[Tooltip("Text to send to the log.")]
		public FsmString noteText;
	}
}
