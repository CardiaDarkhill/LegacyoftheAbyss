using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200127D RID: 4733
	public class CollectableItemTakeV2 : CollectableItemAction
	{
		// Token: 0x06007C9B RID: 31899 RVA: 0x00253E3F File Offset: 0x0025203F
		public override void Reset()
		{
			base.Reset();
			this.Amount = 1;
			this.ShowCounter = true;
			this.TakeDisplay = null;
		}

		// Token: 0x06007C9C RID: 31900 RVA: 0x00253E68 File Offset: 0x00252068
		protected override void DoAction(CollectableItem item)
		{
			item.Take(this.Amount.IsNone ? 1 : this.Amount.Value, this.ShowCounter.Value);
			TakeItemTypes takeItemType = (TakeItemTypes)this.TakeDisplay.Value;
			CollectableUIMsg.ShowTakeMsg(item, takeItemType);
		}

		// Token: 0x04007CAB RID: 31915
		public FsmInt Amount;

		// Token: 0x04007CAC RID: 31916
		public FsmBool ShowCounter;

		// Token: 0x04007CAD RID: 31917
		[ObjectType(typeof(TakeItemTypes))]
		public FsmEnum TakeDisplay;
	}
}
