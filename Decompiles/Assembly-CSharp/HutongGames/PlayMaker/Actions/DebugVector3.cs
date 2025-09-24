using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E80 RID: 3712
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of a Vector3 Variable in the PlayMaker Log Window.")]
	public class DebugVector3 : BaseLogAction
	{
		// Token: 0x0600699E RID: 27038 RVA: 0x0021106A File Offset: 0x0020F26A
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.vector3Variable = null;
			base.Reset();
		}

		// Token: 0x0600699F RID: 27039 RVA: 0x00211080 File Offset: 0x0020F280
		public override void OnEnter()
		{
			string text = "None";
			if (!this.vector3Variable.IsNone)
			{
				text = this.vector3Variable.Name + ": " + this.vector3Variable.Value.ToString();
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068CE RID: 26830
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068CF RID: 26831
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to debug.")]
		public FsmVector3 vector3Variable;
	}
}
