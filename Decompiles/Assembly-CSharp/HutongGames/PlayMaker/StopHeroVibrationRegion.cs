using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000B03 RID: 2819
	[ActionCategory("Vibration")]
	public sealed class StopHeroVibrationRegion : FsmStateAction
	{
		// Token: 0x0600592C RID: 22828 RVA: 0x001C436D File Offset: 0x001C256D
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x0600592D RID: 22829 RVA: 0x001C437C File Offset: 0x001C257C
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				HeroVibrationRegion component = safe.GetComponent<HeroVibrationRegion>();
				if (component != null)
				{
					component.StopVibration();
				}
				else
				{
					Debug.LogError(string.Format("{0} in {1} is missing Hero Vibration Region component", this, base.Owner));
				}
			}
			else
			{
				Debug.LogWarning(string.Format("{0} in {1} is missing Hero Vibration Region object", this, base.Owner));
			}
			base.Finish();
		}

		// Token: 0x04005452 RID: 21586
		public FsmOwnerDefault target;
	}
}
