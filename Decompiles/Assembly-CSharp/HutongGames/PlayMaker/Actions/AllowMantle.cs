using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001246 RID: 4678
	public class AllowMantle : FsmStateAction
	{
		// Token: 0x06007BB2 RID: 31666 RVA: 0x00250464 File Offset: 0x0024E664
		public override void OnEnter()
		{
			base.Owner.GetComponent<HeroController>().AllowMantle(true);
			base.Finish();
		}

		// Token: 0x06007BB3 RID: 31667 RVA: 0x0025047D File Offset: 0x0024E67D
		public override void OnExit()
		{
			base.Owner.GetComponent<HeroController>().AllowMantle(false);
		}
	}
}
