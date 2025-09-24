using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001167 RID: 4455
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the number of distinct scroll positions allowed for a UI Scrollbar component.")]
	public class UiScrollbarSetNumberOfSteps : ComponentAction<Scrollbar>
	{
		// Token: 0x060077A3 RID: 30627 RVA: 0x00245C20 File Offset: 0x00243E20
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077A4 RID: 30628 RVA: 0x00245C40 File Offset: 0x00243E40
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollbar = this.cachedComponent;
			}
			this.originalValue = this.scrollbar.numberOfSteps;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077A5 RID: 30629 RVA: 0x00245C99 File Offset: 0x00243E99
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077A6 RID: 30630 RVA: 0x00245CA1 File Offset: 0x00243EA1
		private void DoSetValue()
		{
			if (this.scrollbar != null)
			{
				this.scrollbar.numberOfSteps = this.value.Value;
			}
		}

		// Token: 0x060077A7 RID: 30631 RVA: 0x00245CC7 File Offset: 0x00243EC7
		public override void OnExit()
		{
			if (this.scrollbar == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.scrollbar.numberOfSteps = this.originalValue;
			}
		}

		// Token: 0x04007820 RID: 30752
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007821 RID: 30753
		[RequiredField]
		[Tooltip("The number of distinct scroll positions allowed for the UI Scrollbar.")]
		public FsmInt value;

		// Token: 0x04007822 RID: 30754
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007823 RID: 30755
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007824 RID: 30756
		private Scrollbar scrollbar;

		// Token: 0x04007825 RID: 30757
		private int originalValue;
	}
}
