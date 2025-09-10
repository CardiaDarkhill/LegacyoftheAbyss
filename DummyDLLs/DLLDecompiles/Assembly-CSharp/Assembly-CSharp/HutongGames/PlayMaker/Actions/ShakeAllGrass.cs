using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D60 RID: 3424
	public class ShakeAllGrass : FsmStateAction
	{
		// Token: 0x06006423 RID: 25635 RVA: 0x001F8ABA File Offset: 0x001F6CBA
		public override void OnEnter()
		{
			base.OnEnter();
			PlayMakerFSM.BroadcastEvent("SHAKE ALL GRASS");
			Grass.PushAll();
			base.Finish();
		}

		// Token: 0x0400628E RID: 25230
		private const string DeprecatedEffectName = "SHAKE ALL GRASS";
	}
}
