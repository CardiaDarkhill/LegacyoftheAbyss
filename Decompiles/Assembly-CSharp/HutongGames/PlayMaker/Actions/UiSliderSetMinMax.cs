using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001174 RID: 4468
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the minimum and maximum limits for the value of a UI Slider component. Optionally resets on exit")]
	public class UiSliderSetMinMax : ComponentAction<Slider>
	{
		// Token: 0x060077E8 RID: 30696 RVA: 0x00246768 File Offset: 0x00244968
		public override void Reset()
		{
			this.gameObject = null;
			this.minValue = new FsmFloat
			{
				UseVariable = true
			};
			this.maxValue = new FsmFloat
			{
				UseVariable = true
			};
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077E9 RID: 30697 RVA: 0x002467A4 File Offset: 0x002449A4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			if (this.resetOnExit.Value)
			{
				this.originalMinValue = this.slider.minValue;
				this.originalMaxValue = this.slider.maxValue;
			}
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077EA RID: 30698 RVA: 0x0024681B File Offset: 0x00244A1B
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077EB RID: 30699 RVA: 0x00246824 File Offset: 0x00244A24
		private void DoSetValue()
		{
			if (this.slider == null)
			{
				return;
			}
			if (!this.minValue.IsNone)
			{
				this.slider.minValue = this.minValue.Value;
			}
			if (!this.maxValue.IsNone)
			{
				this.slider.maxValue = this.maxValue.Value;
			}
		}

		// Token: 0x060077EC RID: 30700 RVA: 0x00246886 File Offset: 0x00244A86
		public override void OnExit()
		{
			if (this.slider == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.slider.minValue = this.originalMinValue;
				this.slider.maxValue = this.originalMaxValue;
			}
		}

		// Token: 0x04007866 RID: 30822
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007867 RID: 30823
		[Tooltip("The minimum value of the UI Slider component. Leave as None for no effect")]
		public FsmFloat minValue;

		// Token: 0x04007868 RID: 30824
		[Tooltip("The maximum value of the UI Slider component. Leave as None for no effect")]
		public FsmFloat maxValue;

		// Token: 0x04007869 RID: 30825
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400786A RID: 30826
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x0400786B RID: 30827
		private Slider slider;

		// Token: 0x0400786C RID: 30828
		private float originalMinValue;

		// Token: 0x0400786D RID: 30829
		private float originalMaxValue;
	}
}
