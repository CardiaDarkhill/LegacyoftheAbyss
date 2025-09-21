using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000B00 RID: 2816
	[ActionCategory("Vibration")]
	public sealed class StartHeroVibrationRegion : FsmStateAction
	{
		// Token: 0x06005920 RID: 22816 RVA: 0x001C3FAA File Offset: 0x001C21AA
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.stopOnExit = null;
		}

		// Token: 0x06005921 RID: 22817 RVA: 0x001C3FC0 File Offset: 0x001C21C0
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				this.vibrationRegion = safe.GetComponent<HeroVibrationRegion>();
				if (this.vibrationRegion != null)
				{
					this.vibrationRegion.StartVibration();
				}
				else
				{
					Debug.LogError(string.Format("{0} in {1} is missing Hero Vibration Region component", this, base.Owner));
				}
			}
			else
			{
				Debug.LogError(string.Format("{0} in {1} is missing Hero Vibration Region object", this, base.Owner));
			}
			base.Finish();
		}

		// Token: 0x06005922 RID: 22818 RVA: 0x001C403E File Offset: 0x001C223E
		public override void OnExit()
		{
			if (this.stopOnExit.Value && this.vibrationRegion != null)
			{
				this.vibrationRegion.StopVibration();
			}
		}

		// Token: 0x0400543A RID: 21562
		public FsmOwnerDefault target;

		// Token: 0x0400543B RID: 21563
		public FsmBool stopOnExit;

		// Token: 0x0400543C RID: 21564
		private HeroVibrationRegion vibrationRegion;
	}
}
