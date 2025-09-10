using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001377 RID: 4983
	public class GetInventoryPaneCount : FsmStateAction
	{
		// Token: 0x06008050 RID: 32848 RVA: 0x0025E26F File Offset: 0x0025C46F
		public override void Reset()
		{
			this.paneListChild = null;
			this.UnlockedPaneCount = null;
		}

		// Token: 0x06008051 RID: 32849 RVA: 0x0025E280 File Offset: 0x0025C480
		public override void OnEnter()
		{
			GameObject safe = this.paneListChild.GetSafe(this);
			if (safe != null)
			{
				InventoryPaneList componentInParent = safe.GetComponentInParent<InventoryPaneList>();
				if (componentInParent != null)
				{
					int unlockedPaneCount = componentInParent.UnlockedPaneCount;
					this.UnlockedPaneCount.Value = unlockedPaneCount;
					base.Fsm.Event((unlockedPaneCount <= 1) ? this.SinglePaneEvent : this.ManyPaneEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007FBE RID: 32702
		[Tooltip("Will find the pane list in the parent of this target.")]
		public FsmOwnerDefault paneListChild;

		// Token: 0x04007FBF RID: 32703
		[UIHint(UIHint.Variable)]
		public FsmInt UnlockedPaneCount;

		// Token: 0x04007FC0 RID: 32704
		public FsmEvent SinglePaneEvent;

		// Token: 0x04007FC1 RID: 32705
		public FsmEvent ManyPaneEvent;
	}
}
