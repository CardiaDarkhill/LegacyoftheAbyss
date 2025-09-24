using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B57 RID: 2903
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Send event to all the GameObjects within an arrayList.")]
	public class ArrayListSendEventToGameObjects : ArrayListActions
	{
		// Token: 0x06005A58 RID: 23128 RVA: 0x001C9084 File Offset: 0x001C7284
		public override void Reset()
		{
			this.eventTarget = new FsmEventTarget();
			this.eventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
			this.gameObject = null;
			this.reference = null;
			this.sendEvent = null;
			this.excludeSelf = false;
			this.sendToChildren = false;
		}

		// Token: 0x06005A59 RID: 23129 RVA: 0x001C90D5 File Offset: 0x001C72D5
		public override void OnEnter()
		{
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				base.Finish();
			}
			this.DoSendEvent();
		}

		// Token: 0x06005A5A RID: 23130 RVA: 0x001C9108 File Offset: 0x001C7308
		private void DoSendEvent()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			foreach (object obj in this.proxy.arrayList)
			{
				GameObject go = (GameObject)obj;
				this.sendEventToGO(go);
			}
		}

		// Token: 0x06005A5B RID: 23131 RVA: 0x001C9170 File Offset: 0x001C7370
		private void sendEventToGO(GameObject _go)
		{
			FsmEventTarget fsmEventTarget = new FsmEventTarget();
			fsmEventTarget.excludeSelf = this.excludeSelf.Value;
			fsmEventTarget.gameObject = new FsmOwnerDefault
			{
				OwnerOption = OwnerDefaultOption.SpecifyGameObject,
				GameObject = new FsmGameObject(),
				GameObject = 
				{
					Value = _go
				}
			};
			fsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
			fsmEventTarget.sendToChildren = this.sendToChildren.Value;
			base.Fsm.Event(fsmEventTarget, this.sendEvent);
		}

		// Token: 0x040055FA RID: 22010
		[ActionSection("Set up")]
		public FsmEventTarget eventTarget;

		// Token: 0x040055FB RID: 22011
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055FC RID: 22012
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055FD RID: 22013
		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmEvent sendEvent;

		// Token: 0x040055FE RID: 22014
		public FsmBool excludeSelf;

		// Token: 0x040055FF RID: 22015
		public FsmBool sendToChildren;
	}
}
