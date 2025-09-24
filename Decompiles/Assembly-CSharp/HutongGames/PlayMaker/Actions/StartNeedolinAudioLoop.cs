using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001208 RID: 4616
	public class StartNeedolinAudioLoop : FsmStateAction
	{
		// Token: 0x06007ABC RID: 31420 RVA: 0x0024D4D9 File Offset: 0x0024B6D9
		public override void Reset()
		{
			this.AudioSource = null;
			this.DefaultClip = null;
		}

		// Token: 0x06007ABD RID: 31421 RVA: 0x0024D4EC File Offset: 0x0024B6EC
		public override void OnEnter()
		{
			AudioSource component = this.AudioSource.GetSafe(this).GetComponent<AudioSource>();
			AudioClip defaultClip = this.DefaultClip.Value as AudioClip;
			OverrideNeedolinLoop.StartSyncedAudio(component, defaultClip);
			base.Finish();
		}

		// Token: 0x04007AFF RID: 31487
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault AudioSource;

		// Token: 0x04007B00 RID: 31488
		[ObjectType(typeof(AudioClip))]
		public FsmObject DefaultClip;
	}
}
