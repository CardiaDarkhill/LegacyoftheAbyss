using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFA RID: 3578
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns if additional layers affects the mass center")]
	public class GetAnimatorLayersAffectMassCenter : ComponentAction<Animator>
	{
		// Token: 0x06006735 RID: 26421 RVA: 0x0020988D File Offset: 0x00207A8D
		public override void Reset()
		{
			this.gameObject = null;
			this.affectMassCenter = null;
			this.affectMassCenterEvent = null;
			this.doNotAffectMassCenterEvent = null;
		}

		// Token: 0x06006736 RID: 26422 RVA: 0x002098AC File Offset: 0x00207AAC
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				bool layersAffectMassCenter = this.cachedComponent.layersAffectMassCenter;
				this.affectMassCenter.Value = layersAffectMassCenter;
				base.Fsm.Event(layersAffectMassCenter ? this.affectMassCenterEvent : this.doNotAffectMassCenterEvent);
			}
			base.Finish();
		}

		// Token: 0x04006686 RID: 26246
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006687 RID: 26247
		[ActionSection("Results")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("If true, additional layers affects the mass center")]
		public FsmBool affectMassCenter;

		// Token: 0x04006688 RID: 26248
		[Tooltip("Event send if additional layers affects the mass center")]
		public FsmEvent affectMassCenterEvent;

		// Token: 0x04006689 RID: 26249
		[Tooltip("Event send if additional layers do no affects the mass center")]
		public FsmEvent doNotAffectMassCenterEvent;
	}
}
