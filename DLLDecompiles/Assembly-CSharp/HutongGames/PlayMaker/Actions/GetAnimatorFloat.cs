using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEF RID: 3567
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the value of a float parameter")]
	public class GetAnimatorFloat : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x06006702 RID: 26370 RVA: 0x002090CC File Offset: 0x002072CC
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006703 RID: 26371 RVA: 0x002090D4 File Offset: 0x002072D4
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
			this.result = null;
		}

		// Token: 0x06006704 RID: 26372 RVA: 0x002090F1 File Offset: 0x002072F1
		public override void OnEnter()
		{
			this.GetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006705 RID: 26373 RVA: 0x00209107 File Offset: 0x00207307
		public override void OnActionUpdate()
		{
			this.GetParameter();
		}

		// Token: 0x06006706 RID: 26374 RVA: 0x00209110 File Offset: 0x00207310
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
			this.result.Value = this.animator.GetFloat(this.paramID);
		}

		// Token: 0x04006657 RID: 26199
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006658 RID: 26200
		[RequiredField]
		[UIHint(UIHint.AnimatorFloat)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x04006659 RID: 26201
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float value of the animator parameter")]
		public FsmFloat result;

		// Token: 0x0400665A RID: 26202
		private string cachedParameter;

		// Token: 0x0400665B RID: 26203
		private int paramID;
	}
}
