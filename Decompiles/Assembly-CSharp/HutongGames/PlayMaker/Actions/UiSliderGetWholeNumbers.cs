using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001171 RID: 4465
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the wholeNumbers property of a UI Slider component. If true, the Slider is constrained to integer values")]
	public class UiSliderGetWholeNumbers : ComponentAction<Slider>
	{
		// Token: 0x060077DA RID: 30682 RVA: 0x0024647E File Offset: 0x0024467E
		public override void Reset()
		{
			this.gameObject = null;
			this.isShowingWholeNumbersEvent = null;
			this.isNotShowingWholeNumbersEvent = null;
			this.wholeNumbers = null;
		}

		// Token: 0x060077DB RID: 30683 RVA: 0x0024649C File Offset: 0x0024469C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x060077DC RID: 30684 RVA: 0x002464DC File Offset: 0x002446DC
		private void DoGetValue()
		{
			bool flag = false;
			if (this.slider != null)
			{
				flag = this.slider.wholeNumbers;
			}
			this.wholeNumbers.Value = flag;
			base.Fsm.Event(flag ? this.isShowingWholeNumbersEvent : this.isNotShowingWholeNumbersEvent);
		}

		// Token: 0x04007856 RID: 30806
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007857 RID: 30807
		[UIHint(UIHint.Variable)]
		[Tooltip("Is the Slider constrained to integer values?")]
		public FsmBool wholeNumbers;

		// Token: 0x04007858 RID: 30808
		[Tooltip("Event sent if slider is showing integers")]
		public FsmEvent isShowingWholeNumbersEvent;

		// Token: 0x04007859 RID: 30809
		[Tooltip("Event sent if slider is showing floats")]
		public FsmEvent isNotShowingWholeNumbersEvent;

		// Token: 0x0400785A RID: 30810
		private Slider slider;
	}
}
