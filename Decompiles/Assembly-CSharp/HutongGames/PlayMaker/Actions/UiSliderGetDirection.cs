using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200116D RID: 4461
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the direction of a UI Slider component.")]
	public class UiSliderGetDirection : ComponentAction<Slider>
	{
		// Token: 0x060077C7 RID: 30663 RVA: 0x002461F6 File Offset: 0x002443F6
		public override void Reset()
		{
			this.gameObject = null;
			this.direction = null;
			this.everyFrame = false;
		}

		// Token: 0x060077C8 RID: 30664 RVA: 0x00246210 File Offset: 0x00244410
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

		// Token: 0x060077C9 RID: 30665 RVA: 0x00246258 File Offset: 0x00244458
		public override void OnUpdate()
		{
			this.DoGetValue();
		}

		// Token: 0x060077CA RID: 30666 RVA: 0x00246260 File Offset: 0x00244460
		private void DoGetValue()
		{
			if (this.slider != null)
			{
				this.direction.Value = this.slider.direction;
			}
		}

		// Token: 0x04007846 RID: 30790
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007847 RID: 30791
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The direction of the UI Slider.")]
		[ObjectType(typeof(Slider.Direction))]
		public FsmEnum direction;

		// Token: 0x04007848 RID: 30792
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007849 RID: 30793
		private Slider slider;
	}
}
