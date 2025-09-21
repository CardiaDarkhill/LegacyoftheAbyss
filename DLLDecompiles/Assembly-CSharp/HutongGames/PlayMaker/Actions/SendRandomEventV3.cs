using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D18 RID: 3352
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
	public class SendRandomEventV3 : FsmStateAction
	{
		// Token: 0x060062EE RID: 25326 RVA: 0x001F4554 File Offset: 0x001F2754
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

		// Token: 0x060062EF RID: 25327 RVA: 0x001F45A0 File Offset: 0x001F27A0
		public override void OnEnter()
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			while (!flag)
			{
				int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
				if (randomWeightedIndex != -1)
				{
					for (int i = 0; i < this.trackingIntsMissed.Length; i++)
					{
						if (this.trackingIntsMissed[i].Value >= this.missedMax[i].Value)
						{
							flag2 = true;
							num = i;
						}
					}
					if (flag2)
					{
						flag = true;
						for (int j = 0; j < this.trackingInts.Length; j++)
						{
							this.trackingInts[j].Value = 0;
							this.trackingIntsMissed[j].Value++;
						}
						this.trackingIntsMissed[num].Value = 0;
						this.trackingInts[num].Value = 1;
						this.loops = 0;
						base.Fsm.Event(this.events[num]);
					}
					else if (this.trackingInts[randomWeightedIndex].Value < this.eventMax[randomWeightedIndex].Value)
					{
						int value = ++this.trackingInts[randomWeightedIndex].Value;
						for (int k = 0; k < this.trackingInts.Length; k++)
						{
							this.trackingInts[k].Value = 0;
							this.trackingIntsMissed[k].Value++;
						}
						this.trackingInts[randomWeightedIndex].Value = value;
						this.trackingIntsMissed[randomWeightedIndex].Value = 0;
						flag = true;
						this.loops = 0;
						base.Fsm.Event(this.events[randomWeightedIndex]);
					}
				}
				this.loops++;
				if (this.loops > 100)
				{
					base.Fsm.Event(this.events[0]);
					flag = true;
					base.Finish();
				}
			}
			base.Finish();
		}

		// Token: 0x04006157 RID: 24919
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		// Token: 0x04006158 RID: 24920
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04006159 RID: 24921
		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingInts;

		// Token: 0x0400615A RID: 24922
		public FsmInt[] eventMax;

		// Token: 0x0400615B RID: 24923
		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingIntsMissed;

		// Token: 0x0400615C RID: 24924
		public FsmInt[] missedMax;

		// Token: 0x0400615D RID: 24925
		private int loops;

		// Token: 0x0400615E RID: 24926
		private DelayedEvent delayedEvent;
	}
}
