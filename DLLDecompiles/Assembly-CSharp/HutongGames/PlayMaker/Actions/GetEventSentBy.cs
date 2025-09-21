using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001086 RID: 4230
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Gets the sender of the last event.")]
	public class GetEventSentBy : FsmStateAction
	{
		// Token: 0x0600732F RID: 29487 RVA: 0x002364CD File Offset: 0x002346CD
		public override void Reset()
		{
			this.sentByGameObject = null;
			this.gameObjectName = null;
			this.fsmName = null;
		}

		// Token: 0x06007330 RID: 29488 RVA: 0x002364E4 File Offset: 0x002346E4
		public override void OnEnter()
		{
			if (Fsm.EventData.SentByGameObject != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByGameObject;
			}
			else if (Fsm.EventData.SentByFsm != null)
			{
				this.sentByGameObject.Value = Fsm.EventData.SentByFsm.GameObject;
				this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
			}
			else
			{
				this.sentByGameObject.Value = null;
				this.fsmName.Value = "";
			}
			if (this.sentByGameObject.Value != null)
			{
				this.gameObjectName.Value = this.sentByGameObject.Value.name;
			}
			base.Finish();
		}

		// Token: 0x0400733A RID: 29498
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the GameObject that sent the event.")]
		public FsmGameObject sentByGameObject;

		// Token: 0x0400733B RID: 29499
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the name of the GameObject that sent the event.")]
		public FsmString gameObjectName;

		// Token: 0x0400733C RID: 29500
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the name of the FSM that sent the event.")]
		public FsmString fsmName;
	}
}
