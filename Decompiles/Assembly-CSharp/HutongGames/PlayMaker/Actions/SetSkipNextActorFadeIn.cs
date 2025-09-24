using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC7 RID: 3015
	public class SetSkipNextActorFadeIn : FsmStateAction
	{
		// Token: 0x06005CA7 RID: 23719 RVA: 0x001D2A13 File Offset: 0x001D0C13
		public override void Reset()
		{
			this.skipNextActorFadeIn = null;
		}

		// Token: 0x06005CA8 RID: 23720 RVA: 0x001D2A1C File Offset: 0x001D0C1C
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.SetSkipNextLevelReadyActorFadeIn(this.skipNextActorFadeIn.Value);
			}
			base.Finish();
		}

		// Token: 0x0400583E RID: 22590
		public FsmBool skipNextActorFadeIn;
	}
}
