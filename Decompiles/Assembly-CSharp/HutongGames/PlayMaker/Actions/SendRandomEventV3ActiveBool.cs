using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D19 RID: 3353
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
	public class SendRandomEventV3ActiveBool : FsmStateAction
	{
		// Token: 0x060062F1 RID: 25329 RVA: 0x001F4778 File Offset: 0x001F2978
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

		// Token: 0x060062F2 RID: 25330 RVA: 0x001F47C4 File Offset: 0x001F29C4
		public override void OnEnter()
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			if (!this.activeBool.IsNone)
			{
				if (!this.activeBool.Value)
				{
					goto IL_1DD;
				}
			}
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
			IL_1DD:
			base.Finish();
		}

		// Token: 0x0400615F RID: 24927
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		// Token: 0x04006160 RID: 24928
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04006161 RID: 24929
		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingInts;

		// Token: 0x04006162 RID: 24930
		public FsmInt[] eventMax;

		// Token: 0x04006163 RID: 24931
		[UIHint(UIHint.Variable)]
		public FsmInt[] trackingIntsMissed;

		// Token: 0x04006164 RID: 24932
		public FsmInt[] missedMax;

		// Token: 0x04006165 RID: 24933
		[UIHint(UIHint.Variable)]
		public FsmBool activeBool;

		// Token: 0x04006166 RID: 24934
		private int loops;

		// Token: 0x04006167 RID: 24935
		private DelayedEvent delayedEvent;
	}
}
