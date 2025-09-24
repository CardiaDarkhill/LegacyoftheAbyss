using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200121F RID: 4639
	[ActionCategory("Hollow Knight")]
	public class CameraRepositionToHeroV2 : FsmStateAction
	{
		// Token: 0x06007B18 RID: 31512 RVA: 0x0024EC32 File Offset: 0x0024CE32
		public override void Reset()
		{
			this.forceDirect = null;
			this.timeOut = null;
		}

		// Token: 0x06007B19 RID: 31513 RVA: 0x0024EC44 File Offset: 0x0024CE44
		public override void OnEnter()
		{
			if (GameManager.instance && GameManager.instance.cameraCtrl)
			{
				this.cameraController = GameManager.instance.cameraCtrl;
				this.cameraController.PositionToHero(this.forceDirect.Value);
				this.timer = this.timeOut.Value;
				this.cameraController.PositionedAtHero += this.OnPositionedAtHero;
				return;
			}
			this.DoFinish();
		}

		// Token: 0x06007B1A RID: 31514 RVA: 0x0024ECC3 File Offset: 0x0024CEC3
		public override void OnExit()
		{
			if (this.cameraController != null)
			{
				this.cameraController.PositionedAtHero -= this.OnPositionedAtHero;
				this.cameraController = null;
			}
		}

		// Token: 0x06007B1B RID: 31515 RVA: 0x0024ECF1 File Offset: 0x0024CEF1
		private void OnPositionedAtHero()
		{
			this.DoFinish();
		}

		// Token: 0x06007B1C RID: 31516 RVA: 0x0024ECF9 File Offset: 0x0024CEF9
		public override void OnUpdate()
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				if (this.timer <= 0f)
				{
					this.DoFinish();
				}
			}
		}

		// Token: 0x06007B1D RID: 31517 RVA: 0x0024ED2D File Offset: 0x0024CF2D
		private void DoFinish()
		{
			base.Fsm.Event(this.onFinish);
			base.Finish();
		}

		// Token: 0x04007B73 RID: 31603
		public FsmBool forceDirect;

		// Token: 0x04007B74 RID: 31604
		public FsmFloat timeOut;

		// Token: 0x04007B75 RID: 31605
		public FsmEvent onFinish;

		// Token: 0x04007B76 RID: 31606
		private CameraController cameraController;

		// Token: 0x04007B77 RID: 31607
		private float timer;
	}
}
