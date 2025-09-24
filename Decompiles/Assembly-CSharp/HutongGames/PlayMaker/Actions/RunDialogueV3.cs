using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001360 RID: 4960
	[ActionCategory("Dialogue")]
	public class RunDialogueV3 : RunDialogueBase
	{
		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x06007FF1 RID: 32753 RVA: 0x0025CD7C File Offset: 0x0025AF7C
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

		// Token: 0x06007FF2 RID: 32754 RVA: 0x0025CDE9 File Offset: 0x0025AFE9
		public bool UsesCustomText()
		{
			return !this.CustomText.IsNone;
		}

		// Token: 0x06007FF3 RID: 32755 RVA: 0x0025CDF9 File Offset: 0x0025AFF9
		public override void Reset()
		{
			base.Reset();
			this.CustomText = new FsmString
			{
				UseVariable = true
			};
			this.Sheet = null;
			this.Key = null;
			this.TextColor = new FsmColor
			{
				UseVariable = true
			};
		}

		// Token: 0x06007FF4 RID: 32756 RVA: 0x0025CE33 File Offset: 0x0025B033
		protected override DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			if (!this.TextColor.IsNone)
			{
				defaultOptions.TextColor = this.TextColor.Value;
			}
			return defaultOptions;
		}

		// Token: 0x04007F5D RID: 32605
		public FsmString CustomText;

		// Token: 0x04007F5E RID: 32606
		[HideIf("UsesCustomText")]
		public FsmString Sheet;

		// Token: 0x04007F5F RID: 32607
		[HideIf("UsesCustomText")]
		public FsmString Key;

		// Token: 0x04007F60 RID: 32608
		public FsmColor TextColor;
	}
}
