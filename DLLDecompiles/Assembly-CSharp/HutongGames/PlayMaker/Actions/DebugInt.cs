using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E7D RID: 3709
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of an Integer Variable in the PlayMaker Log Window.")]
	public class DebugInt : BaseLogAction
	{
		// Token: 0x06006995 RID: 27029 RVA: 0x00210F02 File Offset: 0x0020F102
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.intVariable = null;
		}

		// Token: 0x06006996 RID: 27030 RVA: 0x00210F14 File Offset: 0x0020F114
		public override void OnEnter()
		{
			string text = "None";
			if (!this.intVariable.IsNone)
			{
				text = this.intVariable.Name + ": " + this.intVariable.Value.ToString();
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068C8 RID: 26824
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068C9 RID: 26825
		[UIHint(UIHint.Variable)]
		[Tooltip("The Int variable to debug.")]
		public FsmInt intVariable;
	}
}
