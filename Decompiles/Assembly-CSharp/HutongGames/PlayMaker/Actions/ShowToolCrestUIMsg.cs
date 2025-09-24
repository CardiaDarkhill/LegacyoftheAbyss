using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001393 RID: 5011
	public class ShowToolCrestUIMsg : FsmStateAction
	{
		// Token: 0x060080AE RID: 32942 RVA: 0x0025EEA8 File Offset: 0x0025D0A8
		public override void Reset()
		{
			this.Prefab = null;
			this.Crest = null;
			this.FinishEvent = null;
		}

		// Token: 0x060080AF RID: 32943 RVA: 0x0025EEC0 File Offset: 0x0025D0C0
		public override void OnEnter()
		{
			ToolCrest toolCrest = this.Crest.Value as ToolCrest;
			GameObject value = this.Prefab.Value;
			if (!toolCrest || !value)
			{
				base.Finish();
				return;
			}
			ToolCrestUIMsg.Spawn(toolCrest, value, new Action(this.OnCrestMsgEnd));
		}

		// Token: 0x060080B0 RID: 32944 RVA: 0x0025EF14 File Offset: 0x0025D114
		private void OnCrestMsgEnd()
		{
			base.Fsm.Event(this.FinishEvent);
			base.Finish();
		}

		// Token: 0x04007FF4 RID: 32756
		[CheckForComponent(typeof(ToolCrestUIMsg))]
		[RequiredField]
		public FsmGameObject Prefab;

		// Token: 0x04007FF5 RID: 32757
		[ObjectType(typeof(ToolCrest))]
		[RequiredField]
		public FsmObject Crest;

		// Token: 0x04007FF6 RID: 32758
		public FsmEvent FinishEvent;
	}
}
