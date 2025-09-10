using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001281 RID: 4737
	public class SetCollectablePickupItem : FsmStateAction
	{
		// Token: 0x06007CA7 RID: 31911 RVA: 0x0025412D File Offset: 0x0025232D
		public override void Reset()
		{
			this.Target = null;
			this.Item = null;
		}

		// Token: 0x06007CA8 RID: 31912 RVA: 0x00254140 File Offset: 0x00252340
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				CollectableItemPickup component = safe.GetComponent<CollectableItemPickup>();
				if (component)
				{
					component.SetItem(this.Item.Value as SavedItem, false);
				}
			}
			base.Finish();
		}

		// Token: 0x04007CC3 RID: 31939
		public FsmOwnerDefault Target;

		// Token: 0x04007CC4 RID: 31940
		[ObjectType(typeof(SavedItem))]
		public FsmObject Item;
	}
}
