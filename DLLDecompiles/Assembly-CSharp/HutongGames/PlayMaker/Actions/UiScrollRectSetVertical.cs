using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200116C RID: 4460
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the UI ScrollRect vertical flag")]
	public class UiScrollRectSetVertical : ComponentAction<ScrollRect>
	{
		// Token: 0x060077C1 RID: 30657 RVA: 0x0024611A File Offset: 0x0024431A
		public override void Reset()
		{
			this.gameObject = null;
			this.vertical = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077C2 RID: 30658 RVA: 0x00246138 File Offset: 0x00244338
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

		// Token: 0x060077C3 RID: 30659 RVA: 0x00246191 File Offset: 0x00244391
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077C4 RID: 30660 RVA: 0x00246199 File Offset: 0x00244399
		private void DoSetValue()
		{
			if (this.scrollRect != null)
			{
				this.scrollRect.vertical = this.vertical.Value;
			}
		}

		// Token: 0x060077C5 RID: 30661 RVA: 0x002461BF File Offset: 0x002443BF
		public override void OnExit()
		{
			if (this.scrollRect == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.scrollRect.vertical = this.originalValue;
			}
		}

		// Token: 0x04007840 RID: 30784
		[RequiredField]
		[CheckForComponent(typeof(ScrollRect))]
		[Tooltip("The GameObject with the UI ScrollRect component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007841 RID: 30785
		[Tooltip("The vertical flag")]
		public FsmBool vertical;

		// Token: 0x04007842 RID: 30786
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007843 RID: 30787
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007844 RID: 30788
		private ScrollRect scrollRect;

		// Token: 0x04007845 RID: 30789
		private bool originalValue;
	}
}
