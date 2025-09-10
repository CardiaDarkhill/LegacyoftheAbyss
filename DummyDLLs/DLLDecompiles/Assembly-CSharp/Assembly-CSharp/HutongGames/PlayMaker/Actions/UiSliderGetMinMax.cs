using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200116E RID: 4462
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the minimum and maximum limits for the value of a UI Slider component.")]
	public class UiSliderGetMinMax : ComponentAction<Slider>
	{
		// Token: 0x060077CC RID: 30668 RVA: 0x00246293 File Offset: 0x00244493
		public override void Reset()
		{
			this.gameObject = null;
			this.minValue = null;
			this.maxValue = null;
		}

		// Token: 0x060077CD RID: 30669 RVA: 0x002462AC File Offset: 0x002444AC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			this.DoGetValue();
		}

		// Token: 0x060077CE RID: 30670 RVA: 0x002462E8 File Offset: 0x002444E8
		private void DoGetValue()
		{
			if (this.slider != null)
			{
				if (!this.minValue.IsNone)
				{
					this.minValue.Value = this.slider.minValue;
				}
				if (!this.maxValue.IsNone)
				{
					this.maxValue.Value = this.slider.maxValue;
				}
			}
		}

		// Token: 0x0400784A RID: 30794
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400784B RID: 30795
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the minimum value of the UI Slider.")]
		public FsmFloat minValue;

		// Token: 0x0400784C RID: 30796
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the maximum value of the UI Slider.")]
		public FsmFloat maxValue;

		// Token: 0x0400784D RID: 30797
		private Slider slider;
	}
}
