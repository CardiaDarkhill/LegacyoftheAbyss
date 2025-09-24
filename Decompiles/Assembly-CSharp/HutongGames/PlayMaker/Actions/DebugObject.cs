using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E7F RID: 3711
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of an Object Variable in the PlayMaker Log Window.")]
	public class DebugObject : BaseLogAction
	{
		// Token: 0x0600699B RID: 27035 RVA: 0x00210FE6 File Offset: 0x0020F1E6
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.fsmObject = null;
			base.Reset();
		}

		// Token: 0x0600699C RID: 27036 RVA: 0x00210FFC File Offset: 0x0020F1FC
		public override void OnEnter()
		{
			string text = "None";
			if (!this.fsmObject.IsNone)
			{
				string name = this.fsmObject.Name;
				string str = ": ";
				FsmObject fsmObject = this.fsmObject;
				text = name + str + ((fsmObject != null) ? fsmObject.ToString() : null);
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068CC RID: 26828
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068CD RID: 26829
		[UIHint(UIHint.Variable)]
		[Tooltip("The Object variable to debug.")]
		public FsmObject fsmObject;
	}
}
