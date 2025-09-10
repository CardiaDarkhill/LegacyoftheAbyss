using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E3 RID: 4835
	public class SetInputHandlerSkipMode : FsmStateAction
	{
		// Token: 0x06007E1C RID: 32284 RVA: 0x0025837D File Offset: 0x0025657D
		public override void Reset()
		{
			this.SkipMode = null;
		}

		// Token: 0x06007E1D RID: 32285 RVA: 0x00258388 File Offset: 0x00256588
		public override void OnEnter()
		{
			if (!this.SkipMode.IsNone)
			{
				SkipPromptMode skipMode = (SkipPromptMode)this.SkipMode.Value;
				ManagerSingleton<InputHandler>.Instance.SetSkipMode(skipMode);
			}
			base.Finish();
		}

		// Token: 0x04007DF3 RID: 32243
		[ObjectType(typeof(SkipPromptMode))]
		public FsmEnum SkipMode;
	}
}
