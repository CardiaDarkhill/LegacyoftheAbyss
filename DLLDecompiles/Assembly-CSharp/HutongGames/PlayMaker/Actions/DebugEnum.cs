using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E79 RID: 3705
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of an Enum Variable in the PlayMaker Log Window.")]
	public class DebugEnum : BaseLogAction
	{
		// Token: 0x06006989 RID: 27017 RVA: 0x00210D1F File Offset: 0x0020EF1F
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.enumVariable = null;
			base.Reset();
		}

		// Token: 0x0600698A RID: 27018 RVA: 0x00210D38 File Offset: 0x0020EF38
		public override void OnEnter()
		{
			string text = "None";
			if (!this.enumVariable.IsNone)
			{
				string name = this.enumVariable.Name;
				string str = ": ";
				Enum value = this.enumVariable.Value;
				text = name + str + ((value != null) ? value.ToString() : null);
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068C0 RID: 26816
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068C1 RID: 26817
		[UIHint(UIHint.Variable)]
		[Tooltip("The Enum Variable to debug.")]
		public FsmEnum enumVariable;
	}
}
