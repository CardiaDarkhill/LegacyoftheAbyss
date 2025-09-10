using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E7B RID: 3707
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Print the value of any FSM Variable in the PlayMaker Log Window.")]
	public class DebugFsmVariable : BaseLogAction
	{
		// Token: 0x0600698F RID: 27023 RVA: 0x00210E33 File Offset: 0x0020F033
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.variable = null;
			base.Reset();
		}

		// Token: 0x06006990 RID: 27024 RVA: 0x00210E49 File Offset: 0x0020F049
		public override void OnEnter()
		{
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.variable.DebugString(), this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068C4 RID: 26820
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068C5 RID: 26821
		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to debug.")]
		public FsmVar variable;
	}
}
