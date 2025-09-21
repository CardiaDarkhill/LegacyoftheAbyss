using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDD RID: 3549
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the animator in playback mode.")]
	public class AnimatorStartPlayback : ComponentAction<Animator>
	{
		// Token: 0x060066AC RID: 26284 RVA: 0x002082D1 File Offset: 0x002064D1
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060066AD RID: 26285 RVA: 0x002082DA File Offset: 0x002064DA
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.StartPlayback();
			}
			base.Finish();
		}

		// Token: 0x04006608 RID: 26120
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;
	}
}
