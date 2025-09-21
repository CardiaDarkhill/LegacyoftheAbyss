using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E0B RID: 3595
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the value of a float parameter")]
	public class SetAnimatorFloat : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x0600678C RID: 26508 RVA: 0x0020A5F4 File Offset: 0x002087F4
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x0600678D RID: 26509 RVA: 0x0020A5FC File Offset: 0x002087FC
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.parameter = null;
			this.dampTime = new FsmFloat
			{
				UseVariable = true
			};
			this.Value = null;
		}

		// Token: 0x0600678E RID: 26510 RVA: 0x0020A62B File Offset: 0x0020882B
		public override void OnEnter()
		{
			this.SetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600678F RID: 26511 RVA: 0x0020A641 File Offset: 0x00208841
		public override void OnActionUpdate()
		{
			this.SetParameter();
		}

		// Token: 0x06006790 RID: 26512 RVA: 0x0020A64C File Offset: 0x0020884C
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
			if (this.dampTime.Value > 0f)
			{
				this.animator.SetFloat(this.paramID, this.Value.Value, this.dampTime.Value, Time.deltaTime);
				return;
			}
			this.animator.SetFloat(this.paramID, this.Value.Value);
		}

		// Token: 0x040066CD RID: 26317
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066CE RID: 26318
		[RequiredField]
		[UIHint(UIHint.AnimatorFloat)]
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x040066CF RID: 26319
		[Tooltip("The float value to assign to the animator parameter")]
		public FsmFloat Value;

		// Token: 0x040066D0 RID: 26320
		[Tooltip("Optional: The time allowed to parameter to reach the value. Requires Every Frame to be checked.")]
		public FsmFloat dampTime;

		// Token: 0x040066D1 RID: 26321
		private string cachedParameter;

		// Token: 0x040066D2 RID: 26322
		private int paramID;
	}
}
