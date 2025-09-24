using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE8 RID: 3560
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Check the current State name on a specified layer, this is more than the layer name, it holds the current state as well.")]
	public class GetAnimatorCurrentStateInfoIsName : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x060066DB RID: 26331 RVA: 0x00208AC5 File Offset: 0x00206CC5
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066DC RID: 26332 RVA: 0x00208ACD File Offset: 0x00206CCD
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameMatchEvent = null;
			this.nameDoNotMatchEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x060066DD RID: 26333 RVA: 0x00208AFF File Offset: 0x00206CFF
		public override void OnEnter()
		{
			this.IsName();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066DE RID: 26334 RVA: 0x00208B15 File Offset: 0x00206D15
		public override void OnActionUpdate()
		{
			this.IsName();
		}

		// Token: 0x060066DF RID: 26335 RVA: 0x00208B20 File Offset: 0x00206D20
		private void IsName()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
			if (!this.isMatching.IsNone)
			{
				this.isMatching.Value = currentAnimatorStateInfo.IsName(this.name.Value);
			}
			base.Fsm.Event(currentAnimatorStateInfo.IsName(this.name.Value) ? this.nameMatchEvent : this.nameDoNotMatchEvent);
		}

		// Token: 0x04006634 RID: 26164
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006635 RID: 26165
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x04006636 RID: 26166
		[Tooltip("The name to check the layer against.")]
		public FsmString name;

		// Token: 0x04006637 RID: 26167
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("True if name matches")]
		public FsmBool isMatching;

		// Token: 0x04006638 RID: 26168
		[Tooltip("Event send if name matches")]
		public FsmEvent nameMatchEvent;

		// Token: 0x04006639 RID: 26169
		[Tooltip("Event send if name doesn't match")]
		public FsmEvent nameDoNotMatchEvent;
	}
}
