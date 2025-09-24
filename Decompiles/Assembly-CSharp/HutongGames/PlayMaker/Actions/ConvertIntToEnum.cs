using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C03 RID: 3075
	[ActionCategory(ActionCategory.Convert)]
	public class ConvertIntToEnum : FsmStateAction
	{
		// Token: 0x06005DF3 RID: 24051 RVA: 0x001D9D61 File Offset: 0x001D7F61
		public override void Reset()
		{
			this.IntValue = null;
			this.EnumVariable = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005DF4 RID: 24052 RVA: 0x001D9D78 File Offset: 0x001D7F78
		public override void OnEnter()
		{
			this.DoConvertIntToEnum();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DF5 RID: 24053 RVA: 0x001D9D8E File Offset: 0x001D7F8E
		public override void OnUpdate()
		{
			this.DoConvertIntToEnum();
		}

		// Token: 0x06005DF6 RID: 24054 RVA: 0x001D9D96 File Offset: 0x001D7F96
		private void DoConvertIntToEnum()
		{
			this.EnumVariable.RawValue = Enum.ToObject(this.EnumVariable.EnumType, this.IntValue.Value);
		}

		// Token: 0x04005A4F RID: 23119
		[RequiredField]
		public FsmInt IntValue;

		// Token: 0x04005A50 RID: 23120
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmEnum EnumVariable;

		// Token: 0x04005A51 RID: 23121
		public bool EveryFrame;
	}
}
