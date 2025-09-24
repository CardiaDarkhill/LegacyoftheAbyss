using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001170 RID: 4464
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the value of a UI Slider component.")]
	public class UiSliderGetValue : ComponentAction<Slider>
	{
		// Token: 0x060077D5 RID: 30677 RVA: 0x002463E6 File Offset: 0x002445E6
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.everyFrame = false;
		}

		// Token: 0x060077D6 RID: 30678 RVA: 0x00246400 File Offset: 0x00244600
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.slider = this.cachedComponent;
			}
			this.DoGetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077D7 RID: 30679 RVA: 0x00246448 File Offset: 0x00244648
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x060077D8 RID: 30680 RVA: 0x00246450 File Offset: 0x00244650
		private void DoGetValue()
		{
			if (this.slider != null)
			{
				this.value.Value = this.slider.value;
			}
		}

		// Token: 0x04007852 RID: 30802
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007853 RID: 30803
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The value of the UI Slider component.")]
		public FsmFloat value;

		// Token: 0x04007854 RID: 30804
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007855 RID: 30805
		private Slider slider;
	}
}
