using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D7 RID: 4823
	public class StartFadeOutAudioSource : FsmStateAction
	{
		// Token: 0x06007DD4 RID: 32212 RVA: 0x0025762F File Offset: 0x0025582F
		public override void Reset()
		{
			this.Target = null;
			this.Duration = null;
			this.RecycleOnEnd = null;
		}

		// Token: 0x06007DD5 RID: 32213 RVA: 0x00257648 File Offset: 0x00255848
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				FadeOutAudioSource fadeOutAudioSource = safe.GetComponent<FadeOutAudioSource>() ?? safe.AddComponent<FadeOutAudioSource>();
				if (fadeOutAudioSource)
				{
					fadeOutAudioSource.StartFade(this.Duration.Value, this.RecycleOnEnd.Value ? FadeOutAudioSource.EndBehaviours.Recycle : FadeOutAudioSource.EndBehaviours.None);
				}
			}
			base.Finish();
		}

		// Token: 0x04007DB6 RID: 32182
		public FsmOwnerDefault Target;

		// Token: 0x04007DB7 RID: 32183
		public FsmFloat Duration;

		// Token: 0x04007DB8 RID: 32184
		public FsmBool RecycleOnEnd;
	}
}
