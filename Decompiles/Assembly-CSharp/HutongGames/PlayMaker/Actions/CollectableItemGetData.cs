using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200127E RID: 4734
	public class CollectableItemGetData : CollectableItemAction
	{
		// Token: 0x06007C9E RID: 31902 RVA: 0x00253EC1 File Offset: 0x002520C1
		public override void Reset()
		{
			base.Reset();
			this.CollectedAmount = null;
			this.NeededAmount = null;
			this.IsCollected = null;
			this.NotCollected = null;
			this.ItemName = null;
			this.Sprite = null;
		}

		// Token: 0x06007C9F RID: 31903 RVA: 0x00253EF4 File Offset: 0x002520F4
		protected override void DoAction(CollectableItem item)
		{
			this.ItemName.Value = item.GetDisplayName(CollectableItem.ReadSource.GetPopup);
			this.Sprite.Value = item.GetIcon(CollectableItem.ReadSource.GetPopup);
			int collectedAmount = item.CollectedAmount;
			this.CollectedAmount.Value = collectedAmount;
			int num = this.NeededAmount.IsNone ? 0 : this.NeededAmount.Value;
			base.Fsm.Event((collectedAmount >= num) ? this.IsCollected : this.NotCollected);
		}

		// Token: 0x04007CAE RID: 31918
		[UIHint(UIHint.Variable)]
		public FsmInt CollectedAmount;

		// Token: 0x04007CAF RID: 31919
		public FsmInt NeededAmount;

		// Token: 0x04007CB0 RID: 31920
		public FsmEvent IsCollected;

		// Token: 0x04007CB1 RID: 31921
		public FsmEvent NotCollected;

		// Token: 0x04007CB2 RID: 31922
		[UIHint(UIHint.Variable)]
		public FsmString ItemName;

		// Token: 0x04007CB3 RID: 31923
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Sprite;
	}
}
