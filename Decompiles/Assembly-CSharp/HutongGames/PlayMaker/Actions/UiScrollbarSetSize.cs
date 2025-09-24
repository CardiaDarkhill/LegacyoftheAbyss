using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001168 RID: 4456
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the fractional size of the handle of a UI Scrollbar component. Ranges from 0.0 to 1.0.")]
	public class UiScrollbarSetSize : ComponentAction<Scrollbar>
	{
		// Token: 0x060077A9 RID: 30633 RVA: 0x00245CFE File Offset: 0x00243EFE
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077AA RID: 30634 RVA: 0x00245D1C File Offset: 0x00243F1C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollbar = this.cachedComponent;
			}
			if (this.resetOnExit.Value)
			{
				this.originalValue = this.scrollbar.size;
			}
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077AB RID: 30635 RVA: 0x00245D82 File Offset: 0x00243F82
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077AC RID: 30636 RVA: 0x00245D8A File Offset: 0x00243F8A
		private void DoSetValue()
		{
			if (this.scrollbar != null)
			{
				this.scrollbar.size = this.value.Value;
			}
		}

		// Token: 0x060077AD RID: 30637 RVA: 0x00245DB0 File Offset: 0x00243FB0
		public override void OnExit()
		{
			if (this.scrollbar == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.scrollbar.size = this.originalValue;
			}
		}

		// Token: 0x04007826 RID: 30758
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007827 RID: 30759
		[RequiredField]
		[Tooltip("The fractional size of the handle for the UI Scrollbar. Ranges from 0.0 to 1.0.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat value;

		// Token: 0x04007828 RID: 30760
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007829 RID: 30761
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x0400782A RID: 30762
		private Scrollbar scrollbar;

		// Token: 0x0400782B RID: 30763
		private float originalValue;
	}
}
