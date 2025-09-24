using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001175 RID: 4469
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the normalized value ( between 0 and 1) of a UI Slider component.")]
	public class UiSliderSetNormalizedValue : ComponentAction<Slider>
	{
		// Token: 0x060077EE RID: 30702 RVA: 0x002468CE File Offset: 0x00244ACE
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077EF RID: 30703 RVA: 0x002468EC File Offset: 0x00244AEC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			this.originalValue = this.slider.normalizedValue;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077F0 RID: 30704 RVA: 0x00246945 File Offset: 0x00244B45
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077F1 RID: 30705 RVA: 0x0024694D File Offset: 0x00244B4D
		private void DoSetValue()
		{
			if (this.slider != null)
			{
				this.slider.normalizedValue = this.value.Value;
			}
		}

		// Token: 0x060077F2 RID: 30706 RVA: 0x00246973 File Offset: 0x00244B73
		public override void OnExit()
		{
			if (this.slider == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.slider.normalizedValue = this.originalValue;
			}
		}

		// Token: 0x0400786E RID: 30830
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400786F RID: 30831
		[RequiredField]
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The normalized value ( between 0 and 1) of the UI Slider component.")]
		public FsmFloat value;

		// Token: 0x04007870 RID: 30832
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007871 RID: 30833
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007872 RID: 30834
		private Slider slider;

		// Token: 0x04007873 RID: 30835
		private float originalValue;
	}
}
