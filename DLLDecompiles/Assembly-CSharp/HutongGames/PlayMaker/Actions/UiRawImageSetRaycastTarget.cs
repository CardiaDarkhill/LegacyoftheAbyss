using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001160 RID: 4448
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the raycast target of a UI Raw Image component.")]
	public class UiRawImageSetRaycastTarget : ComponentAction<RawImage>
	{
		// Token: 0x06007780 RID: 30592 RVA: 0x002456A6 File Offset: 0x002438A6
		public override void Reset()
		{
			this.gameObject = null;
			this.raycastTarget = null;
			this.resetOnExit = false;
		}

		// Token: 0x06007781 RID: 30593 RVA: 0x002456C4 File Offset: 0x002438C4
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

		// Token: 0x06007782 RID: 30594 RVA: 0x00245709 File Offset: 0x00243909
		private void DoSetRaycastTarget()
		{
			this.cachedComponent.raycastTarget = this.raycastTarget.Value;
		}

		// Token: 0x06007783 RID: 30595 RVA: 0x00245721 File Offset: 0x00243921
		public override void OnExit()
		{
			if (this.resetOnExit.Value)
			{
				this.cachedComponent.raycastTarget = this.originalBool;
			}
		}

		// Token: 0x04007800 RID: 30720
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the Raw Image UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007801 RID: 30721
		[RequiredField]
		[Tooltip("The raycast target value to be set")]
		public FsmBool raycastTarget;

		// Token: 0x04007802 RID: 30722
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007803 RID: 30723
		private bool originalBool;
	}
}
