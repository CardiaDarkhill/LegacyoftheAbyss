using System;
using JetBrains.Annotations;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC4 RID: 3268
	[UsedImplicitly]
	public class NextFixedFrameEvent : FsmStateAction
	{
		// Token: 0x0600618F RID: 24975 RVA: 0x001EE7AA File Offset: 0x001EC9AA
		public override void Reset()
		{
			this.SendEvent = null;
		}

		// Token: 0x06006190 RID: 24976 RVA: 0x001EE7B3 File Offset: 0x001EC9B3
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x001EE7C1 File Offset: 0x001EC9C1
		public override void OnFixedUpdate()
		{
			base.Finish();
			base.Fsm.Event(this.SendEvent);
		}

		// Token: 0x04005FBE RID: 24510
		public FsmEvent SendEvent;
	}
}
