using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E63 RID: 3683
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Sets the value of a Color Variable.")]
	public class SetColorValue : FsmStateAction
	{
		// Token: 0x0600691C RID: 26908 RVA: 0x0020FE42 File Offset: 0x0020E042
		public override void Reset()
		{
			this.colorVariable = null;
			this.color = null;
			this.everyFrame = false;
		}

		// Token: 0x0600691D RID: 26909 RVA: 0x0020FE59 File Offset: 0x0020E059
		public override void OnEnter()
		{
			this.DoSetColorValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600691E RID: 26910 RVA: 0x0020FE6F File Offset: 0x0020E06F
		public override void OnUpdate()
		{
			this.DoSetColorValue();
		}

		// Token: 0x0600691F RID: 26911 RVA: 0x0020FE77 File Offset: 0x0020E077
		private void DoSetColorValue()
		{
			if (this.colorVariable != null)
			{
				this.colorVariable.Value = this.color.Value;
			}
		}

		// Token: 0x0400686C RID: 26732
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color Variable to set.")]
		public FsmColor colorVariable;

		// Token: 0x0400686D RID: 26733
		[RequiredField]
		[Tooltip("The color to set the variable to.")]
		public FsmColor color;

		// Token: 0x0400686E RID: 26734
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
