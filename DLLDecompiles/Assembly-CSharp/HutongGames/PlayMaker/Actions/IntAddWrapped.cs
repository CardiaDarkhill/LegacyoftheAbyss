using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8E RID: 3214
	public class IntAddWrapped : FsmStateAction
	{
		// Token: 0x0600609E RID: 24734 RVA: 0x001EA216 File Offset: 0x001E8416
		public override void Reset()
		{
			this.Value = null;
			this.AddValue = null;
			this.Min = null;
			this.Max = null;
			this.InclusiveMax = null;
			this.StoreResult = null;
		}

		// Token: 0x0600609F RID: 24735 RVA: 0x001EA244 File Offset: 0x001E8444
		public override void OnEnter()
		{
			int num = this.Value.Value;
			int value = this.AddValue.Value;
			num += value;
			int value2 = this.Min.Value;
			int num2 = this.Max.Value;
			if (this.InclusiveMax.Value)
			{
				num2++;
			}
			if (num < value2)
			{
				num = num2 - (value2 - num) % (num2 - value2);
			}
			else
			{
				num = value2 + (num - value2) % (num2 - value2);
			}
			this.StoreResult.Value = num;
			base.Finish();
		}

		// Token: 0x04005E1D RID: 24093
		public FsmInt Value;

		// Token: 0x04005E1E RID: 24094
		public FsmInt AddValue;

		// Token: 0x04005E1F RID: 24095
		public FsmInt Min;

		// Token: 0x04005E20 RID: 24096
		public FsmInt Max;

		// Token: 0x04005E21 RID: 24097
		public FsmBool InclusiveMax;

		// Token: 0x04005E22 RID: 24098
		public FsmInt StoreResult;
	}
}
