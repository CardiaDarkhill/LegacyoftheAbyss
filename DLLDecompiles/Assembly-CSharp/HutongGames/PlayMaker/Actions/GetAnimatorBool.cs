using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE5 RID: 3557
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the value of a bool parameter")]
	public class GetAnimatorBool : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x060066CC RID: 26316 RVA: 0x0020872B File Offset: 0x0020692B
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066CD RID: 26317 RVA: 0x00208733 File Offset: 0x00206933
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
			this.result = null;
		}

		// Token: 0x060066CE RID: 26318 RVA: 0x00208750 File Offset: 0x00206950
		public override void OnEnter()
		{
			this.GetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066CF RID: 26319 RVA: 0x00208766 File Offset: 0x00206966
		public override void OnActionUpdate()
		{
			this.GetParameter();
		}

		// Token: 0x060066D0 RID: 26320 RVA: 0x00208770 File Offset: 0x00206970
		private void GetParameter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedParameter != this.parameter.Value)
			{
				this.cachedParameter = this.parameter.Value;
				this.paramID = Animator.StringToHash(this.parameter.Value);
			}
			this.result.Value = this.animator.GetBool(this.paramID);
		}

		// Token: 0x0400661F RID: 26143
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006620 RID: 26144
		[RequiredField]
		[UIHint(UIHint.AnimatorBool)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x04006621 RID: 26145
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The bool value of the animator parameter")]
		public FsmBool result;

		// Token: 0x04006622 RID: 26146
		private string cachedParameter;

		// Token: 0x04006623 RID: 26147
		private int paramID;
	}
}
