using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF3 RID: 3571
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the value of an int parameter.")]
	public class GetAnimatorInt : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x06006717 RID: 26391 RVA: 0x0020944C File Offset: 0x0020764C
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006718 RID: 26392 RVA: 0x00209454 File Offset: 0x00207654
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
			this.result = null;
		}

		// Token: 0x06006719 RID: 26393 RVA: 0x00209471 File Offset: 0x00207671
		public override void OnEnter()
		{
			this.GetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600671A RID: 26394 RVA: 0x00209487 File Offset: 0x00207687
		public override void OnActionUpdate()
		{
			this.GetParameter();
		}

		// Token: 0x0600671B RID: 26395 RVA: 0x00209490 File Offset: 0x00207690
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
			this.result.Value = this.animator.GetInteger(this.paramID);
		}

		// Token: 0x0400666A RID: 26218
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400666B RID: 26219
		[RequiredField]
		[UIHint(UIHint.AnimatorInt)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x0400666C RID: 26220
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The int value of the animator parameter")]
		public FsmInt result;

		// Token: 0x0400666D RID: 26221
		private string cachedParameter;

		// Token: 0x0400666E RID: 26222
		private int paramID;
	}
}
