using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200107C RID: 4220
	[Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event to all FSMs in the scene or to all FSMs on a Game Object. NOTE: This action won't work on the very first frame of the game...")]
	public class BroadcastEvent : FsmStateAction
	{
		// Token: 0x0600730C RID: 29452 RVA: 0x00235DA1 File Offset: 0x00233FA1
		public override void Reset()
		{
			this.broadcastEvent = null;
			this.gameObject = null;
			this.sendToChildren = false;
			this.excludeSelf = false;
		}

		// Token: 0x0600730D RID: 29453 RVA: 0x00235DCC File Offset: 0x00233FCC
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.broadcastEvent.Value))
			{
				if (this.gameObject.Value != null)
				{
					base.Fsm.BroadcastEventToGameObject(this.gameObject.Value, this.broadcastEvent.Value, this.sendToChildren.Value, this.excludeSelf.Value);
				}
				else
				{
					base.Fsm.BroadcastEvent(this.broadcastEvent.Value, this.excludeSelf.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007311 RID: 29457
		[RequiredField]
		[Tooltip("The event to broadcast.")]
		public FsmString broadcastEvent;

		// Token: 0x04007312 RID: 29458
		[Tooltip("By default, the event is broadcast to all FSMs in the scene. Optionally you can specify a game object to target. The event will then be broadcast to all FSMs on that game object.")]
		public FsmGameObject gameObject;

		// Token: 0x04007313 RID: 29459
		[Tooltip("Broadcast the event to all the Game Object's children too.")]
		public FsmBool sendToChildren;

		// Token: 0x04007314 RID: 29460
		[Tooltip("Don't send the event to self.")]
		public FsmBool excludeSelf;
	}
}
