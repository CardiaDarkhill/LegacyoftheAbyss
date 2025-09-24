using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001262 RID: 4706
	public sealed class ClearSpoolMossChunks : FsmStateAction
	{
		// Token: 0x06007C3B RID: 31803 RVA: 0x00252548 File Offset: 0x00250748
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.ClearSpoolMossChunks();
			}
			base.Finish();
		}
	}
}
