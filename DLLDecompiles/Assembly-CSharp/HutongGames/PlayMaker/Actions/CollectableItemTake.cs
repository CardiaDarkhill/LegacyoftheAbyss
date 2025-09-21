using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200127C RID: 4732
	public class CollectableItemTake : CollectableItemAction
	{
		// Token: 0x06007C98 RID: 31896 RVA: 0x00253DE9 File Offset: 0x00251FE9
		public override void Reset()
		{
			base.Reset();
			this.Amount = 1;
			this.ShowCounter = true;
		}

		// Token: 0x06007C99 RID: 31897 RVA: 0x00253E09 File Offset: 0x00252009
		protected override void DoAction(CollectableItem item)
		{
			item.Take(this.Amount.IsNone ? 1 : this.Amount.Value, this.ShowCounter.Value);
		}

		// Token: 0x04007CA9 RID: 31913
		public FsmInt Amount;

		// Token: 0x04007CAA RID: 31914
		public FsmBool ShowCounter;
	}
}
