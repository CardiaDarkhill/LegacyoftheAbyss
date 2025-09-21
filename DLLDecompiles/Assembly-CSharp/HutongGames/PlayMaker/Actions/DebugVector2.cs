using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001181 RID: 4481
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of a Vector2 Variable in the PlayMaker Log Window.")]
	public class DebugVector2 : FsmStateAction
	{
		// Token: 0x0600782A RID: 30762 RVA: 0x00247107 File Offset: 0x00245307
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.vector2Variable = null;
		}

		// Token: 0x0600782B RID: 30763 RVA: 0x00247118 File Offset: 0x00245318
		public override void OnEnter()
		{
			string text = "None";
			if (!this.vector2Variable.IsNone)
			{
				text = this.vector2Variable.Name + ": " + this.vector2Variable.Value.ToString();
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, false);
			base.Finish();
		}

		// Token: 0x040078A3 RID: 30883
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040078A4 RID: 30884
		[UIHint(UIHint.Variable)]
		[Tooltip("Prints the value of a Vector2 variable in the PlayMaker log window.")]
		public FsmVector2 vector2Variable;
	}
}
