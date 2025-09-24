using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E97 RID: 3735
	[ActionCategory(ActionCategory.Enum)]
	[Tooltip("Sets the value of an Enum Variable.")]
	public class SetEnumValue : FsmStateAction
	{
		// Token: 0x06006A01 RID: 27137 RVA: 0x002126D4 File Offset: 0x002108D4
		public override void Reset()
		{
			this.enumVariable = null;
			this.enumValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A02 RID: 27138 RVA: 0x002126EB File Offset: 0x002108EB
		public override void OnEnter()
		{
			this.DoSetEnumValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A03 RID: 27139 RVA: 0x00212701 File Offset: 0x00210901
		public override void OnUpdate()
		{
			this.DoSetEnumValue();
		}

		// Token: 0x06006A04 RID: 27140 RVA: 0x00212709 File Offset: 0x00210909
		private void DoSetEnumValue()
		{
			this.enumVariable.Value = this.enumValue.Value;
		}

		// Token: 0x0400695B RID: 26971
		[UIHint(UIHint.Variable)]
		[Tooltip("The Enum Variable to set.")]
		public FsmEnum enumVariable;

		// Token: 0x0400695C RID: 26972
		[MatchFieldType("enumVariable")]
		[Tooltip("The Enum value to set the variable to.")]
		public FsmEnum enumValue;

		// Token: 0x0400695D RID: 26973
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
