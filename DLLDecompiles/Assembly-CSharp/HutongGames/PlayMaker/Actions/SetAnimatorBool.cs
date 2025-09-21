using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E08 RID: 3592
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of a bool parameter")]
	public class SetAnimatorBool : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x06006780 RID: 26496 RVA: 0x0020A451 File Offset: 0x00208651
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006781 RID: 26497 RVA: 0x0020A459 File Offset: 0x00208659
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
			this.Value = null;
		}

		// Token: 0x06006782 RID: 26498 RVA: 0x0020A476 File Offset: 0x00208676
		public override void OnEnter()
		{
			this.SetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006783 RID: 26499 RVA: 0x0020A48C File Offset: 0x0020868C
		public override void OnActionUpdate()
		{
			this.SetParameter();
		}

		// Token: 0x06006784 RID: 26500 RVA: 0x0020A494 File Offset: 0x00208694
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
			this.animator.SetBool(this.paramID, this.Value.Value);
		}

		// Token: 0x040066C3 RID: 26307
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066C4 RID: 26308
		[RequiredField]
		[UIHint(UIHint.AnimatorBool)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x040066C5 RID: 26309
		[Tooltip("The Bool value to assign to the animator parameter")]
		public FsmBool Value;

		// Token: 0x040066C6 RID: 26310
		private string cachedParameter;

		// Token: 0x040066C7 RID: 26311
		private int paramID;
	}
}
