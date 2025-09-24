using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B18 RID: 2840
	public class WaitIfPoolCreation : FsmStateAction
	{
		// Token: 0x06005954 RID: 22868 RVA: 0x001C5096 File Offset: 0x001C3296
		public override void Reset()
		{
			this.isCreating = null;
			this.isNotCreating = null;
		}

		// Token: 0x06005955 RID: 22869 RVA: 0x001C50A6 File Offset: 0x001C32A6
		public override void OnEnter()
		{
			if (!ObjectPool.IsCreatingPool)
			{
				base.Fsm.Event(this.isNotCreating);
				base.Finish();
				return;
			}
			base.Fsm.Event(this.isCreating);
		}

		// Token: 0x06005956 RID: 22870 RVA: 0x001C50D8 File Offset: 0x001C32D8
		public override void OnUpdate()
		{
			if (!ObjectPool.IsCreatingPool)
			{
				base.Fsm.Event(this.isNotCreating);
				base.Finish();
			}
		}

		// Token: 0x040054B6 RID: 21686
		public FsmEvent isCreating;

		// Token: 0x040054B7 RID: 21687
		public FsmEvent isNotCreating;
	}
}
