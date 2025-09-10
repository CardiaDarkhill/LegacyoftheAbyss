using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001207 RID: 4615
	public sealed class LimitedRemoveAudioLooped : FsmStateAction
	{
		// Token: 0x06007AB9 RID: 31417 RVA: 0x0024D44C File Offset: 0x0024B64C
		public override void Reset()
		{
			this.groupId = null;
			this.audioSource = null;
			this.stopSource = null;
		}

		// Token: 0x06007ABA RID: 31418 RVA: 0x0024D464 File Offset: 0x0024B664
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
			AudioGroupManager.RemoveLoopClip(this.groupId.Value, safe);
			if (this.stopSource.Value)
			{
				safe.Stop();
			}
			base.Finish();
		}

		// Token: 0x04007AFC RID: 31484
		public FsmString groupId;

		// Token: 0x04007AFD RID: 31485
		[Space]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault audioSource;

		// Token: 0x04007AFE RID: 31486
		public FsmBool stopSource;
	}
}
