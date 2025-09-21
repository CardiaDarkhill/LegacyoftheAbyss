using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E7A RID: 3706
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of a Float Variable in the PlayMaker Log Window.")]
	public class DebugFloat : BaseLogAction
	{
		// Token: 0x0600698C RID: 27020 RVA: 0x00210DAB File Offset: 0x0020EFAB
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.floatVariable = null;
			base.Reset();
		}

		// Token: 0x0600698D RID: 27021 RVA: 0x00210DC4 File Offset: 0x0020EFC4
		public override void OnEnter()
		{
			string text = "None";
			if (!this.floatVariable.IsNone)
			{
				text = this.floatVariable.Name + ": " + this.floatVariable.Value.ToString();
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068C2 RID: 26818
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068C3 RID: 26819
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to debug.")]
		public FsmFloat floatVariable;
	}
}
