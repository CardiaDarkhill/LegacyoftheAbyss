using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001204 RID: 4612
	public sealed class LimitedPlayAudioClipOnSource : LimitedPlayAudioBase
	{
		// Token: 0x06007AB0 RID: 31408 RVA: 0x0024D12B File Offset: 0x0024B32B
		public override void Reset()
		{
			base.Reset();
			this.audioSource = null;
			this.audioClip = null;
			this.playOneShot = null;
			this.volume = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007AB1 RID: 31409 RVA: 0x0024D15C File Offset: 0x0024B35C
		protected override bool PlayAudio(out AudioSource audioSource)
		{
			AudioSource safe = this.audioSource.GetSafe(this);
			if (safe == null)
			{
				audioSource = null;
				return false;
			}
			AudioClip audioClip = this.audioClip.Value as AudioClip;
			if (audioClip == null)
			{
				audioSource = null;
				return false;
			}
			bool flag;
			if (this.playOneShot.Value)
			{
				flag = AudioGroupManager.PlayOneShotClip(this.groupId.Value, safe, audioClip);
			}
			else
			{
				flag = AudioGroupManager.PlayClip(this.groupId.Value, safe, audioClip);
			}
			if (flag && !this.volume.IsNone)
			{
				safe.volume = this.volume.Value;
			}
			audioSource = safe;
			return flag;
		}

		// Token: 0x04007AED RID: 31469
		[Space]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		// Token: 0x04007AEE RID: 31470
		[RequiredField]
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x04007AEF RID: 31471
		public FsmBool playOneShot;

		// Token: 0x04007AF0 RID: 31472
		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;
	}
}
