using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F84 RID: 3972
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Wraps the value of Int Variable so it stays in a Min/Max range.\n\nExamples:\nWrap 120 between 0 and 100 -> 20\nWrap -10 between 0 and 100 -> 90")]
	public class IntWrap : FsmStateAction
	{
		// Token: 0x06006DF0 RID: 28144 RVA: 0x00221B37 File Offset: 0x0021FD37
		public override void Reset()
		{
			this.intVariable = null;
			this.minValue = null;
			this.maxValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DF1 RID: 28145 RVA: 0x00221B55 File Offset: 0x0021FD55
		public override void OnEnter()
		{
			this.DoWrap();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DF2 RID: 28146 RVA: 0x00221B6B File Offset: 0x0021FD6B
		public override void OnUpdate()
		{
			this.DoWrap();
		}

		// Token: 0x06006DF3 RID: 28147 RVA: 0x00221B74 File Offset: 0x0021FD74
		private void DoWrap()
		{
			int value = this.intVariable.Value;
			int value2 = this.minValue.Value;
			int value3 = this.maxValue.Value;
			if (value < value2)
			{
				this.intVariable.Value = value3 - (value2 - value) % (value3 - value2);
				return;
			}
			this.intVariable.Value = value2 + (value - value2) % (value3 - value2);
		}

		// Token: 0x04006DA3 RID: 28067
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Int variable to wrap.")]
		public FsmInt intVariable;

		// Token: 0x04006DA4 RID: 28068
		[RequiredField]
		[Tooltip("The minimum value allowed.")]
		public FsmInt minValue;

		// Token: 0x04006DA5 RID: 28069
		[RequiredField]
		[Tooltip("The maximum value allowed.")]
		public FsmInt maxValue;

		// Token: 0x04006DA6 RID: 28070
		[Tooltip("Repeat every frame. Useful if the int variable is changing.")]
		public bool everyFrame;
	}
}
