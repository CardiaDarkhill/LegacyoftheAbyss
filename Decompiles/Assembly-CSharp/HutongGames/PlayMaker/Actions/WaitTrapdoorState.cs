using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001319 RID: 4889
	public sealed class WaitTrapdoorState : FsmStateAction
	{
		// Token: 0x06007EDE RID: 32478 RVA: 0x0025A041 File Offset: 0x00258241
		public override void Reset()
		{
			this.Target = null;
			this.everyFrame = null;
			this.openedEvent = null;
			this.closedEvent = null;
		}

		// Token: 0x06007EDF RID: 32479 RVA: 0x0025A060 File Offset: 0x00258260
		public override void OnEnter()
		{
			this.target = this.Target.GetSafe(this);
			if (this.target != null)
			{
				this.OnUpdate();
			}
			if (!this.everyFrame.Value || this.target == null)
			{
				base.Finish();
			}
		}

		// Token: 0x06007EE0 RID: 32480 RVA: 0x0025A0B4 File Offset: 0x002582B4
		public override void OnUpdate()
		{
			if (this.target.IsOpen)
			{
				base.Fsm.Event(this.openedEvent);
				return;
			}
			base.Fsm.Event(this.closedEvent);
		}

		// Token: 0x04007E88 RID: 32392
		[RequiredField]
		[CheckForComponent(typeof(Trapdoor))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E89 RID: 32393
		public FsmBool everyFrame;

		// Token: 0x04007E8A RID: 32394
		public FsmEvent openedEvent;

		// Token: 0x04007E8B RID: 32395
		public FsmEvent closedEvent;

		// Token: 0x04007E8C RID: 32396
		private Trapdoor target;
	}
}
