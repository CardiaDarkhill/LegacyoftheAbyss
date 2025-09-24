using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDF RID: 3551
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Stops the animator playback mode. When playback stops, the avatar resumes getting control from game logic")]
	public class AnimatorStopPlayback : ComponentAction<Animator>
	{
		// Token: 0x060066B2 RID: 26290 RVA: 0x00208362 File Offset: 0x00206562
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060066B3 RID: 26291 RVA: 0x0020836B File Offset: 0x0020656B
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.StopPlayback();
			}
			base.Finish();
		}

		// Token: 0x0400660B RID: 26123
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
	}
}
