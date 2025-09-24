using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEA RID: 3562
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the current transition information on a specified layer. Only valid when during a transition.")]
	public class GetAnimatorCurrentTransitionInfo : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x060066E7 RID: 26343 RVA: 0x00208CBF File Offset: 0x00206EBF
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066E8 RID: 26344 RVA: 0x00208CC7 File Offset: 0x00206EC7
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameHash = null;
			this.userNameHash = null;
			this.normalizedTime = null;
			this.everyFrame = false;
		}

		// Token: 0x060066E9 RID: 26345 RVA: 0x00208D00 File Offset: 0x00206F00
		public override void OnEnter()
		{
			this.GetTransitionInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066EA RID: 26346 RVA: 0x00208D16 File Offset: 0x00206F16
		public override void OnActionUpdate()
		{
			this.GetTransitionInfo();
		}

		// Token: 0x060066EB RID: 26347 RVA: 0x00208D20 File Offset: 0x00206F20
		private void GetTransitionInfo()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			AnimatorTransitionInfo animatorTransitionInfo = this.animator.GetAnimatorTransitionInfo(this.layerIndex.Value);
			if (!this.name.IsNone)
			{
				this.name.Value = this.animator.GetLayerName(this.layerIndex.Value);
			}
			if (!this.nameHash.IsNone)
			{
				this.nameHash.Value = animatorTransitionInfo.nameHash;
			}
			if (!this.userNameHash.IsNone)
			{
				this.userNameHash.Value = animatorTransitionInfo.userNameHash;
			}
			if (!this.normalizedTime.IsNone)
			{
				this.normalizedTime.Value = animatorTransitionInfo.normalizedTime;
			}
		}

		// Token: 0x04006640 RID: 26176
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006641 RID: 26177
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x04006642 RID: 26178
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The unique name of the Transition")]
		public FsmString name;

		// Token: 0x04006643 RID: 26179
		[UIHint(UIHint.Variable)]
		[Tooltip("The unique name of the Transition")]
		public FsmInt nameHash;

		// Token: 0x04006644 RID: 26180
		[UIHint(UIHint.Variable)]
		[Tooltip("The user-specified name of the Transition")]
		public FsmInt userNameHash;

		// Token: 0x04006645 RID: 26181
		[UIHint(UIHint.Variable)]
		[Tooltip("Normalized time of the Transition")]
		public FsmFloat normalizedTime;
	}
}
