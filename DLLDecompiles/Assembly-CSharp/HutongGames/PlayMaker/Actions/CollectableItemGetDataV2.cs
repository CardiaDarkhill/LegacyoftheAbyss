using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200127F RID: 4735
	public class CollectableItemGetDataV2 : CollectableItemAction
	{
		// Token: 0x06007CA1 RID: 31905 RVA: 0x00253F79 File Offset: 0x00252179
		public override void Reset()
		{
			base.Reset();
			this.CollectedAmount = null;
			this.IsHoldingAny = null;
			this.NeededAmount = null;
			this.IsCollected = null;
			this.NotCollected = null;
			this.ItemName = null;
			this.Sprite = null;
		}

		// Token: 0x06007CA2 RID: 31906 RVA: 0x00253FB4 File Offset: 0x002521B4
		protected override void DoAction(CollectableItem item)
		{
			this.ItemName.Value = item.GetDisplayName(CollectableItem.ReadSource.GetPopup);
			this.Sprite.Value = item.GetIcon(CollectableItem.ReadSource.GetPopup);
			int collectedAmount = item.CollectedAmount;
			this.CollectedAmount.Value = collectedAmount;
			this.IsHoldingAny.Value = (collectedAmount > 0);
			int num = this.NeededAmount.IsNone ? 0 : this.NeededAmount.Value;
			base.Fsm.Event((collectedAmount >= num) ? this.IsCollected : this.NotCollected);
		}

		// Token: 0x04007CB4 RID: 31924
		[UIHint(UIHint.Variable)]
		public FsmInt CollectedAmount;

		// Token: 0x04007CB5 RID: 31925
		[UIHint(UIHint.Variable)]
		public FsmBool IsHoldingAny;

		// Token: 0x04007CB6 RID: 31926
		public FsmInt NeededAmount;

		// Token: 0x04007CB7 RID: 31927
		public FsmEvent IsCollected;

		// Token: 0x04007CB8 RID: 31928
		public FsmEvent NotCollected;

		// Token: 0x04007CB9 RID: 31929
		[UIHint(UIHint.Variable)]
		public FsmString ItemName;

		// Token: 0x04007CBA RID: 31930
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Sprite;
	}
}
