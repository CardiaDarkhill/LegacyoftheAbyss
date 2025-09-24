using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200135E RID: 4958
	[ActionCategory("Dialogue")]
	public class RunDialogue : RunDialogueBase
	{
		// Token: 0x17000C3B RID: 3131
		// (get) Token: 0x06007FEA RID: 32746 RVA: 0x0025CC54 File Offset: 0x0025AE54
		protected override string DialogueText
		{
			get
			{
				if (!CheatManager.IsDialogueDebugEnabled)
				{
					return new LocalisedString(this.Sheet.Value, this.Key.Value).ToString(false);
				}
				return this.Sheet.Value + " / " + this.Key.Value;
			}
		}

		// Token: 0x06007FEB RID: 32747 RVA: 0x0025CCAD File Offset: 0x0025AEAD
		public override void Reset()
		{
			base.Reset();
			this.Sheet = null;
			this.Key = null;
		}

		// Token: 0x04007F58 RID: 32600
		[RequiredField]
		public FsmString Sheet;

		// Token: 0x04007F59 RID: 32601
		[RequiredField]
		public FsmString Key;
	}
}
