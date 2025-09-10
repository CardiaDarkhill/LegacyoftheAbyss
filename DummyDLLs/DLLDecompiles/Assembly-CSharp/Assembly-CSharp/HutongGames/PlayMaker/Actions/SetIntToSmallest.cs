using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D31 RID: 3377
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of an Int Variable to the smallest of two values.")]
	public class SetIntToSmallest : FsmStateAction
	{
		// Token: 0x0600635E RID: 25438 RVA: 0x001F601D File Offset: 0x001F421D
		public override void Reset()
		{
			this.intVariable = null;
			this.value1 = null;
			this.value2 = null;
			this.everyFrame = false;
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x001F603C File Offset: 0x001F423C
		public override void OnEnter()
		{
			if (this.value1.Value < this.value2.Value)
			{
				this.intVariable.Value = this.value1.Value;
			}
			else
			{
				this.intVariable.Value = this.value2.Value;
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x001F60A0 File Offset: 0x001F42A0
		public override void OnUpdate()
		{
			if (this.value1.Value < this.value2.Value)
			{
				this.intVariable.Value = this.value1.Value;
				return;
			}
			this.intVariable.Value = this.value2.Value;
		}

		// Token: 0x040061BB RID: 25019
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		// Token: 0x040061BC RID: 25020
		[RequiredField]
		public FsmInt value1;

		// Token: 0x040061BD RID: 25021
		[RequiredField]
		public FsmInt value2;

		// Token: 0x040061BE RID: 25022
		public bool everyFrame;
	}
}
