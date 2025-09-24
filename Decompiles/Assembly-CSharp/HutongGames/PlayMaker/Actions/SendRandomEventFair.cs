using System;
using System.Linq;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D15 RID: 3349
	[Tooltip("Sends a Random Event picked from an array of Events. Will optionally decrease the probability of the same event firing in succession, making it seem more \"fair\".")]
	public class SendRandomEventFair : FsmStateAction
	{
		// Token: 0x060062E4 RID: 25316 RVA: 0x001F4172 File Offset: 0x001F2372
		public bool IsNotUsingTrackingArray()
		{
			return this.TrackingArray.IsNone;
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x001F417F File Offset: 0x001F237F
		public override void Reset()
		{
			this.Events = null;
			this.TrackingArray = null;
			this.MissedMultiplier = 2f;
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x001F41A0 File Offset: 0x001F23A0
		public override void OnEnter()
		{
			float[] array = null;
			if (!this.TrackingArray.IsNone)
			{
				if (this.TrackingArray.floatValues.Length != this.Events.Length)
				{
					array = (from e in this.Events
					select e.Probability).ToArray<float>();
				}
				else
				{
					array = this.TrackingArray.floatValues;
				}
			}
			int num;
			FsmEvent randomItemByProbability = Probability.GetRandomItemByProbability<SendRandomEventFair.ProbabilityFsmEvent, FsmEvent>(this.Events, out num, array, null);
			if (!this.TrackingArray.IsNone)
			{
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
				this.TrackingArray.floatValues = array;
			}
			if (randomItemByProbability != null)
			{
				base.Fsm.Event(randomItemByProbability);
			}
			base.Finish();
		}

		// Token: 0x0400614A RID: 24906
		public SendRandomEventFair.ProbabilityFsmEvent[] Events;

		// Token: 0x0400614B RID: 24907
		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray TrackingArray;

		// Token: 0x0400614C RID: 24908
		[HideIf("IsNotUsingTrackingArray")]
		public FsmFloat MissedMultiplier;

		// Token: 0x02001B8C RID: 7052
		[Serializable]
		public class ProbabilityFsmEvent : Probability.ProbabilityBase<FsmEvent>
		{
			// Token: 0x170011A7 RID: 4519
			// (get) Token: 0x06009A3F RID: 39487 RVA: 0x002B2F8B File Offset: 0x002B118B
			public override FsmEvent Item
			{
				get
				{
					return this.SendEvent;
				}
			}

			// Token: 0x04009D8B RID: 40331
			public FsmEvent SendEvent;
		}
	}
}
