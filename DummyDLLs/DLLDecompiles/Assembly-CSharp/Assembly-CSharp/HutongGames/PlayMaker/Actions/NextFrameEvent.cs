using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010A3 RID: 4259
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
	public class NextFrameEvent : FsmStateAction
	{
		// Token: 0x060073AB RID: 29611 RVA: 0x00237D64 File Offset: 0x00235F64
		public override void Reset()
		{
			this.sendEvent = null;
		}

		// Token: 0x060073AC RID: 29612 RVA: 0x00237D6D File Offset: 0x00235F6D
		public override void OnEnter()
		{
			this.enterFrameCount = Time.frameCount;
		}

		// Token: 0x060073AD RID: 29613 RVA: 0x00237D7A File Offset: 0x00235F7A
		public override void OnUpdate()
		{
			if (Time.frameCount == this.enterFrameCount)
			{
				return;
			}
			base.Finish();
			base.Fsm.Event(this.sendEvent);
		}

		// Token: 0x040073D7 RID: 29655
		[Tooltip("The Event to send.")]
		public FsmEvent sendEvent;

		// Token: 0x040073D8 RID: 29656
		private int enterFrameCount;
	}
}
