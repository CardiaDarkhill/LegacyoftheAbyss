using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6D RID: 3181
	[ActionCategory("Game Text")]
	[Tooltip("Grab a string from the Hollow Knight game text database in the correct language.")]
	public class GetLanguageString : FsmStateAction
	{
		// Token: 0x0600600E RID: 24590 RVA: 0x001E6C6E File Offset: 0x001E4E6E
		public override void Reset()
		{
			this.sheetName = null;
			this.convName = null;
			this.storeValue = null;
		}

		// Token: 0x0600600F RID: 24591 RVA: 0x001E6C88 File Offset: 0x001E4E88
		public override void OnEnter()
		{
			this.storeValue.Value = Language.Get(this.convName.Value, this.sheetName.Value);
			this.storeValue.Value = this.storeValue.Value.Replace("<br>", "\n");
			base.Finish();
		}

		// Token: 0x04005D65 RID: 23909
		[RequiredField]
		public FsmString sheetName;

		// Token: 0x04005D66 RID: 23910
		[RequiredField]
		public FsmString convName;

		// Token: 0x04005D67 RID: 23911
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString storeValue;
	}
}
