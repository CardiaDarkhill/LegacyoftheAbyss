using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E7C RID: 3708
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Logs the value of a Game Object Variable in the PlayMaker Log Window.")]
	public class DebugGameObject : BaseLogAction
	{
		// Token: 0x06006992 RID: 27026 RVA: 0x00210E7B File Offset: 0x0020F07B
		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.gameObject = null;
			base.Reset();
		}

		// Token: 0x06006993 RID: 27027 RVA: 0x00210E94 File Offset: 0x0020F094
		public override void OnEnter()
		{
			string text = "None";
			if (!this.gameObject.IsNone)
			{
				string name = this.gameObject.Name;
				string str = ": ";
				FsmGameObject fsmGameObject = this.gameObject;
				text = name + str + ((fsmGameObject != null) ? fsmGameObject.ToString() : null);
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text, this.sendToUnityLog);
			base.Finish();
		}

		// Token: 0x040068C6 RID: 26822
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		// Token: 0x040068C7 RID: 26823
		[UIHint(UIHint.Variable)]
		[Tooltip("The GameObject variable to debug.")]
		public FsmGameObject gameObject;
	}
}
