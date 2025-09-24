using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D03 RID: 3331
	[ActionCategory("System")]
	[Tooltip("Tell the Garbage Collector to run.")]
	public class RunGarbageCollector : FsmStateAction
	{
		// Token: 0x060062A2 RID: 25250 RVA: 0x001F2F7F File Offset: 0x001F117F
		public override void Reset()
		{
			this.finishEvent = null;
		}

		// Token: 0x060062A3 RID: 25251 RVA: 0x001F2F88 File Offset: 0x001F1188
		public override void OnEnter()
		{
			GCManager.Collect();
			if (this.finishEvent != null)
			{
				base.Fsm.Event(this.finishEvent);
			}
			base.Finish();
		}

		// Token: 0x0400610C RID: 24844
		public FsmEvent finishEvent;
	}
}
