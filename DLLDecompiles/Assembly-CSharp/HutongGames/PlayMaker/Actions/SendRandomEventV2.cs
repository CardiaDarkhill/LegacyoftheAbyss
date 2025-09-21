using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D17 RID: 3351
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
	public class SendRandomEventV2 : FsmStateAction
	{
		// Token: 0x060062EB RID: 25323 RVA: 0x001F43F8 File Offset: 0x001F25F8
		public override void Reset()
		{
			this.events = new FsmEvent[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
		}

		// Token: 0x060062EC RID: 25324 RVA: 0x001F4444 File Offset: 0x001F2644
		public override void OnEnter()
		{
			bool flag = false;
			int num = 1000;
			while (!flag)
			{
				num--;
				if (num < 0)
				{
					Debug.LogErrorFormat(base.Owner, "SendRandomEventV2 infinite loop: Owner: {0}, Fsm: {1}, State: {2}", new object[]
					{
						base.Owner.name,
						base.Fsm.Name,
						base.State.Name
					});
					break;
				}
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1 && this.trackingInts[randomWeightedIndex].Value < this.eventMax[randomWeightedIndex].Value)
				{
					int value = ++this.trackingInts[randomWeightedIndex].Value;
					for (int i = 0; i < this.trackingInts.Length; i++)
					{
						this.trackingInts[i].Value = 0;
					}
					this.trackingInts[randomWeightedIndex].Value = value;
					flag = true;
					base.Fsm.Event(this.events[randomWeightedIndex]);
				}
			}
			base.Finish();
		}

		// Token: 0x04006152 RID: 24914
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		// Token: 0x04006153 RID: 24915
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04006154 RID: 24916
		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingInts;

		// Token: 0x04006155 RID: 24917
		public FsmInt[] eventMax;

		// Token: 0x04006156 RID: 24918
		private DelayedEvent delayedEvent;
	}
}
