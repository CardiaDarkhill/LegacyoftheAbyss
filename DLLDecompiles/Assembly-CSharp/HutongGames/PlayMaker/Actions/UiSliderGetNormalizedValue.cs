using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200116F RID: 4463
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the normalized value (between 0 and 1) of a UI Slider component.")]
	public class UiSliderGetNormalizedValue : ComponentAction<Slider>
	{
		// Token: 0x060077D0 RID: 30672 RVA: 0x00246351 File Offset: 0x00244551
		public override void Reset()
		{
			this.gameObject = null;
			this.value = null;
			this.everyFrame = false;
		}

		// Token: 0x060077D1 RID: 30673 RVA: 0x00246368 File Offset: 0x00244568
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

		// Token: 0x060077D2 RID: 30674 RVA: 0x002463B0 File Offset: 0x002445B0
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x060077D3 RID: 30675 RVA: 0x002463B8 File Offset: 0x002445B8
		private void DoGetValue()
		{
			if (this.slider != null)
			{
				this.value.Value = this.slider.normalizedValue;
			}
		}

		// Token: 0x0400784E RID: 30798
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400784F RID: 30799
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The normalized value (between 0 and 1) of the UI Slider.")]
		public FsmFloat value;

		// Token: 0x04007850 RID: 30800
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007851 RID: 30801
		private Slider slider;
	}
}
