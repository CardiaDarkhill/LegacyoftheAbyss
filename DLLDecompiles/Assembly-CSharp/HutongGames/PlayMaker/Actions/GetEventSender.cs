using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C69 RID: 3177
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets info on the last event that caused a state change. See also Set Event Data action.")]
	public class GetEventSender : FsmStateAction
	{
		// Token: 0x06005FFC RID: 24572 RVA: 0x001E6639 File Offset: 0x001E4839
		public override void Reset()
		{
			this.sentByGameObject = null;
		}

		// Token: 0x06005FFD RID: 24573 RVA: 0x001E6642 File Offset: 0x001E4842
		public override void OnEnter()
		{
			if (Fsm.EventData.SentByFsm != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
			}
			else
			{
				this.sentByGameObject.Value = null;
			}
			base.Finish();
		}

		// Token: 0x04005D50 RID: 23888
		[UIHint(UIHint.Variable)]
		public FsmGameObject sentByGameObject;
	}
}
