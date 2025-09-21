using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200125F RID: 4703
	public class CheckHeroCanSprint : FsmStateAction
	{
		// Token: 0x06007C1E RID: 31774 RVA: 0x00251B4E File Offset: 0x0024FD4E
		public override void Reset()
		{
			this.storeValue = null;
		}

		// Token: 0x06007C1F RID: 31775 RVA: 0x00251B58 File Offset: 0x0024FD58
		public override void OnEnter()
		{
			this.hasHero = this.heroController;
			if (!this.hasHero)
			{
				this.heroController = HeroController.instance;
				this.hasHero = this.heroController;
			}
			if (!this.hasHero || !this.everyFrame.Value)
			{
				this.SendSprintEvent();
				base.Finish();
			}
			if (!this.hasHero)
			{
				Debug.LogError("Failed to find hero controller.");
			}
		}

		// Token: 0x06007C20 RID: 31776 RVA: 0x00251BCD File Offset: 0x0024FDCD
		public override void OnUpdate()
		{
			this.SendSprintEvent();
		}

		// Token: 0x06007C21 RID: 31777 RVA: 0x00251BD8 File Offset: 0x0024FDD8
		private void SendSprintEvent()
		{
			if (!this.hasHero)
			{
				return;
			}
			if (this.heroController.CanSprint())
			{
				this.storeValue.Value = true;
				base.Fsm.Event(this.isTrue);
				return;
			}
			this.storeValue.Value = false;
			base.Fsm.Event(this.isFalse);
		}

		// Token: 0x04007C38 RID: 31800
		public FsmBool storeValue;

		// Token: 0x04007C39 RID: 31801
		public FsmBool everyFrame;

		// Token: 0x04007C3A RID: 31802
		public FsmEvent isTrue;

		// Token: 0x04007C3B RID: 31803
		public FsmEvent isFalse;

		// Token: 0x04007C3C RID: 31804
		private bool hasHero;

		// Token: 0x04007C3D RID: 31805
		private HeroController heroController;
	}
}
