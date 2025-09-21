using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1A RID: 3354
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sends a Random Event picked from an array of Events. Optionally set the relative weight of each event. Use ints to keep events from being fired x times in a row.")]
	public class SendRandomEventV4 : FsmStateAction
	{
		// Token: 0x060062F4 RID: 25332 RVA: 0x001F49BC File Offset: 0x001F2BBC
		public override void Reset()
		{
			this.events = new FsmEvent[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x060062F5 RID: 25333 RVA: 0x001F4A1C File Offset: 0x001F2C1C
		public override void OnEnter()
		{
			if (!this.activeBool.UseVariable)
			{
				Debug.LogWarning("Encountered broken activeBool in SendRandomEventV4! Fixing automatically.", base.Owner);
				this.activeBool.UseVariable = true;
			}
			if (this.activeBool.IsNone || this.activeBool.Value)
			{
				if (!this.setupArrays)
				{
					this.trackingInts = new int[this.eventMax.Length];
					this.trackingIntsMissed = new int[this.missedMax.Length];
					this.setupArrays = true;
				}
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
							if (this.trackingIntsMissed[i] >= this.missedMax[i].Value)
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
								this.trackingInts[j] = 0;
								this.trackingIntsMissed[j]++;
							}
							this.trackingIntsMissed[num] = 0;
							this.trackingInts[num] = 1;
							this.loops = 0;
							base.Fsm.Event(this.events[num]);
						}
						else if (this.trackingInts[randomWeightedIndex] < this.eventMax[randomWeightedIndex].Value)
						{
							int num2 = ++this.trackingInts[randomWeightedIndex];
							for (int k = 0; k < this.trackingInts.Length; k++)
							{
								this.trackingInts[k] = 0;
								this.trackingIntsMissed[k]++;
							}
							this.trackingInts[randomWeightedIndex] = num2;
							this.trackingIntsMissed[randomWeightedIndex] = 0;
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
			}
			base.Finish();
		}

		// Token: 0x04006168 RID: 24936
		[CompoundArray("Events", "Event", "Weight")]
		public FsmEvent[] events;

		// Token: 0x04006169 RID: 24937
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x0400616A RID: 24938
		public FsmInt[] eventMax;

		// Token: 0x0400616B RID: 24939
		public FsmInt[] missedMax;

		// Token: 0x0400616C RID: 24940
		[UIHint(UIHint.Variable)]
		public FsmBool activeBool;

		// Token: 0x0400616D RID: 24941
		private int[] trackingInts;

		// Token: 0x0400616E RID: 24942
		private int[] trackingIntsMissed;

		// Token: 0x0400616F RID: 24943
		private bool setupArrays;

		// Token: 0x04006170 RID: 24944
		private int loops;

		// Token: 0x04006171 RID: 24945
		private DelayedEvent delayedEvent;
	}
}
