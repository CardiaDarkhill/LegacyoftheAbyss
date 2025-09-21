using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0A RID: 3082
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Sends a log message to the Console Log Window.")]
	public class DebugLogConsole : BaseLogAction
	{
		// Token: 0x06005E0C RID: 24076 RVA: 0x001DA4D8 File Offset: 0x001D86D8
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.text = "";
			base.Reset();
		}

		// Token: 0x06005E0D RID: 24077 RVA: 0x001DA4F7 File Offset: 0x001D86F7
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x04005A6F RID: 23151
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x04005A70 RID: 23152
		[Tooltip("Text to send to the log.")]
		public FsmString text;
	}
}
