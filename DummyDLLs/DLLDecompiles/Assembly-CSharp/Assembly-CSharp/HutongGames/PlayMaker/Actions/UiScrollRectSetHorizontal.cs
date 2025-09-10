using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200116A RID: 4458
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the UI ScrollRect horizontal flag")]
	public class UiScrollRectSetHorizontal : ComponentAction<ScrollRect>
	{
		// Token: 0x060077B5 RID: 30645 RVA: 0x00245EC6 File Offset: 0x002440C6
		public override void Reset()
		{
			this.gameObject = null;
			this.horizontal = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077B6 RID: 30646 RVA: 0x00245EE4 File Offset: 0x002440E4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollRect = this.cachedComponent;
			}
			this.originalValue = this.scrollRect.vertical;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077B7 RID: 30647 RVA: 0x00245F3D File Offset: 0x0024413D
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077B8 RID: 30648 RVA: 0x00245F45 File Offset: 0x00244145
		private void DoSetValue()
		{
			if (this.scrollRect != null)
			{
				this.scrollRect.horizontal = this.horizontal.Value;
			}
		}

		// Token: 0x060077B9 RID: 30649 RVA: 0x00245F6B File Offset: 0x0024416B
		public override void OnExit()
		{
			if (this.scrollRect == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.scrollRect.horizontal = this.originalValue;
			}
		}

		// Token: 0x04007832 RID: 30770
		[RequiredField]
		[CheckForComponent(typeof(ScrollRect))]
		[Tooltip("The GameObject with the UI ScrollRect component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007833 RID: 30771
		[Tooltip("The horizontal flag")]
		public FsmBool horizontal;

		// Token: 0x04007834 RID: 30772
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007835 RID: 30773
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007836 RID: 30774
		private ScrollRect scrollRect;

		// Token: 0x04007837 RID: 30775
		private bool originalValue;
	}
}
