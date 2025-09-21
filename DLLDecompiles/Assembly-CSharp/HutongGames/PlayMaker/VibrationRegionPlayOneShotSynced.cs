using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000B02 RID: 2818
	[ActionCategory("Vibration")]
	public sealed class VibrationRegionPlayOneShotSynced : FsmStateAction
	{
		// Token: 0x06005928 RID: 22824 RVA: 0x001C41D8 File Offset: 0x001C23D8
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.vibrationDataAsset = null;
			this.audioSource = new FsmOwnerDefault();
			this.requireInside = null;
			this.loop = null;
			this.isRealTime = null;
			this.tag = null;
			this.stopOnRegionExit = null;
			this.stopOnStateExit = null;
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x001C422C File Offset: 0x001C242C
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
						AudioVibrationSyncer.StartSyncedEmission(this.emission, this.audioSource.GetSafe(this));
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

		// Token: 0x0600592A RID: 22826 RVA: 0x001C433C File Offset: 0x001C253C
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value && this.emission != null)
			{
				this.emission.Stop();
				this.emission = null;
			}
		}

		// Token: 0x04005447 RID: 21575
		public FsmOwnerDefault target;

		// Token: 0x04005448 RID: 21576
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x04005449 RID: 21577
		[ObjectType(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		// Token: 0x0400544A RID: 21578
		public FsmBool requireInside;

		// Token: 0x0400544B RID: 21579
		public FsmBool loop;

		// Token: 0x0400544C RID: 21580
		public FsmBool isRealTime;

		// Token: 0x0400544D RID: 21581
		public FsmString tag;

		// Token: 0x0400544E RID: 21582
		public FsmBool stopOnRegionExit;

		// Token: 0x0400544F RID: 21583
		public FsmBool stopOnStateExit;

		// Token: 0x04005450 RID: 21584
		private HeroVibrationRegion vibrationRegion;

		// Token: 0x04005451 RID: 21585
		private VibrationEmission emission;
	}
}
