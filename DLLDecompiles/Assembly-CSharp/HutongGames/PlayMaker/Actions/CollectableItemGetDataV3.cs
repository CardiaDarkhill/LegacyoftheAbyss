using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001280 RID: 4736
	public class CollectableItemGetDataV3 : CollectableItemAction
	{
		// Token: 0x06007CA4 RID: 31908 RVA: 0x00254048 File Offset: 0x00252248
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
			this.ReadSource = null;
		}

		// Token: 0x06007CA5 RID: 31909 RVA: 0x00254088 File Offset: 0x00252288
		protected override void DoAction(CollectableItem item)
		{
			CollectableItem.ReadSource readSource = (CollectableItem.ReadSource)this.ReadSource.Value;
			this.ItemName.Value = item.GetDisplayName(readSource);
			this.Sprite.Value = item.GetIcon(readSource);
			int collectedAmount = item.CollectedAmount;
			this.CollectedAmount.Value = collectedAmount;
			this.IsHoldingAny.Value = (collectedAmount > 0);
			int num = this.NeededAmount.IsNone ? 0 : this.NeededAmount.Value;
			base.Fsm.Event((collectedAmount >= num) ? this.IsCollected : this.NotCollected);
		}

		// Token: 0x04007CBB RID: 31931
		[UIHint(UIHint.Variable)]
		public FsmInt CollectedAmount;

		// Token: 0x04007CBC RID: 31932
		[UIHint(UIHint.Variable)]
		public FsmBool IsHoldingAny;

		// Token: 0x04007CBD RID: 31933
		public FsmInt NeededAmount;

		// Token: 0x04007CBE RID: 31934
		public FsmEvent IsCollected;

		// Token: 0x04007CBF RID: 31935
		public FsmEvent NotCollected;

		// Token: 0x04007CC0 RID: 31936
		[UIHint(UIHint.Variable)]
		public FsmString ItemName;

		// Token: 0x04007CC1 RID: 31937
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Sprite))]
		public FsmObject Sprite;

		// Token: 0x04007CC2 RID: 31938
		[ObjectType(typeof(CollectableItem.ReadSource))]
		public FsmEnum ReadSource;
	}
}
