using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001203 RID: 4611
	public abstract class LimitedPlayAudioBase : FsmStateAction
	{
		// Token: 0x06007AAC RID: 31404 RVA: 0x0024D05F File Offset: 0x0024B25F
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
		}

		// Token: 0x06007AAD RID: 31405 RVA: 0x0024D08C File Offset: 0x0024B28C
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.groupId.Value))
			{
				base.Finish();
				return;
			}
			AudioGroupManager.EnsureGroupExists(this.groupId.Value, this.maxActive.Value, this.maxPerFrame.Value);
			AudioGroupManager.AudioGroup audioGroup;
			if (!AudioGroupManager.CanPlay(this.groupId.Value, out audioGroup))
			{
				base.Finish();
				return;
			}
			AudioSource source;
			if (this.PlayAudio(out source))
			{
				audioGroup.AddSource(source);
			}
			base.Finish();
		}

		// Token: 0x06007AAE RID: 31406
		protected abstract bool PlayAudio(out AudioSource audioSource);

		// Token: 0x04007AEA RID: 31466
		public FsmString groupId;

		// Token: 0x04007AEB RID: 31467
		public FsmInt maxPerFrame = 2;

		// Token: 0x04007AEC RID: 31468
		public FsmInt maxActive = 5;
	}
}
