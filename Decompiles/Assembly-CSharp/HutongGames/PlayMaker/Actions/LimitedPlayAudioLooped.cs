using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001206 RID: 4614
	public sealed class LimitedPlayAudioLooped : FsmStateAction
	{
		// Token: 0x06007AB6 RID: 31414 RVA: 0x0024D324 File Offset: 0x0024B524
		public override void Reset()
		{
			this.groupId = null;
			this.maxPerFrame = new FsmInt
			{
				UseVariable = true
			};
			this.maxActive = new FsmInt
			{
				UseVariable = true
			};
			this.audioSource = null;
			this.audioClip = null;
			this.volume = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007AB7 RID: 31415 RVA: 0x0024D37C File Offset: 0x0024B57C
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.groupId.Value))
			{
				base.Finish();
				return;
			}
			AudioSource safe = this.audioSource.GetSafe(this);
			if (safe == null)
			{
				base.Finish();
				return;
			}
			AudioGroupManager.EnsureGroupExists(this.groupId.Value, this.maxActive.Value, this.maxPerFrame.Value);
			AudioGroupManager.PlayLoopClip(this.groupId.Value, safe, this.audioClip.Value as AudioClip, this.volume.IsNone ? 1f : this.volume.Value);
			base.Finish();
		}

		// Token: 0x04007AF6 RID: 31478
		public FsmString groupId;

		// Token: 0x04007AF7 RID: 31479
		public FsmInt maxPerFrame = 2;

		// Token: 0x04007AF8 RID: 31480
		public FsmInt maxActive = 5;

		// Token: 0x04007AF9 RID: 31481
		[Space]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		// Token: 0x04007AFA RID: 31482
		[RequiredField]
		[ObjectType(typeof(AudioClip))]
		public FsmObject audioClip;

		// Token: 0x04007AFB RID: 31483
		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;
	}
}
