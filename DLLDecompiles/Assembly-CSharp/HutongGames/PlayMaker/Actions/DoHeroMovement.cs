using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200125D RID: 4701
	public class DoHeroMovement : FsmStateAction
	{
		// Token: 0x06007C17 RID: 31767 RVA: 0x00251AEA File Offset: 0x0024FCEA
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007C18 RID: 31768 RVA: 0x00251AF8 File Offset: 0x0024FCF8
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
		}

		// Token: 0x06007C19 RID: 31769 RVA: 0x00251B05 File Offset: 0x0024FD05
		public override void OnUpdate()
		{
			this.hc.UpdateMoveInput();
		}

		// Token: 0x06007C1A RID: 31770 RVA: 0x00251B12 File Offset: 0x0024FD12
		public override void OnFixedUpdate()
		{
			this.hc.DoMovement(true);
		}

		// Token: 0x04007C36 RID: 31798
		private HeroController hc;
	}
}
