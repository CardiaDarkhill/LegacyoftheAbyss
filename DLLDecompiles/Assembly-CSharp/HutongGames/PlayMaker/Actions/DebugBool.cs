using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E77 RID: 3703
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of a Bool Variable in the PlayMaker Log Window.")]
	public class DebugBool : BaseLogAction
	{
		// Token: 0x06006980 RID: 27008 RVA: 0x00210B21 File Offset: 0x0020ED21
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.boolVariable = null;
			base.Reset();
		}

		// Token: 0x06006981 RID: 27009 RVA: 0x00210B38 File Offset: 0x0020ED38
		public override void OnEnter()
		{
			string text = "None";
			if (!this.boolVariable.IsNone)
			{
				text = this.boolVariable.Name + ": " + this.boolVariable.Value.ToString();
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068B9 RID: 26809
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068BA RID: 26810
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bool variable to debug.")]
		public FsmBool boolVariable;
	}
}
