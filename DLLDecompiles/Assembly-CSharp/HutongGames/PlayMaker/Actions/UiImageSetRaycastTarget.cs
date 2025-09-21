using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001146 RID: 4422
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the raycast target of a UI Image component.")]
	public class UiImageSetRaycastTarget : ComponentAction<Image>
	{
		// Token: 0x060076FF RID: 30463 RVA: 0x0024426E File Offset: 0x0024246E
		public override void Reset()
		{
			this.gameObject = null;
			this.raycastTarget = null;
			this.resetOnExit = false;
		}

		// Token: 0x06007700 RID: 30464 RVA: 0x0024428C File Offset: 0x0024248C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.originalBool = this.cachedComponent.raycastTarget;
				this.DoSetRaycastTarget();
			}
			base.Finish();
		}

		// Token: 0x06007701 RID: 30465 RVA: 0x002442D1 File Offset: 0x002424D1
		private void DoSetRaycastTarget()
		{
			this.cachedComponent.raycastTarget = this.raycastTarget.Value;
		}

		// Token: 0x06007702 RID: 30466 RVA: 0x002442E9 File Offset: 0x002424E9
		public override void OnExit()
		{
			if (this.resetOnExit.Value)
			{
				this.cachedComponent.raycastTarget = this.originalBool;
			}
		}

		// Token: 0x0400777A RID: 30586
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the Image UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400777B RID: 30587
		[RequiredField]
		[Tooltip("The raycast target value to be set")]
		public FsmBool raycastTarget;

		// Token: 0x0400777C RID: 30588
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400777D RID: 30589
		private bool originalBool;
	}
}
