using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000B04 RID: 2820
	public sealed class PlayScaledVibration : FsmStateAction
	{
		// Token: 0x0600592F RID: 22831 RVA: 0x001C43F3 File Offset: 0x001C25F3
		public override void Reset()
		{
			this.target = null;
			this.stopOnStateExit = null;
		}

		// Token: 0x06005930 RID: 22832 RVA: 0x001C4403 File Offset: 0x001C2603
		public override void OnEnter()
		{
			this.vibrationPlayer = this.target.GetSafe(this);
			if (this.vibrationPlayer != null)
			{
				this.vibrationPlayer.PlayVibration(this.fadeInDuration.Value);
			}
			base.Finish();
		}

		// Token: 0x06005931 RID: 22833 RVA: 0x001C4444 File Offset: 0x001C2644
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value && this.vibrationPlayer != null)
			{
				if (this.fadeOutDuration.Value > 0f)
				{
					this.vibrationPlayer.FadeOut(this.fadeOutDuration.Value);
					this.vibrationPlayer = null;
					return;
				}
				this.vibrationPlayer.StopVibration();
				this.vibrationPlayer = null;
			}
		}

		// Token: 0x04005453 RID: 21587
		[ObjectType(typeof(ScaledVibration))]
		[RequiredField]
		public FsmOwnerDefault target;

		// Token: 0x04005454 RID: 21588
		public FsmFloat fadeInDuration;

		// Token: 0x04005455 RID: 21589
		[Space]
		public FsmBool stopOnStateExit;

		// Token: 0x04005456 RID: 21590
		public FsmFloat fadeOutDuration;

		// Token: 0x04005457 RID: 21591
		private ScaledVibration vibrationPlayer;
	}
}
