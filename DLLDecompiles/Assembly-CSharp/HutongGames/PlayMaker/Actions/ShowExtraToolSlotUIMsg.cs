using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001368 RID: 4968
	public class ShowExtraToolSlotUIMsg : FsmStateAction
	{
		// Token: 0x0600801D RID: 32797 RVA: 0x0025D6DF File Offset: 0x0025B8DF
		public override void Reset()
		{
			this.Prefab = null;
			this.UnlockedSlotType = null;
			this.FinishEvent = null;
		}

		// Token: 0x0600801E RID: 32798 RVA: 0x0025D6F8 File Offset: 0x0025B8F8
		public override void OnEnter()
		{
			GameObject value = this.Prefab.Value;
			if (!value)
			{
				base.Finish();
				return;
			}
			ExtraToolSlotUIMsg.Spawn((ToolItemType)this.UnlockedSlotType.Value, value, new Action(this.OnCrestMsgEnd));
		}

		// Token: 0x0600801F RID: 32799 RVA: 0x0025D742 File Offset: 0x0025B942
		private void OnCrestMsgEnd()
		{
			base.Fsm.Event(this.FinishEvent);
			base.Finish();
		}

		// Token: 0x04007F85 RID: 32645
		[CheckForComponent(typeof(ExtraToolSlotUIMsg))]
		[RequiredField]
		public FsmGameObject Prefab;

		// Token: 0x04007F86 RID: 32646
		[ObjectType(typeof(ToolItemType))]
		public FsmEnum UnlockedSlotType;

		// Token: 0x04007F87 RID: 32647
		public FsmEvent FinishEvent;
	}
}
