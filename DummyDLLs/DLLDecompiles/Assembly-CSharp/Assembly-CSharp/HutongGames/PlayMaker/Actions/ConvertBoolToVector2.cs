using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C00 RID: 3072
	[ActionCategory(ActionCategory.Convert)]
	[Tooltip("Converts a Bool value to a Vector2 value.")]
	public class ConvertBoolToVector2 : FsmStateAction
	{
		// Token: 0x06005DE4 RID: 24036 RVA: 0x001D9B11 File Offset: 0x001D7D11
		public override void Reset()
		{
			this.BoolVariable = null;
			this.Vector2Variable = null;
			this.FalseValue = null;
			this.TrueValue = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005DE5 RID: 24037 RVA: 0x001D9B36 File Offset: 0x001D7D36
		public override void OnEnter()
		{
			this.DoConvertBoolToVector2();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DE6 RID: 24038 RVA: 0x001D9B4C File Offset: 0x001D7D4C
		public override void OnUpdate()
		{
			this.DoConvertBoolToVector2();
		}

		// Token: 0x06005DE7 RID: 24039 RVA: 0x001D9B54 File Offset: 0x001D7D54
		private void DoConvertBoolToVector2()
		{
			this.Vector2Variable.Value = (this.BoolVariable.Value ? this.TrueValue.Value : this.FalseValue.Value);
		}

		// Token: 0x04005A3C RID: 23100
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to test.")]
		public FsmBool BoolVariable;

		// Token: 0x04005A3D RID: 23101
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2 variable to set based on the Bool variable value.")]
		public FsmVector2 Vector2Variable;

		// Token: 0x04005A3E RID: 23102
		[Tooltip("Vector2 value if Bool variable is false.")]
		public FsmVector2 FalseValue;

		// Token: 0x04005A3F RID: 23103
		[Tooltip("Vector2 value if Bool variable is true.")]
		public FsmVector2 TrueValue;

		// Token: 0x04005A40 RID: 23104
		[Tooltip("Repeat every frame. Useful if the Bool variable is changing.")]
		public bool EveryFrame;
	}
}
