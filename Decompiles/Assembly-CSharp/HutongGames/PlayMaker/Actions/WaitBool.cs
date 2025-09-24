using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA5 RID: 3493
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Delays a State from finishing by the specified time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
	public class WaitBool : FsmStateAction
	{
		// Token: 0x06006570 RID: 25968 RVA: 0x002001E7 File Offset: 0x001FE3E7
		public override void Reset()
		{
			this.boolTest = new FsmBool
			{
				UseVariable = true
			};
			this.time = 1f;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006571 RID: 25969 RVA: 0x0020021C File Offset: 0x001FE41C
		public override void OnEnter()
		{
			if (!this.boolTest.Value)
			{
				base.Finish();
			}
			if (this.time.Value <= 0f && this.boolTest.Value)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x06006572 RID: 25970 RVA: 0x0020028C File Offset: 0x001FE48C
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer >= this.time.Value)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x04006470 RID: 25712
		[RequiredField]
		public FsmBool boolTest;

		// Token: 0x04006471 RID: 25713
		public FsmFloat time;

		// Token: 0x04006472 RID: 25714
		public FsmEvent finishEvent;

		// Token: 0x04006473 RID: 25715
		public bool realTime;

		// Token: 0x04006474 RID: 25716
		private float startTime;

		// Token: 0x04006475 RID: 25717
		private float timer;
	}
}
