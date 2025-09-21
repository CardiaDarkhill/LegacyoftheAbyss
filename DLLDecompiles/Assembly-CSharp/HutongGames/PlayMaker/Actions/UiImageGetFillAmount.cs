using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001143 RID: 4419
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Set The Fill Amount on a UI Image")]
	public class UiImageGetFillAmount : ComponentAction<Image>
	{
		// Token: 0x060076F1 RID: 30449 RVA: 0x002440B8 File Offset: 0x002422B8
		public override void Reset()
		{
			this.gameObject = null;
			this.ImageFillAmount = null;
			this.everyFrame = false;
		}

		// Token: 0x060076F2 RID: 30450 RVA: 0x002440D0 File Offset: 0x002422D0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.image = this.cachedComponent;
			}
			this.DoGetFillAmount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076F3 RID: 30451 RVA: 0x00244118 File Offset: 0x00242318
		public override void OnUpdate()
		{
			this.DoGetFillAmount();
		}

		// Token: 0x060076F4 RID: 30452 RVA: 0x00244120 File Offset: 0x00242320
		private void DoGetFillAmount()
		{
			if (this.image != null)
			{
				this.ImageFillAmount.Value = this.image.fillAmount;
			}
		}

		// Token: 0x0400776F RID: 30575
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the UI Image component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007770 RID: 30576
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The fill amount.")]
		public FsmFloat ImageFillAmount;

		// Token: 0x04007771 RID: 30577
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007772 RID: 30578
		private Image image;
	}
}
