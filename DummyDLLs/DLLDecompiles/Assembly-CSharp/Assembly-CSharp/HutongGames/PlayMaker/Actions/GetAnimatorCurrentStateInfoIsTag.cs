using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE9 RID: 3561
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Does tag match the tag of the active state in the state machine")]
	public class GetAnimatorCurrentStateInfoIsTag : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x060066E1 RID: 26337 RVA: 0x00208BC3 File Offset: 0x00206DC3
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066E2 RID: 26338 RVA: 0x00208BCB File Offset: 0x00206DCB
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.tag = null;
			this.tagMatch = null;
			this.tagMatchEvent = null;
			this.tagDoNotMatchEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x060066E3 RID: 26339 RVA: 0x00208C04 File Offset: 0x00206E04
		public override void OnEnter()
		{
			this.IsTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066E4 RID: 26340 RVA: 0x00208C1A File Offset: 0x00206E1A
		public override void OnActionUpdate()
		{
			this.IsTag();
		}

		// Token: 0x060066E5 RID: 26341 RVA: 0x00208C24 File Offset: 0x00206E24
		private void IsTag()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value).IsTag(this.tag.Value))
			{
				this.tagMatch.Value = true;
				base.Fsm.Event(this.tagMatchEvent);
				return;
			}
			this.tagMatch.Value = false;
			base.Fsm.Event(this.tagDoNotMatchEvent);
		}

		// Token: 0x0400663A RID: 26170
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400663B RID: 26171
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x0400663C RID: 26172
		[Tooltip("The tag to check the layer against.")]
		public FsmString tag;

		// Token: 0x0400663D RID: 26173
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if tag matches")]
		public FsmBool tagMatch;

		// Token: 0x0400663E RID: 26174
		[Tooltip("Event send if tag matches")]
		public FsmEvent tagMatchEvent;

		// Token: 0x0400663F RID: 26175
		[Tooltip("Event send if tag matches")]
		public FsmEvent tagDoNotMatchEvent;
	}
}
