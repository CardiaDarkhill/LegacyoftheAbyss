using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001289 RID: 4745
	public class CollectableRelicCheck : FsmStateAction
	{
		// Token: 0x06007CC0 RID: 31936 RVA: 0x0025445C File Offset: 0x0025265C
		public override void Reset()
		{
			this.Relic = null;
			this.StoreIsCollected = null;
			this.StoreIsDeposited = null;
			this.StoreIsInInventory = null;
		}

		// Token: 0x06007CC1 RID: 31937 RVA: 0x0025447C File Offset: 0x0025267C
		public override void OnEnter()
		{
			CollectableRelic collectableRelic = this.Relic.Value as CollectableRelic;
			if (collectableRelic != null)
			{
				CollectableRelicsData.Data savedData = collectableRelic.SavedData;
				bool isCollected = savedData.IsCollected;
				bool isDeposited = savedData.IsDeposited;
				bool isInInventory = collectableRelic.IsInInventory;
				this.StoreIsCollected.Value = isCollected;
				this.StoreIsDeposited.Value = isDeposited;
				this.StoreIsInInventory = isInInventory;
				if (isCollected)
				{
					base.Fsm.Event(this.IsCollectedEvent);
				}
				else
				{
					base.Fsm.Event(this.NotCollectedEvent);
				}
				if (isDeposited)
				{
					base.Fsm.Event(this.IsDepositedEvent);
				}
				if (isInInventory)
				{
					base.Fsm.Event(this.IsInInventoryEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04007CD5 RID: 31957
		[ObjectType(typeof(CollectableRelic))]
		[RequiredField]
		public FsmObject Relic;

		// Token: 0x04007CD6 RID: 31958
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsCollected;

		// Token: 0x04007CD7 RID: 31959
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsDeposited;

		// Token: 0x04007CD8 RID: 31960
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsInInventory;

		// Token: 0x04007CD9 RID: 31961
		public FsmEvent NotCollectedEvent;

		// Token: 0x04007CDA RID: 31962
		public FsmEvent IsCollectedEvent;

		// Token: 0x04007CDB RID: 31963
		public FsmEvent IsDepositedEvent;

		// Token: 0x04007CDC RID: 31964
		public FsmEvent IsInInventoryEvent;
	}
}
