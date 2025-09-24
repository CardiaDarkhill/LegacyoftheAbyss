using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001177 RID: 4471
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the wholeNumbers property of a UI Slider component. This defines if the slider will be constrained to integer values.")]
	public class UiSliderSetWholeNumbers : ComponentAction<Slider>
	{
		// Token: 0x060077FA RID: 30714 RVA: 0x00246A8B File Offset: 0x00244C8B
		public override void Reset()
		{
			this.gameObject = null;
			this.wholeNumbers = null;
			this.resetOnExit = null;
		}

		// Token: 0x060077FB RID: 30715 RVA: 0x00246AA4 File Offset: 0x00244CA4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			this.originalValue = this.slider.wholeNumbers;
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x060077FC RID: 30716 RVA: 0x00246AF5 File Offset: 0x00244CF5
		private void DoSetValue()
		{
			if (this.slider != null)
			{
				this.slider.wholeNumbers = this.wholeNumbers.Value;
			}
		}

		// Token: 0x060077FD RID: 30717 RVA: 0x00246B1B File Offset: 0x00244D1B
		public override void OnExit()
		{
			if (this.slider == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.slider.wholeNumbers = this.originalValue;
			}
		}

		// Token: 0x0400787A RID: 30842
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400787B RID: 30843
		[RequiredField]
		[Tooltip("Should the slider be constrained to integer values?")]
		public FsmBool wholeNumbers;

		// Token: 0x0400787C RID: 30844
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400787D RID: 30845
		private Slider slider;

		// Token: 0x0400787E RID: 30846
		private bool originalValue;
	}
}
