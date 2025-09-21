using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001169 RID: 4457
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the position value of a UI Scrollbar component. Ranges from 0.0 to 1.0.")]
	public class UiScrollbarSetValue : ComponentAction<Scrollbar>
	{
		// Token: 0x060077AF RID: 30639 RVA: 0x00245DE7 File Offset: 0x00243FE7
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077B0 RID: 30640 RVA: 0x00245E08 File Offset: 0x00244008
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollbar = this.cachedComponent;
			}
			this.originalValue = this.scrollbar.value;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077B1 RID: 30641 RVA: 0x00245E61 File Offset: 0x00244061
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077B2 RID: 30642 RVA: 0x00245E69 File Offset: 0x00244069
		private void DoSetValue()
		{
			if (this.scrollbar != null)
			{
				this.scrollbar.value = this.value.Value;
			}
		}

		// Token: 0x060077B3 RID: 30643 RVA: 0x00245E8F File Offset: 0x0024408F
		public override void OnExit()
		{
			if (this.scrollbar == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.scrollbar.value = this.originalValue;
			}
		}

		// Token: 0x0400782C RID: 30764
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400782D RID: 30765
		[RequiredField]
		[Tooltip("The position's value of the UI Scrollbar component. Ranges from 0.0 to 1.0.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat value;

		// Token: 0x0400782E RID: 30766
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400782F RID: 30767
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007830 RID: 30768
		private Scrollbar scrollbar;

		// Token: 0x04007831 RID: 30769
		private float originalValue;
	}
}
