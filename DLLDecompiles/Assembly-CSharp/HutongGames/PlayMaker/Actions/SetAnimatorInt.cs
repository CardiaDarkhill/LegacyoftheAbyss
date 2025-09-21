using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E0D RID: 3597
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of an integer parameter")]
	public class SetAnimatorInt : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x06006799 RID: 26521 RVA: 0x0020A998 File Offset: 0x00208B98
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x0600679A RID: 26522 RVA: 0x0020A9A0 File Offset: 0x00208BA0
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
			this.Value = null;
		}

		// Token: 0x0600679B RID: 26523 RVA: 0x0020A9BD File Offset: 0x00208BBD
		public override void OnEnter()
		{
			this.SetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600679C RID: 26524 RVA: 0x0020A9D3 File Offset: 0x00208BD3
		public override void OnActionUpdate()
		{
			this.SetParameter();
		}

		// Token: 0x0600679D RID: 26525 RVA: 0x0020A9DC File Offset: 0x00208BDC
		private void SetParameter()
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
			this.animator.SetInteger(this.paramID, this.Value.Value);
		}

		// Token: 0x040066DD RID: 26333
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066DE RID: 26334
		[RequiredField]
		[UIHint(UIHint.AnimatorInt)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x040066DF RID: 26335
		[Tooltip("The Int value to assign to the animator parameter")]
		public FsmInt Value;

		// Token: 0x040066E0 RID: 26336
		private string cachedParameter;

		// Token: 0x040066E1 RID: 26337
		private int paramID;
	}
}
