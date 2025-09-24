using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB1 RID: 2993
	[ActionCategory(ActionCategory.Audio)]
	[ActionTarget(typeof(AtmosCue), "atmosCue", false)]
	[Tooltip("Plays atmos cues.")]
	public class ApplyAtmosCue : FsmStateAction
	{
		// Token: 0x06005C4F RID: 23631 RVA: 0x001D1418 File Offset: 0x001CF618
		public override void Reset()
		{
			this.atmosCue = null;
			this.transitionTime = 0f;
		}

		// Token: 0x06005C50 RID: 23632 RVA: 0x001D1434 File Offset: 0x001CF634
		public override void OnEnter()
		{
			AtmosCue x = this.atmosCue.Value as AtmosCue;
			GameManager instance = GameManager.instance;
			if (!(x == null))
			{
				if (instance == null)
				{
					Debug.LogErrorFormat(base.Owner, "Failed to play atmos cue, because the game manager is not ready", Array.Empty<object>());
				}
				else
				{
					instance.AudioManager.ApplyAtmosCue(x, this.transitionTime.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x040057C9 RID: 22473
		[Tooltip("Atmos cue to play.")]
		[ObjectType(typeof(AtmosCue))]
		public FsmObject atmosCue;

		// Token: 0x040057CA RID: 22474
		[Tooltip("Transition duration.")]
		public FsmFloat transitionTime;
	}
}
