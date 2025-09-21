using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200127A RID: 4730
	public abstract class CollectableItemAction : FsmStateAction
	{
		// Token: 0x06007C91 RID: 31889 RVA: 0x00253D5F File Offset: 0x00251F5F
		public override void Reset()
		{
			this.Item = null;
		}

		// Token: 0x06007C92 RID: 31890 RVA: 0x00253D68 File Offset: 0x00251F68
		public override void OnEnter()
		{
			CollectableItem collectableItem = this.Item.Value as CollectableItem;
			if (collectableItem != null)
			{
				this.DoAction(collectableItem);
			}
			base.Finish();
		}

		// Token: 0x06007C93 RID: 31891
		protected abstract void DoAction(CollectableItem item);

		// Token: 0x04007CA7 RID: 31911
		[ObjectType(typeof(CollectableItem))]
		public FsmObject Item;
	}
}
