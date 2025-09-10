using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001273 RID: 4723
	public class AudioSyncAction : FsmStateAction
	{
		// Token: 0x06007C75 RID: 31861 RVA: 0x00253298 File Offset: 0x00251498
		public override void Reset()
		{
			this.SyncTarget = null;
			this.SyncTarget = null;
			this.Volume = new FsmFloat
			{
				UseVariable = true
			};
			this.CopyVolume = null;
			this.CopyClip = null;
			this.StartPlaying = null;
		}

		// Token: 0x06007C76 RID: 31862 RVA: 0x002532D0 File Offset: 0x002514D0
		public override void OnEnter()
		{
			AudioSource safe = this.SyncTarget.GetSafe(this);
			if (safe != null)
			{
				AudioSource safe2 = this.SyncSource.GetSafe(this);
				if (safe2 != null)
				{
					if (this.CopyVolume.Value)
					{
						safe.volume = safe2.volume;
					}
					else if (!this.Volume.IsNone)
					{
						safe.volume = this.Volume.Value;
					}
					if (this.CopyClip.Value)
					{
						safe.clip = safe2.clip;
					}
					if (safe.clip != null && safe.clip == safe2.clip)
					{
						safe.timeSamples = safe2.timeSamples;
					}
					if (this.StartPlaying.Value)
					{
						safe.Play();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007C7A RID: 31866
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault SyncSource;

		// Token: 0x04007C7B RID: 31867
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault SyncTarget;

		// Token: 0x04007C7C RID: 31868
		[Range(0f, 1f)]
		public FsmFloat Volume;

		// Token: 0x04007C7D RID: 31869
		public FsmBool CopyVolume;

		// Token: 0x04007C7E RID: 31870
		public FsmBool CopyClip;

		// Token: 0x04007C7F RID: 31871
		public FsmBool StartPlaying;
	}
}
