using System;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001176 RID: 4470
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the value of a UI Slider component.")]
	public class UiSliderSetValue : ComponentAction<Slider>
	{
		// Token: 0x060077F4 RID: 30708 RVA: 0x002469AA File Offset: 0x00244BAA
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077F5 RID: 30709 RVA: 0x002469C8 File Offset: 0x00244BC8
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.slider = this.cachedComponent;
			this.originalValue = this.slider.value;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077F6 RID: 30710 RVA: 0x00246A26 File Offset: 0x00244C26
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077F7 RID: 30711 RVA: 0x00246A2E File Offset: 0x00244C2E
		private void DoSetValue()
		{
			if (this.slider != null)
			{
				this.slider.value = this.value.Value;
			}
		}

		// Token: 0x060077F8 RID: 30712 RVA: 0x00246A54 File Offset: 0x00244C54
		public override void OnExit()
		{
			if (this.slider == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.slider.value = this.originalValue;
			}
		}

		// Token: 0x04007874 RID: 30836
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007875 RID: 30837
		[RequiredField]
		[Tooltip("The value of the UI Slider component.")]
		public FsmFloat value;

		// Token: 0x04007876 RID: 30838
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007877 RID: 30839
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007878 RID: 30840
		private Slider slider;

		// Token: 0x04007879 RID: 30841
		private float originalValue;
	}
}
