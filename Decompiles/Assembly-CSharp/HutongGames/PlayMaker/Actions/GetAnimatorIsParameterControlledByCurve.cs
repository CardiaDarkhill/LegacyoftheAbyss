using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF7 RID: 3575
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns true if a parameter is controlled by an additional curve on an animation")]
	public class GetAnimatorIsParameterControlledByCurve : ComponentAction<Animator>
	{
		// Token: 0x0600672C RID: 26412 RVA: 0x00209735 File Offset: 0x00207935
		public override void Reset()
		{
			this.gameObject = null;
			this.parameterName = null;
			this.isControlledByCurve = null;
			this.isControlledByCurveEvent = null;
			this.isNotControlledByCurveEvent = null;
		}

		// Token: 0x0600672D RID: 26413 RVA: 0x0020975C File Offset: 0x0020795C
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				bool flag = this.cachedComponent.IsParameterControlledByCurve(this.parameterName.Value);
				this.isControlledByCurve.Value = flag;
				base.Fsm.Event(flag ? this.isControlledByCurveEvent : this.isNotControlledByCurveEvent);
			}
			base.Finish();
		}

		// Token: 0x0400667C RID: 26236
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400667D RID: 26237
		[Tooltip("The parameter's name")]
		public FsmString parameterName;

		// Token: 0x0400667E RID: 26238
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if controlled by curve")]
		public FsmBool isControlledByCurve;

		// Token: 0x0400667F RID: 26239
		[Tooltip("Event send if controlled by curve")]
		public FsmEvent isControlledByCurveEvent;

		// Token: 0x04006680 RID: 26240
		[Tooltip("Event send if not controlled by curve")]
		public FsmEvent isNotControlledByCurveEvent;
	}
}
