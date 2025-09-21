using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC9 RID: 3017
	public class GetBitmaskAtIndex : FsmStateAction
	{
		// Token: 0x06005CAD RID: 23725 RVA: 0x001D2AE7 File Offset: 0x001D0CE7
		public override void Reset()
		{
			this.ReadMask = null;
			this.Index = null;
			this.StoreValue = null;
			this.TrueEvent = null;
			this.FalseEvent = null;
		}

		// Token: 0x06005CAE RID: 23726 RVA: 0x001D2B0C File Offset: 0x001D0D0C
		public override void OnEnter()
		{
			int num = 1 << this.Index.Value;
			bool flag = (this.ReadMask.Value & num) == num;
			this.StoreValue.Value = flag;
			base.Fsm.Event(flag ? this.TrueEvent : this.FalseEvent);
			base.Finish();
		}

		// Token: 0x04005843 RID: 22595
		public FsmInt ReadMask;

		// Token: 0x04005844 RID: 22596
		public FsmInt Index;

		// Token: 0x04005845 RID: 22597
		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		// Token: 0x04005846 RID: 22598
		public FsmEvent TrueEvent;

		// Token: 0x04005847 RID: 22599
		public FsmEvent FalseEvent;
	}
}
