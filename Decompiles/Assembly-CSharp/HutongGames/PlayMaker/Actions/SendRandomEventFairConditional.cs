using System;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D16 RID: 3350
	[Tooltip("Sends a Random Event picked from an array of Events. Will optionally decrease the probability of the same event firing in succession, making it seem more \"fair\".")]
	public class SendRandomEventFairConditional : FsmStateAction
	{
		// Token: 0x060062E8 RID: 25320 RVA: 0x001F4291 File Offset: 0x001F2491
		public override void Reset()
		{
			this.Events = null;
			this.TrackingArray = null;
			this.MissedMultiplier = 2f;
		}

		// Token: 0x060062E9 RID: 25321 RVA: 0x001F42B4 File Offset: 0x001F24B4
		public override void OnEnter()
		{
			float[] array = this.TrackingArray.IsNone ? this.selfTrackingArray : this.TrackingArray.floatValues;
			if (array == null || array.Length != this.Events.Length)
			{
				array = (from e in this.Events
				select e.Probability).ToArray<float>();
			}
			if (this.conditionArray == null || this.conditionArray.Length != this.Events.Length)
			{
				this.conditionArray = (from e in this.Events
				select e.Condition.IsNone || e.Condition.Value == e.TargetCondition.Value).ToArray<bool>();
			}
			int num;
			FsmEvent randomItemByProbability = Probability.GetRandomItemByProbability<SendRandomEventFairConditional.ProbabilityFsmEvent, FsmEvent>(this.Events, out num, array, this.conditionArray);
			for (int i = 0; i < array.Length; i++)
			{
				if (i == num)
				{
					array[i] = this.Events[i].Probability;
				}
				else
				{
					array[i] *= this.MissedMultiplier.Value;
				}
			}
			if (!this.TrackingArray.IsNone)
			{
				this.TrackingArray.floatValues = array;
			}
			if (randomItemByProbability != null)
			{
				base.Fsm.Event(randomItemByProbability);
			}
			base.Finish();
		}

		// Token: 0x0400614D RID: 24909
		public SendRandomEventFairConditional.ProbabilityFsmEvent[] Events;

		// Token: 0x0400614E RID: 24910
		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray TrackingArray;

		// Token: 0x0400614F RID: 24911
		public FsmFloat MissedMultiplier;

		// Token: 0x04006150 RID: 24912
		private float[] selfTrackingArray;

		// Token: 0x04006151 RID: 24913
		private bool[] conditionArray;

		// Token: 0x02001B8E RID: 7054
		[Serializable]
		public class ProbabilityFsmEvent : Probability.ProbabilityBase<FsmEvent>
		{
			// Token: 0x170011A8 RID: 4520
			// (get) Token: 0x06009A44 RID: 39492 RVA: 0x002B2FB7 File Offset: 0x002B11B7
			public override FsmEvent Item
			{
				get
				{
					return this.SendEvent;
				}
			}

			// Token: 0x04009D8E RID: 40334
			public FsmBool Condition = new FsmBool
			{
				UseVariable = true
			};

			// Token: 0x04009D8F RID: 40335
			public FsmBool TargetCondition = true;

			// Token: 0x04009D90 RID: 40336
			public FsmEvent SendEvent;
		}
	}
}
