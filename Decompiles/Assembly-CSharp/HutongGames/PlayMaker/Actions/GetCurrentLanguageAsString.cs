using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C63 RID: 3171
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Get currently set language as a string.")]
	public class GetCurrentLanguageAsString : FsmStateAction
	{
		// Token: 0x06005FE0 RID: 24544 RVA: 0x001E61A1 File Offset: 0x001E43A1
		public override void Reset()
		{
			this.stringVariable = null;
		}

		// Token: 0x06005FE1 RID: 24545 RVA: 0x001E61AC File Offset: 0x001E43AC
		public override void OnEnter()
		{
			this.stringVariable.Value = Language.CurrentLanguage().ToString();
			base.Finish();
		}

		// Token: 0x04005D39 RID: 23865
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString stringVariable;
	}
}
