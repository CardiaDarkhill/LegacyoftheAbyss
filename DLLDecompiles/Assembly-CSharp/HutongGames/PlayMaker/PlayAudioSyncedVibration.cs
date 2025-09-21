using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AFF RID: 2815
	public sealed class PlayAudioSyncedVibration : FsmStateAction
	{
		// Token: 0x0600591C RID: 22812 RVA: 0x001C3EDC File Offset: 0x001C20DC
		public override void Reset()
		{
			this.target = null;
			this.fadeInDuration = null;
			this.stopOnStateExit = null;
			this.fadeOutDuration = null;
		}

		// Token: 0x0600591D RID: 22813 RVA: 0x001C3EFA File Offset: 0x001C20FA
		public override void OnEnter()
		{
			this.audioSyncedVibration = this.target.GetSafe(this);
			if (this.audioSyncedVibration != null)
			{
				this.audioSyncedVibration.PlayVibration(this.fadeInDuration.Value);
			}
			base.Finish();
		}

		// Token: 0x0600591E RID: 22814 RVA: 0x001C3F38 File Offset: 0x001C2138
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value && this.audioSyncedVibration != null)
			{
				if (this.fadeOutDuration.Value > 0f)
				{
					this.audioSyncedVibration.FadeOut(this.fadeOutDuration.Value);
					this.audioSyncedVibration = null;
					return;
				}
				this.audioSyncedVibration.StopVibration();
				this.audioSyncedVibration = null;
			}
		}

		// Token: 0x04005435 RID: 21557
		[ObjectType(typeof(AudioSyncedVibration))]
		public FsmOwnerDefault target;

		// Token: 0x04005436 RID: 21558
		public FsmFloat fadeInDuration;

		// Token: 0x04005437 RID: 21559
		[Space]
		public FsmBool stopOnStateExit;

		// Token: 0x04005438 RID: 21560
		public FsmFloat fadeOutDuration;

		// Token: 0x04005439 RID: 21561
		private AudioSyncedVibration audioSyncedVibration;
	}
}
