using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000B01 RID: 2817
	[ActionCategory("Vibration")]
	public sealed class VibrationRegionPlayOneShot : FsmStateAction
	{
		// Token: 0x06005924 RID: 22820 RVA: 0x001C406E File Offset: 0x001C226E
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.vibrationDataAsset = null;
			this.requireInside = null;
			this.loop = null;
			this.isRealTime = null;
			this.tag = null;
			this.stopOnRegionExit = null;
			this.stopOnStateExit = null;
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x001C40AC File Offset: 0x001C22AC
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				this.vibrationRegion = safe.GetComponent<HeroVibrationRegion>();
				if (this.vibrationRegion != null)
				{
					VibrationDataAsset vibrationDataAsset = this.vibrationDataAsset.Value as VibrationDataAsset;
					if (vibrationDataAsset != null)
					{
						HeroVibrationRegion.VibrationSettings vibrationSettings = HeroVibrationRegion.VibrationSettings.None;
						if (this.loop.Value)
						{
							vibrationSettings |= HeroVibrationRegion.VibrationSettings.Loop;
						}
						if (this.isRealTime.Value)
						{
							vibrationSettings |= HeroVibrationRegion.VibrationSettings.RealTime;
						}
						if (this.stopOnRegionExit.Value)
						{
							vibrationSettings |= HeroVibrationRegion.VibrationSettings.StopOnExit;
						}
						this.emission = this.vibrationRegion.PlayVibrationOneShot(vibrationDataAsset, this.requireInside.Value, vibrationSettings, this.tag.Value);
					}
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

		// Token: 0x06005926 RID: 22822 RVA: 0x001C41A5 File Offset: 0x001C23A5
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value && this.emission != null)
			{
				this.emission.Stop();
				this.emission = null;
			}
		}

		// Token: 0x0400543D RID: 21565
		public FsmOwnerDefault target;

		// Token: 0x0400543E RID: 21566
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x0400543F RID: 21567
		public FsmBool requireInside;

		// Token: 0x04005440 RID: 21568
		public FsmBool loop;

		// Token: 0x04005441 RID: 21569
		public FsmBool isRealTime;

		// Token: 0x04005442 RID: 21570
		public FsmString tag;

		// Token: 0x04005443 RID: 21571
		public FsmBool stopOnRegionExit;

		// Token: 0x04005444 RID: 21572
		public FsmBool stopOnStateExit;

		// Token: 0x04005445 RID: 21573
		private HeroVibrationRegion vibrationRegion;

		// Token: 0x04005446 RID: 21574
		private VibrationEmission emission;
	}
}
