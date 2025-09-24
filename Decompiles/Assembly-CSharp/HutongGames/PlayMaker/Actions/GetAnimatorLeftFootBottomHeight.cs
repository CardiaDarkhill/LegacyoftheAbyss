using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFC RID: 3580
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Get the left foot bottom height.")]
	public class GetAnimatorLeftFootBottomHeight : ComponentAction<Animator>
	{
		// Token: 0x0600673E RID: 26430 RVA: 0x002099AE File Offset: 0x00207BAE
		public override void Reset()
		{
			this.gameObject = null;
			this.leftFootHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x0600673F RID: 26431 RVA: 0x002099C5 File Offset: 0x00207BC5
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x06006740 RID: 26432 RVA: 0x002099D3 File Offset: 0x00207BD3
		public override void OnEnter()
		{
			this.GetLeftFootBottomHeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006741 RID: 26433 RVA: 0x002099E9 File Offset: 0x00207BE9
		public override void OnLateUpdate()
		{
			this.GetLeftFootBottomHeight();
		}

		// Token: 0x06006742 RID: 26434 RVA: 0x002099F1 File Offset: 0x00207BF1
		private void GetLeftFootBottomHeight()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.leftFootHeight.Value = this.cachedComponent.leftFeetBottomHeight;
			}
		}

		// Token: 0x0400668D RID: 26253
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400668E RID: 26254
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("the left foot bottom height.")]
		public FsmFloat leftFootHeight;

		// Token: 0x0400668F RID: 26255
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;
	}
}
