using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E7E RID: 3710
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Sends a log message to the PlayMaker Log Window.")]
	public class DebugLog : BaseLogAction
	{
		// Token: 0x06006998 RID: 27032 RVA: 0x00210F83 File Offset: 0x0020F183
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.text = "";
			base.Reset();
		}

		// Token: 0x06006999 RID: 27033 RVA: 0x00210FA2 File Offset: 0x0020F1A2
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.text.Value))
			{
				ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.text.Value, this.sendToUnityLog);
			}
			base.Finish();
		}

		// Token: 0x040068CA RID: 26826
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068CB RID: 26827
		[Tooltip("Text to send to the log.")]
		public FsmString text;
	}
}
