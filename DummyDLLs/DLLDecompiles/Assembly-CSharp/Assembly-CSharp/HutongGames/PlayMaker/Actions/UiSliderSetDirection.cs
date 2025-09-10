using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001173 RID: 4467
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the direction of a UI Slider component.")]
	public class UiSliderSetDirection : ComponentAction<Slider>
	{
		// Token: 0x060077E3 RID: 30691 RVA: 0x00246611 File Offset: 0x00244811
		public override void Reset()
		{
			this.gameObject = null;
			this.direction = Slider.Direction.LeftToRight;
			this.includeRectLayouts = new FsmBool
			{
				UseVariable = true
			};
			this.resetOnExit = null;
		}

		// Token: 0x060077E4 RID: 30692 RVA: 0x00246644 File Offset: 0x00244844
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			this.originalValue = this.slider.direction;
			this.DoSetValue();
		}

		// Token: 0x060077E5 RID: 30693 RVA: 0x00246690 File Offset: 0x00244890
		private void DoSetValue()
		{
			if (this.slider == null)
			{
				return;
			}
			if (this.includeRectLayouts.IsNone)
			{
				this.slider.direction = (Slider.Direction)this.direction.Value;
				return;
			}
			this.slider.SetDirection((Slider.Direction)this.direction.Value, this.includeRectLayouts.Value);
		}

		// Token: 0x060077E6 RID: 30694 RVA: 0x002466FC File Offset: 0x002448FC
		public override void OnExit()
		{
			if (this.slider == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				if (this.includeRectLayouts.IsNone)
				{
					this.slider.direction = this.originalValue;
					return;
				}
				this.slider.SetDirection(this.originalValue, this.includeRectLayouts.Value);
			}
		}

		// Token: 0x04007860 RID: 30816
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007861 RID: 30817
		[RequiredField]
		[Tooltip("The direction of the UI Slider component.")]
		[ObjectType(typeof(Slider.Direction))]
		public FsmEnum direction;

		// Token: 0x04007862 RID: 30818
		[Tooltip("Include the  RectLayouts. Leave to none for no effect")]
		public FsmBool includeRectLayouts;

		// Token: 0x04007863 RID: 30819
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x04007864 RID: 30820
		private Slider slider;

		// Token: 0x04007865 RID: 30821
		private Slider.Direction originalValue;
	}
}
