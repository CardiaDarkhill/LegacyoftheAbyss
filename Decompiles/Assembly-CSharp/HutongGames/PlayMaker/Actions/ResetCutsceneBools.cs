using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFE RID: 3326
	public class ResetCutsceneBools : FsmStateAction
	{
		// Token: 0x0600628C RID: 25228 RVA: 0x001F2B29 File Offset: 0x001F0D29
		public override void OnEnter()
		{
			PlayerData.instance.ResetCutsceneBools();
			base.Finish();
		}
	}
}
