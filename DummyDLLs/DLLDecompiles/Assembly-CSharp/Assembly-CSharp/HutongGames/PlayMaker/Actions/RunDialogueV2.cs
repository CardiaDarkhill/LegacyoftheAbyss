using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200135F RID: 4959
	[ActionCategory("Dialogue")]
	public class RunDialogueV2 : RunDialogueBase
	{
		// Token: 0x17000C3C RID: 3132
		// (get) Token: 0x06007FED RID: 32749 RVA: 0x0025CCCC File Offset: 0x0025AECC
		protected override string DialogueText
		{
			get
			{
				if (this.UsesCustomText())
				{
					return this.CustomText.Value;
				}
				if (!CheatManager.IsDialogueDebugEnabled)
				{
					return new LocalisedString(this.Sheet.Value, this.Key.Value).ToString(false);
				}
				return this.Sheet.Value + " / " + this.Key.Value;
			}
		}

		// Token: 0x06007FEE RID: 32750 RVA: 0x0025CD39 File Offset: 0x0025AF39
		public bool UsesCustomText()
		{
			return !this.CustomText.IsNone;
		}

		// Token: 0x06007FEF RID: 32751 RVA: 0x0025CD49 File Offset: 0x0025AF49
		public override void Reset()
		{
			base.Reset();
			this.CustomText = new FsmString
			{
				UseVariable = true
			};
			this.Sheet = null;
			this.Key = null;
		}

		// Token: 0x04007F5A RID: 32602
		public FsmString CustomText;

		// Token: 0x04007F5B RID: 32603
		[HideIf("UsesCustomText")]
		public FsmString Sheet;

		// Token: 0x04007F5C RID: 32604
		[HideIf("UsesCustomText")]
		public FsmString Key;
	}
}
