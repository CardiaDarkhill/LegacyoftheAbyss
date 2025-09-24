using System;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F4 RID: 4852
	public class SetSceneManagerActorSnapshot : FsmStateAction
	{
		// Token: 0x06007E56 RID: 32342 RVA: 0x00258B6B File Offset: 0x00256D6B
		public override void Reset()
		{
			this.snapshot = null;
		}

		// Token: 0x06007E57 RID: 32343 RVA: 0x00258B74 File Offset: 0x00256D74
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance)
			{
				instance.sm.actorSnapshot = (this.snapshot.Value as AudioMixerSnapshot);
			}
			base.Finish();
		}

		// Token: 0x04007E17 RID: 32279
		[ObjectType(typeof(AudioMixerSnapshot))]
		public FsmObject snapshot;
	}
}
