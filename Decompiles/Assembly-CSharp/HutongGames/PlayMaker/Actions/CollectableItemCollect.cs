using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200127B RID: 4731
	public class CollectableItemCollect : CollectableItemAction
	{
		// Token: 0x06007C95 RID: 31893 RVA: 0x00253DA4 File Offset: 0x00251FA4
		public override void Reset()
		{
			base.Reset();
			this.Amount = new FsmInt(1);
		}

		// Token: 0x06007C96 RID: 31894 RVA: 0x00253DBD File Offset: 0x00251FBD
		protected override void DoAction(CollectableItem item)
		{
			item.Collect(this.Amount.IsNone ? 1 : this.Amount.Value, true);
		}

		// Token: 0x04007CA8 RID: 31912
		public FsmInt Amount;
	}
}
