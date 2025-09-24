using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001145 RID: 4421
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Set The Fill Amount on a UI Image")]
	public class UiImageSetFillAmount : ComponentAction<Image>
	{
		// Token: 0x060076FA RID: 30458 RVA: 0x002441CE File Offset: 0x002423CE
		public override void Reset()
		{
			this.gameObject = null;
			this.ImageFillAmount = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060076FB RID: 30459 RVA: 0x002441F0 File Offset: 0x002423F0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.image = this.cachedComponent;
			}
			this.DoSetFillAmount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076FC RID: 30460 RVA: 0x00244238 File Offset: 0x00242438
		public override void OnUpdate()
		{
			this.DoSetFillAmount();
		}

		// Token: 0x060076FD RID: 30461 RVA: 0x00244240 File Offset: 0x00242440
		private void DoSetFillAmount()
		{
			if (this.image != null)
			{
				this.image.fillAmount = this.ImageFillAmount.Value;
			}
		}

		// Token: 0x04007776 RID: 30582
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the UI Image component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007777 RID: 30583
		[RequiredField]
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The fill amount.")]
		public FsmFloat ImageFillAmount;

		// Token: 0x04007778 RID: 30584
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007779 RID: 30585
		private Image image;
	}
}
